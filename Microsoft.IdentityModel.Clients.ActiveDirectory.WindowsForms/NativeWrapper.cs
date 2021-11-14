// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.NativeWrapper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal class NativeWrapper
  {
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
      public int x;
      public int y;

      public POINT()
      {
      }

      public POINT(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class OLECMD
    {
      [MarshalAs(UnmanagedType.U4)]
      public int cmdID;
      [MarshalAs(UnmanagedType.U4)]
      public int cmdf;
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
    [ComImport]
    public interface IOleCommandTarget
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int QueryStatus(
        ref Guid pguidCmdGroup,
        int cCmds,
        [In, Out] NativeWrapper.OLECMD prgCmds,
        [In, Out] IntPtr pCmdText);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int Exec(IntPtr guid, int nCmdID, int nCmdexecopt, [MarshalAs(UnmanagedType.LPArray), In] object[] pvaIn, IntPtr pvaOut);
    }

    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public class DOCHOSTUIINFO
    {
      [MarshalAs(UnmanagedType.U4)]
      public int cbSize = Marshal.SizeOf(typeof (NativeWrapper.DOCHOSTUIINFO));
      [MarshalAs(UnmanagedType.I4)]
      public int dwFlags;
      [MarshalAs(UnmanagedType.I4)]
      public int dwDoubleClick;
      [MarshalAs(UnmanagedType.I4)]
      public int dwReserved1;
      [MarshalAs(UnmanagedType.I4)]
      public int dwReserved2;
    }

    [Serializable]
    public struct MSG
    {
      public IntPtr hwnd;
      public int message;
      public IntPtr wParam;
      public IntPtr lParam;
      public int time;
      public int pt_x;
      public int pt_y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class COMRECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;

      public override string ToString() => "Left = " + (object) this.left + " Top " + (object) this.top + " Right = " + (object) this.right + " Bottom = " + (object) this.bottom;

      public COMRECT()
      {
      }

      public COMRECT(Rectangle r)
      {
        this.left = r.X;
        this.top = r.Y;
        this.right = r.Right;
        this.bottom = r.Bottom;
      }

      public COMRECT(int left, int top, int right, int bottom)
      {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
      }

      public static NativeWrapper.COMRECT FromXYWH(
        int x,
        int y,
        int width,
        int height)
      {
        return new NativeWrapper.COMRECT(x, y, x + width, y + height);
      }
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000115-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IOleInPlaceUIWindow
    {
      IntPtr GetWindow();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int ContextSensitiveHelp(int fEnterMode);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetBorder([Out] NativeWrapper.COMRECT lprectBorder);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RequestBorderSpace([In] NativeWrapper.COMRECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetBorderSpace([In] NativeWrapper.COMRECT pborderwidths);

      void SetActiveObject(
        [MarshalAs(UnmanagedType.Interface), In] NativeWrapper.IOleInPlaceActiveObject pActiveObject,
        [MarshalAs(UnmanagedType.LPWStr), In] string pszObjName);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000117-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IOleInPlaceActiveObject
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetWindow(out IntPtr hwnd);

      void ContextSensitiveHelp(int fEnterMode);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int TranslateAccelerator([In] ref NativeWrapper.MSG lpmsg);

      void OnFrameWindowActivate(bool fActivate);

      void OnDocWindowActivate(int fActivate);

      void ResizeBorder(
        [In] NativeWrapper.COMRECT prcBorder,
        [In] NativeWrapper.IOleInPlaceUIWindow pUIWindow,
        bool fFrameWindow);

      void EnableModeless(int fEnable);
    }

    [StructLayout(LayoutKind.Sequential)]
    public sealed class tagOleMenuGroupWidths
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
      public int[] widths = new int[6];
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000116-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IOleInPlaceFrame
    {
      IntPtr GetWindow();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int ContextSensitiveHelp(int fEnterMode);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetBorder([Out] NativeWrapper.COMRECT lprectBorder);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RequestBorderSpace([In] NativeWrapper.COMRECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetBorderSpace([In] NativeWrapper.COMRECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetActiveObject(
        [MarshalAs(UnmanagedType.Interface), In] NativeWrapper.IOleInPlaceActiveObject pActiveObject,
        [MarshalAs(UnmanagedType.LPWStr), In] string pszObjName);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int InsertMenus([In] IntPtr hmenuShared, [In, Out] NativeWrapper.tagOleMenuGroupWidths lpMenuWidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetMenu([In] IntPtr hmenuShared, [In] IntPtr holemenu, [In] IntPtr hwndActiveObject);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RemoveMenus([In] IntPtr hmenuShared);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetStatusText([MarshalAs(UnmanagedType.LPWStr), In] string pszStatusText);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int EnableModeless(bool fEnable);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int TranslateAccelerator([In] ref NativeWrapper.MSG lpmsg, [MarshalAs(UnmanagedType.U2), In] short wID);
    }

    [Guid("00000122-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IOleDropTarget
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int OleDragEnter(
        [MarshalAs(UnmanagedType.Interface), In] object pDataObj,
        [MarshalAs(UnmanagedType.U4), In] int grfKeyState,
        [In] NativeWrapper.POINT pt,
        [In, Out] ref int pdwEffect);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int OleDragOver([MarshalAs(UnmanagedType.U4), In] int grfKeyState, [In] NativeWrapper.POINT pt, [In, Out] ref int pdwEffect);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int OleDragLeave();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int OleDrop([MarshalAs(UnmanagedType.Interface), In] object pDataObj, [MarshalAs(UnmanagedType.U4), In] int grfKeyState, [In] NativeWrapper.POINT pt, [In, Out] ref int pdwEffect);
    }

    [Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [ComImport]
    public interface IDocHostUIHandler
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ShowContextMenu(
        [MarshalAs(UnmanagedType.U4), In] int dwID,
        [In] NativeWrapper.POINT pt,
        [MarshalAs(UnmanagedType.Interface), In] object pcmdtReserved,
        [MarshalAs(UnmanagedType.Interface), In] object pdispReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetHostInfo([In, Out] NativeWrapper.DOCHOSTUIINFO info);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ShowUI(
        [MarshalAs(UnmanagedType.I4), In] int dwID,
        [In] NativeWrapper.IOleInPlaceActiveObject activeObject,
        [In] NativeWrapper.IOleCommandTarget commandTarget,
        [In] NativeWrapper.IOleInPlaceFrame frame,
        [In] NativeWrapper.IOleInPlaceUIWindow doc);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int HideUI();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int UpdateUI();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int EnableModeless([MarshalAs(UnmanagedType.Bool), In] bool fEnable);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int OnDocWindowActivate([MarshalAs(UnmanagedType.Bool), In] bool fActivate);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int OnFrameWindowActivate([MarshalAs(UnmanagedType.Bool), In] bool fActivate);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ResizeBorder(
        [In] NativeWrapper.COMRECT rect,
        [In] NativeWrapper.IOleInPlaceUIWindow doc,
        bool fFrameWindow);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int TranslateAccelerator([In] ref NativeWrapper.MSG msg, [In] ref Guid group, [MarshalAs(UnmanagedType.I4), In] int nCmdID);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetOptionKeyPath([MarshalAs(UnmanagedType.LPArray), Out] string[] pbstrKey, [MarshalAs(UnmanagedType.U4), In] int dw);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetDropTarget(
        [MarshalAs(UnmanagedType.Interface), In] NativeWrapper.IOleDropTarget pDropTarget,
        [MarshalAs(UnmanagedType.Interface)] out NativeWrapper.IOleDropTarget ppDropTarget);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetExternal([MarshalAs(UnmanagedType.Interface)] out object ppDispatch);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int TranslateUrl([MarshalAs(UnmanagedType.U4), In] int dwTranslate, [MarshalAs(UnmanagedType.LPWStr), In] string strURLIn, [MarshalAs(UnmanagedType.LPWStr)] out string pstrURLOut);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int FilterDataObject(IDataObject pDO, out IDataObject ppDORet);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E")]
    [ComImport]
    public interface IWebBrowser2
    {
      [DispId(203)]
      object Document { [DispId(203)] [return: MarshalAs(UnmanagedType.IDispatch)] get; }

      [DispId(551)]
      bool Silent { [DispId(551)] [param: MarshalAs(UnmanagedType.Bool)] set; }
    }

    [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface DWebBrowserEvents2
    {
      [DispId(102)]
      void StatusTextChange([In] string text);

      [DispId(108)]
      void ProgressChange([In] int progress, [In] int progressMax);

      [DispId(105)]
      void CommandStateChange([In] long command, [In] bool enable);

      [DispId(106)]
      void DownloadBegin();

      [DispId(104)]
      void DownloadComplete();

      [DispId(113)]
      void TitleChange([In] string text);

      [DispId(112)]
      void PropertyChange([In] string szProperty);

      [DispId(250)]
      void BeforeNavigate2(
        [MarshalAs(UnmanagedType.IDispatch), In] object pDisp,
        [In] ref object URL,
        [In] ref object flags,
        [In] ref object targetFrameName,
        [In] ref object postData,
        [In] ref object headers,
        [In, Out] ref bool cancel);

      [DispId(251)]
      void NewWindow2([MarshalAs(UnmanagedType.IDispatch), In, Out] ref object pDisp, [In, Out] ref bool cancel);

      [DispId(252)]
      void NavigateComplete2([MarshalAs(UnmanagedType.IDispatch), In] object pDisp, [In] ref object URL);

      [DispId(259)]
      void DocumentComplete([MarshalAs(UnmanagedType.IDispatch), In] object pDisp, [In] ref object URL);

      [DispId(253)]
      void OnQuit();

      [DispId(254)]
      void OnVisible([In] bool visible);

      [DispId(255)]
      void OnToolBar([In] bool toolBar);

      [DispId(256)]
      void OnMenuBar([In] bool menuBar);

      [DispId(257)]
      void OnStatusBar([In] bool statusBar);

      [DispId(258)]
      void OnFullScreen([In] bool fullScreen);

      [DispId(260)]
      void OnTheaterMode([In] bool theaterMode);

      [DispId(262)]
      void WindowSetResizable([In] bool resizable);

      [DispId(264)]
      void WindowSetLeft([In] int left);

      [DispId(265)]
      void WindowSetTop([In] int top);

      [DispId(266)]
      void WindowSetWidth([In] int width);

      [DispId(267)]
      void WindowSetHeight([In] int height);

      [DispId(263)]
      void WindowClosing([In] bool isChildWindow, [In, Out] ref bool cancel);

      [DispId(268)]
      void ClientToHostWindow([In, Out] ref long cx, [In, Out] ref long cy);

      [DispId(269)]
      void SetSecureLockIcon([In] int secureLockIcon);

      [DispId(270)]
      void FileDownload([In, Out] ref bool cancel);

      [DispId(271)]
      void NavigateError(
        [MarshalAs(UnmanagedType.IDispatch), In] object pDisp,
        [In] ref object URL,
        [In] ref object frame,
        [In] ref object statusCode,
        [In, Out] ref bool cancel);

      [DispId(225)]
      void PrintTemplateInstantiation([MarshalAs(UnmanagedType.IDispatch), In] object pDisp);

      [DispId(226)]
      void PrintTemplateTeardown([MarshalAs(UnmanagedType.IDispatch), In] object pDisp);

      [DispId(227)]
      void UpdatePageStatus([MarshalAs(UnmanagedType.IDispatch), In] object pDisp, [In] ref object nPage, [In] ref object fDone);

      [DispId(272)]
      void PrivacyImpactedStateChange([In] bool bImpacted);
    }

    internal static class NativeMethods
    {
      [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
      internal static extern IntPtr GetDC(IntPtr hWnd);

      [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
      internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

      [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
      internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

      [DllImport("User32.dll")]
      internal static extern bool IsProcessDPIAware();
    }
  }
}
