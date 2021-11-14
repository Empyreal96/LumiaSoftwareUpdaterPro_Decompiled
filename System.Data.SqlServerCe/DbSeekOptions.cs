// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.DbSeekOptions
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  public enum DbSeekOptions
  {
    FirstEqual = 1,
    LastEqual = 2,
    AfterEqual = 4,
    After = 8,
    BeforeEqual = 16, // 0x00000010
    Before = 32, // 0x00000020
  }
}
