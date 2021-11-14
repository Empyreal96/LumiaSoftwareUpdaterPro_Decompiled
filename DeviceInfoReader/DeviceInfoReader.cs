// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceInfoReaderNamespace.DeviceInfoReader
// Assembly: DeviceInfoReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 751BFCA1-492E-45FD-8982-0907B7D0BE5F
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\DeviceInfoReader.exe

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nokia.Lucid.DeviceInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.DeviceInfoReaderNamespace
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class DeviceInfoReader
  {
    private const string QueryNotSupported = "Query not supported";
    private const string InformationNotAvailable = "NA";
    private const string QueryFailed = "Error reading value";
    private readonly CommandLineParser commandLineParser;
    private string tempDirectory = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), nameof (DeviceInfoReader));
    private string filePath;
    private string[] selectedInfos;
    private string infoSelection;
    private bool readNcsdVersion;
    private bool readAllInfos;
    private bool canExit;
    private bool forceExit;
    private ConnectedDeviceMode deviceMode;
    private Nokia.Lucid.UsbDeviceIo.UsbDeviceIo deviceIo;
    private TaskHelper waitForCloseSignalTask;
    private CancellationTokenSource waitForCloseSignalTaskCancellationTokenSource;
    private CancellationToken waitForCloseSignalTaskCancellationToken;
    private TaskHelper wachdogTask;
    private CancellationTokenSource wachdogTaskCancellationTokenSource;
    private CancellationToken wachdogTaskCancellationToken;
    private IList infosList;

    public DeviceInfoReader(string[] args)
    {
      this.commandLineParser = new CommandLineParser(args);
      this.waitForCloseSignalTaskCancellationTokenSource = new CancellationTokenSource();
      this.waitForCloseSignalTaskCancellationToken = this.waitForCloseSignalTaskCancellationTokenSource.Token;
      this.wachdogTaskCancellationTokenSource = new CancellationTokenSource();
      this.wachdogTaskCancellationToken = this.wachdogTaskCancellationTokenSource.Token;
    }

    public string DevicePath { get; private set; }

    public string PortId { get; private set; }

    internal bool NoWatchdog { get; private set; }

    public void ReadDeviceInfos()
    {
      this.DeleteCommunicationFile();
      this.StartWaitForCloseSignalTask();
      this.StartWatchdogTask();
      this.selectedInfos = this.infoSelection.ToLower(CultureInfo.InvariantCulture).Split(',');
      this.infosList = (IList) ((IEnumerable<string>) this.selectedInfos).ToList<string>();
      try
      {
        if (string.IsNullOrEmpty(this.DevicePath))
        {
          this.DetectDefaultDeviceForInfosReading();
        }
        else
        {
          Tracer.Information("Reading device mode");
          this.deviceMode = LucidConnectivityHelper.GetDeviceModeFromDevicePath(this.DevicePath);
          Tracer.Information("Device mode {0}", (object) this.deviceMode);
        }
        this.ReadDeviceInfoAndWaitForCompletition();
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Device info reading error.", objArray);
        throw;
      }
    }

    private void ReadDeviceInfoAndWaitForCompletition()
    {
      if (this.deviceMode != ConnectedDeviceMode.Normal && this.deviceMode != ConnectedDeviceMode.Label && this.deviceMode != ConnectedDeviceMode.Uefi)
        return;
      if (!this.readAllInfos && this.infosList.Count == 0)
      {
        Tracer.Information("No infos selected for reading.");
      }
      else
      {
        Tracer.Information("Start device info reading.");
        this.ReadDeviceInfoNow();
        if (this.canExit)
          return;
        int num = 0;
        do
        {
          Thread.Sleep(200);
          ++num;
        }
        while (!this.canExit && num < 50);
      }
    }

    private void DetectDefaultDeviceForInfosReading()
    {
      DeviceInfo propertySet = new DeviceInfoSet()
      {
        DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
        Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
      }.EnumeratePresentDevices().Last<DeviceInfo>((Func<DeviceInfo, bool>) (deviceInfo => (uint) deviceInfo.DeviceType > 0U));
      if (propertySet != null)
      {
        this.DevicePath = propertySet.Path;
        Console.WriteLine("Device detected");
      }
      Tracer.Information("Reading device mode");
      this.deviceMode = LucidConnectivityHelper.GetDeviceModeFromDevicePath(this.DevicePath);
      Tracer.Information("Device mode {0}", (object) this.deviceMode);
      Tracer.Information("No -path parameter given. Autodetecting device.");
      string typeDesignator;
      string salesName;
      LucidConnectivityHelper.ParseTypeDesignatorAndSalesName(propertySet.ReadBusReportedDeviceDescription(), out typeDesignator, out salesName);
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("t"))
      {
        Console.WriteLine("TypeDesignator:" + typeDesignator);
        this.infosList.Remove((object) "t");
      }
      if (!this.readAllInfos && !((IEnumerable<string>) this.selectedInfos).Contains<string>("na") || this.deviceMode == ConnectedDeviceMode.Uefi)
        return;
      Console.WriteLine("SalesName:" + salesName);
      this.infosList.Remove((object) "na");
    }

    private void DeleteCommunicationFile()
    {
      this.filePath = Path.Combine(this.tempDirectory, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeviceInfoReader_{0}.txt", (object) this.PortId.Replace(':', '.')));
      if (!File.Exists(this.filePath))
        return;
      File.Delete(this.filePath);
    }

    private void StartWaitForCloseSignalTask()
    {
      this.waitForCloseSignalTask = new TaskHelper((Action) (() => this.WaitForCloseSignalTask()), this.waitForCloseSignalTaskCancellationToken);
      this.waitForCloseSignalTask.Start();
      this.waitForCloseSignalTask.ContinueWith((Action<object>) (t =>
      {
        if (this.wachdogTask.Exception == null)
          return;
        foreach (object innerException in this.wachdogTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void WaitForCloseSignalTask()
    {
      this.filePath = Path.Combine(this.tempDirectory, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeviceInfoReader_{0}.txt", (object) this.PortId.Replace(':', '.')));
      do
      {
        if (!File.Exists(this.filePath))
        {
          Thread.Sleep(500);
        }
        else
        {
          string str;
          try
          {
            StreamReaderHelper streamReaderHelper = new StreamReaderHelper(this.filePath);
            str = streamReaderHelper.ReadLine();
            streamReaderHelper.Close();
          }
          catch (Exception ex)
          {
            object[] objArray = new object[0];
            Tracer.Information(ex, "WaitForCloseSignalTask: Exception", objArray);
            Thread.Sleep(500);
            goto label_10;
          }
          if (!string.IsNullOrEmpty(str) && str == "CloseDeviceInfoReader")
          {
            Tracer.Information("WaitForCloseSignalTask: cancelling tasks");
            if (this.wachdogTaskCancellationTokenSource != null)
              this.wachdogTaskCancellationTokenSource.Cancel();
            this.forceExit = true;
            this.DeleteCommunicationFile();
            break;
          }
          Thread.Sleep(500);
        }
label_10:;
      }
      while (!this.waitForCloseSignalTaskCancellationToken.IsCancellationRequested);
    }

    private void StartWatchdogTask()
    {
      this.wachdogTask = new TaskHelper((Action) (() => this.WatchdogTask()), this.wachdogTaskCancellationToken);
      this.wachdogTask.Start();
      this.wachdogTask.ContinueWith((Action<object>) (t =>
      {
        if (this.wachdogTask.Exception == null)
          return;
        foreach (object innerException in this.wachdogTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void WatchdogTask()
    {
      if (this.NoWatchdog)
      {
        Tracer.Information("No watchdog is used");
      }
      else
      {
        while (this.CheckIfLsuProIsRunning())
        {
          Thread.Sleep(1000);
          if (this.wachdogTaskCancellationToken.IsCancellationRequested)
            return;
        }
        Tracer.Information("WatchdogTask: Lsu Pro was closed.");
        if (this.waitForCloseSignalTaskCancellationTokenSource != null)
          this.waitForCloseSignalTaskCancellationTokenSource.Cancel();
        this.forceExit = true;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private bool CheckIfLsuProIsRunning()
    {
      try
      {
        Process[] processesByName1 = ProcessHelper.GetProcessesByName("LumiaSoftwareUpdaterPro");
        if (processesByName1.Length != 0)
          return !processesByName1[0].HasExited;
        Process[] processesByName2 = ProcessHelper.GetProcessesByName("LUMIAS~1");
        if (processesByName2.Length != 0)
          return !processesByName2[0].HasExited;
        Process[] processesByName3 = ProcessHelper.GetProcessesByName("LumiaSoftwareUpdaterPro.vshost");
        if (processesByName3.Length != 0)
          return !processesByName3[0].HasExited;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Information(ex, "CheckIfLsuProIsRunning: exception occured.", objArray);
        return false;
      }
      return false;
    }

    public bool ParseArguments()
    {
      if (this.commandLineParser.ParseArguments() > 0 && this.commandLineParser.SwitchIsSet("help"))
      {
        DeviceInfoReader.PrintHelp();
        return false;
      }
      this.DevicePath = this.commandLineParser.GetOptionValue("path");
      this.PortId = this.commandLineParser.GetOptionValue("port");
      Tracer.Information("Reading device information from device path {0}; {1}", (object) this.DevicePath, (object) this.PortId);
      this.infoSelection = this.commandLineParser.GetOptionValue("infos");
      Tracer.Information("Reading info according to param {0}", (object) this.infoSelection);
      this.readAllInfos = this.commandLineParser.SwitchIsSet("AllInfo");
      this.NoWatchdog = this.commandLineParser.SwitchIsSet("nowatchdog");
      return true;
    }

    internal static string GetPropertyValue(string line)
    {
      JsonRpcResponse jsonRpcResponse;
      try
      {
        jsonRpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(line);
      }
      catch (JsonReaderException ex)
      {
        Tracer.Information("Unable to parse jsonresponse from output line {0}, exception {1}", (object) line, (object) ex);
        return "NA";
      }
      try
      {
        if (jsonRpcResponse.Error != null)
          return "Error reading value";
        return ((JToken) jsonRpcResponse.Result).Root.ToString().Split(':')[1].Trim('}').Replace("\"", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
      }
      catch (Exception ex)
      {
        Tracer.Information("Unable to parse any value from output line {0}, exception {1}", (object) line, (object) ex);
        return "NA";
      }
    }

    internal static string GetNcsdVersionValue(string line)
    {
      JsonRpcResponse jsonRpcResponse;
      try
      {
        jsonRpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(line);
      }
      catch (JsonReaderException ex)
      {
        Tracer.Information("Unable to parse jsonresponse from output line {0}, exception {1}", (object) line, (object) ex);
        return "NA";
      }
      try
      {
        if (jsonRpcResponse.Error != null)
          return "Error reading value";
        return ((JToken) jsonRpcResponse.Result).Root.ToString().Split(':')[4].Trim('}').Replace("\"", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
      }
      catch (Exception ex)
      {
        Tracer.Information("Unable to parse any value from output line {0}, exception {1}", (object) line, (object) ex);
        return "NA";
      }
    }

    internal static string GetUefiCertificateStatusValue(string line)
    {
      JsonRpcResponse jsonRpcResponse;
      try
      {
        jsonRpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(line);
      }
      catch (JsonReaderException ex)
      {
        Tracer.Information("Unable to parse jsonresponse from output line {0}, exception {1}", (object) line, (object) ex);
        return "NA";
      }
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        if (jsonRpcResponse.Error != null)
          return "Error reading value";
        string[] strArray = ((JToken) jsonRpcResponse.Result).Root.ToString().Split(',');
        foreach (string str1 in strArray)
        {
          string str2 = str1.Trim('}').Trim('{').Replace("\"", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
          stringBuilder.AppendFormat("{0}{1}", (object) str2, str1 != ((IEnumerable<string>) strArray).Last<string>() ? (object) "|" : (object) string.Empty);
        }
      }
      catch (Exception ex)
      {
        Tracer.Information("Unable to parse any value from output line {0}, exception {1}", (object) line, (object) ex);
        return "NA";
      }
      return stringBuilder.ToString();
    }

    internal static void PrintHelp()
    {
      Console.WriteLine("DeviceInfoReader.exe");
      Console.WriteLine("Parameters");
      Console.WriteLine("-path=[device]   Device path for the device that is connected.");
      Console.WriteLine("-port=[port]     Port Id where device is connected.");
      Console.WriteLine("-help            Shows help");
      Console.WriteLine("-AllInfo         Read all available device infos.");
      Console.WriteLine("-infos=[comma separated list of infos]    Specifies what product infos are read.");
      Console.WriteLine("                 DeviceInfoReader does not change device mode. It reads only that info that is available");
      Console.WriteLine("                 in the mode that device is currently in.");
      Console.WriteLine();
      Console.WriteLine("              ** Info available in normal mode **");
      Console.WriteLine("                 N = Ncsd version");
      Console.WriteLine("                 T = TypeDesignator (only when device path not given)");
      Console.WriteLine("                 Sw = SwVersion");
      Console.WriteLine("                 P = ProductCode");
      Console.WriteLine("                 Hw = HwVersion");
      Console.WriteLine("                 SN = SerialNumber");
      Console.WriteLine("                 SN2 = SerialNumber for 2nd SIM");
      Console.WriteLine("                 M = ManufacturerModelName");
      Console.WriteLine("                 O = OperatorName");
      Console.WriteLine("                 Ak = AkVersion");
      Console.WriteLine("                 Bsp = BspVersion");
      Console.WriteLine("                 Rdc = RdcAvailable");
      Console.WriteLine("                 Sec = SecurityMode");
      Console.WriteLine("                 Pvk = PvkAvailable");
      Console.WriteLine("                 SecStat = SecurityStatus");
      Console.WriteLine("                 Na = SalesName (only when device path not given)");
      Console.WriteLine("                 UefiCert = UEFI certificate status");
      Console.WriteLine("                 LabelId = Label ID");
      Console.WriteLine("                 ConfId = Configuration ID");
      Console.WriteLine();
      Console.WriteLine("              ** Info available in Uefi mode **");
      Console.WriteLine("                 T = TypeDesignator (only when device path not given)");
      Console.WriteLine("                 Sec = SecurityStatus");
      Console.WriteLine("                 Sd = SdCardSize");
      Console.WriteLine("                 SFM = SecureFfuMode");
      Console.WriteLine("                 Pid = PlatformId");
      Console.WriteLine("                 FAI = FlashAppInfo");
      Console.WriteLine("                 FCS = FinalConfigStatus");
      Console.WriteLine("                 Na = SalesName (only when device path not given)");
      Console.WriteLine();
      Console.WriteLine("             ** Info available in Label mode **");
      Console.WriteLine("                 T = TypeDesignator (only when device path not given)");
      Console.WriteLine("                 Sw = SwVersion");
      Console.WriteLine("                 P = ProductCode");
      Console.WriteLine("                 BP = BasicProductCode");
      Console.WriteLine("                 M = ModuleCode");
      Console.WriteLine("                 Hw = HwVersion");
      Console.WriteLine("                 Psn=PSN");
      Console.WriteLine("                 SN = SerialNumber");
      Console.WriteLine("                 Sl = SimlockActive");
      Console.WriteLine("                 Sli = SimlockInfo");
      Console.WriteLine("                 PuI = PublicId");
      Console.WriteLine("                 WM = WlanMacAddress");
      Console.WriteLine("                 BT = BluetoothId");
      Console.WriteLine("                 L = LabelAppVersion");
      Console.WriteLine("                 Na = SalesName (only when device path not given)");
    }

    private static string ParseFinalConfigStatus(int finalConfigFlagValue) => finalConfigFlagValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    private static string ParseSecureFfuMode(byte[] result) => ((DeviceInfoReader.WindowsPhone8SecureFfuMode) ((int) result[18] << 8 | (int) result[19])).ToString();

    private static string ParseUefiAppName(byte[] result)
    {
      string str = "NA";
      switch (result[17])
      {
        case 1:
          str = "BootManager";
          break;
        case 2:
          str = "FlashApp";
          break;
        case 3:
          str = "PhoneInfoApp";
          break;
      }
      return str;
    }

    private static string GetSecurityStatusBasedOnByteValue(byte b)
    {
      switch (b)
      {
        case 0:
          return "Disabled";
        case 1:
          return "Enabled";
        case 254:
          return "NA";
        case byte.MaxValue:
          return "NA";
        default:
          return "NA";
      }
    }

    private static void CreateOutput(string namedResponses, string result) => Console.WriteLine("{0}:{1}", (object) namedResponses, (object) result);

    private static void CreateOutput(
      string namedResponses,
      Dictionary<string, string> valuesReadFromDevice)
    {
      if (!valuesReadFromDevice.ContainsKey(namedResponses) || string.IsNullOrWhiteSpace(valuesReadFromDevice[namedResponses]))
        return;
      DeviceInfoReader.CreateOutput(namedResponses, valuesReadFromDevice[namedResponses]);
    }

    private void ReadDeviceInfoNow()
    {
      try
      {
        int num = 0;
        do
        {
          try
          {
            Tracer.Information("Creating UsbDeviceIo on: {0}, retry: {1}", (object) this.DevicePath, (object) num);
            this.deviceIo = new Nokia.Lucid.UsbDeviceIo.UsbDeviceIo(this.DevicePath);
          }
          catch (Exception ex)
          {
            ++num;
            Thread.Sleep(300);
            if (num > 3)
              throw;
          }
        }
        while (this.deviceIo == null);
        this.ReadCorrectModeDeviceInfo();
      }
      catch (ArgumentOutOfRangeException ex)
      {
        Tracer.Error("No device connected in given port, {0}", (object) ex);
        DeviceInfoReader.CreateOutput("Error", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No device connected in given port, {0}", (object) ex));
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Error("Exception caught, {0}", (object) ex);
        DeviceInfoReader.CreateOutput("Error", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception caught, {0}", (object) ex.Message));
        throw;
      }
      finally
      {
        this.ReadDeviceInfoNowCompleteActions();
      }
    }

    private void ReadCorrectModeDeviceInfo()
    {
      if (this.deviceMode == ConnectedDeviceMode.Normal)
        this.ReadNormalModeProductInfos();
      else if (this.deviceMode == ConnectedDeviceMode.Uefi)
      {
        this.ReadUefiModeBasicProductInfos();
      }
      else
      {
        if (this.deviceMode != ConnectedDeviceMode.Label)
          return;
        this.ReadLabelModeBasicProductInfos();
      }
    }

    private void ReadDeviceInfoNowCompleteActions()
    {
      this.deviceIo.Dispose();
      this.deviceIo = (Nokia.Lucid.UsbDeviceIo.UsbDeviceIo) null;
      if (this.wachdogTaskCancellationTokenSource != null)
        this.wachdogTaskCancellationTokenSource.Cancel();
      if (this.waitForCloseSignalTaskCancellationTokenSource != null)
        this.waitForCloseSignalTaskCancellationTokenSource.Cancel();
      this.DeleteCommunicationFile();
      this.canExit = true;
    }

    private void ReadNormalModeProductInfos()
    {
      Dictionary<string, string> valuesReadFromDevice = new Dictionary<string, string>();
      Dictionary<string, string> read = this.SelectNormalModeInfosToRead();
      foreach (string key in read.Keys)
      {
        if (this.forceExit)
        {
          Tracer.Information("Force exit.");
          return;
        }
        Tracer.Information("Sending message {0}", (object) read[key]);
        string message = this.SendAndReceiveMessage(read[key]);
        DeviceInfoReader.ParseResponseValueFromDeviceResponse(valuesReadFromDevice, key, message);
        Tracer.Information("Response {0}", (object) message);
        DeviceInfoReader.CreateOutput(key, valuesReadFromDevice);
      }
      if (!this.readNcsdVersion)
        return;
      Tracer.Information("Sending message {0}", (object) "{\"jsonrpc\": \"2.0\", \"id\": 5,\"method\": \"GetVersion\", \"params\": {\"MessageVersion\": 0}}");
      string message1 = this.SendAndReceiveMessage("{\"jsonrpc\": \"2.0\", \"id\": 5,\"method\": \"GetVersion\", \"params\": {\"MessageVersion\": 0}}");
      Tracer.Information("Response {0}", (object) message1);
      DeviceInfoReader.CreateOutput("NcsdVersion", DeviceInfoReader.GetNcsdVersionValue(message1));
    }

    private static void ParseResponseValueFromDeviceResponse(
      Dictionary<string, string> valuesReadFromDevice,
      string key,
      string result)
    {
      if (key == "SerialNumber2" && valuesReadFromDevice.ContainsKey("SerialNumber") && DeviceInfoReader.GetPropertyValue(result) == valuesReadFromDevice["SerialNumber"])
        return;
      valuesReadFromDevice.Add(key, key.Contains("UefiCertificateStatus") ? DeviceInfoReader.GetUefiCertificateStatusValue(result) : DeviceInfoReader.GetPropertyValue(result));
    }

    private Dictionary<string, string> SelectNormalModeInfosToRead()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("os"))
        dictionary.Add("OsVersion", "{\"jsonrpc\": \"2.0\", \"id\": 20,\"method\": \"ReadOsVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sw"))
        dictionary.Add("SwVersion", "{\"jsonrpc\": \"2.0\", \"id\": 1,\"method\": \"ReadSwVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("p"))
        dictionary.Add("ProductCode", "{\"jsonrpc\": \"2.0\", \"id\": 2,\"method\": \"ReadProductCode\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("hw"))
        dictionary.Add("HwVersion", "{\"jsonrpc\": \"2.0\", \"id\": 3,\"method\": \"ReadHwVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sn"))
      {
        dictionary.Add("SerialNumber", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadSerialNumber\", \"params\": {\"MessageVersion\": 0, \"SubscriptionId\": 0}}");
        dictionary.Add("SerialNumber2", "{\"jsonrpc\": \"2.0\", \"id\": 5,\"method\": \"ReadSerialNumber\", \"params\": {\"MessageVersion\": 0, \"SubscriptionId\": 1}}");
      }
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("m"))
        dictionary.Add("ManufacturerModelName", "{\"jsonrpc\": \"2.0\", \"id\": 6,\"method\": \"ReadManufacturerModelName\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("o"))
        dictionary.Add("OperatorName", "{\"jsonrpc\": \"2.0\", \"id\": 7,\"method\": \"ReadOperatorName\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("ak"))
        dictionary.Add("AkVersion", "{\"jsonrpc\": \"2.0\", \"id\": 8,\"method\": \"ReadAkVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("bsp"))
        dictionary.Add("BspVersion", "{\"jsonrpc\": \"2.0\", \"id\": 9,\"method\": \"ReadBspVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("rdc"))
        dictionary.Add("RdcAvailable", "{\"jsonrpc\": \"2.0\", \"id\": 10,\"method\": \"ReadCertificateAvailable\", \"params\": {\"MessageVersion\": 0, \"AsicIndex\": 0, \"CertName\": \"RDC\"}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sec"))
        dictionary.Add("SecurityMode", "{\"jsonrpc\": \"2.0\", \"id\": 11,\"method\": \"GetSecurityMode\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("pvk"))
        dictionary.Add("PvkAvailable", "{\"jsonrpc\": \"2.0\", \"id\": 12,\"method\": \"ReadPVKAvailable\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("secstat"))
      {
        dictionary.Add("PlatformSecureBootEnabled", "{\"jsonrpc\": \"2.0\", \"id\": 13,\"method\": \"ReadPlatformSecureBootEnabled\", \"params\": {\"MessageVersion\": 0}}");
        dictionary.Add("SecureFfuEnabled", "{\"jsonrpc\": \"2.0\", \"id\": 14,\"method\": \"ReadSecureFfuEnabled\", \"params\": {\"MessageVersion\": 0}}");
        dictionary.Add("DebugDisabled", "{\"jsonrpc\": \"2.0\", \"id\": 15,\"method\": \"ReadDebugDisabled\", \"params\": {\"MessageVersion\": 0}}");
        dictionary.Add("UefiSecureBootEnabled", "{\"jsonrpc\": \"2.0\", \"id\": 16,\"method\": \"ReadUefiSecureBootEnabled\", \"params\": {\"MessageVersion\": 0}}");
      }
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("ueficert"))
        dictionary.Add("UefiCertificateStatus", "{\"jsonrpc\": \"2.0\", \"id\": 17,\"method\": \"GetUefiCertificateStatus\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("labelid"))
        dictionary.Add("LabelId", "{\"jsonrpc\": \"2.0\", \"id\": 18,\"method\": \"ReadLabelID\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("confid"))
        dictionary.Add("ConfigurationId", "{\"jsonrpc\": \"2.0\", \"id\": 19,\"method\": \"ReadConfigurationID\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("n"))
        this.readNcsdVersion = true;
      return dictionary;
    }

    private void ReadUefiModeBasicProductInfos()
    {
      byte[] rawMessage1 = this.SendAndReceiveRawMessage(new byte[4]
      {
        (byte) 78,
        (byte) 79,
        (byte) 75,
        (byte) 86
      });
      Tracer.Information(((IEnumerable<byte>) rawMessage1).Aggregate<byte, string>("InfoResponse", (Func<string, byte, string>) ((current, b) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", (object) current, (object) b))));
      if (rawMessage1.Length > 5 && rawMessage1[5] == (byte) 1)
      {
        Console.WriteLine("SalesName:Device is booting up");
        Tracer.Information("Device is in UEFI boot mgr, skip reading other infos");
      }
      else
      {
        if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("na"))
        {
          Console.WriteLine("SalesName:Device in UEFI mode");
          Tracer.Information("Device is in UEFI FlashApp");
        }
        Dictionary<string, byte[]> namedMessages = new Dictionary<string, byte[]>();
        this.SelectUefiInfosToRead(namedMessages);
        foreach (string key in namedMessages.Keys)
        {
          if (this.forceExit)
          {
            Tracer.Information("Force exit.");
            break;
          }
          Tracer.Information("Sending message {0}", (object) key);
          byte[] rawMessage2 = this.SendAndReceiveRawMessage(namedMessages[key]);
          Tracer.Information("Response {0}", (object) rawMessage2);
          this.ParseFlashModeResponse(key, rawMessage2);
        }
      }
    }

    private void SelectUefiInfosToRead(Dictionary<string, byte[]> namedMessages)
    {
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sec"))
        namedMessages.Add("SecurityStatus", new byte[16]
        {
          (byte) 78,
          (byte) 79,
          (byte) 75,
          (byte) 88,
          (byte) 70,
          (byte) 82,
          (byte) 0,
          (byte) 83,
          (byte) 83,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0
        });
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sd"))
        namedMessages.Add("SdCardSize", new byte[16]
        {
          (byte) 78,
          (byte) 79,
          (byte) 75,
          (byte) 88,
          (byte) 70,
          (byte) 82,
          (byte) 0,
          (byte) 83,
          (byte) 68,
          (byte) 83,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0
        });
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sfm"))
        namedMessages.Add("SecureFfuMode", new byte[16]
        {
          (byte) 78,
          (byte) 79,
          (byte) 75,
          (byte) 88,
          (byte) 70,
          (byte) 82,
          (byte) 0,
          (byte) 83,
          (byte) 70,
          (byte) 80,
          (byte) 73,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0
        });
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("pid"))
        namedMessages.Add("PlatformId", new byte[16]
        {
          (byte) 78,
          (byte) 79,
          (byte) 75,
          (byte) 88,
          (byte) 70,
          (byte) 82,
          (byte) 0,
          (byte) 68,
          (byte) 80,
          (byte) 73,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0
        });
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("fai"))
        namedMessages.Add("FlashAppInfo", new byte[16]
        {
          (byte) 78,
          (byte) 79,
          (byte) 75,
          (byte) 88,
          (byte) 70,
          (byte) 82,
          (byte) 0,
          (byte) 70,
          (byte) 65,
          (byte) 73,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0
        });
      if (!this.readAllInfos && !((IEnumerable<string>) this.selectedInfos).Contains<string>("fcs"))
        return;
      namedMessages.Add("FinalConfigStatus", new byte[16]
      {
        (byte) 78,
        (byte) 79,
        (byte) 75,
        (byte) 88,
        (byte) 70,
        (byte) 82,
        (byte) 0,
        (byte) 70,
        (byte) 67,
        (byte) 83,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      });
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void ParseFlashModeResponse(string key, byte[] result)
    {
      if (result[3] == (byte) 85)
      {
        Tracer.Warning("Unknown command received for {1}, not supported {0}", (object) result[3], (object) key);
        DeviceInfoReader.CreateOutput(key, "Query not supported");
      }
      else if (result[3] != (byte) 88 || result[4] != (byte) 70 || result[5] != (byte) 82)
      {
        Tracer.Warning("Invalid message id received. Id: " + (object) result[3] + " " + (object) result[4] + " " + (object) result[5]);
        DeviceInfoReader.CreateOutput(key, "NA");
      }
      else if (!(key == "SecurityStatus"))
      {
        if (!(key == "SdCardSize"))
        {
          if (!(key == "SecureFfuMode"))
          {
            if (!(key == "PlatformId"))
            {
              if (!(key == "FlashAppInfo"))
              {
                if (!(key == "FinalConfigStatus"))
                  return;
                if (((int) result[6] << 8 | (int) result[7]) != 0)
                {
                  DeviceInfoReader.CreateOutput(key, "Unknown");
                }
                else
                {
                  if (BitConverter.IsLittleEndian)
                    Array.Reverse((Array) result, 17, 4);
                  DeviceInfoReader.CreateOutput(key, DeviceInfoReader.ParseFinalConfigStatus(BitConverter.ToInt32(result, 17)));
                }
              }
              else
              {
                if (BitConverter.IsLittleEndian)
                  Array.Reverse((Array) result, 6, 2);
                if (BitConverter.ToInt16(result, 6) != (short) 0)
                {
                  Tracer.Error("FlashAppInfo query from FlashApp failed, result: " + (object) BitConverter.ToInt16(result, 6));
                  DeviceInfoReader.CreateOutput("FlashAppInfo", "Error reading value");
                }
                else
                {
                  DeviceInfoReader.CreateOutput("UefiApp", DeviceInfoReader.ParseUefiAppName(result));
                  DeviceInfoReader.CreateOutput("FlashAppProtocolVersion", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) int.Parse(result[18].ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture), (object) int.Parse(result[19].ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture)));
                  DeviceInfoReader.CreateOutput("FlashAppVersion", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) int.Parse(result[20].ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture), (object) int.Parse(result[21].ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture)));
                }
              }
            }
            else
            {
              string str = BitConverter.ToString(result, 17).Replace("-", string.Empty);
              StringBuilder stringBuilder = new StringBuilder();
              for (int startIndex = 0; startIndex < str.Length - 2; startIndex += 2)
                stringBuilder.Append(Convert.ToChar(int.Parse(str.Substring(startIndex, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)));
              DeviceInfoReader.CreateOutput(key, stringBuilder.ToString().TrimEnd(new char[1]).Trim());
            }
          }
          else
            DeviceInfoReader.CreateOutput(key, DeviceInfoReader.ParseSecureFfuMode(result));
        }
        else
        {
          if (BitConverter.IsLittleEndian)
            Array.Reverse((Array) result, 17, 4);
          long num = 512L * (long) BitConverter.ToInt32(result, 17);
          DeviceInfoReader.CreateOutput(key, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) num));
        }
      }
      else
      {
        if (BitConverter.IsLittleEndian)
          Array.Reverse((Array) result, 6, 2);
        if (BitConverter.ToInt16(result, 6) != (short) 0)
        {
          Tracer.Warning("SecurityStatus query from FlashApp failed, result: " + (object) BitConverter.ToInt16(result, 6));
          DeviceInfoReader.CreateOutput(key, "NA");
        }
        DeviceInfoReader.CreateOutput("UefiSecureBootStatus", result[17] == (byte) 1 ? "Query not supported" : DeviceInfoReader.GetSecurityStatusBasedOnByteValue(result[23]));
        DeviceInfoReader.CreateOutput("PlatformSecureBootStatus", DeviceInfoReader.GetSecurityStatusBasedOnByteValue(result[18]));
        DeviceInfoReader.CreateOutput("SecureFfuEfuseStatus", DeviceInfoReader.GetSecurityStatusBasedOnByteValue(result[19]));
        DeviceInfoReader.CreateOutput("DebugStatus", DeviceInfoReader.GetSecurityStatusBasedOnByteValue(result[20]));
        DeviceInfoReader.CreateOutput("RdcStatus", DeviceInfoReader.GetSecurityStatusBasedOnByteValue(result[21]));
        DeviceInfoReader.CreateOutput("AuthenticationStatus", DeviceInfoReader.GetSecurityStatusBasedOnByteValue(result[22]));
      }
    }

    private void ReadLabelModeBasicProductInfos()
    {
      Dictionary<string, string> valuesReadFromDevice = new Dictionary<string, string>();
      Dictionary<string, string> read = this.SelectLabelInfosToRead();
      foreach (string key1 in read.Keys)
      {
        if (this.forceExit)
        {
          Tracer.Information("Force exit.");
          break;
        }
        Tracer.Information("Sending message {0}", (object) read[key1]);
        string message = this.SendAndReceiveMessage(read[key1]);
        Tracer.Information("Response {0}", (object) message);
        if (key1 == "WlanMacAddress")
        {
          string str1 = message;
          char[] chArray = new char[1]{ ']' };
          foreach (string str2 in str1.Split(chArray))
          {
            int startIndex = str2.IndexOf("Wlan", StringComparison.Ordinal);
            if (startIndex > 0)
            {
              string result = "{\n   \"id\" : 4,\n   \"jsonrpc\" : \"2.0\",\n   \"result\" : {" + str2.Substring(startIndex - 1) + "]\n   }\n}";
              string key2 = str2.Substring(startIndex).Split('"')[0];
              DeviceInfoReader.ParseResponseValueFromDeviceResponse(valuesReadFromDevice, key2, result);
              DeviceInfoReader.CreateOutput(key1, valuesReadFromDevice);
            }
          }
        }
        else
        {
          DeviceInfoReader.ParseResponseValueFromDeviceResponse(valuesReadFromDevice, key1, message);
          DeviceInfoReader.CreateOutput(key1, valuesReadFromDevice);
        }
      }
    }

    private Dictionary<string, string> SelectLabelInfosToRead()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sw"))
        dictionary.Add("SwVersion", "{\"jsonrpc\": \"2.0\", \"id\": 1,\"method\": \"ReadSwVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("p"))
        dictionary.Add("ProductCode", "{\"jsonrpc\": \"2.0\", \"id\": 2,\"method\": \"ReadProductCode\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("bp"))
        dictionary.Add("BasicProductCode", "{\"jsonrpc\": \"2.0\", \"id\": 3,\"method\": \"ReadBasicProductCode\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("m"))
        dictionary.Add("ModuleCode", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadModuleCode\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("hw"))
        dictionary.Add("HwVersion", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadHWVersion\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("psn"))
        dictionary.Add("PSN", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadPsn\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sn"))
        dictionary.Add("SerialNumber", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadSerialNumber\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sl"))
        dictionary.Add("SimLockActive", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadSimLockActive\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("sli"))
        dictionary.Add("SimLockInfo", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadSimLockInfo\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("pui"))
        dictionary.Add("PublicId", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadPublicId\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("wm"))
        dictionary.Add("WlanMacAddress", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadWlanMacAddress\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("bt"))
        dictionary.Add("BluetoothId", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"ReadBtId\", \"params\": {\"MessageVersion\": 0}}");
      if (this.readAllInfos || ((IEnumerable<string>) this.selectedInfos).Contains<string>("l"))
        dictionary.Add("LabelAppVersion", "{\"jsonrpc\": \"2.0\", \"id\": 4,\"method\": \"GetVersion\", \"params\": {\"MessageVersion\": 0}}");
      return dictionary;
    }

    private string SendAndReceiveMessage(string message)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(message);
      this.deviceIo.Send(bytes, (uint) bytes.Length);
      byte[] receivedData;
      int num = (int) this.deviceIo.Receive(out receivedData, TimeSpan.FromSeconds(5.0));
      return Encoding.Default.GetString(receivedData);
    }

    private byte[] SendAndReceiveRawMessage(byte[] message)
    {
      this.deviceIo.Send(message, (uint) message.Length);
      byte[] receivedData;
      int num = (int) this.deviceIo.Receive(out receivedData, TimeSpan.FromSeconds(5.0));
      return receivedData;
    }

    [Flags]
    private enum WindowsPhone8SecureFfuMode
    {
      None = 0,
      Sync = 1,
      Async = 2,
    }
  }
}
