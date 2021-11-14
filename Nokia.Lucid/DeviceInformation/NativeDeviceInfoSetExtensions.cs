// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.NativeDeviceInfoSetExtensions
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Interop;
using Nokia.Lucid.Interop.Win32Types;
using Nokia.Lucid.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Nokia.Lucid.DeviceInformation
{
  internal static class NativeDeviceInfoSetExtensions
  {
    public static string GetDeviceInstanceId(
      this INativeDeviceInfoSet deviceInfoSet,
      ref SP_DEVINFO_DATA deviceData)
    {
      int RequiredSize;
      if (!SetupApiNativeMethods.SetupDiGetDeviceInstanceId(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, IntPtr.Zero, 0, out RequiredSize) && Marshal.GetLastWin32Error() != 122)
        throw new Win32Exception();
      StringBuilder DeviceInstanceId = new StringBuilder(RequiredSize);
      if (!SetupApiNativeMethods.SetupDiGetDeviceInstanceId(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, DeviceInstanceId, RequiredSize, IntPtr.Zero))
        throw new Win32Exception();
      return DeviceInstanceId.ToString();
    }

    public static bool TryGetDeviceProperty(
      this INativeDeviceInfoSet deviceInfoSet,
      ref SP_DEVINFO_DATA deviceData,
      ref PropertyKey propertyKey,
      out int propertyType,
      out byte[] value)
    {
      int RequiredSize;
      if (!SetupApiNativeMethods.SetupDiGetDeviceProperty(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, ref propertyKey, out propertyType, IntPtr.Zero, 0, out RequiredSize, 0) && Marshal.GetLastWin32Error() != 122)
      {
        value = (byte[]) null;
        return false;
      }
      byte[] PropertyBuffer = new byte[RequiredSize];
      if (!SetupApiNativeMethods.SetupDiGetDeviceProperty(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, ref propertyKey, out propertyType, PropertyBuffer, RequiredSize, IntPtr.Zero, 0))
      {
        value = (byte[]) null;
        return false;
      }
      value = PropertyBuffer;
      return true;
    }

    public static PropertyKey[] GetDevicePropertyKeys(
      this INativeDeviceInfoSet deviceInfoSet,
      ref SP_DEVINFO_DATA deviceData)
    {
      int RequiredPropertyKeyCount;
      if (!SetupApiNativeMethods.SetupDiGetDevicePropertyKeys(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, IntPtr.Zero, 0, out RequiredPropertyKeyCount, 0) && Marshal.GetLastWin32Error() != 122)
        throw new Win32Exception();
      PropertyKey[] PropertyKeyArray = new PropertyKey[RequiredPropertyKeyCount];
      if (!SetupApiNativeMethods.SetupDiGetDevicePropertyKeys(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, PropertyKeyArray, RequiredPropertyKeyCount, IntPtr.Zero, 0))
        throw new Win32Exception();
      return PropertyKeyArray;
    }

    public static byte[] GetDeviceProperty(
      this INativeDeviceInfoSet deviceInfoSet,
      ref SP_DEVINFO_DATA deviceData,
      ref PropertyKey propertyKey,
      out int propertyType)
    {
      int RequiredSize;
      if (!SetupApiNativeMethods.SetupDiGetDeviceProperty(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, ref propertyKey, out propertyType, IntPtr.Zero, 0, out RequiredSize, 0) && Marshal.GetLastWin32Error() != 122)
        throw new Win32Exception();
      byte[] PropertyBuffer = new byte[RequiredSize];
      if (!SetupApiNativeMethods.SetupDiGetDeviceProperty(deviceInfoSet.SafeDeviceInfoSetHandle, ref deviceData, ref propertyKey, out propertyType, PropertyBuffer, RequiredSize, IntPtr.Zero, 0))
        throw new Win32Exception();
      return PropertyBuffer;
    }

    public static SP_DEVICE_INTERFACE_DATA GetDeviceInterface(
      this INativeDeviceInfoSet deviceInfoSet,
      string path)
    {
      SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVICE_INTERFACE_DATA))
      };
      if (SetupApiNativeMethods.SetupDiOpenDeviceInterface(deviceInfoSet.SafeDeviceInfoSetHandle, path, 1, ref DeviceInterfaceData))
        return DeviceInterfaceData;
      if (Marshal.GetLastWin32Error() == -536870363)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidOperationException_MessageFormat_CouldNotRetrieveDeviceInfo, (object) path));
      throw new Win32Exception();
    }

    public static void AddDeviceInterfaceClass(
      this INativeDeviceInfoSet deviceInfoSet,
      Guid interfaceClass)
    {
      IntPtr classDevsEx = SetupApiNativeMethods.SetupDiGetClassDevsEx(ref interfaceClass, IntPtr.Zero, IntPtr.Zero, 16, deviceInfoSet.SafeDeviceInfoSetHandle, IntPtr.Zero, IntPtr.Zero);
      if (classDevsEx == IntPtr.Zero || classDevsEx == new IntPtr(-1))
        throw new Win32Exception();
    }

    public static IEnumerable<SP_DEVICE_INTERFACE_DATA> EnumerateDeviceInterfaces(
      this INativeDeviceInfoSet deviceInfoSet,
      Guid interfaceClass)
    {
      SP_DEVICE_INTERFACE_DATA interfaceData = new SP_DEVICE_INTERFACE_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVICE_INTERFACE_DATA))
      };
      for (int index = 0; SetupApiNativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet.SafeDeviceInfoSetHandle, IntPtr.Zero, ref interfaceClass, index, ref interfaceData); ++index)
        yield return interfaceData;
    }

    public static string GetDevicePath(
      this INativeDeviceInfoSet deviceInfoSet,
      ref SP_DEVICE_INTERFACE_DATA interfaceData,
      out SP_DEVINFO_DATA deviceData)
    {
      int RequiredSize;
      if (!SetupApiNativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet.SafeDeviceInfoSetHandle, ref interfaceData, IntPtr.Zero, 0, out RequiredSize, IntPtr.Zero) && Marshal.GetLastWin32Error() != 122)
        throw new Win32Exception();
      IntPtr num = IntPtr.Zero;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          num = Marshal.AllocHGlobal(RequiredSize);
        }
        deviceData = new SP_DEVINFO_DATA()
        {
          cbSize = Marshal.SizeOf(typeof (SP_DEVINFO_DATA))
        };
        Marshal.StructureToPtr((object) new SP_DEVICE_INTERFACE_DETAIL_DATA()
        {
          cbSize = (IntPtr.Size != 8 ? 4 + Marshal.SystemDefaultCharSize : 8)
        }, num, false);
        if (!SetupApiNativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet.SafeDeviceInfoSetHandle, ref interfaceData, num, RequiredSize, IntPtr.Zero, ref deviceData))
          throw new Win32Exception();
        int int32 = Marshal.OffsetOf(typeof (SP_DEVICE_INTERFACE_DETAIL_DATA), "DevicePath").ToInt32();
        return Marshal.PtrToStringAuto(num + int32);
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeHGlobal(num);
      }
    }
  }
}
