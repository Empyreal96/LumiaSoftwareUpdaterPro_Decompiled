// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeConnection
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Transactions;

namespace System.Data.SqlServerCe
{
  public sealed class SqlCeConnection : DbConnection
  {
    private const int MaxRetrialAttempts = 10;
    private const int StartSleepInterval = 100;
    private static Hashtable connStrCache;
    private SqlCeConnectionStringBuilder connTokens;
    private SqlCeDelegatedTransaction _delegatedTransaction;
    private bool isOpened;
    private bool isHostControlled;
    private bool removePwd;
    private IntPtr pStoreService;
    private IntPtr pStoreServer;
    private IntPtr pSeStore;
    private IntPtr pQpServices;
    private IntPtr pQpDatabase;
    private IntPtr pQpSession;
    private IntPtr pTx;
    private IntPtr pStoreEvents;
    private IntPtr pError;
    private string connStr;
    private string dataSource;
    private string modifiedConnStr;
    private ConnectionState state;
    private bool isDisposed;
    private SqlCeConnection.ObjectLifeTimeTracker weakReferenceCache;
    private bool isClosing;
    private int isNativeAssemblyReleased;
    private FlushFailureEventHandler flushFailureEventHandler;

    static SqlCeConnection() => KillBitHelper.ThrowIfKillBitIsSet();

    public string DatabaseIdentifier
    {
      [SecurityTreatAsSafe, SecurityCritical] get
      {
        this.CheckStateOpen("GetDatabaseInfo");
        string empty = string.Empty;
        IntPtr pwszGuidString = (IntPtr) 0;
        int databaseInstanceId = NativeMethods.GetDatabaseInstanceID(this.pSeStore, out pwszGuidString, this.pError);
        if (databaseInstanceId != 0)
          this.ProcessResults(databaseInstanceId);
        string stringBstr = Marshal.PtrToStringBSTR(pwszGuidString);
        NativeMethods.uwutil_SysFreeString(pwszGuidString);
        return stringBstr;
      }
    }

    [SecurityCritical]
    internal void OnFlushFailure(int hr, IntPtr pError)
    {
      SqlCeFlushFailureEventHandler failureEventHandler = (SqlCeFlushFailureEventHandler) this.Events[ADP.EventFlushFailure];
      if (failureEventHandler == null)
        return;
      try
      {
        failureEventHandler((object) this, new SqlCeFlushFailureEventArgs(hr, pError, (object) this));
      }
      catch (Exception ex)
      {
        if (ADP.IsCatchableExceptionType(ex))
          return;
        throw;
      }
    }

    public override string ConnectionString
    {
      get
      {
        if (this.connStr == null || this.connStr.Trim().Length == 0)
          return this.connStr = string.Empty;
        if (this.removePwd)
        {
          if (this.connTokens == null)
            return string.Empty;
          if (!(bool) this.connTokens["Persist Security Info"])
            this.connStr = ConStringUtil.RemoveKeyValuesFromString(this.connStr, "Password");
          this.removePwd = false;
        }
        return this.connStr;
      }
      set
      {
        if (this.state != ConnectionState.Closed)
          throw new InvalidOperationException(Res.GetString("ADP_OpenConnectionPropertySet", (object) nameof (ConnectionString), (object) this.state));
        Hashtable connStrCache = SqlCeConnection.connStrCache;
        if (connStrCache != null && value != null && connStrCache.Contains((object) value))
        {
          object[] objArray = (object[]) connStrCache[(object) value];
          this.modifiedConnStr = (string) objArray[0];
          if (this.state != ConnectionState.Closed)
            throw new InvalidOperationException(Res.GetString("ADP_OpenConnectionPropertySet", (object) nameof (ConnectionString), (object) this.state));
          this.connTokens = (SqlCeConnectionStringBuilder) objArray[1];
        }
        else if (value != null && value.Length > 0)
        {
          this.connTokens = new SqlCeConnectionStringBuilder(value);
          this.modifiedConnStr = value;
          if (this.connTokens != null)
            SqlCeConnection.CachedConnectionStringAdd(value, this.modifiedConnStr, this.connTokens);
          else
            this.modifiedConnStr = (string) null;
        }
        else
        {
          this.modifiedConnStr = (string) null;
          this.connTokens = (SqlCeConnectionStringBuilder) null;
        }
        this.connStr = value;
        this.removePwd = false;
        if (this.connTokens == null)
          return;
        this.dataSource = (string) this.connTokens["Data Source"];
      }
    }

