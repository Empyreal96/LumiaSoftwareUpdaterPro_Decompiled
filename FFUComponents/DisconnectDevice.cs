// Decompiled with JetBrains decompiler
// Type: FFUComponents.DisconnectDevice
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace FFUComponents
{
  internal class DisconnectDevice
  {
    private EventWaitHandle cancelEvent;
    private Dictionary<Guid, DisconnectDevice> DiscCollection;
    private Thread removalThread;

    public IFFUDeviceInternal FFUDevice { get; private set; }

    public Guid DeviceUniqueId => this.FFUDevice.DeviceUniqueID;

    private static void WaitAndRemove(object obj)
    {
      DisconnectDevice disconnectDevice = obj as DisconnectDevice;
      if (disconnectDevice.WaitForReconnect())
        return;
      disconnectDevice.Remove();
    }

    public DisconnectDevice(
      IFFUDeviceInternal device,
      Dictionary<Guid, DisconnectDevice> collection)
    {
      this.FFUDevice = device;
      this.DiscCollection = collection;
      this.cancelEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
      this.removalThread = new Thread(new ParameterizedThreadStart(DisconnectDevice.WaitAndRemove));
      this.removalThread.Start((object) this);
    }

    ~DisconnectDevice() => this.cancelEvent.Set();

    public void Cancel()
    {
      this.cancelEvent.Set();
      this.Remove();
    }

    public bool WaitForReconnect() => this.cancelEvent.WaitOne(30000, false);

    private void Remove()
    {
      lock (this.DiscCollection)
        this.DiscCollection.Remove(this.DeviceUniqueId);
    }
  }
}
