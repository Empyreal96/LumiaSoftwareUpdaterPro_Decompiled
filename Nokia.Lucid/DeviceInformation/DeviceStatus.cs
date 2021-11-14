// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.DeviceStatus
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.DeviceInformation
{
  [Flags]
  public enum DeviceStatus
  {
    None = 0,
    Present = 1,
    Default = 2,
    Removed = 4,
  }
}
