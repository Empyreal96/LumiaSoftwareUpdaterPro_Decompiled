// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.PiaReader
// Assembly: PiaReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 27422629-2045-43F0-B2EB-AE7A366F98F1
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\PiaReader.dll

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using Microsoft.LumiaConnectivity.EventArgs;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class PiaReader
  {
    private const string GenericErrorMessage = "Operation failed with error {0}";
    private Thor2ExitCode operationExitCode;
    private PiaPhoneInfo piaPhoneInfo;
    private ConnectedDevices connectedDevices;
    private bool deviceDetected;

    public PiaReader()
    {
      this.piaPhoneInfo = new PiaPhoneInfo();
      this.connectedDevices = new ConnectedDevices();
    }

    public event EventHandler<PiaReaderOperationStateUpdatedEventArgs> OperationStateUpdated;

    public event EventHandler<ErrorEventArgs> ErrorOccured;

    public event EventHandler<PiaPhoneInfoEventArgs> OperationCompletedSuccessfully;

    private string PortId { get; set; }

    public int ThorProcessId { get; private set; }

    public void ReadPiaInfo(string givenPortId = null)
    {
      string thorLogfile = TraceWriter.GenerateLogFilePath("thor2_pia");
      Tracer.Information("Thor log file: {0}", (object) thorLogfile);
      this.connectedDevices.Start();
      if (givenPortId != null)
      {
        this.deviceDetected = true;
        this.PortId = givenPortId;
      }
      else
        this.connectedDevices.DeviceConnected += new EventHandler<DeviceConnectedEventArgs>(this.HandleDeviceConnected);
      TaskHelper task = new TaskHelper((Action) (() => this.ExecuteReadPiaInfo(thorLogfile)));
      Tracer.Information("Starting to read PIA info on port {0}", (object) this.PortId);
      task.Start();
      task.ContinueWith((Action<object>) (t =>
      {
        if (task.Exception == null)
          return;
        foreach (Exception innerException in task.Exception.InnerExceptions)
        {
          Tracer.Error("Port {0}, error: {1}", (object) this.PortId, (object) innerException.Message);
          this.OnErrorOccured(new ErrorEventArgs(innerException));
        }
      }), TaskContinuationOptions.OnlyOnFaulted);
      Tracer.Information("Read PIA info {0} completed", (object) this.PortId);
    }

    public void SwitchToFlashApp(string givenPortId = null)
    {
      string thorLogfile = TraceWriter.GenerateLogFilePath("thor2_flashapp");
      Tracer.Information("Thor log file: {0}", (object) thorLogfile);
      this.connectedDevices.Start();
      if (givenPortId != null)
      {
        this.deviceDetected = true;
        this.PortId = givenPortId;
      }
      else
        this.connectedDevices.DeviceConnected += new EventHandler<DeviceConnectedEventArgs>(this.HandleDeviceConnected);
      TaskHelper task = new TaskHelper((Action) (() => this.ExecuteSwitchToFlashApp(thorLogfile)));
      Tracer.Information("Starting to switch device to flash app {0}", (object) this.PortId);
      task.Start();
      task.ContinueWith((Action<object>) (t =>
      {
        if (task.Exception == null)
          return;
        foreach (Exception innerException in task.Exception.InnerExceptions)
        {
          Tracer.Error("Port {0}, error: {1}", (object) this.PortId, (object) innerException.Message);
          this.OnErrorOccured(new ErrorEventArgs(innerException));
        }
      }), TaskContinuationOptions.OnlyOnFaulted);
      Tracer.Information("Switch to flash app completed {0}", (object) this.PortId);
    }

    private void ExecuteReadPiaInfo(string thorLogfile)
    {
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      TaskHelper task = new TaskHelper((Action) (() => this.ExecuteWaitForUefi()));
      Tracer.Information("Waiting for device in UEFI mode");
      task.Start();
      task.ContinueWith((Action<object>) (t =>
      {
        if (task.Exception == null)
          return;
        foreach (Exception innerException in task.Exception.InnerExceptions)
        {
          Tracer.Error("Error: {0}", (object) innerException.Message);
          this.OnErrorOccured(new ErrorEventArgs(innerException));
        }
      }), TaskContinuationOptions.OnlyOnFaulted);
      task.Wait();
      Tracer.Information("Device in UEFI mode detected on port {0}", (object) this.PortId);
      Thread.Sleep(3000);
      int processHelperExitCode = this.StartPiaAppInUefi(thorLogfile, directoryName);
      Thread.Sleep(500);
      if (this.GetErrorType(this.operationExitCode) == Thor2ErrorType.NoError)
      {
        processHelperExitCode = this.ReadPhoneInfoFromPia(thorLogfile, directoryName, processHelperExitCode);
        Thread.Sleep(500);
        if (this.GetErrorType(this.operationExitCode) == Thor2ErrorType.NoError)
          processHelperExitCode = this.SwitchToFlashAppFromPia(thorLogfile, directoryName, processHelperExitCode);
      }
      this.HandleErrors(processHelperExitCode);
    }

    private void HandleErrors(int processHelperExitCode)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) processHelperExitCode, (object) this.operationExitCode));
      switch (this.GetErrorType(this.operationExitCode))
      {
        case Thor2ErrorType.NoError:
          this.OnOperationCompletedSuccessfully(new PiaPhoneInfoEventArgs(this.piaPhoneInfo));
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

    private int SwitchToFlashAppFromPia(
      string thorLogfile,
      string workingDirectory,
      int processHelperExitCode)
    {
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(workingDirectory, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode rnd -bootflashapp -conn \"{0}\" -logfile \"{1}\"", (object) this.PortId, (object) thorLogfile),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = workingDirectory
      };
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      processHelper.Start();
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.SwitchingToFlashModeStarted));
      processHelper.BeginOutputReadLine();
      this.ThorProcessId = processHelper.Id;
      processHelper.WaitForExit(190000);
      try
      {
        this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
        if (this.operationExitCode == Thor2ExitCode.Thor2UnexpectedExit)
          ReportSender.SaveReportAsync(new ReportDetails()
          {
            Uri = 104903L,
            UriDescription = "Thor2 crash - PiaReader crash"
          }, DateTime.Now);
      }
      catch (Exception ex)
      {
        Tracer.Information("Thor process was not closed: {0}", (object) ex.Message);
        if (!processHelper.HasExited)
        {
          processHelper.Kill();
          Tracer.Information("Thor process was killed");
        }
      }
      this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
      processHelperExitCode = processHelper.ExitCode;
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.SwitchingToFlashModeCompleted));
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      return processHelperExitCode;
    }

    private int ReadPhoneInfoFromPia(
      string thorLogfile,
      string workingDirectory,
      int processHelperExitCode)
    {
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(workingDirectory, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode rnd -readphoneinfo -conn \"{0}\" -logfile \"{1}\"", (object) this.PortId, (object) thorLogfile),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = workingDirectory
      };
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      processHelper.Start();
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.ReadingPiaStarted));
      processHelper.BeginOutputReadLine();
      Thread.Sleep(3000);
      this.ThorProcessId = processHelper.Id;
      processHelper.WaitForExit(190000);
      try
      {
        this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
        if (this.operationExitCode == Thor2ExitCode.Thor2UnexpectedExit)
          ReportSender.SaveReportAsync(new ReportDetails()
          {
            Uri = 104903L,
            UriDescription = "Thor2 crash - PiaReader crash"
          }, DateTime.Now);
      }
      catch (Exception ex)
      {
        Tracer.Information("Thor process was not closed: {0}", (object) ex.Message);
        if (!processHelper.HasExited)
        {
          processHelper.Kill();
          Tracer.Information("Thor process was killed");
        }
      }
      this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
      processHelperExitCode = processHelper.ExitCode;
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.ReadingPiaCompleted));
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      return processHelperExitCode;
    }

    private int StartPiaAppInUefi(string thorLogfile, string workingDirectory)
    {
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(workingDirectory, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode rnd -bootphoneinfoapp -conn \"{0}\" -logfile \"{1}\"", (object) this.PortId, (object) thorLogfile),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = workingDirectory
      };
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      processHelper.Start();
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.SwitchingToPiaStarted));
      processHelper.BeginOutputReadLine();
      this.ThorProcessId = processHelper.Id;
      processHelper.WaitForExit(190000);
      try
      {
        this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
        if (this.operationExitCode == Thor2ExitCode.Thor2UnexpectedExit)
          ReportSender.SaveReportAsync(new ReportDetails()
          {
            Uri = 104903L,
            UriDescription = "Thor2 crash - PiaReader crash"
          }, DateTime.Now);
      }
      catch (Exception ex)
      {
        Tracer.Information("Thor process was not closed: {0}", (object) ex.Message);
        if (!processHelper.HasExited)
        {
          processHelper.Kill();
          Tracer.Information("Thor process was killed");
        }
      }
      this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
      int exitCode = processHelper.ExitCode;
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.SwitchingToPiaCompleted));
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      return exitCode;
    }

    private void ExecuteSwitchToFlashApp(string thorLogfile)
    {
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      TaskHelper task = new TaskHelper((Action) (() => this.ExecuteWaitForUefi()));
      Tracer.Information("Waiting for device in UEFI mode");
      task.Start();
      task.ContinueWith((Action<object>) (t =>
      {
        if (task.Exception == null)
          return;
        foreach (Exception innerException in task.Exception.InnerExceptions)
        {
          Tracer.Error("Error: {0}", (object) innerException.Message);
          this.OnErrorOccured(new ErrorEventArgs(innerException));
        }
      }), TaskContinuationOptions.OnlyOnFaulted);
      task.Wait();
      Tracer.Information("Device in UEFI mode detected on port {0}", (object) this.PortId);
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode rnd -bootflashapp -conn \"{0}\" -logfile \"{1}\"", (object) this.PortId, (object) thorLogfile),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      processHelper.Start();
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.SwitchingToFlashModeStarted));
      processHelper.BeginOutputReadLine();
      processHelper.WaitForExit();
      if (processHelper.ExitCode == 1)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104903L,
          UriDescription = "Thor2 crash - PiaReader crash"
        }, DateTime.Now);
      this.operationExitCode = (Thor2ExitCode) processHelper.ExitCode;
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.SwitchingToFlashModeCompleted));
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.OperationOnOutputDataReceived);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) processHelper.ExitCode, (object) this.operationExitCode));
      switch (this.GetErrorType(this.operationExitCode))
      {
        case Thor2ErrorType.NoError:
          this.OnOperationCompletedSuccessfully(new PiaPhoneInfoEventArgs(this.piaPhoneInfo));
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

    private void ExecuteWaitForUefi()
    {
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.DetectingDevice));
      int num = 0;
      while (!this.deviceDetected)
      {
        Thread.Sleep(500);
        ++num;
        if (num >= 60)
        {
          this.connectedDevices.Stop();
          throw new Exception("Timeout when waiting for device in UEFI");
        }
      }
      this.connectedDevices.Stop();
      this.OnStateUpdated(new PiaReaderOperationStateUpdatedEventArgs(PiaReaderOperationState.DeviceDetected));
    }

    private void OperationOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      try
      {
        if (string.IsNullOrEmpty(dataReceivedEventArgs.Data))
          return;
        Tracer.Information("Thor output [{0}]:{1}", (object) this.PortId, (object) dataReceivedEventArgs.Data);
        if (dataReceivedEventArgs.Data.Contains("TYPE:"))
          this.piaPhoneInfo.ProductType = dataReceivedEventArgs.Data.Remove(0, 5).Trim();
        if (!dataReceivedEventArgs.Data.Contains("CTR:"))
          return;
        this.piaPhoneInfo.ProductCode = dataReceivedEventArgs.Data.Remove(0, 4).Trim();
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

    private void HandleDeviceConnected(object sender, DeviceConnectedEventArgs e)
    {
      Tracer.Information("HandleDeviceConnected: {0}, {1}", (object) e.ConnectedDevice.PortId, (object) e.ConnectedDevice.Mode);
      if (e.ConnectedDevice.Mode != ConnectedDeviceMode.Uefi)
        return;
      this.PortId = e.ConnectedDevice.PortId;
      this.deviceDetected = true;
    }

    protected virtual void OnOperationCompletedSuccessfully(PiaPhoneInfoEventArgs e)
    {
      EventHandler<PiaPhoneInfoEventArgs> completedSuccessfully = this.OperationCompletedSuccessfully;
      if (completedSuccessfully == null)
        return;
      completedSuccessfully((object) this, e);
    }

    protected virtual void OnStateUpdated(PiaReaderOperationStateUpdatedEventArgs e)
    {
      EventHandler<PiaReaderOperationStateUpdatedEventArgs> operationStateUpdated = this.OperationStateUpdated;
      if (operationStateUpdated == null)
        return;
      operationStateUpdated((object) this, e);
    }

    protected virtual void OnErrorOccured(ErrorEventArgs e)
    {
      EventHandler<ErrorEventArgs> errorOccured = this.ErrorOccured;
      if (errorOccured == null)
        return;
      errorOccured((object) this, e);
    }
  }
}
