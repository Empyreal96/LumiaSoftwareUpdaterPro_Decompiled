// Decompiled with JetBrains decompiler
// Type: FFUComponents.UsbRequest
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  [Flags]
  internal enum UsbRequest
  {
    DeviceToHost = 128, // 0x00000080
    HostToDevice = 0,
    Standard = 0,
    Class = 32, // 0x00000020
    Vendor = 64, // 0x00000040
    Reserved = Vendor | Class, // 0x00000060
    ForDevice = 0,
    ForInterface = 1,
    ForEndpoint = 2,
    ForOther = ForEndpoint | ForInterface, // 0x00000003
  }
}
