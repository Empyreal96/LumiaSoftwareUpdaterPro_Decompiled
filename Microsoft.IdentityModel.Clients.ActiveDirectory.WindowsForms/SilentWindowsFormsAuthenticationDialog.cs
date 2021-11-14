// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.SilentWindowsFormsAuthenticationDialog
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  [ComVisible(true)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SilentWindowsFormsAuthenticationDialog : WindowsFormsWebAuthenticationDialogBase
  {
    private DateTime navigationExpiry = DateTime.MaxValue;
    private Timer timer;
    private bool doneSignaled;

    internal event SilentWindowsFormsAuthenticationDialog.SilentWebUIDoneEventHandler Done;

    public int NavigationWaitMiliSecs { get; set; }

    public SilentWindowsFormsAuthenticationDialog(object ownerWindow)
      : base(ownerWindow)
    {
      this.SuppressBrowserSubDialogs();
      this.WebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.DocumentCompletedHandler);
    }

    public void CloseBrowser() => this.SignalDone();

    public string Result => this.authenticationResult;

    private void SuppressBrowserSubDialogs() => ((NativeWrapper.IWebBrowser2) this.WebBrowser.ActiveXInstance).Silent = true;

    protected override void WebBrowserNavigatingHandler(
      object sender,
      WebBrowserNavigatingEventArgs e)
    {
      if (this.timer == null)
        this.timer = SilentWindowsFormsAuthenticationDialog.CreateStartedTimer((Action) (() =>
        {
          if (!(DateTime.Now > this.navigationExpiry))
            return;
          this.OnUserInteractionRequired();
        }), this.NavigationWaitMiliSecs);
      this.navigationExpiry = DateTime.MaxValue;
      base.WebBrowserNavigatingHandler(sender, e);
    }

    private static Timer CreateStartedTimer(Action onTickAction, int interval)
    {
      Timer timer = new Timer() { Interval = interval };
      timer.Tick += (EventHandler) ((notUsedsender, notUsedEventArgs) => onTickAction());
      timer.Start();
      return timer;
    }

    private void SignalDone(Exception exception = null)
    {
      if (this.doneSignaled)
        return;
      this.timer.Stop();
      SilentWebUIDoneEventArgs args = new SilentWebUIDoneEventArgs(exception);
      if (this.Done != null)
        this.Done((object) this, args);
      this.doneSignaled = true;
    }

    private void DocumentCompletedHandler(object sender, WebBrowserDocumentCompletedEventArgs args)
    {
      this.navigationExpiry = DateTime.Now.AddMilliseconds((double) this.NavigationWaitMiliSecs);
      if (!this.HasLoginPage())
        return;
      this.OnUserInteractionRequired();
    }

    private void OnUserInteractionRequired() => this.SignalDone((Exception) new AdalException("user_interaction_required"));

    protected override void OnClosingUrl() => this.SignalDone();

    protected override void OnNavigationCanceled(int statusCode) => this.SignalDone((Exception) this.CreateExceptionForAuthenticationUiFailed(statusCode));

    private bool HasLoginPage()
    {
      HtmlDocument document = this.WebBrowser.Document;
      HtmlElement htmlElement = (HtmlElement) null;
      if ((HtmlDocument) null != document)
        htmlElement = document.GetElementsByTagName("INPUT").Cast<HtmlElement>().Where<HtmlElement>((Func<HtmlElement, bool>) (element => string.Compare(element.GetAttribute("type"), "password", true, CultureInfo.InvariantCulture) == 0 && element.Enabled && element.OffsetRectangle.Height > 0 && element.OffsetRectangle.Width > 0)).FirstOrDefault<HtmlElement>();
      return htmlElement != (HtmlElement) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.timer != null)
        this.timer.Dispose();
      base.Dispose(disposing);
    }

    internal delegate void SilentWebUIDoneEventHandler(object sender, SilentWebUIDoneEventArgs args);
  }
}
