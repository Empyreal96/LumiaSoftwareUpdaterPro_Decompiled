// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceSecurityLevel
// Assembly: Wp8DeviceInformation, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 6707A4BB-F60A-40D7-A2BC-1ABC64317FDD
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceInformation.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class DeviceSecurityLevel
  {
    private readonly string uefiCertificateStatus;
    private bool trafficLightStatusChecked;
    private static string alphaCollinsList = "RM-820,RM-821,RM-822,RM-824,RM-825,RM-825,RM-826,RM-845,RM-846,RM-846,RM-860,RM-867,RM-875,RM-875,RM-876,RM-877,RM-877,RM-877,RM-878,RM-885,RM-885,RM-887,RM-892,RM-892,RM-892,RM-893,RM-893,RM-910,RM-913,RM-914,RM-915,RM-915,RM-917,RM-941,RM-942,RM-943,RM-955,RM-994,RM-995,RM-996,RM-997,RM-998";

    public DeviceSecurityLevel()
    {
      this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.Unknown;
      this.trafficLightStatusChecked = false;
      this.RdCertified = false;
      this.IsValid = false;
    }

    public DeviceSecurityLevel(string uefiCertificateStatus)
    {
      this.trafficLightStatusChecked = false;
      this.RdCertified = false;
      this.IsValid = false;
      this.ParseStatus(uefiCertificateStatus);
      this.uefiCertificateStatus = uefiCertificateStatus;
    }

    public static DeviceSecurityLevel Empty => new DeviceSecurityLevel()
    {
      BootPolicy = DeviceSecurityLevel.UefiCertificateStatus.NA,
      Db = DeviceSecurityLevel.UefiCertificateStatus.NA,
      Dbx = DeviceSecurityLevel.UefiCertificateStatus.NA,
      Kek = DeviceSecurityLevel.UefiCertificateStatus.NA,
      Pk = DeviceSecurityLevel.UefiCertificateStatus.NA
    };

    public bool IsValid { get; private set; }

    public void DetermineTrafficLightStatus(string typeDesignator, bool deviceHasRdc)
    {
      this.RdCertified = deviceHasRdc;
      if (DeviceSecurityLevel.TypeDesignatorIsAlphaCollins(typeDesignator) && this.SecurityLevelType == DeviceSecurityLevel.DeviceSecurityLevelType.RetailWithRdUefiCertificates)
      {
        Tracer.Warning("Retail/RD status detected, but device {0} does not support RD conversion => forcing Retail status", (object) typeDesignator);
        this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.Retail;
      }
      if (deviceHasRdc)
      {
        switch (this.SecurityLevelType)
        {
          case DeviceSecurityLevel.DeviceSecurityLevelType.None:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.Rd:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.SupportedBySkippingSignatureCheck;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.Retail:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.NotSupported;
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.RetailWithRdUefiCertificates:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.SupportedBySkippingSignatureCheck;
            break;
          default:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.UnableToDetermine;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.UnableToDetermine;
            break;
        }
      }
      else
      {
        switch (this.SecurityLevelType)
        {
          case DeviceSecurityLevel.DeviceSecurityLevelType.None:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.Rd:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.NotSupported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.Retail:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.NotSupported;
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.RetailWithRdUefiCertificates:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.Supported;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.NotSupported;
            break;
          default:
            this.RetailImageSupport = DeviceSecurityLevel.FlashImageSupport.UnableToDetermine;
            this.ProductionImageSupport = DeviceSecurityLevel.FlashImageSupport.UnableToDetermine;
            break;
        }
      }
      this.trafficLightStatusChecked = true;
      this.IsValid = true;
    }

    public bool RdCertified { get; private set; }

    public DeviceSecurityLevel.FlashImageSupport ProductionImageSupport { get; private set; }

    public DeviceSecurityLevel.FlashImageSupport RetailImageSupport { get; private set; }

    public string RdCertificateStatusMessage
    {
      get
      {
        if (this.trafficLightStatusChecked)
        {
          bool rdCertified = this.RdCertified;
          if (!rdCertified)
            return "RDC not available";
          if (rdCertified)
            return "RDC available";
        }
        return "Unable to determine RDC status";
      }
    }

    public string RdCertificateHoverMessage
    {
      get
      {
        if (this.trafficLightStatusChecked)
        {
          bool rdCertified = this.RdCertified;
          if (rdCertified)
          {
            if (rdCertified)
              return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Variant change supported{0}Manual NV item writing supported", (object) Environment.NewLine);
          }
          else
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Variant change not supported{0}Manual NV item writing not supported", (object) Environment.NewLine);
        }
        return string.Empty;
      }
    }

    public string ProductionImageSupportMessage => this.GetFlashImageSupportMessage(this.ProductionImageSupport, "Prod/RD SW");

    public string RetailImageSupportMessage => this.GetFlashImageSupportMessage(this.RetailImageSupport, "Retail SW");

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string GetFlashImageSupportMessage(
      DeviceSecurityLevel.FlashImageSupport flashImageSupport,
      string image)
    {
      switch (flashImageSupport)
      {
        case DeviceSecurityLevel.FlashImageSupport.Supported:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} can be flashed", (object) image);
        case DeviceSecurityLevel.FlashImageSupport.NotSupported:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Do not flash {0}", (object) image);
        case DeviceSecurityLevel.FlashImageSupport.SupportedBySkippingSignatureCheck:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Skip signature check when flashing {0}", (object) image);
        default:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to determine if {0} can be flashed", (object) image);
      }
    }

    public DeviceSecurityLevel.DeviceSecurityLevelType SecurityLevelType { get; private set; }

    public DeviceSecurityLevel.UefiCertificateStatus BootPolicy { get; private set; }

    public DeviceSecurityLevel.UefiCertificateStatus Db { get; private set; }

    public DeviceSecurityLevel.UefiCertificateStatus Dbx { get; private set; }

    public DeviceSecurityLevel.UefiCertificateStatus Kek { get; private set; }

    public DeviceSecurityLevel.UefiCertificateStatus Pk { get; private set; }

    public string Description
    {
      get
      {
        string str;
        switch (this.SecurityLevelType)
        {
          case DeviceSecurityLevel.DeviceSecurityLevelType.None:
            str = "None";
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.Rd:
            str = "RD";
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.Retail:
            str = "Retail";
            break;
          case DeviceSecurityLevel.DeviceSecurityLevelType.RetailWithRdUefiCertificates:
            str = "Retail/RD";
            break;
          default:
            str = "Unknown";
            break;
        }
        if (!this.trafficLightStatusChecked)
          return str;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) str, this.RdCertified ? (object) "with RDC" : (object) "w/o RDC");
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) this.Description, (object) this.uefiCertificateStatus);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private DeviceSecurityLevel.UefiCertificateStatus ParseStatusFromToken(
      string token)
    {
      int num = token.IndexOf(':');
      DeviceSecurityLevel.UefiCertificateStatus result;
      return num >= 0 && Enum.TryParse<DeviceSecurityLevel.UefiCertificateStatus>(token.Substring(num + 1).Trim(), false, out result) ? result : DeviceSecurityLevel.UefiCertificateStatus.NA;
    }

    private void ParseStatus(string certificateStatus)
    {
      string str = certificateStatus;
      char[] chArray = new char[1]{ '|' };
      foreach (string token in str.Split(chArray))
      {
        if (token.StartsWith("BootPolicy:", StringComparison.OrdinalIgnoreCase))
          this.BootPolicy = this.ParseStatusFromToken(token);
        else if (token.StartsWith("Db:", StringComparison.OrdinalIgnoreCase))
          this.Db = this.ParseStatusFromToken(token);
        else if (token.StartsWith("Dbx:", StringComparison.OrdinalIgnoreCase))
          this.Dbx = this.ParseStatusFromToken(token);
        else if (token.StartsWith("Kek:", StringComparison.OrdinalIgnoreCase))
          this.Kek = this.ParseStatusFromToken(token);
        else if (token.StartsWith("Pk:", StringComparison.OrdinalIgnoreCase))
          this.Pk = this.ParseStatusFromToken(token);
      }
      if (this.Pk == DeviceSecurityLevel.UefiCertificateStatus.Unsecure && this.Db == DeviceSecurityLevel.UefiCertificateStatus.Unsecure)
        this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.None;
      else if (this.Pk == DeviceSecurityLevel.UefiCertificateStatus.SecureRD && this.Db == DeviceSecurityLevel.UefiCertificateStatus.SecureRD)
        this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.Rd;
      else if (this.Pk == DeviceSecurityLevel.UefiCertificateStatus.SecureProduction && this.Db == DeviceSecurityLevel.UefiCertificateStatus.SecureProduction)
        this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.Retail;
      else if (this.Pk == DeviceSecurityLevel.UefiCertificateStatus.SecureProduction && this.Db != DeviceSecurityLevel.UefiCertificateStatus.SecureProduction)
        this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.RetailWithRdUefiCertificates;
      else
        this.SecurityLevelType = DeviceSecurityLevel.DeviceSecurityLevelType.Unknown;
    }

    internal static bool TypeDesignatorIsAlphaCollins(string typeDesignator)
    {
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) DeviceSecurityLevel.alphaCollinsList.Split(','));
      return stringList.Contains(typeDesignator);
    }

    public enum UefiCertificateStatus
    {
      NA,
      Unknown,
      Unsecure,
      SecureRD,
      SecureProduction,
    }

    public enum DeviceSecurityLevelType
    {
      Unknown,
      None,
      Rd,
      Retail,
      RetailWithRdUefiCertificates,
    }

    public enum FlashImageSupport
    {
      Supported,
      NotSupported,
      SupportedBySkippingSignatureCheck,
      UnableToDetermine,
    }
  }
}
