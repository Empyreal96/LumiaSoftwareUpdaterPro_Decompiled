// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyFlashFileSet
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using System.IO;

namespace Microsoft.LsuPro
{
  public class EmergencyFlashFileSet
  {
    public EmergencyFlashFileSet(string path1, string path2, string path3)
    {
      this.File1 = new FileInfo(path1);
      this.File2 = new FileInfo(path2);
      this.File3 = new FileInfo(path3);
    }

    public FileInfo File1 { get; internal set; }

    public FileInfo File2 { get; internal set; }

    public FileInfo File3 { get; internal set; }
  }
}
