// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SqlHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "not needed")]
  public class SqlHelper
  {
    private static readonly object SynchObject = new object();
    private static readonly Dictionary<int, SqlCeConnection> Connections = new Dictionary<int, SqlCeConnection>();
    private static readonly string DatabasePath = Path.Combine(SpecialFolders.Root, "UpdatePackageManager\\UpdatePackageManagerDatabase.sdf");
    private static readonly string ConnectionString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataSource=\"{0}\"", (object) SqlHelper.DatabasePath);

    public static void CheckDatabaseStructure()
    {
      Tracer.Information("Checking database structure");
      SqlHelper sqlHelper = new SqlHelper();
      UpdatePackage updatePackage = new UpdatePackage("pt", "pc", "sv", "vv", "vd", "pup", "put", "bt", "os", "ak", "bsp", "vpl", "CheckDatabaseStructureUniqueId", "source", false, DateTime.Now, new UpdatePackageFile[1]
      {
        new UpdatePackageFile("pt", "123", "456", 789L, DateTime.Now)
      });
      try
      {
        sqlHelper.DeleteUpdatePackage(updatePackage);
        sqlHelper.WriteDataToSqlDatabase(updatePackage);
      }
      catch (Exception ex1)
      {
        Tracer.Warning(ex1.Message);
        Tracer.Warning("Delete existing database and create new one");
        try
        {
          SqlHelper.CloseAllConnections();
          if (File.Exists(SqlHelper.DatabasePath))
            File.Delete(SqlHelper.DatabasePath);
          SqlHelper.CreateSqlCeDatabase();
          Tracer.Warning("New database created");
        }
        catch (Exception ex2)
        {
          Tracer.Error(ex2.Message);
        }
      }
      finally
      {
        sqlHelper.DeleteUpdatePackage(updatePackage);
      }
    }

    private static void CloseAllConnections()
    {
      lock (SqlHelper.SynchObject)
      {
        foreach (SqlCeConnection sqlCeConnection in SqlHelper.Connections.Values)
        {
          if (ConnectionState.Open == sqlCeConnection.State)
          {
            sqlCeConnection.Close();
            sqlCeConnection.Dispose();
          }
        }
        SqlHelper.Connections.Clear();
      }
    }

    private static void CreateSqlCeDatabase()
    {
      try
      {
        if (File.Exists(SqlHelper.DatabasePath))
          return;
        if (!DirectoryHelper.DirectoryExist(SqlHelper.DatabasePath))
          DirectoryHelper.CreateDirectory(PathHelper.GetDirectoryName(SqlHelper.DatabasePath));
        using (SqlCeEngine sqlCeEngine = new SqlCeEngine(SqlHelper.ConnectionString))
        {
          sqlCeEngine.CreateDatabase();
          using (SqlCeConnection connection = new SqlCeConnection(SqlHelper.ConnectionString))
          {
            connection.Open();
            using (SqlCeCommand sqlCeCommand = new SqlCeCommand("CREATE TABLE UpdatePackage (Id INT IDENTITY NOT NULL, ProductType nvarchar (100) NOT NULL, ProductCode nvarchar (100) NOT NULL, SoftwareVersion nvarchar (100) NOT NULL, VariantVersion nvarchar (100) NOT NULL, VariantDescription nvarchar (256) NOT NULL, PackageUsePurpose nvarchar (100) NOT NULL, PackageUseType nvarchar (100) NOT NULL, BuildType nvarchar (100) NOT NULL, OsVersion nvarchar (100) NOT NULL, AkVersion nvarchar (100) NOT NULL, BspVersion nvarchar (100) NOT NULL, VplPath nvarchar (256),UniqueId nvarchar (256) NOT NULL UNIQUE, SourcePath nvarchar (256) NOT NULL, Online bit NOT NULL, NormalizedSoftwareVersion nvarchar (100), Timestamp datetime NOT NULL)", connection))
              sqlCeCommand.ExecuteNonQuery();
            using (SqlCeCommand sqlCeCommand = new SqlCeCommand("CREATE TABLE UpdatePackageFile (Id INT IDENTITY NOT NULL,VariantPackageUniqueId nvarchar (256) NOT NULL, ProductType nvarchar (100) NOT NULL, FileName nvarchar (200) NOT NULL, RelativePath nvarchar (100), FileSize bigint, Timestamp datetime NOT NULL)", connection))
              sqlCeCommand.ExecuteNonQuery();
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex, "Error creating SQL database: {0}", (object) ex.Message);
      }
    }

    public UpdatePackage[] ReadSourcePaths(bool online)
    {
      try
      {
        Tracer.Information("online={0}", (object) online);
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddOnline(online);
        return selectPackageCommand.SelectDistinct("SourcePath");
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    public string[] ReadProductTypes(bool online, IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        return selectPackageCommand.SelectDistinctProductTypes();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new string[0];
    }

    public UpdatePackage[] ReadProductCodes(
      string productType,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        List<UpdatePackage> updatePackageList = SqlHelper.SelectDistinctProductCodes(selectPackageCommand.Select());
        Tracer.Information("Started updating online status of {0} packages", (object) updatePackageList.Count);
        foreach (UpdatePackage updatePackage in updatePackageList)
          updatePackage.OnlyOnlineSiblingsAvailable = this.AreOnlyOnlineSiblingsAvailable(productType, updatePackage.ProductCode, sourcePaths);
        Tracer.Information("Finished updating online status");
        return updatePackageList.ToArray();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    private static List<UpdatePackage> SelectDistinctProductCodes(
      UpdatePackage[] updatePackages)
    {
      Dictionary<string, IList<string>> productCodesDictionary = SqlHelper.CreateProductCodesDictionary(updatePackages);
      List<UpdatePackage> updatePackageList = new List<UpdatePackage>();
      foreach (string key1 in productCodesDictionary.Keys)
      {
        string key = key1;
        UpdatePackage updatePackage1 = (UpdatePackage) null;
        try
        {
          updatePackage1 = ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.ProductCode == key && x.PackageUseType.ToLowerInvariant().Contains("retail")));
        }
        catch (Exception ex)
        {
        }
        UpdatePackage updatePackage2 = (UpdatePackage) null;
        try
        {
          updatePackage2 = ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.ProductCode == key && x.PackageUseType.ToLowerInvariant().Contains("rnd")));
        }
        catch (Exception ex)
        {
        }
        UpdatePackage updatePackage3;
        if (updatePackage1 != null && updatePackage2 != null)
        {
          updatePackage3 = updatePackage1;
          updatePackage3.PackageUseType = "retail & rnd";
        }
        else
          updatePackage3 = updatePackage1 != null || updatePackage2 != null ? (updatePackage1 == null ? updatePackage2 : updatePackage1) : ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.ProductCode == key));
        updatePackageList.Add(updatePackage3);
      }
      return updatePackageList;
    }

    private static Dictionary<string, IList<string>> CreateProductCodesDictionary(
      UpdatePackage[] updatePackages)
    {
      IEnumerable<\u003C\u003Ef__AnonymousType0<string, string>> source = ((IEnumerable<UpdatePackage>) updatePackages).GroupBy(package => new
      {
        ProductCode = package.ProductCode,
        PackageUseType = package.PackageUseType
      }).Select(grp => new
      {
        ProductCode = grp.Key.ProductCode,
        PackageUseType = grp.Key.PackageUseType
      });
      Dictionary<string, IList<string>> dictionary = new Dictionary<string, IList<string>>();
      foreach (var data in source.ToList())
      {
        if (dictionary.ContainsKey(data.ProductCode))
          dictionary[data.ProductCode].Add(data.PackageUseType);
        else
          dictionary.Add(data.ProductCode, (IList<string>) new List<string>()
          {
            data.PackageUseType
          });
      }
      return dictionary;
    }

    public IList<UpdatePackage> ReadProductCodes(
      string productType,
      string softwareVersion,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        List<UpdatePackage> updatePackageList = SqlHelper.SelectDistinctProductCodes(selectPackageCommand.Select());
        Tracer.Information("Started updating online status of {0} packages", (object) updatePackageList.Count);
        foreach (UpdatePackage updatePackage in updatePackageList)
          updatePackage.OnlyOnlineSiblingsAvailable = this.AreOnlyOnlineSiblingsAvailable(productType, updatePackage.ProductCode, softwareVersion, sourcePaths);
        Tracer.Information("Finished updating online status");
        return (IList<UpdatePackage>) updatePackageList;
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return (IList<UpdatePackage>) new UpdatePackage[0];
    }

    public IList<UpdatePackage> ReadProductCodesWinOs(
      string productType,
      string winOsVersion,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("OsVersion", (object) winOsVersion);
        List<UpdatePackage> updatePackageList = SqlHelper.SelectDistinctProductCodes(selectPackageCommand.Select());
        Tracer.Information("Started updating online status of {0} packages", (object) updatePackageList.Count);
        foreach (UpdatePackage updatePackage in updatePackageList)
          updatePackage.OnlyOnlineSiblingsAvailable = this.AreOnlyOnlineSiblingsAvailableOs(productType, updatePackage.ProductCode, winOsVersion, sourcePaths);
        Tracer.Information("Finished updating online status");
        return (IList<UpdatePackage>) updatePackageList;
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return (IList<UpdatePackage>) new UpdatePackage[0];
    }

    public UpdatePackage[] ReadSoftwareVersions(
      string productType,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        List<UpdatePackage> updatePackageList = SqlHelper.SelectDistinctSoftwareVersions(selectPackageCommand.Select());
        Tracer.Information("Started updating online status of {0} packages", (object) updatePackageList.Count);
        foreach (UpdatePackage updatePackage in updatePackageList)
          updatePackage.OnlyOnlineSiblingsAvailable = this.AreOnlyOnlineSiblingsAvailableSw(productType, updatePackage.SoftwareVersion, sourcePaths);
        Tracer.Information("Finished updating online status");
        return updatePackageList.ToArray();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    private static List<UpdatePackage> SelectDistinctSoftwareVersions(
      UpdatePackage[] updatePackages)
    {
      Dictionary<string, IList<string>> versionsDictionary = SqlHelper.CreateSoftwareVersionsDictionary(updatePackages);
      List<UpdatePackage> updatePackageList = new List<UpdatePackage>();
      foreach (string key1 in versionsDictionary.Keys)
      {
        string key = key1;
        UpdatePackage updatePackage1 = (UpdatePackage) null;
        try
        {
          updatePackage1 = ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.SoftwareVersion == key && x.PackageUseType.ToLowerInvariant().Contains("retail")));
        }
        catch (Exception ex)
        {
        }
        UpdatePackage updatePackage2 = (UpdatePackage) null;
        try
        {
          updatePackage2 = ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.SoftwareVersion == key && x.PackageUseType.ToLowerInvariant().Contains("rnd")));
        }
        catch (Exception ex)
        {
        }
        UpdatePackage updatePackage3;
        if (updatePackage1 != null && updatePackage2 != null)
        {
          updatePackage3 = updatePackage1;
          updatePackage3.PackageUseType = "retail & rnd";
        }
        else
          updatePackage3 = updatePackage1 != null || updatePackage2 != null ? (updatePackage1 == null ? updatePackage2 : updatePackage1) : ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.SoftwareVersion == key));
        updatePackageList.Add(updatePackage3);
      }
      return updatePackageList;
    }

    private static Dictionary<string, IList<string>> CreateSoftwareVersionsDictionary(
      UpdatePackage[] updatePackages)
    {
      IEnumerable<\u003C\u003Ef__AnonymousType1<string, string>> source = ((IEnumerable<UpdatePackage>) updatePackages).GroupBy(package => new
      {
        SoftwareVersion = package.SoftwareVersion,
        PackageUseType = package.PackageUseType
      }).Select(grp => new
      {
        SoftwareVersion = grp.Key.SoftwareVersion,
        PackageUseType = grp.Key.PackageUseType
      });
      Dictionary<string, IList<string>> dictionary = new Dictionary<string, IList<string>>();
      foreach (var data in source.ToList())
      {
        if (dictionary.ContainsKey(data.SoftwareVersion))
          dictionary[data.SoftwareVersion].Add(data.PackageUseType);
        else
          dictionary.Add(data.SoftwareVersion, (IList<string>) new List<string>()
          {
            data.PackageUseType
          });
      }
      return dictionary;
    }

    public UpdatePackage[] ReadWinOsVersions(
      string productType,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        List<UpdatePackage> updatePackageList = SqlHelper.SelectDistinctWinOsVersions(selectPackageCommand.Select());
        Tracer.Information("Started updating online status of {0} packages", (object) updatePackageList.Count);
        foreach (UpdatePackage updatePackage in updatePackageList)
          updatePackage.OnlyOnlineSiblingsAvailable = this.AreOnlyOnlineSiblingsAvailableSw(productType, updatePackage.SoftwareVersion, sourcePaths);
        Tracer.Information("Finished updating online status");
        return updatePackageList.ToArray();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    private static List<UpdatePackage> SelectDistinctWinOsVersions(
      UpdatePackage[] updatePackages)
    {
      Dictionary<string, IList<string>> versionsDictionary = SqlHelper.CreateWinOsVersionsDictionary(updatePackages);
      List<UpdatePackage> updatePackageList = new List<UpdatePackage>();
      foreach (string key1 in versionsDictionary.Keys)
      {
        string key = key1;
        UpdatePackage updatePackage1 = (UpdatePackage) null;
        try
        {
          updatePackage1 = ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.OsVersion == key && x.PackageUseType.ToLowerInvariant().Contains("retail")));
        }
        catch (Exception ex)
        {
        }
        UpdatePackage updatePackage2 = (UpdatePackage) null;
        try
        {
          updatePackage2 = ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.OsVersion == key && x.PackageUseType.ToLowerInvariant().Contains("rnd")));
        }
        catch (Exception ex)
        {
        }
        UpdatePackage updatePackage3;
        if (updatePackage1 != null && updatePackage2 != null)
        {
          updatePackage3 = updatePackage1;
          updatePackage3.PackageUseType = "retail & rnd";
        }
        else
          updatePackage3 = updatePackage1 != null || updatePackage2 != null ? (updatePackage1 == null ? updatePackage2 : updatePackage1) : ((IEnumerable<UpdatePackage>) updatePackages).First<UpdatePackage>((Func<UpdatePackage, bool>) (x => x.OsVersion == key));
        updatePackageList.Add(updatePackage3);
      }
      return updatePackageList;
    }

    private static Dictionary<string, IList<string>> CreateWinOsVersionsDictionary(
      UpdatePackage[] updatePackages)
    {
      IEnumerable<\u003C\u003Ef__AnonymousType2<string, string>> source = ((IEnumerable<UpdatePackage>) updatePackages).GroupBy(package => new
      {
        OsVersion = package.OsVersion,
        PackageUseType = package.PackageUseType
      }).Select(grp => new
      {
        OsVersion = grp.Key.OsVersion,
        PackageUseType = grp.Key.PackageUseType
      });
      Dictionary<string, IList<string>> dictionary = new Dictionary<string, IList<string>>();
      foreach (var data in source.ToList())
      {
        if (dictionary.ContainsKey(data.OsVersion))
          dictionary[data.OsVersion].Add(data.PackageUseType);
        else
          dictionary.Add(data.OsVersion, (IList<string>) new List<string>()
          {
            data.PackageUseType
          });
      }
      return dictionary;
    }

    public UpdatePackage[] ReadSoftwareVersions(
      string productType,
      string productCode,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        UpdatePackage[] updatePackages = selectPackageCommand.Select();
        List<UpdatePackage> updatePackageList = SqlHelper.SelectDistinctSoftwareVersions(updatePackages);
        Tracer.Information("Started updating online status of {0} packages", (object) updatePackages.Length);
        foreach (UpdatePackage updatePackage in updatePackageList)
          updatePackage.OnlyOnlineSiblingsAvailable = this.AreOnlyOnlineSiblingsAvailable(productType, productCode, updatePackage.SoftwareVersion, sourcePaths);
        Tracer.Information("Finished updating online status");
        return updatePackageList.ToArray();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    public UpdatePackage[] ReadVariantVersions(
      string productType,
      string productCode,
      string softwareVersion,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        return selectPackageCommand.SelectDistinct("VariantVersion");
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    public UpdatePackage[] ReadVariantVersionsOs(
      string productType,
      string productCode,
      string winOsVersion,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("OsVersion", (object) winOsVersion);
        return selectPackageCommand.SelectDistinct("VariantVersion");
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    public UpdatePackage ReadUpdatePackage(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        selectPackageCommand.AddParameter("VariantVersion", (object) variantVersion);
        return selectPackageCommand.SelectFirst();
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
      return (UpdatePackage) null;
    }

    public UpdatePackage ReadUpdatePackage(
      string productType,
      string uniqueId,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("UniqueId", (object) uniqueId);
        return selectPackageCommand.SelectFirst();
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
      return (UpdatePackage) null;
    }

    public UpdatePackage ReadLatestSoftwareVersion(
      string productType,
      string productCode,
      bool online,
      IList<string> sourcePaths)
    {
      try
      {
        string str;
        if (online)
        {
          if (sourcePaths.Count == 0)
          {
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AND (Online='true' OR Online='false')");
          }
          else
          {
            string empty = string.Empty;
            foreach (string sourcePath in (IEnumerable<string>) sourcePaths)
              empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SourcePath='{0}' OR ", (object) sourcePath);
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AND (Online='true' OR (Online='false' AND ({0})))", (object) empty.Remove(empty.Length - 3, 3));
          }
        }
        else
        {
          string empty = string.Empty;
          foreach (string sourcePath in (IEnumerable<string>) sourcePaths)
            empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SourcePath='{0}' OR ", (object) sourcePath);
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AND Online='false' AND ({0})", (object) empty.Remove(empty.Length - 3, 3));
        }
        return this.ReadDataFromSqlDatabase(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT TOP(1) * FROM UpdatePackage WHERE ProductType='{0}' AND ProductCode='{1}' {2}ORDER BY NormalizedSoftwareVersion DESC, VariantVersion DESC", (object) productType, (object) productCode, (object) str))[0];
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return (UpdatePackage) null;
    }

    public UpdatePackage[] ReadAllUpdatePackages(bool online)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddOnline(online);
        return selectPackageCommand.Select();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return new UpdatePackage[0];
    }

    public UpdatePackage[] ReadUpdatePackage(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        selectPackageCommand.AddParameter("VariantVersion", (object) variantVersion);
        return selectPackageCommand.Select();
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
      return new UpdatePackage[0];
    }

    public bool AreOnlyOnlineSiblingsAvailable(
      string productType,
      string productCode,
      IList<string> sourcePaths)
    {
      return this.GetRowCount(productType, productCode, sourcePaths, false) == 0;
    }

    public bool AreOnlyOnlineSiblingsAvailableSw(
      string productType,
      string softwareVersion,
      IList<string> sourcePaths)
    {
      return this.GetRowCountSw(productType, softwareVersion, sourcePaths, false) == 0;
    }

    public bool AreOnlyOnlineSiblingsAvailableOs(
      string productType,
      string productCode,
      string winOsVersion,
      IList<string> sourcePaths)
    {
      return this.GetRowCountOs(productType, productCode, winOsVersion, sourcePaths, false) == 0;
    }

    public bool AreOnlyOnlineSiblingsAvailable(
      string productType,
      string productCode,
      string softwareVersion,
      IList<string> sourcePaths)
    {
      return this.GetRowCount(productType, productCode, softwareVersion, sourcePaths, false) == 0;
    }

    public int GetRowCount(
      string productType,
      string productCode,
      IList<string> sourcePaths,
      bool online)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        return selectPackageCommand.GetRowCount();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return -1;
    }

    public int GetRowCountSw(
      string productType,
      string softwareVersion,
      IList<string> sourcePaths,
      bool online)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        return selectPackageCommand.GetRowCount();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return -1;
    }

    public int GetRowCountOs(
      string productType,
      string productCode,
      string winOsVersion,
      IList<string> sourcePaths,
      bool online)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("OSVersion", (object) winOsVersion);
        return selectPackageCommand.GetRowCount();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return -1;
    }

    public int GetRowCount(
      string productType,
      string productCode,
      string softwareVersion,
      IList<string> sourcePaths,
      bool online)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        return selectPackageCommand.GetRowCount();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return -1;
    }

    public int GetRowCount(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion,
      IList<string> sourcePaths,
      bool online)
    {
      try
      {
        SqlSelectPackageCommand selectPackageCommand = new SqlSelectPackageCommand(this.GetConnection());
        selectPackageCommand.AddSourcePaths(sourcePaths, online);
        selectPackageCommand.AddOnline(online);
        selectPackageCommand.AddParameter("ProductType", (object) productType);
        selectPackageCommand.AddParameter("ProductCode", (object) productCode);
        selectPackageCommand.AddParameter("SoftwareVersion", (object) softwareVersion);
        selectPackageCommand.AddParameter("VariantVersion", (object) variantVersion);
        return selectPackageCommand.GetRowCount();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
      }
      return -1;
    }

    public void DeleteAllUpdatePackagesForProductType(string productType)
    {
      try
      {
        new SqlSimpleCommand("UpdatePackage", this.GetConnection()).DeleteAll("ProductType", (object) productType);
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
      try
      {
        new SqlSimpleCommand("UpdatePackageFile", this.GetConnection()).DeleteAllFile("ProductType", (object) productType);
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
    }

    public void DeleteObsoleteUpdatePackagesForProductType(string productType)
    {
      try
      {
        new SqlSimpleCommand("UpdatePackage", this.GetConnection()).DeleteObsolete("ProductType", (object) productType);
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
      try
      {
        new SqlSimpleCommand("UpdatePackageFile", this.GetConnection()).DeleteObsoleteFile("ProductType", (object) productType);
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
    }

    public void DeleteUpdatePackage(UpdatePackage updatePackage)
    {
      try
      {
        new SqlSimpleCommand("UpdatePackage", this.GetConnection()).Delete("UniqueId", (object) updatePackage.UniqueId);
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
      try
      {
        new SqlSimpleCommand("UpdatePackageFile", this.GetConnection()).Delete("VariantPackageUniqueId", (object) updatePackage.UniqueId);
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
    }

    public void InsertUpdatePackage(UpdatePackage updatePackage, bool fromLocal = false)
    {
      try
      {
        this.WriteDataToSqlDatabase(updatePackage, fromLocal);
      }
      catch (DuplicateNameException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private SqlCeConnection GetConnection()
    {
      try
      {
        lock (SqlHelper.SynchObject)
        {
          int managedThreadId = Thread.CurrentThread.ManagedThreadId;
          SqlCeConnection sqlCeConnection;
          if (!SqlHelper.Connections.TryGetValue(managedThreadId, out sqlCeConnection))
          {
            Tracer.Information("Creating connection for thread {0}", (object) managedThreadId);
            sqlCeConnection = new SqlCeConnection(SqlHelper.ConnectionString);
            SqlHelper.Connections.Add(managedThreadId, sqlCeConnection);
          }
          if (sqlCeConnection.State != ConnectionState.Open)
          {
            Tracer.Information("Opening connection for thread {0}", (object) managedThreadId);
            sqlCeConnection.Open();
          }
          return sqlCeConnection;
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex, "Error getting SQL connection: {0}", (object) ex.Message);
        throw;
      }
    }

    private Collection<UpdatePackage> ReadDataFromSqlDatabase(string command)
    {
      try
      {
        SqlCeDataReader sqlCeDataReader = new SqlCeCommand(command, this.GetConnection()).ExecuteReader();
        Collection<UpdatePackage> collection = new Collection<UpdatePackage>();
        while (sqlCeDataReader.Read())
        {
          string uniqueId = sqlCeDataReader["UniqueId"] as string;
          UpdatePackageFile[] files = this.ReadUpdatePackageFiles(uniqueId);
          UpdatePackage updatePackage = new UpdatePackage(sqlCeDataReader["ProductType"] as string, sqlCeDataReader["ProductCode"] as string, sqlCeDataReader["SoftwareVersion"] as string, sqlCeDataReader["VariantVersion"] as string, sqlCeDataReader["VariantDescription"] as string, sqlCeDataReader["PackageUsePurpose"] as string, sqlCeDataReader["PackageUseType"] as string, sqlCeDataReader["BuildType"] as string, sqlCeDataReader["OsVersion"] as string, sqlCeDataReader["AkVersion"] as string, sqlCeDataReader["BspVersion"] as string, sqlCeDataReader["VplPath"] as string, uniqueId, sqlCeDataReader["SourcePath"] as string, (bool) sqlCeDataReader["Online"], (DateTime) sqlCeDataReader["Timestamp"], files);
          collection.Add(updatePackage);
        }
        return collection;
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        throw;
      }
    }

    private UpdatePackageFile[] ReadUpdatePackageFiles(string uniqueId)
    {
      try
      {
        return new SqlSelectFilesCommand(this.GetConnection()).Select(uniqueId);
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex.Message);
        return new UpdatePackageFile[0];
      }
    }

    private void WriteDataToSqlDatabase(UpdatePackage updatePackage, bool fromLocal = false)
    {
      try
      {
        SqlCeConnection connection = this.GetConnection();
        try
        {
          UpdatePackage[] updatePackageArray = this.ReadUpdatePackage(updatePackage.ProductType, updatePackage.ProductCode, updatePackage.SoftwareVersion, updatePackage.VariantVersion);
          VplHelper vplHelper = new VplHelper();
          foreach (UpdatePackage updatePackage1 in updatePackageArray)
          {
            if (fromLocal && updatePackage1.VplPath == updatePackage.VplPath && vplHelper.CheckIfAllMandatoryFilesAreOnDisc(updatePackage.VplPath))
              throw new DuplicateNameException();
          }
          new SqlSimpleCommand("UpdatePackage", connection).Delete("UniqueId", (object) updatePackage.UniqueId);
        }
        catch (DuplicateNameException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          Tracer.Information("Cannot remove UpdatePackage: {0}", (object) ex.Message);
        }
        SqlInsertCommand sqlInsertCommand1 = new SqlInsertCommand("UpdatePackage", connection);
        sqlInsertCommand1.AddParameter("ProductType", (object) updatePackage.ProductType);
        sqlInsertCommand1.AddParameter("ProductCode", (object) updatePackage.ProductCode);
        sqlInsertCommand1.AddParameter("SoftwareVersion", (object) updatePackage.SoftwareVersion);
        sqlInsertCommand1.AddParameter("VariantVersion", (object) updatePackage.VariantVersion);
        sqlInsertCommand1.AddParameter("VariantDescription", (object) updatePackage.VariantDescription);
        sqlInsertCommand1.AddParameter("PackageUsePurpose", (object) updatePackage.PackageUsePurpose);
        sqlInsertCommand1.AddParameter("PackageUseType", (object) updatePackage.PackageUseType);
        sqlInsertCommand1.AddParameter("BuildType", (object) updatePackage.BuildType);
        sqlInsertCommand1.AddParameter("OsVersion", (object) updatePackage.OsVersion);
        sqlInsertCommand1.AddParameter("AkVersion", (object) updatePackage.AkVersion);
        sqlInsertCommand1.AddParameter("BspVersion", (object) updatePackage.BspVersion);
        if (!updatePackage.Online)
          sqlInsertCommand1.AddParameter("VplPath", (object) updatePackage.VplPath);
        sqlInsertCommand1.AddParameter("UniqueId", (object) updatePackage.UniqueId);
        sqlInsertCommand1.AddParameter("SourcePath", (object) updatePackage.SourcePath);
        sqlInsertCommand1.AddParameter("Online", (object) updatePackage.Online);
        sqlInsertCommand1.AddParameter("Timestamp", (object) updatePackage.Timespamp);
        sqlInsertCommand1.AddParameter("NormalizedSoftwareVersion", (object) this.NormalizedVersion(updatePackage.SoftwareVersion));
        sqlInsertCommand1.Insert();
        try
        {
          new SqlSimpleCommand("UpdatePackageFile", connection).Delete("VariantPackageUniqueId", (object) updatePackage.UniqueId);
        }
        catch
        {
        }
        foreach (UpdatePackageFile file in updatePackage.Files)
        {
          SqlInsertCommand sqlInsertCommand2 = new SqlInsertCommand("UpdatePackageFile", connection);
          sqlInsertCommand2.AddParameter("VariantPackageUniqueId", (object) updatePackage.UniqueId);
          sqlInsertCommand2.AddParameter("ProductType", (object) file.ProductType);
          sqlInsertCommand2.AddParameter("FileName", (object) file.FileName);
          sqlInsertCommand2.AddParameter("RelativePath", (object) file.RelativePath);
          sqlInsertCommand2.AddParameter("FileSize", (object) file.FileSize);
          sqlInsertCommand2.AddParameter("Timestamp", (object) file.Timestamp);
          sqlInsertCommand2.Insert();
        }
      }
      catch (Exception ex)
      {
        Tracer.Information(ex.Message);
        throw;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string NormalizedVersion(string version)
    {
      string[] array = version.Split('.');
      Array.Resize<string>(ref array, 5);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,10}{1,10}{2,10}{3,10}{4,10}", (object[]) array);
    }
  }
}
