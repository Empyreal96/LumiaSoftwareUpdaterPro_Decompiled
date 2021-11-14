// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceInfoReaderNamespace.DeviceInfoReaderExecutor
// Assembly: DeviceInfoReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 751BFCA1-492E-45FD-8982-0907B7D0BE5F
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\DeviceInfoReader.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.LsuPro.DeviceInfoReaderNamespace
{
  public static class DeviceInfoReaderExecutor
  {
    public static int Main(string[] args)
    {
      try
      {
        Tracer.Information("DeviceInfoReader is called with args {0}", (object) ((IEnumerable<string>) args).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((current, s) => current + s + " ")));
        DeviceInfoReader deviceInfoReader = new DeviceInfoReader(args);
        if (!deviceInfoReader.ParseArguments())
          return 0;
        deviceInfoReader.ReadDeviceInfos();
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Tracer.Warning("Exception {0} caused deviceinforeader to exit", (object) ex);
      }
      return 1;
    }
  }
}
