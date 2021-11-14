// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SETABLEATTRIBOPS
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum SETABLEATTRIBOPS : uint
  {
    ALLOWALL = 0,
    TABLERENAME = 1,
    TABLEDELETE = 2,
    COLUMNCREATE = 4,
    COLUMNRENAME = 8,
    COLUMNDELETE = 16, // 0x00000010
    INDEXCREATE = 32, // 0x00000020
    INDEXRENAME = 64, // 0x00000040
    INDEXDELETE = 128, // 0x00000080
    CONSTRAINTCREATE = 256, // 0x00000100
    CONSTRAINTDELETE = 512, // 0x00000200
    PKCREATE = 2048, // 0x00000800
    PKRENAME = 4096, // 0x00001000
    PKDELETE = 8192, // 0x00002000
    ALTERDEFAULTS = 16384, // 0x00004000
    SWAPCOLUMNORDINALS = 32768, // 0x00008000
    ALLOWNONE = 2147483648, // 0x80000000
    NEEDEDBITS = 2147549183, // 0x8000FFFF
  }
}
