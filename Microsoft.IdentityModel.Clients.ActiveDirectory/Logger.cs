// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Logger
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class Logger
  {
    internal static string PrepareLogMessage(
      CallState callState,
      string classOrComponent,
      string format,
      params object[] args)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}: {1} - {2}: {3}", (object) DateTime.UtcNow, (object) (callState != null ? callState.CorrelationId.ToString() : string.Empty), (object) classOrComponent, (object) str);
    }

    internal static void Verbose(CallState callState, string format, params object[] args)
    {
      string message = Logger.PrepareLogMessage(callState, Logger.GetCallerType(), format, args);
      AdalTrace.TraceSource.TraceEvent(TraceEventType.Verbose, 1, message);
      if (!AdalTrace.LegacyTraceSwitch.TraceVerbose)
        return;
      Trace.TraceInformation(message);
    }

    internal static void Information(CallState callState, string format, params object[] args)
    {
      string message = Logger.PrepareLogMessage(callState, Logger.GetCallerType(), format, args);
      AdalTrace.TraceSource.TraceData(TraceEventType.Information, 2, (object) message);
      if (!AdalTrace.LegacyTraceSwitch.TraceInfo)
        return;
      Trace.TraceInformation(message);
    }

    internal static void Warning(CallState callState, string format, params object[] args)
    {
      string message = Logger.PrepareLogMessage(callState, Logger.GetCallerType(), format, args);
      AdalTrace.TraceSource.TraceEvent(TraceEventType.Warning, 3, message);
      if (!AdalTrace.LegacyTraceSwitch.TraceWarning)
        return;
      Trace.TraceWarning(message);
    }

    internal static void Error(CallState callState, Exception ex)
    {
      string message = Logger.PrepareLogMessage(callState, Logger.GetCallerType(), "{0}", (object) ex);
      AdalTrace.TraceSource.TraceEvent(TraceEventType.Error, 4, message);
      if (!AdalTrace.LegacyTraceSwitch.TraceError)
        return;
      Trace.TraceError(message);
    }

    private static string GetCallerType()
    {
      MethodBase method = new StackFrame(2, false).GetMethod();
      return !(method.ReflectedType != (Type) null) ? (string) null : method.ReflectedType.Name;
    }
  }
}
