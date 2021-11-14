// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Ext7Details
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro
{
  [Flags]
  public enum Ext7Details
  {
    None = 0,
    OutsideNokiaNetwork = 1,
    SkipWriteInUse = 2,
    DevelopmentTeam = 4,
    FactoryReset = 8,
    SkipFfuIntegrity = 16, // 0x00000010
    SkipPlatformId = 32, // 0x00000020
    SkipSignature = 64, // 0x00000040
    PiaReadingDisabled = 128, // 0x00000080
    TestingTeam = 256, // 0x00000100
  }
}
