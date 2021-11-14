// Decompiled with JetBrains decompiler
// Type: FFUComponents.AsyncResultNoResult
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Threading;

namespace FFUComponents
{
  internal class AsyncResultNoResult : IAsyncResult
  {
    private const int statePending = 0;
    private const int stateCompletedSynchronously = 1;
    private const int stateCompletedAsynchronously = 2;
    private readonly AsyncCallback asyncCallback;
    private readonly object asyncState;
    private int completedState;
    private ManualResetEvent asyncWaitHandle;
    private Exception exception;

    public AsyncCallback AsyncCallback => this.asyncCallback;

    public AsyncResultNoResult(AsyncCallback asyncCallback, object state)
    {
      this.asyncCallback = asyncCallback;
      this.asyncState = state;
    }

    public void SetAsCompleted(Exception exception, bool completedSynchronously)
    {
      this.exception = exception;
      if (Interlocked.Exchange(ref this.completedState, completedSynchronously ? 1 : 2) != 0)
        throw new InvalidOperationException("You can set a result only once");
      if (this.asyncWaitHandle != null)
        this.asyncWaitHandle.Set();
      if (this.asyncCallback == null)
        return;
      this.asyncCallback((IAsyncResult) this);
    }

    public void EndInvoke()
    {
      if (!this.IsCompleted)
      {
        TimeSpan timeout = TimeSpan.FromMinutes(2.0);
        try
        {
          if (!this.AsyncWaitHandle.WaitOne(timeout, false))
            throw new TimeoutException();
        }
        finally
        {
          this.AsyncWaitHandle.Close();
          this.asyncWaitHandle = (ManualResetEvent) null;
        }
      }
      if (this.exception != null)
        throw this.exception;
    }

    public object AsyncState => this.asyncState;

    public bool CompletedSynchronously => Thread.VolatileRead(ref this.completedState) == 1;

    public WaitHandle AsyncWaitHandle
    {
      get
      {
        if (this.asyncWaitHandle == null)
        {
          bool isCompleted = this.IsCompleted;
          ManualResetEvent manualResetEvent = new ManualResetEvent(isCompleted);
          if (Interlocked.CompareExchange<ManualResetEvent>(ref this.asyncWaitHandle, manualResetEvent, (ManualResetEvent) null) != null)
            manualResetEvent.Close();
          else if (!isCompleted && this.IsCompleted)
            this.asyncWaitHandle.Set();
        }
        return (WaitHandle) this.asyncWaitHandle;
      }
    }

    public bool IsCompleted => Thread.VolatileRead(ref this.completedState) != 0;
  }
}
