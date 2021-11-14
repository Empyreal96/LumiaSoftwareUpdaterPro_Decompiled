// Decompiled with JetBrains decompiler
// Type: FFUComponents.DTSFUsbStreamReadAsyncResult
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  internal class DTSFUsbStreamReadAsyncResult : AsyncResult<int>
  {
    public DTSFUsbStreamReadAsyncResult(AsyncCallback callback, object state)
      : base(callback, state)
    {
    }

    public byte[] Buffer { get; set; }

    public int Offset { get; set; }

    public int Count { get; set; }

    public int RetryCount { get; set; }
  }
}
