// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FileEntry
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

namespace Microsoft.LsuPro
{
  public class FileEntry
  {
    public FileEntry(string name, string crc)
    {
      this.Name = name;
      this.Crc = crc;
    }

    public string Name { get; private set; }

    public string Crc { get; private set; }
  }
}
