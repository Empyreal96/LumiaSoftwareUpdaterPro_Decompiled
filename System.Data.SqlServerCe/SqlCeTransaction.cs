// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeTransaction
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Data.SqlServerCe
{
  public sealed class SqlCeTransaction : DbTransaction
  {
    private object thisLock = new object();
    private bool isZombied;
    private bool isDisposed;
    private bool isFinalized;
    internal SqlCeConnection connection;
    private IsolationLevel isolationLevel;
    private IntPtr pQpSession;
    private IntPtr pTx;
    private IntPtr pError;
    private long ullTransactionBsn;
    private Guid trackingContext;
    private SqlCeChangeTracking m_tracking;

    static SqlCeTransaction() => KillBitHelper.ThrowIfKillBitIsSet();

    protected override DbConnection DbConnection => (DbConnection) this.connection;

    internal SeTransactionFlags EngineFlags
    {
      [SecurityCritical, SecurityTreatAsSafe] get
      {
        this.EnterPublicAPI();
        SeTransactionFlags seTxFlags = SeTransactionFlags.NOFLAGS;
        this.ProcessResults(NativeMethods.GetTransactionFlags(this.ITransact, ref seTxFlags));
        return seTxFlags;
      }
      [SecurityCritical, SecurityTreatAsSafe] set
      {
        this.EnterPublicAPI();
        SeTransactionFlags seTxFlags = SeTransactionFlags.NOFLAGS;
        this.ProcessResults(NativeMethods.GetTransactionFlags(this.ITransact, ref seTxFlags));
        this.ProcessResults(NativeMethods.SetTransactionFlag(this.ITransact, seTxFlags, false, this.pError));
        this.ProcessResults(NativeMethods.SetTransactionFlag(this.ITransact, value, true, this.pError));
      }
    }

    private SqlCeConnection Connection => this.connection;

    public override IsolationLevel IsolationLevel
    {
      get
      {
        if (IntPtr.Zero == this.Connection.ITransact)
          throw new InvalidOperationException(Res.GetString("ADP_TransactionZombied", (object) this.GetType().Name));
        return this.isolationLevel;
      }
    }

    public Guid TrackingContext
    {
      get
      {
        this.EnterPublicAPI();
        return this.trackingContext;
      }
      [SecurityCritical, SecurityTreatAsSafe] set
      {
        this.EnterPublicAPI();
        if (this.trackingContext.CompareTo(value) == 0)
          return;
        IntPtr bstr = Marshal.StringToBSTR("{" + value.ToString() + "}");
        int hr = NativeMethods.SetTrackingContext(this.ITransact, ref bstr, this.pError);
        NativeMethods.uwutil_SysFreeString(bstr);
        this.trackingContext = hr != 0 ? Guid.Empty : value;
        this.ProcessResults(hr);
      }
    }

    public long CurrentTransactionBsn => this.ullTransactionBsn;

    internal bool IsZombied => this.isZombied;

    internal void SetTrackingObject(SqlCeChangeTracking trk) => this.m_tracking = trk;

    [SecurityCritical]
    internal SqlCeTransaction(
      SqlCeConnection connection,
      IsolationLevel isolationLevel,
      IntPtr pTx,
      IntPtr pQpSession)
    {
      this.pTx = pTx;
      this.pQpSession = pQpSession;
      this.isZombied = false;
      this.isDisposed = false;
      this.isFinalized = false;
      this.isolationLevel = isolationLevel;
      this.connection = connection;
      this.ullTransactionBsn = 0L;
      this.trackingContext = Guid.Empty;
      this.m_tracking = (SqlCeChangeTracking) null;
      int errorInstance = NativeMethods.CreateErrorInstance(ref this.pError);
      if (errorInstance != 0)
        this.ProcessResults(errorInstance);
      int transactionBsn = NativeMethods.GetTransactionBsn(pTx, ref this.ullTransactionBsn, this.pError);
      if (transactionBsn == 0)
        return;
      this.ProcessResults(transactionBsn);
    }

    ~SqlCeTransaction() => this.Dispose(false);

    [SecurityCritical]
    private void ReleaseNativeInterfaces()
    {
      if (IntPtr.Zero != this.pQpSession)
        NativeMethods.SafeRelease(ref this.pQpSession);
      if (IntPtr.Zero != this.pTx)
        NativeMethods.SafeRelease(ref this.pTx);
      if (!(IntPtr.Zero != this.pError))
        return;
      NativeMethods.SafeDelete(ref this.pError);
    }

    private void EnterPublicAPI()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (SqlCeTransaction));
      if (this.Connection == null || IntPtr.Zero == this.Connection.ITransact)
        this.isZombied = true;
      if (this.isZombied)
        throw new InvalidOperationException(Res.GetString("ADP_TransactionZombied", (object) this.GetType().Name));
    }

    public new void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    protected override void Dispose(bool disposing)
    {
      lock (this.thisLock)
      {
        if (this.isFinalized)
          return;
        if (disposing)
        {
          if (IntPtr.Zero != this.pTx)
            this.Rollback();
          if (this.connection != null)
            this.connection.RemoveWeakReference((object) this);
          this.isDisposed = true;
        }
        this.ReleaseNativeInterfaces();
        if (disposing)
          return;
        this.isFinalized = true;
      }
    }

    internal IntPtr ITransact => this.pTx;

    internal IntPtr IQPSession => this.pQpSession;

    public override void Commit() => this.Commit(CommitMode.Deferred);

    [SecurityTreatAsSafe]
    [SecurityCritical]
    public void Commit(CommitMode mode)
    {
      this.EnterPublicAPI();
      if (this.connection.HasOpenedCursors(this))
        throw new InvalidOperationException(Res.GetString("SQLCE_OpenedCursorsOnTxCommit"));
      try
      {
        if (this.m_tracking != null)
          this.m_tracking.Dispose(true);
        int hr = NativeMethods.CommitTransaction(this.pTx, mode, this.pError);
        this.isZombied = true;
        if (hr != 0)
          this.ProcessResults(hr);
        if (this.connection == null)
          return;
        this.connection.Zombie(this);
        this.connection.RemoveWeakReference((object) this);
      }
      finally
      {
        this.Dispose(false);
        GC.SuppressFinalize((object) this);
      }
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    public override void Rollback()
    {
      this.EnterPublicAPI();
      if (this.connection.HasOpenedCursors(this))
        throw new InvalidOperationException(Res.GetString("SQLCE_OpenedCursorsOnTxAbort"));
      try
      {
        if (this.m_tracking != null)
          this.m_tracking.Dispose(true);
        int hr = NativeMethods.AbortTransaction(this.pTx, this.pError);
        if (hr != 0)
          this.ProcessResults(hr);
        this.isZombied = true;
        this.connection.Zombie(this);
        this.connection.RemoveWeakReference((object) this);
      }
      finally
      {
        this.Dispose(false);
        GC.SuppressFinalize((object) this);
      }
    }

    [SecurityCritical]
    private void ProcessResults(int hr)
    {
      Exception exception = (Exception) this.connection.ProcessResults(hr, this.pError, (object) this);
      if (exception != null)
        throw exception;
    }
  }
}
