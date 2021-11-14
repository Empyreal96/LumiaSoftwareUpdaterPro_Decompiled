// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.EventArgs.DeviceModeChangedEventArgs
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using System;

namespace Microsoft.LumiaConnectivity.EventArgs
{
  public class DeviceModeChangedEventArgs : System.EventArgs
  {
    public DeviceModeChangedEventArgs(
      ConnectedDevice connectedDevice,
      ConnectedDeviceMode oldMode,
      ConnectedDeviceMode newMode)
    {
      this.ConnectedDevice = connectedDevice != null ? connectedDevice : throw new ArgumentNullException(nameof (connectedDevice));
      this.OldMode = oldMode;
      this.NewMode = newMode;
    }

    public ConnectedDevice ConnectedDevice { get; private set; }

    public ConnectedDeviceMode OldMode { get; private set; }

    public ConnectedDeviceMode NewMode { get; private set; }
  }
}
