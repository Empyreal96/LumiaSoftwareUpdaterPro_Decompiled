// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.DeviceInfoSet
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Interop;
using Nokia.Lucid.Interop.SafeHandles;
using Nokia.Lucid.Interop.Win32Types;
using Nokia.Lucid.Primitives;
using Nokia.Lucid.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.DeviceInformation
{
  public sealed class DeviceInfoSet
  {
    private volatile bool initialized;
    private Func<DeviceIdentifier, bool> compiledFilter;
    private DeviceTypeMap cachedTypeMap;

    public DeviceInfoSet()
    {
      this.Filter = FilterExpression.DefaultExpression;
      this.DeviceTypeMap = DeviceTypeMap.DefaultMap;
    }

    public Expression<Func<DeviceIdentifier, bool>> Filter { get; set; }

    public DeviceTypeMap DeviceTypeMap { get; set; }

    public DeviceInfo GetDevice(string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      this.EnsureInitialized();
      DeviceIdentifier result;
      if (!DeviceIdentifier.TryParse(path, out result) || !this.compiledFilter(result))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidOperationException_MessageFormat_CouldNotRetrieveDeviceInfo, (object) path));
      SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVICE_INTERFACE_DATA))
      };
      DeviceInfoSet.NativeDeviceInfoSet deviceInfoSet = new DeviceInfoSet.NativeDeviceInfoSet();
      if (!SetupApiNativeMethods.SetupDiOpenDeviceInterface(deviceInfoSet.SafeDeviceInfoSetHandle, path, 0, ref DeviceInterfaceData))
      {
        if (Marshal.GetLastWin32Error() == -536870363)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidOperationException_MessageFormat_CouldNotRetrieveDeviceInfo, (object) path));
        throw new Win32Exception();
      }
      SP_DEVINFO_DATA spDevinfoData = new SP_DEVINFO_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVINFO_DATA))
      };
      if (!SetupApiNativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet.SafeDeviceInfoSetHandle, ref DeviceInterfaceData, IntPtr.Zero, 0, IntPtr.Zero, ref spDevinfoData) && Marshal.GetLastWin32Error() != 122)
        throw new Win32Exception();
      DeviceType mapping = this.cachedTypeMap.GetMapping(DeviceInterfaceData.InterfaceClassGuid);
      string deviceInstanceId = deviceInfoSet.GetDeviceInstanceId(ref spDevinfoData);
      return new DeviceInfo((INativeDeviceInfoSet) deviceInfoSet, path, spDevinfoData.DevInst, deviceInstanceId, spDevinfoData.ClassGuid, mapping, DeviceInterfaceData, spDevinfoData.Reserved);
    }

    public IEnumerable<DeviceInfo> EnumerateDevices()
    {
      this.EnsureInitialized();
      DeviceInfoSet.NativeDeviceInfoSet deviceInfoSet = new DeviceInfoSet.NativeDeviceInfoSet();
      foreach (Guid interfaceClass in this.cachedTypeMap.InterfaceClasses)
      {
        deviceInfoSet.AddDeviceInterfaceClass(interfaceClass);
        foreach (SP_DEVICE_INTERFACE_DATA enumerateDeviceInterface in deviceInfoSet.EnumerateDeviceInterfaces(interfaceClass))
        {
          SP_DEVICE_INTERFACE_DATA temp = enumerateDeviceInterface;
          SP_DEVINFO_DATA deviceData;
          string devicePath = deviceInfoSet.GetDevicePath(ref temp, out deviceData);
          DeviceIdentifier p;
          if (DeviceIdentifier.TryParse(devicePath, out p) && this.compiledFilter(p))
          {
            string deviceInstanceId = deviceInfoSet.GetDeviceInstanceId(ref deviceData);
            DeviceType deviceType = this.cachedTypeMap.GetMapping(interfaceClass);
            DeviceInfo device = new DeviceInfo((INativeDeviceInfoSet) deviceInfoSet, devicePath, deviceData.DevInst, deviceInstanceId, deviceData.ClassGuid, deviceType, enumerateDeviceInterface, deviceData.Reserved);
            yield return device;
          }
        }
      }
    }

    public IEnumerable<DeviceInfo> EnumeratePresentDevices() => this.EnumerateDevices().Where<DeviceInfo>((Func<DeviceInfo, bool>) (d => d.IsPresent));

    private void EnsureInitialized()
    {
      if (this.initialized)
        return;
      this.cachedTypeMap = this.DeviceTypeMap;
      this.compiledFilter = (this.Filter ?? FilterExpression.EmptyExpression).Compile();
      this.initialized = true;
    }

    private sealed class NativeDeviceInfoSet : IDisposable, INativeDeviceInfoSet
    {
      private readonly SafeDeviceInfoSetHandle handle;

      public NativeDeviceInfoSet()
      {
        this.handle = SetupApiNativeMethods.SetupDiCreateDeviceInfoListEx(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (this.handle == null || this.handle.IsInvalid)
          throw new Win32Exception();
      }

      public SafeDeviceInfoSetHandle SafeDeviceInfoSetHandle => this.handle;

      public void Dispose() => this.handle.Dispose();
    }
  }
}
