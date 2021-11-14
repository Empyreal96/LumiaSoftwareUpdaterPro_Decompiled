// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.ThreadExceptionEventArgs
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Properties;
using System;
using System.Threading;

namespace Nokia.Lucid.DeviceDetection
{
  public sealed class ThreadExceptionEventArgs : EventArgs
  {
    private const int NotHandled = 0;
    private const int Handled = 1;
    private readonly Exception exception;
    private int handled;

    public ThreadExceptionEventArgs(Exception exception) => this.exception = exception != null ? exception : throw new ArgumentNullException(nameof (exception));

    public Exception Exception => this.exception;

    public bool IsHandled => this.handled == 1;

    public void SetHandled()
    {
      if (Interlocked.CompareExchange(ref this.handled, 1, 0) != 0)
        throw new InvalidOperationException(Resources.InvalidOperationException_MessageText_ExceptionAlreadyMarkedAsHandled);
    }
  }
}
