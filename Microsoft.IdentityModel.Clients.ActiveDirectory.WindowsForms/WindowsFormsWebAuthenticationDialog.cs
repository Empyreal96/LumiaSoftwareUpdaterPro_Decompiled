// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.WindowsFormsWebAuthenticationDialog
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  [ComVisible(true)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WindowsFormsWebAuthenticationDialog : WindowsFormsWebAuthenticationDialogBase
  {
    private bool zoomed;
    private int statusCode;

    public WindowsFormsWebAuthenticationDialog(object ownerWindow)
      : base(ownerWindow)
    {
      this.Shown += new EventHandler(this.FormShownHandler);
      this.WebBrowser.DocumentTitleChanged += new EventHandler(this.WebBrowserDocumentTitleChangedHandler);
      this.WebBrowser.ObjectForScripting = (object) this;
    }

    protected override void OnAuthenticate()
    {
      this.zoomed = false;
      this.statusCode = 0;
      this.ShowBrowser();
      base.OnAuthenticate();
    }

    public void ShowBrowser()
    {
      switch (this.ShowDialog(this.ownerWindow))
      {
        case DialogResult.OK:
          break;
        case DialogResult.Cancel:
          throw new AdalException("authentication_canceled");
        default:
          throw this.CreateExceptionForAuthenticationUiFailed(this.statusCode);
      }
    }

    protected override void WebBrowserNavigatingHandler(
      object sender,
      WebBrowserNavigatingEventArgs e)
    {
      this.SetBrowserZoom();
      base.WebBrowserNavigatingHandler(sender, e);
    }

    protected override void OnClosingUrl() => this.DialogResult = DialogResult.OK;

    protected override void OnNavigationCanceled(int statusCode)
    {
      this.statusCode = statusCode;
      this.DialogResult = statusCode == 0 ? DialogResult.Cancel : DialogResult.Abort;
    }

    private void SetBrowserZoom()
    {
      int zoomPercent = WindowsFormsWebAuthenticationDialogBase.DpiHelper.ZoomPercent;
      if (!NativeWrapper.NativeMethods.IsProcessDPIAware() || 100 == zoomPercent || this.zoomed)
        return;
      this.SetBrowserControlZoom(zoomPercent - 1);
      this.SetBrowserControlZoom(zoomPercent);
      this.zoomed = true;
    }

    private void SetBrowserControlZoom(int zoomPercent)
    {
      if (!(((NativeWrapper.IWebBrowser2) this.WebBrowser.ActiveXInstance).Document is NativeWrapper.IOleCommandTarget document))
        return;
      object[] pvaIn = new object[1]{ (object) zoomPercent };
      Marshal.ThrowExceptionForHR(document.Exec(IntPtr.Zero, 63, 2, pvaIn, IntPtr.Zero));
    }

    private void FormShownHandler(object sender, EventArgs e)
    {
      if (this.Owner != null)
        return;
      this.Activate();
    }

    private void WebBrowserDocumentTitleChangedHandler(object sender, EventArgs e) => this.Text = this.WebBrowser.DocumentTitle;
  }
}
