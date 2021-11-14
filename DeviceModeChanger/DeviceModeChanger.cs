// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceModeChangerNamespace.DeviceModeChanger
// Assembly: DeviceModeChanger, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 7BFF9D1F-0342-42E9-B090-043A753FF219
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\DeviceModeChanger.exe

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using Microsoft.LumiaConnectivity.EventArgs;
using Nokia.Lucid.DeviceInformation;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.DeviceModeChangerNamespace
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class DeviceModeChanger
  {
    private readonly ConnectedDevices connectedDevices = new ConnectedDevices();
    private readonly CommandLineParser commandLineParser;
    private bool deviceInTargetModeDetected;
    private ConnectedDevice connectedDevice;
    private bool bootToLabelCommandSentInBootManager;
    private TaskHelper wachdogTask;
    private CancellationTokenSource wachdogTaskCancellationTokenSource;
    private CancellationToken wachdogTaskCancellationToken;
    private bool forceExit;
    private const string GenericErrorMessage = "Operation failed with error {0}";
    private Thor2ExitCode operationExitCode;

    public DeviceModeChanger(string[] args)
    {
      this.commandLineParser = new CommandLineParser(args);
      this.wachdogTaskCancellationTokenSource = new CancellationTokenSource();
      this.wachdogTaskCancellationToken = this.wachdogTaskCancellationTokenSource.Token;
    }

    internal string ResetMethod { get; set; }

    internal ConnectedDeviceMode DesiredMode { get; set; }

    internal bool WaitForMode { get; private set; }

    internal bool AfterReboot { get; private set; }

    internal bool NoWatchdog { get; private set; }

    internal string DevicePath { get; set; }

    internal string PortId { get; set; }

    public bool ParseArguments()
    {
      this.commandLineParser.ParseArguments();
      if (this.commandLineParser.SwitchIsSet("help"))
      {
        DeviceModeChanger.PrintHelp();
        return false;
      }
      this.WaitForMode = true;
      this.DevicePath = this.commandLineParser.GetOptionValue("path");
      this.PortId = this.commandLineParser.GetOptionValue("id");
      this.DesiredMode = DeviceModeChanger.GetDesiredConnectedDeviceMode(this.commandLineParser.GetOptionValue("to"));
      this.WaitForMode = !this.commandLineParser.SwitchIsSet("nowait");
      this.AfterReboot = this.commandLineParser.SwitchIsSet("afterreboot");
      this.NoWatchdog = this.commandLineParser.SwitchIsSet("nowatchdog");
      return this.DesiredMode != ConnectedDeviceMode.Unknown;
    }

    public void ChangeDeviceMode()
    {
      try
      {
        this.StartWatchdogTask();
        if (string.IsNullOrEmpty(this.DevicePath) && string.IsNullOrEmpty(this.PortId))
        {
          Tracer.Information("No device path nor port ID provided");
          this.GetLastConnectedDevice();
        }
        this.connectedDevices.DeviceModeChanged += new EventHandler<DeviceModeChangedEventArgs>(this.ConnectedDevicesOnDeviceModeChanged);
        this.connectedDevices.DeviceReadyChanged += new EventHandler<DeviceReadyChangedEventArgs>(this.ConnectedDeviceOnDeviceReadyChanged);
        this.connectedDevices.Start();
        try
        {
          this.SelectCorrectDeviceIfMultipleDevicesConnected();
          if (this.connectedDevice.Mode == this.DesiredMode)
          {
            Console.WriteLine("[{0}] Operation Complete: Device in {1} mode", (object) nameof (DeviceModeChanger), (object) this.connectedDevice.Mode);
            Tracer.Information("[{0}] Operation Complete: Device in {1} mode", (object) nameof (DeviceModeChanger), (object) this.connectedDevice.Mode);
          }
          else
          {
            if (this.WaitForDeviceReboot())
              return;
            this.SetDeviceMode(this.connectedDevice.Mode, this.DesiredMode);
            if (!this.WaitForMode || this.WaitForDeviceEnterTargetMode())
              return;
            Console.WriteLine("[{0}] Operation Complete: Device in {1} mode", (object) nameof (DeviceModeChanger), (object) this.connectedDevice.Mode);
            Tracer.Information("[{0}] Operation Complete: Device in {1} mode", (object) nameof (DeviceModeChanger), (object) this.connectedDevice.Mode);
          }
        }
        catch (Exception ex)
        {
          Tracer.Error(ex, "DeviceModeChanger, Operation failed");
          Console.WriteLine("[{0}] Operation failed: {1}", (object) nameof (DeviceModeChanger), (object) ex.Message);
        }
      }
      finally
      {
        this.connectedDevices.Stop();
        this.connectedDevices.DeviceModeChanged -= new EventHandler<DeviceModeChangedEventArgs>(this.ConnectedDevicesOnDeviceModeChanged);
      }
    }

    private bool WaitForDeviceReboot()
    {
      int num = 0;
      while (!this.forceExit)
      {
        if (num > 540)
          throw new Exception("Timeout occured when waiting for device.");
        Thread.Sleep(500);
        ++num;
        if (!this.AfterReboot)
          return false;
      }
      Tracer.Information("Force exit.");
      return true;
    }

    private bool WaitForDeviceEnterTargetMode()
    {
      int num = 0;
      while (!this.forceExit)
      {
        if (num > 540)
          throw new Exception("Timeout occured when waiting for device.");
        Thread.Sleep(500);
        ++num;
        if (this.deviceInTargetModeDetected)
          return false;
      }
      Tracer.Information("Force exit.");
      return true;
    }

    private void SelectCorrectDeviceIfMultipleDevicesConnected()
    {
      foreach (ConnectedDevice connectedDevice in this.connectedDevices.Devices.Where<ConnectedDevice>((Func<ConnectedDevice, bool>) (device => !string.IsNullOrEmpty(device.DevicePath) && device.DevicePath == this.DevicePath)))
        this.connectedDevice = connectedDevice;
      if (this.connectedDevice != null)
        return;
      if (!string.IsNullOrWhiteSpace(this.PortId))
      {
        int num = 0;
        do
        {
          Tracer.Information("Looking for device with port ID {0}", (object) this.PortId);
          this.connectedDevice = this.connectedDevices.Devices.FirstOrDefault<ConnectedDevice>((Func<ConnectedDevice, bool>) (device => device.PortId == this.PortId));
          if (this.connectedDevice != null)
          {
            Tracer.Information("Found device in mode '{0}' ({1})", (object) this.connectedDevice.Mode, (object) this.connectedDevice.DevicePath);
            break;
          }
          ++num;
          if (num > 10)
          {
            Console.WriteLine("Operation failed. Connected device was null after all retries.");
            throw new NullReferenceException("Connected device was null after all retries.");
          }
          Thread.Sleep(500);
          this.connectedDevices.Restart();
          Tracer.Warning("Connected device was null, starting retry {0}", (object) num);
        }
        while (num < 11);
      }
      else
      {
        Console.WriteLine("Operation failed. No device selected with given criteria.");
        throw new NullReferenceException("Connected device not found.");
      }
    }

    private void GetLastConnectedDevice()
    {
      DeviceInfo propertySet = new DeviceInfoSet()
      {
        DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
        Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
      }.EnumeratePresentDevices().Last<DeviceInfo>((Func<DeviceInfo, bool>) (deviceInfo => (uint) deviceInfo.DeviceType > 0U));
      if (propertySet != null)
      {
        this.DevicePath = propertySet.Path;
        Tracer.Information("Last device: {0}", (object) propertySet.Path);
        Console.WriteLine("Device detected");
      }
      Tracer.Information("No -path parameter given. Autodetecting device.");
      LucidConnectivityHelper.ParseTypeDesignatorAndSalesName(propertySet.ReadBusReportedDeviceDescription(), out string _, out string _);
      if (string.IsNullOrEmpty(this.DevicePath) && string.IsNullOrEmpty(this.PortId))
        throw new NullReferenceException("Connected device not found.");
    }

    private static void PrintHelp()
    {
      Console.WriteLine("DeviceModeChanger.exe");
      Console.WriteLine("Parameters");
      Console.WriteLine("-path=[device]   Device path for the device that is connected.");
      Console.WriteLine("-help            Shows help");
      Console.WriteLine("-nowait          Exits after sending swich mode command. (in Flash -> Test as early as possible.");
      Console.WriteLine("-afterreboot     Wait for device in UEFI mode and then change mode.");
      Console.WriteLine("-rm=[method]     Reset method");
      Console.WriteLine();
      Console.WriteLine("              ** Methods **");
      Console.WriteLine("                 s = sw reset");
      Console.WriteLine("                 h = hw reset (default)");
      Console.WriteLine();
      Console.WriteLine("-to=[desired mode]    Specifies the target mode.");
      Console.WriteLine();
      Console.WriteLine("              ** Modes **");
      Console.WriteLine("                 n = normal");
      Console.WriteLine("                 f and u = flash");
      Console.WriteLine("                 l and t = test");
      Console.WriteLine("                 m and mm = mass memory");
      Console.WriteLine();
    }

    private static string GetJsonModeFromConnectedDeviceMode(ConnectedDeviceMode mode)
    {
      switch (mode)
      {
        case ConnectedDeviceMode.Normal:
          return "Normal";
        case ConnectedDeviceMode.Label:
          return "Test";
        case ConnectedDeviceMode.Uefi:
          return "Flash";
        default:
          return "PowerOff";
      }
    }

    private static ConnectedDeviceMode GetDesiredConnectedDeviceMode(string mode)
    {
      if (mode == "n" || mode == "normal")
        return ConnectedDeviceMode.Normal;
      if (mode == "f" || mode == "u" || (mode == "flashapp" || mode == "uefi"))
        return ConnectedDeviceMode.Uefi;
      if (mode == "l" || mode == "t" || (mode == "label" || mode == "test"))
        return ConnectedDeviceMode.Label;
      return mode == "m" || mode == "mm" || (mode == "massmemory" || mode == "massstorage") ? ConnectedDeviceMode.MassStorage : ConnectedDeviceMode.Unknown;
    }

    private void StartMmos()
    {
      Tracer.Information("Start MMOS");
      this.SendRawMessage(new byte[4]
      {
        (byte) 78,
        (byte) 79,
        (byte) 75,
        (byte) 89
      });
    }

    private bool DisableBootManagerTimeouts()
    {
      Tracer.Information("Disable BootManager timeouts");
      byte[] message = new byte[4]
      {
        (byte) 78,
        (byte) 79,
        (byte) 75,
        (byte) 68
      };
      byte[] numArray = new byte[1024];
      return this.SendAndReceiveRawMessage(message)[3] == (byte) 68;
    }

    private bool IsBootManagerRunning()
    {
      byte[] message = new byte[4]
      {
        (byte) 78,
        (byte) 79,
        (byte) 75,
        (byte) 86
      };
      byte[] numArray = new byte[1024];
      byte[] rawMessage = this.SendAndReceiveRawMessage(message);
      return rawMessage[3] == (byte) 86 && rawMessage[5] == (byte) 1;
    }

    private void RebootDeviceInUefiMode()
    {
      Console.WriteLine("Reboot device in Uefi mode");
      Tracer.Information("Reboot device in Uefi mode");
      this.SendRawMessage(new byte[4]
      {
        (byte) 78,
        (byte) 79,
        (byte) 75,
        (byte) 82
      });
    }

    private void ConnectedDeviceOnDeviceReadyChanged(
      object sender,
      DeviceReadyChangedEventArgs deviceReadyChangedEventArgs)
    {
      Console.WriteLine("[{0}] Device is ready: {2}. Device in {1} mode", (object) nameof (DeviceModeChanger), (object) deviceReadyChangedEventArgs.ConnectedDevice.Mode, (object) deviceReadyChangedEventArgs.DeviceReady);
      if (deviceReadyChangedEventArgs.ConnectedDevice != this.connectedDevice || this.connectedDevice.Mode != this.DesiredMode || !deviceReadyChangedEventArgs.DeviceReady)
        return;
      this.deviceInTargetModeDetected = true;
    }

    private void ConnectedDevicesOnDeviceModeChanged(
      object sender,
      DeviceModeChangedEventArgs modeChangedEventArgs)
    {
      Console.WriteLine("[{0}] Device detected in {1} mode", (object) nameof (DeviceModeChanger), (object) modeChangedEventArgs.ConnectedDevice.Mode);
      if (this.AfterReboot && modeChangedEventArgs.ConnectedDevice.Mode == ConnectedDeviceMode.Uefi)
      {
        this.AfterReboot = false;
      }
      else
      {
        if (this.connectedDevice != modeChangedEventArgs.ConnectedDevice || this.DesiredMode != modeChangedEventArgs.ConnectedDevice.Mode)
          return;
        if (this.DesiredMode == modeChangedEventArgs.ConnectedDevice.Mode && this.DesiredMode == ConnectedDeviceMode.MassStorage)
          this.deviceInTargetModeDetected = true;
        Console.WriteLine("[{0}] Waiting for communication to be available", (object) nameof (DeviceModeChanger));
      }
    }

    private void SetDeviceMode(ConnectedDeviceMode currentMode, ConnectedDeviceMode targetMode)
    {
      if (targetMode == ConnectedDeviceMode.MassStorage)
      {
        this.ChangeDeviceModeToMassMemory();
      }
      else
      {
        switch (currentMode)
        {
          case ConnectedDeviceMode.Normal:
            Tracer.Information("Current device mode is Normal");
            this.SwitchDeviceModeFromTo("Normal", DeviceModeChanger.GetJsonModeFromConnectedDeviceMode(targetMode));
            break;
          case ConnectedDeviceMode.Label:
            Tracer.Information("Current device mode is Label");
            this.SwitchDeviceModeFromTo("Test", DeviceModeChanger.GetJsonModeFromConnectedDeviceMode(targetMode));
            break;
          case ConnectedDeviceMode.Uefi:
            Tracer.Information("Current device mode is UEFI");
            this.SwitchDeviceModeFromUefiTo(targetMode);
            break;
          default:
            Tracer.Information("Current device mode is '{0}'", (object) currentMode);
            Console.WriteLine("Device is currently in {0} mode and mode change for that mode is not supported.", (object) currentMode);
            break;
        }
      }
    }

    private void SwitchDeviceModeFromUefiTo(ConnectedDeviceMode targetMode)
    {
      if (this.connectedDevice == null)
        return;
      this.RebootDeviceInUefiMode();
      Console.WriteLine("[{0}] Mode change from {1} to {2} started", (object) nameof (DeviceModeChanger), (object) "Flash", (object) targetMode);
      if (targetMode != ConnectedDeviceMode.Label)
        return;
      try
      {
        this.connectedDevices.DeviceModeChanged -= new EventHandler<DeviceModeChangedEventArgs>(this.ConnectedDevicesOnDeviceModeChanged);
        this.connectedDevices.DeviceConnected += new EventHandler<DeviceConnectedEventArgs>(this.CatchDeviceInBootManagerAndCommandToTestMode);
        do
        {
          Thread.Sleep(500);
        }
        while (!this.bootToLabelCommandSentInBootManager);
      }
      finally
      {
        this.connectedDevices.DeviceConnected -= new EventHandler<DeviceConnectedEventArgs>(this.CatchDeviceInBootManagerAndCommandToTestMode);
        this.connectedDevices.DeviceModeChanged += new EventHandler<DeviceModeChangedEventArgs>(this.ConnectedDevicesOnDeviceModeChanged);
      }
    }

    private void CatchDeviceInBootManagerAndCommandToTestMode(
      object sender,
      DeviceConnectedEventArgs e)
    {
      Tracer.Information("DeviceConnected event handled. Path: {0} mode: {1}", (object) e.ConnectedDevice.DevicePath, (object) e.ConnectedDevice.Mode);
      if (e.ConnectedDevice != this.connectedDevice || e.ConnectedDevice.Mode != ConnectedDeviceMode.Uefi)
        return;
      this.WaitForUefiAndSwitchToLabel();
    }

    private void WaitForUefiAndSwitchToLabel()
    {
      Tracer.Information("WaitForUefiAndSwitchToLabel called");
      while (this.connectedDevice == null)
        Thread.Sleep(1000);
      int num = 20;
      while (num >= 0)
      {
        if (this.forceExit)
        {
          Tracer.Information("Force exit.");
          return;
        }
        try
        {
          if (!this.IsBootManagerRunning())
            throw new Exception("BootManager is not running yet.");
          Tracer.Information("BootManager is up and running.");
          break;
        }
        catch (Exception ex)
        {
          if (num-- == 0)
          {
            Tracer.Information("BootManager is not running. No retries left.");
            throw;
          }
          else
          {
            Thread.Sleep(1000);
            Tracer.Information("BootManager is not running yet. Retries left {0}.", (object) num);
          }
        }
      }
      Tracer.Information(this.DisableBootManagerTimeouts() ? "Disable BootManagerTimeouts command successful" : "Disable BootManagerTimeouts failed");
      this.StartMmos();
      this.bootToLabelCommandSentInBootManager = true;
    }

    private void SwitchDeviceModeFromTo(string currentMode, string targetMode)
    {
      Tracer.Information("SwitchDeviceModeFromTo:  {0}", (object) targetMode);
      if (currentMode == targetMode)
        return;
      string message = "{\"jsonrpc\": \"2.0\", \"id\": 2,\"method\": \"SetDeviceMode\", \"params\": {\"MessageVersion\": 0, \"DeviceMode\": \"" + targetMode + "\"}}";
      Console.WriteLine("[{0}] Mode change from {1} to {2} started", (object) nameof (DeviceModeChanger), (object) currentMode, (object) targetMode);
      try
      {
        this.SendAndReceiveMessage(message);
      }
      catch (Exception ex)
      {
        Tracer.Error(ex, "Error when communicating with device");
        Console.WriteLine("Unable to send data to device via USB pipe.");
        throw new IOException("Unable to send data to device via USB pipe.", ex);
      }
    }

    private string SendAndReceiveMessage(string message)
    {
      using (Nokia.Lucid.UsbDeviceIo.UsbDeviceIo usbDeviceIo = new Nokia.Lucid.UsbDeviceIo.UsbDeviceIo(this.connectedDevice.DevicePath))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        usbDeviceIo.Send(bytes, (uint) bytes.Length);
        byte[] receivedData;
        int num = (int) usbDeviceIo.Receive(out receivedData, TimeSpan.FromSeconds(5.0));
        return Encoding.Default.GetString(receivedData);
      }
    }

    private byte[] SendAndReceiveRawMessage(byte[] message)
    {
      using (Nokia.Lucid.UsbDeviceIo.UsbDeviceIo usbDeviceIo = new Nokia.Lucid.UsbDeviceIo.UsbDeviceIo(this.connectedDevice.DevicePath))
      {
        usbDeviceIo.Send(message, (uint) message.Length);
        byte[] receivedData;
        int num = (int) usbDeviceIo.Receive(out receivedData, TimeSpan.FromSeconds(5.0));
        return receivedData;
      }
    }

    private void SendRawMessage(byte[] message)
    {
      using (Nokia.Lucid.UsbDeviceIo.UsbDeviceIo usbDeviceIo = new Nokia.Lucid.UsbDeviceIo.UsbDeviceIo(this.connectedDevice.DevicePath))
        usbDeviceIo.Send(message, (uint) message.Length);
    }

    private void ChangeDeviceModeToMassMemory()
    {
      Tracer.Information("ChangeDeviceModeToMassMemory started on port: {0}", (object) this.PortId);
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      string logFilePath = TraceWriter.GenerateLogFilePath("thor2_massmemory");
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode rnd -bootmsc -skipffuflash -conn \"{0}\" -logfile \"{1}\"", (object) this.PortId, (object) logFilePath),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      processHelper.Start();
      processHelper.BeginOutputReadLine();
      processHelper.WaitForExit();
      if (processHelper.ExitCode == 1)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104902L,
          UriDescription = "Thor2 crash - DeviceModeChanger crash"
        }, DateTime.Now);
      this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) processHelper.ExitCode, (object) this.operationExitCode));
      switch (this.GetErrorType(this.operationExitCode))
      {
        case Thor2ErrorType.NoError:
          break;
        case Thor2ErrorType.Thor2Error:
          throw this.HandleThor2Error(this.operationExitCode);
        case Thor2ErrorType.DeviceError:
          throw this.HandleDeviceError(this.operationExitCode);
        case Thor2ErrorType.FlashAppError:
          throw this.HandleFlashAppError(this.operationExitCode);
        case Thor2ErrorType.FfuProgrammingVer2Error:
          throw this.HandleVersion2Error(this.operationExitCode);
        default:
          Tracer.Information(str);
          throw new FlashException((uint) this.operationExitCode, str);
      }
    }

    private void OperationOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      try
      {
        if (dataReceivedEventArgs == null || string.IsNullOrEmpty(dataReceivedEventArgs.Data))
          return;
        Tracer.Information("Thor output [{0}]:{1}", (object) this.PortId, (object) dataReceivedEventArgs.Data);
        if (!dataReceivedEventArgs.Data.Contains("not possible"))
          return;
        Console.WriteLine("[{0}] Operation failed: {1}", (object) nameof (DeviceModeChanger), (object) dataReceivedEventArgs.Data);
      }
      catch (Exception ex)
      {
        Tracer.Warning("Error parsing Thor2 output. Unable to parse string: {0}, exception {1}", (object) dataReceivedEventArgs.Data, (object) ex.Message);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private Thor2ErrorType GetErrorType(Thor2ExitCode exitCode)
    {
      uint num = (uint) exitCode;
      if (num == 0U)
      {
        Tracer.Information("Flash process exited with no error");
        return Thor2ErrorType.NoError;
      }
      if (num >= 84000U && num < 85000U)
      {
        Tracer.Error("THOR2 error occured");
        return Thor2ErrorType.Thor2Error;
      }
      if (num >= 131072U && num < 196608U)
      {
        Tracer.Error("File error occured");
        return Thor2ErrorType.FileError;
      }
      if (num >= 196608U && num < 262144U)
      {
        Tracer.Error("Device error occured");
        return Thor2ErrorType.DeviceError;
      }
      if (num >= 262144U && num < 327680U)
      {
        Tracer.Error("Message error occured");
        return Thor2ErrorType.MessageError;
      }
      if (num >= 327680U && num < 393216U)
      {
        Tracer.Error("Messaging error occured");
        return Thor2ErrorType.MessagingError;
      }
      if (num >= 393216U && num < 2228224U)
      {
        Tracer.Error("Device reported ver 2 error during FFU programming");
        return Thor2ErrorType.FfuProgrammingVer2Error;
      }
      if (num >= 2228224U && num < 2293760U)
      {
        Tracer.Error("FFU parsing error occured");
        return Thor2ErrorType.FfuParsingError;
      }
      if (num >= 4194304000U && num < 4211081215U)
      {
        Tracer.Error("FlashApp error occured");
        return Thor2ErrorType.FlashAppError;
      }
      Tracer.Error("Unhandled error occured");
      return Thor2ErrorType.UnhandledError;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private Exception HandleThor2Error(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      switch (thor2ExitCode)
      {
        case Thor2ExitCode.Thor2ErrorSecureFfuNotSupported:
          return (Exception) new SecureFlashProtocolNotSupportedException(errorCode, message);
        case Thor2ExitCode.Thor2ErrorFactoryResetFailed:
          return (Exception) new FactoryResetFailedException(errorCode, message);
        case Thor2ExitCode.Thor2ErrorUefiDoesNotSupportFullNviUpdate:
          return (Exception) new FullNviUpdateNotSupportedException(errorCode, message);
        case Thor2ExitCode.Thor2ErrorUefiDoesNotSupportProductCodeUpdate:
          return (Exception) new ProductCodeChangeNotSupportedException(errorCode, message);
        default:
          return (Exception) new FlashException(errorCode, message);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private Exception HandleDeviceError(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      switch (thor2ExitCode)
      {
        case Thor2ExitCode.DevRkhMismatchError:
          return (Exception) new RkhCheckException(RkhCheckException.RhkCheckIssue.RkhMismatch, errorCode, str);
        case Thor2ExitCode.DevSbl1NotSigned:
          return (Exception) new RkhCheckException(RkhCheckException.RhkCheckIssue.BootLoadersNotSigned, errorCode, str);
        case Thor2ExitCode.DevUefiNotSigned:
          return (Exception) new RkhCheckException(RkhCheckException.RhkCheckIssue.UefiNotSigned, errorCode, str);
        default:
          return (Exception) new FlashException(errorCode, str);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private Exception HandleVersion2Error(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      return thor2ExitCode == (Thor2ExitCode) 397315 ? (Exception) new SecureFlashHashException(errorCode, message) : (Exception) new FlashException(errorCode, message);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private Exception HandleFlashAppError(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      switch (thor2ExitCode)
      {
        case Thor2ExitCode.FaErrAuthenticationRequired:
          return (Exception) new SecureFlashAuthenticationException(errorCode, message);
        case Thor2ExitCode.FaErrFfuHashFail:
          return (Exception) new SecureFlashHashException(errorCode, message);
        case Thor2ExitCode.FaErrFfuSecHdrValidationFail:
          return (Exception) new SecureFlashSignatureException(errorCode, message);
        case Thor2ExitCode.FaErrFfuStrHdrInvalidPlatformId:
          return (Exception) new SecureFlashPlatformIdException(errorCode, message);
        default:
          return (Exception) new FlashException(errorCode, message);
      }
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
  }
}
