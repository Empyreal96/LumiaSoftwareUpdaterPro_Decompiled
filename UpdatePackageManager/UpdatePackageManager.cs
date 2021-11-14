// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackageManager
// Assembly: UpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 41E723B1-981E-4695-9C69-83D1319A5755
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\UpdatePackageManager.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro
{
  public class UpdatePackageManager
  {
    private readonly SqlHelper sqlHelper;

    public UpdatePackageManager()
      : this(new SqlHelper())
    {
    }

    public UpdatePackageManager(SqlHelper sqlHelper)
    {
      this.sqlHelper = sqlHelper;
      this.Online = true;
    }

    public bool Online { get; set; }

    public IList<string> OfflineSourcePathes { get; set; }

    public string[] GetProductTypes() => this.sqlHelper.ReadProductTypes(this.Online, this.OfflineSourcePathes);

    public UpdatePackage[] GetProductCodes(string productType) => this.sqlHelper.ReadProductCodes(productType, this.Online, this.OfflineSourcePathes);

    public IList<UpdatePackage> GetProductCodes(
      string productType,
      string softwareVersion)
    {
      return this.sqlHelper.ReadProductCodes(productType, softwareVersion, this.Online, this.OfflineSourcePathes);
    }

    public IList<UpdatePackage> GetProductCodesWinOs(
      string productType,
      string winOsVersion)
    {
      return this.sqlHelper.ReadProductCodesWinOs(productType, winOsVersion, this.Online, this.OfflineSourcePathes);
    }

    public UpdatePackage[] GetSoftwareVersions(string productType) => this.sqlHelper.ReadSoftwareVersions(productType, this.Online, this.OfflineSourcePathes);

    public UpdatePackage[] GetWinOsVersions(string productType) => this.sqlHelper.ReadWinOsVersions(productType, this.Online, this.OfflineSourcePathes);

    public UpdatePackage[] GetSoftwareVersions(string productType, string productCode) => this.sqlHelper.ReadSoftwareVersions(productType, productCode, this.Online, this.OfflineSourcePathes);

    public UpdatePackage[] GetVariantVersions(
      string productType,
      string productCode,
      string softwareVersion)
    {
      return this.sqlHelper.ReadVariantVersions(productType, productCode, softwareVersion, this.Online, this.OfflineSourcePathes);
    }

    public UpdatePackage[] GetVariantVersionsOs(
      string productType,
      string productCode,
      string winOsVersion)
    {
      return this.sqlHelper.ReadVariantVersionsOs(productType, productCode, winOsVersion, this.Online, this.OfflineSourcePathes);
    }

    public UpdatePackage GetUpdatePackage(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion)
    {
      return this.sqlHelper.ReadUpdatePackage(productType, productCode, softwareVersion, variantVersion, this.Online, this.OfflineSourcePathes);
    }

    public UpdatePackage GetUpdatePackage(string productType, string uniqueId) => this.sqlHelper.ReadUpdatePackage(productType, uniqueId, this.Online, this.OfflineSourcePathes);

    public UpdatePackage GetLatestSoftwareVersion(
      string productType,
      string productCode)
    {
      return this.sqlHelper.ReadLatestSoftwareVersion(productType, productCode, this.Online, this.OfflineSourcePathes);
    }
  }
}