    public override int ConnectionTimeout => 0;

    public override string Database => this.dataSource;

    public override string DataSource => this.dataSource;

    internal SqlCeDelegatedTransaction DelegatedTransaction
    {
      get => this._delegatedTransaction;
      set => this._delegatedTransaction = value;
    }

    internal bool HasDelegatedTransaction => null != this._delegatedTransaction;

    public override ConnectionState State => this.state;

    public override string ServerVersion => "4.0.8876.1";

    protected override DbProviderFactory DbProviderFactory => (DbProviderFactory) SqlCeProviderFactory.Instance;

    public event SqlCeInfoMessageEventHandler InfoMessage
    {
      add => this.Events.AddHandler(ADP.EventInfoMessage, (Delegate) value);
      remove => this.Events.RemoveHandler(ADP.EventInfoMessage, (Delegate) value);
    }

    public event SqlCeFlushFailureEventHandler FlushFailure
    {
      add => this.Events.AddHandler(ADP.EventFlushFailure, (Delegate) value);
      remove => this.Events.RemoveHandler(ADP.EventFlushFailure, (Delegate) value);
    }

    public override event StateChangeEventHandler StateChange
    {
      add => this.Events.AddHandler(ADP.EventStateChange, (Delegate) value);
      remove => this.Events.RemoveHandler(ADP.EventStateChange, (Delegate) value);
    }

    internal IntPtr ITransact => this.pTx;

    internal IntPtr IQPSession => this.pQpSession;

    internal IntPtr IQPServices => this.pQpServices;

    internal bool IsEnlisted => (bool) this.connTokens["Enlist"];

