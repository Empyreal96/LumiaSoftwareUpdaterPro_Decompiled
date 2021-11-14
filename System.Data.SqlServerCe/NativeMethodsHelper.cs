// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.NativeMethodsHelper
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Data.SqlServerCe
{
  internal sealed class NativeMethodsHelper : IDisposable
  {
    private UnmanagedLibraryHelper assemblyHelper;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_ZeroMemory _uwutil_ZeroMemory;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetSqlCeVersionInfo _ME_GetSqlCeVersionInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetNativeVersionInfo _ME_GetNativeVersionInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetDatabaseInstanceID _ME_GetDatabaseInstanceID;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetEncryptionMode _ME_GetEncryptionMode;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetLocale _ME_GetLocale;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetLocaleFlags _ME_GetLocaleFlags;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_OpenCursor _ME_OpenCursor;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetValues _ME_GetValues;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_Read _ME_Read;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_ReadAt _ME_ReadAt;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_Seek _ME_Seek;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetRange _ME_SetRange;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SafeRelease _ME_SafeRelease;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SafeDelete _ME_SafeDelete;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_DeleteArray _ME_DeleteArray;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_OpenStore _ME_OpenStore;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CloseStore _ME_CloseStore;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CloseAndReleaseStore _ME_CloseAndReleaseStore;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_OpenTransaction _ME_OpenTransaction;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CreateDatabase _ME_CreateDatabase;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_Rebuild _ME_Rebuild;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CreateErrorInstance _ME_CreateErrorInstance;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_ConvertToDBTIMESTAMP _uwutil_ConvertToDBTIMESTAMP;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_ConvertFromDBTIMESTAMP _uwutil_ConvertFromDBTIMESTAMP;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_SysFreeString _uwutil_SysFreeString;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_ReleaseCOMPtr _uwutil_ReleaseCOMPtr;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_get_ErrorCount _uwutil_get_ErrorCount;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_uwutil_get_Error _uwutil_get_Error;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetValues _ME_SetValues;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetValue _ME_SetValue;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_Prepare _ME_Prepare;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_InsertRecord _ME_InsertRecord;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_UpdateRecord _ME_UpdateRecord;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_DeleteRecord _ME_DeleteRecord;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GotoBookmark _ME_GotoBookmark;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetContextErrorInfo _ME_GetContextErrorInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetContextErrorMessage _ME_GetContextErrorMessage;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetMinorError _ME_GetMinorError;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetBookmark _ME_GetBookmark;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetColumnInfo _ME_GetColumnInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetColumnInfo _ME_SetColumnInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetTableInfoAsSystem _ME_SetTableInfoAsSystem;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetParameterInfo _ME_GetParameterInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetIndexColumnOrdinals _ME_GetIndexColumnOrdinals;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetKeyInfo _ME_GetKeyInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CreateCommand _ME_CreateCommand;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CompileQueryPlan _ME_CompileQueryPlan;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_Move _ME_Move;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_AbortTransaction _ME_AbortTransaction;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CommitTransaction _ME_CommitTransaction;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetTransactionFlag _ME_SetTransactionFlag;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetTransactionFlags _ME_GetTransactionFlags;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetTrackingContext _ME_GetTrackingContext;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_SetTrackingContext _ME_SetTrackingContext;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetTransactionBsn _ME_GetTransactionBsn;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_InitChangeTracking _ME_InitChangeTracking;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_ExitChangeTracking _ME_ExitChangeTracking;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_EnableChangeTracking _ME_EnableChangeTracking;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetTrackingOptions _ME_GetTrackingOptions;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_DisableChangeTracking _ME_DisableChangeTracking;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_IsTableChangeTracked _ME_IsTableChangeTracked;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetChangeTrackingInfo _ME_GetChangeTrackingInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CleanupTrackingMetadata _ME_CleanupTrackingMetadata;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CleanupTransactionData _ME_CleanupTransactionData;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_CleanupTombstoneData _ME_CleanupTombstoneData;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetCurrentTrackingTxCsn _ME_GetCurrentTrackingTxCsn;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_GetCurrentTrackingTxBsn _ME_GetCurrentTrackingTxBsn;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_DllRelease _DllRelease;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_DllAddRef _DllAddRef;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_ClearErrorInfo _ME_ClearErrorInfo;
    [SecurityCritical]
    private NativeMethodsHelper.delegate_ME_ExecuteQueryPlan _ME_ExecuteQueryPlan;
    private NativeMethodsHelper.delegate_uwrda_put_ControlReceiveTimeout _uwrda_put_ControlReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_ConnectionRetryTimeout _uwrda_get_ConnectionRetryTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_ConnectionRetryTimeout _uwrda_put_ConnectionRetryTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_CompressionLevel _uwrda_get_CompressionLevel;
    private NativeMethodsHelper.delegate_uwrda_put_CompressionLevel _uwrda_put_CompressionLevel;
    private NativeMethodsHelper.delegate_uwrda_get_ConnectionManager _uwrda_get_ConnectionManager;
    private NativeMethodsHelper.delegate_uwrda_put_ConnectionManager _uwrda_put_ConnectionManager;
    private NativeMethodsHelper.delegate_uwrda_Pull _uwrda_Pull;
    private NativeMethodsHelper.delegate_uwrda_Push _uwrda_Push;
    private NativeMethodsHelper.delegate_uwrda_SubmitSql _uwrda_SubmitSql;
    private NativeMethodsHelper.delegate_uwrda_get_InternetLogin _uwrda_get_InternetLogin;
    private NativeMethodsHelper.delegate_uwrda_put_InternetLogin _uwrda_put_InternetLogin;
    private NativeMethodsHelper.delegate_uwrda_get_InternetPassword _uwrda_get_InternetPassword;
    private NativeMethodsHelper.delegate_uwrda_put_InternetPassword _uwrda_put_InternetPassword;
    private NativeMethodsHelper.delegate_uwrda_get_InternetProxyServer _uwrda_get_InternetProxyServer;
    private NativeMethodsHelper.delegate_uwrda_put_InternetProxyServer _uwrda_put_InternetProxyServer;
    private NativeMethodsHelper.delegate_uwrda_get_InternetProxyLogin _uwrda_get_InternetProxyLogin;
    private NativeMethodsHelper.delegate_uwrda_put_InternetProxyLogin _uwrda_put_InternetProxyLogin;
    private NativeMethodsHelper.delegate_uwrda_get_InternetProxyPassword _uwrda_get_InternetProxyPassword;
    private NativeMethodsHelper.delegate_uwrda_put_InternetProxyPassword _uwrda_put_InternetProxyPassword;
    private NativeMethodsHelper.delegate_uwrda_get_ConnectTimeout _uwrda_get_ConnectTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_ConnectTimeout _uwrda_put_ConnectTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_SendTimeout _uwrda_get_SendTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_SendTimeout _uwrda_put_SendTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_ReceiveTimeout _uwrda_get_ReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_ReceiveTimeout _uwrda_put_ReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_DataSendTimeout _uwrda_get_DataSendTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_DataSendTimeout _uwrda_put_DataSendTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_DataReceiveTimeout _uwrda_get_DataReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_DataReceiveTimeout _uwrda_put_DataReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_ControlSendTimeout _uwrda_get_ControlSendTimeout;
    private NativeMethodsHelper.delegate_uwrda_put_ControlSendTimeout _uwrda_put_ControlSendTimeout;
    private NativeMethodsHelper.delegate_uwrda_get_ControlReceiveTimeout _uwrda_get_ControlReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrda_RemoteDataAccess _uwrda_RemoteDataAccess;
    private NativeMethodsHelper.delegate_uwrda_get_ErrorPointer _uwrda_get_ErrorPointer;
    private NativeMethodsHelper.delegate_uwrda_get_LocalConnectionString _uwrda_get_LocalConnectionString;
    private NativeMethodsHelper.delegate_uwrda_put_LocalConnectionString _uwrda_put_LocalConnectionString;
    private NativeMethodsHelper.delegate_uwrda_get_InternetUrl _uwrda_get_InternetUrl;
    private NativeMethodsHelper.delegate_uwrda_put_InternetUrl _uwrda_put_InternetUrl;
    private NativeMethodsHelper.delegate_uwrepl_put_ConnectionRetryTimeout _uwrepl_put_ConnectionRetryTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_CompressionLevel _uwrepl_get_CompressionLevel;
    private NativeMethodsHelper.delegate_uwrepl_put_CompressionLevel _uwrepl_put_CompressionLevel;
    private NativeMethodsHelper.delegate_uwrepl_get_ConnectionManager _uwrepl_get_ConnectionManager;
    private NativeMethodsHelper.delegate_uwrepl_put_ConnectionManager _uwrepl_put_ConnectionManager;
    private NativeMethodsHelper.delegate_uwrepl_get_SnapshotTransferType _uwrepl_get_SnapshotTransferType;
    private NativeMethodsHelper.delegate_uwrepl_put_SnapshotTransferType _uwrepl_put_SnapshotTransferType;
    private NativeMethodsHelper.delegate_uwrepl_AddSubscription _uwrepl_AddSubscription;
    private NativeMethodsHelper.delegate_uwrepl_DropSubscription _uwrepl_DropSubscription;
    private NativeMethodsHelper.delegate_uwrepl_ReinitializeSubscription _uwrepl_ReinitializeSubscription;
    private NativeMethodsHelper.delegate_uwrepl_Initialize _uwrepl_Initialize;
    private NativeMethodsHelper.delegate_uwrepl_Run _uwrepl_Run;
    private NativeMethodsHelper.delegate_uwrepl_Terminate _uwrepl_Terminate;
    private NativeMethodsHelper.delegate_uwrepl_Cancel _uwrepl_Cancel;
    private NativeMethodsHelper.delegate_uwrepl_LoadProperties _uwrepl_LoadProperties;
    private NativeMethodsHelper.delegate_uwrepl_SaveProperties _uwrepl_SaveProperties;
    private NativeMethodsHelper.delegate_uwrepl_get_Subscriber _uwrepl_get_Subscriber;
    private NativeMethodsHelper.delegate_uwrepl_put_Subscriber _uwrepl_put_Subscriber;
    private NativeMethodsHelper.delegate_uwrepl_get_SubscriberConnectionString _uwrepl_get_SubscriberConnectionString;
    private NativeMethodsHelper.delegate_uwrepl_put_SubscriberConnectionString _uwrepl_put_SubscriberConnectionString;
    private NativeMethodsHelper.delegate_uwrepl_get_SubscriberChanges _uwrepl_get_SubscriberChanges;
    private NativeMethodsHelper.delegate_uwrepl_get_SubscriberConflicts _uwrepl_get_SubscriberConflicts;
    private NativeMethodsHelper.delegate_uwrepl_get_Validate _uwrepl_get_Validate;
    private NativeMethodsHelper.delegate_uwrepl_put_Validate _uwrepl_put_Validate;
    private NativeMethodsHelper.delegate_uwrepl_get_ConnectTimeout _uwrepl_get_ConnectTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_ConnectTimeout _uwrepl_put_ConnectTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_SendTimeout _uwrepl_get_SendTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_SendTimeout _uwrepl_put_SendTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_ReceiveTimeout _uwrepl_get_ReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_ReceiveTimeout _uwrepl_put_ReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_DataSendTimeout _uwrepl_get_DataSendTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_DataSendTimeout _uwrepl_put_DataSendTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_DataReceiveTimeout _uwrepl_get_DataReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_DataReceiveTimeout _uwrepl_put_DataReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_ControlSendTimeout _uwrepl_get_ControlSendTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_ControlSendTimeout _uwrepl_put_ControlSendTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_ControlReceiveTimeout _uwrepl_get_ControlReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_ControlReceiveTimeout _uwrepl_put_ControlReceiveTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_ConnectionRetryTimeout _uwrepl_get_ConnectionRetryTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_LoginTimeout _uwrepl_put_LoginTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_ProfileName _uwrepl_get_ProfileName;
    private NativeMethodsHelper.delegate_uwrepl_put_ProfileName _uwrepl_put_ProfileName;
    private NativeMethodsHelper.delegate_uwrepl_get_Publisher _uwrepl_get_Publisher;
    private NativeMethodsHelper.delegate_uwrepl_put_Publisher _uwrepl_put_Publisher;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherNetwork _uwrepl_get_PublisherNetwork;
    private NativeMethodsHelper.delegate_uwrepl_put_PublisherNetwork _uwrepl_put_PublisherNetwork;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherAddress _uwrepl_get_PublisherAddress;
    private NativeMethodsHelper.delegate_uwrepl_put_PublisherAddress _uwrepl_put_PublisherAddress;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherSecurityMode _uwrepl_get_PublisherSecurityMode;
    private NativeMethodsHelper.delegate_uwrepl_put_PublisherSecurityMode _uwrepl_put_PublisherSecurityMode;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherLogin _uwrepl_get_PublisherLogin;
    private NativeMethodsHelper.delegate_uwrepl_put_PublisherLogin _uwrepl_put_PublisherLogin;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherPassword _uwrepl_get_PublisherPassword;
    private NativeMethodsHelper.delegate_uwrepl_put_PublisherPassword _uwrepl_put_PublisherPassword;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherDatabase _uwrepl_get_PublisherDatabase;
    private NativeMethodsHelper.delegate_uwrepl_put_PublisherDatabase _uwrepl_put_PublisherDatabase;
    private NativeMethodsHelper.delegate_uwrepl_get_Publication _uwrepl_get_Publication;
    private NativeMethodsHelper.delegate_uwrepl_put_Publication _uwrepl_put_Publication;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherChanges _uwrepl_get_PublisherChanges;
    private NativeMethodsHelper.delegate_uwrepl_get_PublisherConflicts _uwrepl_get_PublisherConflicts;
    private NativeMethodsHelper.delegate_uwrepl_get_QueryTimeout _uwrepl_get_QueryTimeout;
    private NativeMethodsHelper.delegate_uwrepl_put_QueryTimeout _uwrepl_put_QueryTimeout;
    private NativeMethodsHelper.delegate_uwrepl_get_DistributorSecurityMode _uwrepl_get_DistributorSecurityMode;
    private NativeMethodsHelper.delegate_uwrepl_put_DistributorSecurityMode _uwrepl_put_DistributorSecurityMode;
    private NativeMethodsHelper.delegate_uwrepl_get_DistributorLogin _uwrepl_get_DistributorLogin;
    private NativeMethodsHelper.delegate_uwrepl_put_DistributorLogin _uwrepl_put_DistributorLogin;
    private NativeMethodsHelper.delegate_uwrepl_get_DistributorPassword _uwrepl_get_DistributorPassword;
    private NativeMethodsHelper.delegate_uwrepl_put_DistributorPassword _uwrepl_put_DistributorPassword;
    private NativeMethodsHelper.delegate_uwrepl_get_ExchangeType _uwrepl_get_ExchangeType;
    private NativeMethodsHelper.delegate_uwrepl_put_ExchangeType _uwrepl_put_ExchangeType;
    private NativeMethodsHelper.delegate_uwrepl_get_HostName _uwrepl_get_HostName;
    private NativeMethodsHelper.delegate_uwrepl_put_HostName _uwrepl_put_HostName;
    private NativeMethodsHelper.delegate_uwrepl_get_InternetUrl _uwrepl_get_InternetUrl;
    private NativeMethodsHelper.delegate_uwrepl_put_InternetUrl _uwrepl_put_InternetUrl;
    private NativeMethodsHelper.delegate_uwrepl_get_InternetLogin _uwrepl_get_InternetLogin;
    private NativeMethodsHelper.delegate_uwrepl_put_InternetLogin _uwrepl_put_InternetLogin;
    private NativeMethodsHelper.delegate_uwrepl_get_InternetPassword _uwrepl_get_InternetPassword;
    private NativeMethodsHelper.delegate_uwrepl_put_InternetPassword _uwrepl_put_InternetPassword;
    private NativeMethodsHelper.delegate_uwrepl_get_InternetProxyServer _uwrepl_get_InternetProxyServer;
    private NativeMethodsHelper.delegate_uwrepl_put_InternetProxyServer _uwrepl_put_InternetProxyServer;
    private NativeMethodsHelper.delegate_uwrepl_get_InternetProxyLogin _uwrepl_get_InternetProxyLogin;
    private NativeMethodsHelper.delegate_uwrepl_put_InternetProxyLogin _uwrepl_put_InternetProxyLogin;
    private NativeMethodsHelper.delegate_uwrepl_get_InternetProxyPassword _uwrepl_get_InternetProxyPassword;
    private NativeMethodsHelper.delegate_uwrepl_put_InternetProxyPassword _uwrepl_put_InternetProxyPassword;
    private NativeMethodsHelper.delegate_uwrepl_get_LoginTimeout _uwrepl_get_LoginTimeout;
    private NativeMethodsHelper.delegate_uwrepl_Replication _uwrepl_Replication;
    private NativeMethodsHelper.delegate_uwrepl_get_ErrorPointer _uwrepl_get_ErrorPointer;
    private NativeMethodsHelper.delegate_uwrepl_get_Distributor _uwrepl_get_Distributor;
    private NativeMethodsHelper.delegate_uwrepl_put_Distributor _uwrepl_put_Distributor;
    private NativeMethodsHelper.delegate_uwrepl_get_PostSyncCleanup _uwrepl_get_PostSyncCleanup;
    private NativeMethodsHelper.delegate_uwrepl_put_PostSyncCleanup _uwrepl_put_PostSyncCleanup;
    private NativeMethodsHelper.delegate_uwrepl_get_DistributorNetwork _uwrepl_get_DistributorNetwork;
    private NativeMethodsHelper.delegate_uwrepl_put_DistributorNetwork _uwrepl_put_DistributorNetwork;
    private NativeMethodsHelper.delegate_uwrepl_get_DistributorAddress _uwrepl_get_DistributorAddress;
    private NativeMethodsHelper.delegate_uwrepl_put_DistributorAddress _uwrepl_put_DistributorAddress;
    private NativeMethodsHelper.delegate_uwrepl_AsyncReplication _uwrepl_AsyncReplication;
    private NativeMethodsHelper.delegate_uwrepl_WaitForNextStatusReport _uwrepl_WaitForNextStatusReport;
    private NativeMethodsHelper.delegate_uwrepl_GetSyncResult _uwrepl_GetSyncResult;

    static NativeMethodsHelper() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal NativeMethodsHelper(string modulePath) => this.assemblyHelper = new UnmanagedLibraryHelper(modulePath);

    [SecurityCritical]
    public void Dispose() => this.assemblyHelper.Dispose();

    [SecurityCritical]
    internal void uwutil_ZeroMemory(IntPtr dest, int length)
    {
      if (this._uwutil_ZeroMemory == null)
        this._uwutil_ZeroMemory = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_ZeroMemory>(nameof (uwutil_ZeroMemory));
      this._uwutil_ZeroMemory(dest, length);
    }

    [SecurityCritical]
    internal int GetSqlCeVersionInfo(ref IntPtr pwszVersion)
    {
      if (this._ME_GetSqlCeVersionInfo == null)
        this._ME_GetSqlCeVersionInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetSqlCeVersionInfo>("ME_GetSqlCeVersionInfo");
      return this._ME_GetSqlCeVersionInfo(ref pwszVersion);
    }

    [SecurityCritical]
    internal int GetNativeVersionInfo(ref int bldMajor, ref int bldMinor)
    {
      if (this._ME_GetNativeVersionInfo == null)
        this._ME_GetNativeVersionInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetNativeVersionInfo>("ME_GetNativeVersionInfo");
      return this._ME_GetNativeVersionInfo(ref bldMajor, ref bldMinor);
    }

    [SecurityCritical]
    internal int GetDatabaseInstanceID(IntPtr pStore, out IntPtr pwszGuidString, IntPtr pError)
    {
      if (this._ME_GetDatabaseInstanceID == null)
        this._ME_GetDatabaseInstanceID = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetDatabaseInstanceID>("ME_GetDatabaseInstanceID");
      return this._ME_GetDatabaseInstanceID(pStore, out pwszGuidString, pError);
    }

    [SecurityCritical]
    internal int GetEncryptionMode(IntPtr pStore, ref int encryptionMode, IntPtr pError)
    {
      if (this._ME_GetEncryptionMode == null)
        this._ME_GetEncryptionMode = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetEncryptionMode>("ME_GetEncryptionMode");
      return this._ME_GetEncryptionMode(pStore, ref encryptionMode, pError);
    }

    [SecurityCritical]
    internal int GetLocale(IntPtr pStore, ref int locale, IntPtr pError)
    {
      if (this._ME_GetLocale == null)
        this._ME_GetLocale = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetLocale>("ME_GetLocale");
      return this._ME_GetLocale(pStore, ref locale, pError);
    }

    [SecurityCritical]
    internal int GetLocaleFlags(IntPtr pStore, ref int sortFlags, IntPtr pError)
    {
      if (this._ME_GetLocaleFlags == null)
        this._ME_GetLocaleFlags = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetLocaleFlags>("ME_GetLocaleFlags");
      return this._ME_GetLocaleFlags(pStore, ref sortFlags, pError);
    }

    [SecurityCritical]
    internal int OpenCursor(
      IntPtr pITransact,
      IntPtr pwszTableName,
      IntPtr pwszIndexName,
      ref IntPtr pSeCursor,
      IntPtr pError)
    {
      if (this._ME_OpenCursor == null)
        this._ME_OpenCursor = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_OpenCursor>("ME_OpenCursor");
      return this._ME_OpenCursor(pITransact, pwszTableName, pwszIndexName, ref pSeCursor, pError);
    }

    [SecurityCritical]
    internal int GetValues(
      IntPtr pSeCursor,
      int seGetColumn,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      IntPtr pError)
    {
      if (this._ME_GetValues == null)
        this._ME_GetValues = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetValues>("ME_GetValues");
      return this._ME_GetValues(pSeCursor, seGetColumn, prgBinding, cDbBinding, pData, pError);
    }

    [SecurityCritical]
    internal unsafe int Read(
      IntPtr pSeqStream,
      void* pBuffer,
      int bufferIndex,
      int byteCount,
      out int bytesRead,
      IntPtr pError)
    {
      if (this._ME_Read == null)
        this._ME_Read = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_Read>("ME_Read");
      return this._ME_Read(pSeqStream, pBuffer, bufferIndex, byteCount, out bytesRead, pError);
    }

    [SecurityCritical]
    internal unsafe int ReadAt(
      IntPtr pLockBytes,
      int srcIndex,
      void* pBuffer,
      int bufferIndex,
      int byteCount,
      out int bytesRead,
      IntPtr pError)
    {
      if (this._ME_ReadAt == null)
        this._ME_ReadAt = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_ReadAt>("ME_ReadAt");
      return this._ME_ReadAt(pLockBytes, srcIndex, pBuffer, bufferIndex, byteCount, out bytesRead, pError);
    }

    [SecurityCritical]
    internal int Seek(
      IntPtr pSeCursor,
      IntPtr pQpServices,
      IntPtr prgBinding,
      int cBinding,
      IntPtr pData,
      int cKeyValues,
      int dbSeekOptions,
      IntPtr pError)
    {
      if (this._ME_Seek == null)
        this._ME_Seek = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_Seek>("ME_Seek");
      return this._ME_Seek(pSeCursor, pQpServices, prgBinding, cBinding, pData, cKeyValues, dbSeekOptions, pError);
    }

    [SecurityCritical]
    internal int SetRange(
      IntPtr pSeCursor,
      IntPtr pQpServices,
      IntPtr prgBinding,
      int cBinding,
      IntPtr pStartData,
      int cStartKeyValues,
      IntPtr pEndData,
      int cEndKeyValues,
      int dbRangeOptions,
      IntPtr pError)
    {
      if (this._ME_SetRange == null)
        this._ME_SetRange = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetRange>("ME_SetRange");
      return this._ME_SetRange(pSeCursor, pQpServices, prgBinding, cBinding, pStartData, cStartKeyValues, pEndData, cEndKeyValues, dbRangeOptions, pError);
    }

    [SecurityCritical]
    internal int SafeRelease(ref IntPtr ppUnknown)
    {
      if (this._ME_SafeRelease == null)
        this._ME_SafeRelease = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SafeRelease>("ME_SafeRelease");
      return this._ME_SafeRelease(ref ppUnknown);
    }

    [SecurityCritical]
    internal int SafeDelete(ref IntPtr ppInstance)
    {
      if (this._ME_SafeDelete == null)
        this._ME_SafeDelete = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SafeDelete>("ME_SafeDelete");
      return this._ME_SafeDelete(ref ppInstance);
    }

    [SecurityCritical]
    internal int DeleteArray(ref IntPtr ppInstance)
    {
      if (this._ME_DeleteArray == null)
        this._ME_DeleteArray = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_DeleteArray>("ME_DeleteArray");
      return this._ME_DeleteArray(ref ppInstance);
    }

    [SecurityCritical]
    internal int OpenStore(
      IntPtr pOpenInfo,
      IntPtr pfnOnFlushFailure,
      ref IntPtr pStoreService,
      ref IntPtr pStoreServer,
      ref IntPtr pQpServices,
      ref IntPtr pSeStore,
      ref IntPtr pTx,
      ref IntPtr pQpDatabase,
      ref IntPtr pQpSession,
      ref IntPtr pStoreEvents,
      ref IntPtr pError)
    {
      if (this._ME_OpenStore == null)
        this._ME_OpenStore = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_OpenStore>("ME_OpenStore");
      return this._ME_OpenStore(pOpenInfo, pfnOnFlushFailure, ref pStoreService, ref pStoreServer, ref pQpServices, ref pSeStore, ref pTx, ref pQpDatabase, ref pQpSession, ref pStoreEvents, ref pError);
    }

    [SecurityCritical]
    internal int CloseStore(IntPtr pSeStore)
    {
      if (this._ME_CloseStore == null)
        this._ME_CloseStore = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CloseStore>("ME_CloseStore");
      return this._ME_CloseStore(pSeStore);
    }

    [SecurityCritical]
    internal int CloseAndReleaseStore(ref IntPtr pSeStore)
    {
      if (this._ME_CloseAndReleaseStore == null)
        this._ME_CloseAndReleaseStore = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CloseAndReleaseStore>("ME_CloseAndReleaseStore");
      return this._ME_CloseAndReleaseStore(ref pSeStore);
    }

    [SecurityCritical]
    internal int OpenTransaction(
      IntPtr pSeStore,
      IntPtr pQpDatabase,
      SEISOLATION isoLevel,
      IntPtr pQpConnSession,
      ref IntPtr pTx,
      ref IntPtr pQpSession,
      IntPtr pError)
    {
      if (this._ME_OpenTransaction == null)
        this._ME_OpenTransaction = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_OpenTransaction>("ME_OpenTransaction");
      return this._ME_OpenTransaction(pSeStore, pQpDatabase, isoLevel, pQpConnSession, ref pTx, ref pQpSession, pError);
    }

    [SecurityCritical]
    internal int CreateDatabase(IntPtr pOpenInfo, ref IntPtr pError)
    {
      if (this._ME_CreateDatabase == null)
        this._ME_CreateDatabase = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CreateDatabase>("ME_CreateDatabase");
      return this._ME_CreateDatabase(pOpenInfo, ref pError);
    }

    [SecurityCritical]
    internal int Rebuild(
      IntPtr pwszSrc,
      IntPtr pwszDst,
      IntPtr pwszTemp,
      IntPtr pwszPwd,
      IntPtr pwszPwdNew,
      int fEncrypt,
      SEFIXOPTION tyOption,
      int fSafeRepair,
      int lcid,
      int dstEncryptionMode,
      int localeFlags,
      ref IntPtr pError)
    {
      if (this._ME_Rebuild == null)
        this._ME_Rebuild = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_Rebuild>("ME_Rebuild");
      return this._ME_Rebuild(pwszSrc, pwszDst, pwszTemp, pwszPwd, pwszPwdNew, fEncrypt, tyOption, fSafeRepair, lcid, dstEncryptionMode, localeFlags, ref pError);
    }

    [SecurityCritical]
    internal int CreateErrorInstance(ref IntPtr pError)
    {
      if (this._ME_CreateErrorInstance == null)
        this._ME_CreateErrorInstance = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CreateErrorInstance>("ME_CreateErrorInstance");
      return this._ME_CreateErrorInstance(ref pError);
    }

    [SecurityCritical]
    internal int uwutil_ConvertToDBTIMESTAMP(ref DBTIMESTAMP pDbTimestamp, uint dtTime, int dtDay)
    {
      if (this._uwutil_ConvertToDBTIMESTAMP == null)
        this._uwutil_ConvertToDBTIMESTAMP = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_ConvertToDBTIMESTAMP>(nameof (uwutil_ConvertToDBTIMESTAMP));
      return this._uwutil_ConvertToDBTIMESTAMP(ref pDbTimestamp, dtTime, dtDay);
    }

    [SecurityCritical]
    internal int uwutil_ConvertFromDBTIMESTAMP(
      DBTIMESTAMP pDbTimestamp,
      ref uint dtTime,
      ref int dtDay)
    {
      if (this._uwutil_ConvertFromDBTIMESTAMP == null)
        this._uwutil_ConvertFromDBTIMESTAMP = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_ConvertFromDBTIMESTAMP>(nameof (uwutil_ConvertFromDBTIMESTAMP));
      return this._uwutil_ConvertFromDBTIMESTAMP(pDbTimestamp, ref dtTime, ref dtDay);
    }

    [SecurityCritical]
    internal void uwutil_SysFreeString(IntPtr p)
    {
      if (this._uwutil_SysFreeString == null)
        this._uwutil_SysFreeString = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_SysFreeString>(nameof (uwutil_SysFreeString));
      this._uwutil_SysFreeString(p);
    }

    [SecurityCritical]
    internal uint uwutil_ReleaseCOMPtr(IntPtr p)
    {
      if (this._uwutil_ReleaseCOMPtr == null)
        this._uwutil_ReleaseCOMPtr = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_ReleaseCOMPtr>(nameof (uwutil_ReleaseCOMPtr));
      return this._uwutil_ReleaseCOMPtr(p);
    }

    [SecurityCritical]
    internal int uwutil_get_ErrorCount(IntPtr pIRDA)
    {
      if (this._uwutil_get_ErrorCount == null)
        this._uwutil_get_ErrorCount = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_get_ErrorCount>(nameof (uwutil_get_ErrorCount));
      return this._uwutil_get_ErrorCount(pIRDA);
    }

    [SecurityCritical]
    internal int uwutil_get_Error(
      IntPtr pIError,
      int errno,
      out int hResult,
      out IntPtr message,
      out int nativeError,
      out IntPtr source,
      out int numericParameter1,
      out int numericParameter2,
      out int numericParameter3,
      out IntPtr errorParameter1,
      out IntPtr errorParameter2,
      out IntPtr errorParameter3)
    {
      if (this._uwutil_get_Error == null)
        this._uwutil_get_Error = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwutil_get_Error>(nameof (uwutil_get_Error));
      return this._uwutil_get_Error(pIError, errno, out hResult, out message, out nativeError, out source, out numericParameter1, out numericParameter2, out numericParameter3, out errorParameter1, out errorParameter2, out errorParameter3);
    }

    [SecurityCritical]
    internal int SetValues(
      IntPtr pQpServices,
      IntPtr pSeCursor,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      IntPtr pError)
    {
      if (this._ME_SetValues == null)
        this._ME_SetValues = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetValues>("ME_SetValues");
      return this._ME_SetValues(pQpServices, pSeCursor, prgBinding, cDbBinding, pData, pError);
    }

    [SecurityCritical]
    internal unsafe int SetValue(
      IntPtr pSeCursor,
      int seSetColumn,
      void* pBuffer,
      int ordinal,
      int size,
      IntPtr pError)
    {
      if (this._ME_SetValue == null)
        this._ME_SetValue = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetValue>("ME_SetValue");
      return this._ME_SetValue(pSeCursor, seSetColumn, pBuffer, ordinal, size, pError);
    }

    [SecurityCritical]
    internal int Prepare(IntPtr pSeCursor, SEPREPAREMODE mode, IntPtr pError)
    {
      if (this._ME_Prepare == null)
        this._ME_Prepare = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_Prepare>("ME_Prepare");
      return this._ME_Prepare(pSeCursor, mode, pError);
    }

    [SecurityCritical]
    internal int InsertRecord(int fMoveTo, IntPtr pSeCursor, ref int hBookmark, IntPtr pError)
    {
      if (this._ME_InsertRecord == null)
        this._ME_InsertRecord = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_InsertRecord>("ME_InsertRecord");
      return this._ME_InsertRecord(fMoveTo, pSeCursor, ref hBookmark, pError);
    }

    [SecurityCritical]
    internal int UpdateRecord(IntPtr pSeCursor, IntPtr pError)
    {
      if (this._ME_UpdateRecord == null)
        this._ME_UpdateRecord = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_UpdateRecord>("ME_UpdateRecord");
      return this._ME_UpdateRecord(pSeCursor, pError);
    }

    [SecurityCritical]
    internal int DeleteRecord(IntPtr pSeCursor, IntPtr pError)
    {
      if (this._ME_DeleteRecord == null)
        this._ME_DeleteRecord = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_DeleteRecord>("ME_DeleteRecord");
      return this._ME_DeleteRecord(pSeCursor, pError);
    }

    [SecurityCritical]
    internal int GotoBookmark(IntPtr pSeCursor, int hBookmark, IntPtr pError)
    {
      if (this._ME_GotoBookmark == null)
        this._ME_GotoBookmark = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GotoBookmark>("ME_GotoBookmark");
      return this._ME_GotoBookmark(pSeCursor, hBookmark, pError);
    }

    [SecurityCritical]
    internal int GetContextErrorInfo(
      IntPtr pError,
      ref int lNumber,
      ref int lNativeError,
      ref IntPtr pwszMessage,
      ref IntPtr pwszSource,
      ref int numPar1,
      ref int numPar2,
      ref int numPar3,
      ref IntPtr pwszErr1,
      ref IntPtr pwszErr2,
      ref IntPtr pwszErr3)
    {
      if (this._ME_GetContextErrorInfo == null)
        this._ME_GetContextErrorInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetContextErrorInfo>("ME_GetContextErrorInfo");
      return this._ME_GetContextErrorInfo(pError, ref lNumber, ref lNativeError, ref pwszMessage, ref pwszSource, ref numPar1, ref numPar2, ref numPar3, ref pwszErr1, ref pwszErr2, ref pwszErr3);
    }

    [SecurityCritical]
    internal int GetContextErrorMessage(int dminorError, ref IntPtr pwszMessage)
    {
      if (this._ME_GetContextErrorMessage == null)
        this._ME_GetContextErrorMessage = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetContextErrorMessage>("ME_GetContextErrorMessage");
      return this._ME_GetContextErrorMessage(dminorError, ref pwszMessage);
    }

    [SecurityCritical]
    internal int GetMinorError(IntPtr pError, ref int lMinor)
    {
      if (this._ME_GetMinorError == null)
        this._ME_GetMinorError = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetMinorError>("ME_GetMinorError");
      return this._ME_GetMinorError(pError, ref lMinor);
    }

    [SecurityCritical]
    internal int GetBookmark(IntPtr pSeCursor, ref int hBookmark, IntPtr pError)
    {
      if (this._ME_GetBookmark == null)
        this._ME_GetBookmark = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetBookmark>("ME_GetBookmark");
      return this._ME_GetBookmark(pSeCursor, ref hBookmark, pError);
    }

    [SecurityCritical]
    internal int GetColumnInfo(
      IntPtr pIUnknown,
      ref int columnCount,
      ref IntPtr prgColumnInfo,
      IntPtr pError)
    {
      if (this._ME_GetColumnInfo == null)
        this._ME_GetColumnInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetColumnInfo>("ME_GetColumnInfo");
      return this._ME_GetColumnInfo(pIUnknown, ref columnCount, ref prgColumnInfo, pError);
    }

    [SecurityCritical]
    internal int SetColumnInfo(
      IntPtr pITransact,
      string TableName,
      string ColumnName,
      SECOLUMNINFO seColumnInfo,
      SECOLUMNATTRIB seColAttrib,
      IntPtr pError)
    {
      if (this._ME_SetColumnInfo == null)
        this._ME_SetColumnInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetColumnInfo>("ME_SetColumnInfo");
      return this._ME_SetColumnInfo(pITransact, TableName, ColumnName, seColumnInfo, seColAttrib, pError);
    }

    [SecurityCritical]
    internal int SetTableInfoAsSystem(IntPtr pITransact, string TableName, IntPtr pError)
    {
      if (this._ME_SetTableInfoAsSystem == null)
        this._ME_SetTableInfoAsSystem = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetTableInfoAsSystem>("ME_SetTableInfoAsSystem");
      return this._ME_SetTableInfoAsSystem(pITransact, TableName, pError);
    }

    [SecurityCritical]
    internal int GetParameterInfo(
      IntPtr pQpCommand,
      ref uint columnCount,
      ref IntPtr prgParamInfo,
      IntPtr pError)
    {
      if (this._ME_GetParameterInfo == null)
        this._ME_GetParameterInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetParameterInfo>("ME_GetParameterInfo");
      return this._ME_GetParameterInfo(pQpCommand, ref columnCount, ref prgParamInfo, pError);
    }

    [SecurityCritical]
    internal int GetIndexColumnOrdinals(
      IntPtr pSeCursor,
      IntPtr pwszIndex,
      ref uint cColumns,
      ref IntPtr priOrdinals,
      IntPtr pError)
    {
      if (this._ME_GetIndexColumnOrdinals == null)
        this._ME_GetIndexColumnOrdinals = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetIndexColumnOrdinals>("ME_GetIndexColumnOrdinals");
      return this._ME_GetIndexColumnOrdinals(pSeCursor, pwszIndex, ref cColumns, ref priOrdinals, pError);
    }

    [SecurityCritical]
    internal int GetKeyInfo(
      IntPtr pIUnknown,
      IntPtr pTx,
      string pwszBaseTable,
      IntPtr prgDbKeyInfo,
      int cDbKeyInfo,
      IntPtr pError)
    {
      if (this._ME_GetKeyInfo == null)
        this._ME_GetKeyInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetKeyInfo>("ME_GetKeyInfo");
      return this._ME_GetKeyInfo(pIUnknown, pTx, pwszBaseTable, prgDbKeyInfo, cDbKeyInfo, pError);
    }

    [SecurityCritical]
    internal int CreateCommand(IntPtr pQpSession, ref IntPtr pQpCommand, IntPtr pError)
    {
      if (this._ME_CreateCommand == null)
        this._ME_CreateCommand = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CreateCommand>("ME_CreateCommand");
      return this._ME_CreateCommand(pQpSession, ref pQpCommand, pError);
    }

    [SecurityCritical]
    internal int CompileQueryPlan(
      IntPtr pQpCommand,
      string pwszCommandText,
      ResultSetOptions options,
      IntPtr[] pParamNames,
      IntPtr prgBinding,
      int cDbBinding,
      ref IntPtr pQpPlan,
      IntPtr pError)
    {
      if (this._ME_CompileQueryPlan == null)
        this._ME_CompileQueryPlan = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CompileQueryPlan>("ME_CompileQueryPlan");
      return this._ME_CompileQueryPlan(pQpCommand, pwszCommandText, options, pParamNames, prgBinding, cDbBinding, ref pQpPlan, pError);
    }

    [SecurityCritical]
    internal int Move(IntPtr pSeCursor, DIRECTION direction, IntPtr pError)
    {
      if (this._ME_Move == null)
        this._ME_Move = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_Move>("ME_Move");
      return this._ME_Move(pSeCursor, direction, pError);
    }

    [SecurityCritical]
    internal int AbortTransaction(IntPtr pTx, IntPtr pError)
    {
      if (this._ME_AbortTransaction == null)
        this._ME_AbortTransaction = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_AbortTransaction>("ME_AbortTransaction");
      return this._ME_AbortTransaction(pTx, pError);
    }

    [SecurityCritical]
    internal int CommitTransaction(IntPtr pTx, CommitMode mode, IntPtr pError)
    {
      if (this._ME_CommitTransaction == null)
        this._ME_CommitTransaction = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CommitTransaction>("ME_CommitTransaction");
      return this._ME_CommitTransaction(pTx, mode, pError);
    }

    [SecurityCritical]
    internal int SetTransactionFlag(
      IntPtr pITransact,
      SeTransactionFlags seTxFlag,
      bool fEnable,
      IntPtr pError)
    {
      if (this._ME_SetTransactionFlag == null)
        this._ME_SetTransactionFlag = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetTransactionFlag>("ME_SetTransactionFlag");
      return this._ME_SetTransactionFlag(pITransact, seTxFlag, fEnable, pError);
    }

    [SecurityCritical]
    internal int GetTransactionFlags(IntPtr pITransact, ref SeTransactionFlags seTxFlags)
    {
      if (this._ME_GetTransactionFlags == null)
        this._ME_GetTransactionFlags = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetTransactionFlags>("ME_GetTransactionFlags");
      return this._ME_GetTransactionFlags(pITransact, ref seTxFlags);
    }

    [SecurityCritical]
    internal int GetTrackingContext(
      IntPtr pITransact,
      out IntPtr pGuidTrackingContext,
      IntPtr pError)
    {
      if (this._ME_GetTrackingContext == null)
        this._ME_GetTrackingContext = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetTrackingContext>("ME_GetTrackingContext");
      return this._ME_GetTrackingContext(pITransact, out pGuidTrackingContext, pError);
    }

    [SecurityCritical]
    internal int SetTrackingContext(
      IntPtr pITransact,
      ref IntPtr pGuidTrackingContext,
      IntPtr pError)
    {
      if (this._ME_SetTrackingContext == null)
        this._ME_SetTrackingContext = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_SetTrackingContext>("ME_SetTrackingContext");
      return this._ME_SetTrackingContext(pITransact, ref pGuidTrackingContext, pError);
    }

    [SecurityCritical]
    internal int GetTransactionBsn(IntPtr pITransact, ref long pTransactionBsn, IntPtr pError)
    {
      if (this._ME_GetTransactionBsn == null)
        this._ME_GetTransactionBsn = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetTransactionBsn>("ME_GetTransactionBsn");
      return this._ME_GetTransactionBsn(pITransact, ref pTransactionBsn, pError);
    }

    [SecurityCritical]
    internal int InitChangeTracking(IntPtr pITransact, ref IntPtr pTracking, ref IntPtr pError)
    {
      if (this._ME_InitChangeTracking == null)
        this._ME_InitChangeTracking = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_InitChangeTracking>("ME_InitChangeTracking");
      return this._ME_InitChangeTracking(pITransact, ref pTracking, ref pError);
    }

    [SecurityCritical]
    internal int ExitChangeTracking(ref IntPtr pTracking, ref IntPtr pError)
    {
      if (this._ME_ExitChangeTracking == null)
        this._ME_ExitChangeTracking = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_ExitChangeTracking>("ME_ExitChangeTracking");
      return this._ME_ExitChangeTracking(ref pTracking, ref pError);
    }

    [SecurityCritical]
    internal int EnableChangeTracking(
      IntPtr pTracking,
      string TableName,
      SETRACKINGTYPE seTrackingType,
      SEOCSTRACKOPTIONS seTrackOpts,
      IntPtr pError)
    {
      if (this._ME_EnableChangeTracking == null)
        this._ME_EnableChangeTracking = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_EnableChangeTracking>("ME_EnableChangeTracking");
      return this._ME_EnableChangeTracking(pTracking, TableName, seTrackingType, seTrackOpts, pError);
    }

    [SecurityCritical]
    internal int GetTrackingOptions(
      IntPtr pTracking,
      string TableName,
      ref SEOCSTRACKOPTIONSV2 iTrackingOptions,
      IntPtr pError)
    {
      if (this._ME_GetTrackingOptions == null)
        this._ME_GetTrackingOptions = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetTrackingOptions>("ME_GetTrackingOptions");
      return this._ME_GetTrackingOptions(pTracking, TableName, ref iTrackingOptions, pError);
    }

    [SecurityCritical]
    internal int DisableChangeTracking(IntPtr pTracking, string TableName, IntPtr pError)
    {
      if (this._ME_DisableChangeTracking == null)
        this._ME_DisableChangeTracking = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_DisableChangeTracking>("ME_DisableChangeTracking");
      return this._ME_DisableChangeTracking(pTracking, TableName, pError);
    }

    [SecurityCritical]
    internal int IsTableChangeTracked(
      IntPtr pTracking,
      string TableName,
      ref bool fTableTracked,
      IntPtr pError)
    {
      if (this._ME_IsTableChangeTracked == null)
        this._ME_IsTableChangeTracked = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_IsTableChangeTracked>("ME_IsTableChangeTracked");
      return this._ME_IsTableChangeTracked(pTracking, TableName, ref fTableTracked, pError);
    }

    [SecurityCritical]
    internal int GetChangeTrackingInfo(
      IntPtr pTracking,
      string TableName,
      ref SEOCSTRACKOPTIONS trackOptions,
      ref SETRACKINGTYPE trackType,
      ref long trackOrdinal,
      IntPtr pError)
    {
      if (this._ME_GetChangeTrackingInfo == null)
        this._ME_GetChangeTrackingInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetChangeTrackingInfo>("ME_GetChangeTrackingInfo");
      return this._ME_GetChangeTrackingInfo(pTracking, TableName, ref trackOptions, ref trackType, ref trackOrdinal, pError);
    }

    [SecurityCritical]
    internal int CleanupTrackingMetadata(
      IntPtr pTracking,
      string TableName,
      int retentionPeriodInDays,
      long cutoffTxCsn,
      long leastTxCsn,
      IntPtr pError)
    {
      if (this._ME_CleanupTrackingMetadata == null)
        this._ME_CleanupTrackingMetadata = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CleanupTrackingMetadata>("ME_CleanupTrackingMetadata");
      return this._ME_CleanupTrackingMetadata(pTracking, TableName, retentionPeriodInDays, cutoffTxCsn, leastTxCsn, pError);
    }

    [SecurityCritical]
    internal int CleanupTransactionData(
      IntPtr pTracking,
      int iRetentionPeriodInDays,
      long ullCutoffTransactionCsn,
      IntPtr pError)
    {
      if (this._ME_CleanupTransactionData == null)
        this._ME_CleanupTransactionData = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CleanupTransactionData>("ME_CleanupTransactionData");
      return this._ME_CleanupTransactionData(pTracking, iRetentionPeriodInDays, ullCutoffTransactionCsn, pError);
    }

    [SecurityCritical]
    internal int CleanupTombstoneData(
      IntPtr pTracking,
      string TableName,
      int iRetentionPeriodInDays,
      long ullCutoffTransactionCsn,
      IntPtr pError)
    {
      if (this._ME_CleanupTombstoneData == null)
        this._ME_CleanupTombstoneData = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_CleanupTombstoneData>("ME_CleanupTombstoneData");
      return this._ME_CleanupTombstoneData(pTracking, TableName, iRetentionPeriodInDays, ullCutoffTransactionCsn, pError);
    }

    [SecurityCritical]
    internal int GetCurrentTrackingTxCsn(IntPtr pTracking, ref long txCsn, IntPtr pError)
    {
      if (this._ME_GetCurrentTrackingTxCsn == null)
        this._ME_GetCurrentTrackingTxCsn = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetCurrentTrackingTxCsn>("ME_GetCurrentTrackingTxCsn");
      return this._ME_GetCurrentTrackingTxCsn(pTracking, ref txCsn, pError);
    }

    [SecurityCritical]
    internal int GetCurrentTrackingTxBsn(IntPtr pTracking, ref long txBsn, IntPtr pError)
    {
      if (this._ME_GetCurrentTrackingTxBsn == null)
        this._ME_GetCurrentTrackingTxBsn = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_GetCurrentTrackingTxBsn>("ME_GetCurrentTrackingTxBsn");
      return this._ME_GetCurrentTrackingTxBsn(pTracking, ref txBsn, pError);
    }

    [SecurityCritical]
    internal int DllAddRef()
    {
      if (this._DllAddRef == null)
        this._DllAddRef = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_DllAddRef>(nameof (DllAddRef));
      return this._DllAddRef();
    }

    [SecurityCritical]
    internal int DllRelease()
    {
      if (this._DllRelease == null)
        this._DllRelease = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_DllRelease>(nameof (DllRelease));
      return this._DllRelease();
    }

    [SecurityCritical]
    internal int ClearErrorInfo(IntPtr pError)
    {
      if (this._ME_ClearErrorInfo == null)
        this._ME_ClearErrorInfo = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_ClearErrorInfo>("ME_ClearErrorInfo");
      return this._ME_ClearErrorInfo(pError);
    }

    [SecurityCritical]
    internal int ExecuteQueryPlan(
      IntPtr pTx,
      IntPtr pQpServices,
      IntPtr pQpCommand,
      IntPtr pQpPlan,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      ref int recordsAffected,
      ref ResultSetOptions cursorCapabilities,
      ref IntPtr pSeCursor,
      ref int fIsBaseTableCursor,
      IntPtr pError)
    {
      if (this._ME_ExecuteQueryPlan == null)
        this._ME_ExecuteQueryPlan = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_ME_ExecuteQueryPlan>("ME_ExecuteQueryPlan");
      return this._ME_ExecuteQueryPlan(pTx, pQpServices, pQpCommand, pQpPlan, prgBinding, cDbBinding, pData, ref recordsAffected, ref cursorCapabilities, ref pSeCursor, ref fIsBaseTableCursor, pError);
    }

    internal int uwrda_put_ControlReceiveTimeout(IntPtr pIRda, int ControlReceiveTimeout)
    {
      if (this._uwrda_put_ControlReceiveTimeout == null)
        this._uwrda_put_ControlReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_ControlReceiveTimeout>(nameof (uwrda_put_ControlReceiveTimeout));
      return this._uwrda_put_ControlReceiveTimeout(pIRda, ControlReceiveTimeout);
    }

    internal int uwrda_get_ConnectionRetryTimeout(IntPtr pIRda, ref ushort ConnectionRetryTimeout)
    {
      if (this._uwrda_get_ConnectionRetryTimeout == null)
        this._uwrda_get_ConnectionRetryTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ConnectionRetryTimeout>(nameof (uwrda_get_ConnectionRetryTimeout));
      return this._uwrda_get_ConnectionRetryTimeout(pIRda, ref ConnectionRetryTimeout);
    }

    internal int uwrda_put_ConnectionRetryTimeout(IntPtr pIRda, ushort ConnectionRetryTimeout)
    {
      if (this._uwrda_put_ConnectionRetryTimeout == null)
        this._uwrda_put_ConnectionRetryTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_ConnectionRetryTimeout>(nameof (uwrda_put_ConnectionRetryTimeout));
      return this._uwrda_put_ConnectionRetryTimeout(pIRda, ConnectionRetryTimeout);
    }

    internal int uwrda_get_CompressionLevel(IntPtr pIRda, ref ushort CompressionLevel)
    {
      if (this._uwrda_get_CompressionLevel == null)
        this._uwrda_get_CompressionLevel = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_CompressionLevel>(nameof (uwrda_get_CompressionLevel));
      return this._uwrda_get_CompressionLevel(pIRda, ref CompressionLevel);
    }

    internal int uwrda_put_CompressionLevel(IntPtr pIRda, ushort CompressionLevel)
    {
      if (this._uwrda_put_CompressionLevel == null)
        this._uwrda_put_CompressionLevel = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_CompressionLevel>(nameof (uwrda_put_CompressionLevel));
      return this._uwrda_put_CompressionLevel(pIRda, CompressionLevel);
    }

    internal int uwrda_get_ConnectionManager(IntPtr pIRda, ref bool ConnectionManager)
    {
      if (this._uwrda_get_ConnectionManager == null)
        this._uwrda_get_ConnectionManager = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ConnectionManager>(nameof (uwrda_get_ConnectionManager));
      return this._uwrda_get_ConnectionManager(pIRda, ref ConnectionManager);
    }

    internal int uwrda_put_ConnectionManager(IntPtr pIRda, bool ConnectionManager)
    {
      if (this._uwrda_put_ConnectionManager == null)
        this._uwrda_put_ConnectionManager = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_ConnectionManager>(nameof (uwrda_put_ConnectionManager));
      return this._uwrda_put_ConnectionManager(pIRda, ConnectionManager);
    }

    internal int uwrda_Pull(
      IntPtr pIRda,
      string zLocalTableName,
      string zSqlSelectString,
      string zOleDbConnectionString,
      RdaTrackOption trackOption,
      string zErrorTable)
    {
      if (this._uwrda_Pull == null)
        this._uwrda_Pull = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_Pull>(nameof (uwrda_Pull));
      return this._uwrda_Pull(pIRda, zLocalTableName, zSqlSelectString, zOleDbConnectionString, trackOption, zErrorTable);
    }

    internal int uwrda_Push(
      IntPtr pIRda,
      string zLocalTableName,
      string zOleDbConnectionString,
      RdaBatchOption batchOption)
    {
      if (this._uwrda_Push == null)
        this._uwrda_Push = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_Push>(nameof (uwrda_Push));
      return this._uwrda_Push(pIRda, zLocalTableName, zOleDbConnectionString, batchOption);
    }

    internal int uwrda_SubmitSql(IntPtr pIRda, string zSqlString, string zOleDbConnectionString)
    {
      if (this._uwrda_SubmitSql == null)
        this._uwrda_SubmitSql = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_SubmitSql>(nameof (uwrda_SubmitSql));
      return this._uwrda_SubmitSql(pIRda, zSqlString, zOleDbConnectionString);
    }

    internal int uwrda_get_InternetLogin(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_InternetLogin == null)
        this._uwrda_get_InternetLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_InternetLogin>(nameof (uwrda_get_InternetLogin));
      return this._uwrda_get_InternetLogin(pIRda, ref rbz);
    }

    internal int uwrda_put_InternetLogin(IntPtr pIRda, string InternetLogin)
    {
      if (this._uwrda_put_InternetLogin == null)
        this._uwrda_put_InternetLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_InternetLogin>(nameof (uwrda_put_InternetLogin));
      return this._uwrda_put_InternetLogin(pIRda, InternetLogin);
    }

    internal int uwrda_get_InternetPassword(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_InternetPassword == null)
        this._uwrda_get_InternetPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_InternetPassword>(nameof (uwrda_get_InternetPassword));
      return this._uwrda_get_InternetPassword(pIRda, ref rbz);
    }

    internal int uwrda_put_InternetPassword(IntPtr pIRda, string InternetPassword)
    {
      if (this._uwrda_put_InternetPassword == null)
        this._uwrda_put_InternetPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_InternetPassword>(nameof (uwrda_put_InternetPassword));
      return this._uwrda_put_InternetPassword(pIRda, InternetPassword);
    }

    internal int uwrda_get_InternetProxyServer(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_InternetProxyServer == null)
        this._uwrda_get_InternetProxyServer = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_InternetProxyServer>(nameof (uwrda_get_InternetProxyServer));
      return this._uwrda_get_InternetProxyServer(pIRda, ref rbz);
    }

    internal int uwrda_put_InternetProxyServer(IntPtr pIRda, string InternetProxyServer)
    {
      if (this._uwrda_put_InternetProxyServer == null)
        this._uwrda_put_InternetProxyServer = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_InternetProxyServer>(nameof (uwrda_put_InternetProxyServer));
      return this._uwrda_put_InternetProxyServer(pIRda, InternetProxyServer);
    }

    internal int uwrda_get_InternetProxyLogin(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_InternetProxyLogin == null)
        this._uwrda_get_InternetProxyLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_InternetProxyLogin>(nameof (uwrda_get_InternetProxyLogin));
      return this._uwrda_get_InternetProxyLogin(pIRda, ref rbz);
    }

    internal int uwrda_put_InternetProxyLogin(IntPtr pIRda, string InternetProxyLogin)
    {
      if (this._uwrda_put_InternetProxyLogin == null)
        this._uwrda_put_InternetProxyLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_InternetProxyLogin>(nameof (uwrda_put_InternetProxyLogin));
      return this._uwrda_put_InternetProxyLogin(pIRda, InternetProxyLogin);
    }

    internal int uwrda_get_InternetProxyPassword(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_InternetProxyPassword == null)
        this._uwrda_get_InternetProxyPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_InternetProxyPassword>(nameof (uwrda_get_InternetProxyPassword));
      return this._uwrda_get_InternetProxyPassword(pIRda, ref rbz);
    }

    internal int uwrda_put_InternetProxyPassword(IntPtr pIRda, string InternetProxyPassword)
    {
      if (this._uwrda_put_InternetProxyPassword == null)
        this._uwrda_put_InternetProxyPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_InternetProxyPassword>(nameof (uwrda_put_InternetProxyPassword));
      return this._uwrda_put_InternetProxyPassword(pIRda, InternetProxyPassword);
    }

    internal int uwrda_get_ConnectTimeout(IntPtr pIRda, ref int connectTimeout)
    {
      if (this._uwrda_get_ConnectTimeout == null)
        this._uwrda_get_ConnectTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ConnectTimeout>(nameof (uwrda_get_ConnectTimeout));
      return this._uwrda_get_ConnectTimeout(pIRda, ref connectTimeout);
    }

    internal int uwrda_put_ConnectTimeout(IntPtr pIRda, int connectTimeout)
    {
      if (this._uwrda_put_ConnectTimeout == null)
        this._uwrda_put_ConnectTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_ConnectTimeout>(nameof (uwrda_put_ConnectTimeout));
      return this._uwrda_put_ConnectTimeout(pIRda, connectTimeout);
    }

    internal int uwrda_get_SendTimeout(IntPtr pIRda, ref int SendTimeout)
    {
      if (this._uwrda_get_SendTimeout == null)
        this._uwrda_get_SendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_SendTimeout>(nameof (uwrda_get_SendTimeout));
      return this._uwrda_get_SendTimeout(pIRda, ref SendTimeout);
    }

    internal int uwrda_put_SendTimeout(IntPtr pIRda, int SendTimeout)
    {
      if (this._uwrda_put_SendTimeout == null)
        this._uwrda_put_SendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_SendTimeout>(nameof (uwrda_put_SendTimeout));
      return this._uwrda_put_SendTimeout(pIRda, SendTimeout);
    }

    internal int uwrda_get_ReceiveTimeout(IntPtr pIRda, ref int ReceiveTimeout)
    {
      if (this._uwrda_get_ReceiveTimeout == null)
        this._uwrda_get_ReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ReceiveTimeout>(nameof (uwrda_get_ReceiveTimeout));
      return this._uwrda_get_ReceiveTimeout(pIRda, ref ReceiveTimeout);
    }

    internal int uwrda_put_ReceiveTimeout(IntPtr pIRda, int ReceiveTimeout)
    {
      if (this._uwrda_put_ReceiveTimeout == null)
        this._uwrda_put_ReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_ReceiveTimeout>(nameof (uwrda_put_ReceiveTimeout));
      return this._uwrda_put_ReceiveTimeout(pIRda, ReceiveTimeout);
    }

    internal int uwrda_get_DataSendTimeout(IntPtr pIRda, ref int DataSendTimeout)
    {
      if (this._uwrda_get_DataSendTimeout == null)
        this._uwrda_get_DataSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_DataSendTimeout>(nameof (uwrda_get_DataSendTimeout));
      return this._uwrda_get_DataSendTimeout(pIRda, ref DataSendTimeout);
    }

    internal int uwrda_put_DataSendTimeout(IntPtr pIRda, int DataSendTimeout)
    {
      if (this._uwrda_put_DataSendTimeout == null)
        this._uwrda_put_DataSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_DataSendTimeout>(nameof (uwrda_put_DataSendTimeout));
      return this._uwrda_put_DataSendTimeout(pIRda, DataSendTimeout);
    }

    internal int uwrda_get_DataReceiveTimeout(IntPtr pIRda, ref int DataReceiveTimeout)
    {
      if (this._uwrda_get_DataReceiveTimeout == null)
        this._uwrda_get_DataReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_DataReceiveTimeout>(nameof (uwrda_get_DataReceiveTimeout));
      return this._uwrda_get_DataReceiveTimeout(pIRda, ref DataReceiveTimeout);
    }

    internal int uwrda_put_DataReceiveTimeout(IntPtr pIRda, int DataReceiveTimeout)
    {
      if (this._uwrda_put_DataReceiveTimeout == null)
        this._uwrda_put_DataReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_DataReceiveTimeout>(nameof (uwrda_put_DataReceiveTimeout));
      return this._uwrda_put_DataReceiveTimeout(pIRda, DataReceiveTimeout);
    }

    internal int uwrda_get_ControlSendTimeout(IntPtr pIRda, ref int ControlSendTimeout)
    {
      if (this._uwrda_get_ControlSendTimeout == null)
        this._uwrda_get_ControlSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ControlSendTimeout>(nameof (uwrda_get_ControlSendTimeout));
      return this._uwrda_get_ControlSendTimeout(pIRda, ref ControlSendTimeout);
    }

    internal int uwrda_put_ControlSendTimeout(IntPtr pIRda, int ControlSendTimeout)
    {
      if (this._uwrda_put_ControlSendTimeout == null)
        this._uwrda_put_ControlSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_ControlSendTimeout>(nameof (uwrda_put_ControlSendTimeout));
      return this._uwrda_put_ControlSendTimeout(pIRda, ControlSendTimeout);
    }

    internal int uwrda_get_ControlReceiveTimeout(IntPtr pIRda, ref int ControlReceiveTimeout)
    {
      if (this._uwrda_get_ControlReceiveTimeout == null)
        this._uwrda_get_ControlReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ControlReceiveTimeout>(nameof (uwrda_get_ControlReceiveTimeout));
      return this._uwrda_get_ControlReceiveTimeout(pIRda, ref ControlReceiveTimeout);
    }

    internal int uwrda_RemoteDataAccess(ref IntPtr pIRda, ref IntPtr pCreationIError)
    {
      if (this._uwrda_RemoteDataAccess == null)
        this._uwrda_RemoteDataAccess = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_RemoteDataAccess>(nameof (uwrda_RemoteDataAccess));
      return this._uwrda_RemoteDataAccess(ref pIRda, ref pCreationIError);
    }

    internal int uwrda_get_ErrorPointer(IntPtr pIRda, ref IntPtr pIErrors)
    {
      if (this._uwrda_get_ErrorPointer == null)
        this._uwrda_get_ErrorPointer = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_ErrorPointer>(nameof (uwrda_get_ErrorPointer));
      return this._uwrda_get_ErrorPointer(pIRda, ref pIErrors);
    }

    internal int uwrda_get_LocalConnectionString(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_LocalConnectionString == null)
        this._uwrda_get_LocalConnectionString = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_LocalConnectionString>(nameof (uwrda_get_LocalConnectionString));
      return this._uwrda_get_LocalConnectionString(pIRda, ref rbz);
    }

    internal int uwrda_put_LocalConnectionString(IntPtr pIRda, string zLocalConnectionString)
    {
      if (this._uwrda_put_LocalConnectionString == null)
        this._uwrda_put_LocalConnectionString = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_LocalConnectionString>(nameof (uwrda_put_LocalConnectionString));
      return this._uwrda_put_LocalConnectionString(pIRda, zLocalConnectionString);
    }

    internal int uwrda_get_InternetUrl(IntPtr pIRda, ref IntPtr rbz)
    {
      if (this._uwrda_get_InternetUrl == null)
        this._uwrda_get_InternetUrl = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_get_InternetUrl>(nameof (uwrda_get_InternetUrl));
      return this._uwrda_get_InternetUrl(pIRda, ref rbz);
    }

    internal int uwrda_put_InternetUrl(IntPtr pIRda, string InternetUrl)
    {
      if (this._uwrda_put_InternetUrl == null)
        this._uwrda_put_InternetUrl = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrda_put_InternetUrl>(nameof (uwrda_put_InternetUrl));
      return this._uwrda_put_InternetUrl(pIRda, InternetUrl);
    }

    internal int uwrepl_put_ConnectionRetryTimeout(
      IntPtr pIReplication,
      ushort ConnectionRetryTimeout)
    {
      if (this._uwrepl_put_ConnectionRetryTimeout == null)
        this._uwrepl_put_ConnectionRetryTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ConnectionRetryTimeout>(nameof (uwrepl_put_ConnectionRetryTimeout));
      return this._uwrepl_put_ConnectionRetryTimeout(pIReplication, ConnectionRetryTimeout);
    }

    internal int uwrepl_get_CompressionLevel(IntPtr pIReplication, ref ushort CompressionLevel)
    {
      if (this._uwrepl_get_CompressionLevel == null)
        this._uwrepl_get_CompressionLevel = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_CompressionLevel>(nameof (uwrepl_get_CompressionLevel));
      return this._uwrepl_get_CompressionLevel(pIReplication, ref CompressionLevel);
    }

    internal int uwrepl_put_CompressionLevel(IntPtr pIReplication, ushort CompressionLevel)
    {
      if (this._uwrepl_put_CompressionLevel == null)
        this._uwrepl_put_CompressionLevel = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_CompressionLevel>(nameof (uwrepl_put_CompressionLevel));
      return this._uwrepl_put_CompressionLevel(pIReplication, CompressionLevel);
    }

    internal int uwrepl_get_ConnectionManager(IntPtr pIReplication, ref bool ConnectionManager)
    {
      if (this._uwrepl_get_ConnectionManager == null)
        this._uwrepl_get_ConnectionManager = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ConnectionManager>(nameof (uwrepl_get_ConnectionManager));
      return this._uwrepl_get_ConnectionManager(pIReplication, ref ConnectionManager);
    }

    internal int uwrepl_put_ConnectionManager(IntPtr pIReplication, bool ConnectionManager)
    {
      if (this._uwrepl_put_ConnectionManager == null)
        this._uwrepl_put_ConnectionManager = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ConnectionManager>(nameof (uwrepl_put_ConnectionManager));
      return this._uwrepl_put_ConnectionManager(pIReplication, ConnectionManager);
    }

    internal int uwrepl_get_SnapshotTransferType(
      IntPtr pIReplication,
      ref SnapshotTransferType SnapshotTransferType)
    {
      if (this._uwrepl_get_SnapshotTransferType == null)
        this._uwrepl_get_SnapshotTransferType = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_SnapshotTransferType>(nameof (uwrepl_get_SnapshotTransferType));
      return this._uwrepl_get_SnapshotTransferType(pIReplication, ref SnapshotTransferType);
    }

    internal int uwrepl_put_SnapshotTransferType(
      IntPtr pIReplication,
      SnapshotTransferType SnapshotTransferType)
    {
      if (this._uwrepl_put_SnapshotTransferType == null)
        this._uwrepl_put_SnapshotTransferType = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_SnapshotTransferType>(nameof (uwrepl_put_SnapshotTransferType));
      return this._uwrepl_put_SnapshotTransferType(pIReplication, SnapshotTransferType);
    }

    internal int uwrepl_AddSubscription(IntPtr pIReplication, AddOption addOption)
    {
      if (this._uwrepl_AddSubscription == null)
        this._uwrepl_AddSubscription = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_AddSubscription>(nameof (uwrepl_AddSubscription));
      return this._uwrepl_AddSubscription(pIReplication, addOption);
    }

    internal int uwrepl_DropSubscription(IntPtr pIReplication, DropOption dropOption)
    {
      if (this._uwrepl_DropSubscription == null)
        this._uwrepl_DropSubscription = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_DropSubscription>(nameof (uwrepl_DropSubscription));
      return this._uwrepl_DropSubscription(pIReplication, dropOption);
    }

    internal int uwrepl_ReinitializeSubscription(IntPtr pIReplication, bool uploadBeforeReinit)
    {
      if (this._uwrepl_ReinitializeSubscription == null)
        this._uwrepl_ReinitializeSubscription = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_ReinitializeSubscription>(nameof (uwrepl_ReinitializeSubscription));
      return this._uwrepl_ReinitializeSubscription(pIReplication, uploadBeforeReinit);
    }

    internal int uwrepl_Initialize(IntPtr pIReplication)
    {
      if (this._uwrepl_Initialize == null)
        this._uwrepl_Initialize = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_Initialize>(nameof (uwrepl_Initialize));
      return this._uwrepl_Initialize(pIReplication);
    }

    internal int uwrepl_Run(IntPtr pIReplication)
    {
      if (this._uwrepl_Run == null)
        this._uwrepl_Run = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_Run>(nameof (uwrepl_Run));
      return this._uwrepl_Run(pIReplication);
    }

    internal int uwrepl_Terminate(IntPtr pIReplication)
    {
      if (this._uwrepl_Terminate == null)
        this._uwrepl_Terminate = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_Terminate>(nameof (uwrepl_Terminate));
      return this._uwrepl_Terminate(pIReplication);
    }

    internal int uwrepl_Cancel(IntPtr pIReplication)
    {
      if (this._uwrepl_Cancel == null)
        this._uwrepl_Cancel = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_Cancel>(nameof (uwrepl_Cancel));
      return this._uwrepl_Cancel(pIReplication);
    }

    internal int uwrepl_LoadProperties(IntPtr pIReplication, ref bool PasswordsLoaded)
    {
      if (this._uwrepl_LoadProperties == null)
        this._uwrepl_LoadProperties = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_LoadProperties>(nameof (uwrepl_LoadProperties));
      return this._uwrepl_LoadProperties(pIReplication, ref PasswordsLoaded);
    }

    internal int uwrepl_SaveProperties(IntPtr pIReplication)
    {
      if (this._uwrepl_SaveProperties == null)
        this._uwrepl_SaveProperties = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_SaveProperties>(nameof (uwrepl_SaveProperties));
      return this._uwrepl_SaveProperties(pIReplication);
    }

    internal int uwrepl_get_Subscriber(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_Subscriber == null)
        this._uwrepl_get_Subscriber = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_Subscriber>(nameof (uwrepl_get_Subscriber));
      return this._uwrepl_get_Subscriber(pIReplication, ref rbz);
    }

    internal int uwrepl_put_Subscriber(IntPtr pIReplication, string Subscriber)
    {
      if (this._uwrepl_put_Subscriber == null)
        this._uwrepl_put_Subscriber = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_Subscriber>(nameof (uwrepl_put_Subscriber));
      return this._uwrepl_put_Subscriber(pIReplication, Subscriber);
    }

    internal int uwrepl_get_SubscriberConnectionString(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_SubscriberConnectionString == null)
        this._uwrepl_get_SubscriberConnectionString = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_SubscriberConnectionString>(nameof (uwrepl_get_SubscriberConnectionString));
      return this._uwrepl_get_SubscriberConnectionString(pIReplication, ref rbz);
    }

    internal int uwrepl_put_SubscriberConnectionString(
      IntPtr pIReplication,
      string SubscriberConnectionString)
    {
      if (this._uwrepl_put_SubscriberConnectionString == null)
        this._uwrepl_put_SubscriberConnectionString = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_SubscriberConnectionString>(nameof (uwrepl_put_SubscriberConnectionString));
      return this._uwrepl_put_SubscriberConnectionString(pIReplication, SubscriberConnectionString);
    }

    internal int uwrepl_get_SubscriberChanges(IntPtr pIReplication, ref int SubscriberChanges)
    {
      if (this._uwrepl_get_SubscriberChanges == null)
        this._uwrepl_get_SubscriberChanges = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_SubscriberChanges>(nameof (uwrepl_get_SubscriberChanges));
      return this._uwrepl_get_SubscriberChanges(pIReplication, ref SubscriberChanges);
    }

    internal int uwrepl_get_SubscriberConflicts(IntPtr pIReplication, ref int SubscriberConflicts)
    {
      if (this._uwrepl_get_SubscriberConflicts == null)
        this._uwrepl_get_SubscriberConflicts = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_SubscriberConflicts>(nameof (uwrepl_get_SubscriberConflicts));
      return this._uwrepl_get_SubscriberConflicts(pIReplication, ref SubscriberConflicts);
    }

    internal int uwrepl_get_Validate(IntPtr pIReplication, ref ValidateType Validate)
    {
      if (this._uwrepl_get_Validate == null)
        this._uwrepl_get_Validate = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_Validate>(nameof (uwrepl_get_Validate));
      return this._uwrepl_get_Validate(pIReplication, ref Validate);
    }

    internal int uwrepl_put_Validate(IntPtr pIReplication, ValidateType Validate)
    {
      if (this._uwrepl_put_Validate == null)
        this._uwrepl_put_Validate = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_Validate>(nameof (uwrepl_put_Validate));
      return this._uwrepl_put_Validate(pIReplication, Validate);
    }

    internal int uwrepl_get_ConnectTimeout(IntPtr pIReplication, ref int connectTimeout)
    {
      if (this._uwrepl_get_ConnectTimeout == null)
        this._uwrepl_get_ConnectTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ConnectTimeout>(nameof (uwrepl_get_ConnectTimeout));
      return this._uwrepl_get_ConnectTimeout(pIReplication, ref connectTimeout);
    }

    internal int uwrepl_put_ConnectTimeout(IntPtr pIReplication, int connectTimeout)
    {
      if (this._uwrepl_put_ConnectTimeout == null)
        this._uwrepl_put_ConnectTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ConnectTimeout>(nameof (uwrepl_put_ConnectTimeout));
      return this._uwrepl_put_ConnectTimeout(pIReplication, connectTimeout);
    }

    internal int uwrepl_get_SendTimeout(IntPtr pIReplication, ref int SendTimeout)
    {
      if (this._uwrepl_get_SendTimeout == null)
        this._uwrepl_get_SendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_SendTimeout>(nameof (uwrepl_get_SendTimeout));
      return this._uwrepl_get_SendTimeout(pIReplication, ref SendTimeout);
    }

    internal int uwrepl_put_SendTimeout(IntPtr pIReplication, int SendTimeout)
    {
      if (this._uwrepl_put_SendTimeout == null)
        this._uwrepl_put_SendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_SendTimeout>(nameof (uwrepl_put_SendTimeout));
      return this._uwrepl_put_SendTimeout(pIReplication, SendTimeout);
    }

    internal int uwrepl_get_ReceiveTimeout(IntPtr pIReplication, ref int ReceiveTimeout)
    {
      if (this._uwrepl_get_ReceiveTimeout == null)
        this._uwrepl_get_ReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ReceiveTimeout>(nameof (uwrepl_get_ReceiveTimeout));
      return this._uwrepl_get_ReceiveTimeout(pIReplication, ref ReceiveTimeout);
    }

    internal int uwrepl_put_ReceiveTimeout(IntPtr pIReplication, int ReceiveTimeout)
    {
      if (this._uwrepl_put_ReceiveTimeout == null)
        this._uwrepl_put_ReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ReceiveTimeout>(nameof (uwrepl_put_ReceiveTimeout));
      return this._uwrepl_put_ReceiveTimeout(pIReplication, ReceiveTimeout);
    }

    internal int uwrepl_get_DataSendTimeout(IntPtr pIReplication, ref int DataSendTimeout)
    {
      if (this._uwrepl_get_DataSendTimeout == null)
        this._uwrepl_get_DataSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DataSendTimeout>(nameof (uwrepl_get_DataSendTimeout));
      return this._uwrepl_get_DataSendTimeout(pIReplication, ref DataSendTimeout);
    }

    internal int uwrepl_put_DataSendTimeout(IntPtr pIReplication, int DataSendTimeout)
    {
      if (this._uwrepl_put_DataSendTimeout == null)
        this._uwrepl_put_DataSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DataSendTimeout>(nameof (uwrepl_put_DataSendTimeout));
      return this._uwrepl_put_DataSendTimeout(pIReplication, DataSendTimeout);
    }

    internal int uwrepl_get_DataReceiveTimeout(IntPtr pIReplication, ref int DataReceiveTimeout)
    {
      if (this._uwrepl_get_DataReceiveTimeout == null)
        this._uwrepl_get_DataReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DataReceiveTimeout>(nameof (uwrepl_get_DataReceiveTimeout));
      return this._uwrepl_get_DataReceiveTimeout(pIReplication, ref DataReceiveTimeout);
    }

    internal int uwrepl_put_DataReceiveTimeout(IntPtr pIReplication, int DataReceiveTimeout)
    {
      if (this._uwrepl_put_DataReceiveTimeout == null)
        this._uwrepl_put_DataReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DataReceiveTimeout>(nameof (uwrepl_put_DataReceiveTimeout));
      return this._uwrepl_put_DataReceiveTimeout(pIReplication, DataReceiveTimeout);
    }

    internal int uwrepl_get_ControlSendTimeout(IntPtr pIReplication, ref int ControlSendTimeout)
    {
      if (this._uwrepl_get_ControlSendTimeout == null)
        this._uwrepl_get_ControlSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ControlSendTimeout>(nameof (uwrepl_get_ControlSendTimeout));
      return this._uwrepl_get_ControlSendTimeout(pIReplication, ref ControlSendTimeout);
    }

    internal int uwrepl_put_ControlSendTimeout(IntPtr pIReplication, int ControlSendTimeout)
    {
      if (this._uwrepl_put_ControlSendTimeout == null)
        this._uwrepl_put_ControlSendTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ControlSendTimeout>(nameof (uwrepl_put_ControlSendTimeout));
      return this._uwrepl_put_ControlSendTimeout(pIReplication, ControlSendTimeout);
    }

    internal int uwrepl_get_ControlReceiveTimeout(
      IntPtr pIReplication,
      ref int ControlReceiveTimeout)
    {
      if (this._uwrepl_get_ControlReceiveTimeout == null)
        this._uwrepl_get_ControlReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ControlReceiveTimeout>(nameof (uwrepl_get_ControlReceiveTimeout));
      return this._uwrepl_get_ControlReceiveTimeout(pIReplication, ref ControlReceiveTimeout);
    }

    internal int uwrepl_put_ControlReceiveTimeout(IntPtr pIReplication, int ControlReceiveTimeout)
    {
      if (this._uwrepl_put_ControlReceiveTimeout == null)
        this._uwrepl_put_ControlReceiveTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ControlReceiveTimeout>(nameof (uwrepl_put_ControlReceiveTimeout));
      return this._uwrepl_put_ControlReceiveTimeout(pIReplication, ControlReceiveTimeout);
    }

    internal int uwrepl_get_ConnectionRetryTimeout(
      IntPtr pIReplication,
      ref ushort ConnectionRetryTimeout)
    {
      if (this._uwrepl_get_ConnectionRetryTimeout == null)
        this._uwrepl_get_ConnectionRetryTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ConnectionRetryTimeout>(nameof (uwrepl_get_ConnectionRetryTimeout));
      return this._uwrepl_get_ConnectionRetryTimeout(pIReplication, ref ConnectionRetryTimeout);
    }

    internal int uwrepl_put_LoginTimeout(IntPtr pIReplication, ushort LoginTimeout)
    {
      if (this._uwrepl_put_LoginTimeout == null)
        this._uwrepl_put_LoginTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_LoginTimeout>(nameof (uwrepl_put_LoginTimeout));
      return this._uwrepl_put_LoginTimeout(pIReplication, LoginTimeout);
    }

    internal int uwrepl_get_ProfileName(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_ProfileName == null)
        this._uwrepl_get_ProfileName = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ProfileName>(nameof (uwrepl_get_ProfileName));
      return this._uwrepl_get_ProfileName(pIReplication, ref rbz);
    }

    internal int uwrepl_put_ProfileName(IntPtr pIReplication, string ProfileName)
    {
      if (this._uwrepl_put_ProfileName == null)
        this._uwrepl_put_ProfileName = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ProfileName>(nameof (uwrepl_put_ProfileName));
      return this._uwrepl_put_ProfileName(pIReplication, ProfileName);
    }

    internal int uwrepl_get_Publisher(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_Publisher == null)
        this._uwrepl_get_Publisher = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_Publisher>(nameof (uwrepl_get_Publisher));
      return this._uwrepl_get_Publisher(pIReplication, ref rbz);
    }

    internal int uwrepl_put_Publisher(IntPtr pIReplication, string Publisher)
    {
      if (this._uwrepl_put_Publisher == null)
        this._uwrepl_put_Publisher = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_Publisher>(nameof (uwrepl_put_Publisher));
      return this._uwrepl_put_Publisher(pIReplication, Publisher);
    }

    internal int uwrepl_get_PublisherNetwork(IntPtr pIReplication, ref NetworkType PublisherNetwork)
    {
      if (this._uwrepl_get_PublisherNetwork == null)
        this._uwrepl_get_PublisherNetwork = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherNetwork>(nameof (uwrepl_get_PublisherNetwork));
      return this._uwrepl_get_PublisherNetwork(pIReplication, ref PublisherNetwork);
    }

    internal int uwrepl_put_PublisherNetwork(IntPtr pIReplication, NetworkType PublisherNetwork)
    {
      if (this._uwrepl_put_PublisherNetwork == null)
        this._uwrepl_put_PublisherNetwork = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PublisherNetwork>(nameof (uwrepl_put_PublisherNetwork));
      return this._uwrepl_put_PublisherNetwork(pIReplication, PublisherNetwork);
    }

    internal int uwrepl_get_PublisherAddress(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_PublisherAddress == null)
        this._uwrepl_get_PublisherAddress = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherAddress>(nameof (uwrepl_get_PublisherAddress));
      return this._uwrepl_get_PublisherAddress(pIReplication, ref rbz);
    }

    internal int uwrepl_put_PublisherAddress(IntPtr pIReplication, string PublisherAddress)
    {
      if (this._uwrepl_put_PublisherAddress == null)
        this._uwrepl_put_PublisherAddress = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PublisherAddress>(nameof (uwrepl_put_PublisherAddress));
      return this._uwrepl_put_PublisherAddress(pIReplication, PublisherAddress);
    }

    internal int uwrepl_get_PublisherSecurityMode(
      IntPtr pIReplication,
      ref SecurityType PublisherSecurityMode)
    {
      if (this._uwrepl_get_PublisherSecurityMode == null)
        this._uwrepl_get_PublisherSecurityMode = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherSecurityMode>(nameof (uwrepl_get_PublisherSecurityMode));
      return this._uwrepl_get_PublisherSecurityMode(pIReplication, ref PublisherSecurityMode);
    }

    internal int uwrepl_put_PublisherSecurityMode(
      IntPtr pIReplication,
      SecurityType PublisherSecurityMode)
    {
      if (this._uwrepl_put_PublisherSecurityMode == null)
        this._uwrepl_put_PublisherSecurityMode = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PublisherSecurityMode>(nameof (uwrepl_put_PublisherSecurityMode));
      return this._uwrepl_put_PublisherSecurityMode(pIReplication, PublisherSecurityMode);
    }

    internal int uwrepl_get_PublisherLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_PublisherLogin == null)
        this._uwrepl_get_PublisherLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherLogin>(nameof (uwrepl_get_PublisherLogin));
      return this._uwrepl_get_PublisherLogin(pIReplication, ref rbz);
    }

    internal int uwrepl_put_PublisherLogin(IntPtr pIReplication, string PublisherLogin)
    {
      if (this._uwrepl_put_PublisherLogin == null)
        this._uwrepl_put_PublisherLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PublisherLogin>(nameof (uwrepl_put_PublisherLogin));
      return this._uwrepl_put_PublisherLogin(pIReplication, PublisherLogin);
    }

    internal int uwrepl_get_PublisherPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_PublisherPassword == null)
        this._uwrepl_get_PublisherPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherPassword>(nameof (uwrepl_get_PublisherPassword));
      return this._uwrepl_get_PublisherPassword(pIReplication, ref rbz);
    }

    internal int uwrepl_put_PublisherPassword(IntPtr pIReplication, string PublisherPassword)
    {
      if (this._uwrepl_put_PublisherPassword == null)
        this._uwrepl_put_PublisherPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PublisherPassword>(nameof (uwrepl_put_PublisherPassword));
      return this._uwrepl_put_PublisherPassword(pIReplication, PublisherPassword);
    }

    internal int uwrepl_get_PublisherDatabase(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_PublisherDatabase == null)
        this._uwrepl_get_PublisherDatabase = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherDatabase>(nameof (uwrepl_get_PublisherDatabase));
      return this._uwrepl_get_PublisherDatabase(pIReplication, ref rbz);
    }

    internal int uwrepl_put_PublisherDatabase(IntPtr pIReplication, string PublisherDatabase)
    {
      if (this._uwrepl_put_PublisherDatabase == null)
        this._uwrepl_put_PublisherDatabase = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PublisherDatabase>(nameof (uwrepl_put_PublisherDatabase));
      return this._uwrepl_put_PublisherDatabase(pIReplication, PublisherDatabase);
    }

    internal int uwrepl_get_Publication(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_Publication == null)
        this._uwrepl_get_Publication = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_Publication>(nameof (uwrepl_get_Publication));
      return this._uwrepl_get_Publication(pIReplication, ref rbz);
    }

    internal int uwrepl_put_Publication(IntPtr pIReplication, string Publication)
    {
      if (this._uwrepl_put_Publication == null)
        this._uwrepl_put_Publication = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_Publication>(nameof (uwrepl_put_Publication));
      return this._uwrepl_put_Publication(pIReplication, Publication);
    }

    internal int uwrepl_get_PublisherChanges(IntPtr pIReplication, ref int PublisherChanges)
    {
      if (this._uwrepl_get_PublisherChanges == null)
        this._uwrepl_get_PublisherChanges = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherChanges>(nameof (uwrepl_get_PublisherChanges));
      return this._uwrepl_get_PublisherChanges(pIReplication, ref PublisherChanges);
    }

    internal int uwrepl_get_PublisherConflicts(IntPtr pIReplication, ref int PublisherConflicts)
    {
      if (this._uwrepl_get_PublisherConflicts == null)
        this._uwrepl_get_PublisherConflicts = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PublisherConflicts>(nameof (uwrepl_get_PublisherConflicts));
      return this._uwrepl_get_PublisherConflicts(pIReplication, ref PublisherConflicts);
    }

    internal int uwrepl_get_QueryTimeout(IntPtr pIReplication, ref ushort QueryTimeout)
    {
      if (this._uwrepl_get_QueryTimeout == null)
        this._uwrepl_get_QueryTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_QueryTimeout>(nameof (uwrepl_get_QueryTimeout));
      return this._uwrepl_get_QueryTimeout(pIReplication, ref QueryTimeout);
    }

    internal int uwrepl_put_QueryTimeout(IntPtr pIReplication, ushort QueryTimeout)
    {
      if (this._uwrepl_put_QueryTimeout == null)
        this._uwrepl_put_QueryTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_QueryTimeout>(nameof (uwrepl_put_QueryTimeout));
      return this._uwrepl_put_QueryTimeout(pIReplication, QueryTimeout);
    }

    internal int uwrepl_get_DistributorSecurityMode(
      IntPtr pIReplication,
      ref SecurityType DistributorSecurityMode)
    {
      if (this._uwrepl_get_DistributorSecurityMode == null)
        this._uwrepl_get_DistributorSecurityMode = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DistributorSecurityMode>(nameof (uwrepl_get_DistributorSecurityMode));
      return this._uwrepl_get_DistributorSecurityMode(pIReplication, ref DistributorSecurityMode);
    }

    internal int uwrepl_put_DistributorSecurityMode(
      IntPtr pIReplication,
      SecurityType DistributorSecurityMode)
    {
      if (this._uwrepl_put_DistributorSecurityMode == null)
        this._uwrepl_put_DistributorSecurityMode = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DistributorSecurityMode>(nameof (uwrepl_put_DistributorSecurityMode));
      return this._uwrepl_put_DistributorSecurityMode(pIReplication, DistributorSecurityMode);
    }

    internal int uwrepl_get_DistributorLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_DistributorLogin == null)
        this._uwrepl_get_DistributorLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DistributorLogin>(nameof (uwrepl_get_DistributorLogin));
      return this._uwrepl_get_DistributorLogin(pIReplication, ref rbz);
    }

    internal int uwrepl_put_DistributorLogin(IntPtr pIReplication, string DistributorLogin)
    {
      if (this._uwrepl_put_DistributorLogin == null)
        this._uwrepl_put_DistributorLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DistributorLogin>(nameof (uwrepl_put_DistributorLogin));
      return this._uwrepl_put_DistributorLogin(pIReplication, DistributorLogin);
    }

    internal int uwrepl_get_DistributorPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_DistributorPassword == null)
        this._uwrepl_get_DistributorPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DistributorPassword>(nameof (uwrepl_get_DistributorPassword));
      return this._uwrepl_get_DistributorPassword(pIReplication, ref rbz);
    }

    internal int uwrepl_put_DistributorPassword(IntPtr pIReplication, string DistributorPassword)
    {
      if (this._uwrepl_put_DistributorPassword == null)
        this._uwrepl_put_DistributorPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DistributorPassword>(nameof (uwrepl_put_DistributorPassword));
      return this._uwrepl_put_DistributorPassword(pIReplication, DistributorPassword);
    }

    internal int uwrepl_get_ExchangeType(IntPtr pIReplication, ref ExchangeType ExchangeType)
    {
      if (this._uwrepl_get_ExchangeType == null)
        this._uwrepl_get_ExchangeType = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ExchangeType>(nameof (uwrepl_get_ExchangeType));
      return this._uwrepl_get_ExchangeType(pIReplication, ref ExchangeType);
    }

    internal int uwrepl_put_ExchangeType(IntPtr pIReplication, ExchangeType ExchangeType)
    {
      if (this._uwrepl_put_ExchangeType == null)
        this._uwrepl_put_ExchangeType = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_ExchangeType>(nameof (uwrepl_put_ExchangeType));
      return this._uwrepl_put_ExchangeType(pIReplication, ExchangeType);
    }

    internal int uwrepl_get_HostName(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_HostName == null)
        this._uwrepl_get_HostName = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_HostName>(nameof (uwrepl_get_HostName));
      return this._uwrepl_get_HostName(pIReplication, ref rbz);
    }

    internal int uwrepl_put_HostName(IntPtr pIReplication, string HostName)
    {
      if (this._uwrepl_put_HostName == null)
        this._uwrepl_put_HostName = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_HostName>(nameof (uwrepl_put_HostName));
      return this._uwrepl_put_HostName(pIReplication, HostName);
    }

    internal int uwrepl_get_InternetUrl(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_InternetUrl == null)
        this._uwrepl_get_InternetUrl = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_InternetUrl>(nameof (uwrepl_get_InternetUrl));
      return this._uwrepl_get_InternetUrl(pIReplication, ref rbz);
    }

    internal int uwrepl_put_InternetUrl(IntPtr pIReplication, string InternetUrl)
    {
      if (this._uwrepl_put_InternetUrl == null)
        this._uwrepl_put_InternetUrl = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_InternetUrl>(nameof (uwrepl_put_InternetUrl));
      return this._uwrepl_put_InternetUrl(pIReplication, InternetUrl);
    }

    internal int uwrepl_get_InternetLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_InternetLogin == null)
        this._uwrepl_get_InternetLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_InternetLogin>(nameof (uwrepl_get_InternetLogin));
      return this._uwrepl_get_InternetLogin(pIReplication, ref rbz);
    }

    internal int uwrepl_put_InternetLogin(IntPtr pIReplication, string InternetLogin)
    {
      if (this._uwrepl_put_InternetLogin == null)
        this._uwrepl_put_InternetLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_InternetLogin>(nameof (uwrepl_put_InternetLogin));
      return this._uwrepl_put_InternetLogin(pIReplication, InternetLogin);
    }

    internal int uwrepl_get_InternetPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_InternetPassword == null)
        this._uwrepl_get_InternetPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_InternetPassword>(nameof (uwrepl_get_InternetPassword));
      return this._uwrepl_get_InternetPassword(pIReplication, ref rbz);
    }

    internal int uwrepl_put_InternetPassword(IntPtr pIReplication, string InternetPassword)
    {
      if (this._uwrepl_put_InternetPassword == null)
        this._uwrepl_put_InternetPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_InternetPassword>(nameof (uwrepl_put_InternetPassword));
      return this._uwrepl_put_InternetPassword(pIReplication, InternetPassword);
    }

    internal int uwrepl_get_InternetProxyServer(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_InternetProxyServer == null)
        this._uwrepl_get_InternetProxyServer = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_InternetProxyServer>(nameof (uwrepl_get_InternetProxyServer));
      return this._uwrepl_get_InternetProxyServer(pIReplication, ref rbz);
    }

    internal int uwrepl_put_InternetProxyServer(IntPtr pIReplication, string InternetProxyServer)
    {
      if (this._uwrepl_put_InternetProxyServer == null)
        this._uwrepl_put_InternetProxyServer = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_InternetProxyServer>(nameof (uwrepl_put_InternetProxyServer));
      return this._uwrepl_put_InternetProxyServer(pIReplication, InternetProxyServer);
    }

    internal int uwrepl_get_InternetProxyLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_InternetProxyLogin == null)
        this._uwrepl_get_InternetProxyLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_InternetProxyLogin>(nameof (uwrepl_get_InternetProxyLogin));
      return this._uwrepl_get_InternetProxyLogin(pIReplication, ref rbz);
    }

    internal int uwrepl_put_InternetProxyLogin(IntPtr pIReplication, string InternetProxyLogin)
    {
      if (this._uwrepl_put_InternetProxyLogin == null)
        this._uwrepl_put_InternetProxyLogin = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_InternetProxyLogin>(nameof (uwrepl_put_InternetProxyLogin));
      return this._uwrepl_put_InternetProxyLogin(pIReplication, InternetProxyLogin);
    }

    internal int uwrepl_get_InternetProxyPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_InternetProxyPassword == null)
        this._uwrepl_get_InternetProxyPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_InternetProxyPassword>(nameof (uwrepl_get_InternetProxyPassword));
      return this._uwrepl_get_InternetProxyPassword(pIReplication, ref rbz);
    }

    internal int uwrepl_put_InternetProxyPassword(
      IntPtr pIReplication,
      string InternetProxyPassword)
    {
      if (this._uwrepl_put_InternetProxyPassword == null)
        this._uwrepl_put_InternetProxyPassword = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_InternetProxyPassword>(nameof (uwrepl_put_InternetProxyPassword));
      return this._uwrepl_put_InternetProxyPassword(pIReplication, InternetProxyPassword);
    }

    internal int uwrepl_get_LoginTimeout(IntPtr pIReplication, ref ushort LoginTimeout)
    {
      if (this._uwrepl_get_LoginTimeout == null)
        this._uwrepl_get_LoginTimeout = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_LoginTimeout>(nameof (uwrepl_get_LoginTimeout));
      return this._uwrepl_get_LoginTimeout(pIReplication, ref LoginTimeout);
    }

    internal int uwrepl_Replication(ref IntPtr pIReplication, ref IntPtr pCreationIError)
    {
      if (this._uwrepl_Replication == null)
        this._uwrepl_Replication = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_Replication>(nameof (uwrepl_Replication));
      return this._uwrepl_Replication(ref pIReplication, ref pCreationIError);
    }

    internal int uwrepl_get_ErrorPointer(IntPtr pIReplication, ref IntPtr pIErrors)
    {
      if (this._uwrepl_get_ErrorPointer == null)
        this._uwrepl_get_ErrorPointer = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_ErrorPointer>(nameof (uwrepl_get_ErrorPointer));
      return this._uwrepl_get_ErrorPointer(pIReplication, ref pIErrors);
    }

    internal int uwrepl_get_Distributor(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_Distributor == null)
        this._uwrepl_get_Distributor = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_Distributor>(nameof (uwrepl_get_Distributor));
      return this._uwrepl_get_Distributor(pIReplication, ref rbz);
    }

    internal int uwrepl_put_Distributor(IntPtr pIReplication, string Distributor)
    {
      if (this._uwrepl_put_Distributor == null)
        this._uwrepl_put_Distributor = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_Distributor>(nameof (uwrepl_put_Distributor));
      return this._uwrepl_put_Distributor(pIReplication, Distributor);
    }

    internal int uwrepl_get_PostSyncCleanup(IntPtr pIReplication, ref short iCleanupType)
    {
      if (this._uwrepl_get_PostSyncCleanup == null)
        this._uwrepl_get_PostSyncCleanup = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_PostSyncCleanup>(nameof (uwrepl_get_PostSyncCleanup));
      return this._uwrepl_get_PostSyncCleanup(pIReplication, ref iCleanupType);
    }

    internal int uwrepl_put_PostSyncCleanup(IntPtr pIReplication, short iCleanupType)
    {
      if (this._uwrepl_put_PostSyncCleanup == null)
        this._uwrepl_put_PostSyncCleanup = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_PostSyncCleanup>(nameof (uwrepl_put_PostSyncCleanup));
      return this._uwrepl_put_PostSyncCleanup(pIReplication, iCleanupType);
    }

    internal int uwrepl_get_DistributorNetwork(
      IntPtr pIReplication,
      ref NetworkType DistributorNetwork)
    {
      if (this._uwrepl_get_DistributorNetwork == null)
        this._uwrepl_get_DistributorNetwork = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DistributorNetwork>(nameof (uwrepl_get_DistributorNetwork));
      return this._uwrepl_get_DistributorNetwork(pIReplication, ref DistributorNetwork);
    }

    internal int uwrepl_put_DistributorNetwork(IntPtr pIReplication, NetworkType DistributorNetwork)
    {
      if (this._uwrepl_put_DistributorNetwork == null)
        this._uwrepl_put_DistributorNetwork = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DistributorNetwork>(nameof (uwrepl_put_DistributorNetwork));
      return this._uwrepl_put_DistributorNetwork(pIReplication, DistributorNetwork);
    }

    internal int uwrepl_get_DistributorAddress(IntPtr pIReplication, ref IntPtr rbz)
    {
      if (this._uwrepl_get_DistributorAddress == null)
        this._uwrepl_get_DistributorAddress = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_get_DistributorAddress>(nameof (uwrepl_get_DistributorAddress));
      return this._uwrepl_get_DistributorAddress(pIReplication, ref rbz);
    }

    internal int uwrepl_put_DistributorAddress(IntPtr pIReplication, string DistributorAddress)
    {
      if (this._uwrepl_put_DistributorAddress == null)
        this._uwrepl_put_DistributorAddress = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_put_DistributorAddress>(nameof (uwrepl_put_DistributorAddress));
      return this._uwrepl_put_DistributorAddress(pIReplication, DistributorAddress);
    }

    internal int uwrepl_AsyncReplication(IntPtr pIReplication, ref IntPtr pAsyncIReplication)
    {
      if (this._uwrepl_AsyncReplication == null)
        this._uwrepl_AsyncReplication = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_AsyncReplication>(nameof (uwrepl_AsyncReplication));
      return this._uwrepl_AsyncReplication(pIReplication, ref pAsyncIReplication);
    }

    internal int uwrepl_WaitForNextStatusReport(
      IntPtr pAsyncReplication,
      ref SyncStatus pSyncStatus,
      ref IntPtr rbzTableName,
      ref int pPrecentCompleted,
      ref bool pCompleted)
    {
      if (this._uwrepl_WaitForNextStatusReport == null)
        this._uwrepl_WaitForNextStatusReport = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_WaitForNextStatusReport>(nameof (uwrepl_WaitForNextStatusReport));
      return this._uwrepl_WaitForNextStatusReport(pAsyncReplication, ref pSyncStatus, ref rbzTableName, ref pPrecentCompleted, ref pCompleted);
    }

    internal int uwrepl_GetSyncResult(IntPtr pIReplication, ref int pHr)
    {
      if (this._uwrepl_GetSyncResult == null)
        this._uwrepl_GetSyncResult = this.assemblyHelper.GetUnmanagedFunction<NativeMethodsHelper.delegate_uwrepl_GetSyncResult>(nameof (uwrepl_GetSyncResult));
      return this._uwrepl_GetSyncResult(pIReplication, ref pHr);
    }

    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate void delegate_uwutil_ZeroMemory(IntPtr dest, int length);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_GetSqlCeVersionInfo(ref IntPtr pwszVersion);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetNativeVersionInfo(ref int bldMajor, ref int bldMinor);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetDatabaseInstanceID(
      IntPtr pStore,
      out IntPtr pwszGuidString,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetEncryptionMode(
      IntPtr pStore,
      ref int encryptionMode,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetLocale(IntPtr pStore, ref int locale, IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_GetLocaleFlags(
      IntPtr pStore,
      ref int sortFlags,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_OpenCursor(
      IntPtr pITransact,
      IntPtr pwszTableName,
      IntPtr pwszIndexName,
      ref IntPtr pSeCursor,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_GetValues(
      IntPtr pSeCursor,
      int seGetColumn,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal unsafe delegate int delegate_ME_Read(
      IntPtr pSeqStream,
      void* pBuffer,
      int bufferIndex,
      int byteCount,
      out int bytesRead,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal unsafe delegate int delegate_ME_ReadAt(
      IntPtr pLockBytes,
      int srcIndex,
      void* pBuffer,
      int bufferIndex,
      int byteCount,
      out int bytesRead,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_Seek(
      IntPtr pSeCursor,
      IntPtr pQpServices,
      IntPtr prgBinding,
      int cBinding,
      IntPtr pData,
      int cKeyValues,
      int dbSeekOptions,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_SetRange(
      IntPtr pSeCursor,
      IntPtr pQpServices,
      IntPtr prgBinding,
      int cBinding,
      IntPtr pStartData,
      int cStartKeyValues,
      IntPtr pEndData,
      int cEndKeyValues,
      int dbRangeOptions,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_SafeRelease(ref IntPtr ppUnknown);

    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_SafeDelete(ref IntPtr ppInstance);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_DeleteArray(ref IntPtr ppInstance);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_OpenStore(
      IntPtr pOpenInfo,
      IntPtr pfnOnFlushFailure,
      ref IntPtr pStoreService,
      ref IntPtr pStoreServer,
      ref IntPtr pQpServices,
      ref IntPtr pSeStore,
      ref IntPtr pTx,
      ref IntPtr pQpDatabase,
      ref IntPtr pQpSession,
      ref IntPtr pStoreEvents,
      ref IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_CloseStore(IntPtr pSeStore);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_CloseAndReleaseStore(ref IntPtr pSeStore);

    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_OpenTransaction(
      IntPtr pSeStore,
      IntPtr pQpDatabase,
      SEISOLATION isoLevel,
      IntPtr pQPConnSession,
      ref IntPtr pTx,
      ref IntPtr pQpSession,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_CreateDatabase(IntPtr pOpenInfo, ref IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_Rebuild(
      IntPtr pwszSrc,
      IntPtr pwszDst,
      IntPtr pwszTemp,
      IntPtr pwszPwd,
      IntPtr pwszPwdNew,
      int fEncrypt,
      SEFIXOPTION tyOption,
      int fSafeRepair,
      int lcid,
      int dstEncryptionMode,
      int localeFlags,
      ref IntPtr pError);

    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_CreateErrorInstance(ref IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_uwutil_ConvertToDBTIMESTAMP(
      ref DBTIMESTAMP pDbTimestamp,
      uint dtTime,
      int dtDay);

    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_uwutil_ConvertFromDBTIMESTAMP(
      DBTIMESTAMP pDbTimestamp,
      ref uint dtTime,
      ref int dtDay);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate void delegate_uwutil_SysFreeString(IntPtr p);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate uint delegate_uwutil_ReleaseCOMPtr(IntPtr p);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_uwutil_get_ErrorCount(IntPtr pIRDA);

    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_uwutil_get_Error(
      IntPtr pIError,
      int errno,
      out int hResult,
      out IntPtr message,
      out int nativeError,
      out IntPtr source,
      out int numericParameter1,
      out int numericParameter2,
      out int numericParameter3,
      out IntPtr errorParameter1,
      out IntPtr errorParameter2,
      out IntPtr errorParameter3);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_SetValues(
      IntPtr pQpServices,
      IntPtr pSeCursor,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical(SecurityCriticalScope.Everything)]
    [SuppressUnmanagedCodeSecurity]
    internal unsafe delegate int delegate_ME_SetValue(
      IntPtr pSeCursor,
      int seSetColumn,
      void* pBuffer,
      int ordinal,
      int size,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_Prepare(IntPtr pSeCursor, SEPREPAREMODE mode, IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_InsertRecord(
      int fMoveTo,
      IntPtr pSeCursor,
      ref int hBookmark,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_UpdateRecord(IntPtr pSeCursor, IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_DeleteRecord(IntPtr pSeCursor, IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_GotoBookmark(IntPtr pSeCursor, int hBookmark, IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_GetContextErrorInfo(
      IntPtr pError,
      ref int lNumber,
      ref int lNativeError,
      ref IntPtr pwszMessage,
      ref IntPtr pwszSource,
      ref int numPar1,
      ref int numPar2,
      ref int numPar3,
      ref IntPtr pwszErr1,
      ref IntPtr pwszErr2,
      ref IntPtr pwszErr3);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetContextErrorMessage(
      int dminorError,
      ref IntPtr pwszMessage);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetMinorError(IntPtr pError, ref int lMinor);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetBookmark(
      IntPtr pSeCursor,
      ref int hBookmark,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetColumnInfo(
      IntPtr pIUnknown,
      ref int columnCount,
      ref IntPtr prgColumnInfo,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_SetColumnInfo(
      IntPtr pITransact,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      [MarshalAs(UnmanagedType.LPWStr)] string ColumnName,
      SECOLUMNINFO seColumnInfo,
      SECOLUMNATTRIB seColAttrib,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_SetTableInfoAsSystem(
      IntPtr pITransact,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetParameterInfo(
      IntPtr pQpCommand,
      ref uint columnCount,
      ref IntPtr prgParamInfo,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetIndexColumnOrdinals(
      IntPtr pSeCursor,
      IntPtr pwszIndex,
      ref uint cColumns,
      ref IntPtr priOrdinals,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetKeyInfo(
      IntPtr pIUnknown,
      IntPtr pTx,
      [MarshalAs(UnmanagedType.LPWStr)] string pwszBaseTable,
      IntPtr prgDbKeyInfo,
      int cDbKeyInfo,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_CreateCommand(
      IntPtr pQpSession,
      ref IntPtr pQpCommand,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_CompileQueryPlan(
      IntPtr pQpCommand,
      [MarshalAs(UnmanagedType.LPWStr)] string pwszCommandText,
      ResultSetOptions options,
      IntPtr[] pParamNames,
      IntPtr prgBinding,
      int cDbBinding,
      ref IntPtr pQpPlan,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_Move(IntPtr pSeCursor, DIRECTION direction, IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_AbortTransaction(IntPtr pTx, IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_CommitTransaction(IntPtr pTx, CommitMode mode, IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_SetTransactionFlag(
      IntPtr pITransact,
      SeTransactionFlags seTxFlag,
      [MarshalAs(UnmanagedType.Bool)] bool fEnable,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetTransactionFlags(
      IntPtr pITransact,
      ref SeTransactionFlags seTxFlags);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetTrackingContext(
      IntPtr pITransact,
      out IntPtr pGuidTrackingContext,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_SetTrackingContext(
      IntPtr pITransact,
      ref IntPtr pGuidTrackingContext,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetTransactionBsn(
      IntPtr pITransact,
      ref long pTransactionBsn,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_InitChangeTracking(
      IntPtr pITransact,
      ref IntPtr pTracking,
      ref IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_ExitChangeTracking(ref IntPtr pTracking, ref IntPtr pError);

    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_ME_EnableChangeTracking(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      SETRACKINGTYPE seTrackingType,
      SEOCSTRACKOPTIONS seTrackOpts,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetTrackingOptions(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      ref SEOCSTRACKOPTIONSV2 iTrackingOptions,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_DisableChangeTracking(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_IsTableChangeTracked(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      [MarshalAs(UnmanagedType.Bool)] ref bool fTableTracked,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_GetChangeTrackingInfo(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      ref SEOCSTRACKOPTIONS trackOptions,
      ref SETRACKINGTYPE trackType,
      ref long trackOrdinal,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_CleanupTrackingMetadata(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      int retentionPeriodInDays,
      long cutoffTxCsn,
      long leastTxCsn,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_CleanupTransactionData(
      IntPtr pTracking,
      int iRetentionPeriodInDays,
      long ullCutoffTransactionCsn,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_CleanupTombstoneData(
      IntPtr pTracking,
      [MarshalAs(UnmanagedType.LPWStr)] string TableName,
      int iRetentionPeriodInDays,
      long ullCutoffTransactionCsn,
      IntPtr pError);

    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_GetCurrentTrackingTxCsn(
      IntPtr pTracking,
      ref long txCsn,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_ME_GetCurrentTrackingTxBsn(
      IntPtr pTracking,
      ref long txBsn,
      IntPtr pError);

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_DllRelease();

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    internal delegate int delegate_DllAddRef();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal delegate int delegate_ME_ClearErrorInfo(IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal delegate int delegate_ME_ExecuteQueryPlan(
      IntPtr pTx,
      IntPtr pQpServices,
      IntPtr pQpCommand,
      IntPtr pQpPlan,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      ref int recordsAffected,
      ref ResultSetOptions cursorCapabilities,
      ref IntPtr pSeCursor,
      ref int fIsBaseTableCursor,
      IntPtr pError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_ControlReceiveTimeout(
      IntPtr pIRda,
      int ControlReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ConnectionRetryTimeout(
      IntPtr pIRda,
      ref ushort ConnectionRetryTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_ConnectionRetryTimeout(
      IntPtr pIRda,
      ushort ConnectionRetryTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_CompressionLevel(
      IntPtr pIRda,
      ref ushort CompressionLevel);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_CompressionLevel(IntPtr pIRda, ushort CompressionLevel);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ConnectionManager(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.Bool)] ref bool ConnectionManager);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_ConnectionManager(IntPtr pIRda, [MarshalAs(UnmanagedType.Bool)] bool ConnectionManager);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_Pull(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string zLocalTableName,
      [MarshalAs(UnmanagedType.LPWStr)] string zSqlSelectString,
      [MarshalAs(UnmanagedType.LPWStr)] string zOleDbConnectionString,
      RdaTrackOption trackOption,
      [MarshalAs(UnmanagedType.LPWStr)] string zErrorTable);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_Push(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string zLocalTableName,
      [MarshalAs(UnmanagedType.LPWStr)] string zOleDbConnectionString,
      RdaBatchOption batchOption);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_SubmitSql(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string zSqlString,
      [MarshalAs(UnmanagedType.LPWStr)] string zOleDbConnectionString);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_InternetLogin(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_InternetLogin(IntPtr pIRda, [MarshalAs(UnmanagedType.LPWStr)] string InternetLogin);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_InternetPassword(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_InternetPassword(IntPtr pIRda, [MarshalAs(UnmanagedType.LPWStr)] string InternetPassword);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_InternetProxyServer(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_InternetProxyServer(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetProxyServer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_InternetProxyLogin(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_InternetProxyLogin(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetProxyLogin);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_InternetProxyPassword(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_InternetProxyPassword(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetProxyPassword);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ConnectTimeout(IntPtr pIRda, ref int connectTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_ConnectTimeout(IntPtr pIRda, int connectTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_SendTimeout(IntPtr pIRda, ref int SendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_SendTimeout(IntPtr pIRda, int SendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ReceiveTimeout(IntPtr pIRda, ref int ReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_ReceiveTimeout(IntPtr pIRda, int ReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_DataSendTimeout(IntPtr pIRda, ref int DataSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_DataSendTimeout(IntPtr pIRda, int DataSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_DataReceiveTimeout(
      IntPtr pIRda,
      ref int DataReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_DataReceiveTimeout(
      IntPtr pIRda,
      int DataReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ControlSendTimeout(
      IntPtr pIRda,
      ref int ControlSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_ControlSendTimeout(
      IntPtr pIRda,
      int ControlSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ControlReceiveTimeout(
      IntPtr pIRda,
      ref int ControlReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_RemoteDataAccess(
      ref IntPtr pIRda,
      ref IntPtr pCreationIError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_ErrorPointer(IntPtr pIRda, ref IntPtr pIErrors);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_LocalConnectionString(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_LocalConnectionString(
      IntPtr pIRda,
      [MarshalAs(UnmanagedType.LPWStr)] string zLocalConnectionString);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_get_InternetUrl(IntPtr pIRda, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrda_put_InternetUrl(IntPtr pIRda, [MarshalAs(UnmanagedType.LPWStr)] string InternetUrl);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ConnectionRetryTimeout(
      IntPtr pIReplication,
      ushort ConnectionRetryTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_CompressionLevel(
      IntPtr pIReplication,
      ref ushort CompressionLevel);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_CompressionLevel(
      IntPtr pIReplication,
      ushort CompressionLevel);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ConnectionManager(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.Bool)] ref bool ConnectionManager);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ConnectionManager(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.Bool)] bool ConnectionManager);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_SnapshotTransferType(
      IntPtr pIReplication,
      ref SnapshotTransferType SnapshotTransferType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_SnapshotTransferType(
      IntPtr pIReplication,
      SnapshotTransferType SnapshotTransferType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_AddSubscription(IntPtr pIReplication, AddOption addOption);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_DropSubscription(
      IntPtr pIReplication,
      DropOption dropOption);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_ReinitializeSubscription(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.Bool)] bool uploadBeforeReinit);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_Initialize(IntPtr pIReplication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_Run(IntPtr pIReplication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_Terminate(IntPtr pIReplication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_Cancel(IntPtr pIReplication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_LoadProperties(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.Bool)] ref bool PasswordsLoaded);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_SaveProperties(IntPtr pIReplication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_Subscriber(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_Subscriber(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string Subscriber);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_SubscriberConnectionString(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_SubscriberConnectionString(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string SubscriberConnectionString);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_SubscriberChanges(
      IntPtr pIReplication,
      ref int SubscriberChanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_SubscriberConflicts(
      IntPtr pIReplication,
      ref int SubscriberConflicts);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_Validate(
      IntPtr pIReplication,
      ref ValidateType Validate);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_Validate(IntPtr pIReplication, ValidateType Validate);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ConnectTimeout(
      IntPtr pIReplication,
      ref int connectTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ConnectTimeout(
      IntPtr pIReplication,
      int connectTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_SendTimeout(IntPtr pIReplication, ref int SendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_SendTimeout(IntPtr pIReplication, int SendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ReceiveTimeout(
      IntPtr pIReplication,
      ref int ReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ReceiveTimeout(
      IntPtr pIReplication,
      int ReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DataSendTimeout(
      IntPtr pIReplication,
      ref int DataSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DataSendTimeout(
      IntPtr pIReplication,
      int DataSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DataReceiveTimeout(
      IntPtr pIReplication,
      ref int DataReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DataReceiveTimeout(
      IntPtr pIReplication,
      int DataReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ControlSendTimeout(
      IntPtr pIReplication,
      ref int ControlSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ControlSendTimeout(
      IntPtr pIReplication,
      int ControlSendTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ControlReceiveTimeout(
      IntPtr pIReplication,
      ref int ControlReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ControlReceiveTimeout(
      IntPtr pIReplication,
      int ControlReceiveTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ConnectionRetryTimeout(
      IntPtr pIReplication,
      ref ushort ConnectionRetryTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_LoginTimeout(
      IntPtr pIReplication,
      ushort LoginTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ProfileName(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ProfileName(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string ProfileName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_Publisher(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_Publisher(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string Publisher);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherNetwork(
      IntPtr pIReplication,
      ref NetworkType PublisherNetwork);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PublisherNetwork(
      IntPtr pIReplication,
      NetworkType PublisherNetwork);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherAddress(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PublisherAddress(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string PublisherAddress);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherSecurityMode(
      IntPtr pIReplication,
      ref SecurityType PublisherSecurityMode);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PublisherSecurityMode(
      IntPtr pIReplication,
      SecurityType PublisherSecurityMode);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherLogin(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PublisherLogin(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string PublisherLogin);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherPassword(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PublisherPassword(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string PublisherPassword);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherDatabase(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PublisherDatabase(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string PublisherDatabase);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_Publication(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_Publication(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string Publication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherChanges(
      IntPtr pIReplication,
      ref int PublisherChanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PublisherConflicts(
      IntPtr pIReplication,
      ref int PublisherConflicts);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_QueryTimeout(
      IntPtr pIReplication,
      ref ushort QueryTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_QueryTimeout(
      IntPtr pIReplication,
      ushort QueryTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DistributorSecurityMode(
      IntPtr pIReplication,
      ref SecurityType DistributorSecurityMode);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DistributorSecurityMode(
      IntPtr pIReplication,
      SecurityType DistributorSecurityMode);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DistributorLogin(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DistributorLogin(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string DistributorLogin);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DistributorPassword(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DistributorPassword(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string DistributorPassword);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ExchangeType(
      IntPtr pIReplication,
      ref ExchangeType ExchangeType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_ExchangeType(
      IntPtr pIReplication,
      ExchangeType ExchangeType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_HostName(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_HostName(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string HostName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_InternetUrl(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_InternetUrl(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string InternetUrl);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_InternetLogin(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_InternetLogin(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetLogin);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_InternetPassword(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_InternetPassword(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetPassword);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_InternetProxyServer(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_InternetProxyServer(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetProxyServer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_InternetProxyLogin(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_InternetProxyLogin(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetProxyLogin);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_InternetProxyPassword(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_InternetProxyPassword(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string InternetProxyPassword);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_LoginTimeout(
      IntPtr pIReplication,
      ref ushort LoginTimeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_Replication(
      ref IntPtr pIReplication,
      ref IntPtr pCreationIError);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_ErrorPointer(
      IntPtr pIReplication,
      ref IntPtr pIErrors);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_Distributor(IntPtr pIReplication, ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_Distributor(IntPtr pIReplication, [MarshalAs(UnmanagedType.LPWStr)] string Distributor);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_PostSyncCleanup(
      IntPtr pIReplication,
      ref short iCleanupType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_PostSyncCleanup(
      IntPtr pIReplication,
      short iCleanupType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DistributorNetwork(
      IntPtr pIReplication,
      ref NetworkType DistributorNetwork);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DistributorNetwork(
      IntPtr pIReplication,
      NetworkType DistributorNetwork);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_get_DistributorAddress(
      IntPtr pIReplication,
      ref IntPtr rbz);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_put_DistributorAddress(
      IntPtr pIReplication,
      [MarshalAs(UnmanagedType.LPWStr)] string DistributorAddress);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_AsyncReplication(
      IntPtr pIReplication,
      ref IntPtr pAsyncIReplication);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_WaitForNextStatusReport(
      IntPtr pAsyncReplication,
      ref SyncStatus pSyncStatus,
      ref IntPtr rbzTableName,
      ref int pPrecentCompleted,
      [MarshalAs(UnmanagedType.Bool)] ref bool pCompleted);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int delegate_uwrepl_GetSyncResult(IntPtr pIReplication, ref int pHr);
  }
}
