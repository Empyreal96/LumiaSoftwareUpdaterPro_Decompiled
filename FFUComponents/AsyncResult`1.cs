// Decompiled with JetBrains decompiler
// Type: FFUComponents.AsyncResult`1
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  internal class AsyncResult<TResult> : AsyncResultNoResult
  {
    private TResult result = default (TResult);

    public AsyncResult(AsyncCallback asyncCallback, object state)
      : base(asyncCallback, state)
    {
    }

    public void SetAsCompleted(TResult result, bool completedSynchronously)
    {
      this.result = result;
      this.SetAsCompleted((Exception) null, completedSynchronously);
    }

    public TResult EndInvoke()
    {
      base.EndInvoke();
      return this.result;
    }
  }
}
