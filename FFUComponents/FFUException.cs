// Decompiled with JetBrains decompiler
// Type: FFUComponents.FFUException
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  public class FFUException : Exception
  {
    private string deviceFriendlyName;
    private Guid deviceUniqueID;

    public string DeviceFriendlyName => this.deviceFriendlyName;

    public Guid DeviceUniqueID => this.deviceUniqueID;

    public FFUException(string name, Guid id, string message)
      : base(message)
    {
      this.deviceFriendlyName = name;
      this.deviceUniqueID = id;
    }

    public FFUException(IFFUDevice device)
    {
      this.deviceFriendlyName = device.DeviceFriendlyName;
      this.deviceUniqueID = device.DeviceUniqueID;
    }

    public FFUException(IFFUDevice device, string message, Exception e)
      : base(message, e)
    {
      this.deviceFriendlyName = device.DeviceFriendlyName;
      this.deviceUniqueID = device.DeviceUniqueID;
    }
  }
}
