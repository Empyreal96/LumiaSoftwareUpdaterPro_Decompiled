// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.TemporaryDirectory
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.LsuPro
{
  public class TemporaryDirectory : IDisposable
  {
    private DirectoryInfo directoryInfo;

    public TemporaryDirectory(string path)
    {
      this.directoryInfo = new DirectoryInfo(Path.Combine(path, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TempDir_{0}", (object) Guid.NewGuid().ToString())));
      Directory.CreateDirectory(this.directoryInfo.FullName);
      this.FullPath = this.directoryInfo.FullName;
    }

    public string FullPath { get; private set; }

    public FileInfo[] GetFiles(string filter) => this.directoryInfo.GetFiles(filter);

    public void Delete()
    {
      if (this.directoryInfo == null || !this.directoryInfo.Exists)
        return;
      this.directoryInfo.Delete(true);
      this.directoryInfo = (DirectoryInfo) null;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing) => this.Delete();
  }
}
