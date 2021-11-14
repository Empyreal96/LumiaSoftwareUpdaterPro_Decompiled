// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackageCleaner
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class UpdatePackageCleaner
  {
    private readonly long warningLevelInBytes;
    private readonly string directory;
    private object state = new object();
    private Timer scheduler;

    public static event EventHandler<EventArgs> UnfinishedDownloadsRemoved;

    public event EventHandler<UnfinishedDownloadsEventArgs> UnfinishedDownloadsAlert;

    public UpdatePackageCleaner(string directory, int warningLevelGigabytes)
    {
      this.warningLevelInBytes = (long) warningLevelGigabytes * 1073741824L;
      this.directory = directory;
      this.scheduler = (Timer) null;
    }

    public void StartScheduler(TimeSpan period)
    {
      if (this.scheduler != null)
        return;
      this.scheduler = new Timer(new TimerCallback(this.OnTimerElapsed), this.state, TimeSpan.Zero, period);
    }

    public void StopScheduler()
    {
      if (this.scheduler == null)
        return;
      this.scheduler.Dispose();
    }

    private void OnTimerElapsed(object state)
    {
      try
      {
        UnfinishedDownloadsInfo unfinishedDownloads = UpdatePackageCleaner.GetListOfUnfinishedDownloads(this.directory);
        if (unfinishedDownloads.TotalSize <= this.warningLevelInBytes)
          return;
        this.SendUnfinishedDownloadsAlert(unfinishedDownloads);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Periodical unfinished download check failed", objArray);
      }
    }

    protected virtual void SendUnfinishedDownloadsAlert(
      UnfinishedDownloadsInfo unfinishedDownloadsInfo)
    {
      EventHandler<UnfinishedDownloadsEventArgs> unfinishedDownloadsAlert = this.UnfinishedDownloadsAlert;
      if (unfinishedDownloadsAlert == null)
        return;
      unfinishedDownloadsAlert((object) this, new UnfinishedDownloadsEventArgs(unfinishedDownloadsInfo));
    }

    public static UnfinishedDownloadsInfo GetListOfUnfinishedDownloads(
      string directory)
    {
      UnfinishedDownloadsInfo unfinishedDownloadsInfo = new UnfinishedDownloadsInfo();
      if (DiskDriveInfo.DirectoryIsOnNetworkDrive(directory))
      {
        Tracer.Warning("Download directory is located on network drive");
        return unfinishedDownloadsInfo;
      }
      foreach (FileInfo accessibleSubdir in DirectoryHelper.GetFileInfosFromAllAccessibleSubdirs(directory, "*.resume"))
      {
        if (accessibleSubdir.Directory != null)
        {
          string searchPattern = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) Path.GetFileNameWithoutExtension(accessibleSubdir.Name)).Substring(37).Insert(0, "*");
          foreach (FileInfo file in accessibleSubdir.Directory.GetFiles(searchPattern))
            unfinishedDownloadsInfo.AddFile(file, false);
        }
        unfinishedDownloadsInfo.AddFile(accessibleSubdir, true);
      }
      foreach (FileInfo accessibleSubdir in DirectoryHelper.GetFileInfosFromAllAccessibleSubdirs(directory, "*.*temp"))
      {
        if (accessibleSubdir.Directory != null)
          unfinishedDownloadsInfo.AddFile(accessibleSubdir, true);
      }
      return unfinishedDownloadsInfo;
    }

    public static bool CleanUnfinishedDownloads(UnfinishedDownloadsInfo unfinishedDownloadsInfo)
    {
      bool flag = true;
      foreach (FileInfo file in unfinishedDownloadsInfo.Files)
      {
        try
        {
          Tracer.Information("Cleaning unfinished download {0}", (object) file.Name);
          File.Delete(file.FullName);
        }
        catch (Exception ex)
        {
          object[] objArray = new object[0];
          Tracer.Warning(ex, "Failed to clean unfinished download", objArray);
          flag = false;
        }
      }
      UpdatePackageCleaner.SendUnfinishedDownloadsRemoved();
      return flag;
    }

    private static void SendUnfinishedDownloadsRemoved()
    {
      EventHandler<EventArgs> downloadsRemoved = UpdatePackageCleaner.UnfinishedDownloadsRemoved;
      if (downloadsRemoved == null)
        return;
      downloadsRemoved((object) downloadsRemoved, EventArgs.Empty);
    }
  }
}
