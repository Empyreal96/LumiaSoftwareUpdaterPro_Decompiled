// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.EventArgs.UsbDeviceEventArgs
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using System;

namespace Microsoft.LumiaConnectivity.EventArgs
{
  public class UsbDeviceEventArgs : System.EventArgs
  {
    public UsbDeviceEventArgs(UsbDevice usbDevice) => this.UsbDevice = usbDevice != null ? usbDevice : throw new ArgumentNullException(nameof (usbDevice));

    public UsbDevice UsbDevice { get; private set; }
  }
}
