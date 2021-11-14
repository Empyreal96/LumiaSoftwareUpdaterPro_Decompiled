// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SqlCommandBase
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;
using System.Data.SqlServerCe;

namespace Microsoft.LsuPro
{
  public class SqlCommandBase
  {
    private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

    protected string TableName { get; private set; }

    protected SqlCeConnection Connection { get; private set; }

    protected SqlCommandBase(string tableName, SqlCeConnection connection)
    {
      this.TableName = tableName;
      this.Connection = connection;
    }

    public virtual void AddParameter(string name, object value) => this.parameters.Add(name, value);

    protected int ExecuteNonQuery(string commandText) => this.GetCommand(commandText).ExecuteNonQuery();

    protected object ExecuteScalar(string commandText) => this.GetCommand(commandText).ExecuteScalar();

    protected SqlCeDataReader ExecuteReader(string commandText) => this.GetCommand(commandText).ExecuteReader();

    private SqlCeCommand GetCommand(string commandText)
    {
      SqlCeCommand command = this.Connection.CreateCommand();
      command.CommandText = commandText;
      foreach (KeyValuePair<string, object> parameter in this.parameters)
        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
      return command;
    }
  }
}
