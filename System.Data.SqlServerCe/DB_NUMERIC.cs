// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.DB_NUMERIC
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal struct DB_NUMERIC
  {
    public byte precision;
    public byte scale;
    public byte sign;
    public unsafe fixed byte val[16];
  }
}
