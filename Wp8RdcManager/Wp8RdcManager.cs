// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Wp8RdcManager
// Assembly: Wp8RdcManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FA7F490B-384D-4433-AD97-E6E4DA27B1A0
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8RdcManager.dll

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class Wp8RdcManager
  {
    private ConnectedDevice connectedDevice;

    public Wp8RdcManager(ConnectedDevice connectedDevice)
    {
      Tracer.Information("Wp8RdcManager instance created for {0}", (object) connectedDevice.PortId);
      this.connectedDevice = connectedDevice;
    }

    public event EventHandler<OperationCompletedEventArgs> OnOperationCompleted;

    public void BackupRdcFromDevice(string rdcBackupFile)
    {
      TaskHelper backupRdcFromDeviceTask = new TaskHelper((Action) (() => this.BackupRdcFromDeviceInternal(rdcBackupFile)));
      Tracer.Information("Start RDC backup for device {0}", (object) this.connectedDevice.PortId);
      backupRdcFromDeviceTask.Start();
      backupRdcFromDeviceTask.ContinueWith((Action<object>) (t =>
      {
        if (backupRdcFromDeviceTask.Exception == null)
          return;
        foreach (object innerException in backupRdcFromDeviceTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void BackupRdcFromDeviceInternal(string rdcBackupFile)
    {
      string logFilePath = TraceWriter.GenerateLogFilePath("thor2_rdc");
      Tracer.Information("Thor log file: {0}", (object) logFilePath);
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper();
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode retail -readrdc \"{0}\" -conn \"{1}\" -logfile \"{2}\"", (object) rdcBackupFile, (object) this.connectedDevice.PortId, (object) logFilePath),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      processHelper.Start();
      processHelper.WaitForExit();
      Tracer.Information("Backup RDC done for device {0}", (object) this.connectedDevice.PortId);
      this.SendOnOperationCompleted("Backup");
      if (processHelper.ExitCode < 0)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104905L,
          UriDescription = "Thor2 crash - Wp8RdcManager crash"
        }, DateTime.Now);
      if (processHelper.ExitCode != 0)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Backup RDC returned with error code {0}", (object) processHelper.ExitCode));
    }

    private void SendOnOperationCompleted(string operationName)
    {
      EventHandler<OperationCompletedEventArgs> operationCompleted = this.OnOperationCompleted;
      if (operationCompleted == null)
        return;
      operationCompleted((object) this, new OperationCompletedEventArgs(operationName));
    }

    public void RestoreRdcToDevice(string rdcBackupFile)
    {
      TaskHelper restoreRdcToDeviceTask = new TaskHelper((Action) (() => this.RestoreRdcToDeviceInternal(rdcBackupFile)));
      Tracer.Information("Star restore RDC for device {0}", (object) this.connectedDevice.PortId);
      restoreRdcToDeviceTask.Start();
      restoreRdcToDeviceTask.ContinueWith((Action<object>) (t =>
      {
        if (restoreRdcToDeviceTask.Exception == null)
          return;
        foreach (object innerException in restoreRdcToDeviceTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void RestoreRdcToDeviceInternal(string rdcBackupFile)
    {
      string logFilePath = TraceWriter.GenerateLogFilePath("thor2_rdc");
      Tracer.Information("Thor log file: {0}", (object) logFilePath);
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper();
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode retail -writerdc \"{0}\" -conn \"{1}\" -logfile \"{2}\"", (object) rdcBackupFile, (object) this.connectedDevice.PortId, (object) logFilePath),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      processHelper.Start();
      processHelper.WaitForExit();
      Tracer.Information("Restore RDC to device done for device {0}", (object) this.connectedDevice.PortId);
      this.SendOnOperationCompleted("Restore");
      if (processHelper.ExitCode < 0)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104905L,
          UriDescription = "Thor2 crash - Wp8RdcManager crash"
        }, DateTime.Now);
      if (processHelper.ExitCode != 0)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Restore RDC returned with error code {0}", (object) processHelper.ExitCode));
    }

    public void EraseRdcFromDevice()
    {
      TaskHelper eraseRdcFromDeviceTask = new TaskHelper((Action) (() => this.EraseRdcFromDeviceInternal()));
      Tracer.Information("Star erase RDC for device {0}", (object) this.connectedDevice.PortId);
      eraseRdcFromDeviceTask.Start();
      eraseRdcFromDeviceTask.ContinueWith((Action<object>) (t =>
      {
        if (eraseRdcFromDeviceTask.Exception == null)
          return;
        foreach (object innerException in eraseRdcFromDeviceTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void EraseRdcFromDeviceInternal()
    {
      string logFilePath = TraceWriter.GenerateLogFilePath("thor2_rdc");
      Tracer.Information("Thor log file: {0}", (object) logFilePath);
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper();
      processHelper.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "thor2.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-mode retail -eraserdc -conn \"{0}\" -logfile \"{1}\"", (object) this.connectedDevice.PortId, (object) logFilePath),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      processHelper.Start();
      processHelper.WaitForExit();
      Tracer.Information("Erase RDC done for device {0}", (object) this.connectedDevice.PortId);
      this.SendOnOperationCompleted("Erase");
      if (processHelper.ExitCode < 0)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104905L,
          UriDescription = "Thor2 crash - Wp8RdcManager crash"
        }, DateTime.Now);
      if (processHelper.ExitCode != 0)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Erase RDC returned with error code {0}", (object) processHelper.ExitCode));
    }
  }
}
