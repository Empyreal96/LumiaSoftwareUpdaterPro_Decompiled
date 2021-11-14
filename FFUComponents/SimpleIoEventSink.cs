// Decompiled with JetBrains decompiler
// Type: FFUComponents.SimpleIoEventSink
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

namespace FFUComponents
{
  internal class SimpleIoEventSink : IUsbEventSink
  {
    private SimpleIoEventSink.ConnectHandler onConnect;
    private SimpleIoEventSink.DisconnectHandler onDisconnect;

    public SimpleIoEventSink(
      SimpleIoEventSink.ConnectHandler connect,
      SimpleIoEventSink.DisconnectHandler disconnect)
    {
      this.onConnect = connect;
      this.onDisconnect = disconnect;
    }

    public void OnDeviceConnect(string usbDevicePath) => this.onConnect(usbDevicePath);

    public void OnDeviceDisconnect(string usbDevicePath) => this.onDisconnect(usbDevicePath);

    public delegate void ConnectHandler(string deviceName);

    public delegate void DisconnectHandler(string deviceName);
  }
}
