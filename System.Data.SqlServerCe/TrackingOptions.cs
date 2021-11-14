// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.TrackingOptions
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  public enum TrackingOptions
  {
    None = 0,
    Insert = 1,
    Update = 2,
    Delete = 4,
    All = Delete | Update | Insert, // 0x00000007
    Max = 8,
  }
}
