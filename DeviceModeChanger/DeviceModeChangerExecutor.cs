// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceModeChangerNamespace.DeviceModeChangerExecutor
// Assembly: DeviceModeChanger, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 7BFF9D1F-0342-42E9-B090-043A753FF219
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\DeviceModeChanger.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.LsuPro.DeviceModeChangerNamespace
{
  public static class DeviceModeChangerExecutor
  {
    public static int Main(string[] args)
    {
      try
      {
        Tracer.Information("DeviceModeChanger is called with args {0}", (object) ((IEnumerable<string>) args).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((current, s) => current + s)));
        DeviceModeChanger deviceModeChanger = new DeviceModeChanger(args);
        if (deviceModeChanger.ParseArguments())
        {
          deviceModeChanger.ChangeDeviceMode();
          Tracer.Information("DeviceModeChanger is exiting with exit code 0.");
          return 0;
        }
      }
      catch (InvalidOperationException ex)
      {
        Tracer.Error("Unknown operation {0}, Exit code 1", (object) ex.Message);
        Console.WriteLine("[DeviceModeChanger] No device detected in given port.");
        return 1;
      }
      catch (Exception ex)
      {
        Console.WriteLine("[DeviceModeChanger] " + ex.Message);
        Tracer.Information("DeviceModeChanger is exiting with exit code 1.");
        return 1;
      }
      Console.WriteLine("Invalid parameters");
      foreach (string str in args)
        Console.WriteLine(str);
      Tracer.Information("DeviceModeChanger is exiting with exit code 1.");
      return 1;
    }
  }
}
