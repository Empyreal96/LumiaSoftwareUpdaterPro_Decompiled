// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.ConnectedDevice
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

namespace Microsoft.LumiaConnectivity
{
  public class ConnectedDevice
  {
    public ConnectedDevice(
      string portId,
      string vid,
      string pid,
      ConnectedDeviceMode mode,
      bool deviceIsConnected,
      string typeDesignator,
      string salesName)
    {
      this.PortId = portId;
      this.Vid = vid;
      this.Pid = pid;
      this.Mode = mode;
      this.IsDeviceConnected = deviceIsConnected;
      this.SuppressConnectedDisconnectedEvents = false;
      this.TypeDesignator = typeDesignator;
      this.SalesName = salesName;
      this.DeviceReady = false;
      this.DevicePath = string.Empty;
    }

    public string TypeDesignator { get; set; }

    public string SalesName { get; set; }

    public string PortId { get; private set; }

    public string Vid { get; set; }

    public string Pid { get; set; }

    public string DevicePath { get; set; }

    public ConnectedDeviceMode Mode { get; set; }

    public bool IsDeviceConnected { get; set; }

    public bool DeviceReady { get; internal set; }

    public bool SuppressConnectedDisconnectedEvents { get; set; }
  }
}
