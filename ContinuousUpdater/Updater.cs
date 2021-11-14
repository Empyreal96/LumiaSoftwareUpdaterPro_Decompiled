// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ContinuousUpdaterNamespace.Updater
// Assembly: ContinuousUpdater, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: E264D3AD-34F4-49F1-910A-A4F17DAFC923
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ContinuousUpdater.exe

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.LsuPro.ContinuousUpdaterNamespace
{
  public class Updater
  {
    private readonly string ffuFileFilePath;
    private readonly bool skipWrite;
    private readonly bool skipPlatformIdCheck;
    private readonly bool skipSignatureCheck;
    private readonly bool resetFactorySettings;
    private int currentProgress;
    private bool currentIndeterminate;
    private string currentStatus = string.Empty;
    private Wp8DeviceUpdater wp8DeviceUpdater;
    private bool skipHashCheck;
    private bool traceUsb;
    private string maxPayloadTransferSize;
    private bool enableLimitedTransferSpeed;
    private bool doFullNviUpdate;
    private bool powerOffAfterFlash = true;

    public Updater(
      ConnectedDevice connectedDevice,
      string ffuFileFilePath,
      bool skipWrite,
      bool skipPlatformIdCheck,
      bool skipSignatureCheck,
      bool skipHashCheck,
      bool resetFactorySettings,
      bool traceUsb,
      string maxPayloadTransferSize,
      bool enableLimitedTransferSpeed,
      bool doFullNviUpdate)
    {
      this.ConnectedDevice = connectedDevice;
      this.ffuFileFilePath = ffuFileFilePath;
      this.skipWrite = skipWrite;
      this.skipPlatformIdCheck = skipPlatformIdCheck;
      this.skipSignatureCheck = skipSignatureCheck;
      this.skipHashCheck = skipHashCheck;
      this.resetFactorySettings = resetFactorySettings;
      this.traceUsb = traceUsb;
      this.maxPayloadTransferSize = maxPayloadTransferSize;
      this.enableLimitedTransferSpeed = enableLimitedTransferSpeed;
      this.doFullNviUpdate = doFullNviUpdate;
    }

    public ConnectedDevice ConnectedDevice { get; private set; }

    public bool FlashingOngoing { get; private set; }

    public void StartFlashing()
    {
      Tracer.Information("Starting device flashing: {0}, {1}", (object) this.ConnectedDevice.PortId, (object) this.ffuFileFilePath);
      this.currentProgress = 0;
      this.currentIndeterminate = false;
      this.currentStatus = string.Empty;
      this.SetConnectDisconnectEventSuppressionState(this.ConnectedDevice, true);
      this.wp8DeviceUpdater = new Wp8DeviceUpdater(this.ConnectedDevice.PortId);
      this.wp8DeviceUpdater.FlashProgressUpdated += new EventHandler<FlashProgressUpdatedEventArgs>(this.HandleFlashProgressUpdated);
      this.wp8DeviceUpdater.FlashStateUpdated += new EventHandler<FlashStateUpdatedEventArgs>(this.HandleFlashStateUpdated);
      this.wp8DeviceUpdater.FlashErrorOccured += new EventHandler<ErrorEventArgs>(this.HandleFlashErrorOccured);
      this.wp8DeviceUpdater.FlashCompletedSuccessfully += new EventHandler<EventArgs>(this.HandleFlashCompletedSuccessfully);
      this.wp8DeviceUpdater.FfuFile = this.ffuFileFilePath;
      this.wp8DeviceUpdater.TraceUsb = this.traceUsb;
      this.wp8DeviceUpdater.SetMaxPayloadTransferSize(this.maxPayloadTransferSize);
      if (!this.wp8DeviceUpdater.CanStartDeviceUpdateOperation)
      {
        Tracer.Warning("Device updater does not allow update to start.");
        if (!Wp8DeviceUpdater.FfuFileAvailable(this.ffuFileFilePath))
          this.WriteError("Device updater does not allow update to start.", "Flash file is not accessible.", "File: " + this.ffuFileFilePath, errorCode: 103009);
        else
          this.WriteError("Device updater does not allow update to start.", "Flashing is already ongoing or flash file is not accessible", string.Empty, errorCode: 103009);
      }
      else
      {
        this.FlashingOngoing = true;
        this.wp8DeviceUpdater.StartDeviceUpdate(this.skipWrite, this.skipPlatformIdCheck, this.skipSignatureCheck, this.skipHashCheck, performFactoryReset: this.resetFactorySettings, powerOffAfterFlash: this.powerOffAfterFlash, targetProductCode: string.Empty, rdcBackupFileName: string.Empty, limitedtransferspeed: this.enableLimitedTransferSpeed, doFullNviUpdate: this.doFullNviUpdate);
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "do not remove that yet", MessageId = "instructionsToUser")]
    private void WriteSuccess(
      string message,
      string instructionsToUser = "",
      string details = "",
      Dictionary<PostFlashOperation, Exception> postFlashOperations = null,
      int uri = 103000)
    {
      Tracer.Information("Device update finished (success): {0}", (object) message);
      string thorVersion = this.wp8DeviceUpdater.ThorVersion;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) this.wp8DeviceUpdater.AverageFlashingSpeed);
      Console.WriteLine("Result.UsbPort:{0},Value:Update succeeded,Description:{1},Details:{2},Uri:{3},UriDescription:{4},ApiError:{5},ApiErrorText:{6},ThorVersion:{7},FlashingSpeed:{8}", (object) this.ConnectedDevice.PortId, (object) message, (object) details, (object) uri, (object) message, (object) string.Empty, (object) string.Empty, (object) thorVersion, (object) str);
      this.SetConnectDisconnectEventSuppressionState(this.ConnectedDevice, false);
      this.FlashingOngoing = false;
      this.WritePostFlashOperationResults(postFlashOperations);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "do not remove that yet", MessageId = "instructionsToUser")]
    private void WriteError(
      string errorMessage,
      string instructionsToUser = "",
      string details = "",
      Exception exception = null,
      Dictionary<PostFlashOperation, Exception> postFlashOperations = null,
      int errorCode = 103002)
    {
      Tracer.Error("Device update finished (error): {0}", (object) errorMessage);
      string apiError;
      string apiErrorText;
      this.SetApiErrorFieldsInReportData(exception, out apiError, out apiErrorText);
      string thorVersion = this.wp8DeviceUpdater.ThorVersion;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) this.wp8DeviceUpdater.AverageFlashingSpeed);
      Console.WriteLine("Result.UsbPort:{0},Value:Update failed,Description:{1},Details:{2},Uri:{3},UriDescription:{4},ApiError:{5},ApiErrorText:{6},ThorVersion:{7},FlashingSpeed:{8}", (object) this.ConnectedDevice.PortId, (object) errorMessage, (object) details, (object) errorCode, (object) errorMessage, (object) apiError, (object) apiErrorText, (object) thorVersion, (object) str);
      this.SetConnectDisconnectEventSuppressionState(this.ConnectedDevice, false);
      this.FlashingOngoing = false;
      this.WritePostFlashOperationResults(postFlashOperations);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void SetApiErrorFieldsInReportData(
      Exception exception,
      out string apiError,
      out string apiErrorText)
    {
      if (exception is FlashException flashException)
      {
        apiError = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8}", (object) flashException.ErrorCode);
        apiErrorText = flashException.Message;
      }
      else if (exception != null)
      {
        apiError = (string) null;
        apiErrorText = exception.Message;
      }
      else
      {
        apiError = (string) null;
        apiErrorText = (string) null;
      }
    }

    private void WriteProgress(int progress, bool indeterminate, string status, string details)
    {
      if (this.currentProgress == progress && this.currentIndeterminate == indeterminate && !(this.currentStatus != status))
        return;
      this.currentProgress = progress;
      this.currentIndeterminate = indeterminate;
      this.currentStatus = status;
      Console.WriteLine("Progress.UsbPort:{0},Value:{1},IsIndeterminate:{2},Description:{3},Details:{4}", (object) this.ConnectedDevice.PortId, (object) progress, (object) indeterminate, (object) status, (object) details);
    }

    private void WriteCustomMessage(string customMessage) => Console.WriteLine("CsuCustomMessage.UsbPort:{0},Value:{1}", (object) this.ConnectedDevice.PortId, (object) customMessage);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void WritePostFlashOperationResults(
      Dictionary<PostFlashOperation, Exception> postFlashOperations)
    {
      if (postFlashOperations == null)
        return;
      foreach (PostFlashOperation key in postFlashOperations.Keys)
      {
        if (postFlashOperations[key] == null)
          Tracer.Information("Post flash operation {0} succeeded", (object) key);
        else
          Tracer.Error(postFlashOperations[key], "Post flash operation {0} failed", (object) key);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void SetConnectDisconnectEventSuppressionState(
      ConnectedDevice connectedDevice,
      bool suppressEvents)
    {
      if (connectedDevice == null)
        return;
      connectedDevice.SuppressConnectedDisconnectedEvents = suppressEvents;
    }

    private void FlashDeviceWithoutResettingSkipOptions(string filename, bool useUnsecureFlashing)
    {
      this.wp8DeviceUpdater.FfuFile = filename;
      this.wp8DeviceUpdater.StartDeviceUpdate(this.skipWrite, this.skipPlatformIdCheck, this.skipSignatureCheck, this.skipHashCheck, useUnsecureFlashing, this.resetFactorySettings, this.powerOffAfterFlash, string.Empty, string.Empty, this.enableLimitedTransferSpeed, this.doFullNviUpdate);
    }

    private void HandleRkhCheckException(RkhCheckException rkhCheckException)
    {
      string instructionsToUser = string.Empty;
      string withoutStackTrace = Toolkit.GetExceptionAsStringWithoutStackTrace((Exception) rkhCheckException);
      int errorCode = 103002;
      switch (rkhCheckException.RkhFailureType)
      {
        case RkhCheckException.RhkCheckIssue.RkhMismatch:
          instructionsToUser = "The boot loaders or UEFI in software image are not signed for this device.";
          errorCode = 103013;
          break;
        case RkhCheckException.RhkCheckIssue.BootLoadersNotSigned:
          instructionsToUser = "The boot loaders in software image are not signed. The device requires correctly signed boot loaders.";
          errorCode = 103012;
          break;
        case RkhCheckException.RhkCheckIssue.UefiNotSigned:
          instructionsToUser = "The UEFI image is not signed. The device requires correctly signed UEFI image.";
          errorCode = 103011;
          break;
      }
      this.WriteError("Software update failed.", instructionsToUser, withoutStackTrace, errorCode: errorCode);
    }

    private void HandleFlashProgressUpdated(object sender, FlashProgressUpdatedEventArgs e)
    {
      string details = string.Empty;
      if (e.TransferredBytes != 0L && e.TotalBytes != 0L && !object.Equals((object) e.MegabytesPerSecond, (object) 0.0))
        details = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) Toolkit.GetFormattedStringForBytes(e.TransferredBytes), (object) Toolkit.GetFormattedStringForBytes(e.TotalBytes)), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:F1} MB/s", (object) e.MegabytesPerSecond));
      this.WriteProgress(e.Progress, this.currentIndeterminate, this.currentStatus, details);
    }

    private void HandleFlashStateUpdated(object sender, FlashStateUpdatedEventArgs e)
    {
      Tracer.Information("Flash state updated to {0}", (object) e.FlashState.ToString());
      switch (e.FlashState)
      {
        case FlashState.OperationStarted:
          this.WriteProgress(this.currentProgress, true, "Initializing device update", string.Empty);
          break;
        case FlashState.SwitchingToFlashMode:
          this.WriteProgress(this.currentProgress, true, "Switching device to flash mode", string.Empty);
          break;
        case FlashState.DeviceProgrammingStarted:
          this.WriteProgress(this.currentProgress, false, "Programming data to device", string.Empty);
          break;
        case FlashState.SwitchingToNormalMode:
          this.WriteProgress(this.currentProgress, true, "Switching device to normal mode", string.Empty);
          break;
        case FlashState.OperationCompleted:
          this.WriteProgress(this.currentProgress, false, "Finalizing operation", string.Empty);
          break;
        case FlashState.UnsecureDeviceProgrammingStarted:
          this.WriteProgress(this.currentProgress, false, "Programming data to device (using unsecure protocol)", string.Empty);
          break;
        case FlashState.PostFlashOperationsStarted:
          this.WriteProgress(this.currentProgress, true, "Executing post programming operations", string.Empty);
          break;
        default:
          this.WriteProgress(this.currentProgress, false, "Updating device", string.Empty);
          break;
      }
    }

    private void HandleFlashCompletedSuccessfully(object sender, EventArgs e)
    {
      Tracer.Information("Flash completed successfully");
      this.WriteSuccess("Software update completed successfully.");
    }

    private void HandleFlashErrorOccured(object sender, ErrorEventArgs e)
    {
      if (this.wp8DeviceUpdater.WasAborted)
      {
        Tracer.Information("Software update was cancelled by user");
        this.WriteSuccess("Software update was cancelled.", string.Empty, string.Empty, this.wp8DeviceUpdater.PostFlashOperationResults, 103022);
      }
      else if (e.GetException() is RkhCheckException)
        this.HandleRkhCheckException(e.GetException() as RkhCheckException);
      else if (e.GetException() is SecureFlashAuthenticationException)
      {
        if (this.skipHashCheck && !this.skipPlatformIdCheck && !this.skipSignatureCheck)
        {
          this.skipHashCheck = false;
          this.FlashDeviceWithoutResettingSkipOptions(this.wp8DeviceUpdater.FfuFile, false);
        }
        else
        {
          Tracer.Information("Authentication failure encountered. Enabling all security checks.");
          this.WriteCustomMessage("Re-enabled security checks");
          this.wp8DeviceUpdater.StartDeviceUpdate(performFactoryReset: this.resetFactorySettings, powerOffAfterFlash: this.powerOffAfterFlash, targetProductCode: string.Empty, rdcBackupFileName: string.Empty, limitedtransferspeed: this.enableLimitedTransferSpeed, doFullNviUpdate: this.doFullNviUpdate);
        }
      }
      else if (e.GetException() is SecureFlashPlatformIdException)
      {
        Tracer.Information("Platform ID user interaction triggered");
        string withoutStackTrace = Toolkit.GetExceptionAsStringWithoutStackTrace(e.GetException());
        this.WriteError("Software update failed.", string.Empty, withoutStackTrace, e.GetException(), this.wp8DeviceUpdater.PostFlashOperationResults, 103015);
      }
      else if (e.GetException() is SecureFlashSignatureException)
      {
        Tracer.Information("Signature user interaction triggered");
        string withoutStackTrace = Toolkit.GetExceptionAsStringWithoutStackTrace(e.GetException());
        this.WriteError("Software update failed.", string.Empty, withoutStackTrace, e.GetException(), this.wp8DeviceUpdater.PostFlashOperationResults, 103016);
      }
      else if (e.GetException() is SecureFlashHashException)
      {
        Tracer.Information("Integrity user interaction triggered");
        string withoutStackTrace = Toolkit.GetExceptionAsStringWithoutStackTrace(e.GetException());
        this.WriteError("Software update failed.", string.Empty, withoutStackTrace, e.GetException(), this.wp8DeviceUpdater.PostFlashOperationResults, 103017);
      }
      else if (e.GetException() is FullNviUpdateNotSupportedException)
        this.WriteSuccess("Software update completed successfully.", "Full NVI update was not supported on device.", string.Empty, this.wp8DeviceUpdater.PostFlashOperationResults, 103018);
      else if (e.GetException() is ProductCodeChangeNotSupportedException)
        this.WriteSuccess("Software update completed successfully.", "Product code change failed.", string.Empty, this.wp8DeviceUpdater.PostFlashOperationResults, 103019);
      else if (e.GetException() is FactoryResetFailedException)
      {
        this.WriteSuccess("Software update completed successfully.", "Factory reset was not performed after SW update.", string.Empty, this.wp8DeviceUpdater.PostFlashOperationResults, 103020);
      }
      else
      {
        string withoutStackTrace = Toolkit.GetExceptionAsStringWithoutStackTrace(e.GetException());
        int errorCode = Toolkit.CheckUriCriticality(e.GetException() as FlashException, 103021, this.wp8DeviceUpdater.ThorVersion);
        this.WriteError("Software update failed.", string.Empty, withoutStackTrace, e.GetException(), this.wp8DeviceUpdater.PostFlashOperationResults, errorCode);
      }
    }
  }
}
