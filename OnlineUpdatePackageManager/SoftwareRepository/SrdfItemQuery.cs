// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SoftwareRepository.SrdfItemQuery
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro.SoftwareRepository
{
  public class SrdfItemQuery
  {
    public string ManufacturerName { get; set; }

    public string ManufacturerProductLine { get; set; }

    public string PackageType { get; set; }

    public string PackageClass { get; set; }

    public string ManufacturerPackageId { get; set; }

    public string CustomerName { get; set; }

    public string ManufacturerHardwareModel { get; set; }

    public List<string> ManufacturerHardwareVariant { get; set; }

    public string ManufacturerModelName { get; set; }

    public string ManufacturerPlatformId { get; set; }

    public string ManufacturerVariantName { get; set; }

    public string OperatorName { get; set; }

    public string PackageRevision { get; set; }

    public string PackageSubRevision { get; set; }

    public string PackageSubtitle { get; set; }

    public string PackageTitle { get; set; }
  }
}
