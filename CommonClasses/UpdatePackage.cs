// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackage
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro
{
  public class UpdatePackage
  {
    public UpdatePackage(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion,
      string variantDescription,
      string packageUsePurpose,
      string packageUseType,
      string buildType,
      string osVersion,
      string akVersion,
      string bspVersion,
      string vplPath,
      string uniqueId,
      string sourcePath,
      bool online,
      DateTime dateTime,
      UpdatePackageFile[] files)
    {
      this.ProductType = productType ?? string.Empty;
      this.ProductCode = productCode ?? string.Empty;
      this.SoftwareVersion = softwareVersion ?? string.Empty;
      this.VariantVersion = variantVersion ?? string.Empty;
      this.VariantDescription = variantDescription ?? string.Empty;
      this.PackageUsePurpose = packageUsePurpose ?? string.Empty;
      this.PackageUseType = packageUseType ?? string.Empty;
      this.BuildType = buildType ?? string.Empty;
      this.OsVersion = osVersion ?? string.Empty;
      this.AkVersion = akVersion ?? string.Empty;
      this.BspVersion = bspVersion ?? string.Empty;
      this.VplPath = vplPath ?? string.Empty;
      this.UniqueId = uniqueId ?? string.Empty;
      this.SourcePath = sourcePath ?? string.Empty;
      this.Online = online;
      this.Timespamp = dateTime;
      this.Files = files ?? new UpdatePackageFile[0];
    }

    public string ProductType { get; private set; }

    public string ProductCode { get; private set; }

    public string SoftwareVersion { get; private set; }

    public string VariantVersion { get; private set; }

    public string VariantDescription { get; private set; }

    public string VplPath { get; set; }

    public string UniqueId { get; private set; }

    public string SourcePath { get; private set; }

    public string PackageUsePurpose { get; private set; }

    public string PackageUseType { get; set; }

    public string BuildType { get; private set; }

    public string OsVersion { get; private set; }

    public string AkVersion { get; private set; }

    public string BspVersion { get; private set; }

    public bool Online { get; set; }

    public bool OnlyOnlineSiblingsAvailable { get; set; }

    public UpdatePackageFile[] Files { get; private set; }

    public DateTime Timespamp { get; private set; }

    public override string ToString() => this.ProductCode;
  }
}
