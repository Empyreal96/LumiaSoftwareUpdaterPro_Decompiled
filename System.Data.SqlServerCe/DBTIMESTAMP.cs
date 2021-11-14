// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.DBTIMESTAMP
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal struct DBTIMESTAMP
  {
    public short year;
    public ushort month;
    public ushort day;
    public ushort hour;
    public ushort minute;
    public ushort second;
    public uint fraction;
  }
}
