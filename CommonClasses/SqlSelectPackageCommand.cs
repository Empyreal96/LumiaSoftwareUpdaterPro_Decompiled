// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SqlSelectPackageCommand
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class SqlSelectPackageCommand : SqlCommandBase
  {
    private string where = string.Empty;

    public SqlSelectPackageCommand(SqlCeConnection connection)
      : base("UpdatePackage", connection)
    {
    }

    public void AddParameter(string name, object value, bool add)
    {
      if (!add)
        return;
      this.AddParameter(name, value);
    }

    public override void AddParameter(string name, object value)
    {
      if (!string.IsNullOrEmpty(this.where))
        this.where += " AND ";
      this.where += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}=@{0}", (object) name);
      base.AddParameter(name, value);
    }

    public void AddOnline(bool online)
    {
      string empty = string.Empty;
      if (!string.IsNullOrEmpty(this.where))
        this.where += " AND ";
      string str = !online ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Online='false'") : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(Online='true' OR Online='false')");
      this.where += str;
      base.AddParameter("Online", (object) str.ToLowerInvariant());
    }

    public void AddSourcePaths(IList<string> sourcePath, bool online)
    {
      string empty = string.Empty;
      if (!string.IsNullOrEmpty(this.where))
        this.where += " AND ";
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) sourcePath);
      if (online)
        stringList.Add(string.Empty);
      foreach (string str1 in stringList)
      {
        string str2 = str1.Replace("'", "''");
        empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SourcePath='{0}' OR ", (object) str2);
      }
      if (string.IsNullOrEmpty(empty))
        return;
      string str3 = empty.Remove(empty.Length - 4, 4).Insert(0, "(");
      string str4 = str3.Insert(str3.Length, ")");
      this.where += str4;
      base.AddParameter("SourcePath", (object) str4.ToLowerInvariant());
    }

    public UpdatePackage SelectFirst()
    {
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      try
      {
        sqlCeDataReader = this.ExecuteReader(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT TOP (1) * FROM {0} {1} ORDER BY NormalizedSoftwareVersion DESC, VariantVersion DESC, Online ASC", (object) this.TableName, (object) this.GetWhere()));
        return sqlCeDataReader.Read() ? this.ReadPackage(sqlCeDataReader) : (UpdatePackage) null;
      }
      finally
      {
        sqlCeDataReader?.Close();
      }
    }

    public UpdatePackage[] Select()
    {
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      try
      {
        sqlCeDataReader = this.ExecuteReader(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT * FROM {0} {1} ORDER BY ProductType ASC, ProductCode ASC, NormalizedSoftwareVersion DESC, VariantVersion DESC, Online ASC", (object) this.TableName, (object) this.GetWhere()));
        List<UpdatePackage> updatePackageList = new List<UpdatePackage>();
        while (sqlCeDataReader.Read())
          updatePackageList.Add(this.ReadPackage(sqlCeDataReader));
        return updatePackageList.ToArray();
      }
      finally
      {
        sqlCeDataReader?.Close();
      }
    }

    public string[] SelectDistinctProductTypes()
    {
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      try
      {
        sqlCeDataReader = this.ExecuteReader(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT * FROM {0} INNER JOIN (SELECT {1}, MIN(Id) AS Id FROM {0} {2} GROUP BY {1}) AS b ON {0}.Id = b.Id ORDER BY {0}.ProductType ASC, {0}.ProductCode ASC, {0}.NormalizedSoftwareVersion DESC, {0}.VariantVersion DESC, {0}.Online ASC", (object) this.TableName, (object) "ProductType", (object) this.GetWhere()));
        List<string> stringList = new List<string>();
        while (sqlCeDataReader.Read())
          stringList.Add(this.ReadPackage(sqlCeDataReader).ProductType);
        return stringList.ToArray();
      }
      finally
      {
        sqlCeDataReader?.Close();
      }
    }

    public UpdatePackage[] SelectDistinct(string distinctField)
    {
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      try
      {
        sqlCeDataReader = this.ExecuteReader(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT * FROM {0} INNER JOIN (SELECT {1}, MIN(Id) AS Id FROM {0} {2} GROUP BY {1}) AS b ON {0}.Id = b.Id ORDER BY {0}.ProductType ASC, {0}.ProductCode ASC, {0}.NormalizedSoftwareVersion DESC, {0}.VariantVersion DESC, {0}.Online ASC", (object) this.TableName, (object) distinctField, (object) this.GetWhere()));
        List<UpdatePackage> updatePackageList = new List<UpdatePackage>();
        while (sqlCeDataReader.Read())
          updatePackageList.Add(this.ReadPackage(sqlCeDataReader));
        return updatePackageList.ToArray();
      }
      finally
      {
        sqlCeDataReader?.Close();
      }
    }

    public int GetRowCount()
    {
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      try
      {
        string commandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT COUNT(*) FROM {0} {1}", (object) this.TableName, (object) this.GetWhere());
        sqlCeDataReader = this.ExecuteReader(commandText);
        return (int) this.ExecuteScalar(commandText);
      }
      finally
      {
        sqlCeDataReader?.Close();
      }
    }

    private string GetWhere() => !string.IsNullOrEmpty(this.where) ? "WHERE " + this.where : string.Empty;

    private UpdatePackage ReadPackage(SqlCeDataReader sqlCeDataReader)
    {
      string uniqueId = sqlCeDataReader["UniqueId"] as string;
      UpdatePackageFile[] files = this.ReadFiles(uniqueId);
      return new UpdatePackage(sqlCeDataReader["ProductType"] as string, sqlCeDataReader["ProductCode"] as string, sqlCeDataReader["SoftwareVersion"] as string, sqlCeDataReader["VariantVersion"] as string, sqlCeDataReader["VariantDescription"] as string, sqlCeDataReader["PackageUsePurpose"] as string, sqlCeDataReader["PackageUseType"] as string, sqlCeDataReader["BuildType"] as string, sqlCeDataReader["OsVersion"] as string, sqlCeDataReader["AkVersion"] as string, sqlCeDataReader["BspVersion"] as string, sqlCeDataReader["VplPath"] as string, uniqueId, sqlCeDataReader["SourcePath"] as string, (bool) sqlCeDataReader["Online"], (DateTime) sqlCeDataReader["Timestamp"], files);
    }

    private UpdatePackageFile[] ReadFiles(string uniqueId)
    {
      try
      {
        return new SqlSelectFilesCommand(this.Connection).Select(uniqueId);
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex.Message);
        return new UpdatePackageFile[0];
      }
    }
  }
}