    [SecurityTreatAsSafe]
    [SecurityCritical]
    public override void EnlistTransaction(System.Transactions.Transaction SysTrans)
    {
      if ((System.Transactions.Transaction) null == SysTrans)
        throw new NullReferenceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (SysTrans)));
      if (!this.IsEnlisted)
        return;
      if (this.Transaction != null)
        throw new InvalidOperationException(Res.GetString("ADP_LocalTransactionPresent"));
      if (this.DelegatedTransaction == null && ConnectionState.Open == this.State)
      {
        this.Enlist(SysTrans);
        SqlCeDelegatedTransaction delegatedTransaction = this.DelegatedTransaction;
        for (int indx = 0; indx < this.weakReferenceCache.Count; ++indx)
        {
          object obj = this.weakReferenceCache.GetObject(indx);
          if (obj is SqlCeCommand)
            ((SqlCeCommand) obj).Transaction = delegatedTransaction.SqlCeTransaction;
        }
      }
      else if (!this.HasDelegatedTransaction || !(SysTrans == this.DelegatedTransaction.Transaction))
        throw new InvalidOperationException(Res.GetString("ADP_ConnectionNotEnlisted"));
    }

    [SecurityCritical]
    internal void Enlist(System.Transactions.Transaction tx)
    {
      SqlCeDelegatedTransaction delegatedTransaction = !((System.Transactions.Transaction) null == tx) ? new SqlCeDelegatedTransaction(this, tx) : throw new NullReferenceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "transaction"));
      this._delegatedTransaction = tx.EnlistPromotableSinglePhase((IPromotableSinglePhaseNotification) delegatedTransaction) ? delegatedTransaction : throw new InvalidOperationException(Res.GetString("ADP_ConnectionNotEnlisted"));
    }

    internal SqlCeTransaction Transaction
    {
      get
      {
        for (int indx = 0; indx < this.weakReferenceCache.Count; ++indx)
        {
          object obj = this.weakReferenceCache.GetObject(indx);
          if (obj is SqlCeTransaction)
          {
            SqlCeTransaction sqlCeTransaction = (SqlCeTransaction) obj;
            if (!this.HasDelegatedTransaction || sqlCeTransaction != this.DelegatedTransaction.SqlCeTransaction)
              return sqlCeTransaction;
          }
        }
        return (SqlCeTransaction) null;
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    public SqlCeConnection()
    {
      NativeMethods.LoadNativeBinaries();
      this.dataSource = string.Empty;
      this.isHostControlled = false;
      this.weakReferenceCache = new SqlCeConnection.ObjectLifeTimeTracker(true);
      NativeMethods.DllAddRef();
      this.isNativeAssemblyReleased = 0;
    }

    public SqlCeConnection(string connectionString)
      : this()
    {
      this.ConnectionString = connectionString;
    }

    ~SqlCeConnection() => this.Dispose(false);

    [SecurityCritical]
    private void ReleaseNativeInterfaces()
    {
      if (IntPtr.Zero != this.pQpSession)
        NativeMethods.SafeRelease(ref this.pQpSession);
      if (IntPtr.Zero != this.pQpDatabase)
        NativeMethods.SafeRelease(ref this.pQpDatabase);
      if (IntPtr.Zero != this.pTx)
        NativeMethods.SafeRelease(ref this.pTx);
      if (IntPtr.Zero != this.pStoreService)
        NativeMethods.SafeRelease(ref this.pStoreService);
      if (IntPtr.Zero != this.pQpServices)
        NativeMethods.SafeRelease(ref this.pQpServices);
      if (IntPtr.Zero != this.pStoreServer)
        NativeMethods.SafeRelease(ref this.pStoreServer);
      if (IntPtr.Zero != this.pStoreEvents)
        NativeMethods.SafeRelease(ref this.pStoreEvents);
      if (IntPtr.Zero != this.pError)
        NativeMethods.SafeDelete(ref this.pError);
      if (!(IntPtr.Zero != this.pSeStore))
        return;
      NativeMethods.CloseAndReleaseStore(ref this.pSeStore);
    }

    internal void DisposeSqlCeDataRdr(SqlCeTransaction tx)
    {
      if (this.weakReferenceCache == null)
        return;
      this.weakReferenceCache.CloseDataRdr(tx);
    }

    public new void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    protected override void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (this.HasDelegatedTransaction)
      {
        if (!disposing)
          return;
        this.isDisposed = true;
        this.state = ConnectionState.Closed;
        if (this.weakReferenceCache == null)
          return;
        this.weakReferenceCache.CloseDataRdr((SqlCeTransaction) null);
      }
      else
      {
        if (disposing)
        {
          if (this.isOpened)
          {
            try
            {
              this.OnStateChange(ConnectionState.Open, ConnectionState.Closed);
            }
            catch (Exception ex)
            {
              if (!ADP.IsCatchableExceptionType(ex))
                throw;
            }
          }
          if (this.weakReferenceCache != null)
          {
            this.weakReferenceCache.Close(true);
            this.weakReferenceCache = (SqlCeConnection.ObjectLifeTimeTracker) null;
          }
          this.connStr = (string) null;
          this.dataSource = (string) null;
          this.modifiedConnStr = (string) null;
          this.isOpened = false;
          this.isDisposed = true;
          this.state = ConnectionState.Closed;
        }
        if (!this.isHostControlled)
          this.ReleaseNativeInterfaces();
        if (Interlocked.Exchange(ref this.isNativeAssemblyReleased, 1) == 0)
          NativeMethods.DllRelease();
        base.Dispose(disposing);
      }
    }

    public override void Close()
    {
      this.Close(false);
      GC.KeepAlive((object) this);
    }

    internal void Zombie(SqlCeTransaction tx)
    {
      if (this.weakReferenceCache == null)
        return;
      this.weakReferenceCache.Zombie(tx);
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    private void Close(bool silent)
    {
      if (!this.isOpened || this.isClosing)
        return;
      this.isClosing = true;
      try
      {
        if (this.HasDelegatedTransaction)
        {
          this.state = ConnectionState.Closed;
          if (this.weakReferenceCache == null)
            return;
          this.weakReferenceCache.CloseDataRdr((SqlCeTransaction) null);
        }
        else
        {
          if (!silent)
            this.OnStateChange(ConnectionState.Open, ConnectionState.Closed);
          if (this.weakReferenceCache != null)
            this.weakReferenceCache.Close(false);
          this.ReleaseNativeInterfaces();
          this.isOpened = false;
          this.state = ConnectionState.Closed;
        }
      }
      finally
      {
        this.isClosing = false;
      }
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    public List<KeyValuePair<string, string>> GetDatabaseInfo()
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int locale1 = 0;
      this.CheckStateOpen(nameof (GetDatabaseInfo));
      int locale2 = NativeMethods.GetLocale(this.pSeStore, ref locale1, this.pError);
      if (locale2 != 0)
        this.ProcessResults(locale2);
      keyValuePairList.Add(new KeyValuePair<string, string>("Locale Identifier", locale1.ToString()));
      int encryptionMode1 = 0;
      int encryptionMode2 = NativeMethods.GetEncryptionMode(this.pSeStore, ref encryptionMode1, this.pError);
      if (encryptionMode2 != 0)
        this.ProcessResults(encryptionMode2);
      string str1 = (string) null;
      switch (encryptionMode1)
      {
        case 0:
          str1 = string.Empty;
          break;
        case 1:
          str1 = "Platform Default";
          break;
        case 2:
          str1 = "Engine Default";
          break;
      }
      keyValuePairList.Add(new KeyValuePair<string, string>("Encryption Mode", str1));
      int sortFlags = 0;
      int localeFlags = NativeMethods.GetLocaleFlags(this.pSeStore, ref sortFlags, this.pError);
      if (localeFlags != 0)
        this.ProcessResults(localeFlags);
      string str2 = 1 != (sortFlags & 1) ? bool.TrueString : bool.FalseString;
      keyValuePairList.Add(new KeyValuePair<string, string>("Case Sensitive", str2));
      return keyValuePairList;
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    public SqlCeTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
    {
      if (this.HasDelegatedTransaction)
        throw new InvalidOperationException(Res.GetString("ADP_ParallelTransactionsNotSupported", (object) this.GetType().Name));
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (SqlCeConnection));
      this.CheckStateOpen(nameof (BeginTransaction));
      SEISOLATION isoLevel;
      switch (isolationLevel)
      {
        case System.Data.IsolationLevel.Unspecified:
        case System.Data.IsolationLevel.ReadCommitted:
          isoLevel = SEISOLATION.ISO_READ_COMMITTED;
          break;
        case System.Data.IsolationLevel.RepeatableRead:
          isoLevel = SEISOLATION.ISO_REPEATABLE_READ;
          break;
        case System.Data.IsolationLevel.Serializable:
          isoLevel = SEISOLATION.ISO_SERIALIZABLE;
          break;
        default:
          throw new ArgumentException(Res.GetString("ADP_InvalidIsolationLevel", (object) isolationLevel.ToString()));
      }
      IntPtr zero1 = IntPtr.Zero;
      IntPtr zero2 = IntPtr.Zero;
      SqlCeTransaction sqlCeTransaction;
      try
      {
        int hr = NativeMethods.OpenTransaction(this.pSeStore, this.pQpDatabase, isoLevel, this.IQPSession, ref zero1, ref zero2, this.pError);
        if (hr != 0)
          this.ProcessResults(hr);
        sqlCeTransaction = new SqlCeTransaction(this, isolationLevel, zero1, zero2);
        this.AddWeakReference((object) sqlCeTransaction);
      }
      catch (Exception ex)
      {
        if (IntPtr.Zero != zero2)
          NativeMethods.SafeRelease(ref zero2);
        if (IntPtr.Zero != zero1)
          NativeMethods.SafeRelease(ref zero1);
        throw;
      }
      return sqlCeTransaction;
    }

    protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel) => (DbTransaction) this.BeginTransaction(isolationLevel);

    public SqlCeTransaction BeginTransaction() => this.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

    public override void ChangeDatabase(string value)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (SqlCeConnection));
      this.CheckStateOpen(nameof (ChangeDatabase));
      if (value == null || value.Trim().Length == 0)
        throw new ArgumentException(Res.GetString("ADP_EmptyDatabaseName"));
      string connectionString = this.ConnectionString;
      try
      {
        this.Close(true);
        this.ConnectionString = new SqlCeConnectionStringBuilder(this.ConnectionString)
        {
          DataSource = value
        }.ConnectionString;
        this.Open(true);
      }
      catch (Exception ex)
      {
        this.ConnectionString = connectionString;
        throw;
      }
    }

    internal void CheckStateOpen(string method)
    {
      if (ConnectionState.Open != this.State)
      {
        string name = (string) null;
        switch (method)
        {
          case "BeginTransaction":
            name = "ADP_OpenConnectionRequired_BeginTransaction";
            break;
          case "ChangeDatabase":
            name = "ADP_OpenConnectionRequired_ChangeDatabase";
            break;
          case "CommitTransaction":
            name = "ADP_OpenConnectionRequired_CommitTransaction";
            break;
          case "RollbackTransaction":
            name = "ADP_OpenConnectionRequired_RollbackTransaction";
            break;
          case "set_Connection":
            name = "ADP_OpenConnectionRequired_SetConnection";
            break;
          case "GetDatabaseInfo":
            name = "ADP_OpenConnectionRequired_GetDatabaseInfo";
            break;
        }
        throw new InvalidOperationException(Res.GetString(name, (object) method, (object) this.State));
      }
    }

    internal void AddWeakReference(object value)
    {
      if (this.isDisposed || this.weakReferenceCache == null)
        throw new ObjectDisposedException(nameof (SqlCeConnection));
      this.weakReferenceCache.Add(value);
    }

    internal void RemoveWeakReference(object value)
    {
      if (this.weakReferenceCache == null)
        throw new ObjectDisposedException(nameof (SqlCeConnection));
      this.weakReferenceCache.Remove(value);
    }

    public override DataTable GetSchema() => SchemaCollections.GetSchema(this);

    public override DataTable GetSchema(string collectionName) => SchemaCollections.GetSchema(this, collectionName);

    public override DataTable GetSchema(string collectionName, string[] restrictionValues) => SchemaCollections.GetSchema(this, collectionName, restrictionValues);

    internal bool HasOpenedCursors(SqlCeTransaction tx) => this.weakReferenceCache != null && this.weakReferenceCache.HasOpenedCursors(tx);

    protected override DbCommand CreateDbCommand() => (DbCommand) this.CreateCommand();

    public SqlCeCommand CreateCommand()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (SqlCeConnection));
      return new SqlCeCommand("", this);
    }

    private void OnStateChange(ConnectionState original, ConnectionState state)
    {
      StateChangeEventHandler changeEventHandler = (StateChangeEventHandler) this.Events[ADP.EventStateChange];
      if (changeEventHandler == null)
        return;
      try
      {
        changeEventHandler((object) this, new StateChangeEventArgs(original, state));
      }
      catch (Exception ex)
      {
        if (ADP.IsCatchableExceptionType(ex))
          return;
        throw;
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    public override void Open()
    {
      if (this.HasDelegatedTransaction)
        throw new InvalidOperationException(Res.GetString("ADP_ConnectionNotEnlisted"));
      this.Open(false);
      if (!this.IsEnlisted)
        return;
      if (!((System.Transactions.Transaction) null != System.Transactions.Transaction.Current))
        return;
      try
      {
        this.Enlist(System.Transactions.Transaction.Current);
        SqlCeDelegatedTransaction delegatedTransaction = this.DelegatedTransaction;
        for (int indx = 0; indx < this.weakReferenceCache.Count; ++indx)
        {
          object obj = this.weakReferenceCache.GetObject(indx);
          if (obj is SqlCeCommand)
            ((SqlCeCommand) obj).Transaction = delegatedTransaction.SqlCeTransaction;
        }
      }
      catch
      {
        this.Close();
        throw;
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    internal void Open(bool silent)
    {
      int num1 = -1;
      int num2 = -1;
      int num3 = -1;
      int num4 = -1;
      int num5 = -1;
      int num6 = -1;
      int num7 = -1;
      int num8 = -1;
      string source = (string) null;
      string str1 = (string) null;
      string str2 = (string) null;
      SEOPENFLAGS seopenflags = SEOPENFLAGS.MODE_READ | SEOPENFLAGS.MODE_WRITE;
      bool flag = false;
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (SqlCeConnection));
      DateTime utcNow1 = DateTime.UtcNow;
      if (this.ConnectionString == null || this.ConnectionString.Length == 0)
        throw new InvalidOperationException(Res.GetString("ADP_NoConnectionString"));
      if (this.dataSource == null || this.dataSource.Trim().Length == 0)
        throw new ArgumentException(Res.GetString("ADP_EmptyDatabaseName"));
      if (this.isOpened)
        throw new InvalidOperationException(Res.GetString("ADP_ConnectionAlreadyOpen", (object) ConnectionState.Open.ToString()));
      MEOPENINFO meopeninfo = new MEOPENINFO();
      IntPtr num9 = Marshal.AllocCoTaskMem(sizeof (MEOPENINFO));
      if (IntPtr.Zero == num9)
        throw new OutOfMemoryException();
      try
      {
        if (ADP.IsEmpty(this.modifiedConnStr))
          throw new InvalidOperationException(Res.GetString("ADP_NoConnectionString"));
        string str3 = ConStringUtil.ReplaceDataDirectory(this.dataSource);
        object connToken1 = this.connTokens["Locale Identifier"];
        if (connToken1 != null)
          num1 = (int) connToken1;
        object connToken2 = this.connTokens["Max Buffer Size"];
        if (connToken2 != null)
          num2 = (int) connToken2 * 1024;
        object connToken3 = this.connTokens["Autoshrink Threshold"];
        if (connToken3 != null)
          num3 = (int) connToken3;
        object connToken4 = this.connTokens["Max Database Size"];
        if (connToken4 != null)
          num4 = (int) connToken4 * 256;
        object connToken5 = this.connTokens["Temp File Max Size"];
        if (connToken5 != null)
          num5 = (int) connToken5 * 256;
        object connToken6 = this.connTokens["Flush Interval"];
        if (connToken6 != null)
          num8 = (int) connToken6;
        object connToken7 = this.connTokens["Default Lock Escalation"];
        if (connToken7 != null)
          num6 = (int) connToken7;
        object connToken8 = this.connTokens["Default Lock Timeout"];
        if (connToken8 != null)
          num7 = (int) connToken8;
        object connToken9 = this.connTokens["Temp File Directory"];
        if (connToken9 != null)
          str1 = (string) connToken9;
        object connToken10 = this.connTokens["Encryption Mode"];
        if (connToken10 != null)
          str2 = (string) connToken10;
        object connToken11 = this.connTokens["Password"];
        if (connToken11 != null)
        {
          string str4 = (string) connToken11;
          if (str4.Length > 0)
            source = str4;
        }
        object connToken12 = this.connTokens["Case Sensitive"];
        if (connToken12 != null)
          flag = (bool) connToken12;
        string str5 = (string) null;
        object connToken13 = this.connTokens["Mode"];
        if (connToken13 != null)
          str5 = (string) connToken13;
        int num10 = this.connTokens.FileAccessRetryTimeout * 1000;
        if (str5 != null)
        {
          switch (str5)
          {
            case "Read Only":
              seopenflags = SEOPENFLAGS.MODE_READ;
              break;
            case "Read Write":
              seopenflags = SEOPENFLAGS.MODE_READ | SEOPENFLAGS.MODE_WRITE;
              break;
            case "Exclusive":
              seopenflags = SEOPENFLAGS.MODE_READ | SEOPENFLAGS.MODE_WRITE | SEOPENFLAGS.MODE_SHARE_DENY_READ | SEOPENFLAGS.MODE_SHARE_DENY_WRITE;
              break;
            case "Shared Read":
              seopenflags = SEOPENFLAGS.MODE_READ | SEOPENFLAGS.MODE_WRITE | SEOPENFLAGS.MODE_SHARE_DENY_WRITE;
              break;
          }
        }
        FileIOPermissionAccess permissions = FileIOPermissionAccess.Read;
        if (!string.IsNullOrEmpty(str5) && !str5.Equals("Read Only", StringComparison.OrdinalIgnoreCase))
          permissions = permissions | FileIOPermissionAccess.Write | FileIOPermissionAccess.Append;
        SqlCeUtil.DemandForPermission(str3, permissions);
        if (!string.IsNullOrEmpty(str1))
          SqlCeUtil.DemandForPermission(str1, FileIOPermissionAccess.AllAccess);
        meopeninfo.pwszFileName = NativeMethods.MarshalStringToLPWSTR(str3);
        meopeninfo.pwszPassword = NativeMethods.MarshalStringToLPWSTR(source);
        meopeninfo.pwszTempPath = NativeMethods.MarshalStringToLPWSTR(str1);
        meopeninfo.lcidLocale = num1;
        meopeninfo.cbBufferPool = num2;
        meopeninfo.dwAutoShrinkPercent = num3;
        meopeninfo.dwFlushInterval = num8;
        meopeninfo.cMaxPages = num4;
        meopeninfo.cMaxTmpPages = num5;
        meopeninfo.dwDefaultTimeout = num7;
        meopeninfo.dwDefaultEscalation = num6;
        meopeninfo.dwFlags = seopenflags;
        meopeninfo.dwEncryptionMode = ConStringUtil.MapEncryptionMode(str2);
        meopeninfo.dwLocaleFlags = 0;
        if (flag)
          meopeninfo.dwLocaleFlags &= 1;
        this.flushFailureEventHandler = new FlushFailureEventHandler(this.OnFlushFailure);
        Marshal.StructureToPtr((object) meopeninfo, num9, false);
        SqlCeException sqlCeException = this.ProcessResults(NativeMethods.OpenStore(num9, Marshal.GetFunctionPointerForDelegate((Delegate) this.flushFailureEventHandler), ref this.pStoreService, ref this.pStoreServer, ref this.pQpServices, ref this.pSeStore, ref this.pTx, ref this.pQpDatabase, ref this.pQpSession, ref this.pStoreEvents, ref this.pError), this.pError, (object) this);
        if (sqlCeException != null)
        {
          if (sqlCeException.NativeError != 25035 || num10 == 0)
            throw sqlCeException;
          int millisecondsTimeout = 100;
          int num11 = 1;
          DateTime utcNow2 = DateTime.UtcNow;
          for (TimeSpan timeSpan = utcNow2 - utcNow1; sqlCeException != null && sqlCeException.NativeError == 25035 && (utcNow1 <= utcNow2 && timeSpan.TotalMilliseconds < (double) num10) && num11 <= 10; timeSpan = utcNow2 - utcNow1)
          {
            int num12 = num10 - (int) timeSpan.TotalMilliseconds;
            if (num12 < millisecondsTimeout)
              millisecondsTimeout = num12;
            Thread.Sleep(millisecondsTimeout);
            millisecondsTimeout *= 2;
            sqlCeException = this.ProcessResults(NativeMethods.OpenStore(num9, Marshal.GetFunctionPointerForDelegate((Delegate) this.flushFailureEventHandler), ref this.pStoreService, ref this.pStoreServer, ref this.pQpServices, ref this.pSeStore, ref this.pTx, ref this.pQpDatabase, ref this.pQpSession, ref this.pStoreEvents, ref this.pError), this.pError, (object) this);
            ++num11;
            utcNow2 = DateTime.UtcNow;
          }
          if (sqlCeException != null)
            throw sqlCeException;
        }
        this.removePwd = true;
        this.state = ConnectionState.Open;
        this.isOpened = true;
      }
      finally
      {
        Marshal.FreeCoTaskMem(meopeninfo.pwszFileName);
        Marshal.FreeCoTaskMem(meopeninfo.pwszPassword);
        Marshal.FreeCoTaskMem(meopeninfo.pwszTempPath);
        Marshal.FreeCoTaskMem(num9);
        if (ConnectionState.Open != this.state)
        {
          this.Close();
          this.removePwd = false;
          this.state = ConnectionState.Closed;
        }
      }
      if (silent)
        return;
      this.OnStateChange(ConnectionState.Closed, ConnectionState.Open);
    }

    private static void CachedConnectionStringAdd(
      string connStr,
      string modifiedConnStr,
      SqlCeConnectionStringBuilder connTokens)
    {
      Hashtable hashtable = SqlCeConnection.connStrCache;
      lock (typeof (SqlCeConnection))
      {
        if (hashtable == null)
        {
          hashtable = new Hashtable();
          hashtable[(object) connStr] = (object) new object[2]
          {
            (object) modifiedConnStr,
            (object) connTokens
          };
          SqlCeConnection.connStrCache = hashtable;
          return;
        }
      }
      lock (hashtable.SyncRoot)
      {
        if (hashtable.Contains((object) connStr))
          return;
        if (hashtable.Count < 250)
          hashtable[(object) connStr] = (object) new object[2]
          {
            (object) modifiedConnStr,
            (object) connTokens
          };
        else
          SqlCeConnection.connStrCache = (Hashtable) null;
      }
    }

    [SecurityCritical]
    private void ProcessResults(int hr)
    {
      Exception exception = (Exception) this.ProcessResults(hr, this.pError, (object) this);
      if (exception != null)
        throw exception;
    }

    [SecurityCritical]
    internal SqlCeException ProcessResults(int hr, IntPtr pError, object src)
    {
      if (hr == 0)
        return (SqlCeException) null;
      if (NativeMethods.Failed(hr))
      {
        Exception exception = SqlCeUtil.CreateException(pError, hr);
        return exception is SqlCeException ? (SqlCeException) exception : throw exception;
      }
      if ((object) this.Events[ADP.EventInfoMessage] != null)
      {
        SqlCeInfoMessageEventHandler messageEventHandler = (SqlCeInfoMessageEventHandler) this.Events[ADP.EventInfoMessage];
        if (messageEventHandler != null)
        {
          try
          {
            messageEventHandler((object) this, new SqlCeInfoMessageEventArgs(hr, pError, src));
          }
          catch (Exception ex)
          {
            if (!ADP.IsCatchableExceptionType(ex))
              throw;
          }
        }
      }
      else
        NativeMethods.ClearErrorInfo(pError);
      return (SqlCeException) null;
    }

    private class ObjectLifeTimeTracker : WeakReferenceCache
    {
      static ObjectLifeTimeTracker() => KillBitHelper.ThrowIfKillBitIsSet();

      internal ObjectLifeTimeTracker(bool trackResurrection)
        : base(trackResurrection)
      {
      }

      internal bool HasOpenedCursors(SqlCeTransaction tx)
      {
        lock (this)
        {
          int length = this.items.Length;
          for (int index = 0; index < length; ++index)
          {
            WeakReference weakReference = this.items[index];
            if (ADP.IsAlive(weakReference))
            {
              object target = weakReference.Target;
              if (target != null && target is SqlCeDataReader)
              {
                SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) target;
                if (tx == sqlCeDataReader.transaction && !sqlCeDataReader.IsClosed)
                  return true;
              }
            }
          }
          return false;
        }
      }

      internal void CloseDataRdr(SqlCeTransaction tx)
      {
        ArrayList arrayList = new ArrayList();
        int length = this.items.Length;
        for (int index = 0; index < length; ++index)
        {
          WeakReference weakReference = this.items[index];
          if (ADP.IsAlive(weakReference))
          {
            object target = weakReference.Target;
            if (target != null && target is SqlCeDataReader)
            {
              SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) target;
              if ((tx == null || tx == sqlCeDataReader.transaction) && !sqlCeDataReader.IsClosed)
              {
                arrayList.Add(target);
                this.items[index] = (WeakReference) null;
              }
            }
          }
        }
        foreach (SqlCeDataReader sqlCeDataReader in arrayList)
          sqlCeDataReader.Dispose();
      }

      internal void Close(bool isDisposing)
      {
        ArrayList arrayList1 = new ArrayList();
        ArrayList arrayList2 = new ArrayList();
        ArrayList arrayList3 = new ArrayList();
        ArrayList arrayList4 = new ArrayList();
        int length = this.items.Length;
        for (int index = 0; index < length; ++index)
        {
          WeakReference weakReference = this.items[index];
          if (ADP.IsAlive(weakReference))
          {
            object target = weakReference.Target;
            switch (target)
            {
              case SqlCeDataReader _:
                arrayList3.Add(target);
                this.items[index] = (WeakReference) null;
                continue;
              case SqlCeCommand _:
                arrayList2.Add(target);
                continue;
              case SqlCeTransaction _:
                arrayList1.Add(target);
                this.items[index] = (WeakReference) null;
                continue;
              case SqlCeChangeTracking _:
                arrayList4.Add(target);
                this.items[index] = (WeakReference) null;
                continue;
              default:
                continue;
            }
          }
        }
        foreach (SqlCeDataReader sqlCeDataReader in arrayList3)
          sqlCeDataReader.Dispose();
        foreach (SqlCeChangeTracking ceChangeTracking in arrayList4)
          ceChangeTracking.Dispose();
        foreach (SqlCeCommand sqlCeCommand in arrayList2)
        {
          sqlCeCommand.CloseFromConnection();
          if (isDisposing)
            sqlCeCommand.Connection = (SqlCeConnection) null;
        }
        foreach (SqlCeTransaction sqlCeTransaction in arrayList1)
          sqlCeTransaction.Dispose();
      }

      internal void Zombie(SqlCeTransaction tx)
      {
        lock (this)
        {
          int length = this.items.Length;
          for (int index = 0; index < length; ++index)
          {
            WeakReference weakReference = this.items[index];
            if (ADP.IsAlive(weakReference))
            {
              object target = weakReference.Target;
              if (target != null && target is SqlCeCommand)
              {
                if (tx == ((SqlCeCommand) target).Transaction)
                  ((SqlCeCommand) target).Transaction = (SqlCeTransaction) null;
                else if (tx == ((SqlCeCommand) target).InternalTransaction)
                  ((SqlCeCommand) target).InternalTransaction = (SqlCeTransaction) null;
              }
            }
          }
        }
      }
    }
  }
}
