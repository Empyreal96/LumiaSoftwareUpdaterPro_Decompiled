// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SqlSelectFilesCommand
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class SqlSelectFilesCommand : SqlCommandBase
  {
    public SqlSelectFilesCommand(SqlCeConnection connection)
      : base("UpdatePackageFile", connection)
    {
    }

    public UpdatePackageFile[] Select(string uniqueId)
    {
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      try
      {
        sqlCeDataReader = this.ExecuteReader(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE VariantPackageUniqueId='{1}'", (object) this.TableName, (object) uniqueId));
        List<UpdatePackageFile> updatePackageFileList = new List<UpdatePackageFile>();
        while (sqlCeDataReader.Read())
          updatePackageFileList.Add(new UpdatePackageFile(sqlCeDataReader["ProductType"] as string, sqlCeDataReader["FileName"] as string, sqlCeDataReader["RelativePath"] as string, (long) sqlCeDataReader["FileSize"], (DateTime) sqlCeDataReader["Timestamp"]));
        return updatePackageFileList.ToArray();
      }
      finally
      {
        sqlCeDataReader?.Close();
      }
    }
  }
}
