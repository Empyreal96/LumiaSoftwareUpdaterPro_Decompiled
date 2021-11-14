// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SyncAsyncResult
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Data.SqlServerCe
{
  [SecurityCritical(SecurityCriticalScope.Everything)]
  [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
  internal class SyncAsyncResult : IAsyncResult
  {
    private ManualResetEvent completeEvent;
    private object callerContext;
    private AsyncCallback callerCompletionCallback;
    private OnStartTableUpload callerOnStartTableUpload;
    private OnStartTableDownload callerOnStartTableDownload;
    private OnSynchronization callerOnSynchronization;
    private SqlCeReplication callerReplobj;
    private bool isCompleted;
    private Exception exception;

    static SyncAsyncResult() => KillBitHelper.ThrowIfKillBitIsSet();

    internal SyncAsyncResult(
      SqlCeReplication replication,
      AsyncCallback completionCallback,
      object context)
    {
      this.completeEvent = new ManualResetEvent(false);
      this.callerContext = context;
      this.callerCompletionCallback = completionCallback;
      this.callerReplobj = replication;
    }

    internal SyncAsyncResult(
      SqlCeReplication replication,
      AsyncCallback completionCallback,
      OnStartTableUpload onStartTableUpload,
      OnStartTableDownload onStartTableDownload,
      OnSynchronization onSynchronization,
      object context)
    {
      this.completeEvent = new ManualResetEvent(false);
      this.callerReplobj = replication;
      this.callerCompletionCallback = completionCallback;
      this.callerOnStartTableUpload = onStartTableUpload;
      this.callerOnStartTableDownload = onStartTableDownload;
      this.callerOnSynchronization = onSynchronization;
      this.callerContext = context;
    }

    internal void SyncThread()
    {
      try
      {
        if (this.callerOnStartTableUpload == null && this.callerOnStartTableDownload == null && this.callerOnSynchronization == null)
          this.callerReplobj.Synchronize();
        else
          this.BeginSyncAndStatusReporting();
      }
      catch (Exception ex)
      {
        this.exception = ex;
      }
      this.callerCompletionCallback((IAsyncResult) this);
      this.isCompleted = true;
      this.completeEvent.Set();
    }

    private void DispatchEventToCaller(SyncStatus status, string tablename, int percentComplete)
    {
      if (SyncStatus.Uploading == status)
      {
        if (this.callerOnStartTableUpload == null)
          return;
        this.callerOnStartTableUpload((IAsyncResult) this, tablename);
      }
      else if (SyncStatus.Downloading == status)
      {
        if (this.callerOnStartTableDownload == null)
          return;
        this.callerOnStartTableDownload((IAsyncResult) this, tablename);
      }
      else
      {
        if (SyncStatus.Synchronizing != status || this.callerOnSynchronization == null)
          return;
        this.callerOnSynchronization((IAsyncResult) this, percentComplete);
      }
    }

    private void BeginSyncAndStatusReporting()
    {
      IntPtr zero = IntPtr.Zero;
      NativeMethods.CheckHRESULT(this.callerReplobj.pIErrors, NativeMethods.uwrepl_AsyncReplication(this.callerReplobj.pIReplication, ref zero));
      try
      {
        bool pCompleted;
        do
        {
          SyncStatus pSyncStatus = SyncStatus.Start;
          pCompleted = false;
          IntPtr rbzTableName = new IntPtr(0);
          string tablename = (string) null;
          int pPrecentCompleted = 0;
          int hr = NativeMethods.uwrepl_WaitForNextStatusReport(zero, ref pSyncStatus, ref rbzTableName, ref pPrecentCompleted, ref pCompleted);
          try
          {
            NativeMethods.CheckHRESULT(this.callerReplobj.pIErrors, hr);
            tablename = Marshal.PtrToStringUni(rbzTableName);
          }
          finally
          {
            NativeMethods.uwutil_SysFreeString(rbzTableName);
          }
          this.DispatchEventToCaller(pSyncStatus, tablename, pPrecentCompleted);
        }
        while (!pCompleted);
        int pHr = 0;
        NativeMethods.CheckHRESULT(this.callerReplobj.pIErrors, NativeMethods.uwrepl_GetSyncResult(zero, ref pHr));
        NativeMethods.CheckHRESULT(this.callerReplobj.pIErrors, pHr);
      }
      finally
      {
        if (IntPtr.Zero != zero)
        {
          int num = (int) NativeMethods.uwutil_ReleaseCOMPtr(zero);
        }
      }
    }

    public Exception GetException() => this.exception;

    bool IAsyncResult.IsCompleted => this.isCompleted;

    bool IAsyncResult.CompletedSynchronously => false;

    WaitHandle IAsyncResult.AsyncWaitHandle => (WaitHandle) this.completeEvent;

    object IAsyncResult.AsyncState => this.callerContext;
  }
}
