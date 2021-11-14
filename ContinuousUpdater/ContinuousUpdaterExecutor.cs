// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ContinuousUpdaterNamespace.ContinuousUpdaterExecutor
// Assembly: ContinuousUpdater, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: E264D3AD-34F4-49F1-910A-A4F17DAFC923
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ContinuousUpdater.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.LsuPro.ContinuousUpdaterNamespace
{
  public static class ContinuousUpdaterExecutor
  {
    public static int Main(string[] args)
    {
      try
      {
        Tracer.Information("ContinuousUpdater is called with args {0}", (object) ((IEnumerable<string>) args).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((current, s) => current + s)));
        ContinuousUpdater continuousUpdater = new ContinuousUpdater(args);
        if (!continuousUpdater.ParseArguments())
          return 0;
        continuousUpdater.PerformOperation();
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Tracer.Warning("Exception {0} caused ContinuousUpdater to exit", (object) ex);
      }
      return 1;
    }
  }
}
