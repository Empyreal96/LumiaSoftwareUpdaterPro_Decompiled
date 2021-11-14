// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvUpdaterLogPollerNamespace.NvUpdaterLogPollerExecutor
// Assembly: NvUpdaterLogPoller, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: BEB2359E-8F80-4FFF-A3A2-60B1221F2B0A
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvUpdaterLogPoller.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.LsuPro.NvUpdaterLogPollerNamespace
{
  public static class NvUpdaterLogPollerExecutor
  {
    public static int Main(string[] args)
    {
      try
      {
        Tracer.Information("NvUpdaterLogPoller is called with args {0}", (object) ((IEnumerable<string>) args).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((current, s) => current + s + " ")));
        Thread.Sleep(5000);
        NvUpdaterLogPoller updaterLogPoller = new NvUpdaterLogPoller(args);
        if (!updaterLogPoller.ParseArguments())
          return 0;
        updaterLogPoller.TryReadDeviceLog();
        Console.WriteLine("Success!");
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Tracer.Warning("Exception {0} caused NvUpdaterLogPoller to exit", (object) ex);
      }
      return 1;
    }
  }
}
