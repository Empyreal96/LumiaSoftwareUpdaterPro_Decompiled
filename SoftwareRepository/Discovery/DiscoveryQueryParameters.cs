// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Discovery.DiscoveryQueryParameters
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Runtime.Serialization;

namespace SoftwareRepository.Discovery
{
  [DataContract]
  public class DiscoveryQueryParameters
  {
    [DataMember(EmitDefaultValue = false, Name = "customerName")]
    public string CustomerName { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "extendedAttributes")]
    public ExtendedAttributes ExtendedAttributes { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manufacturerHardwareModel")]
    public string ManufacturerHardwareModel { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manufacturerHardwareVariant")]
    public string ManufacturerHardwareVariant { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manufacturerModelName")]
    public string ManufacturerModelName { get; set; }

    [DataMember(Name = "manufacturerName")]
    public string ManufacturerName { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manufacturerPackageId")]
    public string ManufacturerPackageId { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manufacturerPlatformId")]
    public string ManufacturerPlatformId { get; set; }

    [DataMember(Name = "manufacturerProductLine")]
    public string ManufacturerProductLine { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manufacturerVariantName")]
    public string ManufacturerVariantName { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "operatorName")]
    public string OperatorName { get; set; }

    [DataMember(Name = "packageClass")]
    public string PackageClass { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "packageRevision")]
    public string PackageRevision { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "packageState")]
    public string PackageState { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "packageSubRevision")]
    public string PackageSubRevision { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "packageSubtitle")]
    public string PackageSubtitle { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "packageTitle")]
    public string PackageTitle { get; set; }

    [DataMember(Name = "packageType")]
    public string PackageType { get; set; }
  }
}
