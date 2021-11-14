// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.UsbDeviceIo.WinUsbIo
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Microsoft.Win32.SafeHandles;
using Nokia.Lucid.Diagnostics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Nokia.Lucid.UsbDeviceIo
{
  public class WinUsbIo : IDisposable
  {
    private readonly ManualResetEvent writeEvent = new ManualResetEvent(false);
    private readonly ManualResetEvent readEvent = new ManualResetEvent(false);
    private readonly object senderLock = new object();
    private SafeFileHandle deviceHandle;
    private IntPtr winUsbHandle;
    private NativeMethods.USB_INTERFACE_DESCRIPTOR interfaceDescriptor;
    private byte bulkInPipe;
    private byte bulkOutPipe;
    private bool disposed;

    public WinUsbIo(string devicePath)
    {
      using (EntryExitLogger.Log("WinUsbIo.WinUsbIo(string devicePath)", (TraceSource) UsbDeviceIoTraceSource.Instance))
        this.DevicePath = devicePath;
    }

    ~WinUsbIo()
    {
      using (EntryExitLogger.Log("WinUsbIo.~WinUsbIo()", (TraceSource) UsbDeviceIoTraceSource.Instance))
        this.Dispose(false);
    }

    public string DevicePath { get; private set; }

    public uint Write(byte[] data, uint length)
    {
      using (EntryExitLogger.Log("WinUsbIo.Write(byte[] data, uint length)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        lock (this.senderLock)
        {
          this.CheckIfDisposed();
          NativeMethods.OVERLAPPED overlapped = new NativeMethods.OVERLAPPED();
          this.writeEvent.Reset();
          overlapped.EventHandle = this.writeEvent.SafeWaitHandle.DangerousGetHandle();
          GCHandle gcHandle1 = GCHandle.Alloc((object) overlapped, GCHandleType.Pinned);
          GCHandle gcHandle2 = GCHandle.Alloc((object) data, GCHandleType.Pinned);
          uint num = 0;
          try
          {
            if (!NativeMethods.WinUsb_WritePipe(this.winUsbHandle, this.bulkOutPipe, data, length, ref num, ref overlapped))
            {
              if (Marshal.GetLastWin32Error() != 997)
                throw new Win32Exception();
              if (!this.writeEvent.WaitOne(15000))
              {
                if (!NativeMethods.CancelIoEx(this.deviceHandle.DangerousGetHandle(), ref overlapped))
                  RobustTrace.Trace<Win32Exception>(new Action<Win32Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), new Win32Exception());
                throw new TimeoutException("send operation timed out");
              }
              WinUsbIo.CheckError(NativeMethods.WinUsb_GetOverlappedResult(this.winUsbHandle, ref overlapped, ref num, false), "Get write overlapped result");
            }
            return num;
          }
          finally
          {
            gcHandle1.Free();
            gcHandle2.Free();
          }
        }
      }
    }

    public uint Read(byte[] data, uint length)
    {
      using (EntryExitLogger.Log("WinUsbIo.Read(ref byte[] data, uint length)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.CheckIfDisposed();
        NativeMethods.OVERLAPPED overlapped = new NativeMethods.OVERLAPPED();
        this.readEvent.Reset();
        overlapped.EventHandle = this.readEvent.SafeWaitHandle.DangerousGetHandle();
        GCHandle gcHandle1 = GCHandle.Alloc((object) overlapped, GCHandleType.Pinned);
        GCHandle gcHandle2 = GCHandle.Alloc((object) data, GCHandleType.Pinned);
        try
        {
          uint num = 0;
          if (!NativeMethods.WinUsb_ReadPipe(this.winUsbHandle, this.bulkInPipe, data, length, ref num, ref overlapped))
          {
            if (Marshal.GetLastWin32Error() != 997)
              throw new Win32Exception();
            WinUsbIo.CheckError(NativeMethods.WinUsb_GetOverlappedResult(this.winUsbHandle, ref overlapped, ref num, true), "Get read overlapped result");
          }
          return num;
        }
        finally
        {
          gcHandle1.Free();
          gcHandle2.Free();
        }
      }
    }

    public void Open()
    {
      using (EntryExitLogger.Log("WinUsbIo.Open()", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.CheckIfDisposed();
        this.deviceHandle = this.DevicePath.Length != 0 ? NativeMethods.CreateFile(this.DevicePath, 3221225472U, 0, IntPtr.Zero, 3, 1073741952, IntPtr.Zero) : throw new Exception("Device Path length == 0");
        if (this.deviceHandle.IsInvalid)
        {
          Win32Exception win32Exception = new Win32Exception();
          string message = win32Exception.Message;
          throw new Win32Exception(win32Exception.NativeErrorCode, "CreateFile failed to create valid handle to device. " + message);
        }
        WinUsbIo.CheckError(NativeMethods.WinUsb_Initialize(this.deviceHandle, out this.winUsbHandle), "Initializing WinUSB with device handle.");
        WinUsbIo.CheckError(NativeMethods.WinUsb_QueryInterfaceSettings(this.winUsbHandle, (byte) 0, out this.interfaceDescriptor), "Querying Interface Settings");
        for (int index = 0; index < (int) this.interfaceDescriptor.bNumEndpoints; ++index)
        {
          NativeMethods.WINUSB_PIPE_INFORMATION pipeInformation;
          WinUsbIo.CheckError(NativeMethods.WinUsb_QueryPipe(this.winUsbHandle, (byte) 0, Convert.ToByte(index), out pipeInformation), "Querying Pipe Information.");
          if (pipeInformation.PipeType == NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeBulk && ((int) pipeInformation.PipeId & 128) == 128)
            this.bulkInPipe = pipeInformation.PipeId;
          else if (pipeInformation.PipeType == NativeMethods.USBD_PIPE_TYPE.UsbdPipeTypeBulk && ((int) pipeInformation.PipeId & 128) == 0)
            this.bulkOutPipe = pipeInformation.PipeId;
        }
      }
    }

    public void CancelIo()
    {
      using (EntryExitLogger.Log("WinUsbIo.CancelIo()", (TraceSource) UsbDeviceIoTraceSource.Instance))
        WinUsbIo.CheckError(NativeMethods.CancelIoEx(this.deviceHandle.DangerousGetHandle(), IntPtr.Zero), "Cancel IO:");
    }

    public void Close()
    {
      using (EntryExitLogger.Log("WinUsbIo.Close()", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
    }

    public void Dispose()
    {
      using (EntryExitLogger.Log("WinUsbIo.Dispose()", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
    }

    public void SetPipePolicy(
      WinUsbIo.PIPE_TYPE pipeType,
      WinUsbIo.POLICY_TYPE policyType,
      uint value)
    {
      using (EntryExitLogger.Log("WinUsbIo.SetPipePolicy(PIPE_TYPE pipeType, POLICY_TYPE policyType, dynamic value)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.CheckIfDisposed();
        if (pipeType == WinUsbIo.PIPE_TYPE.PipeTypeBulkIn)
        {
          WinUsbIo.CheckError(NativeMethods.WinUsb_SetPipePolicy(this.winUsbHandle, this.bulkInPipe, (uint) policyType, (uint) Marshal.SizeOf((object) value), ref value), "Setting Bulk In Pipe Policy");
        }
        else
        {
          if (pipeType != WinUsbIo.PIPE_TYPE.PipeTypeBulkOut)
            return;
          WinUsbIo.CheckError(NativeMethods.WinUsb_SetPipePolicy(this.winUsbHandle, this.bulkOutPipe, (uint) policyType, (uint) Marshal.SizeOf((object) value), ref value), "Setting Bulk Out Pipe Policy");
        }
      }
    }

    public void Flush()
    {
      using (EntryExitLogger.Log("WinUsbIo.Flush()", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.CheckIfDisposed();
        WinUsbIo.CheckError(NativeMethods.WinUsb_FlushPipe(this.winUsbHandle, this.bulkInPipe), "Flushing Bulk In Pipe");
      }
    }

    internal static void CheckError(bool expression, string message)
    {
      if (!expression)
      {
        Win32Exception win32Exception = new Win32Exception();
        string message1 = win32Exception.Message;
        throw new Win32Exception(win32Exception.NativeErrorCode, message + " " + message1);
      }
    }

    private void Dispose(bool disposing)
    {
      using (EntryExitLogger.Log("WinUsbIo.Dispose(bool disposing)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        if (this.disposed)
          return;
        if (disposing)
        {
          if (this.deviceHandle != null && !this.deviceHandle.IsInvalid)
          {
            this.deviceHandle.Dispose();
            this.deviceHandle = (SafeFileHandle) null;
          }
          if (this.readEvent != null)
            this.readEvent.Dispose();
          if (this.writeEvent != null)
            this.writeEvent.Dispose();
        }
        if (this.winUsbHandle != IntPtr.Zero)
        {
          WinUsbIo.CheckError(NativeMethods.WinUsb_Free(this.winUsbHandle), "Freeing WinUsb resources.");
          this.winUsbHandle = IntPtr.Zero;
        }
        this.disposed = true;
      }
    }

    private void CheckIfDisposed()
    {
      using (EntryExitLogger.Log("WinUsbIo.CheckIfDisposed()", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        if (this.disposed)
          throw new ObjectDisposedException("WinUsbIo object has been disposed.");
      }
    }

    public enum PIPE_TYPE
    {
      PipeTypeBulkIn,
      PipeTypeBulkOut,
    }

    public enum POLICY_TYPE
    {
      SHORT_PACKET_TERMINATE = 1,
      AUTO_CLEAR_STALL = 2,
      PIPE_TRANSFER_TIMEOUT = 3,
      IGNORE_SHORT_PACKETS = 4,
      ALLOW_PARTIAL_READS = 5,
      AUTO_FLUSH = 6,
      RAW_IO = 7,
    }
  }
}
