// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.User32NativeMethods
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Interop.Win32Types;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.Interop
{
  internal static class User32NativeMethods
  {
    private const string User32DllName = "user32.dll";

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int MsgWaitForMultipleObjectsEx(
      int nCount,
      IntPtr pHandles,
      int dwMilliseconds,
      int dwWakeMask,
      int dwFlags);

    [DllImport("user32.dll")]
    public static extern int GetQueueStatus(int flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr DefWindowProc(
      IntPtr hWnd,
      int Msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekMessage(
      out MSG lpMsg,
      HandleRef hWnd,
      int wMsgFilterMin,
      int wMsgFilterMax,
      int wRemoveMsg);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(HandleRef hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr DispatchMessage(ref MSG lpmsg);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr CreateWindowEx(
      int dwExStyle,
      string lpClassName,
      string lpWindowName,
      int dwStyle,
      int x,
      int y,
      int nWidth,
      int nHeight,
      IntPtr hWndParent,
      IntPtr hMenu,
      IntPtr hInstance,
      IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(HandleRef hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern WNDPROC SetWindowLong(IntPtr hWnd, int nIndex, WNDPROC dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern WNDPROC SetWindowLongPtr(
      IntPtr hWnd,
      int nIndex,
      WNDPROC dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr RegisterDeviceNotification(
      HandleRef hRecipient,
      ref DEV_BROADCAST_DEVICEINTERFACE NotificationFilter,
      int Flags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterDeviceNotification(IntPtr Handle);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern short RegisterClassEx(ref WNDCLASSEX lpwcx);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);
  }
}
