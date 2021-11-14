// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DirectoryEntry
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

namespace Microsoft.LsuPro
{
  public class DirectoryEntry
  {
    public DirectoryEntry(string name, string user, string genericAll)
    {
      this.Name = name;
      this.User = user;
      this.GenericAll = genericAll;
    }

    public string Name { get; private set; }

    public string User { get; private set; }

    public string GenericAll { get; private set; }
  }
}
