// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.ExecutionStates
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro.Helpers
{
  [Flags]
  [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "on purpose")]
  public enum ExecutionStates : uint
  {
    Continuous = 2147483648, // 0x80000000
    SystemRequired = 1,
    DisplayRequired = 2,
    AwayModeRequired = 64, // 0x00000040
  }
}
