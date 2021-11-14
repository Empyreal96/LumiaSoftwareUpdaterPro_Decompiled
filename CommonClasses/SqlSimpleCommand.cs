// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SqlSimpleCommand
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Data.SqlServerCe;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class SqlSimpleCommand : SqlCommandBase
  {
    public SqlSimpleCommand(string tableName, SqlCeConnection connection)
      : base(tableName, connection)
    {
    }

    public int Delete(string fieldName, object fieldValue)
    {
      string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}=@{1}", (object) this.TableName, (object) fieldName);
      this.AddParameter(fieldName, fieldValue);
      return this.ExecuteNonQuery(commandText);
    }

    public int DeleteObsolete(string fieldName, object fieldValue)
    {
      string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}=@{1} AND {2}=@{2} AND {3}=@{3} AND {4}<=@{4}", (object) this.TableName, (object) fieldName, (object) "Online", (object) "VplPath", (object) "Timestamp");
      this.AddParameter(fieldName, fieldValue);
      this.AddParameter("Online", (object) true);
      this.AddParameter("VplPath", (object) string.Empty);
      this.AddParameter("Timestamp", (object) DateTime.Now.AddHours(-1.0));
      return this.ExecuteNonQuery(commandText);
    }

    public int DeleteObsoleteFile(string fieldName, object fieldValue)
    {
      string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}=@{1} AND {2}=@{2}", (object) this.TableName, (object) fieldName, (object) "Timestamp");
      this.AddParameter(fieldName, fieldValue);
      this.AddParameter("Timestamp", (object) DateTime.Now.AddHours(-1.0));
      return this.ExecuteNonQuery(commandText);
    }

    public int DeleteAll(string fieldName, object fieldValue)
    {
      string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}=@{1} AND {2}=@{2}", (object) this.TableName, (object) fieldName, (object) "Online");
      this.AddParameter(fieldName, fieldValue);
      this.AddParameter("Online", (object) true);
      return this.ExecuteNonQuery(commandText);
    }

    public int DeleteAllFile(string fieldName, object fieldValue)
    {
      string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}=@{1}", (object) this.TableName, (object) fieldName);
      this.AddParameter(fieldName, fieldValue);
      return this.ExecuteNonQuery(commandText);
    }

    public int GetRowCount() => (int) this.ExecuteScalar(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT COUNT(*) FROM {0}", (object) this.TableName));

    public int GetRowCount(string fieldName, object fieldValue)
    {
      string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT COUNT(*) FROM {0} WHERE {1}=@{1}", (object) this.TableName, (object) fieldName);
      this.AddParameter(fieldName, fieldValue);
      return (int) this.ExecuteScalar(commandText);
    }
  }
}
