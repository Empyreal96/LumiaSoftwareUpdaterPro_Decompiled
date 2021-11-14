// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Tracer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.LsuPro
{
  public static class Tracer
  {
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Error(Exception ex, string format, params object[] args) => Tracer.Event(TraceEventType.Error, ex, format, args);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Error(string format, params object[] args) => Tracer.Event(TraceEventType.Error, (Exception) null, format, args);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Warning(Exception ex, string format, params object[] args) => Tracer.Event(TraceEventType.Warning, ex, format, args);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Warning(string format, params object[] args) => Tracer.Event(TraceEventType.Warning, (Exception) null, format, args);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Information(Exception ex, string format, params object[] args) => Tracer.Event(TraceEventType.Information, ex, format, args);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Information(string format, params object[] args) => Tracer.Event(TraceEventType.Information, (Exception) null, format, args);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Event(
      TraceEventType traceEventType,
      Exception ex,
      string format,
      params object[] args)
    {
      try
      {
        MethodBase method = new StackFrame(2).GetMethod();
        Type declaringType = method.DeclaringType;
        StringBuilder stringBuilder = new StringBuilder(4096);
        stringBuilder.Append(DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append(" | ");
        stringBuilder.Append(Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append(" | ");
        stringBuilder.Append((Type) null == declaringType ? "<unknown>" : Path.GetFileName(declaringType.Assembly.Location));
        stringBuilder.Append(" | ");
        stringBuilder.Append((Type) null == declaringType ? "<unknown>" : declaringType.FullName);
        stringBuilder.Append(".");
        stringBuilder.Append((Type) null == declaringType ? "<unknown>" : method.Name.Replace(".ctor", declaringType.Name));
        stringBuilder.Append("() | ");
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) traceEventType);
        stringBuilder.Append(" | ");
        if (args.Length != 0)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, args);
        else
          stringBuilder.Append(format);
        Tracer.WriteLine(stringBuilder.ToString());
        for (; ex != null; ex = ex.InnerException)
          Tracer.Raw("{0}", (object) ex.ToString());
      }
      catch (Exception ex1)
      {
        Trace.WriteLine("Exception in Tracer!\n{0}\n", ex1.Message);
      }
    }

    public static void Raw(string format, params object[] args) => Tracer.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));

    private static void WriteLine(string line)
    {
      Trace.WriteLine(line);
      if (TraceWriter.Instance == null)
        return;
      TraceWriter.Instance.Write(line);
    }
  }
}
