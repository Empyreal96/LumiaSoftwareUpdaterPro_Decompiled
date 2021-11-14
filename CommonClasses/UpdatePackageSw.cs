// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackageSw
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro
{
  public class UpdatePackageSw
  {
    public UpdatePackageSw(UpdatePackage package) => this.Package = package;

    public UpdatePackage Package { get; private set; }

    public string ProductType => this.Package.ProductType;

    public string ProductCode => this.Package.ProductCode;

    public string SoftwareVersion => this.Package.SoftwareVersion;

    public string VariantVersion => this.Package.VariantVersion;

    public string VariantDescription => this.Package.VariantDescription;

    public string VplPath => this.Package.VplPath;

    public string UniqueId => this.Package.UniqueId;

    public string SourcePath => this.Package.SourcePath;

    public string PackageUsePurpose => this.Package.PackageUsePurpose;

    public string PackageUseType => this.Package.PackageUseType;

    public string BuildType => this.Package.BuildType;

    public bool Online => this.Package.Online;

    public bool OnlyOnlineSiblingsAvailable => this.Package.OnlyOnlineSiblingsAvailable;

    public DateTime Timespamp => this.Package.Timespamp;

    public UpdatePackageFile[] Files => this.Package.Files;

    public override string ToString() => this.Package.SoftwareVersion;
  }
}
