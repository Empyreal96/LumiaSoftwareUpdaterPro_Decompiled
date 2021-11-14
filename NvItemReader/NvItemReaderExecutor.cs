// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemReaderNamespace.NvItemReaderExecutor
// Assembly: NvItemReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F6886691-C1CC-4CFC-8E60-297EAF353617
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemReader.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.LsuPro.NvItemReaderNamespace
{
  public static class NvItemReaderExecutor
  {
    public static int Main(string[] args)
    {
      try
      {
        Tracer.Information("NvItemReader is called with args {0}", (object) ((IEnumerable<string>) args).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((current, s) => current + s + " ")));
        NvItemReader nvItemReader = new NvItemReader(args);
        if (!nvItemReader.ParseArguments())
          return 0;
        nvItemReader.PerformOperation();
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Tracer.Warning("Exception {0} caused nvitemreader to exit", (object) ex);
      }
      return 1;
    }
  }
}
