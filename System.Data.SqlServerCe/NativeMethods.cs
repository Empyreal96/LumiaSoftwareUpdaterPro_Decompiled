// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.NativeMethods
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace System.Data.SqlServerCe
{
  internal static class NativeMethods
  {
    private const int VersionMismatchKB = 974247;
    private static bool m_fTryLoadingNativeLibraries = true;
    private static NativeMethodsHelper NativeMethodsHelper = (NativeMethodsHelper) null;
    private static readonly string ProcArchitecture = (string) null;

    [SecurityCritical]
    static NativeMethods()
    {
      KillBitHelper.ThrowIfKillBitIsSet();
      new EnvironmentPermission(EnvironmentPermissionAccess.Read, "PROCESSOR_ARCHITECTURE").Assert();
      NativeMethods.ProcArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
      CodeAccessPermission.RevertAssert();
    }

    internal static bool Failed(int hr) => hr < 0;

    [SecurityCritical]
    public static void CheckHRESULT(IntPtr pISSCEErrors, int hr)
    {
      if ((2147483648L & (long) hr) == 0L)
        return;
      SqlCeException sqlCeException = SqlCeException.FillErrorCollection(hr, pISSCEErrors);
      if (sqlCeException != null)
        throw sqlCeException;
    }

    [SecurityCritical]
    internal static IntPtr MarshalStringToLPWSTR(string source)
    {
      if (source == null)
        return IntPtr.Zero;
      int length = source.Length;
      int num1 = (length + 1) * 2;
      IntPtr num2 = Marshal.AllocCoTaskMem(num1);
      if (IntPtr.Zero == num2)
        throw new OutOfMemoryException();
      NativeMethods.uwutil_ZeroMemory(num2, num1);
      Marshal.Copy(source.ToCharArray(), 0, num2, length);
      return num2;
    }

    [SecurityCritical]
    internal static unsafe string GetMinorErrorMessage(int minorError)
    {
      IntPtr num = new IntPtr(0);
      string str = (string) null;
      if (NativeMethods.GetContextErrorMessage(minorError, ref num) == 0)
        str = new string((char*) (void*) num);
      NativeMethods.SafeDelete(ref num);
      return str;
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    internal static void LoadNativeBinaries()
    {
      try
      {
        if (!NativeMethods.m_fTryLoadingNativeLibraries)
          return;
        lock (typeof (NativeMethods))
        {
          if (!NativeMethods.m_fTryLoadingNativeLibraries)
            return;
          if (Assembly.GetExecutingAssembly().GlobalAssemblyCache)
          {
            if (NativeMethods.LoadValidLibrary(SqlCeUtil.GetModuleInstallPath("sqlceme40.dll")))
              return;
          }
          else
          {
            string localPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            if (!string.IsNullOrEmpty(localPath) && NativeMethods.LoadNativeBinariesFromPrivateFolder(Path.GetDirectoryName(localPath)))
              return;
          }
          Assembly entryAssembly = Assembly.GetEntryAssembly();
          if ((entryAssembly == null || !NativeMethods.LoadNativeBinariesFromPrivateFolder(Path.GetDirectoryName(entryAssembly.Location))) && NativeMethods.m_fTryLoadingNativeLibraries)
            throw SqlCeException.CreateException(string.Format(Res.GetString("ADP_LoadNativeBinaryFail"), (object) SqlCeVersion.BuildMajor, (object) 974247));
        }
      }
      catch (SqlCeException ex)
      {
        throw;
      }
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    public static bool IsValidBinary(string filename)
    {
      bool flag = HashProvider.MatchHash(filename);
      return flag ? flag : throw SqlCeException.CreateException(string.Format(Res.GetString("SQLCE_NativeBinaryIsNotProper"), (object) "sqlceme40.dll"));
    }

    private static void ThrowIfNativeLibraryNotLoaded()
    {
      if (NativeMethods.NativeMethodsHelper == null)
        throw SqlCeException.CreateException(Res.GetString("SQLCE_NativeEngineNotLoaded"));
    }

    [SecurityCritical]
    internal static bool ValidateNativeBinary(string modulePath) => !string.IsNullOrEmpty(modulePath) && NativeMethods.IsValidBinary(modulePath);

    [SecurityCritical]
    internal static unsafe bool LoadValidLibrary(string modulePath)
    {
      FileStream fileStream1 = (FileStream) null;
      FileStream fileStream2 = (FileStream) null;
      FileStream fileStream3 = (FileStream) null;
      if (string.IsNullOrEmpty(modulePath.Trim()))
        return false;
      try
      {
        new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, modulePath).Assert();
        if (!File.Exists(modulePath))
          return false;
        fileStream1 = File.Open(modulePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (!NativeMethods.ValidateNativeBinary(modulePath))
          return false;
        string str1 = Path.Combine(Path.GetDirectoryName(modulePath), "msvcr90.dll");
        if (File.Exists(str1))
        {
          fileStream2 = File.Open(str1, FileMode.Open, FileAccess.Read, FileShare.Read);
          NativeMethods.ValidateCRT(str1);
        }
        string str2 = Path.Combine(Path.Combine(Path.GetDirectoryName(modulePath), "Microsoft.VC90.CRT"), "msvcr90.dll");
        if (File.Exists(str2))
        {
          fileStream3 = File.Open(str2, FileMode.Open, FileAccess.Read, FileShare.Read);
          NativeMethods.ValidateCRT(str2);
        }
        IntPtr zero = IntPtr.Zero;
        NativeMethodsHelper nativeMethodsHelper = new NativeMethodsHelper(modulePath);
        nativeMethodsHelper.GetSqlCeVersionInfo(ref zero);
        string version = new string((char*) (void*) zero);
        nativeMethodsHelper.SafeDelete(ref zero);
        if (string.IsNullOrEmpty(version))
        {
          nativeMethodsHelper.Dispose();
          return false;
        }
        if (new Version(version).Build != SqlCeVersion.BuildMajor)
        {
          nativeMethodsHelper.Dispose();
          return false;
        }
        NativeMethods.NativeMethodsHelper = nativeMethodsHelper;
        NativeMethods.m_fTryLoadingNativeLibraries = false;
        return true;
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
        fileStream1?.Close();
        fileStream2?.Close();
        fileStream3?.Close();
      }
    }

    [SecurityCritical]
    private static void ValidateCRT(string CRTPath)
    {
      bool flag = new WinTrust().VerifyEmbeddedSignature(CRTPath);
      if (flag)
      {
        try
        {
          X509Certificate2 x509Certificate2 = new X509Certificate2(CRTPath);
          flag &= x509Certificate2.GetPublicKeyString().Equals("3082010a0282010100bd72b489e71c9f85c774b8605c03363d9cfd997a9a294622b0a78753edee463ac75b050b57a8b7ca05ccd34c77477085b3e5cbdf67e7a3fd742793679fd78a034430c6f7c9bac93a1d0856444f17080df9b41968aa241cfb055785e9c54e072137a7ebce2c2fb642cd2105a7d6e6d32857c71b7ace293607cd9e55ccbbf122eba823a40d29c2fbd0c35a3e633dc72c490b7b7985f088ef71bd435ae3a3b30df355fb25e0e220d3e79a5e94a5332d287f571b556a0c3244ef666c6ff0389cef02ad9aa1dd9807100e3c1869e2794e4614e0b98cd0756d9cac009c2d42f551b85af4784583e92e7c2bbb5dcd196128ad94430ac56a42ffb532aea42922de16e8d30203010001", StringComparison.OrdinalIgnoreCase);
        }
        catch (CryptographicException ex)
        {
          throw SqlCeException.CreateException(string.Format("{0} : {1}", (object) "sqlceme40.dll", (object) ex.Message), (Exception) ex);
        }
      }
      if (!flag)
        throw SqlCeException.CreateException(Res.GetString("SQLCE_CRTNotSigned"));
    }

    [SecurityCritical]
    internal static bool LoadNativeBinariesFromPrivateFolder(string privateInstall)
    {
      bool flag = false;
      if (NativeMethods.LoadValidLibrary(Path.Combine(privateInstall, "sqlceme40.dll")))
        flag = true;
      if (!flag && NativeMethods.LoadValidLibrary(Path.Combine(Path.Combine(privateInstall, NativeMethods.ProcArchitecture), "sqlceme40.dll")))
        flag = true;
      if (flag && !SqlCeUtil.IsWebHosted && !string.IsNullOrEmpty(privateInstall))
        SqlCeServicing.DoBreadcrumbServicing(privateInstall);
      return flag;
    }

    [SecurityCritical]
    internal static void uwutil_ZeroMemory(IntPtr dest, int length)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      NativeMethods.NativeMethodsHelper.uwutil_ZeroMemory(dest, length);
    }

    [SecurityCritical]
    internal static int GetSqlCeVersionInfo(ref IntPtr pwszVersion)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetSqlCeVersionInfo(ref pwszVersion);
    }

    [SecurityCritical]
    internal static int GetNativeVersionInfo(ref int bldMajor, ref int bldMinor)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetNativeVersionInfo(ref bldMajor, ref bldMinor);
    }

    [SecurityCritical]
    internal static int GetDatabaseInstanceID(
      IntPtr pStore,
      out IntPtr pwszGuidString,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetDatabaseInstanceID(pStore, out pwszGuidString, pError);
    }

    [SecurityCritical]
    internal static int GetEncryptionMode(IntPtr pStore, ref int encryptionMode, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetEncryptionMode(pStore, ref encryptionMode, pError);
    }

    [SecurityCritical]
    internal static int GetLocale(IntPtr pStore, ref int locale, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetLocale(pStore, ref locale, pError);
    }

    [SecurityCritical]
    internal static int GetLocaleFlags(IntPtr pStore, ref int sortFlags, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetLocaleFlags(pStore, ref sortFlags, pError);
    }

    [SecurityCritical]
    internal static int OpenCursor(
      IntPtr pITransact,
      IntPtr pwszTableName,
      IntPtr pwszIndexName,
      ref IntPtr pSeCursor,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.OpenCursor(pITransact, pwszTableName, pwszIndexName, ref pSeCursor, pError);
    }

    [SecurityCritical]
    internal static int GetValues(
      IntPtr pSeCursor,
      int seGetColumn,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetValues(pSeCursor, seGetColumn, prgBinding, cDbBinding, pData, pError);
    }

    [SecurityCritical]
    internal static unsafe int Read(
      IntPtr pSeqStream,
      void* pBuffer,
      int bufferIndex,
      int byteCount,
      out int bytesRead,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.Read(pSeqStream, pBuffer, bufferIndex, byteCount, out bytesRead, pError);
    }

    [SecurityCritical]
    internal static unsafe int ReadAt(
      IntPtr pLockBytes,
      int srcIndex,
      void* pBuffer,
      int bufferIndex,
      int byteCount,
      out int bytesRead,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.ReadAt(pLockBytes, srcIndex, pBuffer, bufferIndex, byteCount, out bytesRead, pError);
    }

    [SecurityCritical]
    internal static int Seek(
      IntPtr pSeCursor,
      IntPtr pQpServices,
      IntPtr prgBinding,
      int cBinding,
      IntPtr pData,
      int cKeyValues,
      int dbSeekOptions,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.Seek(pSeCursor, pQpServices, prgBinding, cBinding, pData, cKeyValues, dbSeekOptions, pError);
    }

    [SecurityCritical]
    internal static int SetRange(
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
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetRange(pSeCursor, pQpServices, prgBinding, cBinding, pStartData, cStartKeyValues, pEndData, cEndKeyValues, dbRangeOptions, pError);
    }

    [SecurityCritical]
    internal static int SafeRelease(ref IntPtr ppUnknown)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SafeRelease(ref ppUnknown);
    }

    [SecurityCritical]
    internal static int SafeDelete(ref IntPtr ppInstance)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SafeDelete(ref ppInstance);
    }

    [SecurityCritical]
    internal static int DeleteArray(ref IntPtr ppInstance)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.DeleteArray(ref ppInstance);
    }

    [SecurityCritical]
    internal static int OpenStore(
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
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.OpenStore(pOpenInfo, pfnOnFlushFailure, ref pStoreService, ref pStoreServer, ref pQpServices, ref pSeStore, ref pTx, ref pQpDatabase, ref pQpSession, ref pStoreEvents, ref pError);
    }

    [SecurityCritical]
    internal static int CloseStore(IntPtr pSeStore)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CloseStore(pSeStore);
    }

    [SecurityCritical]
    internal static int CloseAndReleaseStore(ref IntPtr pSeStore)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CloseAndReleaseStore(ref pSeStore);
    }

    [SecurityCritical]
    internal static int OpenTransaction(
      IntPtr pSeStore,
      IntPtr pQpDatabase,
      SEISOLATION isoLevel,
      IntPtr pQPConnSession,
      ref IntPtr pTx,
      ref IntPtr pQpSession,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.OpenTransaction(pSeStore, pQpDatabase, isoLevel, pQPConnSession, ref pTx, ref pQpSession, pError);
    }

    [SecurityCritical]
    internal static int CreateDatabase(IntPtr pOpenInfo, ref IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CreateDatabase(pOpenInfo, ref pError);
    }

    [SecurityCritical]
    internal static int Rebuild(
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
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.Rebuild(pwszSrc, pwszDst, pwszTemp, pwszPwd, pwszPwdNew, fEncrypt, tyOption, fSafeRepair, lcid, dstEncryptionMode, localeFlags, ref pError);
    }

    [SecurityCritical]
    internal static int CreateErrorInstance(ref IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CreateErrorInstance(ref pError);
    }

    [SecurityCritical]
    internal static int uwutil_ConvertToDBTIMESTAMP(
      ref DBTIMESTAMP pDbTimestamp,
      uint dtTime,
      int dtDay)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwutil_ConvertToDBTIMESTAMP(ref pDbTimestamp, dtTime, dtDay);
    }

    [SecurityCritical]
    internal static int uwutil_ConvertFromDBTIMESTAMP(
      DBTIMESTAMP pDbTimestamp,
      ref uint dtTime,
      ref int dtDay)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwutil_ConvertFromDBTIMESTAMP(pDbTimestamp, ref dtTime, ref dtDay);
    }

    [SecurityCritical]
    internal static void uwutil_SysFreeString(IntPtr p)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      NativeMethods.NativeMethodsHelper.uwutil_SysFreeString(p);
    }

    [SecurityCritical]
    internal static uint uwutil_ReleaseCOMPtr(IntPtr p)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwutil_ReleaseCOMPtr(p);
    }

    [SecurityCritical]
    internal static int uwutil_get_ErrorCount(IntPtr pIRDA)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwutil_get_ErrorCount(pIRDA);
    }

    [SecurityCritical]
    internal static int uwutil_get_Error(
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
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwutil_get_Error(pIError, errno, out hResult, out message, out nativeError, out source, out numericParameter1, out numericParameter2, out numericParameter3, out errorParameter1, out errorParameter2, out errorParameter3);
    }

    [SecurityCritical]
    internal static int SetValues(
      IntPtr pQpServices,
      IntPtr pSeCursor,
      IntPtr prgBinding,
      int cDbBinding,
      IntPtr pData,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetValues(pQpServices, pSeCursor, prgBinding, cDbBinding, pData, pError);
    }

    [SecurityCritical]
    internal static unsafe int SetValue(
      IntPtr pSeCursor,
      int seSetColumn,
      void* pBuffer,
      int ordinal,
      int size,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetValue(pSeCursor, seSetColumn, pBuffer, ordinal, size, pError);
    }

    [SecurityCritical]
    internal static int Prepare(IntPtr pSeCursor, SEPREPAREMODE mode, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.Prepare(pSeCursor, mode, pError);
    }

    [SecurityCritical]
    internal static int InsertRecord(
      int fMoveTo,
      IntPtr pSeCursor,
      ref int hBookmark,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.InsertRecord(fMoveTo, pSeCursor, ref hBookmark, pError);
    }

    [SecurityCritical]
    internal static int UpdateRecord(IntPtr pSeCursor, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.UpdateRecord(pSeCursor, pError);
    }

    [SecurityCritical]
    internal static int DeleteRecord(IntPtr pSeCursor, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.DeleteRecord(pSeCursor, pError);
    }

    [SecurityCritical]
    internal static int GotoBookmark(IntPtr pSeCursor, int hBookmark, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GotoBookmark(pSeCursor, hBookmark, pError);
    }

    [SecurityCritical]
    internal static int GetContextErrorInfo(
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
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetContextErrorInfo(pError, ref lNumber, ref lNativeError, ref pwszMessage, ref pwszSource, ref numPar1, ref numPar2, ref numPar3, ref pwszErr1, ref pwszErr2, ref pwszErr3);
    }

    [SecurityCritical]
    internal static int GetContextErrorMessage(int dminorError, ref IntPtr pwszMessage)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetContextErrorMessage(dminorError, ref pwszMessage);
    }

    [SecurityCritical]
    internal static int GetMinorError(IntPtr pError, ref int lMinor)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetMinorError(pError, ref lMinor);
    }

    [SecurityCritical]
    internal static int GetBookmark(IntPtr pSeCursor, ref int hBookmark, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetBookmark(pSeCursor, ref hBookmark, pError);
    }

    [SecurityCritical]
    internal static int GetColumnInfo(
      IntPtr pIUnknown,
      ref int columnCount,
      ref IntPtr prgColumnInfo,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetColumnInfo(pIUnknown, ref columnCount, ref prgColumnInfo, pError);
    }

    [SecurityCritical]
    internal static int SetColumnInfo(
      IntPtr pITransact,
      string TableName,
      string ColumnName,
      SECOLUMNINFO seColumnInfo,
      SECOLUMNATTRIB seColAttrib,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetColumnInfo(pITransact, TableName, ColumnName, seColumnInfo, seColAttrib, pError);
    }

    [SecurityCritical]
    internal static int SetTableInfoAsSystem(IntPtr pITransact, string TableName, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetTableInfoAsSystem(pITransact, TableName, pError);
    }

    [SecurityCritical]
    internal static int GetParameterInfo(
      IntPtr pQpCommand,
      ref uint columnCount,
      ref IntPtr prgParamInfo,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetParameterInfo(pQpCommand, ref columnCount, ref prgParamInfo, pError);
    }

    [SecurityCritical]
    internal static int GetIndexColumnOrdinals(
      IntPtr pSeCursor,
      IntPtr pwszIndex,
      ref uint cColumns,
      ref IntPtr priOrdinals,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetIndexColumnOrdinals(pSeCursor, pwszIndex, ref cColumns, ref priOrdinals, pError);
    }

    [SecurityCritical]
    internal static int GetKeyInfo(
      IntPtr pIUnknown,
      IntPtr pTx,
      string pwszBaseTable,
      IntPtr prgDbKeyInfo,
      int cDbKeyInfo,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetKeyInfo(pIUnknown, pTx, pwszBaseTable, prgDbKeyInfo, cDbKeyInfo, pError);
    }

    [SecurityCritical]
    internal static int CreateCommand(IntPtr pQpSession, ref IntPtr pQpCommand, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CreateCommand(pQpSession, ref pQpCommand, pError);
    }

    [SecurityCritical]
    internal static int CompileQueryPlan(
      IntPtr pQpCommand,
      string pwszCommandText,
      ResultSetOptions options,
      IntPtr[] pParamNames,
      IntPtr prgBinding,
      int cDbBinding,
      ref IntPtr pQpPlan,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CompileQueryPlan(pQpCommand, pwszCommandText, options, pParamNames, prgBinding, cDbBinding, ref pQpPlan, pError);
    }

    [SecurityCritical]
    internal static int Move(IntPtr pSeCursor, DIRECTION direction, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.Move(pSeCursor, direction, pError);
    }

    [SecurityCritical]
    internal static int AbortTransaction(IntPtr pTx, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.AbortTransaction(pTx, pError);
    }

    [SecurityCritical]
    internal static int CommitTransaction(IntPtr pTx, CommitMode mode, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CommitTransaction(pTx, mode, pError);
    }

    [SecurityCritical]
    internal static int SetTransactionFlag(
      IntPtr pITransact,
      SeTransactionFlags seTxFlag,
      bool fEnable,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetTransactionFlag(pITransact, seTxFlag, fEnable, pError);
    }

    [SecurityCritical]
    internal static int GetTransactionFlags(IntPtr pITransact, ref SeTransactionFlags seTxFlags)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetTransactionFlags(pITransact, ref seTxFlags);
    }

    [SecurityCritical]
    internal static int GetTrackingContext(
      IntPtr pITransact,
      out IntPtr pGuidTrackingContext,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetTrackingContext(pITransact, out pGuidTrackingContext, pError);
    }

    [SecurityCritical]
    internal static int SetTrackingContext(
      IntPtr pITransact,
      ref IntPtr pGuidTrackingContext,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.SetTrackingContext(pITransact, ref pGuidTrackingContext, pError);
    }

    [SecurityCritical]
    internal static int GetTransactionBsn(
      IntPtr pITransact,
      ref long pTransactionBsn,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetTransactionBsn(pITransact, ref pTransactionBsn, pError);
    }

    [SecurityCritical]
    internal static int InitChangeTracking(
      IntPtr pITransact,
      ref IntPtr pTracking,
      ref IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.InitChangeTracking(pITransact, ref pTracking, ref pError);
    }

    [SecurityCritical]
    internal static int ExitChangeTracking(ref IntPtr pTracking, ref IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.ExitChangeTracking(ref pTracking, ref pError);
    }

    [SecurityCritical]
    internal static int EnableChangeTracking(
      IntPtr pTracking,
      string TableName,
      SETRACKINGTYPE seTrackingType,
      SEOCSTRACKOPTIONS seTrackOpts,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.EnableChangeTracking(pTracking, TableName, seTrackingType, seTrackOpts, pError);
    }

    [SecurityCritical]
    internal static int GetTrackingOptions(
      IntPtr pTracking,
      string TableName,
      ref SEOCSTRACKOPTIONSV2 iTrackingOptions,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetTrackingOptions(pTracking, TableName, ref iTrackingOptions, pError);
    }

    [SecurityCritical]
    internal static int DisableChangeTracking(IntPtr pTracking, string TableName, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.DisableChangeTracking(pTracking, TableName, pError);
    }

    [SecurityCritical]
    internal static int IsTableChangeTracked(
      IntPtr pTracking,
      string TableName,
      ref bool fTableTracked,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.IsTableChangeTracked(pTracking, TableName, ref fTableTracked, pError);
    }

    [SecurityCritical]
    internal static int GetChangeTrackingInfo(
      IntPtr pTracking,
      string TableName,
      ref SEOCSTRACKOPTIONS trackOptions,
      ref SETRACKINGTYPE trackType,
      ref long trackOrdinal,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetChangeTrackingInfo(pTracking, TableName, ref trackOptions, ref trackType, ref trackOrdinal, pError);
    }

    [SecurityCritical]
    internal static int CleanupTrackingMetadata(
      IntPtr pTracking,
      string TableName,
      int retentionPeriodInDays,
      long cutoffTxCsn,
      long leastTxCsn,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CleanupTrackingMetadata(pTracking, TableName, retentionPeriodInDays, cutoffTxCsn, leastTxCsn, pError);
    }

    [SecurityCritical]
    internal static int CleanupTransactionData(
      IntPtr pTracking,
      int iRetentionPeriodInDays,
      long ullCutoffTransactionCsn,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CleanupTransactionData(pTracking, iRetentionPeriodInDays, ullCutoffTransactionCsn, pError);
    }

    [SecurityCritical]
    internal static int CleanupTombstoneData(
      IntPtr pTracking,
      string TableName,
      int iRetentionPeriodInDays,
      long ullCutoffTransactionCsn,
      IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.CleanupTombstoneData(pTracking, TableName, iRetentionPeriodInDays, ullCutoffTransactionCsn, pError);
    }

    [SecurityCritical]
    internal static int GetCurrentTrackingTxCsn(IntPtr pTracking, ref long txCsn, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetCurrentTrackingTxCsn(pTracking, ref txCsn, pError);
    }

    [SecurityCritical]
    internal static int GetCurrentTrackingTxBsn(IntPtr pTracking, ref long txBsn, IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.GetCurrentTrackingTxBsn(pTracking, ref txBsn, pError);
    }

    [SecurityCritical]
    internal static int DllAddRef()
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.DllAddRef();
    }

    [SecurityCritical]
    internal static int DllRelease()
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.DllRelease();
    }

    [SecurityCritical]
    internal static int ClearErrorInfo(IntPtr pError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.ClearErrorInfo(pError);
    }

    [SecurityCritical]
    internal static int ExecuteQueryPlan(
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
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.ExecuteQueryPlan(pTx, pQpServices, pQpCommand, pQpPlan, prgBinding, cDbBinding, pData, ref recordsAffected, ref cursorCapabilities, ref pSeCursor, ref fIsBaseTableCursor, pError);
    }

    internal static int uwrda_put_ControlReceiveTimeout(IntPtr pIRda, int ControlReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_ControlReceiveTimeout(pIRda, ControlReceiveTimeout);
    }

    internal static int uwrda_get_ConnectionRetryTimeout(
      IntPtr pIRda,
      ref ushort ConnectionRetryTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ConnectionRetryTimeout(pIRda, ref ConnectionRetryTimeout);
    }

    internal static int uwrda_put_ConnectionRetryTimeout(
      IntPtr pIRda,
      ushort ConnectionRetryTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_ConnectionRetryTimeout(pIRda, ConnectionRetryTimeout);
    }

    internal static int uwrda_get_CompressionLevel(IntPtr pIRda, ref ushort CompressionLevel)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_CompressionLevel(pIRda, ref CompressionLevel);
    }

    internal static int uwrda_put_CompressionLevel(IntPtr pIRda, ushort CompressionLevel)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_CompressionLevel(pIRda, CompressionLevel);
    }

    internal static int uwrda_get_ConnectionManager(IntPtr pIRda, ref bool ConnectionManager)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ConnectionManager(pIRda, ref ConnectionManager);
    }

    internal static int uwrda_put_ConnectionManager(IntPtr pIRda, bool ConnectionManager)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_ConnectionManager(pIRda, ConnectionManager);
    }

    internal static int uwrda_Pull(
      IntPtr pIRda,
      string zLocalTableName,
      string zSqlSelectString,
      string zOleDbConnectionString,
      RdaTrackOption trackOption,
      string zErrorTable)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_Pull(pIRda, zLocalTableName, zSqlSelectString, zOleDbConnectionString, trackOption, zErrorTable);
    }

    internal static int uwrda_Push(
      IntPtr pIRda,
      string zLocalTableName,
      string zOleDbConnectionString,
      RdaBatchOption batchOption)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_Push(pIRda, zLocalTableName, zOleDbConnectionString, batchOption);
    }

    internal static int uwrda_SubmitSql(
      IntPtr pIRda,
      string zSqlString,
      string zOleDbConnectionString)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_SubmitSql(pIRda, zSqlString, zOleDbConnectionString);
    }

    internal static int uwrda_get_InternetLogin(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_InternetLogin(pIRda, ref rbz);
    }

    internal static int uwrda_put_InternetLogin(IntPtr pIRda, string InternetLogin)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_InternetLogin(pIRda, InternetLogin);
    }

    internal static int uwrda_get_InternetPassword(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_InternetPassword(pIRda, ref rbz);
    }

    internal static int uwrda_put_InternetPassword(IntPtr pIRda, string InternetPassword)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_InternetPassword(pIRda, InternetPassword);
    }

    internal static int uwrda_get_InternetProxyServer(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_InternetProxyServer(pIRda, ref rbz);
    }

    internal static int uwrda_put_InternetProxyServer(IntPtr pIRda, string InternetProxyServer)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_InternetProxyServer(pIRda, InternetProxyServer);
    }

    internal static int uwrda_get_InternetProxyLogin(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_InternetProxyLogin(pIRda, ref rbz);
    }

    internal static int uwrda_put_InternetProxyLogin(IntPtr pIRda, string InternetProxyLogin)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_InternetProxyLogin(pIRda, InternetProxyLogin);
    }

    internal static int uwrda_get_InternetProxyPassword(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_InternetProxyPassword(pIRda, ref rbz);
    }

    internal static int uwrda_put_InternetProxyPassword(IntPtr pIRda, string InternetProxyPassword)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_InternetProxyPassword(pIRda, InternetProxyPassword);
    }

    internal static int uwrda_get_ConnectTimeout(IntPtr pIRda, ref int connectTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ConnectTimeout(pIRda, ref connectTimeout);
    }

    internal static int uwrda_put_ConnectTimeout(IntPtr pIRda, int connectTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_ConnectTimeout(pIRda, connectTimeout);
    }

    internal static int uwrda_get_SendTimeout(IntPtr pIRda, ref int SendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_SendTimeout(pIRda, ref SendTimeout);
    }

    internal static int uwrda_put_SendTimeout(IntPtr pIRda, int SendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_SendTimeout(pIRda, SendTimeout);
    }

    internal static int uwrda_get_ReceiveTimeout(IntPtr pIRda, ref int ReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ReceiveTimeout(pIRda, ref ReceiveTimeout);
    }

    internal static int uwrda_put_ReceiveTimeout(IntPtr pIRda, int ReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_ReceiveTimeout(pIRda, ReceiveTimeout);
    }

    internal static int uwrda_get_DataSendTimeout(IntPtr pIRda, ref int DataSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_DataSendTimeout(pIRda, ref DataSendTimeout);
    }

    internal static int uwrda_put_DataSendTimeout(IntPtr pIRda, int DataSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_DataSendTimeout(pIRda, DataSendTimeout);
    }

    internal static int uwrda_get_DataReceiveTimeout(IntPtr pIRda, ref int DataReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_DataReceiveTimeout(pIRda, ref DataReceiveTimeout);
    }

    internal static int uwrda_put_DataReceiveTimeout(IntPtr pIRda, int DataReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_DataReceiveTimeout(pIRda, DataReceiveTimeout);
    }

    internal static int uwrda_get_ControlSendTimeout(IntPtr pIRda, ref int ControlSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ControlSendTimeout(pIRda, ref ControlSendTimeout);
    }

    internal static int uwrda_put_ControlSendTimeout(IntPtr pIRda, int ControlSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_ControlSendTimeout(pIRda, ControlSendTimeout);
    }

    internal static int uwrda_get_ControlReceiveTimeout(IntPtr pIRda, ref int ControlReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ControlReceiveTimeout(pIRda, ref ControlReceiveTimeout);
    }

    internal static int uwrda_RemoteDataAccess(ref IntPtr pIRda, ref IntPtr pCreationIError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_RemoteDataAccess(ref pIRda, ref pCreationIError);
    }

    internal static int uwrda_get_ErrorPointer(IntPtr pIRda, ref IntPtr pIErrors)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_ErrorPointer(pIRda, ref pIErrors);
    }

    internal static int uwrda_get_LocalConnectionString(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_LocalConnectionString(pIRda, ref rbz);
    }

    internal static int uwrda_put_LocalConnectionString(IntPtr pIRda, string zLocalConnectionString)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_LocalConnectionString(pIRda, zLocalConnectionString);
    }

    internal static int uwrda_get_InternetUrl(IntPtr pIRda, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_get_InternetUrl(pIRda, ref rbz);
    }

    internal static int uwrda_put_InternetUrl(IntPtr pIRda, string InternetUrl)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrda_put_InternetUrl(pIRda, InternetUrl);
    }

    internal static int uwrepl_put_ConnectionRetryTimeout(
      IntPtr pIReplication,
      ushort ConnectionRetryTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ConnectionRetryTimeout(pIReplication, ConnectionRetryTimeout);
    }

    internal static int uwrepl_get_CompressionLevel(
      IntPtr pIReplication,
      ref ushort CompressionLevel)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_CompressionLevel(pIReplication, ref CompressionLevel);
    }

    internal static int uwrepl_put_CompressionLevel(IntPtr pIReplication, ushort CompressionLevel)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_CompressionLevel(pIReplication, CompressionLevel);
    }

    internal static int uwrepl_get_ConnectionManager(
      IntPtr pIReplication,
      ref bool ConnectionManager)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ConnectionManager(pIReplication, ref ConnectionManager);
    }

    internal static int uwrepl_put_ConnectionManager(IntPtr pIReplication, bool ConnectionManager)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ConnectionManager(pIReplication, ConnectionManager);
    }

    internal static int uwrepl_get_SnapshotTransferType(
      IntPtr pIReplication,
      ref SnapshotTransferType SnapshotTransferType)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_SnapshotTransferType(pIReplication, ref SnapshotTransferType);
    }

    internal static int uwrepl_put_SnapshotTransferType(
      IntPtr pIReplication,
      SnapshotTransferType SnapshotTransferType)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_SnapshotTransferType(pIReplication, SnapshotTransferType);
    }

    internal static int uwrepl_AddSubscription(IntPtr pIReplication, AddOption addOption)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_AddSubscription(pIReplication, addOption);
    }

    internal static int uwrepl_DropSubscription(IntPtr pIReplication, DropOption dropOption)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_DropSubscription(pIReplication, dropOption);
    }

    internal static int uwrepl_ReinitializeSubscription(
      IntPtr pIReplication,
      bool uploadBeforeReinit)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_ReinitializeSubscription(pIReplication, uploadBeforeReinit);
    }

    internal static int uwrepl_Initialize(IntPtr pIReplication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_Initialize(pIReplication);
    }

    internal static int uwrepl_Run(IntPtr pIReplication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_Run(pIReplication);
    }

    internal static int uwrepl_Terminate(IntPtr pIReplication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_Terminate(pIReplication);
    }

    internal static int uwrepl_Cancel(IntPtr pIReplication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_Cancel(pIReplication);
    }

    internal static int uwrepl_LoadProperties(IntPtr pIReplication, ref bool PasswordsLoaded)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_LoadProperties(pIReplication, ref PasswordsLoaded);
    }

    internal static int uwrepl_SaveProperties(IntPtr pIReplication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_SaveProperties(pIReplication);
    }

    internal static int uwrepl_get_Subscriber(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_Subscriber(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_Subscriber(IntPtr pIReplication, string Subscriber)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_Subscriber(pIReplication, Subscriber);
    }

    internal static int uwrepl_get_SubscriberConnectionString(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_SubscriberConnectionString(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_SubscriberConnectionString(
      IntPtr pIReplication,
      string SubscriberConnectionString)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_SubscriberConnectionString(pIReplication, SubscriberConnectionString);
    }

    internal static int uwrepl_get_SubscriberChanges(
      IntPtr pIReplication,
      ref int SubscriberChanges)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_SubscriberChanges(pIReplication, ref SubscriberChanges);
    }

    internal static int uwrepl_get_SubscriberConflicts(
      IntPtr pIReplication,
      ref int SubscriberConflicts)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_SubscriberConflicts(pIReplication, ref SubscriberConflicts);
    }

    internal static int uwrepl_get_Validate(IntPtr pIReplication, ref ValidateType Validate)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_Validate(pIReplication, ref Validate);
    }

    internal static int uwrepl_put_Validate(IntPtr pIReplication, ValidateType Validate)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_Validate(pIReplication, Validate);
    }

    internal static int uwrepl_get_ConnectTimeout(IntPtr pIReplication, ref int connectTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ConnectTimeout(pIReplication, ref connectTimeout);
    }

    internal static int uwrepl_put_ConnectTimeout(IntPtr pIReplication, int connectTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ConnectTimeout(pIReplication, connectTimeout);
    }

    internal static int uwrepl_get_SendTimeout(IntPtr pIReplication, ref int SendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_SendTimeout(pIReplication, ref SendTimeout);
    }

    internal static int uwrepl_put_SendTimeout(IntPtr pIReplication, int SendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_SendTimeout(pIReplication, SendTimeout);
    }

    internal static int uwrepl_get_ReceiveTimeout(IntPtr pIReplication, ref int ReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ReceiveTimeout(pIReplication, ref ReceiveTimeout);
    }

    internal static int uwrepl_put_ReceiveTimeout(IntPtr pIReplication, int ReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ReceiveTimeout(pIReplication, ReceiveTimeout);
    }

    internal static int uwrepl_get_DataSendTimeout(IntPtr pIReplication, ref int DataSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DataSendTimeout(pIReplication, ref DataSendTimeout);
    }

    internal static int uwrepl_put_DataSendTimeout(IntPtr pIReplication, int DataSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DataSendTimeout(pIReplication, DataSendTimeout);
    }

    internal static int uwrepl_get_DataReceiveTimeout(
      IntPtr pIReplication,
      ref int DataReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DataReceiveTimeout(pIReplication, ref DataReceiveTimeout);
    }

    internal static int uwrepl_put_DataReceiveTimeout(IntPtr pIReplication, int DataReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DataReceiveTimeout(pIReplication, DataReceiveTimeout);
    }

    internal static int uwrepl_get_ControlSendTimeout(
      IntPtr pIReplication,
      ref int ControlSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ControlSendTimeout(pIReplication, ref ControlSendTimeout);
    }

    internal static int uwrepl_put_ControlSendTimeout(IntPtr pIReplication, int ControlSendTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ControlSendTimeout(pIReplication, ControlSendTimeout);
    }

    internal static int uwrepl_get_ControlReceiveTimeout(
      IntPtr pIReplication,
      ref int ControlReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ControlReceiveTimeout(pIReplication, ref ControlReceiveTimeout);
    }

    internal static int uwrepl_put_ControlReceiveTimeout(
      IntPtr pIReplication,
      int ControlReceiveTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ControlReceiveTimeout(pIReplication, ControlReceiveTimeout);
    }

    internal static int uwrepl_get_ConnectionRetryTimeout(
      IntPtr pIReplication,
      ref ushort ConnectionRetryTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ConnectionRetryTimeout(pIReplication, ref ConnectionRetryTimeout);
    }

    internal static int uwrepl_put_LoginTimeout(IntPtr pIReplication, ushort LoginTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_LoginTimeout(pIReplication, LoginTimeout);
    }

    internal static int uwrepl_get_ProfileName(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ProfileName(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_ProfileName(IntPtr pIReplication, string ProfileName)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ProfileName(pIReplication, ProfileName);
    }

    internal static int uwrepl_get_Publisher(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_Publisher(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_Publisher(IntPtr pIReplication, string Publisher)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_Publisher(pIReplication, Publisher);
    }

    internal static int uwrepl_get_PublisherNetwork(
      IntPtr pIReplication,
      ref NetworkType PublisherNetwork)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherNetwork(pIReplication, ref PublisherNetwork);
    }

    internal static int uwrepl_put_PublisherNetwork(
      IntPtr pIReplication,
      NetworkType PublisherNetwork)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PublisherNetwork(pIReplication, PublisherNetwork);
    }

    internal static int uwrepl_get_PublisherAddress(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherAddress(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_PublisherAddress(IntPtr pIReplication, string PublisherAddress)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PublisherAddress(pIReplication, PublisherAddress);
    }

    internal static int uwrepl_get_PublisherSecurityMode(
      IntPtr pIReplication,
      ref SecurityType PublisherSecurityMode)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherSecurityMode(pIReplication, ref PublisherSecurityMode);
    }

    internal static int uwrepl_put_PublisherSecurityMode(
      IntPtr pIReplication,
      SecurityType PublisherSecurityMode)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PublisherSecurityMode(pIReplication, PublisherSecurityMode);
    }

    internal static int uwrepl_get_PublisherLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherLogin(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_PublisherLogin(IntPtr pIReplication, string PublisherLogin)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PublisherLogin(pIReplication, PublisherLogin);
    }

    internal static int uwrepl_get_PublisherPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherPassword(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_PublisherPassword(IntPtr pIReplication, string PublisherPassword)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PublisherPassword(pIReplication, PublisherPassword);
    }

    internal static int uwrepl_get_PublisherDatabase(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherDatabase(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_PublisherDatabase(IntPtr pIReplication, string PublisherDatabase)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PublisherDatabase(pIReplication, PublisherDatabase);
    }

    internal static int uwrepl_get_Publication(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_Publication(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_Publication(IntPtr pIReplication, string Publication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_Publication(pIReplication, Publication);
    }

    internal static int uwrepl_get_PublisherChanges(IntPtr pIReplication, ref int PublisherChanges)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherChanges(pIReplication, ref PublisherChanges);
    }

    internal static int uwrepl_get_PublisherConflicts(
      IntPtr pIReplication,
      ref int PublisherConflicts)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PublisherConflicts(pIReplication, ref PublisherConflicts);
    }

    internal static int uwrepl_get_QueryTimeout(IntPtr pIReplication, ref ushort QueryTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_QueryTimeout(pIReplication, ref QueryTimeout);
    }

    internal static int uwrepl_put_QueryTimeout(IntPtr pIReplication, ushort QueryTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_QueryTimeout(pIReplication, QueryTimeout);
    }

    internal static int uwrepl_get_DistributorSecurityMode(
      IntPtr pIReplication,
      ref SecurityType DistributorSecurityMode)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DistributorSecurityMode(pIReplication, ref DistributorSecurityMode);
    }

    internal static int uwrepl_put_DistributorSecurityMode(
      IntPtr pIReplication,
      SecurityType DistributorSecurityMode)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DistributorSecurityMode(pIReplication, DistributorSecurityMode);
    }

    internal static int uwrepl_get_DistributorLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DistributorLogin(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_DistributorLogin(IntPtr pIReplication, string DistributorLogin)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DistributorLogin(pIReplication, DistributorLogin);
    }

    internal static int uwrepl_get_DistributorPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DistributorPassword(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_DistributorPassword(
      IntPtr pIReplication,
      string DistributorPassword)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DistributorPassword(pIReplication, DistributorPassword);
    }

    internal static int uwrepl_get_ExchangeType(IntPtr pIReplication, ref ExchangeType ExchangeType)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ExchangeType(pIReplication, ref ExchangeType);
    }

    internal static int uwrepl_put_ExchangeType(IntPtr pIReplication, ExchangeType ExchangeType)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_ExchangeType(pIReplication, ExchangeType);
    }

    internal static int uwrepl_get_HostName(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_HostName(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_HostName(IntPtr pIReplication, string HostName)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_HostName(pIReplication, HostName);
    }

    internal static int uwrepl_get_InternetUrl(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_InternetUrl(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_InternetUrl(IntPtr pIReplication, string InternetUrl)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_InternetUrl(pIReplication, InternetUrl);
    }

    internal static int uwrepl_get_InternetLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_InternetLogin(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_InternetLogin(IntPtr pIReplication, string InternetLogin)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_InternetLogin(pIReplication, InternetLogin);
    }

    internal static int uwrepl_get_InternetPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_InternetPassword(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_InternetPassword(IntPtr pIReplication, string InternetPassword)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_InternetPassword(pIReplication, InternetPassword);
    }

    internal static int uwrepl_get_InternetProxyServer(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_InternetProxyServer(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_InternetProxyServer(
      IntPtr pIReplication,
      string InternetProxyServer)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_InternetProxyServer(pIReplication, InternetProxyServer);
    }

    internal static int uwrepl_get_InternetProxyLogin(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_InternetProxyLogin(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_InternetProxyLogin(
      IntPtr pIReplication,
      string InternetProxyLogin)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_InternetProxyLogin(pIReplication, InternetProxyLogin);
    }

    internal static int uwrepl_get_InternetProxyPassword(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_InternetProxyPassword(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_InternetProxyPassword(
      IntPtr pIReplication,
      string InternetProxyPassword)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_InternetProxyPassword(pIReplication, InternetProxyPassword);
    }

    internal static int uwrepl_get_LoginTimeout(IntPtr pIReplication, ref ushort LoginTimeout)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_LoginTimeout(pIReplication, ref LoginTimeout);
    }

    internal static int uwrepl_Replication(ref IntPtr pIReplication, ref IntPtr pCreationIError)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_Replication(ref pIReplication, ref pCreationIError);
    }

    internal static int uwrepl_get_ErrorPointer(IntPtr pIReplication, ref IntPtr pIErrors)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_ErrorPointer(pIReplication, ref pIErrors);
    }

    internal static int uwrepl_get_Distributor(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_Distributor(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_Distributor(IntPtr pIReplication, string Distributor)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_Distributor(pIReplication, Distributor);
    }

    internal static int uwrepl_get_PostSyncCleanup(IntPtr pIReplication, ref short iCleanupType)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_PostSyncCleanup(pIReplication, ref iCleanupType);
    }

    internal static int uwrepl_put_PostSyncCleanup(IntPtr pIReplication, short iCleanupType)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_PostSyncCleanup(pIReplication, iCleanupType);
    }

    internal static int uwrepl_get_DistributorNetwork(
      IntPtr pIReplication,
      ref NetworkType DistributorNetwork)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DistributorNetwork(pIReplication, ref DistributorNetwork);
    }

    internal static int uwrepl_put_DistributorNetwork(
      IntPtr pIReplication,
      NetworkType DistributorNetwork)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DistributorNetwork(pIReplication, DistributorNetwork);
    }

    internal static int uwrepl_get_DistributorAddress(IntPtr pIReplication, ref IntPtr rbz)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_get_DistributorAddress(pIReplication, ref rbz);
    }

    internal static int uwrepl_put_DistributorAddress(
      IntPtr pIReplication,
      string DistributorAddress)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_put_DistributorAddress(pIReplication, DistributorAddress);
    }

    internal static int uwrepl_AsyncReplication(IntPtr pIReplication, ref IntPtr pAsyncIReplication)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_AsyncReplication(pIReplication, ref pAsyncIReplication);
    }

    internal static int uwrepl_WaitForNextStatusReport(
      IntPtr pAsyncReplication,
      ref SyncStatus pSyncStatus,
      ref IntPtr rbzTableName,
      ref int pPrecentCompleted,
      ref bool pCompleted)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_WaitForNextStatusReport(pAsyncReplication, ref pSyncStatus, ref rbzTableName, ref pPrecentCompleted, ref pCompleted);
    }

    internal static int uwrepl_GetSyncResult(IntPtr pIReplication, ref int pHr)
    {
      NativeMethods.ThrowIfNativeLibraryNotLoaded();
      return NativeMethods.NativeMethodsHelper.uwrepl_GetSyncResult(pIReplication, ref pHr);
    }
  }
}
