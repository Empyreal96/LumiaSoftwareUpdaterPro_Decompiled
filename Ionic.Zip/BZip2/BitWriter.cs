// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.BitWriter
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System.IO;

namespace Ionic.BZip2
{
  internal class BitWriter
  {
    private uint accumulator;
    private int nAccumulatedBits;
    private Stream output;
    private int totalBytesWrittenOut;

    public BitWriter(Stream s) => this.output = s;

    public byte RemainingBits => (byte) (this.accumulator >> 32 - this.nAccumulatedBits & (uint) byte.MaxValue);

    public int NumRemainingBits => this.nAccumulatedBits;

    public int TotalBytesWrittenOut => this.totalBytesWrittenOut;

    public void Reset()
    {
      this.accumulator = 0U;
      this.nAccumulatedBits = 0;
      this.totalBytesWrittenOut = 0;
      this.output.Seek(0L, SeekOrigin.Begin);
      this.output.SetLength(0L);
    }

    public void WriteBits(int nbits, uint value)
    {
      int nAccumulatedBits = this.nAccumulatedBits;
      uint accumulator = this.accumulator;
      for (; nAccumulatedBits >= 8; nAccumulatedBits -= 8)
      {
        this.output.WriteByte((byte) (accumulator >> 24 & (uint) byte.MaxValue));
        ++this.totalBytesWrittenOut;
        accumulator <<= 8;
      }
      this.accumulator = accumulator | value << 32 - nAccumulatedBits - nbits;
      this.nAccumulatedBits = nAccumulatedBits + nbits;
    }

    public void WriteByte(byte b) => this.WriteBits(8, (uint) b);

    public void WriteInt(uint u)
    {
      this.WriteBits(8, u >> 24 & (uint) byte.MaxValue);
      this.WriteBits(8, u >> 16 & (uint) byte.MaxValue);
      this.WriteBits(8, u >> 8 & (uint) byte.MaxValue);
      this.WriteBits(8, u & (uint) byte.MaxValue);
    }

    public void Flush() => this.WriteBits(0, 0U);

    public void FinishAndPad()
    {
      this.Flush();
      if (this.NumRemainingBits <= 0)
        return;
      this.output.WriteByte((byte) (this.accumulator >> 24 & (uint) byte.MaxValue));
      ++this.totalBytesWrittenOut;
    }
  }
}
