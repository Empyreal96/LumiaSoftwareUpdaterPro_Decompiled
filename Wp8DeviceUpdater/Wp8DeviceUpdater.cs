// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Wp8DeviceUpdater
// Assembly: Wp8DeviceUpdater, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 88C20D73-1EDE-4FB2-B734-F8968E9CB6A0
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceUpdater.dll

using Microsoft.LsuPro.Cryptography;
using Microsoft.LsuPro.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class Wp8DeviceUpdater
  {
    private const string GenericErrorMessage = "Flashing failed with error {0}";
    private const string FactoryResetResultIdentifier = "[Factory reset result]";
    private const string FullNviUpdateResultIdentifier = "[Full NVI update result]";
    private const string ProductCodeUpdateResultIdentifier = "[Product code update result]";
    private const string SafeWriteDescriptorIndexNotReached = "Safe write descriptor index reached: false";
    private bool deviceUpdateOperationInProgress;
    private bool skipWrite;
    private bool skipPlatformIdCheck;
    private bool skipHashCheck;
    private bool skipSignatureCheck;
    private bool nonSecureFlashing;
    private bool performFactoryReset;
    private bool powerOffAfterFlash;
    private bool doFullNviUpdate;
    private string targetProductCode;
    private int lastProgressPercentage;
    private long lastTransferredBytes;
    private long lastTotalBytes;
    private double lastTransferSpeed;
    private double cumulativeTransferSpeed;
    private int transferSpeedCalculations;
    private bool wasAborted;
    private bool safePointReached;
    private bool useCustomBlockSize;
    private uint customBlockSize;
    private Dictionary<PostFlashOperation, Exception> postFlashOperationResults;
    private Thor2ExitCode flashProcessExitCode;

    public Wp8DeviceUpdater(string portId)
    {
      this.PortId = portId;
      this.postFlashOperationResults = new Dictionary<PostFlashOperation, Exception>();
      this.TraceUsb = false;
      this.useCustomBlockSize = false;
      this.ThorVersion = Toolkit.DetermineThor2Version();
      this.ProductTypeInfo = string.Empty;
    }

    public event EventHandler<FlashProgressUpdatedEventArgs> FlashProgressUpdated;

    public event EventHandler<FlashStateUpdatedEventArgs> FlashStateUpdated;

    public event EventHandler<ErrorEventArgs> FlashErrorOccured;

    public event EventHandler<EventArgs> FlashCompletedSuccessfully;

    public string FfuFile { get; set; }

    public byte[] ExpectedCrc32 { get; set; }

    public string ThorVersion { get; private set; }

    public string PortId { get; set; }

    public bool TraceUsb { get; set; }

    public double AverageFlashingSpeed => this.transferSpeedCalculations <= 0 ? 0.0 : this.cumulativeTransferSpeed / (double) this.transferSpeedCalculations;

    public void SetMaxPayloadTransferSize(string transferSizeKb)
    {
      this.useCustomBlockSize = uint.TryParse(transferSizeKb, out this.customBlockSize);
      if (this.useCustomBlockSize)
        Tracer.Information("Using custom payload transfer size: {0} KB", (object) this.customBlockSize);
      else
        Tracer.Information("Using default payload transfer size");
    }

    private int ThorProcessId { get; set; }

    public bool CanStartDeviceUpdateOperation => !this.deviceUpdateOperationInProgress && Wp8DeviceUpdater.FfuFileAvailable(this.FfuFile);

    public bool DeviceUpdateOperationInProgress => this.deviceUpdateOperationInProgress;

    public Dictionary<PostFlashOperation, Exception> PostFlashOperationResults => this.postFlashOperationResults;

    public static bool FfuFileAvailable(string fakeflashfileFfu) => File.Exists(fakeflashfileFfu);

    public void StartDeviceUpdate(
      bool skipWrite = false,
      bool skipPlatformIdCheck = false,
      bool skipSignatureCheck = false,
      bool skipHashCheck = false,
      bool nonSecureFlashing = false,
      bool performFactoryReset = false,
      bool powerOffAfterFlash = false,
      string targetProductCode = "",
      string rdcBackupFileName = "",
      bool limitedtransferspeed = true,
      bool doFullNviUpdate = true)
    {
      this.skipWrite = skipWrite;
      this.skipPlatformIdCheck = skipPlatformIdCheck;
      this.skipHashCheck = skipHashCheck;
      this.skipSignatureCheck = skipSignatureCheck;
      this.nonSecureFlashing = nonSecureFlashing;
      this.performFactoryReset = performFactoryReset;
      this.powerOffAfterFlash = powerOffAfterFlash;
      this.targetProductCode = targetProductCode;
      this.doFullNviUpdate = doFullNviUpdate;
      string str = string.Empty;
      if (this.ProductTypeInfo.Contains("RM-"))
        str = "_" + this.ProductTypeInfo;
      string thorLogfile = TraceWriter.GenerateLogFilePath("thor2" + str);
      Tracer.Information("Thor log file: {0}", (object) thorLogfile);
      TaskHelper deviceUpdateTask = new TaskHelper((Action) (() => this.ExecuteDeviceUpdateOperation(this.FfuFile, this.PortId, thorLogfile, rdcBackupFileName, limitedtransferspeed)));
      this.deviceUpdateOperationInProgress = true;
      Tracer.Information("Starting to flash device on port {0} with file {1}", (object) this.PortId, (object) this.FfuFile);
      deviceUpdateTask.Start();
      deviceUpdateTask.ContinueWith((Action<object>) (t =>
      {
        if (deviceUpdateTask.Exception != null)
        {
          foreach (Exception innerException in deviceUpdateTask.Exception.InnerExceptions)
          {
            Tracer.Error("Port {0}, Flash error: {2}. File used in flashing {1}", (object) this.PortId, (object) this.FfuFile, (object) innerException.Message);
            this.OnFlashErrorOccured(new ErrorEventArgs(innerException));
          }
        }
        this.deviceUpdateOperationInProgress = false;
        Tracer.Information("Flash device on port {0} completed", (object) this.PortId);
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    public bool WasAborted => this.wasAborted;

    public string ProductTypeInfo { get; set; }

    public void AbortDeviceUpdate()
    {
      try
      {
        Process processById = ProcessHelper.GetProcessById(this.ThorProcessId);
        if (!processById.ProcessName.Equals("thor2", StringComparison.OrdinalIgnoreCase))
          return;
        Tracer.Information("Killing process {0}", (object) this.ThorProcessId);
        processById.Kill();
        Tracer.Information("Process {0} killed", (object) this.ThorProcessId);
        this.wasAborted = true;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[1]
        {
          (object) this.ThorProcessId
        };
        Tracer.Error(ex, "Aborting device update process {0} failed", objArray);
      }
    }

    internal void FlashProcessOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      try
      {
        if (string.IsNullOrEmpty(dataReceivedEventArgs.Data))
          return;
        Tracer.Information("Thor output [{0}]:{1}", (object) this.PortId, (object) dataReceivedEventArgs.Data);
        if (dataReceivedEventArgs.Data.Contains("Percents"))
        {
          this.lastProgressPercentage = int.Parse(dataReceivedEventArgs.Data.Substring(9), (IFormatProvider) CultureInfo.InvariantCulture);
          this.OnFlashProgressUpdated(new FlashProgressUpdatedEventArgs(this.lastProgressPercentage, this.lastTransferredBytes, this.lastTotalBytes, this.lastTransferSpeed));
        }
        else
        {
          if (dataReceivedEventArgs.Data.Contains("Detailed progress: "))
          {
            Toolkit.ParseDetailedProgress(dataReceivedEventArgs.Data, out this.lastTransferredBytes, out this.lastTotalBytes, out this.lastTransferSpeed);
            this.cumulativeTransferSpeed += this.lastTransferSpeed;
            ++this.transferSpeedCalculations;
            this.OnFlashProgressUpdated(new FlashProgressUpdatedEventArgs(this.lastProgressPercentage, this.lastTransferredBytes, this.lastTotalBytes, this.lastTransferSpeed));
          }
          if (dataReceivedEventArgs.Data.Contains("Flashed section") || dataReceivedEventArgs.Data.Contains("Skipped section"))
          {
            string[] strArray = dataReceivedEventArgs.Data.Substring(16).Split('/');
            if (strArray.Length != 2)
              return;
            this.OnFlashProgressUpdated(new FlashProgressUpdatedEventArgs(int.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture) * 100 / int.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), 0L, 0L, 0.0));
          }
          else
          {
            if (dataReceivedEventArgs.Data.Contains("[THOR2_flash_state] Switching to flash mode"))
              this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.SwitchingToFlashMode));
            if (dataReceivedEventArgs.Data.Contains("[THOR2_flash_state] Device programming started"))
              this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.DeviceProgrammingStarted));
            if (dataReceivedEventArgs.Data.Contains("Using legacy (non-secure) flash method"))
              this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.UnsecureDeviceProgrammingStarted));
            if (dataReceivedEventArgs.Data.Contains("[THOR2_flash_state] Rebooting device to normal mode"))
              this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.SwitchingToNormalMode));
            if (dataReceivedEventArgs.Data.Contains("[THOR2_flash_state] Post programming operations"))
              this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.PostFlashOperationsStarted));
            if (dataReceivedEventArgs.Data.StartsWith("[Factory reset result]", StringComparison.OrdinalIgnoreCase))
              this.HandleFactoryResetResult(dataReceivedEventArgs.Data);
            if (dataReceivedEventArgs.Data.StartsWith("[Product code update result]", StringComparison.OrdinalIgnoreCase))
              this.HandleProductCodeUpdateResult(dataReceivedEventArgs.Data);
            if (dataReceivedEventArgs.Data.StartsWith("[Full NVI update result]", StringComparison.OrdinalIgnoreCase))
              this.HandleFullNviUpdateResult(dataReceivedEventArgs.Data);
            if (!dataReceivedEventArgs.Data.StartsWith("Safe write descriptor index reached: false", StringComparison.OrdinalIgnoreCase))
              return;
            this.safePointReached = false;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning("Error parsing Thor2 output. Unable to parse string: {0}, exception {1}", (object) dataReceivedEventArgs.Data, (object) ex);
      }
    }

    private void HandleFactoryResetResult(string data)
    {
      Thor2ExitCode thor2ExitCode = (Thor2ExitCode) int.Parse(data.Substring("[Factory reset result]".Length + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      if (thor2ExitCode == Thor2ExitCode.Thor2AllOk)
        this.postFlashOperationResults.Add(PostFlashOperation.FactoryReset, (Exception) null);
      else
        this.postFlashOperationResults.Add(PostFlashOperation.FactoryReset, (Exception) new FactoryResetFailedException((uint) thor2ExitCode, "Factory reset failed"));
    }

    private void HandleProductCodeUpdateResult(string data)
    {
      Thor2ExitCode thor2ExitCode = (Thor2ExitCode) int.Parse(data.Substring("[Product code update result]".Length + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      switch (thor2ExitCode)
      {
        case Thor2ExitCode.Thor2AllOk:
          this.postFlashOperationResults.Add(PostFlashOperation.ProductCodeChange, (Exception) null);
          break;
        case Thor2ExitCode.Thor2ErrorUefiDoesNotSupportProductCodeUpdate:
          this.postFlashOperationResults.Add(PostFlashOperation.ProductCodeChange, (Exception) new ProductCodeChangeNotSupportedException((uint) thor2ExitCode, "Product code change failed"));
          break;
        default:
          this.postFlashOperationResults.Add(PostFlashOperation.ProductCodeChange, (Exception) new FlashException((uint) thor2ExitCode, "Product code change failed"));
          break;
      }
    }

    private void HandleFullNviUpdateResult(string data)
    {
      Thor2ExitCode thor2ExitCode = (Thor2ExitCode) int.Parse(data.Substring("[Full NVI update result]".Length + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      switch (thor2ExitCode)
      {
        case Thor2ExitCode.Thor2AllOk:
          this.postFlashOperationResults.Add(PostFlashOperation.FullNviUpdate, (Exception) null);
          break;
        case Thor2ExitCode.Thor2ErrorUefiDoesNotSupportFullNviUpdate:
          this.postFlashOperationResults.Add(PostFlashOperation.FullNviUpdate, (Exception) new FullNviUpdateNotSupportedException((uint) thor2ExitCode, "Full NVI update failed"));
          break;
        default:
          this.postFlashOperationResults.Add(PostFlashOperation.FullNviUpdate, (Exception) new FlashException((uint) thor2ExitCode, "Full NVI update failed"));
          break;
      }
    }

    protected virtual void OnFlashProgressUpdated(FlashProgressUpdatedEventArgs e)
    {
      EventHandler<FlashProgressUpdatedEventArgs> flashProgressUpdated = this.FlashProgressUpdated;
      if (flashProgressUpdated == null)
        return;
      flashProgressUpdated((object) this, e);
    }

    protected virtual void OnFlashStateUpdated(FlashStateUpdatedEventArgs e)
    {
      EventHandler<FlashStateUpdatedEventArgs> flashStateUpdated = this.FlashStateUpdated;
      if (flashStateUpdated == null)
        return;
      flashStateUpdated((object) this, e);
    }

    protected virtual void OnFlashErrorOccured(ErrorEventArgs e)
    {
      EventHandler<ErrorEventArgs> flashErrorOccured = this.FlashErrorOccured;
      if (flashErrorOccured == null)
        return;
      flashErrorOccured((object) this, e);
    }

    protected virtual void OnFlashCompletedSuccessfully(EventArgs e)
    {
      EventHandler<EventArgs> completedSuccessfully = this.FlashCompletedSuccessfully;
      if (completedSuccessfully == null)
        return;
      completedSuccessfully((object) this, e);
    }

    private void ExecuteDeviceUpdateOperation(
      string filename,
      string portId,
      string thorLogfile,
      string backupRdcFile,
      bool limitedTransferSpeed)
    {
      this.lastProgressPercentage = 0;
      this.lastTransferredBytes = 0L;
      this.lastTotalBytes = 0L;
      this.lastTransferSpeed = 0.0;
      this.cumulativeTransferSpeed = 0.0;
      this.transferSpeedCalculations = 0;
      string str1 = string.Empty;
      if (!string.IsNullOrWhiteSpace(backupRdcFile))
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -readrdc \"{0}\"", (object) backupRdcFile);
      string str2 = limitedTransferSpeed ? " -maxtransfersizekb 128" : string.Empty;
      this.postFlashOperationResults.Clear();
      this.wasAborted = false;
      this.safePointReached = true;
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      ProcessStartInfo flashProcessStartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode uefiflash -ffufile \"{0}\" -skip_exit_on_post_op_failure -disable_stdout_buffering -use_boot_to_flsapp_json -conn \"{1}\" -logfile \"{2}\" {3} {4}", (object) filename, (object) portId, (object) thorLogfile, (object) str1, (object) str2),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      this.SetThor2FlashOptions(this.doFullNviUpdate, flashProcessStartInfo);
      processHelper.StartInfo = flashProcessStartInfo;
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.FlashProcessOnOutputDataReceived);
      processHelper.Start();
      this.ThorProcessId = processHelper.Id;
      this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.OperationStarted));
      processHelper.BeginOutputReadLine();
      processHelper.WaitForExit();
      this.flashProcessExitCode = (Thor2ExitCode) processHelper.ExitCode;
      if (!this.WasAborted && this.flashProcessExitCode == Thor2ExitCode.Thor2UnexpectedExit)
      {
        Tracer.Warning("<Thor2 crashed>");
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104901L,
          UriDescription = "Thor2 crash - Wp8DeviceUpdater crash"
        }, DateTime.Now);
      }
      this.OnFlashStateUpdated(new FlashStateUpdatedEventArgs(FlashState.OperationCompleted));
      this.deviceUpdateOperationInProgress = false;
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.FlashProcessOnOutputDataReceived);
      string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flashing failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) processHelper.ExitCode, (object) this.flashProcessExitCode));
      switch (this.GetErrorType(this.flashProcessExitCode))
      {
        case Thor2ErrorType.NoError:
          this.OnFlashCompletedSuccessfully(new EventArgs());
          break;
        case Thor2ErrorType.Thor2Error:
          throw this.HandleThor2Error(this.flashProcessExitCode);
        case Thor2ErrorType.DeviceError:
          throw this.HandleDeviceError(this.flashProcessExitCode);
        case Thor2ErrorType.FfuParsingError:
          throw this.HandleFfuParsingError(this.flashProcessExitCode);
        case Thor2ErrorType.FlashAppError:
          throw this.HandleFlashAppError(this.flashProcessExitCode);
        case Thor2ErrorType.FfuProgrammingVer2Error:
          throw this.HandleVersion2Error(this.flashProcessExitCode);
        default:
          Tracer.Information(str3);
          throw new FlashException((uint) this.flashProcessExitCode, str3, this.safePointReached);
      }
    }

    private void SetThor2FlashOptions(bool doFullNviUpdate, ProcessStartInfo flashProcessStartInfo)
    {
      if (doFullNviUpdate)
      {
        Tracer.Information("Full NVI update enabled");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -do_full_nvi_update", (object) flashProcessStartInfo.Arguments);
      }
      if (this.performFactoryReset)
      {
        Tracer.Information("Factory reset enabled");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -do_factory_reset", (object) flashProcessStartInfo.Arguments);
      }
      if (this.powerOffAfterFlash)
      {
        Tracer.Information("Powering off device after flash");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -power_off", (object) flashProcessStartInfo.Arguments);
      }
      if (!string.IsNullOrEmpty(this.targetProductCode))
      {
        Tracer.Information("Product code change enabled ({0})", (object) this.targetProductCode);
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -productcodeupdate {1}", (object) flashProcessStartInfo.Arguments, (object) this.targetProductCode);
      }
      if (this.skipWrite)
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_write", (object) flashProcessStartInfo.Arguments);
      if (this.skipPlatformIdCheck)
      {
        Tracer.Warning("Skipping Platform ID check");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_id_check", (object) flashProcessStartInfo.Arguments);
      }
      if (this.skipHashCheck)
      {
        Tracer.Warning("Skipping integrity check");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_hash", (object) flashProcessStartInfo.Arguments);
      }
      if (this.skipSignatureCheck)
      {
        Tracer.Warning("Skipping signature check");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_signature_check", (object) flashProcessStartInfo.Arguments);
      }
      if (this.nonSecureFlashing)
      {
        Tracer.Warning("Using non-secure (legacy) flashing protocol");
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -nosecure", (object) flashProcessStartInfo.Arguments);
      }
      else
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -show_detailed_progress", (object) flashProcessStartInfo.Arguments);
      if (this.useCustomBlockSize)
      {
        Tracer.Information("Using custom block size {0}", (object) this.customBlockSize);
        flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -maxtransfersizekb {1}", (object) flashProcessStartInfo.Arguments, (object) this.customBlockSize);
      }
      if (!this.TraceUsb)
        return;
      Tracer.Information("USB tracing enabled for THOR2");
      flashProcessStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -trace_usb", (object) flashProcessStartInfo.Arguments);
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

    private Exception HandleThor2Error(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flashing failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
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
          return (Exception) new FlashException(errorCode, message, this.safePointReached);
      }
    }

    private Exception HandleDeviceError(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flashing failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      switch (thor2ExitCode)
      {
        case Thor2ExitCode.DevRkhMismatchError:
          return (Exception) new RkhCheckException(RkhCheckException.RhkCheckIssue.RkhMismatch, errorCode, str);
        case Thor2ExitCode.DevSbl1NotSigned:
          return (Exception) new RkhCheckException(RkhCheckException.RhkCheckIssue.BootLoadersNotSigned, errorCode, str);
        case Thor2ExitCode.DevUefiNotSigned:
          return (Exception) new RkhCheckException(RkhCheckException.RhkCheckIssue.UefiNotSigned, errorCode, str);
        default:
          return (Exception) new FlashException(errorCode, str, this.safePointReached);
      }
    }

    private Exception HandleVersion2Error(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flashing failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      return thor2ExitCode == (Thor2ExitCode) 397315 ? (Exception) new SecureFlashHashException(errorCode, message) : (Exception) new FlashException(errorCode, message, this.safePointReached);
    }

    private Exception HandleFlashAppError(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flashing failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
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
          return (Exception) new FlashException(errorCode, message, this.safePointReached);
      }
    }

    private Exception HandleFfuParsingError(Thor2ExitCode thor2ExitCode)
    {
      uint errorCode = (uint) thor2ExitCode;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flashing failed with error {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8} ({1})", (object) errorCode, (object) thor2ExitCode));
      if (thor2ExitCode == Thor2ExitCode.FfuParsingError)
      {
        Tracer.Error("FFU parsing error");
        try
        {
          if (this.ExpectedCrc32 != null)
          {
            Tracer.Information("Expected CRC32 known, calculating actual CRC32...");
            string expectedCrc = HashCalculator.Crc32BytesToString(this.ExpectedCrc32);
            byte[] crc32 = new HashCalculator().CalculateCrc32(this.FfuFile);
            string actualCrc = HashCalculator.Crc32BytesToString(crc32);
            Tracer.Information("File size: {0}, calculated CRC32: {1}", (object) new FileInfo(this.FfuFile).Length, (object) actualCrc);
            if (((IEnumerable<byte>) crc32).SequenceEqual<byte>((IEnumerable<byte>) this.ExpectedCrc32))
            {
              Tracer.Information("CRC32 is a match");
            }
            else
            {
              Tracer.Warning("CRC32 is a mismatch (expected {0})", (object) expectedCrc);
              return (Exception) new FfuParsingException(errorCode, "FFU file CRC32 mismatch", FfuParsingException.FfuParsingFailureType.CrcMismatch, expectedCrc, actualCrc);
            }
          }
          else
          {
            Tracer.Information("Expected CRC32 not known, calculating MD5...");
            string md5ChecksumForFile = HashCalculator.GetMd5ChecksumForFile(this.FfuFile);
            Tracer.Information("File size: {0}, calculated MD5: {1}", (object) new FileInfo(this.FfuFile).Length, (object) md5ChecksumForFile);
            return (Exception) new FfuParsingException(errorCode, "FFU file parsing exception", FfuParsingException.FfuParsingFailureType.Generic, md5ChecksumForFile);
          }
        }
        catch (Exception ex)
        {
          object[] objArray = new object[0];
          Tracer.Error(ex, "Failed to calculate checksums of file", objArray);
        }
      }
      return (Exception) new FlashException(errorCode, message, this.safePointReached);
    }
  }
}
