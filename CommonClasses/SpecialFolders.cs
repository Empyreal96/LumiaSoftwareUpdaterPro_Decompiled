// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SpecialFolders
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.LsuPro
{
  public static class SpecialFolders
  {
    public static string Root => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Lumia Software Updater Pro");

    public static string Bin => Path.Combine(SpecialFolders.Root, nameof (Bin));

    public static string BcdEdit => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Lumia Software Updater Pro\\BcdEdit");

    public static string UpdatePackages => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Packages\\Products");

    public static string DefaultUpdatePackageDownloadPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Packages");

    public static string Reports => Path.Combine(SpecialFolders.Root, nameof (Reports));

    public static string Logs => Path.Combine(SpecialFolders.Root, nameof (Logs));

    public static string ExecutingAssembly => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

    public static string Styles => Path.Combine(SpecialFolders.ExecutingAssembly, nameof (Styles));
  }
}
