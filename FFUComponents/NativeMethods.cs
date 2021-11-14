// Decompiled with JetBrains decompiler
// Type: FFUComponents.NativeMethods
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace FFUComponents
{
  internal static class NativeMethods
  {
    [DllImport("winusb.dll", EntryPoint = "WinUsb_Initialize", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbInitialize(
      SafeFileHandle deviceHandle,
      ref IntPtr interfaceHandle);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_Free", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbFree(IntPtr interfaceHandle);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbControlTransfer(
      IntPtr interfaceHandle,
      WinUsbSetupPacket setupPacket,
      IntPtr buffer,
      uint bufferLength,
      ref uint lengthTransferred,
      IntPtr overlapped);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern unsafe bool WinUsbControlTransfer(
      IntPtr interfaceHandle,
      WinUsbSetupPacket setupPacket,
      byte* buffer,
      uint bufferLength,
      ref uint lengthTransferred,
      IntPtr overlapped);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_QueryInterfaceSettings", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbQueryInterfaceSettings(
      IntPtr interfaceHandle,
      byte alternateInterfaceNumber,
      ref WinUsbInterfaceDescriptor usbAltInterfaceDescriptor);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_QueryPipe", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbQueryPipe(
      IntPtr interfaceHandle,
      byte alternateInterfaceNumber,
      byte pipeIndex,
      ref WinUsbPipeInformation pipeInformation);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbSetPipePolicy(
      IntPtr interfaceHandle,
      byte pipeID,
      uint policyType,
      uint valueLength,
      ref bool value);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbSetPipePolicy(
      IntPtr interfaceHandle,
      byte pipeID,
      uint policyType,
      uint valueLength,
      ref uint value);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_ResetPipe", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbResetPipe(IntPtr interfaceHandle, byte pipeID);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_AbortPipe", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbAbortPipe(IntPtr interfaceHandle, byte pipeID);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_FlushPipe", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WinUsbFlushPipe(IntPtr interfaceHandle, byte pipeID);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern unsafe bool WinUsbReadPipe(
      IntPtr interfaceHandle,
      byte pipeID,
      byte* buffer,
      uint bufferLength,
      IntPtr lenghtTransferred,
      NativeOverlapped* overlapped);

    [DllImport("winusb.dll", EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern unsafe bool WinUsbWritePipe(
      IntPtr interfaceHandle,
      byte pipeID,
      byte* buffer,
      uint bufferLength,
      IntPtr lenghtTransferred,
      NativeOverlapped* overlapped);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr SetupDiGetClassDevs(
      ref Guid classGuid,
      string enumerator,
      int parent,
      int flags);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetupDiEnumDeviceInterfaces(
      IntPtr deviceInfoSet,
      int deviceInfoData,
      ref Guid interfaceClassGuid,
      int memberIndex,
      ref DeviceInterfaceData deviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
      IntPtr deviceInfoSet,
      ref DeviceInterfaceData deviceInterfaceData,
      IntPtr deviceInterfaceDetailData,
      int deviceInterfaceDetailDataSize,
      ref int requiredSize,
      IntPtr deviceInfoData);

    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern unsafe bool SetupDiGetDeviceInterfaceDetail(
      IntPtr deviceInfoSet,
      ref DeviceInterfaceData deviceInterfaceData,
      DeviceInterfaceDetailData* deviceInterfaceDetailData,
      int deviceInterfaceDetailDataSize,
      ref int requiredSize,
      ref DeviceInformationData deviceInfoData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern SafeFileHandle CreateFile(
      string fileName,
      uint desiredAccess,
      uint shareMode,
      IntPtr securityAttributes,
      uint creationDisposition,
      uint flagsAndAttributes,
      IntPtr templateFileHandle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern void CloseHandle(SafeHandle handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CancelIo(SafeFileHandle handle);

    [DllImport("iphlpapi.dll")]
    public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr RegisterDeviceNotification(
      IntPtr hRecipient,
      IntPtr NotificationFilter,
      int Flags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterDeviceNotification(IntPtr hNotification);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
  }
}
