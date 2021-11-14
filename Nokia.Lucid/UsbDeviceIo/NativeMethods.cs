// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.UsbDeviceIo.NativeMethods
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.UsbDeviceIo
{
  internal static class NativeMethods
  {
    internal const byte USB_ENDPOINT_DIRECTION_MASK = 128;
    internal const int FILE_ATTRIBUTE_NORMAL = 128;
    internal const int FILE_FLAG_OVERLAPPED = 1073741824;
    internal const int FILE_SHARE_READ = 1;
    internal const int FILE_SHARE_WRITE = 2;
    internal const uint GENERIC_READ = 2147483648;
    internal const uint GENERIC_WRITE = 1073741824;
    internal const int INVALID_HANDLE_VALUE = -1;
    internal const int OPEN_EXISTING = 3;
    internal const int ERROR_IO_PENDING = 997;

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_Free(IntPtr interfaceHandle);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_Initialize(
      SafeFileHandle deviceHandle,
      out IntPtr interfaceHandle);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_QueryInterfaceSettings(
      IntPtr interfaceHandle,
      byte alternateInterfaceNumber,
      out NativeMethods.USB_INTERFACE_DESCRIPTOR usbAlternateInterfaceDescriptor);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_QueryPipe(
      IntPtr interfaceHandle,
      byte alternateInterfaceNumber,
      byte pipeIndex,
      out NativeMethods.WINUSB_PIPE_INFORMATION pipeInformation);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_SetPipePolicy(
      IntPtr interfaceHandle,
      byte pipeId,
      uint policyType,
      uint valueLength,
      ref uint value);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_FlushPipe(IntPtr interfaceHandle, byte pipeId);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_AbortPipe(IntPtr interfaceHandle, byte pipeId);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_ReadPipe(
      IntPtr interfaceHandle,
      byte pipeId,
      byte[] buffer,
      uint bufferLength,
      ref uint lengthTransferred,
      ref NativeMethods.OVERLAPPED overlapped);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_WritePipe(
      IntPtr interfaceHandle,
      byte pipeId,
      byte[] buffer,
      uint bufferLength,
      ref uint lengthTransferred,
      ref NativeMethods.OVERLAPPED overlapped);

    [DllImport("winusb.dll", SetLastError = true)]
    internal static extern bool WinUsb_GetOverlappedResult(
      IntPtr interfaceHandle,
      ref NativeMethods.OVERLAPPED overlapped,
      ref uint numberOfBytesTransferred,
      bool wait);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern SafeFileHandle CreateFile(
      string fileName,
      uint desiredAccess,
      int shareMode,
      IntPtr securityAttributes,
      int creationDisposition,
      int flagsAndAttributes,
      IntPtr templateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CancelIoEx(IntPtr hFile, IntPtr overlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CancelIoEx(IntPtr hFile, ref NativeMethods.OVERLAPPED overlapped);

    internal enum POLICY_TYPE
    {
      SHORT_PACKET_TERMINATE = 1,
      AUTO_CLEAR_STALL = 2,
      PIPE_TRANSFER_TIMEOUT = 3,
      IGNORE_SHORT_PACKETS = 4,
      ALLOW_PARTIAL_READS = 5,
      AUTO_FLUSH = 6,
      RAW_IO = 7,
    }

    internal enum USBD_PIPE_TYPE
    {
      UsbdPipeTypeControl,
      UsbdPipeTypeIsochronous,
      UsbdPipeTypeBulk,
      UsbdPipeTypeInterrupt,
    }

    internal struct USB_INTERFACE_DESCRIPTOR
    {
      internal byte bLength;
      internal byte bDescriptorType;
      internal byte bInterfaceNumber;
      internal byte bAlternateSetting;
      internal byte bNumEndpoints;
      internal byte bInterfaceClass;
      internal byte bInterfaceSubClass;
      internal byte bInterfaceProtocol;
      internal byte iInterface;
    }

    internal struct WINUSB_PIPE_INFORMATION
    {
      internal NativeMethods.USBD_PIPE_TYPE PipeType;
      internal byte PipeId;
      internal ushort MaximumPacketSize;
      internal byte Interval;
    }

    internal struct OVERLAPPED
    {
      internal IntPtr InternalLow;
      internal IntPtr InternalHigh;
      internal int OffsetLow;
      internal int OffsetHigh;
      internal IntPtr EventHandle;
    }
  }
}
