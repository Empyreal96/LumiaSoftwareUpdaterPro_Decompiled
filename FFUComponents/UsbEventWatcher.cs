// Decompiled with JetBrains decompiler
// Type: FFUComponents.UsbEventWatcher
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Threading;
using System.Windows.Forms;

namespace FFUComponents
{
  internal class UsbEventWatcher : IDisposable
  {
    private ApplicationContext runningContext;

    public UsbEventWatcher(IUsbEventSink eventSink, Guid classGuid, Guid ifGuid)
    {
      UsbEventWatcher.UsbFormArgs usbFormArgs1 = new UsbEventWatcher.UsbFormArgs()
      {
        sink = eventSink,
        devClass = classGuid,
        devIf = ifGuid,
        eventWatcher = this,
        contextEvent = new ManualResetEvent(false)
      };
      new Thread((ParameterizedThreadStart) (a =>
      {
        bool flag = false;
        do
        {
          try
          {
            UsbEventWatcher.UsbFormArgs usbFormArgs2 = (UsbEventWatcher.UsbFormArgs) a;
            using (UsbEventWatcherForm eventWatcherForm = new UsbEventWatcherForm(usbFormArgs2.sink, usbFormArgs2.devClass, usbFormArgs2.devIf))
            {
              using (ApplicationContext context = new ApplicationContext((Form) eventWatcherForm))
              {
                usbFormArgs2.eventWatcher.runningContext = context;
                usbFormArgs2.contextEvent.Set();
                Application.Run(context);
                flag = true;
              }
            }
          }
          catch (Exception ex)
          {
            FFUManager.HostLogger.EventWriteThreadException(ex.ToString());
          }
        }
        while (!flag);
      })).Start((object) usbFormArgs1);
      usbFormArgs1.contextEvent.WaitOne();
    }

    private void Dispose(bool fDisposing)
    {
      if (!fDisposing)
        return;
      this.runningContext.MainForm.BeginInvoke((Delegate) (() => this.runningContext.MainForm.Close())).AsyncWaitHandle.WaitOne();
      this.runningContext.ExitThread();
      this.runningContext = (ApplicationContext) null;
    }

    public void Dispose() => this.Dispose(true);

    private struct UsbFormArgs
    {
      public IUsbEventSink sink;
      public Guid devClass;
      public Guid devIf;
      public UsbEventWatcher eventWatcher;
      public ManualResetEvent contextEvent;
    }
  }
}
