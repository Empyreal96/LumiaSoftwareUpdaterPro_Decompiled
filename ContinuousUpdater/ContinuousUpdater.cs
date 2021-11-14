// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ContinuousUpdaterNamespace.ContinuousUpdater
// Assembly: ContinuousUpdater, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: E264D3AD-34F4-49F1-910A-A4F17DAFC923
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ContinuousUpdater.exe

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using Microsoft.LumiaConnectivity.EventArgs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.ContinuousUpdaterNamespace
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class ContinuousUpdater
  {
    private readonly CommandLineParser commandLineParser;
    private readonly ConnectedDevices lumiaConnetivityConnectedDevices = new ConnectedDevices();
    private readonly string tempDirectory = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "ContinuousUpdate");
    private readonly Collection<Updater> updaters = new Collection<Updater>();
    private TaskHelper waitForCloseSignalTask;
    private CancellationTokenSource waitForCloseSignalTaskCancellationTokenSource;
    private CancellationToken waitForCloseSignalTaskCancellationToken;
    private TaskHelper wachdogTask;
    private CancellationTokenSource wachdogTaskCancellationTokenSource;
    private CancellationToken wachdogTaskCancellationToken;
    private TaskHelper mainTask;
    private CancellationTokenSource mainTaskCancellationTokenSource;
    private CancellationToken mainTaskCancellationToken;
    private string filePath;
    private string ffuFileFilePath;
    private string maxPayloadTransferSize;
    private bool traceUsb;
    private bool skipWrite;
    private bool skipPlatformIdCheck;
    private bool skipSignatureCheck;
    private bool skipHashCheck;
    private bool resetFactorySettings;
    private bool enableLimitedTransferSpeed;
    private bool doFullNviUpdate;

    public ContinuousUpdater(string[] args)
    {
      this.commandLineParser = new CommandLineParser(args);
      this.waitForCloseSignalTaskCancellationTokenSource = new CancellationTokenSource();
      this.waitForCloseSignalTaskCancellationToken = this.waitForCloseSignalTaskCancellationTokenSource.Token;
      this.wachdogTaskCancellationTokenSource = new CancellationTokenSource();
      this.wachdogTaskCancellationToken = this.wachdogTaskCancellationTokenSource.Token;
      this.mainTaskCancellationTokenSource = new CancellationTokenSource();
      this.mainTaskCancellationToken = this.mainTaskCancellationTokenSource.Token;
    }

    public bool ParseArguments()
    {
      if (this.commandLineParser.ParseArguments() > 0 && this.commandLineParser.SwitchIsSet("help"))
      {
        ContinuousUpdater.PrintHelp();
        return false;
      }
      this.ffuFileFilePath = this.commandLineParser.GetOptionValue("ffufile");
      this.maxPayloadTransferSize = this.commandLineParser.GetOptionValue("maxpayloadtransfersize");
      this.traceUsb = this.commandLineParser.SwitchIsSet("traceusb");
      this.skipWrite = this.commandLineParser.SwitchIsSet("skipwrite");
      this.skipPlatformIdCheck = this.commandLineParser.SwitchIsSet("skipplatformidcheck");
      this.skipSignatureCheck = this.commandLineParser.SwitchIsSet("skipsignaturecheck");
      this.skipHashCheck = this.commandLineParser.SwitchIsSet("skiphashcheck");
      this.resetFactorySettings = this.commandLineParser.SwitchIsSet("resetfactorysettings");
      this.enableLimitedTransferSpeed = this.commandLineParser.SwitchIsSet("enablelimitedtransferspeed");
      this.doFullNviUpdate = this.commandLineParser.SwitchIsSet("dofullnviupdate");
      return true;
    }

    public void PerformOperation()
    {
      this.StartWaitForCloseSignalTask();
      this.StartWatchdogTask();
      this.StartDeviceDetection();
      this.StartMainTask();
    }

    private void StartDeviceDetection()
    {
      this.lumiaConnetivityConnectedDevices.DeviceConnected += new EventHandler<DeviceConnectedEventArgs>(this.HandleDeviceLumiaConnetivityConnected2);
      this.lumiaConnetivityConnectedDevices.DeviceDisconnected += new EventHandler<DeviceConnectedEventArgs>(this.HandleDeviceDisconnected);
      this.lumiaConnetivityConnectedDevices.Start();
    }

    private void HandleDeviceLumiaConnetivityConnected2(object sender, DeviceConnectedEventArgs e)
    {
      Tracer.Information("Device connected in {0} mode, port {1}", (object) e.ConnectedDevice.Mode, (object) e.ConnectedDevice.PortId);
      if (e.ConnectedDevice.Mode != ConnectedDeviceMode.Uefi)
        return;
      foreach (Updater updater in this.updaters)
      {
        if (updater.ConnectedDevice == e.ConnectedDevice)
        {
          Tracer.Information("Device already on list, port {0}", (object) e.ConnectedDevice.PortId);
          return;
        }
      }
      Updater updater1 = new Updater(e.ConnectedDevice, this.ffuFileFilePath, this.skipWrite, this.skipPlatformIdCheck, this.skipSignatureCheck, this.skipHashCheck, this.resetFactorySettings, this.traceUsb, this.maxPayloadTransferSize, this.enableLimitedTransferSpeed, this.doFullNviUpdate);
      this.updaters.Add(updater1);
      this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeviceConnected.UsbPort:{0}", (object) e.ConnectedDevice.PortId));
      updater1.StartFlashing();
    }

    private void HandleDeviceDisconnected(object sender, DeviceConnectedEventArgs e)
    {
      Tracer.Information("Device disconnected in {0} mode, port {1}", (object) e.ConnectedDevice.Mode, (object) e.ConnectedDevice.PortId);
      foreach (Updater updater in this.updaters)
      {
        if (updater.ConnectedDevice == e.ConnectedDevice)
        {
          this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeviceDisconnected.UsbPort:{0}", (object) updater.ConnectedDevice.PortId));
          this.updaters.Remove(updater);
          break;
        }
      }
    }

    private void CancelMainTask()
    {
      if (this.lumiaConnetivityConnectedDevices != null)
      {
        this.lumiaConnetivityConnectedDevices.DeviceConnected -= new EventHandler<DeviceConnectedEventArgs>(this.HandleDeviceLumiaConnetivityConnected2);
        this.lumiaConnetivityConnectedDevices.DeviceDisconnected -= new EventHandler<DeviceConnectedEventArgs>(this.HandleDeviceDisconnected);
        this.lumiaConnetivityConnectedDevices.Stop();
      }
      if (this.mainTaskCancellationTokenSource == null)
        return;
      this.mainTaskCancellationTokenSource.Cancel();
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
      this.filePath = Path.Combine(this.tempDirectory, "ContinuousUpdateCommunication.txt");
      do
      {
        if (!File.Exists(this.filePath))
        {
          Thread.Sleep(1000);
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
            Thread.Sleep(1000);
            goto label_12;
          }
          if (!string.IsNullOrEmpty(str) && str == "CloseContinuousUpdater")
          {
            Tracer.Information("WaitForCloseSignalTask: cancelling tasks");
            if (this.wachdogTaskCancellationTokenSource != null)
              this.wachdogTaskCancellationTokenSource.Cancel();
            this.CancelMainTask();
            if (!DirectoryHelper.DirectoryExist(this.tempDirectory))
              break;
            DirectoryHelper.Delete(this.tempDirectory, true);
            break;
          }
          Thread.Sleep(1000);
        }
label_12:;
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
      while (this.CheckIfLsuProIsRunning())
      {
        Thread.Sleep(1000);
        if (this.wachdogTaskCancellationToken.IsCancellationRequested)
          return;
      }
      Tracer.Information("WatchdogTask: Lsu Pro was closed.");
      if (this.waitForCloseSignalTaskCancellationTokenSource != null)
        this.waitForCloseSignalTaskCancellationTokenSource.Cancel();
      this.CancelMainTask();
    }

    private void StartMainTask()
    {
      this.mainTask = new TaskHelper((Action) (() => this.MainTask()), this.mainTaskCancellationToken);
      this.mainTask.Start();
      this.mainTask.ContinueWith((Action<object>) (t =>
      {
        if (this.mainTask.Exception == null)
          return;
        foreach (object innerException in this.mainTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
      this.mainTask.Wait();
    }

    private void MainTask()
    {
      bool flag;
      do
      {
        Thread.Sleep(1000);
        flag = this.mainTaskCancellationToken.IsCancellationRequested;
        foreach (Updater updater in this.updaters)
          flag = !updater.FlashingOngoing && this.mainTaskCancellationToken.IsCancellationRequested;
      }
      while (!flag);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void WriteLine(string line)
    {
      Tracer.Information(line);
      Console.WriteLine(line);
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

    internal static void PrintHelp()
    {
      Console.WriteLine("ContinuousUpdater.exe");
      Console.WriteLine("Parameters:");
      Console.WriteLine("-ffufile=[ffufilepath]     FFU file path.");
      Console.WriteLine("-maxpayloadtransfersize=[maxpayloadtransfersize]     max payload transfer size.");
      Console.WriteLine("-traceusb");
      Console.WriteLine("-skipwrite");
      Console.WriteLine("-skipplatformidcheck");
      Console.WriteLine("-skipsignaturecheck");
      Console.WriteLine("-skiphashcheck");
      Console.WriteLine("-resetfactorysettings");
    }
  }
}
