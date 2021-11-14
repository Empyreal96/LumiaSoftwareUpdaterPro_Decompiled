// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyModeDetector
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using Nokia.Lucid;
using Nokia.Lucid.DeviceDetection;
using Nokia.Lucid.DeviceInformation;
using Nokia.Lucid.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.LsuPro
{
  public class EmergencyModeDetector
  {
    private static readonly Guid GuidUsbDevice = new Guid(2782707472U, (ushort) 25904, (ushort) 4562, (byte) 144, (byte) 31, (byte) 0, (byte) 192, (byte) 79, (byte) 185, (byte) 81, (byte) 237);
    private static readonly Guid GuidEmergencyMode = new Guid(1910413645U, (ushort) 35708, (ushort) 17371, (byte) 162, (byte) 126, (byte) 42, (byte) 231, (byte) 205, (byte) 87, (byte) 154, (byte) 12);
    private DeviceWatcher deviceWatcher;
    private IDisposable disposableToken;
    private readonly DeviceTypeMap deviceTypeMap = new DeviceTypeMap((IEnumerable<KeyValuePair<Guid, DeviceType>>) new Dictionary<Guid, DeviceType>()
    {
      {
        EmergencyModeDetector.GuidUsbDevice,
        DeviceType.PhysicalDevice
      },
      {
        EmergencyModeDetector.GuidEmergencyMode,
        DeviceType.Interface
      }
    });
    private readonly Expression<Func<DeviceIdentifier, bool>> deviceVidPidFilter = (Expression<Func<DeviceIdentifier, bool>>) (s => s.Vid("05C6") && s.Pid("9008"));

    public event EventHandler<EmergencyModeDeviceEventArgs> EmergencyModeDeviceConnected;

    public void Start()
    {
      if (this.deviceWatcher != null)
        return;
      this.deviceWatcher = new DeviceWatcher();
      this.deviceWatcher.Filter = this.deviceVidPidFilter;
      this.deviceWatcher.DeviceTypeMap = this.deviceTypeMap;
      this.deviceWatcher.DeviceChanged += new EventHandler<DeviceChangedEventArgs>(this.DeviceWatcherDeviceChanged);
      foreach (EmergencyModeDeviceEventArgs connectedDevice in this.GetConnectedDevices())
        this.SendConnectedEvent(connectedDevice);
      this.disposableToken = this.deviceWatcher.Start();
    }

    public void Stop()
    {
      if (this.disposableToken == null)
        return;
      this.disposableToken.Dispose();
      this.disposableToken = (IDisposable) null;
    }

    private IEnumerable<EmergencyModeDeviceEventArgs> GetConnectedDevices() => (IEnumerable<EmergencyModeDeviceEventArgs>) new DeviceInfoSet()
    {
      DeviceTypeMap = this.deviceTypeMap,
      Filter = this.deviceVidPidFilter
    }.EnumeratePresentDevices().Select<DeviceInfo, EmergencyModeDeviceEventArgs>((Func<DeviceInfo, EmergencyModeDeviceEventArgs>) (device => this.GetEventArgs(device))).Where<EmergencyModeDeviceEventArgs>((Func<EmergencyModeDeviceEventArgs, bool>) (args => args != null)).ToList<EmergencyModeDeviceEventArgs>();

    private void DeviceWatcherDeviceChanged(object sender, DeviceChangedEventArgs e)
    {
      if (e.Action != DeviceChangeAction.Attach)
        return;
      EmergencyModeDeviceEventArgs eventArgs = this.GetEventArgs(new DeviceInfoSet()
      {
        DeviceTypeMap = this.deviceTypeMap,
        Filter = this.deviceVidPidFilter
      }.GetDevice(e.Path));
      if (eventArgs == null)
        return;
      this.SendConnectedEvent(eventArgs);
    }

    private EmergencyModeDeviceEventArgs GetEventArgs(DeviceInfo device)
    {
      string comPort;
      return this.IsQComDriver(device, out comPort) ? new EmergencyModeDeviceEventArgs(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\\\\.\\{0}", (object) comPort), EmergencyDriverType.QComDriver) : (device.DeviceInterfaceGuid == EmergencyModeDetector.GuidEmergencyMode ? new EmergencyModeDeviceEventArgs(device.Path, EmergencyDriverType.NokiaDriver) : (EmergencyModeDeviceEventArgs) null);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private bool IsQComDriver(DeviceInfo device, out string comPort)
    {
      comPort = string.Empty;
      try
      {
        if (device.DeviceInterfaceGuid == EmergencyModeDetector.GuidEmergencyMode)
          return false;
        string str = device.ReadFriendlyName();
        if (!str.Contains("COM"))
          return false;
        comPort = str.Substring(str.LastIndexOf('(') + 1).Replace(")", string.Empty);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private void SendConnectedEvent(EmergencyModeDeviceEventArgs args)
    {
      EventHandler<EmergencyModeDeviceEventArgs> modeDeviceConnected = this.EmergencyModeDeviceConnected;
      if (modeDeviceConnected == null)
        return;
      modeDeviceConnected((object) this, args);
    }
  }
}
