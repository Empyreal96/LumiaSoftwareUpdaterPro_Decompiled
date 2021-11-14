// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SEOCSTRACKOPTIONS
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum SEOCSTRACKOPTIONS
  {
    NONE = 0,
    UPSERT = 1,
    INSERTUPDATE = 2,
    DELETE = 4,
    COLUMNS = 8,
    ALL = COLUMNS | DELETE | INSERTUPDATE | UPSERT, // 0x0000000F
  }
}
