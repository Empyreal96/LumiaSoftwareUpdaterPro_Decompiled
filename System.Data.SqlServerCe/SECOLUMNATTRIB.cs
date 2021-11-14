// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SECOLUMNATTRIB
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum SECOLUMNATTRIB
  {
    NAME = 1,
    IDCOL = 2,
    IDRANGE = 4,
    WRITEABLE = 16, // 0x00000010
    NULLABLE = 32, // 0x00000020
    TYPE = 64, // 0x00000040
    IDENTITY = 128, // 0x00000080
    IDNEXT = 256, // 0x00000100
    SYSCOL = 512, // 0x00000200
    IDRANGE1 = 1024, // 0x00000400
    IDRANGE2 = 2048, // 0x00000800
  }
}
