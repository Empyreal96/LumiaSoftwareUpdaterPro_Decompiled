// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OemFlasherDetector
// Assembly: Wp8OemFlasher, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0F564F-0EF5-4D78-8BB5-4C7A3BFE4321
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8OemFlasher.dll

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using Nokia.Lucid;
using Nokia.Lucid.DeviceDetection;
using Nokia.Lucid.DeviceInformation;
using Nokia.Lucid.Primitives;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.LsuPro
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class OemFlasherDetector
  {
    private const string GuidSimpleIo = "67EA0A90-FF06-417d-AB66-6676DCE879CD";
    private const string MicrosoftFlashModeVid = "045E";
    private const string MicrosoftFlashModePid = "062A";
    private static readonly Guid GuidUsbDevice = new Guid(2782707472U, (ushort) 25904, (ushort) 4562, (byte) 144, (byte) 31, (byte) 0, (byte) 192, (byte) 79, (byte) 185, (byte) 81, (byte) 237);
    private DeviceTypeMap map = new DeviceTypeMap(OemFlasherDetector.GuidUsbDevice, DeviceType.PhysicalDevice);
    private Expression<Func<DeviceIdentifier, bool>> filter;
    private DeviceWatcher deviceWatcher;
    private IDisposable disposableToken;

    public event EventHandler<OemDeviceDetectionEventArgs> DeviceConnected;

    public event EventHandler<OemDeviceDetectionEventArgs> DeviceDisconnected;

    public OemFlasherDetector()
    {
      this.map = UsbDeviceScanner.SupportedDevicesMap;
      this.filter = UsbDeviceScanner.GetSupportedVidAndPidExpression();
    }

    public static bool IsDriverInstalled() => Toolkit.InfFilesContainGuidString("67EA0A90-FF06-417d-AB66-6676DCE879CD");

    public OemFlasherDeviceMap GetCurrentDeviceConnectionStatus()
    {
      OemFlasherDeviceMap flasherDeviceMap = new OemFlasherDeviceMap()
      {
        DevicesInMicrosoftFlashMode = 0,
        OtherDevices = 0
      };
      foreach (DeviceInfo enumeratePresentDevice in new DeviceInfoSet()
      {
        DeviceTypeMap = this.map,
        Filter = this.filter
      }.EnumeratePresentDevices())
      {
        if (enumeratePresentDevice.DeviceType == DeviceType.PhysicalDevice)
        {
          if (OemFlasherDetector.DeviceIsInMicrosoftFlashMode(enumeratePresentDevice.Path))
            ++flasherDeviceMap.DevicesInMicrosoftFlashMode;
          else
            ++flasherDeviceMap.OtherDevices;
        }
      }
      return flasherDeviceMap;
    }

    public void Start()
    {
      if (this.deviceWatcher != null)
        return;
      this.deviceWatcher = new DeviceWatcher()
      {
        Filter = this.filter,
        DeviceTypeMap = this.map
      };
      this.deviceWatcher.DeviceChanged += new EventHandler<DeviceChangedEventArgs>(this.DeviceWatcherDeviceChanged);
      this.disposableToken = this.deviceWatcher.Start();
      Tracer.Information("OEM Flasher device detection started");
    }

    public void Stop()
    {
      if (this.disposableToken == null)
        return;
      this.deviceWatcher.DeviceChanged -= new EventHandler<DeviceChangedEventArgs>(this.DeviceWatcherDeviceChanged);
      this.deviceWatcher = (DeviceWatcher) null;
      this.disposableToken.Dispose();
      this.disposableToken = (IDisposable) null;
    }

    private void DeviceWatcherDeviceChanged(object sender, DeviceChangedEventArgs e)
    {
      OemFlasherDeviceMap connectionStatus = this.GetCurrentDeviceConnectionStatus();
      if (e.DeviceType != DeviceType.PhysicalDevice)
        return;
      if (e.Action == DeviceChangeAction.Attach)
      {
        if (this.DeviceInUefiMode(e.Path))
          return;
        this.SendConnectedEvent(new OemDeviceDetectionEventArgs(connectionStatus));
      }
      else
      {
        if (e.Action != DeviceChangeAction.Detach)
          return;
        this.SendDisconnectedEvent(new OemDeviceDetectionEventArgs(connectionStatus));
      }
    }

    private bool DeviceInUefiMode(string devicePath)
    {
      try
      {
        string vid;
        string pid;
        LucidConnectivityHelper.GetVidAndPid(new DeviceInfoSet()
        {
          DeviceTypeMap = this.map,
          Filter = this.filter
        }.GetDevice(devicePath).InstanceId, out vid, out pid);
        if (LucidConnectivityHelper.GetDeviceMode(vid, pid) == ConnectedDeviceMode.Uefi)
        {
          Tracer.Information("Ignoring UEFI mode");
          return true;
        }
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Failed to check if device is in UEFI mode", objArray);
      }
      return false;
    }

    internal static bool DeviceIsInMicrosoftFlashMode(string devicePath) => DeviceIdentifier.Parse(devicePath).Matches("045E", "062A", OemFlasherDetector.GuidUsbDevice);

    private void SendConnectedEvent(OemDeviceDetectionEventArgs args)
    {
      EventHandler<OemDeviceDetectionEventArgs> deviceConnected = this.DeviceConnected;
      if (deviceConnected == null)
        return;
      deviceConnected((object) this, args);
    }

    private void SendDisconnectedEvent(OemDeviceDetectionEventArgs args)
    {
      EventHandler<OemDeviceDetectionEventArgs> deviceDisconnected = this.DeviceDisconnected;
      if (deviceDisconnected == null)
        return;
      deviceDisconnected((object) this, args);
    }
  }
}
