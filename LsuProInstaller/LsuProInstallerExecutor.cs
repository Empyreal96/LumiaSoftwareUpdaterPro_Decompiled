// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.LsuProInstallerNamespace.LsuProInstallerExecutor
// Assembly: LsuProInstaller, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0D77AD-19D4-4325-A92F-5D3ED484BABF
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LsuProInstaller.exe

using System;

namespace Microsoft.LsuPro.LsuProInstallerNamespace
{
  public static class LsuProInstallerExecutor
  {
    public static int Main(string[] args)
    {
      try
      {
        LsuProInstaller lsuProInstaller = new LsuProInstaller(args);
        if (!lsuProInstaller.ParseArguments())
          return 0;
        lsuProInstaller.PerformOperation();
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception {0} caused LsuProInstaller to exit", (object) ex.Message);
      }
      return 1;
    }
  }
}
