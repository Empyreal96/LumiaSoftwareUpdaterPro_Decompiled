// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.DeviceChangedEventArgs
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.DeviceDetection
{
  public sealed class DeviceChangedEventArgs : EventArgs
  {
    private readonly DeviceChangeAction action;
    private readonly string path;
    private readonly DeviceType deviceType;

    public DeviceChangedEventArgs(DeviceChangeAction action, string path, DeviceType deviceType)
    {
      this.action = action;
      this.path = path;
      this.deviceType = deviceType;
    }

    public DeviceChangeAction Action => this.action;

    public string Path => this.path;

    public DeviceType DeviceType => this.deviceType;
  }
}
