// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyFlasher
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using Microsoft.LsuPro.Helpers;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class EmergencyFlasher
  {
    private string currentProgressText;
    private EmergencyFlashStage currentEmergencyFlashStage;
    private int currentProgressPercentage;
    private long lastTransferredBytes;
    private long lastTotalBytes;
    private double lastTransferSpeed;
    private bool skipPlatformIdCheck;
    private bool skipSignatureCheck;
    private bool skipIntegrityCheck;

    public event EventHandler<EventArgs> EmergencyFlashStarted;

    public event EventHandler<EmergencyFlashProgressEventArgs> EmergencyFlashProgress;

    public event EventHandler<EmergencyFlashCompletedEventArgs> EmergencyFlashCompleted;

    public EmergencyFlasher() => this.ThorVersion = Toolkit.DetermineThor2Version();

    public string ThorVersion { get; private set; }

    public bool SkipPlatformIdCheck
    {
      get
      {
        Tracer.Information("Skip platform ID check setting is {0}", (object) this.skipPlatformIdCheck);
        return this.skipPlatformIdCheck;
      }
      set
      {
        this.skipPlatformIdCheck = value;
        Tracer.Information("Skip platform ID check setting was set to {0}", (object) this.skipPlatformIdCheck);
      }
    }

    public bool SkipSignatureCheck
    {
      get
      {
        Tracer.Information("Skip signature check setting is {0}", (object) this.skipSignatureCheck);
        return this.skipSignatureCheck;
      }
      set
      {
        this.skipSignatureCheck = value;
        Tracer.Information("Skip signature check setting was set to {0}", (object) this.skipSignatureCheck);
      }
    }

    public bool SkipIntegrityCheck
    {
      get
      {
        Tracer.Information("Skip integrity check setting is {0}", (object) this.skipIntegrityCheck);
        return this.skipIntegrityCheck;
      }
      set
      {
        this.skipIntegrityCheck = value;
        Tracer.Information("Skip integrity check setting was set to {0}", (object) this.skipIntegrityCheck);
      }
    }

    public void StartAlphaCollinsEmergencyFlash(EmergencyFlashFileSet e) => this.StartAlphaCollinsEmergencyFlash(e.File1.FullName, e.File2.FullName, e.File3.FullName);

    public void StartQuattroEmergencyFlash(EmergencyFlashFileSet e) => this.StartQuattroEmergencyFlash(e.File1.FullName, e.File2.FullName, e.File3.FullName);

    private void StartAlphaCollinsEmergencyFlash(string hexPath, string mbnPath, string ffuPath)
    {
      Tracer.Information("<<Emergency flash 1>>");
      string logFilePath = TraceWriter.GenerateLogFilePath("thor2_emergency1");
      Tracer.Information("Logging to file {0}", (object) logFilePath);
      string arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode emergency -hexfile \"{0}\" -mbnfile \"{1}\" -ffufile \"{2}\" -disable_stdout_buffering -show_detailed_progress -logfile \"{3}\"", (object) hexPath, (object) mbnPath, (object) ffuPath, (object) logFilePath);
      arguments = this.SetSkipSettings(arguments);
      TaskHelper deviceUpdateTask = new TaskHelper((Action) (() => this.EmergencyFlashProcess(arguments, EmergencyFlashType.AlphaCollins)));
      deviceUpdateTask.Start();
      deviceUpdateTask.ContinueWith((Action<object>) (t =>
      {
        if (deviceUpdateTask.Exception != null)
        {
          foreach (Exception innerException in deviceUpdateTask.Exception.InnerExceptions)
            Tracer.Error("Emergency flash 1 error: {0}", (object) innerException.Message);
        }
        Tracer.Information("Emergency flash 1 completed");
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void StartQuattroEmergencyFlash(string hexPath, string edPath, string ffuPath)
    {
      Tracer.Information("<<Emergency flash 2>>");
      string logFilePath = TraceWriter.GenerateLogFilePath("thor2_emergency2");
      Tracer.Information("Logging to file {0}", (object) logFilePath);
      string arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode emergency -hexfile \"{0}\" -edfile \"{1}\" -ffufile \"{2}\" -disable_stdout_buffering -show_detailed_progress -logfile \"{3}\"", (object) hexPath, (object) edPath, (object) ffuPath, (object) logFilePath);
      arguments = this.SetSkipSettings(arguments);
      TaskHelper deviceUpdateTask = new TaskHelper((Action) (() => this.EmergencyFlashProcess(arguments, EmergencyFlashType.Quattro)));
      deviceUpdateTask.Start();
      deviceUpdateTask.ContinueWith((Action<object>) (t =>
      {
        if (deviceUpdateTask.Exception != null)
        {
          foreach (Exception innerException in deviceUpdateTask.Exception.InnerExceptions)
            Tracer.Error("Emergency flash 2 error: {0}", (object) innerException.Message);
        }
        Tracer.Information("Emergency flash 2 completed");
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private string SetSkipSettings(string arguments)
    {
      if (this.SkipPlatformIdCheck)
        arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_id_check", (object) arguments);
      if (this.SkipSignatureCheck)
        arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_signature_check", (object) arguments);
      if (this.SkipIntegrityCheck)
        arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -skip_hash", (object) arguments);
      return arguments;
    }

    private void EmergencyFlashProcess(string arguments, EmergencyFlashType flashType)
    {
      Tracer.Information("Starting emergency flash process");
      this.currentProgressText = "Emergency flashing";
      this.currentEmergencyFlashStage = EmergencyFlashStage.EmergencyFlash;
      this.currentProgressPercentage = 0;
      this.lastTransferredBytes = 0L;
      this.lastTotalBytes = 0L;
      this.lastTransferSpeed = 0.0;
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = arguments,
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.FlashProcessOutputDataReceived);
      processHelper.Start();
      this.SendEmergencyFlashStartedEvent();
      processHelper.BeginOutputReadLine();
      processHelper.WaitForExit();
      if (processHelper.ExitCode < 0)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104904L,
          UriDescription = "Thor2 crash - EmergencyFlasher crash"
        }, DateTime.Now);
      Thor2ExitCode exitCode = (Thor2ExitCode) processHelper.ExitCode;
      Tracer.Information("Emergency flash tool exited with exit code {0}", (object) exitCode);
      this.SendEmergencyFlashCompletedEvent(flashType, exitCode);
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.FlashProcessOutputDataReceived);
    }

    private void FlashProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      if (string.IsNullOrEmpty(e.Data))
        return;
      if (this.currentEmergencyFlashStage == EmergencyFlashStage.EmergencyFlash)
      {
        if (e.Data.StartsWith("Programming ", StringComparison.OrdinalIgnoreCase) && e.Data.Length > 13)
          this.currentProgressText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Emergency flashing ({0})", (object) e.Data.Substring(12));
        else if (!this.currentProgressText.Equals("Emergency flashing", StringComparison.OrdinalIgnoreCase))
          this.currentProgressText = "Emergency flashing";
      }
      if (e.Data.StartsWith("Percents: ", StringComparison.OrdinalIgnoreCase))
      {
        this.currentEmergencyFlashStage = EmergencyFlashStage.FfuFlash;
        this.currentProgressPercentage = int.Parse(e.Data.Substring(9), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      if (e.Data.Contains("Detailed progress: "))
      {
        Toolkit.ParseDetailedProgress(e.Data, out this.lastTransferredBytes, out this.lastTotalBytes, out this.lastTransferSpeed);
        this.currentProgressText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Programming FFU file to device {0} ({1})", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) Toolkit.GetFormattedStringForBytes(this.lastTransferredBytes), (object) Toolkit.GetFormattedStringForBytes(this.lastTotalBytes)), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:F1} MB/s", (object) this.lastTransferSpeed));
      }
      this.SendEmergencyFlashProgressEvent(new EmergencyFlashProgressEventArgs()
      {
        ProgressInfoText = this.currentProgressText,
        RawData = e.Data,
        IsIndeterminate = this.currentEmergencyFlashStage == EmergencyFlashStage.EmergencyFlash,
        Percentage = this.currentProgressPercentage,
        Stage = this.currentEmergencyFlashStage
      });
    }

    private void SendEmergencyFlashStartedEvent()
    {
      Tracer.Information("Sending emergency flash started event");
      EventHandler<EventArgs> emergencyFlashStarted = this.EmergencyFlashStarted;
      if (emergencyFlashStarted == null)
        return;
      EventArgs e = new EventArgs();
      emergencyFlashStarted((object) this, e);
    }

    private void SendEmergencyFlashProgressEvent(EmergencyFlashProgressEventArgs args)
    {
      EventHandler<EmergencyFlashProgressEventArgs> emergencyFlashProgress = this.EmergencyFlashProgress;
      if (emergencyFlashProgress == null)
        return;
      emergencyFlashProgress((object) this, args);
    }

    private void SendEmergencyFlashCompletedEvent(
      EmergencyFlashType flashType,
      Thor2ExitCode exitCode)
    {
      Tracer.Information("Sending emergency flash completed event with exit code {0}", (object) exitCode);
      EventHandler<EmergencyFlashCompletedEventArgs> emergencyFlashCompleted = this.EmergencyFlashCompleted;
      if (emergencyFlashCompleted == null)
        return;
      EmergencyFlashCompletedEventArgs e = new EmergencyFlashCompletedEventArgs()
      {
        EmergencyFlashType = flashType,
        Success = exitCode == Thor2ExitCode.Thor2AllOk,
        ExitCode = exitCode
      };
      emergencyFlashCompleted((object) this, e);
    }
  }
}
