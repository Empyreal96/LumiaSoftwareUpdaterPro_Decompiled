// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Discovery.SoftwarePackage
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoftwareRepository.Discovery
{
  [DataContract]
  public class SoftwarePackage
  {
    [DataMember(Name = "customerName")]
    public List<string> CustomerName { get; set; }

    [DataMember(Name = "extendedAttributes")]
    public ExtendedAttributes ExtendedAttributes { get; set; }

    [DataMember(Name = "files")]
    public List<SoftwareFile> Files { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "manufacturerHardwareModel")]
    public List<string> ManufacturerHardwareModel { get; set; }

    [DataMember(Name = "manufacturerHardwareVariant")]
    public List<string> ManufacturerHardwareVariant { get; set; }

    [DataMember(Name = "manufacturerModelName")]
    public List<string> ManufacturerModelName { get; set; }

    [DataMember(Name = "manufacturerName")]
    public string ManufacturerName { get; set; }

    [DataMember(Name = "manufacturerPackageId")]
    public string ManufacturerPackageId { get; set; }

    [DataMember(Name = "manufacturerPlatformId")]
    public List<string> ManufacturerPlatformId { get; set; }

    [DataMember(Name = "manufacturerProductLine")]
    public string ManufacturerProductLine { get; set; }

    [DataMember(Name = "manufacturerVariantName")]
    public List<string> ManufacturerVariantName { get; set; }

    [DataMember(Name = "operatorName")]
    public List<string> OperatorName { get; set; }

    [DataMember(Name = "packageClass")]
    public List<string> PackageClass { get; set; }

    [DataMember(Name = "packageDescription")]
    public string PackageDescription { get; set; }

    [DataMember(Name = "packageRevision")]
    public string PackageRevision { get; set; }

    [DataMember(Name = "packageState")]
    public string PackageState { get; set; }

    [DataMember(Name = "packageSubRevision")]
    public string PackageSubRevision { get; set; }

    [DataMember(Name = "packageSubtitle")]
    public string PackageSubtitle { get; set; }

    [DataMember(Name = "packageTitle")]
    public string PackageTitle { get; set; }

    [DataMember(Name = "packageType")]
    public string PackageType { get; set; }
  }
}
