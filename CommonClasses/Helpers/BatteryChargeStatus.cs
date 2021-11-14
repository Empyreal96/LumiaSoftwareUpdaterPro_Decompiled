// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.BatteryChargeStatus
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro.Helpers
{
  [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "on purpose")]
  [Flags]
  public enum BatteryChargeStatus
  {
    High = 1,
    Low = 2,
    Critical = 4,
    Charging = 8,
    NoSystemBattery = 128, // 0x00000080
    Unknown = 255, // 0x000000FF
  }
}
