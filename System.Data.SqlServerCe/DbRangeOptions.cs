// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.DbRangeOptions
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  public enum DbRangeOptions
  {
    InclusiveStart = 0,
    InclusiveEnd = 0,
    ExclusiveStart = 1,
    ExclusiveEnd = 2,
    ExcludeNulls = 4,
    Prefix = 8,
    Match = 16, // 0x00000010
    Default = 0,
  }
}
