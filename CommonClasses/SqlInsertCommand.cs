// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SqlInsertCommand
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Data.SqlServerCe;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class SqlInsertCommand : SqlCommandBase
  {
    private string parameterNames = string.Empty;
    private string parameterValues = string.Empty;

    public SqlInsertCommand(string tableName, SqlCeConnection connection)
      : base(tableName, connection)
    {
    }

    public override void AddParameter(string name, object value)
    {
      if (!string.IsNullOrEmpty(this.parameterNames))
      {
        this.parameterNames += ", ";
        this.parameterValues += ", ";
      }
      this.parameterNames += name;
      this.parameterValues = this.parameterValues + "@" + name;
      base.AddParameter(name, value);
    }

    public int Insert() => this.ExecuteNonQuery(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INSERT INTO {0} ({1}) VALUES({2})", (object) this.TableName, (object) this.parameterNames, (object) this.parameterValues));
  }
}
