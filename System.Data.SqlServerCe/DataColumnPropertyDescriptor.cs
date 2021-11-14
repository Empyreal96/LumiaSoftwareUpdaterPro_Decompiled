// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.DataColumnPropertyDescriptor
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.ComponentModel;

namespace System.Data.SqlServerCe
{
  internal class DataColumnPropertyDescriptor : PropertyDescriptor
  {
    private int ordinal;
    private bool isReadOnly;
    private SqlCeResultSet parent;

    static DataColumnPropertyDescriptor() => KillBitHelper.ThrowIfKillBitIsSet();

    internal DataColumnPropertyDescriptor(int ordinal, SqlCeResultSet resultSet, bool isReadOnly)
      : base(resultSet.GetSqlMetaData(ordinal).Name, (Attribute[]) null)
    {
      this.isReadOnly = isReadOnly;
      this.ordinal = ordinal;
      this.parent = resultSet;
    }

    public override Type ComponentType => typeof (RowView);

    public override bool IsReadOnly => this.isReadOnly;

    public override Type PropertyType => SqlCeType.FromSqlDbType(this.parent.GetSqlMetaData(this.ordinal).SqlDbType).clrType;

    public override bool Equals(object other) => other is DataColumnPropertyDescriptor && ((DataColumnPropertyDescriptor) other).parent == this.parent;

    public override int GetHashCode() => this.parent.GetHashCode();

    public override bool CanResetValue(object component) => this.isReadOnly;

    public override object GetValue(object component)
    {
      SqlCeUpdatableRecord updatableRecord = ((RowView) component).UpdatableRecord;
      if (updatableRecord == null)
        return (object) DBNull.Value;
      return updatableRecord.IsDBNull(this.ordinal) ? (object) DBNull.Value : updatableRecord.GetValue(this.ordinal);
    }

    public override void ResetValue(object component)
    {
      ((RowView) component).UpdatableRecord.SetValue(this.ordinal, (object) DBNull.Value);
      this.OnValueChanged(component, EventArgs.Empty);
    }

    public override void SetValue(object component, object value)
    {
      ((RowView) component).UpdatableRecord.SetValue(this.ordinal, value);
      this.OnValueChanged(component, EventArgs.Empty);
    }

    public override bool ShouldSerializeValue(object component) => false;
  }
}
