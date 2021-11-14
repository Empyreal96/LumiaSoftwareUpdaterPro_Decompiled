// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.Primitives.MessageLoop
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Diagnostics;
using Nokia.Lucid.Interop;
using Nokia.Lucid.Interop.Win32Types;
using Nokia.Lucid.Primitives;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Nokia.Lucid.DeviceDetection.Primitives
{
  internal static class MessageLoop
  {
    [ThreadStatic]
    private static int level;

    public static bool IsRunning => MessageLoop.level > 0;

    internal static int Level => MessageLoop.level;

    public static int Run()
    {
      int num = 0;
      bool flag1 = true;
      bool success1 = false;
      bool success2 = false;
      bool flag2 = false;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        ThreadAffinity.BeginThreadAffinity(ref success1);
        CriticalRegion.BeginCriticalRegion(ref success2);
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          ++MessageLoop.level;
          flag2 = true;
        }
        RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopEnter_StartStop));
        while (flag1)
        {
          if (User32NativeMethods.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 100, 1279, 4) != 258)
          {
            MSG lpMsg;
            while (User32NativeMethods.PeekMessage(out lpMsg, new HandleRef(), 0, 0, 1))
            {
              if (lpMsg.message == 18)
              {
                RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopExit_Start));
                flag1 = false;
                num = lpMsg.wParam.ToInt32();
                break;
              }
              if (lpMsg.hwnd == IntPtr.Zero)
              {
                RobustTrace.Trace<int, IntPtr, IntPtr>(new Action<int, IntPtr, IntPtr>(MessageTraceSource.Instance.ThreadMessage), lpMsg.message, lpMsg.wParam, lpMsg.lParam);
              }
              else
              {
                User32NativeMethods.TranslateMessage(ref lpMsg);
                RobustTrace.Trace<IntPtr, int, IntPtr, IntPtr>(new Action<IntPtr, int, IntPtr, IntPtr>(MessageTraceSource.Instance.MessageDispatch_Start), lpMsg.hwnd, lpMsg.message, lpMsg.wParam, lpMsg.lParam);
                try
                {
                  User32NativeMethods.DispatchMessage(ref lpMsg);
                  RobustTrace.Trace<IntPtr, int, IntPtr, IntPtr>(new Action<IntPtr, int, IntPtr, IntPtr>(MessageTraceSource.Instance.MessageDispatch_Stop), lpMsg.hwnd, lpMsg.message, lpMsg.wParam, lpMsg.lParam);
                }
                catch (Exception ex)
                {
                  if (ExceptionServices.IsCriticalException(ex))
                    throw;
                  else
                    RobustTrace.Trace<IntPtr, int, IntPtr, IntPtr, Exception>(new Action<IntPtr, int, IntPtr, IntPtr, Exception>(MessageTraceSource.Instance.MessageDispatch_Error), lpMsg.hwnd, lpMsg.message, lpMsg.wParam, lpMsg.lParam, ex);
                }
              }
            }
          }
          else
            Thread.Yield();
        }
      }
      finally
      {
        if (success2)
          CriticalRegion.EndCriticalRegion();
        if (success1)
          ThreadAffinity.EndThreadAffinity();
        if (flag2)
          --MessageLoop.level;
      }
      RobustTrace.Trace(new Action(MessageTraceSource.Instance.MessageLoopExit_Stop));
      return num;
    }
  }
}
