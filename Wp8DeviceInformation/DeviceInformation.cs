// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceInformation
// Assembly: Wp8DeviceInformation, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 6707A4BB-F60A-40D7-A2BC-1ABC64317FDD
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceInformation.dll

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class DeviceInformation
  {
    private List<string> deviceInfoReaderList = new List<string>();

    public DeviceInformation(ConnectedDevice connectedDevice)
    {
      this.ConnectedDevice = connectedDevice;
      this.SecurityLevel = new DeviceSecurityLevel();
      if (this.ConnectedDevice == null)
        return;
      this.TypeDesignator = this.ConnectedDevice.TypeDesignator;
      this.SalesName = this.ConnectedDevice.SalesName;
    }

    public event EventHandler<EventArgs> DeviceInfoReaderProcessExecutionCompleted;

    public event EventHandler<DeviceInformationReadEventArgs> DeviceInformationRead;

    public ConnectedDevice ConnectedDevice { get; private set; }

    public string Error { get; private set; }

    public string TypeDesignator { get; private set; }

    public string ProductCode { get; private set; }

    public string NcsdVersion { get; private set; }

    public string SoftwareVersion { get; private set; }

    public string OsVersion { get; private set; }

    public string HwVersion { get; private set; }

    public string Imei { get; private set; }

    public string Imei2 { get; private set; }

    public string OemDeviceName { get; private set; }

    public string MobileOperator { get; private set; }

    public string AkVersion { get; private set; }

    public string BspVersion { get; private set; }

    public string ModemFirewallMode { get; private set; }

    public string Pvk { get; private set; }

    public string BasicProductCode { get; private set; }

    public string ModuleCode { get; private set; }

    public string PSN { get; private set; }

    public string SimLockActive { get; private set; }

    public string SimLockInfo { get; private set; }

    public string PublicId { get; private set; }

    public string WlanMacAddress1 { get; private set; }

    public string WlanMacAddress2 { get; private set; }

    public string WlanMacAddress3 { get; private set; }

    public string WlanMacAddress4 { get; private set; }

    public string BluetoothId { get; private set; }

    public string LabelAppVersion { get; private set; }

    public string UefiSecureBootStatus { get; private set; }

    public string PlatformSecureBootStatus { get; private set; }

    public string SecureFfuEfuseStatus { get; private set; }

    public string DebugStatus { get; private set; }

    public string RdcStatus { get; private set; }

    public string AuthenticationStatus { get; private set; }

    public string SalesName { get; private set; }

    public string SdCardSize { get; private set; }

    public string SecureFfuMode { get; private set; }

    public string PlatformId { get; private set; }

    public string UefiApp { get; private set; }

    public string FlashAppProtocolVersion { get; private set; }

    public string FlashAppVersion { get; private set; }

    public string FinalConfigStatus { get; private set; }

    public DeviceSecurityLevel SecurityLevel { get; private set; }

    public string LabelId { get; set; }

    public string ConfigurationId { get; set; }

    public void ReadSpecificProductInformation(string infos)
    {
      TaskHelper readBasicProductInfoTask = new TaskHelper((Action) (() =>
      {
        bool flag = false;
        int num = 0;
        do
        {
          if (this.ConnectedDevice.DeviceReady)
          {
            flag = true;
            this.ExecuteReadInfoOperation(this.ConnectedDevice.PortId, this.ConnectedDevice.DevicePath, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-infos={0}", (object) infos));
          }
          else
          {
            if (num > 100)
            {
              flag = true;
              Tracer.Warning("Timeout in device info reading. Device is not ready to communicate.");
              this.Error = "Timeout in device info reading. Device is not ready to communicate.";
              this.OnDeviceInfoReaderProcessExecutionCompleted();
            }
            ++num;
            Thread.Sleep(300);
          }
        }
        while (!flag);
        Tracer.Information("{0}: Product infos read.", (object) this.ConnectedDevice.PortId);
      }));
      Tracer.Information("{0}: Start reading specific product infos in DeviceInfoReader process.", (object) this.ConnectedDevice.PortId);
      readBasicProductInfoTask.Start();
      readBasicProductInfoTask.ContinueWith((Action<object>) (t =>
      {
        if (readBasicProductInfoTask.Exception == null)
          return;
        foreach (object innerException in readBasicProductInfoTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    public void ReadAllProductInformation()
    {
      TaskHelper readAllProductInfoTask = new TaskHelper((Action) (() =>
      {
        bool flag = false;
        int num = 0;
        do
        {
          if (this.ConnectedDevice.DeviceReady)
          {
            flag = true;
            this.ExecuteReadInfoOperation(this.ConnectedDevice.PortId, this.ConnectedDevice.DevicePath, "-allinfo");
          }
          else
          {
            if (num > 100)
            {
              flag = true;
              Tracer.Warning("Timeout in device info reading. Device is not ready to communicate.");
              this.Error = "Timeout in device info reading. Device is not ready to communicate.";
              this.OnDeviceInfoReaderProcessExecutionCompleted();
            }
            ++num;
            Thread.Sleep(300);
          }
        }
        while (!flag);
        Tracer.Information("{0}: Product infos read.", (object) this.ConnectedDevice.PortId);
      }));
      Tracer.Information("{0}: Start reading product infos in DeviceInfoReader process.", (object) this.ConnectedDevice.PortId);
      readAllProductInfoTask.Start();
      readAllProductInfoTask.ContinueWith((Action<object>) (t =>
      {
        if (readAllProductInfoTask.Exception == null)
          return;
        foreach (object innerException in readAllProductInfoTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    internal void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(e.Data))
          return;
        char[] separator = new char[2]{ '\n', '\r' };
        string[] strArray = e.Data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        if (!string.IsNullOrEmpty(strArray[0]) && strArray[0].ToLowerInvariant().Contains("error"))
        {
          this.Error = this.GetPropertyValue(strArray[0]);
        }
        else
        {
          foreach (string line in strArray)
          {
            Tracer.Information("{0}: Output from DeviceInfoReader: {1}", (object) this.ConnectedDevice.PortId, (object) line);
            this.SetPropertyBasedOnResponseString(line);
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "{0}: Cannot parse device information: {1}", (object) this.ConnectedDevice.PortId, (object) ex.Message);
      }
    }

    protected virtual void OnDeviceInfoReaderProcessExecutionCompleted()
    {
      EventHandler<EventArgs> executionCompleted = this.DeviceInfoReaderProcessExecutionCompleted;
      if (executionCompleted == null)
        return;
      executionCompleted((object) this, EventArgs.Empty);
    }

    protected virtual void OnDeviceInformationRead(DeviceInformationReadEventArgs e)
    {
      EventHandler<DeviceInformationReadEventArgs> deviceInformationRead = this.DeviceInformationRead;
      if (deviceInformationRead == null)
        return;
      deviceInformationRead((object) this, e);
    }

    private void ExecuteReadInfoOperation(string physicalPort, string devicepath, string infos)
    {
      if (this.deviceInfoReaderList.Contains(physicalPort))
      {
        Tracer.Error("DeviceInfoReader.exe is already working on port: {0}", (object) physicalPort);
      }
      else
      {
        this.deviceInfoReaderList.Add(physicalPort);
        Tracer.Information("{0}: Starting device info reading operation (device path {1})", (object) physicalPort, (object) devicepath);
        string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        Process process = new Process()
        {
          EnableRaisingEvents = true
        };
        process.StartInfo = new ProcessStartInfo()
        {
          FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "DeviceInfoReader.exe")),
          Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-port={0} -path={1} {2}", (object) physicalPort, (object) devicepath, (object) infos),
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          WorkingDirectory = directoryName
        };
        this.Error = string.Empty;
        process.OutputDataReceived += new DataReceivedEventHandler(this.OnOutputDataReceived);
        Tracer.Information("Starting process");
        process.Start();
        Tracer.Information("Process started");
        process.BeginOutputReadLine();
        Tracer.Information("Waiting process {0} to exit", (object) process.Id);
        if (process.WaitForExit(180000))
          process.WaitForExit();
        if (process.HasExited)
        {
          Tracer.Information("Process terminated, exit code {0}", (object) process.ExitCode);
          if (process.ExitCode < 0)
          {
            ReportSender.SaveReportAsync(new ReportDetails()
            {
              Uri = 104906L,
              UriDescription = "DeviceInfoReader crash"
            }, DateTime.Now);
            this.Error = "DeviceInfoReader crashed";
          }
        }
        else
        {
          Tracer.Warning("DeviceInfoReader has not exited during given timeout. Killing process.");
          process.Kill();
          this.Error = "Timeout occured during device info reading";
        }
        this.deviceInfoReaderList.Remove(physicalPort);
        process.OutputDataReceived -= new DataReceivedEventHandler(this.OnOutputDataReceived);
        this.OnDeviceInfoReaderProcessExecutionCompleted();
      }
    }

    private void SetPropertyBasedOnResponseString(string line)
    {
      try
      {
        if (line.EndsWith(":NA", StringComparison.OrdinalIgnoreCase))
          Tracer.Warning("{0}: Information not available: '{1}'", (object) this.ConnectedDevice.PortId, (object) line);
        else if (line.Contains(DeviceInformationItem.TypeDesignator.ToString()))
        {
          this.TypeDesignator = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.TypeDesignator, this.TypeDesignator));
        }
        else if (line.Contains(DeviceInformationItem.SalesName.ToString()))
        {
          this.SalesName = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SalesName, this.SalesName));
        }
        else if (line.Contains(DeviceInformationItem.SwVersion.ToString()))
        {
          this.SoftwareVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SwVersion, this.SoftwareVersion));
        }
        else if (line.Contains(DeviceInformationItem.OsVersion.ToString()))
        {
          this.OsVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.OsVersion, this.OsVersion));
        }
        else if (line.Contains(DeviceInformationItem.BasicProductCode.ToString()))
        {
          this.BasicProductCode = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.BasicProductCode, this.BasicProductCode));
        }
        else if (line.Contains(DeviceInformationItem.ProductCode.ToString()))
        {
          this.ProductCode = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.ProductCode, this.ProductCode));
        }
        else if (line.Contains(DeviceInformationItem.HwVersion.ToString()))
        {
          this.HwVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.HwVersion, this.HwVersion));
        }
        else if (line.Contains(DeviceInformationItem.SerialNumber2.ToString()))
        {
          this.Imei2 = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SerialNumber2, this.Imei2));
        }
        else if (line.Contains(DeviceInformationItem.SerialNumber.ToString()))
        {
          this.Imei = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SerialNumber, this.Imei));
        }
        else if (line.Contains(DeviceInformationItem.NcsdVersion.ToString()))
        {
          this.NcsdVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.NcsdVersion, this.NcsdVersion));
        }
        else if (line.Contains(DeviceInformationItem.ManufacturerModelName.ToString()))
        {
          this.OemDeviceName = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.ManufacturerModelName, this.OemDeviceName));
        }
        else if (line.Contains(DeviceInformationItem.OperatorName.ToString()))
        {
          this.MobileOperator = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.OperatorName, this.MobileOperator));
        }
        else if (line.Contains(DeviceInformationItem.AkVersion.ToString()))
        {
          this.AkVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.AkVersion, this.AkVersion));
        }
        else if (line.Contains(DeviceInformationItem.BspVersion.ToString()))
        {
          this.BspVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.BspVersion, this.BspVersion));
        }
        else if (line.Contains(DeviceInformationItem.RdcAvailable.ToString()))
        {
          this.RdcStatus = this.GetSecurityStatusValue(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.RdcStatus, this.RdcStatus));
        }
        else if (line.Contains(DeviceInformationItem.SecurityMode.ToString()))
        {
          this.ModemFirewallMode = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SecurityMode, this.ModemFirewallMode));
        }
        else if (line.Contains(DeviceInformationItem.PvkAvailable.ToString()))
        {
          this.Pvk = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.PvkAvailable, this.Pvk));
        }
        else if (line.Contains(DeviceInformationItem.ModuleCode.ToString()))
        {
          this.ModuleCode = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.ModuleCode, this.ModuleCode));
        }
        else if (line.Contains(DeviceInformationItem.PSN.ToString()))
        {
          this.PSN = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.PSN, this.PSN));
        }
        else if (line.Contains(DeviceInformationItem.SimLockActive.ToString()))
        {
          this.SimLockActive = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SimLockActive, this.SimLockActive));
        }
        else if (line.Contains(DeviceInformationItem.SimLockInfo.ToString()))
        {
          this.SimLockInfo = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SimLockInfo, this.SimLockInfo));
        }
        else if (line.Contains(DeviceInformationItem.PublicId.ToString()))
        {
          this.PublicId = this.ToMAC48(this.GetPropertyValue(line), ',');
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.PublicId, this.PublicId));
        }
        else if (line.Contains(DeviceInformationItem.WlanMacAddress1.ToString()))
        {
          this.WlanMacAddress1 = this.ToMAC48(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.WlanMacAddress1, this.WlanMacAddress1));
        }
        else if (line.Contains(DeviceInformationItem.WlanMacAddress2.ToString()))
        {
          this.WlanMacAddress2 = this.ToMAC48(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.WlanMacAddress2, this.WlanMacAddress2));
        }
        else if (line.Contains(DeviceInformationItem.WlanMacAddress3.ToString()))
        {
          this.WlanMacAddress3 = this.ToMAC48(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.WlanMacAddress3, this.WlanMacAddress3));
        }
        else if (line.Contains(DeviceInformationItem.WlanMacAddress4.ToString()))
        {
          this.WlanMacAddress4 = this.ToMAC48(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.WlanMacAddress4, this.WlanMacAddress4));
        }
        else if (line.Contains(DeviceInformationItem.BluetoothId.ToString()))
        {
          this.BluetoothId = this.ToMAC48(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.BluetoothId, this.BluetoothId));
        }
        else if (line.Contains(DeviceInformationItem.LabelAppVersion.ToString()))
        {
          this.LabelAppVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.LabelAppVersion, this.LabelAppVersion));
        }
        else if (line.Contains(DeviceInformationItem.UefiSecureBootStatus.ToString()))
        {
          this.UefiSecureBootStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.UefiSecureBootStatus, this.UefiSecureBootStatus));
        }
        else if (line.Contains(DeviceInformationItem.PlatformSecureBootStatus.ToString()))
        {
          this.PlatformSecureBootStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.PlatformSecureBootStatus, this.PlatformSecureBootStatus));
        }
        else if (line.Contains(DeviceInformationItem.SecureFfuEfuseStatus.ToString()))
        {
          this.SecureFfuEfuseStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SecureFfuEfuseStatus, this.SecureFfuEfuseStatus));
        }
        else if (line.Contains(DeviceInformationItem.DebugStatus.ToString()))
        {
          this.DebugStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.DebugStatus, this.DebugStatus));
        }
        else if (line.Contains(DeviceInformationItem.RdcStatus.ToString()))
        {
          this.RdcStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.RdcStatus, this.RdcStatus));
        }
        else if (line.Contains(DeviceInformationItem.AuthenticationStatus.ToString()))
        {
          this.AuthenticationStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.AuthenticationStatus, this.AuthenticationStatus));
        }
        else if (line.Contains(DeviceInformationItem.SdCardSize.ToString()))
        {
          this.SdCardSize = Toolkit.GetCleanSdCardSize(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SdCardSize, this.SdCardSize));
        }
        else if (line.Contains(DeviceInformationItem.SecureFfuMode.ToString()))
        {
          this.SecureFfuMode = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SecureFfuMode, this.SecureFfuMode));
        }
        else if (line.Contains(DeviceInformationItem.PlatformId.ToString()))
        {
          this.PlatformId = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.PlatformId, this.PlatformId));
        }
        else if (line.Contains(DeviceInformationItem.UefiApp.ToString()))
        {
          this.UefiApp = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.UefiApp, this.UefiApp));
        }
        else if (line.Contains(DeviceInformationItem.FlashAppProtocolVersion.ToString()))
        {
          this.FlashAppProtocolVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.FlashAppProtocolVersion, this.FlashAppProtocolVersion));
        }
        else if (line.Contains(DeviceInformationItem.FlashAppVersion.ToString()))
        {
          this.FlashAppVersion = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.FlashAppVersion, this.FlashAppVersion));
        }
        else if (line.Contains(DeviceInformationItem.FinalConfigStatus.ToString()))
        {
          this.FinalConfigStatus = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.FinalConfigStatus, this.FinalConfigStatus));
        }
        else if (line.Contains(DeviceInformationItem.PlatformSecureBootEnabled.ToString()))
        {
          this.PlatformSecureBootStatus = this.GetSecurityStatusValue(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.PlatformSecureBootStatus, this.PlatformSecureBootStatus));
        }
        else if (line.Contains(DeviceInformationItem.SecureFfuEnabled.ToString()))
        {
          this.SecureFfuEfuseStatus = this.GetSecurityStatusValue(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SecureFfuEfuseStatus, this.SecureFfuEfuseStatus));
        }
        else if (line.Contains(DeviceInformationItem.UefiSecureBootEnabled.ToString()))
        {
          this.UefiSecureBootStatus = this.GetSecurityStatusValue(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.UefiSecureBootStatus, this.UefiSecureBootStatus));
        }
        else if (line.Contains(DeviceInformationItem.DebugDisabled.ToString()))
        {
          this.DebugStatus = this.GetSecurityStatusValue(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.DebugStatus, this.DebugStatus));
        }
        else if (line.Contains(DeviceInformationItem.UefiCertificateStatus.ToString()))
        {
          this.SecurityLevel = new DeviceSecurityLevel(this.GetPropertyValue(line));
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.SecurityLevel, this.SecurityLevel.Description));
        }
        else if (line.Contains(DeviceInformationItem.LabelId.ToString()))
        {
          this.LabelId = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.LabelId, this.LabelId));
        }
        else
        {
          if (!line.Contains(DeviceInformationItem.ConfigurationId.ToString()))
            return;
          this.ConfigurationId = this.GetPropertyValue(line);
          this.OnDeviceInformationRead(new DeviceInformationReadEventArgs(DeviceInformationItem.ConfigurationId, this.ConfigurationId));
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "{0}: Cannot parse device information: {1}, line: '{2}'", (object) this.ConnectedDevice.PortId, (object) ex.Message, (object) line);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string ToMAC48(string getPropertyValue, char separator = ':')
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = getPropertyValue;
      char[] separator1 = new char[3]{ '[', ']', ',' };
      foreach (string s in str.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
      {
        int num = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder.Append(num.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append(separator);
      }
      return stringBuilder.ToString(0, stringBuilder.Length - 1);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string GetPropertyValue(string line)
    {
      string str = line.Substring(line.IndexOf(':') + 1);
      return str == "Unknown" || str == "Error reading value" || str == "Query not supported" ? string.Empty : str;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string GetSecurityStatusValue(string boolVal)
    {
      if (boolVal.Equals("true", StringComparison.OrdinalIgnoreCase))
        return "Enabled";
      return boolVal.Equals("false", StringComparison.OrdinalIgnoreCase) ? "Disabled" : string.Empty;
    }
  }
}
