// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeRemoteDataAccess
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
  public sealed class SqlCeRemoteDataAccess : IDisposable
  {
    private IntPtr pIRda = IntPtr.Zero;
    private IntPtr pIErrors = IntPtr.Zero;
    private int isNativeAssemblyReleased;

    static SqlCeRemoteDataAccess() => KillBitHelper.ThrowIfKillBitIsSet();

    public SqlCeRemoteDataAccess()
    {
      NativeMethods.LoadNativeBinaries();
      this.pIRda = new IntPtr(0);
      IntPtr pCreationIError = new IntPtr(0);
      int hr = NativeMethods.uwrda_RemoteDataAccess(ref this.pIRda, ref pCreationIError);
      if (NativeMethods.Failed(hr))
      {
        SqlCeException sqlCeException = SqlCeException.FillErrorInformation(hr, pCreationIError);
        int num = IntPtr.Zero != pCreationIError ? (int) NativeMethods.uwutil_ReleaseCOMPtr(pCreationIError) : throw sqlCeException;
      }
      else
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ErrorPointer(this.pIRda, ref this.pIErrors));
        NativeMethods.DllAddRef();
        this.isNativeAssemblyReleased = 0;
      }
    }

    public SqlCeRemoteDataAccess(string internetUrl, string localConnectionString)
    {
      NativeMethods.LoadNativeBinaries();
      this.pIRda = new IntPtr(0);
      IntPtr pCreationIError = new IntPtr(0);
      int hr = NativeMethods.uwrda_RemoteDataAccess(ref this.pIRda, ref pCreationIError);
      if (NativeMethods.Failed(hr))
      {
        SqlCeException sqlCeException = SqlCeException.FillErrorInformation(hr, pCreationIError);
        int num = IntPtr.Zero != pCreationIError ? (int) NativeMethods.uwutil_ReleaseCOMPtr(pCreationIError) : throw sqlCeException;
      }
      else
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ErrorPointer(this.pIRda, ref this.pIErrors));
        this.InternetUrl = internetUrl;
        this.LocalConnectionString = localConnectionString;
        NativeMethods.DllAddRef();
        this.isNativeAssemblyReleased = 0;
      }
    }

    public SqlCeRemoteDataAccess(
      string internetUrl,
      string internetLogin,
      string internetPassword,
      string localConnectionString)
    {
      NativeMethods.LoadNativeBinaries();
      this.pIRda = new IntPtr(0);
      IntPtr pCreationIError = new IntPtr(0);
      int hr = NativeMethods.uwrda_RemoteDataAccess(ref this.pIRda, ref pCreationIError);
      if (NativeMethods.Failed(hr))
      {
        SqlCeException sqlCeException = SqlCeException.FillErrorInformation(hr, pCreationIError);
        int num = IntPtr.Zero != pCreationIError ? (int) NativeMethods.uwutil_ReleaseCOMPtr(pCreationIError) : throw sqlCeException;
      }
      else
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ErrorPointer(this.pIRda, ref this.pIErrors));
        this.InternetUrl = internetUrl;
        this.LocalConnectionString = localConnectionString;
        this.InternetLogin = internetLogin;
        this.InternetPassword = internetPassword;
        NativeMethods.DllAddRef();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~SqlCeRemoteDataAccess() => this.Dispose(false);

    private void Dispose(bool disposing)
    {
      if (IntPtr.Zero != this.pIRda)
      {
        int num = (int) NativeMethods.uwutil_ReleaseCOMPtr(this.pIRda);
        this.pIRda = IntPtr.Zero;
      }
      if (IntPtr.Zero != this.pIErrors)
      {
        int num = (int) NativeMethods.uwutil_ReleaseCOMPtr(this.pIErrors);
        this.pIErrors = IntPtr.Zero;
      }
      GC.KeepAlive((object) this.pIRda);
      GC.KeepAlive((object) this.pIErrors);
      if (Interlocked.Exchange(ref this.isNativeAssemblyReleased, 1) != 0)
        return;
      NativeMethods.DllRelease();
    }

    public string LocalConnectionString
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int connectionString = NativeMethods.uwrda_get_LocalConnectionString(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, connectionString);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        SqlCeConnectionStringBuilder connectionStringBuilder = new SqlCeConnectionStringBuilder(value);
        connectionStringBuilder.DataSource = ConStringUtil.ReplaceDataDirectory(connectionStringBuilder.DataSource);
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_LocalConnectionString(this.pIRda, connectionStringBuilder.OledbConnectionString));
      }
    }

    public string InternetUrl
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int internetUrl = NativeMethods.uwrda_get_InternetUrl(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, internetUrl);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_InternetUrl(this.pIRda, value));
      }
    }

    public string InternetLogin
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int internetLogin = NativeMethods.uwrda_get_InternetLogin(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, internetLogin);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_InternetLogin(this.pIRda, value));
      }
    }

    public string InternetPassword
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int internetPassword = NativeMethods.uwrda_get_InternetPassword(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, internetPassword);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_InternetPassword(this.pIRda, value));
      }
    }

    public bool ConnectionManager
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        bool ConnectionManager = false;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ConnectionManager(this.pIRda, ref ConnectionManager));
        return ConnectionManager;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_ConnectionManager(this.pIRda, value));
      }
    }

    public string InternetProxyServer
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int internetProxyServer = NativeMethods.uwrda_get_InternetProxyServer(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, internetProxyServer);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_InternetProxyServer(this.pIRda, value));
      }
    }

    public string InternetProxyLogin
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int internetProxyLogin = NativeMethods.uwrda_get_InternetProxyLogin(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, internetProxyLogin);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_InternetProxyLogin(this.pIRda, value));
      }
    }

    public string InternetProxyPassword
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        IntPtr rbz = new IntPtr(0);
        int internetProxyPassword = NativeMethods.uwrda_get_InternetProxyPassword(this.pIRda, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, internetProxyPassword);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_InternetProxyPassword(this.pIRda, value));
      }
    }

    public int ConnectTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int connectTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ConnectTimeout(this.pIRda, ref connectTimeout));
        return connectTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_ConnectTimeout(this.pIRda, value));
      }
    }

    public int SendTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int SendTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_SendTimeout(this.pIRda, ref SendTimeout));
        return SendTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_SendTimeout(this.pIRda, value));
      }
    }

    public int ReceiveTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int ReceiveTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ReceiveTimeout(this.pIRda, ref ReceiveTimeout));
        return ReceiveTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_ReceiveTimeout(this.pIRda, value));
      }
    }

    private int DataSendTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int DataSendTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_DataSendTimeout(this.pIRda, ref DataSendTimeout));
        return DataSendTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_DataSendTimeout(this.pIRda, value));
      }
    }

    private int DataReceiveTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int DataReceiveTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_DataReceiveTimeout(this.pIRda, ref DataReceiveTimeout));
        return DataReceiveTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_DataReceiveTimeout(this.pIRda, value));
      }
    }

    private int ControlSendTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int ControlSendTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ControlSendTimeout(this.pIRda, ref ControlSendTimeout));
        return ControlSendTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_ControlSendTimeout(this.pIRda, value));
      }
    }

    private int ControlReceiveTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        int ControlReceiveTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ControlReceiveTimeout(this.pIRda, ref ControlReceiveTimeout));
        return ControlReceiveTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_ControlReceiveTimeout(this.pIRda, value));
      }
    }

    public short ConnectionRetryTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        ushort ConnectionRetryTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_ConnectionRetryTimeout(this.pIRda, ref ConnectionRetryTimeout));
        return (short) ConnectionRetryTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_ConnectionRetryTimeout(this.pIRda, (ushort) value));
      }
    }

    public short CompressionLevel
    {
      get
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        ushort CompressionLevel = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_get_CompressionLevel(this.pIRda, ref CompressionLevel));
        return (short) CompressionLevel;
      }
      set
      {
        if (IntPtr.Zero == this.pIRda)
          throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_put_CompressionLevel(this.pIRda, (ushort) value));
      }
    }

    public void Pull(string localTableName, string sqlSelectString, string oleDBConnectionString)
    {
      if (IntPtr.Zero == this.pIRda)
        throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_Pull(this.pIRda, localTableName, sqlSelectString, oleDBConnectionString, RdaTrackOption.TrackingOff, ""));
    }

    public void Pull(
      string localTableName,
      string sqlSelectString,
      string oleDBConnectionString,
      RdaTrackOption trackOption)
    {
      if (IntPtr.Zero == this.pIRda)
        throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_Pull(this.pIRda, localTableName, sqlSelectString, oleDBConnectionString, trackOption, ""));
    }

    public void Pull(
      string localTableName,
      string sqlSelectString,
      string oleDBConnectionString,
      RdaTrackOption trackOption,
      string errorTable)
    {
      if (IntPtr.Zero == this.pIRda)
        throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_Pull(this.pIRda, localTableName, sqlSelectString, oleDBConnectionString, trackOption, errorTable));
    }

    public void Push(string localTableName, string oleDBConnectionString)
    {
      if (IntPtr.Zero == this.pIRda)
        throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_Push(this.pIRda, localTableName, oleDBConnectionString, RdaBatchOption.BatchingOff));
    }

    public void Push(
      string localTableName,
      string oleDBConnectionString,
      RdaBatchOption batchOption)
    {
      if (IntPtr.Zero == this.pIRda)
        throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_Push(this.pIRda, localTableName, oleDBConnectionString, batchOption));
    }

    public void SubmitSql(string sqlString, string oleDBConnectionString)
    {
      if (IntPtr.Zero == this.pIRda)
        throw new ObjectDisposedException(nameof (SqlCeRemoteDataAccess));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrda_SubmitSql(this.pIRda, sqlString, oleDBConnectionString));
    }
  }
}
