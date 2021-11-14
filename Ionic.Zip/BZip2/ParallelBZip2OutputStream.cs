// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.ParallelBZip2OutputStream
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ionic.BZip2
{
  public class ParallelBZip2OutputStream : Stream
  {
    private static readonly int BufferPairsPerCore = 4;
    private int _maxWorkers;
    private bool firstWriteDone;
    private int lastFilled;
    private int lastWritten;
    private int latestCompressed;
    private int currentlyFilling;
    private volatile Exception pendingException;
    private bool handlingException;
    private bool emitting;
    private Queue<int> toWrite;
    private Queue<int> toFill;
    private List<WorkItem> pool;
    private object latestLock = new object();
    private object eLock = new object();
    private object outputLock = new object();
    private AutoResetEvent newlyCompressedBlob;
    private long totalBytesWrittenIn;
    private long totalBytesWrittenOut;
    private bool leaveOpen;
    private uint combinedCRC;
    private Stream output;
    private BitWriter bw;
    private int blockSize100k;
    private ParallelBZip2OutputStream.TraceBits desiredTrace = ParallelBZip2OutputStream.TraceBits.Crc | ParallelBZip2OutputStream.TraceBits.Write;

    public ParallelBZip2OutputStream(Stream output)
      : this(output, Ionic.BZip2.BZip2.MaxBlockSize, false)
    {
    }

    public ParallelBZip2OutputStream(Stream output, int blockSize)
      : this(output, blockSize, false)
    {
    }

    public ParallelBZip2OutputStream(Stream output, bool leaveOpen)
      : this(output, Ionic.BZip2.BZip2.MaxBlockSize, leaveOpen)
    {
    }

    public ParallelBZip2OutputStream(Stream output, int blockSize, bool leaveOpen)
    {
      if (blockSize < Ionic.BZip2.BZip2.MinBlockSize || blockSize > Ionic.BZip2.BZip2.MaxBlockSize)
        throw new ArgumentException(string.Format("blockSize={0} is out of range; must be between {1} and {2}", (object) blockSize, (object) Ionic.BZip2.BZip2.MinBlockSize, (object) Ionic.BZip2.BZip2.MaxBlockSize), nameof (blockSize));
      this.output = output;
      this.bw = this.output.CanWrite ? new BitWriter(this.output) : throw new ArgumentException("The stream is not writable.", nameof (output));
      this.blockSize100k = blockSize;
      this.leaveOpen = leaveOpen;
      this.combinedCRC = 0U;
      this.MaxWorkers = 16;
      this.EmitHeader();
    }

    private void InitializePoolOfWorkItems()
    {
      this.toWrite = new Queue<int>();
      this.toFill = new Queue<int>();
      this.pool = new List<WorkItem>();
      int num = Math.Min(ParallelBZip2OutputStream.BufferPairsPerCore * Environment.ProcessorCount, this.MaxWorkers);
      for (int ix = 0; ix < num; ++ix)
      {
        this.pool.Add(new WorkItem(ix, this.blockSize100k));
        this.toFill.Enqueue(ix);
      }
      this.newlyCompressedBlob = new AutoResetEvent(false);
      this.currentlyFilling = -1;
      this.lastFilled = -1;
      this.lastWritten = -1;
      this.latestCompressed = -1;
    }

    public int MaxWorkers
    {
      get => this._maxWorkers;
      set => this._maxWorkers = value >= 4 ? value : throw new ArgumentException(nameof (MaxWorkers), "Value must be 4 or greater.");
    }

    public override void Close()
    {
      if (this.pendingException != null)
      {
        this.handlingException = true;
        Exception pendingException = this.pendingException;
        this.pendingException = (Exception) null;
        throw pendingException;
      }
      if (this.handlingException || this.output == null)
        return;
      Stream output = this.output;
      try
      {
        this.FlushOutput(true);
      }
      finally
      {
        this.output = (Stream) null;
        this.bw = (BitWriter) null;
      }
      if (this.leaveOpen)
        return;
      output.Close();
    }

    private void FlushOutput(bool lastInput)
    {
      if (this.emitting)
        return;
      if (this.currentlyFilling >= 0)
      {
        this.CompressOne((object) this.pool[this.currentlyFilling]);
        this.currentlyFilling = -1;
      }
      if (lastInput)
      {
        this.EmitPendingBuffers(true, false);
        this.EmitTrailer();
      }
      else
        this.EmitPendingBuffers(false, false);
    }

    public override void Flush()
    {
      if (this.output == null)
        return;
      this.FlushOutput(false);
      this.bw.Flush();
      this.output.Flush();
    }

    private void EmitHeader()
    {
      byte[] buffer = new byte[4]
      {
        (byte) 66,
        (byte) 90,
        (byte) 104,
        (byte) (48 + this.blockSize100k)
      };
      this.output.Write(buffer, 0, buffer.Length);
    }

    private void EmitTrailer()
    {
      this.bw.WriteByte((byte) 23);
      this.bw.WriteByte((byte) 114);
      this.bw.WriteByte((byte) 69);
      this.bw.WriteByte((byte) 56);
      this.bw.WriteByte((byte) 80);
      this.bw.WriteByte((byte) 144);
      this.bw.WriteInt(this.combinedCRC);
      this.bw.FinishAndPad();
    }

    public int BlockSize => this.blockSize100k;

    public override void Write(byte[] buffer, int offset, int count)
    {
      bool mustWait = false;
      if (this.output == null)
        throw new IOException("the stream is not open");
      if (this.pendingException != null)
      {
        this.handlingException = true;
        Exception pendingException = this.pendingException;
        this.pendingException = (Exception) null;
        throw pendingException;
      }
      if (offset < 0)
        throw new IndexOutOfRangeException(string.Format("offset ({0}) must be > 0", (object) offset));
      if (count < 0)
        throw new IndexOutOfRangeException(string.Format("count ({0}) must be > 0", (object) count));
      if (offset + count > buffer.Length)
        throw new IndexOutOfRangeException(string.Format("offset({0}) count({1}) bLength({2})", (object) offset, (object) count, (object) buffer.Length));
      if (count == 0)
        return;
      if (!this.firstWriteDone)
      {
        this.InitializePoolOfWorkItems();
        this.firstWriteDone = true;
      }
      int num1 = 0;
      int count1 = count;
      do
      {
        this.EmitPendingBuffers(false, mustWait);
        mustWait = false;
        int index;
        if (this.currentlyFilling >= 0)
          index = this.currentlyFilling;
        else if (this.toFill.Count == 0)
        {
          mustWait = true;
          goto label_25;
        }
        else
        {
          index = this.toFill.Dequeue();
          ++this.lastFilled;
        }
        WorkItem workItem = this.pool[index];
        workItem.ordinal = this.lastFilled;
        int num2 = workItem.Compressor.Fill(buffer, offset, count1);
        if (num2 != count1)
        {
          if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompressOne), (object) workItem))
            throw new Exception("Cannot enqueue workitem");
          this.currentlyFilling = -1;
          offset += num2;
        }
        else
          this.currentlyFilling = index;
        count1 -= num2;
        num1 += num2;
label_25:;
      }
      while (count1 > 0);
      this.totalBytesWrittenIn += (long) num1;
    }

    private void EmitPendingBuffers(bool doAll, bool mustWait)
    {
      if (this.emitting)
        return;
      this.emitting = true;
      if (doAll || mustWait)
        this.newlyCompressedBlob.WaitOne();
      do
      {
        int num1 = -1;
        int millisecondsTimeout = doAll ? 200 : (mustWait ? -1 : 0);
        int index1;
        do
        {
          if (Monitor.TryEnter((object) this.toWrite, millisecondsTimeout))
          {
            index1 = -1;
            try
            {
              if (this.toWrite.Count > 0)
                index1 = this.toWrite.Dequeue();
            }
            finally
            {
              Monitor.Exit((object) this.toWrite);
            }
            if (index1 >= 0)
            {
              WorkItem workItem = this.pool[index1];
              if (workItem.ordinal != this.lastWritten + 1)
              {
                lock (this.toWrite)
                  this.toWrite.Enqueue(index1);
                if (num1 == index1)
                {
                  this.newlyCompressedBlob.WaitOne();
                  num1 = -1;
                }
                else if (num1 == -1)
                  num1 = index1;
              }
              else
              {
                num1 = -1;
                BitWriter bw = workItem.bw;
                bw.Flush();
                MemoryStream ms = workItem.ms;
                ms.Seek(0L, SeekOrigin.Begin);
                long num2 = 0;
                byte[] buffer = new byte[1024];
                int num3;
                while ((num3 = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                  for (int index2 = 0; index2 < num3; ++index2)
                    this.bw.WriteByte(buffer[index2]);
                  num2 += (long) num3;
                }
                if (bw.NumRemainingBits > 0)
                  this.bw.WriteBits(bw.NumRemainingBits, (uint) bw.RemainingBits);
                this.combinedCRC = this.combinedCRC << 1 | this.combinedCRC >> 31;
                this.combinedCRC ^= workItem.Compressor.Crc32;
                this.totalBytesWrittenOut += num2;
                bw.Reset();
                this.lastWritten = workItem.ordinal;
                workItem.ordinal = -1;
                this.toFill.Enqueue(workItem.index);
                if (millisecondsTimeout == -1)
                  millisecondsTimeout = 0;
              }
            }
          }
          else
            index1 = -1;
        }
        while (index1 >= 0);
      }
      while (doAll && this.lastWritten != this.latestCompressed);
      int num = doAll ? 1 : 0;
      this.emitting = false;
    }

    private void CompressOne(object wi)
    {
      WorkItem workItem = (WorkItem) wi;
      try
      {
        workItem.Compressor.CompressAndWrite();
        lock (this.latestLock)
        {
          if (workItem.ordinal > this.latestCompressed)
            this.latestCompressed = workItem.ordinal;
        }
        lock (this.toWrite)
          this.toWrite.Enqueue(workItem.index);
        this.newlyCompressedBlob.Set();
      }
      catch (Exception ex)
      {
        lock (this.eLock)
        {
          if (this.pendingException == null)
            return;
          this.pendingException = ex;
        }
      }
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => this.output != null ? this.output.CanWrite : throw new ObjectDisposedException("BZip2Stream");

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => this.totalBytesWrittenIn;
      set => throw new NotImplementedException();
    }

    public long BytesWrittenOut => this.totalBytesWrittenOut;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    [Conditional("Trace")]
    private void TraceOutput(
      ParallelBZip2OutputStream.TraceBits bits,
      string format,
      params object[] varParams)
    {
      if ((bits & this.desiredTrace) == ParallelBZip2OutputStream.TraceBits.None)
        return;
      lock (this.outputLock)
      {
        int hashCode = Thread.CurrentThread.GetHashCode();
        Console.ForegroundColor = (ConsoleColor) (hashCode % 8 + 10);
        Console.Write("{0:000} PBOS ", (object) hashCode);
        Console.WriteLine(format, varParams);
        Console.ResetColor();
      }
    }

    [Flags]
    private enum TraceBits : uint
    {
      None = 0,
      Crc = 1,
      Write = 2,
      All = 4294967295, // 0xFFFFFFFF
    }
  }
}
