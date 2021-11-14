// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Trace
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.LsuPro
{
  public static class Trace
  {
    private static TextWriter textWriter;
    private static bool disableConsoleOutput;

    static Trace()
    {
      string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:yyyyMMdd}_{0:HHmm}_{0:ssfff}_{1}_{2}.log", (object) DateTime.Now, (object) "installationverification", (object) new Random().Next(100000, 999999));
      Trace.textWriter = (TextWriter) new StreamWriter((Stream) new FileStream(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Lumia Software Updater Pro\\Logs"), path2), FileMode.Append, FileAccess.Write))
      {
        AutoFlush = true
      };
    }

    public static void DisableConsoleOutput(bool disable) => Trace.disableConsoleOutput = disable;

    public static void Error(string format, params object[] args) => Trace.Info("Error: " + format, args);

    public static void Info(string format, params object[] args)
    {
      try
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
        System.Diagnostics.Trace.WriteLine(message);
        if (!Trace.disableConsoleOutput)
          Console.WriteLine(message);
        if (Trace.textWriter == null)
          return;
        Trace.textWriter.WriteLine(message);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine("\n\nTrace.Info failed: '" + ex.ToString() + "'\n\n");
      }
    }
  }
}
