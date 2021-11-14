// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.UsbDeviceScanner
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using Microsoft.LsuPro;
using Microsoft.LumiaConnectivity.EventArgs;
using Nokia.Lucid;
using Nokia.Lucid.DeviceDetection;
using Nokia.Lucid.DeviceInformation;
using Nokia.Lucid.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LumiaConnectivity
{
  public class UsbDeviceScanner
  {
    private readonly InterfaceHandlingLocks interfaceLocks = new InterfaceHandlingLocks();
    private static readonly Guid UsbDeviceClassGuid = new Guid(2782707472U, (ushort) 25904, (ushort) 4562, (byte) 144, (byte) 31, (byte) 0, (byte) 192, (byte) 79, (byte) 185, (byte) 81, (byte) 237);
    private static DeviceTypeMap supportedGuidMap = DeviceTypeMap.DefaultMap.InterfaceClasses.Aggregate<Guid, DeviceTypeMap>(new DeviceTypeMap(UsbDeviceScanner.UsbDeviceClassGuid, DeviceType.PhysicalDevice), (Func<DeviceTypeMap, Guid, DeviceTypeMap>) ((current, guid) => current.SetMapping(guid, DeviceType.Interface)));
    private readonly Dictionary<string, Tuple<string, UsbDevice>> deviceDictionary = new Dictionary<string, Tuple<string, UsbDevice>>();
    private DeviceWatcher deviceWatcher;
    private IDisposable deviceWatcherDisposableToken;

    public event EventHandler<UsbDeviceEventArgs> DeviceConnected;

    public event EventHandler<UsbDeviceEventArgs> DeviceDisconnected;

    public event EventHandler<UsbDeviceEventArgs> DeviceEndpointConnected;

    public static DeviceTypeMap SupportedDevicesMap
    {
      get => UsbDeviceScanner.supportedGuidMap;
      internal set => UsbDeviceScanner.supportedGuidMap = value;
    }

    public void Start()
    {
      Tracer.Information(">>>> Starting USB device scanner <<<<");
      this.deviceDictionary.Clear();
      if (this.deviceWatcher == null)
      {
        this.deviceWatcher = new DeviceWatcher()
        {
          DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
          Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
        };
        this.deviceWatcher.DeviceChanged += new EventHandler<DeviceChangedEventArgs>(this.DeviceWatcherOnDeviceChanged);
        this.deviceWatcherDisposableToken = this.deviceWatcher.Start();
      }
      Tracer.Information(">>>> USB device scanner started <<<<");
    }

    public void Stop()
    {
      Tracer.Information(">>>> Stopping USB device scanner <<<<");
      try
      {
        if (this.deviceWatcherDisposableToken != null)
        {
          this.deviceWatcherDisposableToken.Dispose();
          this.deviceWatcherDisposableToken = (IDisposable) null;
        }
        if (this.deviceWatcher != null)
        {
          this.deviceWatcher.DeviceChanged -= new EventHandler<DeviceChangedEventArgs>(this.DeviceWatcherOnDeviceChanged);
          this.deviceWatcher = (DeviceWatcher) null;
        }
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Stopping Lucid device watcher failed", objArray);
      }
      Tracer.Information(">>>> USB device scanner stopped <<<<");
    }

    public ReadOnlyCollection<UsbDevice> GetDevices()
    {
      Tracer.Information(">>Getting list of connected USB devices");
      List<UsbDevice> usbDeviceList = new List<UsbDevice>();
      try
      {
        DeviceInfoSet deviceInfoSet = new DeviceInfoSet()
        {
          DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
          Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
        };
        Tracer.Information("-> USB devices enumeration start <-");
        int num = 0;
        foreach (DeviceInfo enumeratePresentDevice in deviceInfoSet.EnumeratePresentDevices())
        {
          ++num;
          Tracer.Information("({0}) : {1}", (object) num, (object) enumeratePresentDevice.InstanceId);
          switch (enumeratePresentDevice.DeviceType)
          {
            case DeviceType.PhysicalDevice:
              Tracer.Information("PHYSICAL USB DEVICE");
              UsbDevice usbDevice = this.GetUsbDevice(enumeratePresentDevice);
              if (usbDevice != null)
              {
                this.InsertDeviceToDictionary(enumeratePresentDevice.InstanceId, usbDevice);
                usbDeviceList.Add(usbDevice);
                Tracer.Information("Device added: {0}&{1} at {2}", (object) usbDevice.Vid, (object) usbDevice.Pid, (object) usbDevice.PortId);
                continue;
              }
              continue;
            case DeviceType.Interface:
              Tracer.Information("INTERFACE");
              string locationPath = UsbDeviceScanner.GetLocationPath((IDevicePropertySet) enumeratePresentDevice);
              if (!string.IsNullOrEmpty(locationPath))
              {
                string fromLocationPath = this.GetDictionaryKeyFromLocationPath(locationPath);
                if (!string.IsNullOrEmpty(fromLocationPath))
                {
                  if (!LucidConnectivityHelper.IsWrongDefaultNcsdInterface(enumeratePresentDevice))
                  {
                    this.deviceDictionary[fromLocationPath].Item2.AddInterface(enumeratePresentDevice.Path);
                    Tracer.Information("Endpoint {0} added to device connected to {1}", (object) enumeratePresentDevice.Path, (object) this.deviceDictionary[fromLocationPath].Item2.PortId);
                    continue;
                  }
                  Tracer.Warning("Wrong interface {0} for NCSd communication. Ignoring.", (object) enumeratePresentDevice.Path);
                  continue;
                }
                Tracer.Warning("No physical device entry found for this interface endpoint");
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex, "Can't get devices: {0}", (object) ex.Message);
      }
      Tracer.Information("<< List of connected USB devices retrieved ({0} devices found)", (object) usbDeviceList.Count);
      Tracer.Information("-> USB devices enumeration end <-");
      return usbDeviceList.AsReadOnly();
    }

    internal static string GetLocationPath(IDevicePropertySet propertySet)
    {
      for (int index = 0; index < 40; ++index)
      {
        Tracer.Information("Reading location paths (attempt {0})", (object) index);
        try
        {
          propertySet.EnumeratePropertyKeys();
          Tracer.Information("Property keys enumerated");
          string[] strArray = propertySet.ReadLocationPaths();
          if (strArray.Length != 0)
          {
            Tracer.Information("Location path: {0}", (object) strArray[0]);
            return strArray[0];
          }
        }
        catch (Exception ex)
        {
          object[] objArray = new object[0];
          Tracer.Warning(ex, "Location paths not found", objArray);
          if (index < 39)
          {
            Tracer.Warning("Retrying after delay");
            Thread.Sleep(100 * index + 100);
          }
        }
      }
      Tracer.Error("Location paths not found (after all retries).");
      return string.Empty;
    }

    internal static bool GetNeededProperties(
      DeviceInfo deviceInfo,
      out string locationPath,
      out string locationInfo,
      out string busReportedDeviceDescription)
    {
      locationPath = string.Empty;
      locationInfo = string.Empty;
      busReportedDeviceDescription = string.Empty;
      IDevicePropertySet propertySet = (IDevicePropertySet) deviceInfo;
      try
      {
        locationPath = UsbDeviceScanner.GetLocationPath(propertySet);
        Tracer.Information("Location path = {0}", (object) locationPath);
        try
        {
          busReportedDeviceDescription = propertySet.ReadBusReportedDeviceDescription();
        }
        catch (Exception ex)
        {
          object[] objArray = new object[0];
          Tracer.Warning(ex, "Property not found", objArray);
          busReportedDeviceDescription = "UNKNOWN_DEVICE_DESCRIPTION";
        }
        Tracer.Information("Bus reported device description = {0}", (object) busReportedDeviceDescription);
        locationInfo = propertySet.ReadLocationInformation();
        Tracer.Information("Location info = {0}", (object) locationInfo);
        if (!string.IsNullOrEmpty(locationPath))
          return true;
        Tracer.Warning("Location path is empty");
        return false;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Property not found", objArray);
        return false;
      }
    }

    internal static void DetermineDeviceTypeDesignatorAndSalesName(
      string pid,
      string busReportedDeviceDescription,
      out string typeDesignator,
      out string salesName)
    {
      if ((pid == "066E" ? 1 : (pid == "0714" ? 1 : 0)) != 0)
      {
        typeDesignator = "Device in UEFI mode";
        salesName = "Device in UEFI mode";
      }
      else
        LucidConnectivityHelper.ParseTypeDesignatorAndSalesName(busReportedDeviceDescription, out typeDesignator, out salesName);
      Tracer.Information("Type designator: {0}, Sales name: {1}", (object) typeDesignator, (object) salesName);
    }

    internal static string GetPhysicalPortId(string locationPath, string locationInfo)
    {
      string str1 = string.Empty;
      if (!string.IsNullOrEmpty(locationPath))
        str1 = LucidConnectivityHelper.LocationPath2ControllerId(locationPath);
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) str1, (object) LucidConnectivityHelper.GetHubAndPort(locationInfo));
      Tracer.Information("Parsed port identifier: {0}", (object) str2);
      return str2;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "on purpose")]
    public static Expression<Func<DeviceIdentifier, bool>> GetSupportedVidAndPidExpression() => (Expression<Func<DeviceIdentifier, bool>>) (s => s.Vid("0421") && (s.Pid("0660") && s.MI(4) || s.Pid("0661") && s.MI(new int[]
    {
      2,
      3
    }) || s.Pid("066E") || s.Pid("06FC") || s.Pid("0713") || s.Pid("0714")) || s.Vid("05C6") && (s.Pid("319B") || s.Pid("9001") || s.Pid("9006") || s.Pid("9008")) || s.Vid("3495") && s.Pid("00E0") || s.Vid("045E") && (s.Pid("062A") || s.Pid("04EC") || s.Pid("0A00") || s.Pid("0A01") || s.Pid("0A02") || s.Pid("0A03") || s.Pid("9006")) || s.Vid("0421") && (!s.Pid("0601") && !s.Pid("0608") && !s.Pid("060A") && !s.Pid("0604")));

    private static string FindGuidFromDevicePath(string devicePath)
    {
      int startIndex = devicePath.LastIndexOf('{');
      return startIndex <= 0 ? string.Empty : devicePath.Substring(startIndex);
    }

    private void DeviceWatcherOnDeviceChanged(object sender, DeviceChangedEventArgs e)
    {
      string guid = UsbDeviceScanner.FindGuidFromDevicePath(e.Path);
      Tracer.Information("<LUCID>: DeviceChanged '{0}' event handling START ({1})", (object) e.Action, (object) guid);
      Task.Factory.StartNew((Action) (() =>
      {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        if (e.Action == DeviceChangeAction.Attach)
        {
          switch (e.DeviceType)
          {
            case DeviceType.PhysicalDevice:
              this.HandleDeviceAdded(e);
              break;
            case DeviceType.Interface:
              this.HandleInterfaceAdded(e);
              break;
          }
        }
        else if (e.DeviceType == DeviceType.PhysicalDevice)
          this.HandleDeviceRemoved(e);
        Tracer.Information("<LUCID>: DeviceChanged '{0}' event handling END ({1}). Duration = {2}", (object) e.Action, (object) guid, (object) stopwatch.Elapsed);
      }));
    }

    private void HandleDeviceAdded(DeviceChangedEventArgs e)
    {
      try
      {
        Tracer.Information("Device connected");
        DeviceInfo device = new DeviceInfoSet()
        {
          DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
          Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
        }.GetDevice(e.Path);
        Tracer.Information("Device path: {0}", (object) device.Path);
        Tracer.Information("InstanceId: {0}", (object) device.InstanceId);
        string instanceId = device.InstanceId;
        this.interfaceLocks.CreateLock(instanceId);
        this.interfaceLocks.Lock(instanceId);
        UsbDevice usbDevice = this.GetUsbDevice(device);
        if (usbDevice == null)
        {
          Tracer.Information("USB device was null => Ignored");
        }
        else
        {
          this.InsertDeviceToDictionary(device.InstanceId, usbDevice);
          this.SendConnectionAddedEvent(usbDevice);
          Tracer.Information("Device added event sent: {0}/{1}&{2}", (object) usbDevice.PortId, (object) usbDevice.Vid, (object) usbDevice.Pid);
          this.interfaceLocks.Unlock(instanceId);
        }
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Error handling connected device", objArray);
      }
    }

    private void HandleInterfaceAdded(DeviceChangedEventArgs e)
    {
      try
      {
        Tracer.Information("Interface added event handling started");
        DeviceInfo device = new DeviceInfoSet()
        {
          DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
          Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
        }.GetDevice(e.Path);
        string interfaceUnlockKey = this.GetInterfaceUnlockKey(device);
        this.interfaceLocks.Wait(interfaceUnlockKey, 5000);
        this.interfaceLocks.Discard(interfaceUnlockKey);
        if (LucidConnectivityHelper.IsWrongDefaultNcsdInterface(device))
        {
          Tracer.Warning("Wrong interface {0} for NCSd communication. Ignoring.", (object) device.Path);
        }
        else
        {
          Tracer.Information("Device path: {0}", (object) device.Path);
          Tracer.Information("InstanceId: {0}", (object) device.InstanceId);
          string locationPath = UsbDeviceScanner.GetLocationPath((IDevicePropertySet) device);
          if (!string.IsNullOrEmpty(locationPath))
          {
            string fromLocationPath = this.GetDictionaryKeyFromLocationPath(locationPath);
            if (!string.IsNullOrEmpty(fromLocationPath))
            {
              this.deviceDictionary[fromLocationPath].Item2.AddInterface(e.Path);
              this.SendConnectionEndpointAddedEvent(this.deviceDictionary[fromLocationPath].Item2);
            }
          }
          else
            Tracer.Information("Ignored");
          Tracer.Information("Interface added event handling ended");
        }
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Error handling interface added", objArray);
      }
    }

    private void HandleDeviceRemoved(DeviceChangedEventArgs e)
    {
      try
      {
        Tracer.Information("Device disconnected");
        string keyFromInstanceId = this.GetDictionaryKeyFromInstanceId(new DeviceInfoSet()
        {
          DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
          Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
        }.GetDevice(e.Path).InstanceId);
        if (!string.IsNullOrEmpty(keyFromInstanceId))
        {
          UsbDevice connection = this.deviceDictionary[keyFromInstanceId].Item2;
          this.SendConnectionRemovedEvent(connection);
          Tracer.Information("Device removed event sent: {0}/{1}&{2}", (object) connection.PortId, (object) connection.Vid, (object) connection.Pid);
          this.deviceDictionary.Remove(keyFromInstanceId);
        }
        else
          Tracer.Information("Ignored");
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Error handling disconnected device", objArray);
      }
    }

    private void InsertDeviceToDictionary(string instanceId, UsbDevice usbDevice)
    {
      string fromLocationPath = this.GetDictionaryKeyFromLocationPath(usbDevice.LocationPath);
      if (!string.IsNullOrEmpty(fromLocationPath))
        this.deviceDictionary[fromLocationPath] = new Tuple<string, UsbDevice>(instanceId, usbDevice);
      else
        this.deviceDictionary.Add(usbDevice.LocationPath, new Tuple<string, UsbDevice>(instanceId, usbDevice));
    }

    private string GetDictionaryKeyFromLocationPath(string locationPath)
    {
      foreach (string key in this.deviceDictionary.Keys)
      {
        if (locationPath.StartsWith(key, StringComparison.OrdinalIgnoreCase))
          return key;
      }
      return string.Empty;
    }

    private string GetDictionaryKeyFromInstanceId(string instanceId)
    {
      using (IEnumerator<KeyValuePair<string, Tuple<string, UsbDevice>>> enumerator = this.deviceDictionary.Where<KeyValuePair<string, Tuple<string, UsbDevice>>>((Func<KeyValuePair<string, Tuple<string, UsbDevice>>, bool>) (item => instanceId == item.Value.Item1)).GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current.Key;
      }
      return string.Empty;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private UsbDevice GetUsbDevice(DeviceInfo deviceInfo)
    {
      try
      {
        Tracer.Information("Getting USB device");
        string instanceId = deviceInfo.InstanceId;
        Tracer.Information("Device detected: {0}", (object) instanceId);
        string locationPath;
        string locationInfo;
        string busReportedDeviceDescription;
        if (!UsbDeviceScanner.GetNeededProperties(deviceInfo, out locationPath, out locationInfo, out busReportedDeviceDescription))
        {
          Tracer.Error("Needed properties are not available");
          return (UsbDevice) null;
        }
        string physicalPortId = UsbDeviceScanner.GetPhysicalPortId(locationPath, locationInfo);
        string vid;
        string pid;
        LucidConnectivityHelper.GetVidAndPid(instanceId, out vid, out pid);
        string typeDesignator;
        string salesName;
        UsbDeviceScanner.DetermineDeviceTypeDesignatorAndSalesName(pid, busReportedDeviceDescription, out typeDesignator, out salesName);
        Tracer.Information("USB device: {0}/{1}&{2}", (object) physicalPortId, (object) vid, (object) pid);
        return new UsbDevice(physicalPortId, vid, pid, locationPath, typeDesignator, salesName);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Cannot get USB device", objArray);
      }
      Tracer.Information("Device not compatible");
      return (UsbDevice) null;
    }

    private void SendConnectionAddedEvent(UsbDevice connection)
    {
      Tracer.Information("SendConnectionAddedEvent: portID: {0}, VID&PID: {1}", (object) connection.PortId, (object) (connection.Vid + "&" + connection.Pid));
      EventHandler<UsbDeviceEventArgs> deviceConnected = this.DeviceConnected;
      if (deviceConnected == null)
        return;
      deviceConnected((object) this, new UsbDeviceEventArgs(connection));
    }

    private void SendConnectionEndpointAddedEvent(UsbDevice connection)
    {
      Tracer.Information("SendConnectionEndpointAddedEvent: portID: {0}, VID&PID: {1}", (object) connection.PortId, (object) (connection.Vid + "&" + connection.Pid));
      EventHandler<UsbDeviceEventArgs> endpointConnected = this.DeviceEndpointConnected;
      if (endpointConnected == null)
        return;
      endpointConnected((object) this, new UsbDeviceEventArgs(connection));
    }

    private void SendConnectionRemovedEvent(UsbDevice connection)
    {
      Tracer.Information("SendConnectionRemovedEvent: portID: {0}, VID&PID: {1}", (object) connection.PortId, (object) (connection.Vid + "&" + connection.Pid));
      EventHandler<UsbDeviceEventArgs> deviceDisconnected = this.DeviceDisconnected;
      if (deviceDisconnected == null)
        return;
      deviceDisconnected((object) this, new UsbDeviceEventArgs(connection));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string GetInterfaceUnlockKey(DeviceInfo device)
    {
      try
      {
        string vid;
        string pid;
        LucidConnectivityHelper.GetVidAndPid(device.InstanceId, out vid, out pid);
        return LucidConnectivityHelper.GetDeviceMode(vid, pid) == ConnectedDeviceMode.Uefi ? device.InstanceId : device.ReadParentInstanceId();
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Could not determine interface unlock key", objArray);
        return string.Empty;
      }
    }
  }
}
