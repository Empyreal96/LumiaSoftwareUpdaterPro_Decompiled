// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceInformationReadEventArgs
// Assembly: Wp8DeviceInformation, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 6707A4BB-F60A-40D7-A2BC-1ABC64317FDD
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceInformation.dll

using System;

namespace Microsoft.LsuPro
{
  public class DeviceInformationReadEventArgs : EventArgs
  {
    public DeviceInformationReadEventArgs(DeviceInformationItem item, string value)
    {
      this.Name = DeviceInformationReadEventArgs.MapDeviceInformationItemWithDisplayPropertyName(item);
      this.Value = value;
    }

    public string Name { get; set; }

    public string Value { get; set; }

    private static string MapDeviceInformationItemWithDisplayPropertyName(DeviceInformationItem item)
    {
      switch (item)
      {
        case DeviceInformationItem.SwVersion:
          return "SoftwareVersion";
        case DeviceInformationItem.SerialNumber:
          return "Imei";
        case DeviceInformationItem.ManufacturerModelName:
          return "OemDeviceName";
        case DeviceInformationItem.OperatorName:
          return "MobileOperator";
        case DeviceInformationItem.RdcAvailable:
          return "Rdc";
        case DeviceInformationItem.PvkAvailable:
          return "Pvk";
        default:
          return item.ToString();
      }
    }
  }
}
