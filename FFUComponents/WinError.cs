// Decompiled with JetBrains decompiler
// Type: FFUComponents.WinError
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

namespace FFUComponents
{
  internal enum WinError : uint
  {
    Success = 0,
    FileNotFound = 2,
    PathNotFound = 3,
    NoMoreFiles = 18, // 0x00000012
    NotReady = 21, // 0x00000015
    GeneralFailure = 31, // 0x0000001F
    InvalidParameter = 87, // 0x00000057
    SemTimeout = 121, // 0x00000079
    InsufficientBuffer = 122, // 0x0000007A
    AlreadyExists = 183, // 0x000000B7
    WaitTimeout = 258, // 0x00000102
    NoMoreItems = 259, // 0x00000103
    OperationAborted = 995, // 0x000003E3
    IoPending = 997, // 0x000003E5
    DeviceNotConnected = 1167, // 0x0000048F
    InvalidHandleValue = 4294967295, // 0xFFFFFFFF
    TimeZoneIdInvalid = 4294967295, // 0xFFFFFFFF
  }
}
