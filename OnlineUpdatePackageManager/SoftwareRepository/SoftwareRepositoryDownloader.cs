// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SoftwareRepository.SoftwareRepositoryDownloader
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using SoftwareRepository;
using SoftwareRepository.Streaming;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.SoftwareRepository
{
  public class SoftwareRepositoryDownloader
  {
    private Dictionary<string, SoftwareRepositoryDownloader.DownloadItem> downloadItems;
    private Downloader downloader;
    private int parallelDownloadsCounter;

    public event EventHandler<SoftwareRepositoryDownloader.DownloadFilesProgressEventArgs> DownloadFilesProgress;

    public event EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> DownloadFilesReady;

    public SoftwareRepositoryDownloader(
      IWebProxy proxy,
      string alternateSoftwareRepositoryBaseUrl = null,
      bool allowUseWindowsAuth = true)
    {
      this.downloadItems = new Dictionary<string, SoftwareRepositoryDownloader.DownloadItem>();
      this.downloader = new Downloader()
      {
        SoftwareRepositoryAlternativeBaseUrl = alternateSoftwareRepositoryBaseUrl,
        SoftwareRepositoryUserAgent = "LsuPro",
        SoftwareRepositoryProxy = proxy,
        AllowWindowsAuth = allowUseWindowsAuth
      };
      this.downloader.DownloadReady += new DownloadReadyEventHandler(this.DownloaderOnDownloadReady);
    }

    public void SetAccessToken(string accessToken) => this.downloader.SoftwareRepositoryAuthenticationToken = accessToken;

    public void SetWebProxy(IWebProxy getWebProxy) => this.downloader.SoftwareRepositoryProxy = getWebProxy;

    public void SetAllowUseWindowsAuth(bool allowUseWindowsAuth) => this.downloader.AllowWindowsAuth = allowUseWindowsAuth;

    public bool IsDownloadOngoing() => this.parallelDownloadsCounter != 0;

    public async void DownloadFiles(
      string uniqueId,
      string packageId,
      UpdatePackageFile[] files,
      string path,
      CancellationToken token)
    {
      if (!DirectoryHelper.DirectoryExist(path))
        DirectoryHelper.CreateDirectory(path);
      for (int index1 = 0; index1 < files.Length; ++index1)
      {
        for (int index2 = 0; index2 < files.Length - 1; ++index2)
        {
          if (files[index2].FileSize > files[index2 + 1].FileSize)
          {
            UpdatePackageFile file = files[index2 + 1];
            files[index2 + 1] = files[index2];
            files[index2] = file;
          }
        }
      }
      while (this.parallelDownloadsCounter >= 2)
      {
        await Task.Delay(500);
        if (token.IsCancellationRequested)
        {
          EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> downloadFilesReady = this.DownloadFilesReady;
          if (downloadFilesReady == null)
            return;
          downloadFilesReady((object) this, new SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs(uniqueId, packageId, token.IsCancellationRequested, 0, (Exception) null, new TimeSpan(0L), path));
          return;
        }
      }
      ++this.parallelDownloadsCounter;
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, SoftwareRepositoryDownloader.DownloadItem> downloadItem in this.downloadItems)
      {
        if (downloadItem.Value.PackageCompleted)
          stringList.Add(downloadItem.Key);
      }
      foreach (string key in stringList)
        this.downloadItems.Remove(key);
      SoftwareRepositoryDownloader.DownloadItem item = new SoftwareRepositoryDownloader.DownloadItem(packageId, files, path, token);
      try
      {
        this.downloadItems.Add(uniqueId, item);
      }
      catch (ArgumentException ex)
      {
        Tracer.Error("DownloadFiles: cannot add item to dictionary: {0}", (object) ex.Message);
      }
      foreach (KeyValuePair<string, SoftwareRepositoryDownloader.FileItem> fileItem in item.FileItems)
      {
        if (!token.IsCancellationRequested)
          await this.DownloadFile(fileItem.Value.FileName, item, uniqueId);
      }
      if (!token.IsCancellationRequested || item.ErrorOccured)
        return;
      EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> downloadFilesReady1 = this.DownloadFilesReady;
      if (downloadFilesReady1 == null)
        return;
      downloadFilesReady1((object) this, new SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs(uniqueId, packageId, token.IsCancellationRequested, 0, (Exception) null, new TimeSpan(0L), path));
    }

    private async Task DownloadFile(
      string fileName,
      SoftwareRepositoryDownloader.DownloadItem downloadItem,
      string uniqueId)
    {
      FileStreamer fileStreamer = (FileStreamer) null;
      string filePath = string.Empty;
      try
      {
        if (System.IO.File.Exists(Microsoft.LsuPro.PathHelper.Combine(downloadItem.DownloadPath, fileName)))
        {
          filePath = Microsoft.LsuPro.PathHelper.Combine(downloadItem.DownloadPath, fileName);
        }
        else
        {
          filePath = Microsoft.LsuPro.PathHelper.Combine(downloadItem.DownloadPath, fileName + "temp");
          if (System.IO.File.Exists(filePath))
          {
            FileInfo info = new FileInfo(filePath);
            SoftwareRepositoryDownloader.FileItem item = (SoftwareRepositoryDownloader.FileItem) null;
            while (this.IsFileLocked(info) && System.IO.File.Exists(filePath))
            {
              CancellationToken cancellationToken = downloadItem.CancellationToken;
              if (cancellationToken.IsCancellationRequested)
              {
                EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> downloadFilesReady = this.DownloadFilesReady;
                if (downloadFilesReady != null)
                {
                  EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> eventHandler = downloadFilesReady;
                  string uniqueId1 = uniqueId;
                  string packageId = downloadItem.PackageId;
                  cancellationToken = downloadItem.CancellationToken;
                  int num = cancellationToken.IsCancellationRequested ? 1 : 0;
                  TimeSpan elapsed = downloadItem.Stopwatch.Elapsed;
                  string downloadPath = downloadItem.DownloadPath;
                  SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs e = new SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs(uniqueId1, packageId, num != 0, 0, (Exception) null, elapsed, downloadPath);
                  eventHandler((object) this, e);
                }
                --this.parallelDownloadsCounter;
                return;
              }
              long bytesReceived = 0;
              downloadItem.FileItems.TryGetValue(fileName, out item);
              if (item != null)
                bytesReceived = item.Downloaded;
              this.OnFileStreamProgressChanged(new DownloadProgressInfo(bytesReceived, downloadItem.FilesSize, fileName));
              await Task.Delay(500);
            }
            if (item != null)
            {
              foreach (KeyValuePair<string, SoftwareRepositoryDownloader.DownloadItem> downloadItem1 in this.downloadItems)
              {
                downloadItem1.Value.FileItems.TryGetValue(fileName, out item);
                if (item != null && item.Completed)
                {
                  filePath = Microsoft.LsuPro.PathHelper.Combine(downloadItem.DownloadPath, fileName);
                  break;
                }
              }
            }
            info = (FileInfo) null;
            item = (SoftwareRepositoryDownloader.FileItem) null;
          }
        }
        Random random = new Random();
        bool ready = false;
        do
        {
          try
          {
            await Task.Delay(random.Next(0, 500), downloadItem.CancellationToken);
            fileStreamer = new FileStreamer(filePath, downloadItem.PackageId, downloadItem.DownloadPath);
            ready = true;
          }
          catch (DownloadException ex)
          {
            if (ex.InnerException == null)
              throw;
            else if (ex.InnerException.GetType() == typeof (IOException))
              Tracer.Information("FileStreamer: The process cannot access the file: {0}", (object) ex.InnerException.Message);
          }
        }
        while (!ready || downloadItem.CancellationToken.IsCancellationRequested);
        ready = false;
        do
        {
          try
          {
            await Task.Delay(random.Next(0, 500), downloadItem.CancellationToken);
            await this.downloader.GetFileAsync(downloadItem.PackageId, fileName, (Streamer) fileStreamer, downloadItem.CancellationToken, new DownloadProgress<DownloadProgressInfo>(new Action<DownloadProgressInfo>(this.OnFileStreamProgressChanged)));
            ready = true;
          }
          catch (DownloadException ex)
          {
            if (ex.InnerException == null)
              throw;
            else if (ex.InnerException.GetType() == typeof (IOException))
              Tracer.Information("GetFileAsync: The process cannot access the file: {0}", (object) ex.InnerException.Message);
          }
        }
        while (!ready);
        random = (Random) null;
      }
      catch (Exception ex1)
      {
        Tracer.Information("DownloadFile failed: {0}", (object) ex1.Message);
        if (ex1.Message.Contains("File integrity error"))
        {
          try
          {
            fileStreamer?.Dispose();
            System.IO.File.Delete(filePath);
          }
          catch (Exception ex2)
          {
            Tracer.Information("DownloadFile failed: cannot delete tmp file: {0}", (object) ex2.Message);
          }
        }
        CancellationToken cancellationToken = downloadItem.CancellationToken;
        if (!cancellationToken.IsCancellationRequested)
          downloadItem.ErrorOccured = true;
        EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> downloadFilesReady = this.DownloadFilesReady;
        if (downloadFilesReady != null)
        {
          EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> eventHandler = downloadFilesReady;
          string uniqueId1 = uniqueId;
          string packageId = downloadItem.PackageId;
          cancellationToken = downloadItem.CancellationToken;
          int num = cancellationToken.IsCancellationRequested ? 1 : 0;
          cancellationToken = downloadItem.CancellationToken;
          Exception error = cancellationToken.IsCancellationRequested ? (Exception) null : ex1;
          TimeSpan elapsed = downloadItem.Stopwatch.Elapsed;
          string downloadPath = downloadItem.DownloadPath;
          SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs e = new SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs(uniqueId1, packageId, num != 0, 0, error, elapsed, downloadPath);
          eventHandler((object) this, e);
        }
        --this.parallelDownloadsCounter;
      }
      finally
      {
        fileStreamer?.Dispose();
      }
    }

    private bool IsFileLocked(FileInfo file)
    {
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch (IOException ex)
      {
        return true;
      }
      finally
      {
        fileStream?.Close();
      }
      return false;
    }

    private void DownloaderOnDownloadReady(
      object sender,
      DownloadReadyEventArgs downloadReadyEventArgs)
    {
      foreach (KeyValuePair<string, SoftwareRepositoryDownloader.DownloadItem> downloadItem in this.downloadItems)
      {
        KeyValuePair<string, SoftwareRepositoryDownloader.DownloadItem> item = downloadItem;
        if (item.Value.PackageId == downloadReadyEventArgs.PackageId)
        {
          SoftwareRepositoryDownloader.FileItem fileItem1;
          item.Value.FileItems.TryGetValue(downloadReadyEventArgs.FileName, out fileItem1);
          if (fileItem1 != null)
          {
            if (!System.IO.File.Exists(Microsoft.LsuPro.PathHelper.Combine(item.Value.DownloadPath, downloadReadyEventArgs.FileName)))
            {
              new Task((Action) (() => this.CopyFile(downloadReadyEventArgs, item))).Start();
              break;
            }
            try
            {
              System.IO.File.Delete(Microsoft.LsuPro.PathHelper.Combine(item.Value.DownloadPath, downloadReadyEventArgs.FileName + "temp"));
            }
            catch (Exception ex)
            {
            }
            fileItem1.Completed = true;
            if (item.Value.PackageId == downloadReadyEventArgs.PackageId)
            {
              bool flag = true;
              foreach (KeyValuePair<string, SoftwareRepositoryDownloader.FileItem> fileItem2 in item.Value.FileItems)
              {
                if (!fileItem2.Value.Completed)
                {
                  flag = false;
                  break;
                }
              }
              if (flag && !item.Value.PackageCompleted)
              {
                Tracer.Information("DownloaderOnDownloadReady: download completed {0}", (object) item.Value.PackageId);
                item.Value.PackageCompleted = true;
                --this.parallelDownloadsCounter;
                EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> downloadFilesReady = this.DownloadFilesReady;
                if (downloadFilesReady != null)
                  downloadFilesReady((object) this, new SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs(item.Key, item.Value.PackageId, item.Value.CancellationToken.IsCancellationRequested, 0, (Exception) null, item.Value.Stopwatch.Elapsed, item.Value.DownloadPath));
                item.Value.Stopwatch.Stop();
              }
            }
          }
        }
      }
    }

    private void CopyFile(
      DownloadReadyEventArgs downloadReadyEventArgs,
      KeyValuePair<string, SoftwareRepositoryDownloader.DownloadItem> item)
    {
      try
      {
        System.IO.File.Copy(Microsoft.LsuPro.PathHelper.Combine(item.Value.DownloadPath, downloadReadyEventArgs.FileName + "temp"), Microsoft.LsuPro.PathHelper.Combine(item.Value.DownloadPath, downloadReadyEventArgs.FileName), true);
      }
      catch (Exception ex)
      {
        Tracer.Information("CopyFile failed: {0}", (object) ex.Message);
        EventHandler<SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs> downloadFilesReady = this.DownloadFilesReady;
        if (downloadFilesReady != null)
          downloadFilesReady((object) this, new SoftwareRepositoryDownloader.DownloadFilesCompletedEventArgs(item.Key, item.Value.PackageId, item.Value.CancellationToken.IsCancellationRequested, 0, item.Value.CancellationToken.IsCancellationRequested ? (Exception) null : ex, item.Value.Stopwatch.Elapsed, item.Value.DownloadPath));
        --this.parallelDownloadsCounter;
        return;
      }
      this.DownloaderOnDownloadReady((object) this, downloadReadyEventArgs);
    }

    private void OnFileStreamProgressChanged(DownloadProgressInfo progress)
    {
      foreach (KeyValuePair<string, SoftwareRepositoryDownloader.DownloadItem> downloadItem in this.downloadItems)
      {
        SoftwareRepositoryDownloader.FileItem fileItem1;
        downloadItem.Value.FileItems.TryGetValue(progress.FileName, out fileItem1);
        if (fileItem1 != null)
        {
          fileItem1.Downloaded = progress.BytesReceived;
          long num = 0;
          foreach (KeyValuePair<string, SoftwareRepositoryDownloader.FileItem> fileItem2 in downloadItem.Value.FileItems)
            num = fileItem2.Value.Downloaded + num;
          int progressPercentage = (int) (100L * num / downloadItem.Value.FilesSize);
          if (downloadItem.Value.Stopwatch.ElapsedMilliseconds < 1000L || ((double) downloadItem.Value.Stopwatch.ElapsedMilliseconds - downloadItem.Value.DurationSinceLastProgressEvent.TotalMilliseconds > 1000.0 || downloadItem.Value.PreviousSetProgressPercentage != progressPercentage))
          {
            this.CalculateBytesPerSecond(num, downloadItem.Value);
            int estimatedSecondsRemaining = downloadItem.Value.BytesPerSecond != 0 ? (int) ((downloadItem.Value.FilesSize - num) / (long) downloadItem.Value.BytesPerSecond) : (progressPercentage != 100 ? int.MaxValue : (int) ((downloadItem.Value.FilesSize - num) / (long) int.MaxValue));
            EventHandler<SoftwareRepositoryDownloader.DownloadFilesProgressEventArgs> downloadFilesProgress = this.DownloadFilesProgress;
            if (downloadFilesProgress != null)
              downloadFilesProgress((object) this, new SoftwareRepositoryDownloader.DownloadFilesProgressEventArgs(downloadItem.Key, downloadItem.Value.PackageId, progressPercentage, num, downloadItem.Value.FilesSize, downloadItem.Value.BytesPerSecond, estimatedSecondsRemaining, downloadItem.Value.CancellationToken.IsCancellationRequested));
            downloadItem.Value.PreviousSetProgressPercentage = progressPercentage;
            downloadItem.Value.DurationSinceLastProgressEvent = downloadItem.Value.Stopwatch.Elapsed;
          }
        }
      }
    }

    private void CalculateBytesPerSecond(
      long totalDownloadedSoFar,
      SoftwareRepositoryDownloader.DownloadItem downloadItem)
    {
      TimeSpan timeSpan = downloadItem.Stopwatch.Elapsed.Subtract(TimeSpan.FromMilliseconds(2000.0));
      if (downloadItem.DownloadSpeedTimeSpan < timeSpan)
      {
        long num = totalDownloadedSoFar - downloadItem.PreviousTotalDownloaded;
        downloadItem.PreviousTotalDownloaded = totalDownloadedSoFar;
        downloadItem.BytesPerSecond = (int) (num * 1000L / 2000L);
        downloadItem.DownloadSpeedTimeSpan = downloadItem.Stopwatch.Elapsed;
      }
      if (downloadItem.BytesPerSecond > 0)
        return;
      downloadItem.BytesPerSecond = 0;
    }

    public bool DirectoryHasEnoughSpaceForPackage(
      string targetDirectory,
      UpdatePackageFile[] files,
      out SoftwareRepositoryDownloader.DiskSpaceInformation diskSpaceInfo)
    {
      long num = ((IEnumerable<UpdatePackageFile>) files).Sum<UpdatePackageFile>((Func<UpdatePackageFile, long>) (i => i.FileSize));
      Tracer.Information("Combined size of package = {0}", (object) num);
      long totalFreeSpace = DiskDriveInfo.GetTotalFreeSpace(targetDirectory);
      Tracer.Information("Space available = {0}", (object) totalFreeSpace);
      long required = (long) (3.0 * (double) num);
      if (required > totalFreeSpace)
      {
        Tracer.Warning("There is not enough space on disk for package download (required={0}, free={1})", (object) required, (object) totalFreeSpace);
        diskSpaceInfo = new SoftwareRepositoryDownloader.DiskSpaceInformation(required, totalFreeSpace);
        return false;
      }
      Tracer.Information("There is enough space available for package download");
      diskSpaceInfo = new SoftwareRepositoryDownloader.DiskSpaceInformation(required, totalFreeSpace);
      return true;
    }

    internal class DownloadItem
    {
      public DownloadItem(
        string packageId,
        UpdatePackageFile[] files,
        string path,
        CancellationToken token)
      {
        this.Stopwatch = new Stopwatch();
        this.Stopwatch.Start();
        this.DurationSinceLastProgressEvent = this.Stopwatch.Elapsed;
        this.DownloadSpeedTimeSpan = this.Stopwatch.Elapsed;
        this.BytesPerSecond = 65536;
        this.PreviousTotalDownloaded = 0L;
        this.CancellationToken = token;
        this.DownloadPath = path;
        this.PackageId = packageId;
        this.FilesSize = 0L;
        this.FileItems = new Dictionary<string, SoftwareRepositoryDownloader.FileItem>();
        this.ErrorOccured = false;
        this.PackageCompleted = false;
        foreach (UpdatePackageFile file in files)
        {
          this.FilesSize = file.FileSize + this.FilesSize;
          this.FileItems.Add(file.FileName, new SoftwareRepositoryDownloader.FileItem(file.FileName, false, 0L));
        }
      }

      public string PackageId { get; private set; }

      public Dictionary<string, SoftwareRepositoryDownloader.FileItem> FileItems { get; private set; }

      public long FilesSize { get; set; }

      public CancellationToken CancellationToken { get; private set; }

      public string DownloadPath { get; private set; }

      public int BytesPerSecond { get; set; }

      public TimeSpan DownloadSpeedTimeSpan { get; set; }

      public long PreviousTotalDownloaded { get; set; }

      public Stopwatch Stopwatch { get; private set; }

      public TimeSpan DurationSinceLastProgressEvent { get; set; }

      public int PreviousSetProgressPercentage { get; set; }

      public bool ErrorOccured { get; set; }

      public bool PackageCompleted { get; set; }
    }

    internal class FileItem
    {
      public FileItem(string filename, bool completed, long downloaded)
      {
        this.FileName = filename;
        this.Completed = completed;
        this.Downloaded = downloaded;
      }

      public string FileName { get; private set; }

      public bool Completed { get; set; }

      public long Downloaded { get; set; }
    }

    public class DownloadFilesCompletedEventArgs : EventArgs
    {
      public DownloadFilesCompletedEventArgs(
        string uniqueId,
        string packageId,
        bool canceled,
        int numberOfSkippedFiles,
        Exception error,
        TimeSpan duration,
        string localPath)
      {
        this.UniqueId = uniqueId;
        this.PackageId = packageId;
        this.Canceled = canceled;
        this.NumberOfSkippedFiles = numberOfSkippedFiles;
        this.Error = error;
        this.Duration = duration;
        this.LocalPath = localPath;
      }

      public string UniqueId { get; private set; }

      public string PackageId { get; private set; }

      public bool Canceled { get; private set; }

      public string LocalPath { get; private set; }

      public int NumberOfSkippedFiles { get; private set; }

      public Exception Error { get; private set; }

      public TimeSpan Duration { get; private set; }

      public int AverageDownloadSpeed { get; internal set; }
    }

    public class DownloadFilesProgressEventArgs : EventArgs
    {
      public DownloadFilesProgressEventArgs(
        string uniqueId,
        string packageId,
        int progressPercentage,
        long downloaded,
        long total,
        int approximatedBytesPerSecond,
        int estimatedSecondsRemaining,
        bool cancel)
      {
        if (progressPercentage > 100 || progressPercentage < 0 || downloaded > total)
          throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Set progress went beyond constraints ({0}%, received {1}, total {2}", (object) progressPercentage, (object) downloaded, (object) total));
        this.UniqueId = uniqueId;
        this.ProgressPercentage = progressPercentage;
        this.BytesReceived = downloaded;
        this.TotalBytes = total;
        this.PackageId = packageId;
        this.TransferSpeed = approximatedBytesPerSecond;
        this.EstimatedSecondsRemaining = estimatedSecondsRemaining;
        this.Cancel = cancel;
      }

      public string UniqueId { get; private set; }

      public bool Cancel { get; set; }

      public int ProgressPercentage { get; private set; }

      public long BytesReceived { get; private set; }

      public long TotalBytes { get; private set; }

      public string PackageId { get; private set; }

      public int TransferSpeed { get; private set; }

      public int EstimatedSecondsRemaining { get; private set; }
    }

    public class DiskSpaceInformation
    {
      internal DiskSpaceInformation(long required, long available)
      {
        this.RequiredBytes = required;
        this.AvailableBytes = available;
      }

      public long RequiredBytes { get; private set; }

      public long AvailableBytes { get; private set; }
    }
  }
}
