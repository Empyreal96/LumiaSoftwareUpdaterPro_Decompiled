// Decompiled with JetBrains decompiler
// Type: FFUComponents.WinUsbPolicyType
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

namespace FFUComponents
{
  internal enum WinUsbPolicyType : uint
  {
    ShortPacketTerminate = 1,
    AutoClearStall = 2,
    PipeTransferTimeout = 3,
    IgnoreShortPackets = 4,
    AllowPartialReads = 5,
    AutoFlush = 6,
    RawIO = 7,
  }
}
