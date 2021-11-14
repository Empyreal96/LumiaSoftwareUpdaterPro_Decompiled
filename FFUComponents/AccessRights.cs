// Decompiled with JetBrains decompiler
// Type: FFUComponents.AccessRights
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  [Flags]
  internal enum AccessRights : uint
  {
    Read = 2147483648, // 0x80000000
    Write = 1073741824, // 0x40000000
    Execute = 536870912, // 0x20000000
    All = 268435456, // 0x10000000
  }
}
