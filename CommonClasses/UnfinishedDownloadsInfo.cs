// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UnfinishedDownloadsInfo
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Microsoft.LsuPro
{
  public class UnfinishedDownloadsInfo
  {
    private List<FileInfo> files;
    private int unfinishedDownloads;

    internal UnfinishedDownloadsInfo()
    {
      this.files = new List<FileInfo>();
      this.unfinishedDownloads = 0;
    }

    internal void AddFile(FileInfo file, bool increaseCount)
    {
      foreach (FileSystemInfo file1 in this.files)
      {
        if (file1.FullName == file.FullName)
          return;
      }
      this.files.Add(file);
      if (!increaseCount)
        return;
      ++this.unfinishedDownloads;
    }

    public int NumberOfFiles => this.unfinishedDownloads;

    public long TotalSize => this.files.Sum<FileInfo>((Func<FileInfo, long>) (file => file.Length));

    internal ReadOnlyCollection<FileInfo> Files => this.files.AsReadOnly();
  }
}
