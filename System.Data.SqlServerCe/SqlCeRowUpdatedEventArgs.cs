// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeRowUpdatedEventArgs
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.Common;

namespace System.Data.SqlServerCe
{
  public sealed class SqlCeRowUpdatedEventArgs : RowUpdatedEventArgs
  {
    static SqlCeRowUpdatedEventArgs() => KillBitHelper.ThrowIfKillBitIsSet();

    public SqlCeRowUpdatedEventArgs(
      DataRow dataRow,
      IDbCommand command,
      StatementType statementType,
      DataTableMapping tableMapping)
      : base(dataRow, command, statementType, tableMapping)
    {
    }

    public SqlCeCommand Command => (SqlCeCommand) base.Command;
  }
}
