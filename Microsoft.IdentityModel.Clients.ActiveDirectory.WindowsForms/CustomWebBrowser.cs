// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.CustomWebBrowser
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal class CustomWebBrowser : WebBrowser
  {
    private const int S_OK = 0;
    private const int S_FALSE = 1;
    private const int WM_CHAR = 258;
    private AxHost.ConnectionPointCookie webBrowserEventCookie;
    private CustomWebBrowser.CustomWebBrowserEvent webBrowserEvent;
    private static readonly HashSet<Shortcut> shortcutBlacklist = new HashSet<Shortcut>();

    static CustomWebBrowser()
    {
      CustomWebBrowser.shortcutBlacklist.Add(Shortcut.AltBksp);
      CustomWebBrowser.shortcutBlacklist.Add(Shortcut.AltDownArrow);
      CustomWebBrowser.shortcutBlacklist.Add(Shortcut.AltLeftArrow);
      CustomWebBrowser.shortcutBlacklist.Add(Shortcut.AltRightArrow);
      CustomWebBrowser.shortcutBlacklist.Add(Shortcut.AltUpArrow);
    }

    protected override WebBrowserSiteBase CreateWebBrowserSiteBase() => (WebBrowserSiteBase) new CustomWebBrowser.CustomSite((WebBrowser) this);

    protected override void CreateSink()
    {
      base.CreateSink();
      object activeXinstance = this.ActiveXInstance;
      if (activeXinstance == null)
        return;
      this.webBrowserEvent = new CustomWebBrowser.CustomWebBrowserEvent(this);
      this.webBrowserEventCookie = new AxHost.ConnectionPointCookie(activeXinstance, (object) this.webBrowserEvent, typeof (NativeWrapper.DWebBrowserEvents2));
    }

    protected override void DetachSink()
    {
      if (this.webBrowserEventCookie != null)
      {
        this.webBrowserEventCookie.Disconnect();
        this.webBrowserEventCookie = (AxHost.ConnectionPointCookie) null;
      }
      base.DetachSink();
    }

    protected virtual void OnNavigateError(WebBrowserNavigateErrorEventArgs e)
    {
      if (this.NavigateError == null)
        return;
      this.NavigateError((object) this, e);
    }

    public event WebBrowserNavigateErrorEventHandler NavigateError;

    [ComVisible(true)]
    [ComDefaultInterface(typeof (NativeWrapper.IDocHostUIHandler))]
    protected class CustomSite : 
      WebBrowser.WebBrowserSite,
      NativeWrapper.IDocHostUIHandler,
      ICustomQueryInterface
    {
      private const int NotImplemented = -2147467263;
      private readonly WebBrowser host;

      public CustomSite(WebBrowser host)
        : base(host)
      {
        this.host = host;
      }

      public int EnableModeless(bool fEnable) => -2147467263;

      public int FilterDataObject(System.Runtime.InteropServices.ComTypes.IDataObject pDO, out System.Runtime.InteropServices.ComTypes.IDataObject ppDORet)
      {
        ppDORet = (System.Runtime.InteropServices.ComTypes.IDataObject) null;
        return 1;
      }

      public int GetDropTarget(
        NativeWrapper.IOleDropTarget pDropTarget,
        out NativeWrapper.IOleDropTarget ppDropTarget)
      {
        ppDropTarget = (NativeWrapper.IOleDropTarget) null;
        return 0;
      }

      public int GetExternal(out object ppDispatch)
      {
        WebBrowser host = this.host;
        ppDispatch = host.ObjectForScripting;
        return 0;
      }

      public int GetHostInfo(NativeWrapper.DOCHOSTUIINFO info)
      {
        WebBrowser host = this.host;
        info.dwDoubleClick = 0;
        info.dwFlags = 131088;
        if (NativeWrapper.NativeMethods.IsProcessDPIAware())
          info.dwFlags |= 1073741824;
        if (host.ScrollBarsEnabled)
          info.dwFlags |= 128;
        else
          info.dwFlags |= 8;
        if (Application.RenderWithVisualStyles)
          info.dwFlags |= 262144;
        else
          info.dwFlags |= 524288;
        info.dwFlags |= 67108864;
        return 0;
      }

      public int GetOptionKeyPath(string[] pbstrKey, int dw) => -2147467263;

      public int HideUI() => -2147467263;

      public int OnDocWindowActivate(bool fActivate) => -2147467263;

      public int OnFrameWindowActivate(bool fActivate) => -2147467263;

      public int ResizeBorder(
        NativeWrapper.COMRECT rect,
        NativeWrapper.IOleInPlaceUIWindow doc,
        bool fFrameWindow)
      {
        return -2147467263;
      }

      public int ShowContextMenu(
        int dwID,
        NativeWrapper.POINT pt,
        object pcmdtReserved,
        object pdispReserved)
      {
        switch (dwID)
        {
          case 2:
          case 4:
          case 9:
          case 16:
            return 1;
          default:
            return 0;
        }
      }

      public int ShowUI(
        int dwID,
        NativeWrapper.IOleInPlaceActiveObject activeObject,
        NativeWrapper.IOleCommandTarget commandTarget,
        NativeWrapper.IOleInPlaceFrame frame,
        NativeWrapper.IOleInPlaceUIWindow doc)
      {
        return 1;
      }

      public int TranslateAccelerator(ref NativeWrapper.MSG msg, ref Guid group, int nCmdID)
      {
        if (msg.message != 258 && (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.Control))
        {
          Shortcut shortcut = (Shortcut) ((Keys) (int) msg.wParam | Control.ModifierKeys);
          if (CustomWebBrowser.shortcutBlacklist.Contains(shortcut))
            return 0;
        }
        return 1;
      }

      public int TranslateUrl(int dwTranslate, string strUrlIn, out string pstrUrlOut)
      {
        pstrUrlOut = (string) null;
        return 1;
      }

      public int UpdateUI() => -2147467263;

      public CustomQueryInterfaceResult GetInterface(
        ref Guid iid,
        out IntPtr ppv)
      {
        ppv = IntPtr.Zero;
        if (!(iid == typeof (NativeWrapper.IDocHostUIHandler).GUID))
          return CustomQueryInterfaceResult.NotHandled;
        ppv = Marshal.GetComInterfaceForObject((object) this, typeof (NativeWrapper.IDocHostUIHandler), CustomQueryInterfaceMode.Ignore);
        return CustomQueryInterfaceResult.Handled;
      }
    }

    [ClassInterface(ClassInterfaceType.None)]
    private sealed class CustomWebBrowserEvent : 
      StandardOleMarshalObject,
      NativeWrapper.DWebBrowserEvents2
    {
      private readonly CustomWebBrowser parent;

      public CustomWebBrowserEvent(CustomWebBrowser parent) => this.parent = parent;

      public void NavigateError(
        object pDisp,
        ref object url,
        ref object frame,
        ref object statusCode,
        ref bool cancel)
      {
        WebBrowserNavigateErrorEventArgs e = new WebBrowserNavigateErrorEventArgs(url == null ? "" : (string) url, frame == null ? "" : (string) frame, statusCode == null ? 0 : (int) statusCode, pDisp);
        this.parent.OnNavigateError(e);
        cancel = e.Cancel;
      }

      public void BeforeNavigate2(
        object pDisp,
        ref object urlObject,
        ref object flags,
        ref object targetFrameName,
        ref object postData,
        ref object headers,
        ref bool cancel)
      {
      }

      public void ClientToHostWindow(ref long cX, ref long cY)
      {
      }

      public void CommandStateChange(long command, bool enable)
      {
      }

      public void DocumentComplete(object pDisp, ref object urlObject)
      {
      }

      public void DownloadBegin()
      {
      }

      public void DownloadComplete()
      {
      }

      public void FileDownload(ref bool cancel)
      {
      }

      public void NavigateComplete2(object pDisp, ref object urlObject)
      {
      }

      public void NewWindow2(ref object ppDisp, ref bool cancel)
      {
      }

      public void OnFullScreen(bool fullScreen)
      {
      }

      public void OnMenuBar(bool menuBar)
      {
      }

      public void OnQuit()
      {
      }

      public void OnStatusBar(bool statusBar)
      {
      }

      public void OnTheaterMode(bool theaterMode)
      {
      }

      public void OnToolBar(bool toolBar)
      {
      }

      public void OnVisible(bool visible)
      {
      }

      public void PrintTemplateInstantiation(object pDisp)
      {
      }

      public void PrintTemplateTeardown(object pDisp)
      {
      }

      public void PrivacyImpactedStateChange(bool bImpacted)
      {
      }

      public void ProgressChange(int progress, int progressMax)
      {
      }

      public void PropertyChange(string szProperty)
      {
      }

      public void SetSecureLockIcon(int secureLockIcon)
      {
      }

      public void StatusTextChange(string text)
      {
      }

      public void TitleChange(string text)
      {
      }

      public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
      {
      }

      public void WindowClosing(bool isChildWindow, ref bool cancel)
      {
      }

      public void WindowSetHeight(int height)
      {
      }

      public void WindowSetLeft(int left)
      {
      }

      public void WindowSetResizable(bool resizable)
      {
      }

      public void WindowSetTop(int top)
      {
      }

      public void WindowSetWidth(int width)
      {
      }
    }
  }
}
