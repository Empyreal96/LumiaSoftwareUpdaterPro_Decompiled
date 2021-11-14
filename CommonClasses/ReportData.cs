// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ReportData
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.LsuPro
{
  [Serializable]
  public class ReportData
  {
    private string computerId;
    private string sessionId;
    private string clientApplicationName;
    private string clientApplicationVersion;
    private string clientApplicationVendorName;
    private string localIp;
    private string userSiteLanguageId;
    private string productType;
    private string productCode;
    private string basicProductCode;
    private string hwId;
    private string imei;
    private string imsi;
    private string actionDescription;
    private string flashDevice;
    private string softwareVersion;
    private string softwareVersionNew;
    private string fwGrading;
    private string rdInfo;
    private string currentMode;
    private string targetMode;
    private string languagePackageOld;
    private string productCodeNew;
    private string uriDescription;
    private string apiError;
    private string apiErrorText;
    private string debugField;
    private string ext1;
    private string ext2;
    private string ext3;
    private string ext4;
    private string ext5;
    private string ext6;
    private string ext7;
    private string ext8;
    private string ext9;
    private string serviceLayerInfo;
    private string systemInfo;
    private string flashDeviceInfo;

    public ReportData()
    {
      this.ComputerId = this.GetComputerId();
      this.StartNewSession();
      this.ClientApplicationName = "Lumia Software Updater Pro";
      this.ClientApplicationVendorName = "Microsoft";
      try
      {
        this.ClientApplicationVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;
      }
      catch
      {
        this.ClientApplicationVersion = "8.8.8.8";
      }
      this.Timestamp = (DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000L;
      this.LocalIp = this.GetLocalIps();
    }

    public string ComputerId
    {
      get => this.computerId;
      private set => this.computerId = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string SessionId
    {
      get => this.sessionId;
      private set => this.sessionId = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ClientApplicationName
    {
      get => this.clientApplicationName;
      private set => this.clientApplicationName = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ClientApplicationVersion
    {
      get => this.clientApplicationVersion;
      private set => this.clientApplicationVersion = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ClientApplicationVendorName
    {
      get => this.clientApplicationVendorName;
      private set => this.clientApplicationVendorName = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public long Timestamp { get; private set; }

    public string LocalIp
    {
      get => this.localIp;
      private set => this.localIp = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string UserSiteLanguageId
    {
      get => this.userSiteLanguageId;
      private set => this.userSiteLanguageId = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ProductType
    {
      get => this.productType;
      set => this.productType = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ProductCode
    {
      get => this.productCode;
      set => this.productCode = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string BasicProductCode
    {
      get => this.basicProductCode;
      private set => this.basicProductCode = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string HwId
    {
      get => this.hwId;
      set => this.hwId = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string Imei
    {
      get => this.imei;
      set => this.imei = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string Imsi
    {
      get => this.imsi;
      private set => this.imsi = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ActionDescription
    {
      get => this.actionDescription;
      set => this.actionDescription = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string FlashDevice
    {
      get => this.flashDevice;
      set => this.flashDevice = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public long Duration { get; private set; }

    public long DownloadDuration { get; set; }

    public long UpdateDuration { get; set; }

    public string SoftwareVersion
    {
      get => this.softwareVersion;
      set => this.softwareVersion = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "reviewed")]
    public string SoftwareVersionNew
    {
      get => this.softwareVersionNew;
      set => this.softwareVersionNew = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string FwGrading
    {
      get => this.fwGrading;
      private set => this.fwGrading = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string RdInfo
    {
      get => this.rdInfo;
      set => this.rdInfo = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string CurrentMode
    {
      get => this.currentMode;
      private set => this.currentMode = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string TargetMode
    {
      get => this.targetMode;
      private set => this.targetMode = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string LanguagePackageOld
    {
      get => this.languagePackageOld;
      private set => this.languagePackageOld = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "reviewed")]
    public string ProductCodeNew
    {
      get => this.productCodeNew;
      set => this.productCodeNew = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public long Uri { get; set; }

    public string UriDescription
    {
      get => this.uriDescription;
      set => this.uriDescription = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ApiError
    {
      get => this.apiError;
      set => this.apiError = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string ApiErrorText
    {
      get => this.apiErrorText;
      set => this.apiErrorText = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string DebugField
    {
      get => this.debugField;
      set => this.debugField = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';').Replace(Environment.NewLine, "|");
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string Ext1
    {
      get => this.ext1;
      private set => this.ext1 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string Ext2
    {
      get => this.ext2;
      private set => this.ext2 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string Ext3
    {
      get => this.ext3;
      private set => this.ext3 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string Ext4
    {
      get => this.ext4;
      set => this.ext4 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string Ext5
    {
      get => this.ext5;
      set => this.ext5 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string Ext6
    {
      get => this.ext6;
      private set => this.ext6 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string Ext7
    {
      get => this.ext7;
      set => this.ext7 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string Ext8
    {
      get => this.ext8;
      set => this.ext8 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string Ext9
    {
      get => this.ext9;
      set => this.ext9 = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string ServiceLayerInfo
    {
      get => this.serviceLayerInfo;
      private set => this.serviceLayerInfo = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "not needed yet")]
    public string SystemInfo
    {
      get => this.systemInfo;
      private set => this.systemInfo = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    public string FlashDeviceInfo
    {
      get => this.flashDeviceInfo;
      set => this.flashDeviceInfo = string.IsNullOrEmpty(value) ? value : value.Replace(',', ';');
    }

    private void StartNewSession() => this.SessionId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:yyyyMMddHHmmssfff}_{2}", (object) this.ComputerId, (object) DateTime.Now, (object) new Random().Next(100000, 999999));

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string GetLocalIps()
    {
      IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
      string empty = string.Empty;
      foreach (IPAddress ipAddress in hostAddresses)
      {
        if (AddressFamily.InterNetwork == ipAddress.AddressFamily)
        {
          if (!string.IsNullOrEmpty(empty))
            empty += "|";
          empty += ipAddress.ToString();
        }
      }
      return empty;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string ShuffleString(string text)
    {
      Random random = new Random(777);
      return new string(text.OrderBy<char, int>((Func<char, int>) (x => random.Next())).ToArray<char>());
    }

    private string GetComputerId()
    {
      string empty1 = string.Empty;
      try
      {
        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementClass("win32_processor").GetInstances().GetEnumerator())
        {
          if (enumerator.MoveNext())
            empty1 = enumerator.Current.Properties["processorID"].Value.ToString();
        }
      }
      catch
      {
      }
      string empty2 = string.Empty;
      try
      {
        ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"C:\"");
        managementObject.Get();
        empty2 = managementObject["VolumeSerialNumber"].ToString();
      }
      catch
      {
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) this.ShuffleString(empty1), (object) this.ShuffleString(empty2));
    }

    public void Trace()
    {
      Tracer.Raw("Uri={0}\nUriDescription={1}", (object) this.Uri, (object) this.UriDescription);
      Tracer.Raw("ComputerId={0}\nSessionId={1}", (object) this.ComputerId, (object) this.SessionId);
      Tracer.Raw("ClientApplicationVendorName={0}\nClientApplicationName={1}\nClientApplicationVersion={2}\nTimestamp={3}\nLocalIp={4}", (object) this.ClientApplicationVendorName, (object) this.ClientApplicationName, (object) this.ClientApplicationVersion, (object) this.Timestamp, (object) this.LocalIp);
      Tracer.Raw("ProductType={0}\nProductCode={1}\nSoftwareVersion={2}", (object) this.ProductType, (object) this.ProductCode, (object) this.SoftwareVersion);
      Tracer.Raw("Imei={0}\nHwId={1}\nRdInfo={2}", (object) this.Imei, (object) this.HwId, (object) this.RdInfo);
      Tracer.Raw("ProductCodeNew={0}\nSoftwareVersionNew={1}", (object) this.ProductCodeNew, (object) this.SoftwareVersionNew);
      Tracer.Raw("ApiError={0}\nApiErrorText={1}", (object) this.ApiError, (object) this.ApiErrorText);
      Tracer.Raw("DownloadDuration={0}", (object) this.DownloadDuration);
      Tracer.Raw("Ext7={0}", (object) this.Ext7);
      Tracer.Raw("Ext8={0}", (object) this.Ext8);
    }

    public void Write(string fileName)
    {
      using (Stream fileStream = new FileStreamHelper().CreateFileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
        new BinaryFormatter().Serialize(fileStream, (object) this);
    }

    public static ReportData Read(string fileName)
    {
      using (Stream fileStream = new FileStreamHelper().CreateFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
        return new BinaryFormatter().Deserialize(fileStream) as ReportData;
    }
  }
}
