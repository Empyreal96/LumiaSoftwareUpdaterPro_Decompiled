// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.ChunkManager
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace SoftwareRepository.Streaming
{
  internal class ChunkManager
  {
    private object StateLock = new object();
    private Streamer Streamer;
    private SemaphoreSlim SyncLock;
    private ChunkState[] ChunkStates;
    private Dictionary<int, long> ResumedPartialProgress;
    private int NextUndownloadedIndex;
    private int DownloadedChunks;
    private CancellationToken ClientCancellationToken;
    internal int? ChunkTimeoutInMilliseconds;
    internal IWebProxy SoftwareRepositoryProxy;
    internal bool AllowWindowsAuth;

    internal int TotalChunks => this.ChunkStates.Length;

    internal bool IsDownloaded => this.DownloadedChunks == this.TotalChunks;

    internal long ProgressBytes
    {
      get
      {
        lock (this.StateLock)
        {
          long num1 = (long) this.DownloadedChunks * this.ChunkSize;
          if (this.FileSize % this.ChunkSize != 0L && this.ChunkStates[this.ChunkStates.Length - 1] == ChunkState.Downlodaded)
            num1 = num1 - this.ChunkSize + this.FileSize % this.ChunkSize;
          foreach (DownloadChunk downloadingChunk in this.DownloadingChunks)
            num1 += downloadingChunk.BytesRead;
          if (this.ResumedPartialProgress != null)
          {
            foreach (long num2 in this.ResumedPartialProgress.Values)
              num1 += num2;
          }
          return num1;
        }
      }
    }

    internal HashSet<DownloadChunk> DownloadingChunks { get; private set; }

    internal long ChunkSize { get; private set; }

    internal long FileSize { get; set; }

    internal string FileName { get; set; }

    internal string FileUrl { get; set; }

    internal ChunkManager(
      long chunkSize,
      long fileSize,
      string filename,
      Streamer streamer,
      SemaphoreSlim syncLock,
      CancellationToken cancellationToken)
      : this((DownloadMetadata) null, chunkSize, fileSize, filename, streamer, syncLock, cancellationToken)
    {
    }

    internal ChunkManager(
      DownloadMetadata metadata,
      long chunkSize,
      long fileSize,
      string filename,
      Streamer streamer,
      SemaphoreSlim syncLock,
      CancellationToken cancellationToken)
    {
      long length = fileSize / chunkSize + (fileSize % chunkSize == 0L ? 0L : 1L);
      if (metadata == null || (long) metadata.ChunkStates.Length != length)
      {
        this.ChunkStates = new ChunkState[length];
      }
      else
      {
        this.ChunkStates = metadata.ChunkStates;
        this.ResumedPartialProgress = metadata.PartialProgress;
      }
      this.NextUndownloadedIndex = 0;
      this.DownloadedChunks = 0;
      for (; this.NextUndownloadedIndex < this.ChunkStates.Length && this.ChunkStates[this.NextUndownloadedIndex] != ChunkState.Undownloaded; ++this.NextUndownloadedIndex)
      {
        if (this.ChunkStates[this.NextUndownloadedIndex] == ChunkState.Downlodaded)
          ++this.DownloadedChunks;
      }
      for (long index = (long) (this.NextUndownloadedIndex + 1); index < (long) this.ChunkStates.Length; ++index)
      {
        if (this.ChunkStates[index] == ChunkState.Downlodaded)
          ++this.DownloadedChunks;
      }
      this.ChunkSize = chunkSize;
      this.DownloadingChunks = new HashSet<DownloadChunk>();
      this.FileName = filename;
      this.FileSize = fileSize;
      this.ClientCancellationToken = cancellationToken;
      this.SyncLock = syncLock;
      this.Streamer = streamer;
    }

    internal DownloadChunk GetNextChunk()
    {
      lock (this.StateLock)
      {
        DownloadChunk downloadChunk;
        if (this.ResumedPartialProgress != null && this.ResumedPartialProgress.Count > 0)
        {
          int num1 = this.ResumedPartialProgress.Keys.First<int>();
          long num2 = this.ResumedPartialProgress[num1];
          long startPosition = (long) num1 * this.ChunkSize + num2;
          long downloadLength = Math.Min(this.ChunkSize - num2, this.FileSize - startPosition);
          downloadChunk = this.MakeChunk(num1, startPosition, downloadLength);
          this.ResumedPartialProgress.Remove(num1);
          if (this.ResumedPartialProgress.Count == 0)
            this.ResumedPartialProgress = (Dictionary<int, long>) null;
        }
        else
        {
          while (this.NextUndownloadedIndex < this.ChunkStates.Length && this.ChunkStates[this.NextUndownloadedIndex] != ChunkState.Undownloaded)
            ++this.NextUndownloadedIndex;
          if (this.NextUndownloadedIndex >= this.ChunkStates.Length)
            return (DownloadChunk) null;
          long startPosition = (long) this.NextUndownloadedIndex * this.ChunkSize;
          long downloadLength = Math.Min(this.ChunkSize, this.FileSize - startPosition);
          downloadChunk = this.MakeChunk(this.NextUndownloadedIndex, startPosition, downloadLength);
          ++this.NextUndownloadedIndex;
        }
        this.ChunkStates[downloadChunk.ChunkIndex] = ChunkState.PartiallyDownloaded;
        this.DownloadingChunks.Add(downloadChunk);
        return downloadChunk;
      }
    }

    internal DownloadChunk GetTestChunk()
    {
      lock (this.StateLock)
      {
        while (this.NextUndownloadedIndex < this.ChunkStates.Length && this.ChunkStates[this.NextUndownloadedIndex] != ChunkState.Undownloaded)
          ++this.NextUndownloadedIndex;
        if (this.NextUndownloadedIndex < this.ChunkStates.Length - 1 || this.NextUndownloadedIndex == this.ChunkStates.Length - 1 && this.FileSize % this.ChunkSize == 0L)
          return this.MakeChunk(this.NextUndownloadedIndex, (long) this.NextUndownloadedIndex * this.ChunkSize, this.ChunkSize);
        int num1 = -1;
        long num2 = -1;
        long num3 = 0;
        if (this.ChunkStates[this.ChunkStates.Length - 1] == ChunkState.Undownloaded)
        {
          num1 = this.ChunkStates.Length - 1;
          num2 = this.FileSize % this.ChunkSize;
        }
        if (this.ResumedPartialProgress != null)
        {
          foreach (int key in this.ResumedPartialProgress.Keys)
          {
            long num4 = (key == this.ChunkStates.Length - 1 ? this.FileSize % this.ChunkSize : this.ChunkSize) - this.ResumedPartialProgress[key];
            if (num4 > num2)
            {
              num1 = key;
              num2 = num4;
              num3 = this.ResumedPartialProgress[key];
            }
          }
        }
        if (num1 < 0 || num2 <= 0L)
          return (DownloadChunk) null;
        int chunkIndex;
        long startPosition = (long) (chunkIndex = num1) * this.ChunkSize + num3;
        long downloadLength = num2;
        return this.MakeChunk(chunkIndex, startPosition, downloadLength);
      }
    }

    internal void MarkDownloaded(DownloadChunk chunk)
    {
      lock (this.StateLock)
      {
        if (this.ChunkStates[chunk.ChunkIndex] != ChunkState.Downlodaded)
        {
          ++this.DownloadedChunks;
          this.ChunkStates[chunk.ChunkIndex] = ChunkState.Downlodaded;
        }
        this.DownloadingChunks.Remove(chunk);
        if (this.ResumedPartialProgress == null || !this.ResumedPartialProgress.ContainsKey(chunk.ChunkIndex))
          return;
        this.ResumedPartialProgress.Remove(chunk.ChunkIndex);
        if (this.ResumedPartialProgress.Count != 0)
          return;
        this.ResumedPartialProgress = (Dictionary<int, long>) null;
      }
    }

    internal DownloadMetadata GetMetadata(bool includeInProgress)
    {
      lock (this.StateLock)
      {
        if (includeInProgress)
          return new DownloadMetadata()
          {
            ChunkStates = this.ChunkStates.Clone() as ChunkState[],
            PartialProgress = this.DownloadingChunks.ToDictionary<DownloadChunk, int, long>((Func<DownloadChunk, int>) (d => d.ChunkIndex), (Func<DownloadChunk, long>) (d => d.BytesRead))
          };
        return new DownloadMetadata()
        {
          ChunkStates = ((IEnumerable<ChunkState>) this.ChunkStates).Select<ChunkState, ChunkState>((Func<ChunkState, ChunkState>) (s => s != ChunkState.Downlodaded ? ChunkState.Undownloaded : ChunkState.Downlodaded)).ToArray<ChunkState>(),
          PartialProgress = (Dictionary<int, long>) null
        };
      }
    }

    internal void SaveMetadata(Streamer target, bool includeInProgress = false)
    {
      byte[] metadata = this.GetMetadata(includeInProgress).Serialize();
      if (metadata != null)
        target.SetMetadata(metadata);
      else
        target.ClearMetadata();
    }

    private DownloadChunk MakeChunk(
      int chunkIndex,
      long startPosition,
      long downloadLength)
    {
      DownloadChunk downloadChunk = new DownloadChunk(this.FileName, this.FileUrl, startPosition, downloadLength, this.ClientCancellationToken)
      {
        SyncLock = this.SyncLock,
        ChunkIndex = chunkIndex,
        OutStream = this.Streamer.GetStream(),
        FileSize = this.FileSize,
        AllowWindowsAuth = this.AllowWindowsAuth
      };
      if (this.ChunkTimeoutInMilliseconds.HasValue)
        downloadChunk.TimeoutInMilliseconds = this.ChunkTimeoutInMilliseconds.Value;
      if (this.SoftwareRepositoryProxy != null)
        downloadChunk.SoftwareRepositoryProxy = this.SoftwareRepositoryProxy;
      return downloadChunk;
    }
  }
}
