// Decompiled with JetBrains decompiler
// Type: FFUComponents.FFUManager
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FFUComponents
{
  public static class FFUManager
  {
    private static List<UsbEventWatcher> eventWatchers;
    private static IList<IFFUDeviceInternal> activeFFUDevices;
    private static DisconnectTimer disconnectTimer;
    private static bool isStarted = false;
    public static readonly Guid SimpleIOGuid = new Guid("{67EA0A90-FF06-417D-AB66-6676DCE879CD}");
    public static readonly Guid WinUSBClassGuid = new Guid("{88BAE032-5A81-49F0-BC3D-A4FF138216D6}");
    public static readonly Guid WinUSBFlashingIfGuid = new Guid("{82809DD0-51F5-11E1-B86C-0800200C9A66}");

    public static event EventHandler<ConnectEventArgs> DeviceConnectEvent;

    public static event EventHandler<DisconnectEventArgs> DeviceDisconnectEvent;

    internal static FlashingHost HostLogger { get; private set; }

    internal static FlashingDeviceLogger DeviceLogger { get; private set; }

    private static void OnSimpleIoConnect(string usbDevicePath)
    {
      SimpleIODevice device = new SimpleIODevice(usbDevicePath);
      if (!device.OnConnect(device))
        return;
      IFFUDeviceInternal device1 = (IFFUDeviceInternal) null;
      IFFUDeviceInternal device2 = (IFFUDeviceInternal) null;
      lock (FFUManager.activeFFUDevices)
      {
        IFFUDeviceInternal ffuDeviceInternal1 = FFUManager.activeFFUDevices.SingleOrDefault<IFFUDeviceInternal>((Func<IFFUDeviceInternal, bool>) (deviceInstance => deviceInstance.DeviceUniqueID == device.DeviceUniqueID));
        IFFUDeviceInternal ffuDeviceInternal2 = FFUManager.disconnectTimer.StopTimer((IFFUDeviceInternal) device);
        if (ffuDeviceInternal1 == null && ffuDeviceInternal2 != null)
        {
          FFUManager.activeFFUDevices.Add(ffuDeviceInternal2);
          ffuDeviceInternal1 = ffuDeviceInternal2;
          device2 = ffuDeviceInternal2;
        }
        if (ffuDeviceInternal1 != null && !((SimpleIODevice) ffuDeviceInternal1).OnConnect(device))
        {
          FFUManager.activeFFUDevices.Remove(ffuDeviceInternal1);
          device1 = ffuDeviceInternal1;
          ffuDeviceInternal1 = (IFFUDeviceInternal) null;
        }
        if (ffuDeviceInternal1 == null)
        {
          device2 = (IFFUDeviceInternal) device;
          FFUManager.activeFFUDevices.Add((IFFUDeviceInternal) device);
        }
      }
      FFUManager.OnDisconnect(device1);
      FFUManager.OnConnect(device2);
    }

    private static void OnSimpleIoDisconnect(string usbDevicePath)
    {
      List<IFFUDeviceInternal> ffuDeviceInternalList = new List<IFFUDeviceInternal>();
      lock (FFUManager.activeFFUDevices)
      {
        if (usbDevicePath != null)
        {
          foreach (SimpleIODevice simpleIoDevice in FFUManager.activeFFUDevices.Where<IFFUDeviceInternal>((Func<IFFUDeviceInternal, bool>) (d => d.UsbDevicePath.Equals(usbDevicePath, StringComparison.InvariantCultureIgnoreCase))))
          {
            if (simpleIoDevice != null && !simpleIoDevice.IsConnected())
              ffuDeviceInternalList.Add((IFFUDeviceInternal) simpleIoDevice);
          }
        }
        else
        {
          foreach (IFFUDeviceInternal activeFfuDevice in (IEnumerable<IFFUDeviceInternal>) FFUManager.activeFFUDevices)
          {
            if (activeFfuDevice is SimpleIODevice simpleIoDevice4 && !simpleIoDevice4.IsConnected())
              ffuDeviceInternalList.Add(activeFfuDevice);
          }
        }
        foreach (IFFUDeviceInternal device in ffuDeviceInternalList)
        {
          FFUManager.activeFFUDevices.Remove(device);
          FFUManager.StartTimerIfNecessary(device);
        }
      }
      foreach (IFFUDeviceInternal device in ffuDeviceInternalList)
        FFUManager.OnDisconnect(device);
    }

    private static void StartTimerIfNecessary(IFFUDeviceInternal device)
    {
      if (!device.NeedsTimer())
        return;
      FFUManager.disconnectTimer?.StartTimer(device);
    }

    internal static void OnConnect(IFFUDeviceInternal device)
    {
      if (device == null)
        return;
      if (FFUManager.DeviceConnectEvent != null)
        FFUManager.DeviceConnectEvent((object) null, new ConnectEventArgs((IFFUDevice) device));
      FFUManager.HostLogger.EventWriteDevice_Attach(device.DeviceUniqueID, device.DeviceFriendlyName);
    }

    internal static bool DevicePresent(Guid id)
    {
      bool flag = false;
      lock (FFUManager.activeFFUDevices)
      {
        for (int index = 0; index < FFUManager.activeFFUDevices.Count; ++index)
        {
          if (FFUManager.activeFFUDevices[index].DeviceUniqueID == id)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    internal static void OnDisconnect(IFFUDeviceInternal device)
    {
      if (device == null || FFUManager.DevicePresent(device.DeviceUniqueID))
        return;
      if (FFUManager.DeviceDisconnectEvent != null)
        FFUManager.DeviceDisconnectEvent((object) null, new DisconnectEventArgs(device.DeviceUniqueID));
      FFUManager.HostLogger.EventWriteDevice_Remove(device.DeviceUniqueID, device.DeviceFriendlyName);
    }

    internal static void DisconnectDevice(Guid id)
    {
      List<IFFUDeviceInternal> ffuDeviceInternalList = new List<IFFUDeviceInternal>(FFUManager.activeFFUDevices.Count);
      lock (FFUManager.activeFFUDevices)
      {
        for (int index = 0; index < FFUManager.activeFFUDevices.Count; ++index)
        {
          if (FFUManager.activeFFUDevices[index].DeviceUniqueID == id)
          {
            ffuDeviceInternalList.Add(FFUManager.activeFFUDevices[index]);
            FFUManager.activeFFUDevices.RemoveAt(index);
          }
        }
      }
      foreach (IFFUDeviceInternal device in ffuDeviceInternalList)
      {
        FFUManager.disconnectTimer.StopTimer(device);
        FFUManager.OnDisconnect(device);
      }
    }

    internal static void DisconnectDevice(SimpleIODevice deviceToRemove)
    {
      IFFUDeviceInternal device = (IFFUDeviceInternal) null;
      lock (FFUManager.activeFFUDevices)
      {
        if (FFUManager.activeFFUDevices.Remove((IFFUDeviceInternal) deviceToRemove))
          device = (IFFUDeviceInternal) deviceToRemove;
      }
      if (device == null)
        return;
      FFUManager.disconnectTimer.StopTimer(device);
      FFUManager.OnDisconnect(device);
    }

    static FFUManager()
    {
      FFUManager.activeFFUDevices = (IList<IFFUDeviceInternal>) new List<IFFUDeviceInternal>();
      FFUManager.eventWatchers = new List<UsbEventWatcher>();
      FFUManager.HostLogger = new FlashingHost();
      FFUManager.DeviceLogger = new FlashingDeviceLogger();
    }

    public static void Start()
    {
      lock (FFUManager.eventWatchers)
      {
        if (FFUManager.isStarted)
          return;
        FFUManager.disconnectTimer = new DisconnectTimer();
        if (FFUManager.eventWatchers.Count <= 0)
        {
          IUsbEventSink eventSink = (IUsbEventSink) new SimpleIoEventSink(new SimpleIoEventSink.ConnectHandler(FFUManager.OnSimpleIoConnect), new SimpleIoEventSink.DisconnectHandler(FFUManager.OnSimpleIoDisconnect));
          FFUManager.eventWatchers.Add(new UsbEventWatcher(eventSink, FFUManager.SimpleIOGuid, FFUManager.SimpleIOGuid));
          FFUManager.eventWatchers.Add(new UsbEventWatcher(eventSink, FFUManager.WinUSBClassGuid, FFUManager.WinUSBFlashingIfGuid));
        }
        FFUManager.isStarted = true;
      }
    }

    public static void Stop()
    {
      lock (FFUManager.eventWatchers)
      {
        if (!FFUManager.isStarted)
          return;
        FFUManager.eventWatchers.ForEach((Action<UsbEventWatcher>) (m => m.Dispose()));
        FFUManager.eventWatchers.Clear();
        lock (FFUManager.activeFFUDevices)
          FFUManager.activeFFUDevices.Clear();
        Interlocked.Exchange<DisconnectTimer>(ref FFUManager.disconnectTimer, (DisconnectTimer) null).StopAllTimers();
        FFUManager.isStarted = false;
      }
    }

    public static void GetFlashableDevices(ref ICollection<IFFUDevice> devices)
    {
      devices.Clear();
      lock (FFUManager.activeFFUDevices)
      {
        foreach (IFFUDeviceInternal activeFfuDevice in (IEnumerable<IFFUDeviceInternal>) FFUManager.activeFFUDevices)
          devices.Add((IFFUDevice) activeFfuDevice);
      }
    }
  }
}
