// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Reporting.ReportUploadLocationParameters
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Runtime.Serialization;

namespace SoftwareRepository.Reporting
{
  [DataContract]
  internal class ReportUploadLocationParameters
  {
    [DataMember(Name = "manufacturerName")]
    internal string ManufacturerName { get; set; }

    [DataMember(Name = "manufacturerProductLine")]
    internal string ManufacturerProductLine { get; set; }

    [DataMember(Name = "reportClassification")]
    internal string ReportClassification { get; set; }

    [DataMember(Name = "fileName")]
    internal string FileName { get; set; }
  }
}
