// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.DbColumnFlags
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum DbColumnFlags
  {
    ISBOOKMARK = 1,
    MAYDEFER = 2,
    WRITE = 4,
    WRITEUNKNOWN = 8,
    ISFIXEDLENGTH = 16, // 0x00000010
    ISNULLABLE = 32, // 0x00000020
    MAYBENULL = 64, // 0x00000040
    ISLONG = 128, // 0x00000080
    ISROWID = 256, // 0x00000100
    ISROWVER = 512, // 0x00000200
    CACHEDEFERRED = 4096, // 0x00001000
  }
}
