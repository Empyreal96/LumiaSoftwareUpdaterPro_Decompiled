// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.Primitives.MessageWindowClass
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Diagnostics;
using Nokia.Lucid.Interop;
using Nokia.Lucid.Interop.Win32Types;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace Nokia.Lucid.DeviceDetection.Primitives
{
  internal static class MessageWindowClass
  {
    private static readonly WNDPROC CachedDefWindowProc = new WNDPROC(MessageWindowClass.DefWindowProc);
    [ThreadStatic]
    private static int referenceCount;
    [ThreadStatic]
    private static string threadClassName;

    public static bool IsClassAcquired => MessageWindowClass.referenceCount > 0;

    internal static int ReferenceCount => MessageWindowClass.referenceCount;

    public static void AcquireClass(ref string className)
    {
      if (MessageWindowClass.referenceCount > 0)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          ++MessageWindowClass.referenceCount;
          className = MessageWindowClass.threadClassName;
        }
      }
      else
      {
        IntPtr moduleHandle = Kernel32NativeMethods.GetModuleHandle(IntPtr.Zero);
        if (moduleHandle == IntPtr.Zero)
          throw new Win32Exception();
        string str = typeof (MessageWindowClass).FullName + "." + (object) Thread.CurrentThread.ManagedThreadId;
        WNDCLASSEX lpwcx = new WNDCLASSEX()
        {
          cbSize = Marshal.SizeOf(typeof (WNDCLASSEX)),
          lpszClassName = str,
          hInstance = moduleHandle,
          lpfnWndProc = MessageWindowClass.CachedDefWindowProc
        };
        RuntimeHelpers.PrepareConstrainedRegions();
        int num;
        try
        {
        }
        finally
        {
          num = (int) User32NativeMethods.RegisterClassEx(ref lpwcx);
          if (num != 0)
          {
            ++MessageWindowClass.referenceCount;
            MessageWindowClass.threadClassName = str;
            className = str;
          }
        }
        if (num == 0)
          throw new Win32Exception();
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    public static void ReleaseClass()
    {
      if (MessageWindowClass.referenceCount <= 0)
        throw new InvalidOperationException();
      if (MessageWindowClass.referenceCount > 1)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          --MessageWindowClass.referenceCount;
        }
      }
      else
      {
        IntPtr moduleHandle = Kernel32NativeMethods.GetModuleHandle(IntPtr.Zero);
        if (moduleHandle == IntPtr.Zero)
          throw new Win32Exception();
        RuntimeHelpers.PrepareConstrainedRegions();
        bool flag;
        try
        {
        }
        finally
        {
          flag = User32NativeMethods.UnregisterClass(MessageWindowClass.threadClassName, moduleHandle);
          if (flag)
          {
            --MessageWindowClass.referenceCount;
            MessageWindowClass.threadClassName = (string) null;
          }
        }
        if (!flag)
          throw new Win32Exception();
      }
    }

    private static IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
      RobustTrace.Trace<IntPtr, int, IntPtr, IntPtr>(new Action<IntPtr, int, IntPtr, IntPtr>(MessageTraceSource.Instance.WindowMessage), hWnd, msg, wParam, lParam);
      if (msg != 2)
        return User32NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
      RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopExitRequest_Start));
      User32NativeMethods.PostQuitMessage(0);
      RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopExitRequest_Stop));
      return IntPtr.Zero;
    }
  }
}
