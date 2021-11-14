// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.WindowsFormsWebAuthenticationDialogBase
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ComVisible(true)]
  public abstract class WindowsFormsWebAuthenticationDialogBase : Form
  {
    private const int UIWidth = 566;
    private static readonly NavigateErrorStatus NavigateErrorStatus = new NavigateErrorStatus();
    private Panel webBrowserPanel;
    private readonly CustomWebBrowser webBrowser;
    private Uri desiredCallbackUri;
    protected string authenticationResult;
    protected IWin32Window ownerWindow;
    private Keys key;

    static WindowsFormsWebAuthenticationDialogBase()
    {
      if (WindowsFormsWebAuthenticationDialogBase.NativeMethods.SetQueryNetSessionCount(WindowsFormsWebAuthenticationDialogBase.NativeMethods.SessionOp.SESSION_QUERY) != 0)
        return;
      WindowsFormsWebAuthenticationDialogBase.NativeMethods.SetQueryNetSessionCount(WindowsFormsWebAuthenticationDialogBase.NativeMethods.SessionOp.SESSION_INCREMENT);
    }

    protected WindowsFormsWebAuthenticationDialogBase(object ownerWindow)
    {
      switch (ownerWindow)
      {
        case null:
          this.ownerWindow = (IWin32Window) null;
          break;
        case IWin32Window _:
          this.ownerWindow = (IWin32Window) ownerWindow;
          break;
        case IntPtr num:
          this.ownerWindow = (IWin32Window) new WindowsFormsWebAuthenticationDialogBase.WindowsFormsWin32Window()
          {
            Handle = num
          };
          break;
        default:
          throw new AdalException("invalid_owner_window_type", "Invalid owner window type. Expected types are IWin32Window or IntPtr (for window handle).");
      }
      this.webBrowser = new CustomWebBrowser();
      this.webBrowser.PreviewKeyDown += new PreviewKeyDownEventHandler(this.webBrowser_PreviewKeyDown);
      this.InitializeComponent();
    }

    private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      if (e.KeyCode != Keys.Back)
        return;
      this.key = Keys.Back;
    }

    public WebBrowser WebBrowser => (WebBrowser) this.webBrowser;

    protected virtual void WebBrowserNavigatingHandler(
      object sender,
      WebBrowserNavigatingEventArgs e)
    {
      if (this.webBrowser.IsDisposed)
      {
        Logger.Verbose((CallState) null, "We cancel all flows in disposed object and just do nothing, let object close.");
        e.Cancel = true;
      }
      else
      {
        if (this.key == Keys.Back)
        {
          this.key = Keys.None;
          e.Cancel = true;
        }
        e.Cancel = this.CheckForClosingUrl(e.Url);
        if (e.Cancel)
          return;
        Logger.Verbose((CallState) null, string.Format("Navigating to '{0}'.", (object) EncodingHelper.UrlDecode(e.Url.ToString())));
      }
    }

    private void WebBrowserNavigatedHandler(object sender, WebBrowserNavigatedEventArgs e)
    {
      if (this.CheckForClosingUrl(e.Url))
        return;
      Logger.Verbose((CallState) null, string.Format("Navigated to '{0}'.", (object) EncodingHelper.UrlDecode(e.Url.ToString())));
    }

    protected virtual void WebBrowserNavigateErrorHandler(
      object sender,
      WebBrowserNavigateErrorEventArgs e)
    {
      if (this.webBrowser.IsDisposed)
      {
        e.Cancel = true;
      }
      else
      {
        if (this.webBrowser.ActiveXInstance != e.WebBrowserActiveXInstance || e.StatusCode >= 300 && e.StatusCode < 400)
          return;
        e.Cancel = true;
        this.StopWebBrowser();
        this.OnNavigationCanceled(e.StatusCode);
      }
    }

    private bool CheckForClosingUrl(Uri url)
    {
      if (!url.Authority.Equals(this.desiredCallbackUri.Authority, StringComparison.OrdinalIgnoreCase) || !url.AbsolutePath.Equals(this.desiredCallbackUri.AbsolutePath))
        return false;
      this.authenticationResult = url.AbsoluteUri;
      this.StopWebBrowser();
      this.OnClosingUrl();
      return true;
    }

    private void StopWebBrowser()
    {
      if (this.webBrowser.IsDisposed || !this.webBrowser.IsBusy)
        return;
      Logger.Verbose((CallState) null, string.Format("WebBrowser state: IsBusy: {0}, ReadyState: {1}, Created: {2}, Disposing: {3}, IsDisposed: {4}, IsOffline: {5}", (object) this.webBrowser.IsBusy, (object) this.webBrowser.ReadyState, (object) this.webBrowser.Created, (object) this.webBrowser.Disposing, (object) this.webBrowser.IsDisposed, (object) this.webBrowser.IsOffline));
      this.webBrowser.Stop();
      Logger.Verbose((CallState) null, string.Format("WebBrowser state (after Stop): IsBusy: {0}, ReadyState: {1}, Created: {2}, Disposing: {3}, IsDisposed: {4}, IsOffline: {5}", (object) this.webBrowser.IsBusy, (object) this.webBrowser.ReadyState, (object) this.webBrowser.Created, (object) this.webBrowser.Disposing, (object) this.webBrowser.IsDisposed, (object) this.webBrowser.IsOffline));
    }

    protected abstract void OnClosingUrl();

    protected abstract void OnNavigationCanceled(int statusCode);

    public string AuthenticateAAD(Uri requestUri, Uri callbackUri)
    {
      this.desiredCallbackUri = callbackUri;
      this.authenticationResult = (string) null;
      this.webBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.WebBrowserNavigatingHandler);
      this.webBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.WebBrowserNavigatedHandler);
      this.webBrowser.NavigateError += new WebBrowserNavigateErrorEventHandler(this.WebBrowserNavigateErrorHandler);
      this.webBrowser.Navigate(requestUri);
      this.OnAuthenticate();
      return this.authenticationResult;
    }

    protected virtual void OnAuthenticate()
    {
    }

    private void InitializeComponent()
    {
      int height = (int) ((double) Math.Max((this.ownerWindow != null ? Screen.FromHandle(this.ownerWindow.Handle) : Screen.PrimaryScreen).WorkingArea.Height, 160) * 70.0 / (double) WindowsFormsWebAuthenticationDialogBase.DpiHelper.ZoomPercent);
      this.webBrowserPanel = new Panel();
      this.webBrowserPanel.SuspendLayout();
      this.SuspendLayout();
      this.webBrowser.Dock = DockStyle.Fill;
      this.webBrowser.Location = new Point(0, 25);
      this.webBrowser.MinimumSize = new Size(20, 20);
      this.webBrowser.Name = "webBrowser";
      this.webBrowser.Size = new Size(566, 565);
      this.webBrowser.TabIndex = 1;
      this.webBrowser.IsWebBrowserContextMenuEnabled = false;
      this.webBrowserPanel.Controls.Add((Control) this.webBrowser);
      this.webBrowserPanel.Dock = DockStyle.Fill;
      this.webBrowserPanel.BorderStyle = BorderStyle.None;
      this.webBrowserPanel.Location = new Point(0, 0);
      this.webBrowserPanel.Name = "webBrowserPanel";
      this.webBrowserPanel.Size = new Size(566, height);
      this.webBrowserPanel.TabIndex = 2;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(566, height);
      this.Controls.Add((Control) this.webBrowserPanel);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Name = "BrowserAuthenticationWindow";
      this.StartPosition = this.ownerWindow != null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
      this.Text = string.Empty;
      this.ShowIcon = false;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.ShowInTaskbar = null == this.ownerWindow;
      this.webBrowserPanel.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.StopWebBrowser();
      base.Dispose(disposing);
    }

    protected AdalException CreateExceptionForAuthenticationUiFailed(int statusCode)
    {
      if (WindowsFormsWebAuthenticationDialogBase.NavigateErrorStatus.Messages.ContainsKey(statusCode))
        return (AdalException) new AdalServiceException("authentication_ui_failed", string.Format("The browser based authentication dialog failed to complete. Reason: {0}", (object) WindowsFormsWebAuthenticationDialogBase.NavigateErrorStatus.Messages[statusCode]))
        {
          StatusCode = statusCode
        };
      return (AdalException) new AdalServiceException("authentication_ui_failed", string.Format("The browser based authentication dialog failed to complete for an unknown reason. StatusCode: {0}", (object) statusCode))
      {
        StatusCode = statusCode
      };
    }

    private sealed class WindowsFormsWin32Window : IWin32Window
    {
      public IntPtr Handle { get; set; }
    }

    protected static class DpiHelper
    {
      static DpiHelper()
      {
        IntPtr dc = NativeWrapper.NativeMethods.GetDC(IntPtr.Zero);
        double num1;
        double num2;
        if (dc != IntPtr.Zero)
        {
          num1 = (double) NativeWrapper.NativeMethods.GetDeviceCaps(dc, 88);
          num2 = (double) NativeWrapper.NativeMethods.GetDeviceCaps(dc, 90);
          NativeWrapper.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
        }
        else
        {
          num1 = 96.0;
          num2 = 96.0;
        }
        WindowsFormsWebAuthenticationDialogBase.DpiHelper.ZoomPercent = Math.Min((int) (100.0 * (num1 / 96.0)), (int) (100.0 * (num2 / 96.0)));
      }

      public static int ZoomPercent { get; private set; }
    }

    internal static class NativeMethods
    {
      [DllImport("IEFRAME.dll", CallingConvention = CallingConvention.StdCall)]
      internal static extern int SetQueryNetSessionCount(
        WindowsFormsWebAuthenticationDialogBase.NativeMethods.SessionOp sessionOp);

      internal enum SessionOp
      {
        SESSION_QUERY,
        SESSION_INCREMENT,
        SESSION_DECREMENT,
      }
    }
  }
}
