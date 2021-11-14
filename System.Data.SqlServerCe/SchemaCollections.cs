// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SchemaCollections
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace System.Data.SqlServerCe
{
  internal static class SchemaCollections
  {
    private const string PopulationMechanism = "PopulationMechanism";
    private const string PopulationString = "PopulationString";
    private const string CustomCollection = "CustomCollection";
    private const string XMLResource = "XMLResource";
    private const string SQLStatement = "SQLStatement";
    private const string RestrictionName = "RestrictionName";
    private const string ParameterName = "ParameterName";
    private const string RestrictionDefault = "RestrictionDefault";
    private const string RestrictionNumber = "RestrictionNumber";
    private static DataTable metadataCollections;
    private static DataTable restrictionsTable;
    private static readonly Dictionary<string, string> DataTypesExtraInfo;

    static SchemaCollections()
    {
      KillBitHelper.ThrowIfKillBitIsSet();
      SchemaCollections.metadataCollections = new DataTable("");
      int num1 = (int) SchemaCollections.metadataCollections.ReadXml(Assembly.GetExecutingAssembly().GetManifestResourceStream("MetaDataCollections.xml"));
      SchemaCollections.restrictionsTable = new DataTable();
      int num2 = (int) SchemaCollections.restrictionsTable.ReadXml(Assembly.GetExecutingAssembly().GetManifestResourceStream("Restrictions.xml"));
      SchemaCollections.DataTypesExtraInfo = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      SchemaCollections.DataTypesExtraInfo.Add("smallint", "smallint#System.Int16");
      SchemaCollections.DataTypesExtraInfo.Add("int", "int#System.Int32");
      SchemaCollections.DataTypesExtraInfo.Add("real", "real#System.Single");
      SchemaCollections.DataTypesExtraInfo.Add("float", "float#System.Double");
      SchemaCollections.DataTypesExtraInfo.Add("money", "money#System.Decimal");
      SchemaCollections.DataTypesExtraInfo.Add("bit", "bit#System.Boolean");
      SchemaCollections.DataTypesExtraInfo.Add("tinyint", "tinyint#System.SByte");
      SchemaCollections.DataTypesExtraInfo.Add("bigint", "bigint#System.Int64");
      SchemaCollections.DataTypesExtraInfo.Add("uniqueidentifier", "uniqueidentifier#System.Guid");
      SchemaCollections.DataTypesExtraInfo.Add("varbinary", "varbinary({0})#System.Byte[]");
      SchemaCollections.DataTypesExtraInfo.Add("binary", "binary({0})#System.Byte[]");
      SchemaCollections.DataTypesExtraInfo.Add("image", "image#System.Byte[]");
      SchemaCollections.DataTypesExtraInfo.Add("nvarchar", "nvarchar({0})#System.String");
      SchemaCollections.DataTypesExtraInfo.Add("nchar", "nchar({0})#System.String");
      SchemaCollections.DataTypesExtraInfo.Add("ntext", "ntext#System.String");
      SchemaCollections.DataTypesExtraInfo.Add("numeric", "numeric({0}, {1})#System.Decimal");
      SchemaCollections.DataTypesExtraInfo.Add("datetime", "datetime#System.DateTime");
      SchemaCollections.DataTypesExtraInfo.Add("rowversion", "timestamp#System.Byte[]");
    }

    internal static DataTable GetSchema(SqlCeConnection conn) => SchemaCollections.GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);

    internal static DataTable GetSchema(SqlCeConnection conn, string schemaName) => SchemaCollections.GetSchema(conn, schemaName, (string[]) null);

    internal static DataTable GetSchema(
      SqlCeConnection conn,
      string schemaName,
      string[] restrictions)
    {
      DataRow collectionRow = (DataRow) null;
      if (conn.State != ConnectionState.Open)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Res.GetString("SQLCE_ConnectionNotOpened")));
      foreach (DataRow row in (InternalDataCollectionBase) SchemaCollections.metadataCollections.Rows)
      {
        if (((string) row[DbMetaDataColumnNames.CollectionName]).Equals(schemaName, StringComparison.OrdinalIgnoreCase))
        {
          collectionRow = row;
          break;
        }
      }
      if (collectionRow == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Res.GetString("SQLCE_SchemaCollectionNotDefined"), (object) schemaName));
      if (((string) collectionRow["PopulationMechanism"]).Equals("CustomCollection", StringComparison.OrdinalIgnoreCase))
        return SchemaCollections.GetCustomCollections(conn, collectionRow, restrictions);
      if (((string) collectionRow["PopulationMechanism"]).Equals("XMLResource", StringComparison.OrdinalIgnoreCase))
        return SchemaCollections.GetXMLResourceCollections(conn, collectionRow, restrictions);
      if (((string) collectionRow["PopulationMechanism"]).Equals("SQLStatement", StringComparison.OrdinalIgnoreCase))
        return SchemaCollections.GetSqlStatementCollections(conn, collectionRow, restrictions);
      throw new InvalidOperationException(Res.GetString("SQLCE_InternalErrorInGetSchema"));
    }

    private static DataTable GetCustomCollections(
      SqlCeConnection conn,
      DataRow collectionRow,
      string[] restrictions)
    {
      DataTable dataTable = new DataTable();
      string str = (string) collectionRow[DbMetaDataColumnNames.CollectionName];
      if (!SchemaCollections.IsNullOrEmptyArray((Array) restrictions))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Res.GetString("SQLCE_RestrictionsMismatch"), (object) str));
      if (str.Equals(DbMetaDataCollectionNames.MetaDataCollections, StringComparison.OrdinalIgnoreCase))
      {
        dataTable = SchemaCollections.metadataCollections.Copy();
        dataTable.Columns.Remove("PopulationMechanism");
        dataTable.Columns.Remove("PopulationString");
      }
      else if (str.Equals(DbMetaDataCollectionNames.DataSourceInformation, StringComparison.OrdinalIgnoreCase))
      {
        dataTable = SchemaCollections.GetXMLResourceCollections(conn, collectionRow, restrictions);
        dataTable.Rows[0][DbMetaDataColumnNames.DataSourceProductName] = (object) "Microsoft® SQL Server® Compact";
        Version version = new Version("4.0.8876.1");
        dataTable.Rows[0][DbMetaDataColumnNames.DataSourceProductVersion] = (object) version.ToString(4);
        dataTable.Rows[0][DbMetaDataColumnNames.DataSourceProductVersionNormalized] = (object) (version.Major.ToString("d2") + "." + version.Minor.ToString("d2") + "." + version.Build.ToString("d4") + "." + version.Revision.ToString("d2"));
      }
      else if (str.Equals(DbMetaDataCollectionNames.DataTypes, StringComparison.OrdinalIgnoreCase))
      {
        dataTable = SchemaCollections.GetSqlStatementCollections(conn, collectionRow, restrictions);
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          string key = (string) row[DbMetaDataColumnNames.TypeName];
          string[] strArray = SchemaCollections.DataTypesExtraInfo[key].Split("#".ToCharArray());
          row[DbMetaDataColumnNames.CreateFormat] = (object) strArray[0];
          row[DbMetaDataColumnNames.DataType] = (object) strArray[1];
        }
      }
      return dataTable != null ? dataTable : throw new InvalidOperationException(Res.GetString("SQLCE_InternalErrorInGetSchema"));
    }

    private static DataTable GetXMLResourceCollections(
      SqlCeConnection conn,
      DataRow collectionRow,
      string[] restrictions)
    {
      DataTable dataTable = new DataTable("");
      string str = (string) collectionRow[DbMetaDataColumnNames.CollectionName];
      if (!SchemaCollections.IsNullOrEmptyArray((Array) restrictions))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Res.GetString("SQLCE_RestrictionsMismatch"), (object) str));
      int num = (int) dataTable.ReadXml(Assembly.GetExecutingAssembly().GetManifestResourceStream((string) collectionRow["PopulationString"]));
      return dataTable;
    }

    private static DataTable GetSqlStatementCollections(
      SqlCeConnection conn,
      DataRow collectionRow,
      string[] restrictions)
    {
      string tableName = (string) collectionRow[DbMetaDataColumnNames.CollectionName];
      int num = (int) collectionRow[DbMetaDataColumnNames.NumberOfRestrictions];
      DataTable dataTable = new DataTable(tableName);
      if (!SchemaCollections.IsNullOrEmptyArray((Array) restrictions) && num < restrictions.Length)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Res.GetString("SQLCE_RestrictionsMismatch"), (object) tableName));
      Dictionary<int, DataRow> dictionary = new Dictionary<int, DataRow>();
      SqlCeCommand sqlCeCommand = new SqlCeCommand((string) collectionRow["PopulationString"], conn);
      foreach (DataRow row in (InternalDataCollectionBase) SchemaCollections.restrictionsTable.Rows)
      {
        if (((string) row[DbMetaDataColumnNames.CollectionName]).Equals(tableName, StringComparison.OrdinalIgnoreCase))
          dictionary.Add((int) row["RestrictionNumber"], row);
      }
      for (int key = 1; key <= dictionary.Keys.Count; ++key)
      {
        object restriction = (object) DBNull.Value;
        if (!SchemaCollections.IsNullOrEmptyArray((Array) restrictions) && key <= restrictions.Length && restrictions[key - 1] != null)
          restriction = (object) restrictions[key - 1];
        string parameterName = (string) dictionary[key]["ParameterName"];
        sqlCeCommand.Parameters.Add(parameterName, SqlDbType.NVarChar);
        sqlCeCommand.Parameters[parameterName].Value = restriction;
      }
      new SqlCeDataAdapter()
      {
        SelectCommand = sqlCeCommand
      }.Fill(dataTable);
      return dataTable;
    }

    private static bool IsNullOrEmptyArray(Array array) => array == null || array.Length == 0;
  }
}
