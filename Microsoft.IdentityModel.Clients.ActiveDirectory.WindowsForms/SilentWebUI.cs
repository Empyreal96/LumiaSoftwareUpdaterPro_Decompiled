// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.SilentWebUI
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal class SilentWebUI : WebUI, IDisposable
  {
    private const int NavigationWaitMiliSecs = 250;
    private const int NavigationOverallTimeout = 2000;
    private bool disposed;
    private WindowsFormsSynchronizationContext formsSyncContext;
    private string result;
    private Exception uiException;
    private ManualResetEvent threadInitializedEvent;
    private SilentWindowsFormsAuthenticationDialog dialog;

    public SilentWebUI() => this.threadInitializedEvent = new ManualResetEvent(false);

    ~SilentWebUI() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void WaitForCompletionOrTimeout(Thread uiThread)
    {
      long num1 = 2000;
      long ticks = DateTime.Now.Ticks;
      if (!this.threadInitializedEvent.WaitOne((int) num1))
        return;
      long num2 = (DateTime.Now.Ticks - ticks) / 10000L;
      long num3 = num1 - num2;
      if (uiThread.Join(num3 > 0L ? (int) num3 : 0))
        return;
      Logger.Information((CallState) null, "Silent login thread did not complete on time.");
      this.formsSyncContext.Post((SendOrPostCallback) (state => this.dialog.CloseBrowser()), (object) null);
    }

    private Thread StartUIThread()
    {
      Thread thread = new Thread((ThreadStart) (() =>
      {
        try
        {
          this.formsSyncContext = new WindowsFormsSynchronizationContext();
          this.dialog = new SilentWindowsFormsAuthenticationDialog(this.OwnerWindow)
          {
            NavigationWaitMiliSecs = 250
          };
          this.dialog.Done += new SilentWindowsFormsAuthenticationDialog.SilentWebUIDoneEventHandler(this.UIDoneHandler);
          this.threadInitializedEvent.Set();
          this.dialog.AuthenticateAAD(this.RequestUri, this.CallbackUri);
          Application.Run();
          this.result = this.dialog.Result;
        }
        catch (Exception ex)
        {
          this.uiException = ex;
        }
      }));
      thread.SetApartmentState(ApartmentState.STA);
      thread.IsBackground = true;
      thread.Start();
      return thread;
    }

    protected override string OnAuthenticate()
    {
      if ((Uri) null == this.CallbackUri)
        throw new InvalidOperationException("CallbackUri cannot be null");
      this.WaitForCompletionOrTimeout(this.StartUIThread());
      this.Cleanup();
      this.ThrowIfTransferredException();
      return !string.IsNullOrEmpty(this.result) ? this.result : throw new AdalException("user_interaction_required");
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
      {
        if (this.threadInitializedEvent != null)
        {
          this.threadInitializedEvent.Dispose();
          this.threadInitializedEvent = (ManualResetEvent) null;
        }
        if (this.formsSyncContext != null)
        {
          this.formsSyncContext.Dispose();
          this.formsSyncContext = (WindowsFormsSynchronizationContext) null;
        }
      }
      this.disposed = true;
    }

    private void Cleanup()
    {
      this.threadInitializedEvent.Dispose();
      this.threadInitializedEvent = (ManualResetEvent) null;
    }

    private void ThrowIfTransferredException()
    {
      if (this.uiException != null)
        throw this.uiException;
    }

    private void UIDoneHandler(object sender, SilentWebUIDoneEventArgs e)
    {
      if (this.uiException == null)
        this.uiException = e.TransferedException;
      ((Component) sender).Dispose();
      Application.ExitThread();
    }
  }
}
