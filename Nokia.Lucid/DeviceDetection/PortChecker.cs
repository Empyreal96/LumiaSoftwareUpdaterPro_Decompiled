// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.PortChecker
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.DeviceDetection
{
  public class PortChecker
  {
    public PortChecker(string[] parentDeviceLocationPaths) => this.PortIds = parentDeviceLocationPaths != null ? parentDeviceLocationPaths : throw new ArgumentNullException(nameof (parentDeviceLocationPaths));

    public PortChecker(string parentDeviceLocationPath) => this.PortIds = parentDeviceLocationPath != null ? new string[1]
    {
      parentDeviceLocationPath
    } : throw new ArgumentNullException(nameof (parentDeviceLocationPath));

    public string[] PortIds { get; private set; }

    public int Check(string locationPath)
    {
      if (locationPath == null)
        throw new ArgumentNullException(nameof (locationPath));
      int num = 0;
      foreach (string portId in this.PortIds)
      {
        if (locationPath.Contains(portId))
          return num;
        ++num;
      }
      return -1;
    }
  }
}
