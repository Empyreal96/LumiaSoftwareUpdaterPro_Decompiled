// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.SystemPowerState
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro.Helpers
{
  public class SystemPowerState
  {
    public bool? OnBatteryPower { get; internal set; }

    public int RemainingBatteryPercentage { get; internal set; }

    public TimeSpan RemainingBatteryLifetime { get; internal set; }

    public TimeSpan FullBatteryLifetime { get; internal set; }

    public BatteryChargeStatus BatteryChargeStatus { get; internal set; }
  }
}
