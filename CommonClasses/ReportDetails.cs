// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ReportDetails
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Microsoft.LsuPro.Network;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.LsuPro
{
  public class ReportDetails
  {
    public ReportDetails()
    {
      if (!NokiaNetwork.IsInside())
        this.Ext7Details = Ext7Details.OutsideNokiaNetwork;
      this.LocationInfo = CountryChecker.LocationInfo;
      string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Lumia Software Updater Pro\\Bin"), "lsuprodevelopmentteam.txt");
      if (File.Exists("lsuprodevelopmentteam.txt") || File.Exists(path))
        this.Ext7Details |= Ext7Details.DevelopmentTeam;
      if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "testingteam.txt")))
        return;
      this.Ext7Details |= Ext7Details.TestingTeam;
    }

    public long Uri { get; set; }

    public string UriDescription { get; set; }

    public string ProductType { get; set; }

    public string ProductCode { get; set; }

    public string SoftwareVersion { get; set; }

    public string Imei { get; set; }

    public string HwId { get; set; }

    public string RdInfo { get; set; }

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "reviewed")]
    public string ProductCodeNew { get; set; }

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "reviewed")]
    public string SoftwareVersionNew { get; set; }

    public string ApiError { get; set; }

    public string ApiErrorText { get; set; }

    public string DebugField { get; set; }

    public long UpdateDuration { get; set; }

    public string FlashDeviceInfo { get; set; }

    public string ActionDescription { get; set; }

    public string ThorVersion { get; set; }

    public long DownloadDuration { get; set; }

    public long AverageDownloadSpeed { get; set; }

    public double AverageFlashingSpeed { get; set; }

    public Ext7Details Ext7Details { get; private set; }

    public string LocationInfo { get; private set; }

    public int GetExt7Field() => (int) this.Ext7Details;

    public void SetExt7Details(
      bool skipWriteInUse,
      bool factoryReset,
      bool skipFfuIntegrity,
      bool skipPlatformId,
      bool skipSignature,
      bool piaReadingDisabled)
    {
      if (skipWriteInUse)
        this.Ext7Details |= Ext7Details.SkipWriteInUse;
      if (factoryReset)
        this.Ext7Details |= Ext7Details.FactoryReset;
      if (skipFfuIntegrity)
        this.Ext7Details |= Ext7Details.SkipFfuIntegrity;
      if (skipPlatformId)
        this.Ext7Details |= Ext7Details.SkipPlatformId;
      if (skipSignature)
        this.Ext7Details |= Ext7Details.SkipSignature;
      if (!piaReadingDisabled)
        return;
      this.Ext7Details |= Ext7Details.PiaReadingDisabled;
    }
  }
}
