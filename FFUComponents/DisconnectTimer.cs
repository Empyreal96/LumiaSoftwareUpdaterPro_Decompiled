// Decompiled with JetBrains decompiler
// Type: FFUComponents.DisconnectTimer
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Collections.Generic;

namespace FFUComponents
{
  internal class DisconnectTimer
  {
    private Dictionary<Guid, DisconnectDevice> devices;

    public DisconnectTimer() => this.devices = new Dictionary<Guid, DisconnectDevice>();

    public void StopAllTimers()
    {
      DisconnectDevice[] array = new DisconnectDevice[this.devices.Values.Count];
      this.devices.Values.CopyTo(array, 0);
      foreach (DisconnectDevice disconnectDevice in array)
        disconnectDevice.Cancel();
    }

    public void StartTimer(IFFUDeviceInternal device)
    {
      lock (this.devices)
      {
        if (this.devices.TryGetValue(device.DeviceUniqueID, out DisconnectDevice _))
          throw new FFUException(device.DeviceFriendlyName, device.DeviceUniqueID, "Received multiple disconnect notifications for device.");
        this.devices[device.DeviceUniqueID] = new DisconnectDevice(device, this.devices);
      }
    }

    public IFFUDeviceInternal StopTimer(IFFUDeviceInternal device)
    {
      IFFUDeviceInternal ffuDeviceInternal = (IFFUDeviceInternal) null;
      lock (this.devices)
      {
        DisconnectDevice disconnectDevice;
        if (this.devices.TryGetValue(device.DeviceUniqueID, out disconnectDevice))
        {
          disconnectDevice.Cancel();
          ffuDeviceInternal = disconnectDevice.FFUDevice;
        }
      }
      return ffuDeviceInternal;
    }
  }
}
