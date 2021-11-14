// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FileSystemEventWatcher
// Assembly: LocalUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F47836D-0FA4-443D-8CFF-1138F5AB3C6A
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LocalUpdatePackageManager.dll

using Microsoft.LsuPro.IO;
using System;
using System.IO;

namespace Microsoft.LsuPro
{
  public class FileSystemEventWatcher
  {
    private FileSystemWatcherHelper fileSystemWatcher;

    public event EventHandler<EventArgs> FileSystemChanged;

    public void Start(string path)
    {
      this.fileSystemWatcher = new FileSystemWatcherHelper();
      this.fileSystemWatcher.Path = path;
      this.fileSystemWatcher.IncludeSubdirectories = true;
      this.fileSystemWatcher.Filter = "*.*";
      this.fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
      this.fileSystemWatcher.EnableRaisingEvents = true;
      this.fileSystemWatcher.Changed += new FileSystemEventHandler(this.HandleFileSystemChanged);
    }

    public void Stop()
    {
      this.fileSystemWatcher.Changed -= new FileSystemEventHandler(this.HandleFileSystemChanged);
      this.fileSystemWatcher.EnableRaisingEvents = false;
      this.fileSystemWatcher.Dispose();
    }

    private void HandleFileSystemChanged(object sender, FileSystemEventArgs e)
    {
      if (e.Name.Contains(".dlmetadata"))
        return;
      EventHandler<EventArgs> fileSystemChanged = this.FileSystemChanged;
      if (fileSystemChanged == null)
        return;
      fileSystemChanged((object) this, new EventArgs());
    }
  }
}
