// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.SetupApiNativeMethods
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.DeviceInformation;
using Nokia.Lucid.Interop.SafeHandles;
using Nokia.Lucid.Interop.Win32Types;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace Nokia.Lucid.Interop
{
  internal static class SetupApiNativeMethods
  {
    private const string SetupApiDllName = "setupapi.dll";

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr SetupDiGetClassDevsEx(
      ref Guid ClassGuid,
      IntPtr Enumerator,
      IntPtr hwndParent,
      int Flags,
      SafeDeviceInfoSetHandle DeviceInfoSet,
      IntPtr MachineName,
      IntPtr Reserved);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern SafeDeviceInfoSetHandle SetupDiCreateDeviceInfoListEx(
      IntPtr ClassGuid,
      IntPtr hwndParent,
      IntPtr MachineName,
      IntPtr Reserved);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("setupapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiOpenDeviceInterface(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      string DevicePath,
      int OpenFlags,
      ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDeviceInterfaceDetail(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
      IntPtr DeviceInterfaceDetailData,
      int DeviceInterfaceDetailDataSize,
      IntPtr RequiredSize,
      ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDeviceInterfaceDetail(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
      IntPtr DeviceInterfaceDetailData,
      int DeviceInterfaceDetailDataSize,
      out int RequiredSize,
      IntPtr DeviceInfoData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDeviceInstanceId(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      IntPtr DeviceInstanceId,
      int DeviceInstanceIdSize,
      out int RequiredSize);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDeviceInstanceId(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      StringBuilder DeviceInstanceId,
      int DeviceInstanceIdSize,
      IntPtr RequiredSize);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiEnumDeviceInterfaces(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      IntPtr DeviceInfoData,
      ref Guid InterfaceClassGuid,
      int MemberIndex,
      ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDeviceProperty(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      ref PropertyKey PropertyKey,
      out int PropertyType,
      byte[] PropertyBuffer,
      int PropertyBufferSize,
      IntPtr RequiredSize,
      int Flags);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDeviceProperty(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      ref PropertyKey PropertyKey,
      out int PropertyType,
      IntPtr PropertyBuffer,
      int PropertyBufferSize,
      out int RequiredSize,
      int Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDevicePropertyKeys(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      [Out] PropertyKey[] PropertyKeyArray,
      int PropertyKeyCount,
      IntPtr RequiredPropertyKeyCount,
      int Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetupDiGetDevicePropertyKeys(
      SafeDeviceInfoSetHandle DeviceInfoSet,
      ref SP_DEVINFO_DATA DeviceInfoData,
      IntPtr PropertyKeyArray,
      int PropertyKeyCount,
      out int RequiredPropertyKeyCount,
      int Flags);
  }
}
