// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.ConnectedDevices
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using Microsoft.LsuPro;
using Microsoft.LumiaConnectivity.EventArgs;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace Microsoft.LumiaConnectivity
{
  public class ConnectedDevices
  {
    private Collection<ConnectedDevice> devices = new Collection<ConnectedDevice>();
    private UsbDeviceScanner usbDeviceDetector;

    public event EventHandler<DeviceConnectedEventArgs> DeviceConnected;

    public event EventHandler<DeviceConnectedEventArgs> DeviceDisconnected;

    public event EventHandler<DeviceModeChangedEventArgs> DeviceModeChanged;

    public event EventHandler<DeviceReadyChangedEventArgs> DeviceReadyChanged;

    public Collection<ConnectedDevice> Devices
    {
      get
      {
        Tracer.Information("Devices: count: {0}", (object) this.devices.Count);
        foreach (ConnectedDevice device in this.devices)
          Tracer.Information("Devices: portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
        return this.devices;
      }
    }

    public void Start()
    {
      if (this.usbDeviceDetector != null)
        return;
      this.usbDeviceDetector = new UsbDeviceScanner();
      this.usbDeviceDetector.DeviceConnected += new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceConnected);
      this.usbDeviceDetector.DeviceDisconnected += new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceDisconnected);
      this.usbDeviceDetector.DeviceEndpointConnected += new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceEndpointConnected);
      this.usbDeviceDetector.Start();
      foreach (UsbDevice device in this.usbDeviceDetector.GetDevices())
        this.devices.Add(LucidConnectivityHelper.GetConnectedDeviceFromUsbDevice(device));
    }

    public void Restart()
    {
      if (this.usbDeviceDetector == null)
        return;
      this.devices.Clear();
      this.usbDeviceDetector.DeviceConnected += new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceConnected);
      this.usbDeviceDetector.DeviceDisconnected += new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceDisconnected);
      this.usbDeviceDetector.DeviceEndpointConnected += new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceEndpointConnected);
      this.usbDeviceDetector.Start();
      foreach (UsbDevice device in this.usbDeviceDetector.GetDevices())
        this.devices.Add(LucidConnectivityHelper.GetConnectedDeviceFromUsbDevice(device));
    }

    public void Stop()
    {
      if (this.usbDeviceDetector == null)
        return;
      this.usbDeviceDetector.Stop();
      this.usbDeviceDetector.DeviceConnected -= new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceConnected);
      this.usbDeviceDetector.DeviceDisconnected -= new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceDisconnected);
      this.usbDeviceDetector.DeviceEndpointConnected -= new EventHandler<UsbDeviceEventArgs>(this.HandleDeviceEndpointConnected);
    }

    public bool WaitForModeChange(
      ConnectedDevice connectedDevice,
      ConnectedDeviceMode newMode,
      int timeout = -1)
    {
      Tracer.Information("WaitForModeChange.");
      int num = 0;
      if (this.devices.IndexOf(connectedDevice) < 0)
      {
        Tracer.Information("WaitForModeChange: Device is not on the list: {0}", (object) connectedDevice.PortId);
        return false;
      }
      while (this.devices[this.devices.IndexOf(connectedDevice)].Mode != newMode)
      {
        num += 200;
        Thread.Sleep(200);
        Tracer.Information("WaitForModeChange: Waiting: PortId: {0}, CurrentMode: {1}, TargetMode: {2}, Time: {3}", (object) connectedDevice.PortId, (object) this.devices[this.devices.IndexOf(connectedDevice)].Mode, (object) newMode, (object) num);
        if (num > timeout)
        {
          Tracer.Error("WaitForModeChange: Timeout: PortId: {0}, CurrentMode: {1}, TargetMode: {2}, Time: {3}", (object) connectedDevice.PortId, (object) this.devices[this.devices.IndexOf(connectedDevice)].Mode, (object) newMode, (object) num);
          return false;
        }
      }
      Tracer.Information("WaitForModeChange: Mode was changed: PortId: {0}, TargetMode: {1}, Time: {2}", (object) connectedDevice.PortId, (object) this.devices[this.devices.IndexOf(connectedDevice)].Mode, (object) num);
      return true;
    }

    public void Refresh(ConnectedDevice connectedDevice)
    {
      if (connectedDevice == null)
        return;
      Tracer.Information(">> Refreshing device connected to port {0} <<", (object) connectedDevice.PortId);
      foreach (ConnectedDevice device in this.Devices)
      {
        if (device.PortId == connectedDevice.PortId)
        {
          try
          {
            this.SendDeviceDisconnectedEvent(device);
            if (device.IsDeviceConnected)
              this.SendDeviceConnectedEvent(device);
          }
          catch (Exception ex)
          {
            object[] objArray = new object[0];
            Tracer.Error(ex, "Refresh failed", objArray);
          }
        }
      }
    }

    private void HandleDeviceConnected(object sender, UsbDeviceEventArgs e)
    {
      if (e.UsbDevice == null)
        return;
      Tracer.Information("HandleDeviceConnected: portID: {0}, VID&PID: {1}", (object) e.UsbDevice.PortId, (object) (e.UsbDevice.Vid + "&" + e.UsbDevice.Pid));
      foreach (ConnectedDevice device in this.devices)
      {
        if (device.PortId == e.UsbDevice.PortId)
        {
          if (device.IsDeviceConnected)
            return;
          this.devices[this.devices.IndexOf(device)].IsDeviceConnected = true;
          this.devices[this.devices.IndexOf(device)].DeviceReady = false;
          this.devices[this.devices.IndexOf(device)].DevicePath = string.Empty;
          this.devices[this.devices.IndexOf(device)].Vid = e.UsbDevice.Vid;
          this.devices[this.devices.IndexOf(device)].Pid = e.UsbDevice.Pid;
          ConnectedDeviceMode mode = this.devices[this.devices.IndexOf(device)].Mode;
          ConnectedDeviceMode deviceMode = LucidConnectivityHelper.GetDeviceMode(e.UsbDevice.Vid, e.UsbDevice.Pid);
          this.devices[this.devices.IndexOf(device)].Mode = deviceMode;
          this.devices[this.devices.IndexOf(device)].TypeDesignator = e.UsbDevice.TypeDesignator;
          this.devices[this.devices.IndexOf(device)].SalesName = e.UsbDevice.SalesName;
          if (deviceMode != mode)
          {
            this.SendDeviceConnectedEvent(this.devices[this.devices.IndexOf(device)]);
            this.SendDeviceModeChangedEvent(mode, this.devices[this.devices.IndexOf(device)]);
            return;
          }
          this.SendDeviceConnectedEvent(this.devices[this.devices.IndexOf(device)]);
          return;
        }
      }
      ConnectedDevice device1 = new ConnectedDevice(e.UsbDevice.PortId, e.UsbDevice.Vid, e.UsbDevice.Pid, LucidConnectivityHelper.GetDeviceMode(e.UsbDevice.Vid, e.UsbDevice.Pid), true, e.UsbDevice.TypeDesignator, e.UsbDevice.SalesName);
      this.devices.Add(device1);
      this.SendDeviceConnectedEvent(device1);
    }

    private void HandleDeviceEndpointConnected(object sender, UsbDeviceEventArgs e)
    {
      if (e.UsbDevice == null)
        return;
      Tracer.Information("HandleDeviceEndpointConnected: portID: {0}, VID&PID: {1}", (object) e.UsbDevice.PortId, (object) (e.UsbDevice.Vid + "&" + e.UsbDevice.Pid));
      foreach (ConnectedDevice device in this.devices)
      {
        if (device.PortId == e.UsbDevice.PortId)
        {
          string empty = string.Empty;
          switch (this.devices[this.devices.IndexOf(device)].Mode)
          {
            case ConnectedDeviceMode.Normal:
            case ConnectedDeviceMode.Label:
            case ConnectedDeviceMode.Uefi:
              if (e.UsbDevice.Interfaces.Count <= 0)
                return;
              string devicePath = e.UsbDevice.Interfaces[0].DevicePath;
              this.devices[this.devices.IndexOf(device)].DeviceReady = true;
              this.devices[this.devices.IndexOf(device)].DevicePath = devicePath;
              this.SendDeviceReadyChangedEvent(this.devices[this.devices.IndexOf(device)]);
              return;
            default:
              return;
          }
        }
      }
    }

    private void HandleDeviceDisconnected(object sender, UsbDeviceEventArgs e)
    {
      if (e.UsbDevice == null)
        return;
      Tracer.Information("HandleDeviceDisconnected: portID: {0}, VID&PID: {1}", (object) e.UsbDevice.PortId, (object) (e.UsbDevice.Vid + "&" + e.UsbDevice.Pid));
      foreach (ConnectedDevice device in this.devices)
      {
        if (device.PortId == e.UsbDevice.PortId)
        {
          this.devices[this.devices.IndexOf(device)].IsDeviceConnected = false;
          this.devices[this.devices.IndexOf(device)].DeviceReady = false;
          this.devices[this.devices.IndexOf(device)].DevicePath = string.Empty;
          this.SendDeviceDisconnectedEvent(this.devices[this.devices.IndexOf(device)]);
          break;
        }
      }
    }

    private void SendDeviceConnectedEvent(ConnectedDevice device)
    {
      if (!device.SuppressConnectedDisconnectedEvents)
      {
        Tracer.Information("SendDeviceConnectedEvent: portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
        EventHandler<DeviceConnectedEventArgs> deviceConnected = this.DeviceConnected;
        if (deviceConnected == null)
          return;
        deviceConnected((object) this, new DeviceConnectedEventArgs(device));
      }
      else
        Tracer.Information("SendDeviceConnectedEvent: event suppressed. portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
    }

    private void SendDeviceDisconnectedEvent(ConnectedDevice device)
    {
      if (!device.SuppressConnectedDisconnectedEvents)
      {
        Tracer.Information("SendDeviceDisconnectedEvent: portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
        EventHandler<DeviceConnectedEventArgs> deviceDisconnected = this.DeviceDisconnected;
        if (deviceDisconnected == null)
          return;
        deviceDisconnected((object) this, new DeviceConnectedEventArgs(device));
      }
      else
        Tracer.Information("SendDeviceDisconnectedEvent: event suppressed. portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
    }

    private void SendDeviceModeChangedEvent(ConnectedDeviceMode oldMode, ConnectedDevice device)
    {
      Tracer.Information("SendDeviceModeChangedEvent: portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
      EventHandler<DeviceModeChangedEventArgs> deviceModeChanged = this.DeviceModeChanged;
      if (deviceModeChanged == null)
        return;
      deviceModeChanged((object) this, new DeviceModeChangedEventArgs(device, oldMode, device.Mode));
    }

    private void SendDeviceReadyChangedEvent(ConnectedDevice device)
    {
      Tracer.Information("SendDeviceReadyChangedEvent: portID: {0}, VID&PID: {1}, mode: {2}, connected: {3}, typeDesignator: {4}", (object) device.PortId, (object) (device.Vid + "&" + device.Pid), (object) device.Mode, (object) device.IsDeviceConnected, (object) device.TypeDesignator);
      EventHandler<DeviceReadyChangedEventArgs> deviceReadyChanged = this.DeviceReadyChanged;
      if (deviceReadyChanged == null)
        return;
      deviceReadyChanged((object) this, new DeviceReadyChangedEventArgs(device, device.DeviceReady, device.Mode));
    }
  }
}
