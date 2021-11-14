// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.EntryExitLogger
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Diagnostics;

namespace Nokia.Lucid.Diagnostics
{
  public class EntryExitLogger : IDisposable
  {
    private readonly string methodName;
    private readonly TraceSource traceSource;

    private EntryExitLogger(string methodName, TraceSource source)
    {
      this.traceSource = source;
      this.methodName = methodName;
      this.traceSource.TraceEvent(TraceEventType.Start, 0, "Enter: " + this.methodName);
    }

    public static IDisposable Log(string methodName, TraceSource source)
    {
      IDisposable disposable = (IDisposable) null;
      if ((uint) source.Switch.Level > 31U)
        disposable = (IDisposable) new EntryExitLogger(methodName, source);
      return disposable;
    }

    public void Dispose() => this.traceSource.TraceEvent(TraceEventType.Stop, 0, "Exit: " + this.methodName);
  }
}
