// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.EventArgs.DeviceReadyChangedEventArgs
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

namespace Microsoft.LumiaConnectivity.EventArgs
{
  public class DeviceReadyChangedEventArgs : System.EventArgs
  {
    public DeviceReadyChangedEventArgs(
      ConnectedDevice device,
      bool deviceReady,
      ConnectedDeviceMode mode)
    {
      this.ConnectedDevice = device;
      this.DeviceReady = deviceReady;
      this.Mode = mode;
    }

    public ConnectedDevice ConnectedDevice { get; private set; }

    public bool DeviceReady { get; private set; }

    public ConnectedDeviceMode Mode { get; private set; }
  }
}
