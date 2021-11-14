// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.LocalUpdatePackageManager
// Assembly: LocalUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F47836D-0FA4-443D-8CFF-1138F5AB3C6A
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LocalUpdatePackageManager.dll

using Microsoft.LsuPro.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "not needed here")]
  public class LocalUpdatePackageManager
  {
    private const int PoolingTimeInSeconds = 8;
    private SqlHelper sqlHelper;
    private VplHelper vplHelper;
    private TaskHelper poolingTask;
    private Collection<FileSystemEventWatcher> fileSystemEventWatchers;
    private bool fileSystemChanged;
    private Stopwatch elapsedTime;
    private long startTime;
    private CancellationTokenSource poolingTaskCancellationTokenSource;
    private CancellationToken poolingTaskCancellationToken;
    private static object synchOb = new object();
    private bool sqlDbUpdated;
    private List<string> directories = new List<string>();
    private List<string> files = new List<string>();

    public event EventHandler<EventArgs> SqlDbUpdated;

    public LocalUpdatePackageManager()
      : this(new Collection<FileSystemEventWatcher>())
    {
    }

    public LocalUpdatePackageManager(
      Collection<FileSystemEventWatcher> fileSystemEventWatchers)
    {
      this.vplHelper = new VplHelper();
      this.sqlHelper = new SqlHelper();
      this.fileSystemEventWatchers = fileSystemEventWatchers;
      this.elapsedTime = new Stopwatch();
      this.Sources = new Collection<string>();
      this.poolingTaskCancellationTokenSource = new CancellationTokenSource();
      this.poolingTaskCancellationToken = this.poolingTaskCancellationTokenSource.Token;
    }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "on purpose")]
    public Collection<string> Sources { get; set; }

    public void Start()
    {
      Tracer.Information("Starting pooling task");
      this.poolingTask = new TaskHelper(new Action(this.PoolingTask), this.poolingTaskCancellationToken);
      this.poolingTask.Start();
      this.poolingTask.ContinueWith((Action<object>) (t =>
      {
        if (this.poolingTask.Exception == null)
          return;
        foreach (Exception innerException in this.poolingTask.Exception.InnerExceptions)
          Tracer.Error(innerException.Message);
      }), TaskContinuationOptions.OnlyOnFaulted);
      Tracer.Information("Creating file system watchers");
      foreach (string source in this.Sources)
      {
        Tracer.Information("Creating watcher for '{0}'", (object) source);
        if (string.IsNullOrEmpty(source) || !DirectoryHelper.DirectoryExist(source))
        {
          Tracer.Information("Directory does not exist");
        }
        else
        {
          FileSystemEventWatcher systemEventWatcher = new FileSystemEventWatcher();
          systemEventWatcher.FileSystemChanged += new EventHandler<EventArgs>(this.HandleFileSystemChanged);
          systemEventWatcher.Start(source);
          this.fileSystemEventWatchers.Add(systemEventWatcher);
        }
      }
      Tracer.Information("Starting timer");
      this.startTime = 0L;
      this.elapsedTime.Start();
    }

    public void Stop()
    {
      foreach (FileSystemEventWatcher systemEventWatcher in this.fileSystemEventWatchers)
      {
        systemEventWatcher.FileSystemChanged -= new EventHandler<EventArgs>(this.HandleFileSystemChanged);
        systemEventWatcher.Stop();
      }
      if (this.poolingTask != null)
      {
        this.poolingTaskCancellationTokenSource.Cancel();
        try
        {
          this.poolingTask.Wait();
        }
        catch (Exception ex)
        {
          Tracer.Information("Pooling operation was cancelled: {0}", (object) ex.Message);
        }
        this.poolingTask.Dispose();
      }
      if (this.elapsedTime == null)
        return;
      this.elapsedTime.Stop();
    }

    private void PoolingTask()
    {
      Thread.CurrentThread.Name = nameof (LocalUpdatePackageManager);
      try
      {
        this.SearchForLocalUpdatePackages(false);
      }
      catch (OperationCanceledException ex)
      {
        Tracer.Information("PoolingTask was cancelled");
        return;
      }
      do
      {
        Thread.Sleep(1000);
        if (this.poolingTaskCancellationToken.IsCancellationRequested)
          break;
        this.TriggerSearching();
      }
      while (!this.poolingTaskCancellationToken.IsCancellationRequested);
    }

    private void TriggerSearching()
    {
      if ((this.elapsedTime.ElapsedMilliseconds - this.startTime) / 1000L <= 8L)
        return;
      if (!this.fileSystemChanged)
        return;
      try
      {
        Tracer.Information("Something was changed in file system, trigger searching");
        this.SearchForLocalUpdatePackages(true);
      }
      catch (OperationCanceledException ex)
      {
        Tracer.Information("PoolingTask was cancelled");
        return;
      }
      this.startTime = 0L;
      this.elapsedTime.Restart();
      this.fileSystemChanged = false;
    }

    public void SearchForLocalUpdatePackages(bool sendEvent)
    {
      lock (LocalUpdatePackageManager.synchOb)
      {
        this.sqlDbUpdated = false;
        this.RemoveNotExistingUpdatePackages();
        foreach (string source in this.Sources)
        {
          Tracer.Information("SearchForLocalUpdatePackages: {0}", (object) source);
          if (this.poolingTaskCancellationToken.IsCancellationRequested)
            this.poolingTaskCancellationToken.ThrowIfCancellationRequested();
          foreach (string vplPath in this.GetVplPathList(source))
          {
            if (this.poolingTaskCancellationToken.IsCancellationRequested)
              this.poolingTaskCancellationToken.ThrowIfCancellationRequested();
            if (this.vplHelper.CheckIfAllMandatoryFilesAreOnDisc(vplPath))
            {
              Tracer.Information("SearchForLocalUpdatePackages: InsertUpdatePackage: {0}", (object) vplPath);
              UpdatePackage updatePackage = new UpdatePackage(this.vplHelper.GetProductTypeFromVpl(vplPath), this.vplHelper.GetProductCodeFromVpl(vplPath), this.vplHelper.GetSoftwareVersionFromVpl(vplPath), this.vplHelper.GetVariantVersionFromVpl(vplPath), this.vplHelper.GetVariantDescriptionFromVpl(vplPath), this.vplHelper.GetPackageUsePurposeFromVpl(vplPath), this.vplHelper.GetPackageUseTypeFromVpl(vplPath), this.vplHelper.GetBuildTypeFromVpl(vplPath), this.vplHelper.GetOsVersionFromVpl(vplPath), this.vplHelper.GetAkVersionFromVpl(vplPath), this.vplHelper.GetBspVersionFromVpl(vplPath), vplPath, vplPath, source, false, DateTime.Now, (UpdatePackageFile[]) null);
              try
              {
                this.sqlHelper.InsertUpdatePackage(updatePackage, true);
              }
              catch (DuplicateNameException ex)
              {
                continue;
              }
              this.sqlDbUpdated = true;
            }
          }
        }
        if (!(this.sqlDbUpdated & sendEvent))
          return;
        EventHandler<EventArgs> sqlDbUpdated = this.SqlDbUpdated;
        if (sqlDbUpdated == null)
          return;
        sqlDbUpdated((object) this, new EventArgs());
      }
    }

    private void RemoveNotExistingUpdatePackages()
    {
      try
      {
        foreach (UpdatePackage allUpdatePackage in this.sqlHelper.ReadAllUpdatePackages(false))
        {
          if (this.poolingTaskCancellationToken.IsCancellationRequested)
            this.poolingTaskCancellationToken.ThrowIfCancellationRequested();
          foreach (string source in this.Sources)
          {
            if (string.Compare(allUpdatePackage.SourcePath, source, StringComparison.OrdinalIgnoreCase) != 0)
              break;
          }
          VplHelper vplHelper = new VplHelper();
          string ffuFileFromVpl = vplHelper.GetFfuFileFromVpl(allUpdatePackage.VplPath);
          if (!File.Exists(allUpdatePackage.VplPath) || !File.Exists(ffuFileFromVpl) || vplHelper.GetVariantGroupFromVpl(allUpdatePackage.VplPath).ToLowerInvariant() == "enosw")
          {
            Tracer.Information("RemoveNotExistingUpdatePackages: DeleteUpdatePackage: {0}", (object) allUpdatePackage.VplPath);
            this.sqlHelper.DeleteUpdatePackage(allUpdatePackage);
            this.sqlDbUpdated = true;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
    }

    private string[] GetVplPathList(string searchPath)
    {
      this.files.Clear();
      this.directories.Clear();
      this.directories.AddRange((IEnumerable<string>) this.GetDirectories(searchPath));
      string searchPattern = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "*.vpl");
      foreach (string directory in this.directories)
      {
        try
        {
          this.files.AddRange((IEnumerable<string>) DirectoryHelper.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly));
        }
        catch (Exception ex)
        {
        }
      }
      return this.files.ToArray();
    }

    private string[] GetDirectories(string searchPath)
    {
      string[] strArray = new string[0];
      string[] directories;
      try
      {
        directories = DirectoryHelper.GetDirectories(searchPath, "*.*", SearchOption.TopDirectoryOnly);
        foreach (string searchPath1 in directories)
          this.directories.AddRange((IEnumerable<string>) this.GetDirectories(searchPath1));
      }
      catch (Exception ex)
      {
        return new string[0];
      }
      return directories;
    }

    private void HandleFileSystemChanged(object sender, EventArgs e) => this.fileSystemChanged = true;
  }
}
