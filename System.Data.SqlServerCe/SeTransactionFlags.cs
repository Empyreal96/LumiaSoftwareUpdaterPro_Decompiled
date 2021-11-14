// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SeTransactionFlags
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum SeTransactionFlags
  {
    NOFLAGS = 0,
    SYSTEM = 1,
    GENERATEIDENTITY = 2,
    GENERATEROWGUID = 4,
    TRACK = 8,
    REPLACECOLUMN = 16, // 0x00000010
    DISABLETRIGGERS = 32, // 0x00000020
    COMPRESSEDLVSTREAM = 64, // 0x00000040
    VALIDFLAGS = COMPRESSEDLVSTREAM | DISABLETRIGGERS | REPLACECOLUMN | TRACK | GENERATEROWGUID | GENERATEIDENTITY | SYSTEM, // 0x0000007F
  }
}
