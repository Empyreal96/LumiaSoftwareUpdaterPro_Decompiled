// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Wp8DeviceModeChanger
// Assembly: Wp8DeviceModeChanger, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 4F0CEEBE-2E94-4BD1-9254-36B88BE96C0D
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceModeChanger.dll

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
  public class Wp8DeviceModeChanger
  {
    private ConnectedDevice connectedDevice;
    private DeviceModeChangeStatus processExecutionResult = DeviceModeChangeStatus.NoResult;

    public Wp8DeviceModeChanger(ConnectedDevice device) => this.connectedDevice = device;

    public event EventHandler<DeviceModeChangeEventArgs> DeviceModeChangeResult;

    public DeviceModeChangeStatus ExecuteDeviceModeChange(
      ConnectedDeviceMode desiredMode,
      Action completitionAction,
      bool afterReboot = false,
      bool waitForExit = false)
    {
      this.processExecutionResult = DeviceModeChangeStatus.NoResult;
      string modeSwitch = string.Empty;
      if (desiredMode == ConnectedDeviceMode.Normal)
        modeSwitch = "n";
      if (desiredMode == ConnectedDeviceMode.Uefi)
        modeSwitch = "u";
      if (desiredMode == ConnectedDeviceMode.Label)
        modeSwitch = "l";
      if (desiredMode == ConnectedDeviceMode.MassStorage)
        modeSwitch = "m";
      if (string.IsNullOrEmpty(modeSwitch))
      {
        Tracer.Information("Method execution is not enabled with parameter: {0}", (object) desiredMode);
        return DeviceModeChangeStatus.OtherError;
      }
      TaskHelper th = new TaskHelper((Action) (() => this.RunDeviceModeChangerProcess(modeSwitch, afterReboot, waitForExit)));
      th.Start();
      th.ContinueWith((Action<object>) (t =>
      {
        if (th.Exception != null)
        {
          this.processExecutionResult = DeviceModeChangeStatus.OtherError;
          foreach (Exception innerException in th.Exception.InnerExceptions)
            Tracer.Error("Port {0}, DeviceModeChanger: {1}", (object) this.connectedDevice.PortId, (object) innerException.Message);
        }
        completitionAction();
      }), TaskContinuationOptions.None);
      return this.processExecutionResult;
    }

    private void RunDeviceModeChangerProcess(string modeSwitch, bool afterReboot = false, bool waitForExit = false)
    {
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      ProcessStartInfo processStartInfo;
      if (afterReboot)
        processStartInfo = new ProcessStartInfo()
        {
          FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "DeviceModeChanger.exe")),
          Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-to={0} -id={1} -afterreboot", (object) modeSwitch, (object) this.connectedDevice.PortId),
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          WorkingDirectory = directoryName
        };
      else
        processStartInfo = new ProcessStartInfo()
        {
          FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "DeviceModeChanger.exe")),
          Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-to={0} -id={1}", (object) modeSwitch, (object) this.connectedDevice.PortId),
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          WorkingDirectory = directoryName
        };
      processHelper.StartInfo = processStartInfo;
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.ExecuteDeviceModeChangeOperationOnOutputDataReceived);
      Tracer.Information("Starting process for {0}", (object) this.connectedDevice.PortId);
      processHelper.Start();
      Tracer.Information("Process started");
      processHelper.BeginOutputReadLine();
      Tracer.Information("Waiting process {0} to exit for {1}", (object) processHelper.Id, (object) this.connectedDevice.PortId);
      if (afterReboot | waitForExit)
        processHelper.WaitForExit();
      else
        processHelper.WaitForExit(180000);
      if (processHelper.HasExited)
      {
        Tracer.Information("Process terminated, exit code {0}", (object) processHelper.ExitCode);
        if (processHelper.ExitCode < 0)
          ReportSender.SaveReportAsync(new ReportDetails()
          {
            Uri = 104908L,
            UriDescription = "DeviceModeChanger crash"
          }, DateTime.Now);
      }
      else
      {
        Tracer.Warning("DeviceModeChanger for {0} has not exited during given timeout. Killing process.", (object) this.connectedDevice.PortId);
        processHelper.Kill();
        this.processExecutionResult = DeviceModeChangeStatus.Timeout;
      }
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.ExecuteDeviceModeChangeOperationOnOutputDataReceived);
      Tracer.Information("DeviceModeChanger has stopped execution for {0}.", (object) this.connectedDevice.PortId);
    }

    private void ExecuteDeviceModeChangeOperationOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (dataReceivedEventArgs.Data == null)
        return;
      Tracer.Information(dataReceivedEventArgs.Data);
      string message1 = "success";
      if (dataReceivedEventArgs.Data.ToLowerInvariant().Contains("operation complete:"))
      {
        this.processExecutionResult = DeviceModeChangeStatus.Success;
        EventHandler<DeviceModeChangeEventArgs> modeChangeResult = this.DeviceModeChangeResult;
        if (modeChangeResult != null)
          modeChangeResult((object) this, new DeviceModeChangeEventArgs(message1));
      }
      if (!dataReceivedEventArgs.Data.ToLowerInvariant().Contains("operation failed:"))
        return;
      this.processExecutionResult = DeviceModeChangeStatus.Failure;
      string message2 = dataReceivedEventArgs.Data.Remove(0, dataReceivedEventArgs.Data.IndexOf(']') + 1);
      EventHandler<DeviceModeChangeEventArgs> modeChangeResult1 = this.DeviceModeChangeResult;
      if (modeChangeResult1 == null)
        return;
      modeChangeResult1((object) this, new DeviceModeChangeEventArgs(message2));
    }
  }
}
