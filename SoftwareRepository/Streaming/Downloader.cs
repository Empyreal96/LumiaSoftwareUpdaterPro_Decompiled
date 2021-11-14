// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.Downloader
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using SoftwareRepository.Discovery;
using SoftwareRepository.Reporting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareRepository.Streaming
{
  public class Downloader
  {
    private const int Canceled = 206;
    private const int Completed = 200;
    private long DefaultChunkSize = 1048576;
    private const int DefaultParallelConnections = 4;
    private const string DefaultSoftwareRepositoryBaseUrl = "https://api.swrepository.com";
    private const string DefaultSoftwareRepositoryFileUrl = "/rest-api/discovery/fileurl";
    private const string DefaultSoftwareRepositoryFileUrlApiVersion = "/1";
    private const string DefaultSoftwareRepositoryUserAgent = "SoftwareRepository";
    private const int DefaultTimeoutInMilliseconds = 10000;

    public Downloader()
    {
      this.TimeoutInMilliseconds = 10000;
      this.MaxParallelConnections = 4;
      this.ChunkSize = this.DefaultChunkSize;
      this.AllowWindowsAuth = false;
    }

    public long ChunkSize { get; set; }

    public int MaxParallelConnections { get; set; }

    public string SoftwareRepositoryAlternativeBaseUrl { get; set; }

    public string SoftwareRepositoryAuthenticationToken { get; set; }

    public IWebProxy SoftwareRepositoryProxy { get; set; }

    public string SoftwareRepositoryUserAgent { get; set; }

    public int TimeoutInMilliseconds { get; set; }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Acceptable abbreviation", MessageId = "Auth")]
    public bool AllowWindowsAuth { get; set; }

    public event DownloadReadyEventHandler DownloadReady;

    public event BestUrlSelectionEventHandler OnUrlSelection;

    public async Task GetFileAsync(string packageId, string filename, Streamer streamer) => await this.GetFileAsync(packageId, filename, streamer, CancellationToken.None, (DownloadProgress<DownloadProgressInfo>) null);

    public async Task GetFileAsync(
      string packageId,
      string filename,
      Streamer streamer,
      DownloadProgress<DownloadProgressInfo> progress)
    {
      await this.GetFileAsync(packageId, filename, streamer, CancellationToken.None, progress);
    }

    public async Task GetFileAsync(
      string packageId,
      string filename,
      Streamer streamer,
      CancellationToken cancellationToken)
    {
      await this.GetFileAsync(packageId, filename, streamer, cancellationToken, (DownloadProgress<DownloadProgressInfo>) null);
    }

    public async Task GetFileAsync(
      string packageId,
      string filename,
      Streamer streamer,
      CancellationToken cancellationToken,
      DownloadProgress<DownloadProgressInfo> progress)
    {
      Exception exception = (Exception) null;
      int reportStatus = -1;
      DateTime reportTimeBegin = DateTime.Now;
      FileUrlResult fileUrlResult = (FileUrlResult) null;
      ChunkManager chunkManager = (ChunkManager) null;
      SemaphoreSlim syncLock = new SemaphoreSlim(1, 1);
      Downloader.ProgressWrapper progressWrapper = new Downloader.ProgressWrapper(progress);
      UrlSelectionResult urlSelectionResult = new UrlSelectionResult();
      try
      {
        try
        {
          fileUrlResult = await this.GetFileUrlAsync(packageId, filename, cancellationToken);
          List<string> fileUrls = fileUrlResult.GetFileUrls();
          if (fileUrlResult.StatusCode != HttpStatusCode.OK || fileUrls.Count <= 0)
            throw new DownloadException(404, "File not found (" + filename + ").");
          long streamSize = streamer.GetStream().Length;
          if (streamSize > fileUrlResult.FileSize)
            throw new DownloadException(412, "Incorrect file size, can't resume download. Stream contains more data than expected.");
          bool flag = streamSize == fileUrlResult.FileSize;
          if (flag)
            flag = await Downloader.FileIntegrityPreservedAsync(fileUrlResult.Checksum, streamer.GetStream());
          if (flag)
          {
            this.ReportCompleted(progressWrapper, packageId, filename, fileUrlResult.FileSize);
            return;
          }
          byte[] data = streamer.GetMetadata();
          if (data != null)
          {
            chunkManager = new ChunkManager(DownloadMetadata.Deserialize(data), this.ChunkSize, fileUrlResult.FileSize, filename, streamer, syncLock, cancellationToken);
            if (streamSize < chunkManager.ProgressBytes)
            {
              data = (byte[]) null;
              streamer.ClearMetadata();
            }
          }
          if (data == null)
            chunkManager = new ChunkManager(this.ChunkSize, fileUrlResult.FileSize, filename, streamer, syncLock, cancellationToken);
          chunkManager.SoftwareRepositoryProxy = this.SoftwareRepositoryProxy;
          chunkManager.ChunkTimeoutInMilliseconds = new int?(this.TimeoutInMilliseconds);
          chunkManager.AllowWindowsAuth = this.AllowWindowsAuth;
          this.OnUrlSelection += (BestUrlSelectionEventHandler) (result => urlSelectionResult = result);
          string str = await this.SelectBestUrlAsync(chunkManager.GetTestChunk(), fileUrlResult, streamer, cancellationToken, progressWrapper, chunkManager);
          chunkManager.FileUrl = str;
          if (chunkManager.IsDownloaded)
            reportStatus = 200;
          else
            reportStatus = await this.DownloadAsync(chunkManager, fileUrlResult, streamer, progressWrapper);
        }
        catch (OperationCanceledException ex)
        {
          reportStatus = 206;
          exception = (Exception) ex;
        }
        catch (HttpRequestException ex)
        {
          reportStatus = 999;
          exception = (Exception) new DownloadException(reportStatus, "HttpRequestException.", (Exception) ex);
        }
        catch (DownloadException ex)
        {
          reportStatus = ex.StatusCode;
          exception = (Exception) ex;
        }
        if (chunkManager != null)
        {
          if (chunkManager.IsDownloaded)
            streamer.ClearMetadata();
          else
            chunkManager.SaveMetadata(streamer, true);
        }
        await streamer.GetStream().FlushAsync();
      }
      catch (Exception ex)
      {
        if (reportStatus == -1)
          reportStatus = 999;
        exception = (Exception) new DownloadException(999, "Unknown exception.", ex);
      }
      if (exception == null)
      {
        if (!await Downloader.FileIntegrityPreservedAsync(fileUrlResult.Checksum, streamer.GetStream()))
        {
          reportStatus = 417;
          exception = (Exception) new DownloadException(reportStatus, "File integrity error. MD5 checksum of the file does not match with data received from server.");
        }
        else
          reportStatus = 200;
      }
      try
      {
        long time = (long) (int) (DateTime.Now - reportTimeBegin).TotalSeconds;
        if (time <= 0L)
          time = 1L;
        await new Reporter()
        {
          SoftwareRepositoryAlternativeBaseUrl = this.SoftwareRepositoryAlternativeBaseUrl,
          SoftwareRepositoryUserAgent = this.SoftwareRepositoryUserAgent,
          SoftwareRepositoryProxy = this.SoftwareRepositoryProxy
        }.SendDownloadReport(packageId, filename, new List<string>()
        {
          urlSelectionResult.ToJson()
        }, reportStatus, time, fileUrlResult.FileSize, Math.Min(this.MaxParallelConnections, chunkManager.TotalChunks), cancellationToken);
      }
      catch (Exception ex)
      {
      }
      if (exception != null)
        throw exception;
      this.ReportCompleted(progressWrapper, packageId, filename, fileUrlResult.FileSize);
    }

    private static async Task<bool> FileIntegrityPreservedAsync(
      List<SoftwareFileChecksum> checksums,
      Stream stream)
    {
      if (checksums == null)
        return false;
      bool ret = false;
      foreach (SoftwareFileChecksum checksum1 in checksums)
      {
        SoftwareFileChecksum checksum = checksum1;
        if (checksum != null && checksum.ChecksumType != null && checksum.Value != null)
        {
          if (checksum.ChecksumType.ToUpperInvariant() == "MD5")
          {
            using (MD5 md5 = MD5.Create())
            {
              stream.Seek(0L, SeekOrigin.Begin);
              byte[] inArray = await Task.Run<byte[]>((Func<byte[]>) (() => md5.ComputeHash(stream)));
              string base64String = Convert.ToBase64String(inArray);
              string lowerInvariant = BitConverter.ToString(inArray).Replace("-", string.Empty).ToLowerInvariant();
              if (!checksum.Value.Equals(base64String) && !checksum.Value.ToLowerInvariant().Equals(lowerInvariant))
                return false;
              ret = true;
            }
          }
          checksum = (SoftwareFileChecksum) null;
        }
      }
      return ret;
    }

    private async Task<FileUrlResult> GetFileUrlAsync(
      string packageId,
      string filename,
      CancellationToken cancellationToken)
    {
      FileUrlResult ret = new FileUrlResult();
      try
      {
        string str = "https://api.swrepository.com";
        if (this.SoftwareRepositoryAlternativeBaseUrl != null)
          str = this.SoftwareRepositoryAlternativeBaseUrl;
        Uri requestUri = new Uri(str + "/rest-api/discovery/fileurl/1/" + packageId + "/" + filename);
        HttpClient httpClient = (HttpClient) null;
        if (this.SoftwareRepositoryProxy != null)
          httpClient = new HttpClient((HttpMessageHandler) new HttpClientHandler()
          {
            Proxy = this.SoftwareRepositoryProxy,
            UseProxy = true
          });
        else
          httpClient = new HttpClient();
        string input = "SoftwareRepository";
        if (this.SoftwareRepositoryUserAgent != null)
          input = this.SoftwareRepositoryUserAgent;
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(input);
        if (this.SoftwareRepositoryAuthenticationToken != null)
        {
          httpClient.DefaultRequestHeaders.Add("X-Authentication", this.SoftwareRepositoryAuthenticationToken);
          httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.SoftwareRepositoryAuthenticationToken);
        }
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpResponseMessage responseMsg = await httpClient.GetAsync(requestUri, cancellationToken);
        if (responseMsg.StatusCode == HttpStatusCode.OK)
          ret = (FileUrlResult) new DataContractJsonSerializer(typeof (FileUrlResult)).ReadObject((Stream) new MemoryStream(Encoding.UTF8.GetBytes(await responseMsg.Content.ReadAsStringAsync())));
        ret.StatusCode = responseMsg.StatusCode;
        httpClient.Dispose();
        httpClient = (HttpClient) null;
        responseMsg = (HttpResponseMessage) null;
      }
      catch (OperationCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DownloadException(999, "Cannot get file url.", ex);
      }
      return ret;
    }

    private async Task<string> SelectBestUrlAsync(
      DownloadChunk testChunk,
      FileUrlResult fileUrlResult,
      Streamer streamer,
      CancellationToken cancellationToken,
      Downloader.ProgressWrapper progress,
      ChunkManager chunkManager)
    {
      UrlSelectionResult urlSelectionResult = new UrlSelectionResult();
      urlSelectionResult.TestBytes = testChunk.Bytes;
      List<string> fileUrls = fileUrlResult.GetFileUrls();
      if (fileUrls.Count == 1)
      {
        urlSelectionResult.UrlResults.Add(new UrlResult()
        {
          FileUrl = fileUrls.First<string>(),
          IsSelected = true
        });
        if (this.OnUrlSelection != null)
          this.OnUrlSelection(urlSelectionResult);
        return fileUrls.First<string>();
      }
      urlSelectionResult.UrlResults.AddRange(fileUrlResult.GetFileUrls().Select<string, UrlResult>((Func<string, UrlResult>) (x => new UrlResult()
      {
        FileUrl = x
      })));
      string ret = (string) null;
      Exception exception = (Exception) null;
      Task<int> winner = (Task<int>) null;
      int statusCode = -1;
      List<DownloadChunk> chunks = new List<DownloadChunk>();
      List<Task<int>> tasks = new List<Task<int>>();
      long currentDownloaded = chunkManager.ProgressBytes;
      long bestChunk = 0;
      DownloadProgressEventHandler progressHandler = (DownloadProgressEventHandler) ((sender, args) =>
      {
        DownloadChunk chunk = sender as DownloadChunk;
        urlSelectionResult.UrlResults.Where<UrlResult>((Func<UrlResult, bool>) (x => x.FileUrl.Equals(chunk.Url, StringComparison.OrdinalIgnoreCase))).ToList<UrlResult>().ForEach((Action<UrlResult>) (x =>
        {
          x.TestSpeed = chunk.DownloadSpeed;
          x.BytesRead = chunk.BytesRead;
        }));
        if (chunk.BytesRead <= bestChunk)
          return;
        bestChunk = chunk.BytesRead;
        progress.Report(new DownloadProgressInfo(currentDownloaded + bestChunk, chunkManager.FileSize, chunkManager.FileName));
      });
      using (CancellationTokenSource cts = new CancellationTokenSource())
      {
        CancellationTokenRegistration tokenRegistration = cancellationToken.Register((Action) (() => cts.Cancel()));
        try
        {
          foreach (string str in fileUrls)
          {
            DownloadChunk downloadChunk = testChunk.Clone();
            downloadChunk.Url = str;
            downloadChunk.OutStream = (Stream) new MemoryStream();
            downloadChunk.AllowSeek = false;
            downloadChunk.DownloadProgress += progressHandler;
            chunks.Add(downloadChunk);
            tasks.Add(downloadChunk.Download());
          }
          while (statusCode == -1)
          {
            if (tasks.Count > 0)
            {
              try
              {
                winner = await Task.WhenAny<int>((IEnumerable<Task<int>>) tasks);
                statusCode = await winner;
              }
              catch (OperationCanceledException ex)
              {
                exception = (Exception) ex;
              }
              catch (Exception ex)
              {
                if (!(exception is OperationCanceledException))
                  exception = ex;
              }
              finally
              {
                if (statusCode == -1)
                {
                  int index = tasks.IndexOf(winner);
                  tasks.Remove(winner);
                  fileUrls.RemoveAt(index);
                  DownloadChunk chunk = chunks[index];
                  Func<Exception, string> walkException = (Func<Exception, string>) null;
                  walkException = (Func<Exception, string>) (e => e.InnerException != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1} ({2})", (object) e.GetType().Name, (object) e.Message, (object) walkException(e.InnerException)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", (object) e.GetType().Name, (object) e.Message));
                  urlSelectionResult.UrlResults.Where<UrlResult>((Func<UrlResult, bool>) (x => x.FileUrl.Equals(chunk.Url, StringComparison.OrdinalIgnoreCase))).ToList<UrlResult>().ForEach((Action<UrlResult>) (x => x.Error = walkException(exception)));
                  chunk.OutStream.Dispose();
                  chunk.DownloadProgress -= progressHandler;
                  chunks.RemoveAt(index);
                }
              }
            }
            else
              break;
          }
        }
        finally
        {
          tokenRegistration.Dispose();
        }
        tokenRegistration = new CancellationTokenRegistration();
        if (!cts.IsCancellationRequested)
          cts.Cancel();
      }
      if (statusCode != -1)
      {
        int index = tasks.IndexOf(winner);
        ret = fileUrls[index];
        urlSelectionResult.UrlResults.Where<UrlResult>((Func<UrlResult, bool>) (x => x.FileUrl.Equals(ret, StringComparison.OrdinalIgnoreCase))).ToList<UrlResult>().ForEach((Action<UrlResult>) (x => x.IsSelected = true));
        DownloadChunk chunk = chunks[index];
        chunk.OutStream.Seek(0L, SeekOrigin.Begin);
        streamer.GetStream().Seek(chunk.BytesFrom, SeekOrigin.Begin);
        await chunk.OutStream.CopyToAsync(streamer.GetStream());
        await streamer.GetStream().FlushAsync();
        chunkManager.MarkDownloaded(chunk);
        chunkManager.SaveMetadata(streamer);
        chunk = (DownloadChunk) null;
      }
      foreach (DownloadChunk downloadChunk in chunks)
      {
        downloadChunk.OutStream.Dispose();
        downloadChunk.DownloadProgress -= progressHandler;
      }
      if (statusCode == -1 && exception != null)
        throw exception;
      if (this.OnUrlSelection != null)
        this.OnUrlSelection(urlSelectionResult);
      return ret;
    }

    private async Task<int> DownloadAsync(
      ChunkManager chunkManager,
      FileUrlResult fileUrlResult,
      Streamer streamer,
      Downloader.ProgressWrapper progress)
    {
      int parallelConnections = this.MaxParallelConnections;
      DownloadProgressEventHandler progressHandler = (DownloadProgressEventHandler) ((sender, args) => progress.Report(new DownloadProgressInfo(chunkManager.ProgressBytes, chunkManager.FileSize, chunkManager.FileName)));
      List<DownloadChunk> chunks = new List<DownloadChunk>();
      List<Task<int>> tasks = new List<Task<int>>();
      for (int index = 0; index < parallelConnections; ++index)
      {
        DownloadChunk nextChunk = chunkManager.GetNextChunk();
        if (nextChunk != null)
        {
          nextChunk.DownloadProgress += progressHandler;
          chunks.Add(nextChunk);
          tasks.Add(nextChunk.Download());
        }
        else
          break;
      }
      int ret = 200;
      while (tasks.Count > 0)
      {
        Task<int> task = await Task.WhenAny<int>((IEnumerable<Task<int>>) tasks);
        int taskIndex = tasks.IndexOf(task);
        DownloadChunk chunk = chunks[taskIndex];
        chunk.DownloadProgress -= progressHandler;
        int num = await task;
        switch (num)
        {
          case 200:
          case 206:
            chunkManager.MarkDownloaded(chunk);
            chunkManager.SaveMetadata(streamer);
            chunk = chunkManager.GetNextChunk();
            if (chunk == null)
            {
              tasks.RemoveAt(taskIndex);
              chunks.RemoveAt(taskIndex);
            }
            else
            {
              chunk.OutStream = streamer.GetStream();
              chunk.DownloadProgress += progressHandler;
              chunks[taskIndex] = chunk;
              tasks[taskIndex] = chunk.Download();
            }
            chunk = (DownloadChunk) null;
            continue;
          default:
            return num;
        }
      }
      streamer.ClearMetadata();
      return ret;
    }

    private void ReportCompleted(
      Downloader.ProgressWrapper progressWrapper,
      string packageId,
      string fileName,
      long fileSize)
    {
      Downloader.ProgressWrapper progressWrapper1 = progressWrapper;
      long num = fileSize;
      DownloadProgressInfo report = new DownloadProgressInfo(num, num, fileName);
      progressWrapper1.Report(report);
      if (this.DownloadReady == null)
        return;
      this.DownloadReady((object) this, new DownloadReadyEventArgs(packageId, fileName));
    }

    private class ProgressWrapper
    {
      private long LargestReported = -1;
      private DownloadProgress<DownloadProgressInfo> Listener;

      internal ProgressWrapper(DownloadProgress<DownloadProgressInfo> listener) => this.Listener = listener;

      internal void Report(DownloadProgressInfo report)
      {
        if (this.Listener == null)
          return;
        lock (this)
        {
          if (this.LargestReported >= report.BytesReceived)
            return;
          this.Listener.Report(report);
          this.LargestReported = report.BytesReceived;
        }
      }
    }
  }
}
