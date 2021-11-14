// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeChangeTracking
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Data.SqlServerCe
{
  [SecurityCritical(SecurityCriticalScope.Everything)]
  [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
  public sealed class SqlCeChangeTracking : IDisposable
  {
    private object thisLock = new object();
    private IntPtr pTracking;
    private IntPtr pError;
    private SqlCeTransaction m_transaction;
    private SqlCeConnection m_connection;
    private bool isFinalized;
    private static int trackingOptionsMask = 16;
    private TrackingVersion iTrackingVersion;
    private bool hasLocalTransaction;
    private string m_tableName;
    private List<string> m_primaryKeyColumns;
    private Dictionary<string, byte> m_numericPrecisions;
    private SqlCeTableColumns m_tableColumns;
    private TrackingKeyType m_keyType;

    static SqlCeChangeTracking() => KillBitHelper.ThrowIfKillBitIsSet();

    private SqlCeConnection Connection
    {
      get
      {
        if (this.m_connection != null)
          return this.m_connection;
        return this.m_transaction != null ? (SqlCeConnection) ((DbTransaction) this.m_transaction).Connection : (SqlCeConnection) null;
      }
    }

    internal TrackingVersion DetectTrackingType()
    {
      this.iTrackingVersion = TrackingVersion.Unknown;
      if (!this.TableExists("__sysOCSTrackedObjects"))
        return this.iTrackingVersion;
      SqlCeCommand command = this.Connection.CreateCommand();
      command.Transaction = this.m_transaction;
      command.CommandText = "select count(*) from __sysOCSTrackedObjects";
      if ((int) command.ExecuteScalar() == 0)
      {
        command.Dispose();
        return this.iTrackingVersion;
      }
      command.CommandText = "select count(*) from __sysOCSTrackedObjects where __sysTrackOpt > 0xf";
      if ((int) command.ExecuteScalar() == 0)
      {
        this.iTrackingVersion = TrackingVersion.TypeV1;
        command.Dispose();
        return this.iTrackingVersion;
      }
      command.CommandText = "select count(*) from __sysOCSTrackedObjects where __sysTrackOpt = 0xf";
      if ((int) command.ExecuteScalar() == 0)
      {
        this.iTrackingVersion = TrackingVersion.TypeV2;
        command.Dispose();
        return this.iTrackingVersion;
      }
      command.Dispose();
      this.ProcessResults(30001);
      return this.iTrackingVersion;
    }

    internal void VersionCheck(
      ref TrackingVersion iCurrentVersion,
      TrackingVersion tSupportedVersion,
      TrackingVersion tUnsupportedVersion)
    {
      if (iCurrentVersion == tUnsupportedVersion)
        this.ProcessResults(30001);
      if (iCurrentVersion != TrackingVersion.Unknown)
        return;
      iCurrentVersion = tSupportedVersion;
    }

    private void ProcessResults(int hr)
    {
      if (hr == 30001)
        throw new InvalidOperationException(Res.GetString("SQLCE_WrongTrackingVersion"));
      if (hr == 30002)
        throw new InvalidOperationException(Res.GetString("SQLCE_WrongCleanupSequence"));
      if (this.Connection == null)
        throw new NullReferenceException("Connection");
      Exception exception = this.Connection != null ? (Exception) this.Connection.ProcessResults(hr, this.pError, (object) this) : (Exception) null;
      if (exception != null)
        throw exception;
    }

    public SqlCeChangeTracking(SqlCeTransaction transaction)
    {
      if (transaction == null)
        throw new ArgumentNullException("Transaction");
      if (((DbTransaction) transaction).Connection == null)
        throw new ArgumentException(nameof (Connection));
      if (ConnectionState.Open != ((DbTransaction) transaction).Connection.State)
        throw new InvalidOperationException(Res.GetString("ADP_OpenConnectionRequired_Prepare", (object) nameof (SqlCeChangeTracking), (object) ((DbTransaction) transaction).Connection.State));
      if (transaction.IsZombied)
        throw new InvalidOperationException(Res.GetString("ADP_TransactionZombied", (object) "Transaction"));
      this.isFinalized = false;
      transaction.SetTrackingObject(this);
      NativeMethods.LoadNativeBinaries();
      int hr = NativeMethods.InitChangeTracking(transaction.ITransact, ref this.pTracking, ref this.pError);
      if (hr != 0)
        this.ProcessResults(hr);
      this.m_transaction = transaction;
      this.Connection.AddWeakReference((object) this);
      this.m_connection = (SqlCeConnection) null;
      this.hasLocalTransaction = false;
      int num = (int) this.DetectTrackingType();
    }

    private int InitChangeTracking(ref IntPtr pTracking, ref IntPtr pError)
    {
      if (!this.hasLocalTransaction)
      {
        if (this.m_transaction == null || this.m_transaction.IsZombied)
          throw new InvalidOperationException(Res.GetString("ADP_TransactionZombied", (object) "Transaction"));
        return 0;
      }
      SqlCeTransaction sqlCeTransaction = this.Connection.BeginTransaction();
      int num = NativeMethods.InitChangeTracking(sqlCeTransaction.ITransact, ref pTracking, ref pError);
      if (num == 0)
        this.m_transaction = sqlCeTransaction;
      return num;
    }

    private int ExitChangeTracking(ref IntPtr pTracking, ref IntPtr pError, bool fCommit)
    {
      if (!this.hasLocalTransaction)
        return 0;
      int num = NativeMethods.ExitChangeTracking(ref pTracking, ref pError);
      if (this.m_transaction != null && !this.m_transaction.IsZombied)
      {
        if (fCommit)
          this.m_transaction.Commit();
        else
          this.m_transaction.Rollback();
        this.m_transaction.Dispose();
        this.m_transaction = (SqlCeTransaction) null;
      }
      return num;
    }

    public SqlCeChangeTracking(SqlCeConnection connection)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (Connection));
      this.m_connection = ConnectionState.Open == connection.State ? connection : throw new InvalidOperationException(Res.GetString("ADP_OpenConnectionRequired_Prepare", (object) nameof (SqlCeChangeTracking), (object) connection.State));
      this.isFinalized = false;
      NativeMethods.LoadNativeBinaries();
      this.Connection.AddWeakReference((object) this);
      this.hasLocalTransaction = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      if (hr1 != 0)
        this.ProcessResults(hr1);
      int num = (int) this.DetectTrackingType();
      int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, true);
      if (hr2 == 0)
        return;
      this.ProcessResults(hr2);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~SqlCeChangeTracking() => this.Dispose(false);

    public void Dispose(bool disposing)
    {
      lock (this.thisLock)
      {
        if (this.isFinalized)
          return;
        if (IntPtr.Zero != this.pTracking || IntPtr.Zero != this.pError)
        {
          int hr = NativeMethods.ExitChangeTracking(ref this.pTracking, ref this.pError);
          if (hr != 0)
            this.ProcessResults(hr);
        }
        if (disposing && this.Connection != null)
        {
          this.Connection.RemoveWeakReference((object) this);
          if (this.m_transaction != null)
            this.m_transaction.SetTrackingObject((SqlCeChangeTracking) null);
          this.hasLocalTransaction = false;
          this.m_transaction = (SqlCeTransaction) null;
        }
        if (disposing)
          return;
        this.isFinalized = true;
      }
    }

    [Obsolete("Obsolete method. Use the new, public EnableTracking method instead")]
    internal void EnableTracking(
      string tableName,
      SETRACKINGTYPE trackType,
      SEOCSTRACKOPTIONS trackOpts)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV1, TrackingVersion.TypeV2);
        int hr2 = NativeMethods.EnableChangeTracking(this.pTracking, tableName, trackType, trackOpts, this.pError);
        if (hr2 == 0)
          return;
        this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
    }

    public void EnableTracking(
      string tableName,
      TrackingKeyType trackingKeyType,
      TrackingOptions trackingOptions)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
        if (trackingOptions <= TrackingOptions.None || trackingOptions >= TrackingOptions.Max)
          throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (trackingOptions), (object) 0, (object) 7));
        if (trackingKeyType <= TrackingKeyType.None || trackingKeyType >= TrackingKeyType.Max)
          throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (trackingKeyType), (object) 0, (object) 2));
        int hr2 = NativeMethods.EnableChangeTracking(this.pTracking, tableName, (SETRACKINGTYPE) trackingKeyType, (SEOCSTRACKOPTIONS) ((int) trackingOptions * SqlCeChangeTracking.trackingOptionsMask), this.pError);
        if (hr2 == 0)
          return;
        this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
    }

    public bool GetTrackingOptions(string tableName, out TrackingOptions trackingOptions)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
        SEOCSTRACKOPTIONSV2 iTrackingOptions = SEOCSTRACKOPTIONSV2.NONE;
        trackingOptions = TrackingOptions.None;
        int trackingOptions1 = NativeMethods.GetTrackingOptions(this.pTracking, tableName, ref iTrackingOptions, this.pError);
        try
        {
          if (trackingOptions1 != 0)
            this.ProcessResults(trackingOptions1);
        }
        catch (SqlCeException ex)
        {
          if (ex != null && ex.Errors != null && (ex.Errors.Count == 1 && ex.Errors[0].NativeError == 28543))
            return false;
          throw;
        }
        trackingOptions = (TrackingOptions) ((int) iTrackingOptions / SqlCeChangeTracking.trackingOptionsMask);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
      return true;
    }

    public void DisableTracking(string tableName)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        int hr2 = NativeMethods.DisableChangeTracking(this.pTracking, tableName, this.pError);
        if (hr2 == 0)
          return;
        this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
    }

    [Obsolete("Obsolete method. Use the new, public GetTrackingOptions method instead")]
    internal bool IsTableTracked(string tableName)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      bool fTableTracked = false;
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV1, TrackingVersion.TypeV2);
        int hr2 = NativeMethods.IsTableChangeTracked(this.pTracking, tableName, ref fTableTracked, this.pError);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
      return fTableTracked;
    }

    [Obsolete("Obsolete method. Use the new, public GetTrackingOptions method instead")]
    internal SETRACKINGTYPE GetTrackingType(string tableName)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      SETRACKINGTYPE trackType = SETRACKINGTYPE.INVALID;
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV1, TrackingVersion.TypeV2);
        SEOCSTRACKOPTIONS trackOptions = SEOCSTRACKOPTIONS.NONE;
        long trackOrdinal = 0;
        int changeTrackingInfo = NativeMethods.GetChangeTrackingInfo(this.pTracking, tableName, ref trackOptions, ref trackType, ref trackOrdinal, this.pError);
        if (changeTrackingInfo != 0)
          this.ProcessResults(changeTrackingInfo);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
      return trackType;
    }

    public void PurgeTombstoneTableData(string tableName, PurgeType pType, long retentionValue)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        if (retentionValue < 0L)
          throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (retentionValue), (object) 0U, (object) uint.MaxValue));
        if (pType <= PurgeType.None || pType >= PurgeType.Max)
          throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (pType), (object) 0, (object) 2));
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
        int hr2 = pType != PurgeType.TimeBased ? NativeMethods.CleanupTombstoneData(this.pTracking, tableName, 0, retentionValue, this.pError) : NativeMethods.CleanupTombstoneData(this.pTracking, tableName, (int) retentionValue, 0L, this.pError);
        if (hr2 == 0)
          return;
        this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
    }

    private void CheckIfDataIsPresentInTombstone(long retentionValue, PurgeType pType)
    {
      SqlCeCommand sqlCeCommand = this.Connection != null ? this.Connection.CreateCommand() : throw new InvalidOperationException(Res.GetString("ADP_NoConnectionString"));
      sqlCeCommand.Transaction = this.m_transaction;
      if (pType == PurgeType.CsnBased)
      {
        sqlCeCommand.CommandText = "select count(*) from __sysOCSDeletedRows where __sysDeleteTxBsn in  (select __sysTxBsn from __sysTxCommitSequence where __sysTxCsn >= 0 and __sysTxCsn < @RETENTION_VALUE ) or  (__sysDeleteTxBsn not in (select __sysTxBsn from __sysTxCommitSequence) and  __sysDeleteTxBsn < @RETENTION_VALUE )";
        sqlCeCommand.Parameters.Clear();
        sqlCeCommand.Parameters.AddWithValue("@RETENTION_VALUE", (object) retentionValue);
      }
      else
      {
        sqlCeCommand.CommandText = "select count(*) from __sysOCSDeletedRows where __sysDeletedTime < @RETENTION_DATE";
        sqlCeCommand.Parameters.Clear();
        sqlCeCommand.Parameters.AddWithValue("@RETENTION_DATE", (object) DateTime.Now.AddDays((double) (int) -retentionValue));
      }
      object obj = sqlCeCommand.ExecuteScalar();
      sqlCeCommand.Dispose();
      if (obj.ToString().Equals("0"))
        return;
      this.ProcessResults(30002);
    }

    public void PurgeTransactionSequenceData(PurgeType pType, long retentionValue)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
        if (retentionValue < 0L)
          throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (retentionValue), (object) 0U, (object) uint.MaxValue));
        if (pType <= PurgeType.None || pType >= PurgeType.Max)
          throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (pType), (object) 0, (object) 2));
        this.CheckIfDataIsPresentInTombstone(retentionValue, pType);
        int hr2 = pType != PurgeType.TimeBased ? NativeMethods.CleanupTransactionData(this.pTracking, 0, retentionValue, this.pError) : NativeMethods.CleanupTransactionData(this.pTracking, (int) retentionValue, 0L, this.pError);
        if (hr2 == 0)
          return;
        this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
    }

    public long GetLastCommittedCsn()
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      long txCsn = 0;
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
        int currentTrackingTxCsn = NativeMethods.GetCurrentTrackingTxCsn(this.pTracking, ref txCsn, this.pError);
        if (currentTrackingTxCsn != 0)
          this.ProcessResults(currentTrackingTxCsn);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
      return txCsn - 1L;
    }

    [Obsolete("Obsolete method. Use the new, public Purge methods instead")]
    internal void CleanupMetadata(
      string tableName,
      int retentionPeriod,
      long cutOffTxCsn,
      long leastTxCsn)
    {
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV1, TrackingVersion.TypeV2);
        int hr2 = NativeMethods.CleanupTrackingMetadata(this.pTracking, tableName, retentionPeriod, cutOffTxCsn, leastTxCsn, this.pError);
        if (hr2 == 0)
          return;
        this.ProcessResults(hr2);
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
    }

    [Obsolete("Obsolete method. Not to be used")]
    internal long CurrentTxBsn
    {
      get
      {
        bool fCommit = true;
        int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
        long txBsn = 0;
        try
        {
          if (hr1 != 0)
            this.ProcessResults(hr1);
          this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV1, TrackingVersion.TypeV2);
          NativeMethods.GetCurrentTrackingTxBsn(this.pTracking, ref txBsn, this.pError);
          if (hr1 != 0)
            this.ProcessResults(hr1);
        }
        catch (Exception ex)
        {
          fCommit = false;
          throw;
        }
        finally
        {
          int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
          if (hr2 != 0)
            this.ProcessResults(hr2);
        }
        return txBsn;
      }
    }

    [Obsolete("Obsolete method. Use the new, public method GetLastCommittedCsn() instead")]
    internal long CurrentTxCsn
    {
      get
      {
        bool fCommit = true;
        int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
        long txCsn = 0;
        try
        {
          if (hr1 != 0)
            this.ProcessResults(hr1);
          this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV1, TrackingVersion.TypeV2);
          int currentTrackingTxCsn = NativeMethods.GetCurrentTrackingTxCsn(this.pTracking, ref txCsn, this.pError);
          if (currentTrackingTxCsn != 0)
            this.ProcessResults(currentTrackingTxCsn);
        }
        catch (Exception ex)
        {
          fCommit = false;
          throw;
        }
        finally
        {
          int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
          if (hr2 != 0)
            this.ProcessResults(hr2);
        }
        return txCsn;
      }
    }

    internal byte GetPrecision(string tableName, string columnName)
    {
      SqlCeCommand command = this.Connection.CreateCommand();
      command.CommandText = "select numeric_precision from information_schema.columns where table_name = @TABLE_NAME and column_name = @COLUMN_NAME";
      command.Parameters.Clear();
      command.Parameters.AddWithValue("@TABLE_NAME", (object) tableName);
      command.Parameters.AddWithValue("@COLUMN_NAME", (object) columnName);
      object obj = command.ExecuteScalar();
      command.Dispose();
      return (byte) (short) obj;
    }

    internal TrackingKeyType GetTrackingKeyType(string tableName)
    {
      SqlCeCommand command = this.Connection.CreateCommand();
      command.CommandText = "select __sysTrackType from __sysOCSTrackedObjects where __sysTName = @TABLE_NAME ";
      command.Parameters.Clear();
      command.Parameters.AddWithValue("@TABLE_NAME", (object) tableName);
      object obj = command.ExecuteScalar();
      command.Dispose();
      if ((int) obj == 1)
        return TrackingKeyType.PrimaryKey;
      if ((int) obj == 2)
        return TrackingKeyType.Guid;
      throw new ArgumentException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) "trackingKeyType", (object) 0, (object) 2));
    }

    internal bool TableExists(string tableName)
    {
      SqlCeCommand command = this.Connection.CreateCommand();
      command.Transaction = this.m_transaction;
      command.CommandText = "select count(*) from information_schema.tables where table_name = @TABLE_NAME";
      command.Parameters.Clear();
      command.Parameters.AddWithValue("@TABLE_NAME", (object) tableName);
      int num = (int) command.ExecuteScalar();
      command.Dispose();
      return num > 0;
    }

    internal bool SystemCommandExecuteNonQuery(string commandText)
    {
      SqlCeCommand sqlCeCommand = (SqlCeCommand) null;
      SeTransactionFlags transactionFlags = SeTransactionFlags.NOFLAGS;
      bool fCommit = true;
      int hr1 = this.InitChangeTracking(ref this.pTracking, ref this.pError);
      try
      {
        if (hr1 != 0)
          this.ProcessResults(hr1);
        sqlCeCommand = this.Connection.CreateCommand();
        sqlCeCommand.CommandText = commandText;
        sqlCeCommand.Transaction = this.m_transaction;
        transactionFlags = this.m_transaction.EngineFlags;
        this.m_transaction.EngineFlags |= SeTransactionFlags.SYSTEM;
        sqlCeCommand.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        fCommit = false;
        throw;
      }
      finally
      {
        this.m_transaction.EngineFlags = transactionFlags;
        sqlCeCommand?.Dispose();
        int hr2 = this.ExitChangeTracking(ref this.pTracking, ref this.pError, fCommit);
        if (hr2 != 0)
          this.ProcessResults(hr2);
      }
      return true;
    }

    public static bool UpgradePublicTracking(string connectionString)
    {
      if (connectionString == null)
        throw new ArgumentNullException(nameof (connectionString));
      SqlCeConnection sqlCeConnection = (SqlCeConnection) null;
      SqlCeTransaction transaction = (SqlCeTransaction) null;
      SqlCeChangeTracking ceChangeTracking = (SqlCeChangeTracking) null;
      SqlCeCommand sqlCeCommand1 = (SqlCeCommand) null;
      SqlCeCommand sqlCeCommand2 = (SqlCeCommand) null;
      SqlCeDataReader sqlCeDataReader = (SqlCeDataReader) null;
      bool flag1 = true;
      try
      {
        sqlCeConnection = new SqlCeConnection(connectionString);
        sqlCeConnection.Open();
        transaction = sqlCeConnection.BeginTransaction();
        ceChangeTracking = new SqlCeChangeTracking(transaction);
        if (!ceChangeTracking.TableExists("__sysOCSTrackedObjects"))
          return true;
        bool flag2 = false;
        sqlCeCommand1 = sqlCeConnection.CreateCommand();
        sqlCeCommand1.CommandText = "select __sysTName, __sysTrackType, __sysTrackOpt, __sysTrackColOrd from __sysOCSTrackedObjects";
        sqlCeCommand1.Transaction = transaction;
        sqlCeDataReader = sqlCeCommand1.ExecuteReader();
        object[] values1 = new object[4];
        while (sqlCeDataReader.Read())
        {
          flag2 = true;
          sqlCeDataReader.GetValues(values1);
          flag1 = flag1 && ceChangeTracking.SystemCommandExecuteNonQuery("update __sysOCSTrackedObjects set __sysTrackOpt = 0x70 where __sysTrackOpt = 0xf and __sysTName = '" + (string) values1[0] + "'");
        }
        sqlCeDataReader.Close();
        sqlCeDataReader.Dispose();
        sqlCeDataReader = (SqlCeDataReader) null;
        sqlCeCommand1.Dispose();
        sqlCeCommand1 = (SqlCeCommand) null;
        if (!flag2)
        {
          if (ceChangeTracking.TableExists("__sysSyncArticles"))
            ceChangeTracking.SystemCommandExecuteNonQuery("drop table __sysSyncArticles");
          if (ceChangeTracking.TableExists("__sysSyncSubscriptions"))
            ceChangeTracking.SystemCommandExecuteNonQuery("drop table __sysSyncSubscriptions");
          return true;
        }
        sqlCeCommand1 = sqlCeConnection.CreateCommand();
        sqlCeCommand1.CommandText = "select table_name from information_schema.tables where table_name in (select table_name from information_schema.columns where column_name = '__sysTrackingContext') and table_name <> '__sysOCSDeletedRows' and table_name not in (select __sysTName from __sysOCSTrackedObjects)";
        sqlCeCommand1.Transaction = transaction;
        sqlCeDataReader = sqlCeCommand1.ExecuteReader();
        object[] values2 = new object[1];
        while (sqlCeDataReader.Read())
        {
          sqlCeDataReader.GetValues(values2);
          flag1 = flag1 && ceChangeTracking.SystemCommandExecuteNonQuery("alter table " + (string) values2[0] + " drop column __sysTrackingContext");
        }
        sqlCeDataReader.Close();
        sqlCeDataReader.Dispose();
        sqlCeDataReader = (SqlCeDataReader) null;
        sqlCeCommand1.Dispose();
        sqlCeCommand1 = (SqlCeCommand) null;
        if (ceChangeTracking.TableExists("__sysSyncSubscriptions"))
        {
          if (!ceChangeTracking.TableExists("__syncSubscriptions"))
          {
            sqlCeCommand2 = sqlCeConnection.CreateCommand();
            sqlCeCommand2.CommandText = "CREATE TABLE __syncSubscriptions (ClientId uniqueidentifier NOT NULL, ServerId uniqueidentifier NOT NULL, MachineId uniqueidentifier NOT NULL, CONSTRAINT __syncSubscriptions_PK PRIMARY KEY(ClientId));";
            sqlCeCommand2.Transaction = transaction;
            sqlCeCommand2.ExecuteNonQuery();
          }
          sqlCeCommand1 = sqlCeConnection.CreateCommand();
          sqlCeCommand1.Transaction = transaction;
          sqlCeCommand1.CommandText = "select * from __sysSyncSubscriptions";
          sqlCeDataReader = sqlCeCommand1.ExecuteReader();
          object[] values3 = new object[sqlCeDataReader.FieldCount];
          while (sqlCeDataReader.Read())
          {
            sqlCeDataReader.GetValues(values3);
            sqlCeCommand2.Parameters.Clear();
            sqlCeCommand2.CommandText = "insert into __syncSubscriptions (ClientID, ServerId, MachineId) values (@CID, @SID, @MID)";
            sqlCeCommand2.Parameters.AddWithValue("@CID", values3[0]);
            sqlCeCommand2.Parameters.AddWithValue("@SID", values3[1]);
            sqlCeCommand2.Parameters.AddWithValue("@MID", values3[2]);
            sqlCeCommand2.ExecuteNonQuery();
          }
          sqlCeCommand2.Parameters.Clear();
          sqlCeCommand2.Dispose();
          sqlCeCommand2 = (SqlCeCommand) null;
          sqlCeDataReader.Close();
          sqlCeDataReader.Dispose();
          sqlCeDataReader = (SqlCeDataReader) null;
          sqlCeCommand1.Dispose();
          sqlCeCommand1 = (SqlCeCommand) null;
        }
        if (ceChangeTracking.TableExists("__sysSyncArticles"))
        {
          if (!ceChangeTracking.TableExists("__syncArticles"))
          {
            sqlCeCommand2 = sqlCeConnection.CreateCommand();
            sqlCeCommand2.CommandText = "CREATE TABLE __syncArticles (TableName nvarchar(128) NOT NULL, SentAnchor image, ReceivedAnchor image, ClientId uniqueidentifier NOT NULL, CONSTRAINT __syncArticles_PK PRIMARY KEY (TableName), CONSTRAINT __syncArticles_FK FOREIGN KEY (ClientId) REFERENCES __syncSubscriptions (ClientId) ON DELETE CASCADE ON UPDATE CASCADE);";
            sqlCeCommand2.Transaction = transaction;
            sqlCeCommand2.ExecuteNonQuery();
          }
          sqlCeCommand1 = sqlCeConnection.CreateCommand();
          sqlCeCommand1.Transaction = transaction;
          sqlCeCommand1.CommandText = "select * from __sysSyncArticles";
          sqlCeDataReader = sqlCeCommand1.ExecuteReader();
          object[] values3 = new object[sqlCeDataReader.FieldCount];
          while (sqlCeDataReader.Read())
          {
            sqlCeDataReader.GetValues(values3);
            sqlCeCommand2.Parameters.Clear();
            sqlCeCommand2.CommandText = "insert into __syncArticles (TableName, SentAnchor, ReceivedAnchor, ClientID) values (@TN, @SA, @RA, @CID)";
            sqlCeCommand2.Parameters.AddWithValue("@TN", values3[0]);
            sqlCeCommand2.Parameters.AddWithValue("@SA", values3[1]);
            sqlCeCommand2.Parameters.AddWithValue("@RA", values3[2]);
            sqlCeCommand2.Parameters.AddWithValue("@CID", values3[3]);
            sqlCeCommand2.ExecuteNonQuery();
          }
          sqlCeCommand2.Parameters.Clear();
          sqlCeCommand2.Dispose();
          sqlCeCommand2 = (SqlCeCommand) null;
          sqlCeDataReader.Close();
          sqlCeDataReader.Dispose();
          sqlCeDataReader = (SqlCeDataReader) null;
          sqlCeCommand1.Dispose();
          sqlCeCommand1 = (SqlCeCommand) null;
        }
        if (ceChangeTracking.TableExists("__sysSyncArticles"))
          ceChangeTracking.SystemCommandExecuteNonQuery("drop table __sysSyncArticles");
        if (ceChangeTracking.TableExists("__sysSyncSubscriptions"))
          ceChangeTracking.SystemCommandExecuteNonQuery("drop table __sysSyncSubscriptions");
      }
      catch (Exception ex)
      {
        flag1 = false;
      }
      finally
      {
        if (sqlCeDataReader != null)
        {
          sqlCeDataReader.Close();
          sqlCeDataReader.Dispose();
        }
        if (sqlCeCommand2 != null)
        {
          sqlCeCommand2.Parameters.Clear();
          sqlCeCommand2.Dispose();
        }
        sqlCeCommand1?.Dispose();
        ceChangeTracking.Dispose();
        if (flag1)
          transaction.Commit();
        else
          transaction.Rollback();
        transaction.Dispose();
        sqlCeConnection.Close();
        sqlCeConnection.Dispose();
      }
      return flag1;
    }

    public byte[] PackTombstoneKey(string tableName, object[] columnValues)
    {
      this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
      if (tableName == null)
        throw new ArgumentNullException(nameof (tableName));
      if (columnValues == null || columnValues.Length == 0)
        throw new ArgumentNullException(nameof (columnValues));
      if (!tableName.Equals(this.m_tableName))
      {
        this.m_tableColumns = this.TableExists(tableName) ? ADP.GetTableColumns(tableName, this.Connection) : throw new ArgumentException(Res.GetString("SQLCE_NoTable", (object) tableName));
        this.m_keyType = this.GetTrackingKeyType(tableName);
        this.m_tableName = tableName;
        string tableRowGuidColumn = ADP.GetTableRowGuidColumn(tableName, this.Connection);
        if (this.m_keyType == TrackingKeyType.Guid)
        {
          this.m_primaryKeyColumns = new List<string>(1);
          this.m_primaryKeyColumns.Add(tableRowGuidColumn);
        }
        else
          this.m_primaryKeyColumns = ADP.GetTablePrimaryKey(tableName, this.Connection);
        this.m_numericPrecisions = new Dictionary<string, byte>();
        foreach (SqlCeInfoSchemaColumn tableColumn in this.m_tableColumns)
        {
          if (tableColumn.SqlCeType.SeType == SETYPE.NUMERIC)
            this.m_numericPrecisions.Add(tableColumn.ColumnName, this.GetPrecision(tableName, tableColumn.ColumnName));
        }
        if (this.m_primaryKeyColumns.Count == 0)
          throw new ArgumentException(Res.GetString("SQLCE_NoPrimaryKey", (object) tableName));
      }
      if (this.m_primaryKeyColumns.Count != columnValues.Length)
        throw new ArgumentException(Res.GetString("SQLCE_IncorrectValue", (object) nameof (columnValues), (object) columnValues.Length, (object) this.m_primaryKeyColumns.Count));
      ArrayList arrayList = new ArrayList();
      int length1 = 0;
      int count = this.m_primaryKeyColumns.Count;
      int length2 = 0;
      if (this.m_keyType != TrackingKeyType.Guid)
      {
        arrayList.Add((object) BitConverter.GetBytes(count));
        length2 = 4;
      }
      for (int index = 0; index < count; ++index)
      {
        string str = this.m_primaryKeyColumns[index].ToString();
        SqlCeInfoSchemaColumn valueByColumnName = this.m_tableColumns.GetValueByColumnName(str);
        int num = valueByColumnName.MaxLength;
        if (columnValues[index] == null)
          throw new ArgumentNullException("columnValues[" + (object) index + "]");
        if (valueByColumnName.SqlCeType.SeType == SETYPE.NCHAR || valueByColumnName.SqlCeType.SeType == SETYPE.NVARCHAR)
        {
          num = Encoding.Unicode.GetMaxByteCount(valueByColumnName.MaxLength);
          if (valueByColumnName.MaxLength > 0 && ((string) columnValues[index]).Length > valueByColumnName.MaxLength)
            throw new ArgumentException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) ("columnValues[" + (object) index + "].Length"), (object) 0, (object) valueByColumnName.MaxLength));
        }
        byte[] bytes = ADP.BitConverterGetBytes(valueByColumnName.SqlCeType, columnValues[index], ref length1);
        if (valueByColumnName.SqlCeType.SeType == SETYPE.NUMERIC)
          bytes[0] = this.m_numericPrecisions[str];
        if (num > 0 && length1 > num)
          throw new ArgumentException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) ("columnValues[" + (object) index + "].Length"), (object) 0, (object) valueByColumnName.MaxLength));
        if (this.m_keyType != TrackingKeyType.Guid)
        {
          arrayList.Add((object) BitConverter.GetBytes(length1));
          length2 += 4;
        }
        arrayList.Add((object) bytes);
        length2 += length1;
      }
      byte[] numArray1 = new byte[length2];
      int destinationIndex = 0;
      for (int index = 0; index < arrayList.Count; ++index)
      {
        byte[] numArray2 = (byte[]) arrayList[index];
        Array.Copy((Array) numArray2, 0, (Array) numArray1, destinationIndex, numArray2.Length);
        destinationIndex += numArray2.Length;
      }
      return numArray1;
    }

    public object[] UnpackTombstoneKey(string tableName, byte[] tombstoneKey)
    {
      this.VersionCheck(ref this.iTrackingVersion, TrackingVersion.TypeV2, TrackingVersion.TypeV1);
      if (tableName == null)
        throw new ArgumentNullException(nameof (tableName));
      if (tombstoneKey == null || tombstoneKey.Length == 0)
        throw new ArgumentNullException(nameof (tombstoneKey));
      if (tombstoneKey.Length <= 4)
        throw new ArgumentException(Res.GetString("SQLCE_ArgumentOutOfRange", (object) nameof (tombstoneKey), (object) 5, (object) uint.MaxValue));
      if (!tableName.Equals(this.m_tableName))
      {
        this.m_tableColumns = this.TableExists(tableName) ? ADP.GetTableColumns(tableName, this.Connection) : throw new ArgumentException(Res.GetString("SQLCE_NoTable", (object) tableName));
        this.m_keyType = this.GetTrackingKeyType(tableName);
        this.m_tableName = tableName;
        string tableRowGuidColumn = ADP.GetTableRowGuidColumn(tableName, this.Connection);
        if (this.m_keyType == TrackingKeyType.Guid)
        {
          this.m_primaryKeyColumns = new List<string>(1);
          this.m_primaryKeyColumns.Add(tableRowGuidColumn);
        }
        else
          this.m_primaryKeyColumns = ADP.GetTablePrimaryKey(tableName, this.Connection);
        if (this.m_primaryKeyColumns.Count == 0)
          throw new ArgumentException(Res.GetString("SQLCE_NoPrimaryKey", (object) tableName));
      }
      int num1 = 0;
      ArrayList arrayList = new ArrayList(this.m_primaryKeyColumns.Count);
      for (int index = 0; index < this.m_primaryKeyColumns.Count; ++index)
        arrayList.Add((object) DBNull.Value);
      int num2;
      if (this.m_keyType != TrackingKeyType.Guid)
      {
        num2 = BitConverter.ToInt32(tombstoneKey, num1);
        num1 += 4;
      }
      else
        num2 = 1;
      if (num2 != this.m_primaryKeyColumns.Count)
        throw new ArgumentException(Res.GetString("SQLCE_FormatException", (object) nameof (tombstoneKey)));
      for (int index = 0; index < num2; ++index)
      {
        int length;
        if (this.m_keyType != TrackingKeyType.Guid)
        {
          length = BitConverter.ToInt32(tombstoneKey, num1);
          num1 += 4;
        }
        else
          length = 16;
        if (length > tombstoneKey.Length - num1)
          throw new InvalidOperationException(Res.GetString("SQL_InvalidBufferSizeOrIndex", (object) num1, (object) length));
        SqlCeInfoSchemaColumn valueByColumnName = this.m_tableColumns.GetValueByColumnName(this.m_primaryKeyColumns[index].ToString());
        arrayList[index] = ADP.BitConverterGetObject(valueByColumnName.SqlCeType, tombstoneKey, num1, length);
        num1 += length;
      }
      if (num1 < tombstoneKey.Length)
        throw new ArgumentException(Res.GetString("SQLCE_FormatException", (object) nameof (tombstoneKey)));
      return arrayList.ToArray();
    }
  }
}
