// Decompiled with JetBrains decompiler
// Type: LsuProReportingLib.LsuProReportItem
// Assembly: LsuProReportingLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 782B57DA-0F4D-41E4-ABE7-FF958E1377E1
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LsuProReportingLib.dll

using System;
using System.ComponentModel.DataAnnotations;

namespace LsuProReportingLib
{
  public class LsuProReportItem
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string ComputerId { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    public string URI { get; set; }

    public string UriDescription { get; set; }

    [Required]
    public string ApplicationVersion { get; set; }

    public string ProductType { get; set; }

    public string ProductCode { get; set; }

    public string NewProductCode { get; set; }

    public string SoftwareVersion { get; set; }

    public string NewSoftwareVersion { get; set; }

    public string HwId { get; set; }

    public string RdcStatus { get; set; }

    public string AkVersion { get; set; }

    public string BspVersion { get; set; }

    public string NcsdVersion { get; set; }

    public string BasicProductCode { get; set; }

    public string ApiError { get; set; }

    public string ApiErrorDescription { get; set; }

    public string Thor2Version { get; set; }

    public bool OutsideCompanyNetwork { get; set; }

    public bool SkipWrite { get; set; }

    public bool DevelopmentPc { get; set; }

    public bool TestingPc { get; set; }

    public bool FactoryResetEnabled { get; set; }

    public bool SkipFfuIntegrityCheck { get; set; }

    public bool SkipPlatformIdCheck { get; set; }

    public bool SkipSignatureCheck { get; set; }

    public bool DisablePiaReading { get; set; }

    public string AverageDownloadSpeed { get; set; }

    public string AverageFlashingSpeed { get; set; }
  }
}
