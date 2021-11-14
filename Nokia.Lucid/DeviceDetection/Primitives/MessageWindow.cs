// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.Primitives.MessageWindow
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Diagnostics;
using Nokia.Lucid.Interop;
using Nokia.Lucid.Interop.Win32Types;
using Nokia.Lucid.Primitives;
using Nokia.Lucid.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.DeviceDetection.Primitives
{
  internal sealed class MessageWindow : IDisposable
  {
    private readonly object syncRoot = new object();
    private readonly IHandleDeviceChanged deviceChangeHandler;
    private readonly IHandleThreadException threadExceptionHandler;
    private readonly Stack<IntPtr> devNotifyHandles = new Stack<IntPtr>();
    private readonly List<System.Exception> deferredExceptions = new List<System.Exception>();
    private volatile AggregateException exception;
    private volatile MessageWindowStatus currentStatus;
    private IntPtr handle;
    private bool disposed;
    private int unmanagedThreadId;
    private WNDPROC cachedWindowProc;

    private MessageWindow(
      IHandleDeviceChanged deviceChangeHandler,
      IHandleThreadException threadExceptionHandler)
    {
      this.deviceChangeHandler = deviceChangeHandler;
      this.threadExceptionHandler = threadExceptionHandler;
    }

    public AggregateException Exception
    {
      get
      {
        if (this.currentStatus == MessageWindowStatus.Faulted && this.exception == null)
          this.exception = new AggregateException((IEnumerable<System.Exception>) this.deferredExceptions);
        return this.exception;
      }
    }

    public MessageWindowStatus Status => this.currentStatus;

    public static void Create(
      IHandleDeviceChanged deviceChangeHandler,
      IHandleThreadException threadExceptionHandler,
      ref MessageWindow window)
    {
      bool success1 = false;
      bool success2 = false;
      string className = (string) null;
      IntPtr num = IntPtr.Zero;
      MessageWindow messageWindow = new MessageWindow(deviceChangeHandler, threadExceptionHandler);
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        MessageWindowClass.AcquireClass(ref className);
        ThreadAffinity.BeginThreadAffinity(ref success1);
        CriticalRegion.BeginCriticalRegion(ref success2);
        messageWindow.unmanagedThreadId = Kernel32NativeMethods.GetCurrentThreadId();
        IntPtr moduleHandle = Kernel32NativeMethods.GetModuleHandle(IntPtr.Zero);
        if (moduleHandle == IntPtr.Zero)
          throw new Win32Exception();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          RobustTrace.Trace<string>(new Action<string>(MessageTraceSource.Instance.MessageWindowCreation_Start), className);
          num = User32NativeMethods.CreateWindowEx(0, className, (string) null, 0, 0, 0, 0, 0, new IntPtr(-3), IntPtr.Zero, moduleHandle, IntPtr.Zero);
          if (num != IntPtr.Zero)
          {
            messageWindow.handle = num;
            window = messageWindow;
          }
        }
        if (num == IntPtr.Zero)
        {
          try
          {
            throw new Win32Exception();
          }
          catch (Win32Exception ex)
          {
            RobustTrace.Trace<string, int, string>(new Action<string, int, string>(MessageTraceSource.Instance.MessageWindowCreation_Error), className, ex.NativeErrorCode, ex.Message);
            throw;
          }
        }
        else
          RobustTrace.Trace<string, IntPtr>(new Action<string, IntPtr>(MessageTraceSource.Instance.MessageWindowCreation_Stop), className, num);
      }
      catch
      {
        if (num == IntPtr.Zero)
        {
          if (success2)
            CriticalRegion.EndCriticalRegion();
          if (success1)
            ThreadAffinity.EndThreadAffinity();
          if (className != null)
            MessageWindowClass.ReleaseClass();
        }
        throw;
      }
    }

    public void RegisterDeviceNotification(IEnumerable<Guid> classGuids)
    {
      this.VerifyNotDisposed();
      this.VerifyAccess();
      foreach (Guid classGuid in classGuids)
      {
        DEV_BROADCAST_DEVICEINTERFACE NotificationFilter = new DEV_BROADCAST_DEVICEINTERFACE()
        {
          dbcc_size = Marshal.SizeOf(typeof (DEV_BROADCAST_DEVICEINTERFACE)),
          dbcc_devicetype = 5,
          dbcc_classguid = classGuid
        };
        RobustTrace.Trace<IntPtr, Guid>(new Action<IntPtr, Guid>(MessageTraceSource.Instance.DeviceNotificationRegistration_Start), this.handle, classGuid);
        try
        {
          IntPtr num = User32NativeMethods.RegisterDeviceNotification(new HandleRef((object) this, this.handle), ref NotificationFilter, 0);
          if (num == IntPtr.Zero)
            throw new Win32Exception();
          RobustTrace.Trace<IntPtr, Guid, IntPtr>(new Action<IntPtr, Guid, IntPtr>(MessageTraceSource.Instance.DeviceNotificationRegistration_Stop), this.handle, classGuid, num);
          this.devNotifyHandles.Push(num);
        }
        catch (Win32Exception ex1)
        {
          RobustTrace.Trace<IntPtr, Guid, int, string>(new Action<IntPtr, Guid, int, string>(MessageTraceSource.Instance.DeviceNotificationRegistration_Error), this.handle, classGuid, ex1.NativeErrorCode, ex1.Message);
          try
          {
            this.CloseAsync();
          }
          catch (Win32Exception ex2)
          {
            throw new AggregateException(new System.Exception[2]
            {
              (System.Exception) ex1,
              (System.Exception) ex2
            });
          }
          throw new AggregateException(new System.Exception[1]
          {
            (System.Exception) ex1
          });
        }
      }
    }

    public void AttachWindowProc()
    {
      this.VerifyNotDisposed();
      this.VerifyAccess();
      try
      {
        WNDPROC dwNewLong = new WNDPROC(this.WindowProc);
        RobustTrace.Trace<IntPtr>(new Action<IntPtr>(MessageTraceSource.Instance.MessageWindowProcAttach_Start), this.handle);
        if ((IntPtr.Size == 4 ? User32NativeMethods.SetWindowLong(this.handle, -4, dwNewLong) : User32NativeMethods.SetWindowLongPtr(this.handle, -4, dwNewLong)) == null)
          throw new Win32Exception();
        RobustTrace.Trace<IntPtr>(new Action<IntPtr>(MessageTraceSource.Instance.MessageWindowProcAttach_Stop), this.handle);
        this.cachedWindowProc = dwNewLong;
      }
      catch (Win32Exception ex1)
      {
        RobustTrace.Trace<IntPtr, int, string>(new Action<IntPtr, int, string>(MessageTraceSource.Instance.MessageWindowProcAttach_Error), this.handle, ex1.NativeErrorCode, ex1.Message);
        try
        {
          this.CloseAsync();
        }
        catch (Win32Exception ex2)
        {
          throw new AggregateException(new System.Exception[2]
          {
            (System.Exception) ex1,
            (System.Exception) ex2
          });
        }
        throw new AggregateException(new System.Exception[1]
        {
          (System.Exception) ex1
        });
      }
    }

    public void CloseAsync()
    {
      IntPtr handle;
      lock (this.syncRoot)
        handle = this.handle;
      if (handle == IntPtr.Zero)
        return;
      RobustTrace.Trace<IntPtr>(new Action<IntPtr>(MessageTraceSource.Instance.MessageWindowCloseRequest_Start), handle);
      if (!User32NativeMethods.PostMessage(new HandleRef((object) this, handle), 16, IntPtr.Zero, IntPtr.Zero))
      {
        try
        {
          throw new Win32Exception();
        }
        catch (Win32Exception ex)
        {
          RobustTrace.Trace<IntPtr, int, string>(new Action<IntPtr, int, string>(MessageTraceSource.Instance.MessageWindowCloseRequest_Error), handle, ex.NativeErrorCode, ex.Message);
          throw;
        }
      }
      else
        RobustTrace.Trace<IntPtr>(new Action<IntPtr>(MessageTraceSource.Instance.MessageWindowCloseRequest_Stop), handle);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    public void Dispose()
    {
      if (this.disposed)
        return;
      this.VerifyAccess();
      this.disposed = true;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        CriticalRegion.EndCriticalRegion();
        ThreadAffinity.EndThreadAffinity();
        MessageWindowClass.ReleaseClass();
      }
    }

    private IntPtr WindowProc(IntPtr hwnd, int uMsg, IntPtr wParam, IntPtr lParam)
    {
      RobustTrace.Trace<IntPtr, int, IntPtr, IntPtr>(new Action<IntPtr, int, IntPtr, IntPtr>(MessageTraceSource.Instance.WindowMessage), hwnd, uMsg, wParam, lParam);
      IntPtr num = IntPtr.Zero;
      switch (uMsg)
      {
        case 2:
          this.WmDestroy();
          break;
        case 537:
          this.WmDeviceChange(wParam, lParam);
          break;
        default:
          num = User32NativeMethods.DefWindowProc(hwnd, uMsg, wParam, lParam);
          break;
      }
      return num;
    }

    private void WmDeviceChange(IntPtr wParam, IntPtr lParam)
    {
      DEV_BROADCAST_HDR structure1 = (DEV_BROADCAST_HDR) Marshal.PtrToStructure(lParam, typeof (DEV_BROADCAST_HDR));
      int int32 = wParam.ToInt32();
      RobustTrace.Trace<IntPtr, int, int>(new Action<IntPtr, int, int>(MessageTraceSource.Instance.DeviceNotification), this.handle, int32, structure1.dbch_devicetype);
      if (structure1.dbch_devicetype != 5 || wParam != new IntPtr(32768) && wParam != new IntPtr(32772))
        return;
      DEV_BROADCAST_DEVICEINTERFACE structure2 = (DEV_BROADCAST_DEVICEINTERFACE) Marshal.PtrToStructure(lParam, typeof (DEV_BROADCAST_DEVICEINTERFACE));
      try
      {
        RobustTrace.Trace<IntPtr, string, Guid, int>(new Action<IntPtr, string, Guid, int>(MessageTraceSource.Instance.DeviceNotificationProcessing_Start), this.handle, structure2.dbcc_name, structure2.dbcc_classguid, int32);
        this.deviceChangeHandler.HandleDeviceChanged(int32, ref structure2);
        RobustTrace.Trace<IntPtr, string, Guid, int>(new Action<IntPtr, string, Guid, int>(MessageTraceSource.Instance.DeviceNotificationProcessing_Stop), this.handle, structure2.dbcc_name, structure2.dbcc_classguid, int32);
      }
      catch (System.Exception ex)
      {
        if (ExceptionServices.IsCriticalException(ex))
        {
          throw;
        }
        else
        {
          RobustTrace.Trace<IntPtr, string, Guid, int, System.Exception>(new Action<IntPtr, string, Guid, int, System.Exception>(MessageTraceSource.Instance.DeviceNotificationProcessing_Error), this.handle, structure2.dbcc_name, structure2.dbcc_classguid, int32, ex);
          this.HandleThreadException(ex);
        }
      }
    }

    private void HandleThreadException(System.Exception error)
    {
      bool flag = false;
      RobustTrace.Trace<IntPtr, System.Exception>(new Action<IntPtr, System.Exception>(MessageTraceSource.Instance.ThreadExceptionDelegation_Start), this.handle, error);
      try
      {
        flag = this.threadExceptionHandler.TryHandleThreadException(error);
      }
      catch (System.Exception ex)
      {
        if (ExceptionServices.IsCriticalException(ex))
        {
          throw;
        }
        else
        {
          RobustTrace.Trace<IntPtr, System.Exception, System.Exception>(new Action<IntPtr, System.Exception, System.Exception>(MessageTraceSource.Instance.ThreadExceptionDelegation_Error), this.handle, error, ex);
          this.deferredExceptions.Add(ex);
        }
      }
      RobustTrace.Trace<IntPtr, bool, System.Exception>(new Action<IntPtr, bool, System.Exception>(MessageTraceSource.Instance.ThreadExceptionDelegation_Stop), this.handle, flag, error);
      if (flag)
        return;
      this.deferredExceptions.Add(error);
      this.CloseAsync();
    }

    private void WmDestroy()
    {
      RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopExitRequest_Start));
      User32NativeMethods.PostQuitMessage(0);
      RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopExitRequest_Stop));
      IntPtr handle = this.handle;
      lock (this.syncRoot)
        this.handle = new IntPtr(-1);
      while (this.devNotifyHandles.Count > 0)
      {
        IntPtr Handle = this.devNotifyHandles.Pop();
        RobustTrace.Trace<IntPtr, IntPtr>(new Action<IntPtr, IntPtr>(MessageTraceSource.Instance.DeviceNotificationUnregistration_Start), handle, Handle);
        if (User32NativeMethods.UnregisterDeviceNotification(Handle))
        {
          RobustTrace.Trace<IntPtr, IntPtr>(new Action<IntPtr, IntPtr>(MessageTraceSource.Instance.DeviceNotificationUnregistration_Stop), handle, Handle);
        }
        else
        {
          try
          {
            throw new Win32Exception();
          }
          catch (Win32Exception ex)
          {
            RobustTrace.Trace<IntPtr, IntPtr, int, string>(new Action<IntPtr, IntPtr, int, string>(MessageTraceSource.Instance.DeviceNotificationUnregistration_Error), handle, Handle, ex.NativeErrorCode, ex.Message);
            this.deferredExceptions.Add((System.Exception) ex);
          }
        }
      }
      MessageWindowStatus currentStatus = this.currentStatus;
      MessageWindowStatus messageWindowStatus = this.deferredExceptions.Count == 0 ? MessageWindowStatus.Destroyed : MessageWindowStatus.Faulted;
      RobustTrace.Trace<IntPtr, MessageWindowStatus, MessageWindowStatus>(new Action<IntPtr, MessageWindowStatus, MessageWindowStatus>(MessageTraceSource.Instance.MessageWindowStatusChange_Start), handle, currentStatus, messageWindowStatus);
      this.currentStatus = messageWindowStatus;
      RobustTrace.Trace<IntPtr, MessageWindowStatus, MessageWindowStatus>(new Action<IntPtr, MessageWindowStatus, MessageWindowStatus>(MessageTraceSource.Instance.MessageWindowStatusChange_Stop), handle, currentStatus, messageWindowStatus);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private void VerifyAccess()
    {
      if (this.unmanagedThreadId != Kernel32NativeMethods.GetCurrentThreadId())
        throw new InvalidOperationException(Resources.InvalidOperationException_MessageText_CallingThreadDoesNotHaveAccessToThisMessageWindowInstance);
    }

    private bool CheckNotDisposed() => this.disposed;

    private void VerifyNotDisposed()
    {
      if (this.CheckNotDisposed())
        throw new ObjectDisposedException(this.GetType().FullName);
    }
  }
}
