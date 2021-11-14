// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.EtwSession
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.Windows.ImageTools
{
  internal class EtwSession : IDisposable
  {
    private string sessionName;
    private ulong traceHandle;

    public EtwSession()
    {
      this.sessionName = Process.GetCurrentProcess().ProcessName + Process.GetCurrentProcess().Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.EtlPath = Path.Combine(Path.GetTempPath(), this.sessionName + ".etl");
      this.traceHandle = ulong.MaxValue;
      this.StartTracing();
    }

    public string EtlPath { get; private set; }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults")]
    public void Dispose()
    {
      if (this.traceHandle == ulong.MaxValue)
        return;
      NativeMethods.StopTrace(this.traceHandle, this.sessionName, out EventTraceProperties _);
      this.traceHandle = ulong.MaxValue;
    }

    private void StartTracing()
    {
      EventTraceProperties properties = EventTraceProperties.CreateProperties(this.sessionName, this.EtlPath, LoggingModeConstant.PrivateLoggerMode | LoggingModeConstant.PrivateInProc);
      int error1 = NativeMethods.StartTrace(out this.traceHandle, this.sessionName, ref properties);
      if (error1 != 0)
        throw new Win32Exception(error1);
      Guid[] guidArray = new Guid[2]
      {
        new Guid("3bbd891e-180f-4386-94b5-d71ba7ac25a9"),
        new Guid("fb961307-bc64-4de4-8828-81d583524da0")
      };
      foreach (Guid providerId in guidArray)
      {
        int error2 = NativeMethods.EnableTraceEx2(this.traceHandle, providerId, 1U);
        if (error2 != 0)
          throw new Win32Exception(error2);
      }
    }
  }
}
