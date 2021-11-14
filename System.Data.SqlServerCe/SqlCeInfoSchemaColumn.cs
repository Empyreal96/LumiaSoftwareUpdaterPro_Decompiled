// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeInfoSchemaColumn
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal class SqlCeInfoSchemaColumn
  {
    public string ParamName;
    public string ColumnName;
    public SqlCeType SqlCeType;
    public int MaxLength;
    public object DefaultValue;
    public byte Precision;
    public byte Scale;
    public int Ordinal;

    static SqlCeInfoSchemaColumn() => KillBitHelper.ThrowIfKillBitIsSet();
  }
}
