// Decompiled with JetBrains decompiler
// Type: FFUComponents.DTSFUsbStream
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace FFUComponents
{
  internal class DTSFUsbStream : Stream
  {
    private const byte UsbEndpointDirectionMask = 128;
    private const int retryCount = 10;
    private string deviceName;
    private SafeFileHandle deviceHandle;
    private IntPtr usbHandle;
    private byte bulkInPipeId;
    private byte bulkOutPipeId;
    private bool isDisposed;
    private TimeSpan completionTimeout = TimeSpan.FromSeconds(5.0);

    public DTSFUsbStream(string deviceName, FileShare shareMode, TimeSpan transferTimeout)
    {
      if (string.IsNullOrEmpty(deviceName))
        throw new ArgumentException("Invalid Argument", nameof (deviceName));
      this.isDisposed = false;
      this.deviceName = deviceName;
      try
      {
        this.deviceHandle = DTSFUsbStream.CreateDeviceHandle(this.deviceName, shareMode);
        if (this.deviceHandle.IsInvalid)
          throw new IOException(string.Format("Handle for {0} is invalid, last error: 0x{1:x}", (object) deviceName, (object) Marshal.GetLastWin32Error()));
        this.InitializeDevice();
        this.SetTransferTimeout(transferTimeout);
        if (!ThreadPool.BindHandle((SafeHandle) this.deviceHandle))
          throw new IOException(string.Format("BindHandle on device {0} failed.", (object) deviceName));
        this.Connect();
      }
      catch (Exception ex)
      {
        this.CloseDeviceHandle();
        throw;
      }
    }

    public DTSFUsbStream(string deviceName, TimeSpan transferTimeout)
      : this(deviceName, FileShare.None, transferTimeout)
    {
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Flush()
    {
    }

    private void HandleAsyncTimeout(IAsyncResult asyncResult)
    {
      if (!NativeMethods.CancelIo(this.deviceHandle))
        return;
      asyncResult.AsyncWaitHandle.WaitOne(this.completionTimeout, false);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      IAsyncResult asyncResult = this.BeginRead(buffer, offset, count, (AsyncCallback) null, (object) null);
      try
      {
        return this.EndRead(asyncResult);
      }
      catch (TimeoutException ex)
      {
        this.HandleAsyncTimeout(asyncResult);
        throw new Win32Exception("Timeout waiting for completion callback.", (Exception) ex);
      }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      IAsyncResult asyncResult = this.BeginWrite(buffer, offset, count, (AsyncCallback) null, (object) null);
      try
      {
        this.EndWrite(asyncResult);
      }
      catch (TimeoutException ex)
      {
        this.HandleAsyncTimeout(asyncResult);
        throw new Win32Exception("Timeout waiting for completion callback.", (Exception) ex);
      }
    }

    private void RetryRead(
      uint errorCode,
      DTSFUsbStreamReadAsyncResult asyncResult,
      out Exception exception)
    {
      exception = (Exception) null;
      if (this.IsDeviceDisconnected(errorCode))
        exception = (Exception) new Win32Exception((int) errorCode);
      else if (asyncResult.RetryCount > 10)
      {
        exception = (Exception) new Win32Exception((int) errorCode);
      }
      else
      {
        int errorCode1 = 0;
        this.ClearPipeStall(this.bulkInPipeId, out errorCode1);
        if (errorCode1 != 0)
        {
          exception = (Exception) new Win32Exception(errorCode1);
        }
        else
        {
          try
          {
            this.BeginReadInternal(asyncResult.Buffer, asyncResult.Offset, asyncResult.Count, asyncResult.RetryCount++, asyncResult.AsyncCallback, asyncResult.AsyncState);
          }
          catch (Exception ex)
          {
            exception = ex;
          }
        }
      }
    }

    private unsafe void ReadIOCompletionCallback(
      uint errorCode,
      uint numBytes,
      NativeOverlapped* nativeOverlapped)
    {
      try
      {
        DTSFUsbStreamReadAsyncResult asyncResult = (DTSFUsbStreamReadAsyncResult) Overlapped.Unpack(nativeOverlapped).AsyncResult;
        Overlapped.Free(nativeOverlapped);
        Exception exception = (Exception) null;
        if (errorCode != 0U)
        {
          this.RetryRead(errorCode, asyncResult, out exception);
          if (exception == null)
            return;
          asyncResult.SetAsCompleted(exception, false);
        }
        else
          asyncResult.SetAsCompleted((int) numBytes, false);
      }
      catch (Exception ex)
      {
      }
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback userCallback,
      object stateObject)
    {
      if (this.deviceHandle.IsClosed)
        throw new ObjectDisposedException("File closed");
      if (buffer == null)
        throw new ArgumentNullException("array");
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset), "ArgumentOutOfRange_NeedNonNegNum");
      if (count < 0)
        throw new ArgumentOutOfRangeException("numBytes", "ArgumentOutOfRange_NeedNonNegNum");
      if (buffer.Length - offset < count)
        throw new ArgumentException("Argument_InvalidOffLen");
      return this.BeginReadInternal(buffer, offset, count, 10, userCallback, stateObject);
    }

    private unsafe IAsyncResult BeginReadInternal(
      byte[] buffer,
      int offset,
      int count,
      int retryCount,
      AsyncCallback userCallback,
      object stateObject)
    {
      DTSFUsbStreamReadAsyncResult streamReadAsyncResult = new DTSFUsbStreamReadAsyncResult(userCallback, stateObject)
      {
        Buffer = buffer,
        Offset = offset,
        Count = count,
        RetryCount = retryCount
      };
      NativeOverlapped* nativeOverlappedPtr = new Overlapped(0, 0, IntPtr.Zero, (IAsyncResult) streamReadAsyncResult).Pack(new IOCompletionCallback(this.ReadIOCompletionCallback), (object) buffer);
      fixed (byte* numPtr = buffer)
      {
        if (!NativeMethods.WinUsbReadPipe(this.usbHandle, this.bulkInPipeId, numPtr + offset, (uint) count, IntPtr.Zero, nativeOverlappedPtr))
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          if (997 != lastWin32Error)
          {
            Overlapped.Unpack(nativeOverlappedPtr);
            Overlapped.Free(nativeOverlappedPtr);
            throw new Win32Exception(lastWin32Error);
          }
        }
      }
      return (IAsyncResult) streamReadAsyncResult;
    }

    public override int EndRead(IAsyncResult asyncResult) => ((AsyncResult<int>) asyncResult).EndInvoke();

    private void RetryWrite(
      uint errorCode,
      DTSFUsbStreamWriteAsyncResult asyncResult,
      out Exception exception)
    {
      exception = (Exception) null;
      if (this.IsDeviceDisconnected(errorCode))
        exception = (Exception) new Win32Exception((int) errorCode);
      else if (asyncResult.RetryCount > 10)
      {
        exception = (Exception) new Win32Exception((int) errorCode);
      }
      else
      {
        int errorCode1 = 0;
        this.ClearPipeStall(this.bulkOutPipeId, out errorCode1);
        if (errorCode1 != 0)
        {
          exception = (Exception) new Win32Exception(errorCode1);
        }
        else
        {
          try
          {
            this.BeginWriteInternal(asyncResult.Buffer, asyncResult.Offset, asyncResult.Count, asyncResult.RetryCount++, asyncResult.AsyncCallback, asyncResult.AsyncState);
          }
          catch (Exception ex)
          {
            exception = ex;
          }
        }
      }
    }

    private unsafe void WriteIOCompletionCallback(
      uint errorCode,
      uint numBytes,
      NativeOverlapped* nativeOverlapped)
    {
      DTSFUsbStreamWriteAsyncResult asyncResult = (DTSFUsbStreamWriteAsyncResult) Overlapped.Unpack(nativeOverlapped).AsyncResult;
      Overlapped.Free(nativeOverlapped);
      Exception exception = (Exception) null;
      try
      {
        if (errorCode != 0U)
        {
          this.RetryWrite(errorCode, asyncResult, out exception);
          if (exception == null)
            return;
          asyncResult.SetAsCompleted(exception, false);
        }
        else
          asyncResult.SetAsCompleted(exception, false);
      }
      catch (Exception ex)
      {
      }
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback userCallback,
      object stateObject)
    {
      if (this.deviceHandle.IsClosed)
        throw new ObjectDisposedException("File closed");
      if (buffer == null)
        throw new ArgumentNullException("array");
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset), "ArgumentOutOfRange_NeedNonNegNum");
      if (count < 0)
        throw new ArgumentOutOfRangeException("numBytes", "ArgumentOutOfRange_NeedNonNegNum");
      if (buffer.Length - offset < count)
        throw new ArgumentException("Argument_InvalidOffLen");
      return this.BeginWriteInternal(buffer, offset, count, 0, userCallback, stateObject);
    }

    private unsafe IAsyncResult BeginWriteInternal(
      byte[] buffer,
      int offset,
      int count,
      int retryCount,
      AsyncCallback userCallback,
      object stateObject)
    {
      DTSFUsbStreamWriteAsyncResult writeAsyncResult = new DTSFUsbStreamWriteAsyncResult(userCallback, stateObject)
      {
        Buffer = buffer,
        Offset = offset,
        Count = count,
        RetryCount = retryCount
      };
      NativeOverlapped* nativeOverlappedPtr = new Overlapped(0, 0, IntPtr.Zero, (IAsyncResult) writeAsyncResult).Pack(new IOCompletionCallback(this.WriteIOCompletionCallback), (object) buffer);
      fixed (byte* numPtr = buffer)
      {
        if (!NativeMethods.WinUsbWritePipe(this.usbHandle, this.bulkOutPipeId, numPtr + offset, (uint) count, IntPtr.Zero, nativeOverlappedPtr))
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          if (997 != lastWin32Error)
          {
            Overlapped.Unpack(nativeOverlappedPtr);
            Overlapped.Free(nativeOverlappedPtr);
            throw new Win32Exception(lastWin32Error);
          }
        }
      }
      return (IAsyncResult) writeAsyncResult;
    }

    public override void EndWrite(IAsyncResult asyncResult) => ((AsyncResultNoResult) asyncResult).EndInvoke();

    private static SafeFileHandle CreateDeviceHandle(
      string deviceName,
      FileShare shareMode)
    {
      return NativeMethods.CreateFile(deviceName, 3221225472U, (uint) shareMode, IntPtr.Zero, 3U, 1073741952U, IntPtr.Zero);
    }

    private void CloseDeviceHandle()
    {
      if (IntPtr.Zero != this.usbHandle)
      {
        NativeMethods.WinUsbFree(this.usbHandle);
        this.usbHandle = IntPtr.Zero;
      }
      if (this.deviceHandle.IsInvalid || this.deviceHandle.IsClosed)
        return;
      this.deviceHandle.Close();
      this.deviceHandle.SetHandleAsInvalid();
    }

    private void InitializeDevice()
    {
      WinUsbInterfaceDescriptor usbAltInterfaceDescriptor = new WinUsbInterfaceDescriptor();
      WinUsbPipeInformation pipeInformation = new WinUsbPipeInformation();
      if (!NativeMethods.WinUsbInitialize(this.deviceHandle, ref this.usbHandle))
        throw new IOException("WinUsb Initialization failed.");
      if (!NativeMethods.WinUsbQueryInterfaceSettings(this.usbHandle, (byte) 0, ref usbAltInterfaceDescriptor))
        throw new IOException("WinUsb Query Interface Settings failed.");
      for (byte pipeIndex = 0; (int) pipeIndex < (int) usbAltInterfaceDescriptor.NumEndpoints; ++pipeIndex)
      {
        if (!NativeMethods.WinUsbQueryPipe(this.usbHandle, (byte) 0, pipeIndex, ref pipeInformation))
          throw new IOException(string.Format("WinUsb Query Pipe Information failed"));
        if (pipeInformation.PipeType == WinUsbPipeType.Bulk)
        {
          if (this.IsBulkInEndpoint(pipeInformation.PipeId))
          {
            this.SetupBulkInEndpoint(pipeInformation.PipeId);
          }
          else
          {
            if (!this.IsBulkOutEndpoint(pipeInformation.PipeId))
              throw new IOException("Invalid Endpoint Type.");
            this.SetupBulkOutEndpoint(pipeInformation.PipeId);
          }
        }
      }
    }

    private bool IsBulkInEndpoint(byte pipeId) => ((int) pipeId & 128) == 128;

    private bool IsBulkOutEndpoint(byte pipeId) => ((int) pipeId & 128) == 0;

    public void PipeReset()
    {
      int errorCode;
      this.ClearPipeStall(this.bulkInPipeId, out errorCode);
      this.ClearPipeStall(this.bulkOutPipeId, out errorCode);
    }

    public void SetTransferTimeout(TimeSpan transferTimeout)
    {
      uint totalMilliseconds = (uint) transferTimeout.TotalMilliseconds;
      this.SetPipePolicy(this.bulkInPipeId, 3U, totalMilliseconds);
      this.SetPipePolicy(this.bulkOutPipeId, 3U, totalMilliseconds);
    }

    public void SetShortPacketTerminate() => this.SetPipePolicy(this.bulkOutPipeId, 1U, true);

    private void SetupBulkInEndpoint(byte pipeId) => this.bulkInPipeId = pipeId;

    private void SetupBulkOutEndpoint(byte pipeId) => this.bulkOutPipeId = pipeId;

    private void SetPipePolicy(byte pipeId, uint policyType, uint value)
    {
      if (!NativeMethods.WinUsbSetPipePolicy(this.usbHandle, pipeId, policyType, (uint) Marshal.SizeOf(typeof (uint)), ref value))
        throw new IOException("WinUsb SetPipe Policy failed.");
    }

    private void SetPipePolicy(byte pipeId, uint policyType, bool value)
    {
      if (!NativeMethods.WinUsbSetPipePolicy(this.usbHandle, pipeId, policyType, (uint) Marshal.SizeOf(typeof (bool)), ref value))
        throw new IOException("WinUsb SetPipe Policy failed.");
    }

    private unsafe void ControlTransferSetData(UsbControlRequest request, ushort value)
    {
      WinUsbSetupPacket setupPacket = new WinUsbSetupPacket();
      setupPacket.RequestType = (byte) 33;
      setupPacket.Request = (byte) request;
      setupPacket.Value = value;
      setupPacket.Index = (ushort) 0;
      setupPacket.Length = (ushort) 0;
      uint lengthTransferred = 0;
      fixed (byte* buffer = (byte[]) null)
      {
        if (!NativeMethods.WinUsbControlTransfer(this.usbHandle, setupPacket, buffer, (uint) setupPacket.Length, ref lengthTransferred, IntPtr.Zero))
          throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }

    private unsafe void ControlTransferGetData(UsbControlRequest request, byte[] buffer)
    {
      WinUsbSetupPacket setupPacket = new WinUsbSetupPacket();
      setupPacket.RequestType = (byte) 161;
      setupPacket.Request = (byte) request;
      setupPacket.Value = (ushort) 0;
      setupPacket.Index = (ushort) 0;
      setupPacket.Length = buffer == null ? (ushort) 0 : (ushort) buffer.Length;
      uint lengthTransferred = 0;
      fixed (byte* buffer1 = buffer)
      {
        if (!NativeMethods.WinUsbControlTransfer(this.usbHandle, setupPacket, buffer1, (uint) setupPacket.Length, ref lengthTransferred, IntPtr.Zero))
          throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }

    private void ClearPipeStall(byte pipeId, out int errorCode)
    {
      errorCode = 0;
      if (!NativeMethods.WinUsbAbortPipe(this.usbHandle, pipeId))
        errorCode = Marshal.GetLastWin32Error();
      if (NativeMethods.WinUsbResetPipe(this.usbHandle, pipeId))
        return;
      errorCode = Marshal.GetLastWin32Error();
    }

    private bool IsDeviceDisconnected(uint errorCode) => errorCode == 2U || errorCode == 1167U || (errorCode == 31U || errorCode == 121U) || errorCode == 995U;

    protected override void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (!disposing)
        return;
      try
      {
        this.CloseDeviceHandle();
      }
      catch (Exception ex)
      {
      }
      finally
      {
        base.Dispose(disposing);
        this.isDisposed = true;
      }
    }

    ~DTSFUsbStream() => this.Dispose(false);

    private void Connect() => this.ControlTransferSetData(UsbControlRequest.LineStateSet, (ushort) 1);
  }
}
