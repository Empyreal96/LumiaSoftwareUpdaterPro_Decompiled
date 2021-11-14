// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.IO.FileSystemWatcherHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.LsuPro.IO
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class FileSystemWatcherHelper
  {
    private FileSystemWatcher fileSystemWatcher;

    public FileSystemWatcherHelper() => this.fileSystemWatcher = new FileSystemWatcher();

    public event FileSystemEventHandler Changed
    {
      add
      {
        lock (this.fileSystemWatcher)
          this.fileSystemWatcher.Changed += value;
      }
      remove
      {
        lock (this.fileSystemWatcher)
          this.fileSystemWatcher.Changed -= value;
      }
    }

    public string Path
    {
      get => this.fileSystemWatcher.Path;
      set => this.fileSystemWatcher.Path = value;
    }

    public bool IncludeSubdirectories
    {
      get => this.fileSystemWatcher.IncludeSubdirectories;
      set => this.fileSystemWatcher.IncludeSubdirectories = value;
    }

    public string Filter
    {
      get => this.fileSystemWatcher.Filter;
      set => this.fileSystemWatcher.Filter = value;
    }

    public NotifyFilters NotifyFilter
    {
      get => this.fileSystemWatcher.NotifyFilter;
      set => this.fileSystemWatcher.NotifyFilter = value;
    }

    public bool EnableRaisingEvents
    {
      get => this.fileSystemWatcher.EnableRaisingEvents;
      set => this.fileSystemWatcher.EnableRaisingEvents = value;
    }

    public void Dispose() => this.fileSystemWatcher.Dispose();
  }
}
