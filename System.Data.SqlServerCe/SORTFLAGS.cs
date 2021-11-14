// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SORTFLAGS
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  [Flags]
  internal enum SORTFLAGS
  {
    NORM_IGNORECASE = 1,
    NORM_IGNOREKANATYPE = 65536, // 0x00010000
    NORM_IGNOREWIDTH = 131072, // 0x00020000
  }
}
