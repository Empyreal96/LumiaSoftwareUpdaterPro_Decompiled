// Decompiled with JetBrains decompiler
// Type: FFUComponents.TimeoutHelper
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Diagnostics;

namespace FFUComponents
{
  internal class TimeoutHelper
  {
    private TimeSpan timeout;
    private Stopwatch stopWatch;

    public TimeoutHelper(int timeoutMilliseconds)
      : this(TimeSpan.FromMilliseconds((double) timeoutMilliseconds))
    {
    }

    public TimeoutHelper(TimeSpan timeout)
    {
      this.timeout = timeout;
      this.stopWatch = Stopwatch.StartNew();
    }

    public bool HasExpired => this.stopWatch.Elapsed > this.timeout;

    public TimeSpan Remaining => this.timeout - this.stopWatch.Elapsed;

    public TimeSpan Elapsed => this.stopWatch.Elapsed;
  }
}
