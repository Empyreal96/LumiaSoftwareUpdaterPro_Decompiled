// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeDataAdapter
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.Common;

namespace System.Data.SqlServerCe
{
  public sealed class SqlCeDataAdapter : DbDataAdapter, ICloneable
  {
    static SqlCeDataAdapter() => KillBitHelper.ThrowIfKillBitIsSet();

    public SqlCeDataAdapter() => GC.SuppressFinalize((object) this);

    public SqlCeDataAdapter(SqlCeCommand selectCommand)
    {
      GC.SuppressFinalize((object) this);
      this.SelectCommand = selectCommand;
    }

    public SqlCeDataAdapter(string selectCommandText, string selectConnectionString)
    {
      GC.SuppressFinalize((object) this);
      this.SelectCommand = new SqlCeCommand(selectCommandText, new SqlCeConnection(selectConnectionString));
    }

    public new void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.DeleteCommand = (DbCommand) null;
      this.SelectCommand = (DbCommand) null;
      this.InsertCommand = (DbCommand) null;
      this.UpdateCommand = (DbCommand) null;
    }

    public SqlCeDataAdapter(string selectCommandText, SqlCeConnection selectConnection)
    {
      GC.SuppressFinalize((object) this);
      this.SelectCommand = new SqlCeCommand(selectCommandText, selectConnection);
    }

    private SqlCeDataAdapter(SqlCeDataAdapter from)
      : base((DbDataAdapter) from)
    {
      GC.SuppressFinalize((object) this);
    }

    public SqlCeCommand DeleteCommand
    {
      get => base.DeleteCommand as SqlCeCommand;
      set => this.DeleteCommand = (DbCommand) value;
    }

    public SqlCeCommand InsertCommand
    {
      get => base.InsertCommand as SqlCeCommand;
      set => this.InsertCommand = (DbCommand) value;
    }

    public SqlCeCommand SelectCommand
    {
      get => base.SelectCommand as SqlCeCommand;
      set => this.SelectCommand = (DbCommand) value;
    }

    public SqlCeCommand UpdateCommand
    {
      get => base.UpdateCommand as SqlCeCommand;
      set => this.UpdateCommand = (DbCommand) value;
    }

    public event SqlCeRowUpdatedEventHandler RowUpdated
    {
      add => this.Events.AddHandler(ADP.EventRowUpdated, (Delegate) value);
      remove => this.Events.RemoveHandler(ADP.EventRowUpdated, (Delegate) value);
    }

    public event SqlCeRowUpdatingEventHandler RowUpdating
    {
      add => this.Events.AddHandler(ADP.EventRowUpdating, (Delegate) value);
      remove => this.Events.RemoveHandler(ADP.EventRowUpdating, (Delegate) value);
    }

    object ICloneable.Clone() => (object) new SqlCeDataAdapter(this);

    protected override RowUpdatedEventArgs CreateRowUpdatedEvent(
      DataRow dataRow,
      IDbCommand command,
      StatementType statementType,
      DataTableMapping tableMapping)
    {
      return (RowUpdatedEventArgs) new SqlCeRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
    }

    protected override RowUpdatingEventArgs CreateRowUpdatingEvent(
      DataRow dataRow,
      IDbCommand command,
      StatementType statementType,
      DataTableMapping tableMapping)
    {
      return (RowUpdatingEventArgs) new SqlCeRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
    }

    protected override void OnRowUpdated(RowUpdatedEventArgs value)
    {
      SqlCeRowUpdatedEventHandler updatedEventHandler = (SqlCeRowUpdatedEventHandler) this.Events[ADP.EventRowUpdated];
      if (updatedEventHandler == null || !(value is SqlCeRowUpdatedEventArgs))
        return;
      updatedEventHandler((object) this, (SqlCeRowUpdatedEventArgs) value);
    }

    protected override void OnRowUpdating(RowUpdatingEventArgs value)
    {
      SqlCeRowUpdatingEventHandler updatingEventHandler = (SqlCeRowUpdatingEventHandler) this.Events[ADP.EventRowUpdating];
      if (updatingEventHandler == null || !(value is SqlCeRowUpdatingEventArgs))
        return;
      updatingEventHandler((object) this, (SqlCeRowUpdatingEventArgs) value);
    }
  }
}
