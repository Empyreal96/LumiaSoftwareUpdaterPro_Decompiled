// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.Primitives.IHandleDeviceChanged
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Interop.Win32Types;

namespace Nokia.Lucid.DeviceDetection.Primitives
{
  internal interface IHandleDeviceChanged
  {
    void HandleDeviceChanged(int changeType, ref DEV_BROADCAST_DEVICEINTERFACE data);
  }
}
