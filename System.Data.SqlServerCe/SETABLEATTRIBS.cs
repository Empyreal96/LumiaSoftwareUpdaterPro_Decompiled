// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SETABLEATTRIBS
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum SETABLEATTRIBS
  {
    TRACKING = 1,
    GRANTEDOPS = 2,
    ISSYSTEM = 4,
    EDBTYPE = 8,
    ISREADONLY = 16, // 0x00000010
    VALIDATTRIBS = ISREADONLY | EDBTYPE | ISSYSTEM | GRANTEDOPS | TRACKING, // 0x0000001F
  }
}
