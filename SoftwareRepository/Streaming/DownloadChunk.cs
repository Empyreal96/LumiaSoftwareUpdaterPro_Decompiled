// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.DownloadChunk
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareRepository.Streaming
{
  internal class DownloadChunk
  {
    private const int DefaultTimeoutInMilliseconds = 10000;
    internal const int BufferSize = 4096;
    internal long BytesFrom;
    internal long Bytes;
    internal long BytesRead;
    internal double DownloadSpeed;
    internal CancellationToken CancellationToken;
    internal Stream OutStream;
    internal int TimeoutInMilliseconds;
    internal SemaphoreSlim SyncLock;
    internal int ChunkIndex;
    internal bool AllowSeek = true;
    internal bool AllowWindowsAuth;

    internal DownloadChunk(
      string filename,
      string url,
      long bytesFrom,
      long byteCount,
      CancellationToken cancellationToken)
    {
      this.FileName = filename;
      this.Url = url;
      this.BytesFrom = bytesFrom;
      this.Bytes = byteCount;
      this.CancellationToken = cancellationToken;
      this.TimeoutInMilliseconds = 10000;
    }

    internal string FileName { get; set; }

    internal string Url { get; set; }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Unused for now but might prove useful later.")]
    internal long FileSize { get; set; }

    internal IWebProxy SoftwareRepositoryProxy { get; set; }

    internal event DownloadProgressEventHandler DownloadProgress;

    private void OnDownloadProgress(EventArgs e)
    {
      if (this.DownloadProgress == null)
        return;
      this.DownloadProgress((object) this, e);
    }

    internal DownloadChunk Clone() => new DownloadChunk(this.FileName, this.Url, this.BytesFrom, this.Bytes, this.CancellationToken)
    {
      FileSize = this.FileSize,
      OutStream = this.OutStream,
      SoftwareRepositoryProxy = this.SoftwareRepositoryProxy,
      TimeoutInMilliseconds = this.TimeoutInMilliseconds,
      SyncLock = this.SyncLock,
      ChunkIndex = this.ChunkIndex,
      AllowSeek = this.AllowSeek,
      AllowWindowsAuth = this.AllowWindowsAuth
    };

    internal async Task<int> Download()
    {
      if (this.Bytes == 0L)
        return 200;
      int ret = -1;
      DateTime downloadStartTime = DateTime.UtcNow;
      HttpClientHandler httpClientHandler = new HttpClientHandler()
      {
        UseDefaultCredentials = this.AllowWindowsAuth
      };
      if (this.SoftwareRepositoryProxy != null)
      {
        httpClientHandler.Proxy = this.SoftwareRepositoryProxy;
        httpClientHandler.UseProxy = true;
      }
      HttpClient httpClient = new HttpClient((HttpMessageHandler) httpClientHandler);
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(this.Url));
      long rangeStart = this.BytesFrom + this.BytesRead;
      long rangeEnd = this.BytesFrom + this.Bytes - 1L;
      request.Headers.Range = new RangeHeaderValue(new long?(rangeStart), new long?(rangeEnd));
      HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, this.CancellationToken);
      HttpStatusCode httpStatusCode = httpResponseMessage.StatusCode;
      if (httpStatusCode == HttpStatusCode.OK || httpStatusCode == HttpStatusCode.PartialContent)
      {
        HttpContentHeaders headers = httpResponseMessage.Content.Headers;
        if (!headers.ContentLength.HasValue)
          SoftwareRepository.Diagnostics.Log(SoftwareRepository.LogLevel.Warning, "Missing Content-Length header");
        else if (headers.ContentLength.Value != rangeEnd - rangeStart + 1L)
          throw new DownloadException(0, "Content-Length does not match request range");
        if (headers.ContentRange == null)
        {
          SoftwareRepository.Diagnostics.Log(SoftwareRepository.LogLevel.Warning, "Missing Content-Range header");
        }
        else
        {
          if (!(headers.ContentRange.Unit.ToLowerInvariant() != "bytes"))
          {
            long? from = headers.ContentRange.From;
            long num1 = rangeStart;
            if ((from.GetValueOrDefault() == num1 ? (!from.HasValue ? 1 : 0) : 1) == 0)
            {
              long? to = headers.ContentRange.To;
              long num2 = rangeEnd;
              if ((to.GetValueOrDefault() == num2 ? (!to.HasValue ? 1 : 0) : 1) == 0)
                goto label_16;
            }
          }
          throw new DownloadException(0, "Content-Range does not match request range");
        }
label_16:
        using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
        {
          byte[] buffer = new byte[4096];
          int bytesRead = 0;
          do
          {
            int num = await DownloadChunk.WithTimeout<int>(stream.ReadAsync(buffer, 0, buffer.Length, this.CancellationToken), this.TimeoutInMilliseconds);
            if ((bytesRead = num) != 0)
            {
              this.CancellationToken.ThrowIfCancellationRequested();
              bytesRead = (int) Math.Min(this.Bytes - this.BytesRead, (long) bytesRead);
              if (this.SyncLock != null)
                await this.SyncLock.WaitAsync();
              try
              {
                if (this.AllowSeek)
                  this.OutStream.Seek(this.BytesFrom + this.BytesRead, SeekOrigin.Begin);
                await this.OutStream.WriteAsync(buffer, 0, bytesRead, this.CancellationToken);
                this.BytesRead += (long) bytesRead;
                if (this.BytesRead >= this.Bytes)
                  await this.OutStream.FlushAsync();
              }
              catch (OperationCanceledException ex)
              {
                httpClient.Dispose();
                throw ex;
              }
              catch (Exception ex)
              {
                httpClient.Dispose();
                throw new DownloadException(508, "Download interrupted because disk full, out of memory or other local storage reason prevents storing file locally.");
              }
              finally
              {
                if (this.SyncLock != null)
                  this.SyncLock.Release();
              }
              this.DownloadSpeed = (double) this.BytesRead / (DateTime.UtcNow - downloadStartTime).TotalSeconds;
              this.OnDownloadProgress((EventArgs) null);
            }
            else
              goto label_36;
          }
          while (this.BytesRead < this.Bytes);
          httpClient.Dispose();
          return (int) httpStatusCode;
label_36:
          buffer = (byte[]) null;
        }
        return ret;
      }
      httpClient.Dispose();
      throw new DownloadException((int) httpStatusCode, "HTTP Response status code: " + (object) (int) httpStatusCode);
    }

    private static async Task<T> WithTimeout<T>(Task<T> task, int time)
    {
      Task delayTask = Task.Delay(time);
      if (await Task.WhenAny((Task) task, delayTask) == delayTask)
        throw new DownloadException(408, "Download request or streaming timeout.");
      return await task;
    }
  }
}
