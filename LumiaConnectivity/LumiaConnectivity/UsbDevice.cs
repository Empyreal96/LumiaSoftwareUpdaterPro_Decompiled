// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.UsbDevice
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LumiaConnectivity
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class UsbDevice
  {
    private List<UsbDeviceEndpoint> interfaces;

    public UsbDevice(
      string portId,
      string vid,
      string pid,
      string locationPath,
      string typeDesignator,
      string salesName)
    {
      this.PortId = portId;
      this.LocationPath = locationPath;
      this.Vid = vid;
      this.Pid = pid;
      this.TypeDesignator = typeDesignator;
      this.SalesName = salesName;
      this.interfaces = new List<UsbDeviceEndpoint>();
    }

    public string PortId { get; private set; }

    public string LocationPath { get; private set; }

    public string Vid { get; private set; }

    public string Pid { get; private set; }

    public string TypeDesignator { get; private set; }

    public string SalesName { get; private set; }

    public ReadOnlyCollection<UsbDeviceEndpoint> Interfaces => this.interfaces.AsReadOnly();

    public bool SamePortAs(UsbDevice usbDevice) => this.PortId.Equals(usbDevice.PortId, StringComparison.OrdinalIgnoreCase);

    internal void AddInterface(string devicePath) => this.interfaces.Add(new UsbDeviceEndpoint()
    {
      DevicePath = devicePath
    });
  }
}
