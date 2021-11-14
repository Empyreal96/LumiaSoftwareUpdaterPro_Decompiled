// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SoftwareRepository.SoftwareRepositoryReportData
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using LsuProReportingLib;
using System;

namespace Microsoft.LsuPro.SoftwareRepository
{
  [Serializable]
  public class SoftwareRepositoryReportData
  {
    public SoftwareRepositoryReportData()
    {
    }

    public SoftwareRepositoryReportData(LsuProReportItem reportData)
    {
      this.AkVersion = reportData.AkVersion ?? string.Empty;
      this.ApiError = reportData.ApiError ?? string.Empty;
      this.ApiErrorDescription = reportData.ApiErrorDescription ?? string.Empty;
      this.ApplicationVersion = reportData.ApplicationVersion ?? string.Empty;
      this.AverageDownloadSpeed = reportData.AverageDownloadSpeed ?? string.Empty;
      this.AverageFlashingSpeed = reportData.AverageFlashingSpeed ?? string.Empty;
      this.BasicProductCode = reportData.BasicProductCode ?? string.Empty;
      this.BspVersion = reportData.BspVersion ?? string.Empty;
      this.ComputerId = reportData.ComputerId ?? string.Empty;
      this.DevelopmentPc = reportData.DevelopmentPc;
      this.DisablePiaReading = reportData.DisablePiaReading;
      this.FactoryResetEnabled = reportData.FactoryResetEnabled;
      this.HwId = reportData.HwId ?? string.Empty;
      this.Id = reportData.Id;
      this.NcsdVersion = reportData.NcsdVersion ?? string.Empty;
      this.NewProductCode = reportData.NewProductCode ?? string.Empty;
      this.NewSoftwareVersion = reportData.NewSoftwareVersion ?? string.Empty;
      this.OutsideCompanyNetwork = reportData.OutsideCompanyNetwork;
      this.ProductCode = reportData.ProductCode ?? string.Empty;
      this.ProductType = reportData.ProductType ?? string.Empty;
      this.RdcStatus = reportData.RdcStatus ?? string.Empty;
      this.SkipFfuIntegrityCheck = reportData.SkipFfuIntegrityCheck;
      this.SkipPlatformIdCheck = reportData.SkipPlatformIdCheck;
      this.SkipSignatureCheck = reportData.SkipSignatureCheck;
      this.SkipWrite = reportData.SkipWrite;
      this.SoftwareVersion = reportData.SoftwareVersion ?? string.Empty;
      this.TestingPc = reportData.TestingPc;
      this.Thor2Version = reportData.Thor2Version ?? string.Empty;
      this.Timestamp = reportData.Timestamp;
      this.URI = reportData.URI ?? string.Empty;
      this.UriDescription = reportData.UriDescription ?? string.Empty;
    }

    public int Id { get; set; }

    public string ComputerId { get; set; }

    public DateTime Timestamp { get; set; }

    public string URI { get; set; }

    public string UriDescription { get; set; }

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
