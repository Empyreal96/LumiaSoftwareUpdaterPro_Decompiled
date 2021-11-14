// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeReplication
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
  public sealed class SqlCeReplication : IDisposable
  {
    internal IntPtr pIReplication = IntPtr.Zero;
    internal IntPtr pIErrors = IntPtr.Zero;
    private int isNativeAssemblyReleased;

    static SqlCeReplication() => KillBitHelper.ThrowIfKillBitIsSet();

    public SqlCeReplication()
    {
      NativeMethods.LoadNativeBinaries();
      this.pIReplication = new IntPtr(0);
      IntPtr pCreationIError = new IntPtr(0);
      int hr = NativeMethods.uwrepl_Replication(ref this.pIReplication, ref pCreationIError);
      if (NativeMethods.Failed(hr))
      {
        SqlCeException sqlCeException = SqlCeException.FillErrorInformation(hr, pCreationIError);
        int num = IntPtr.Zero != pCreationIError ? (int) NativeMethods.uwutil_ReleaseCOMPtr(pCreationIError) : throw sqlCeException;
      }
      else
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ErrorPointer(this.pIReplication, ref this.pIErrors));
        NativeMethods.DllAddRef();
      }
    }

    public SqlCeReplication(
      string internetUrl,
      string internetLogin,
      string internetPassword,
      string publisher,
      string publisherDatabase,
      string publisherLogin,
      string publisherPassword,
      string publication,
      string subscriber,
      string subscriberConnectionString)
    {
      NativeMethods.LoadNativeBinaries();
      this.pIReplication = new IntPtr(0);
      IntPtr pCreationIError = new IntPtr(0);
      int hr = NativeMethods.uwrepl_Replication(ref this.pIReplication, ref pCreationIError);
      if (NativeMethods.Failed(hr))
      {
        SqlCeException sqlCeException = SqlCeException.FillErrorInformation(hr, pCreationIError);
        int num = IntPtr.Zero != pCreationIError ? (int) NativeMethods.uwutil_ReleaseCOMPtr(pCreationIError) : throw sqlCeException;
      }
      else
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ErrorPointer(this.pIReplication, ref this.pIErrors));
        this.InternetUrl = internetUrl;
        this.InternetLogin = internetLogin;
        this.InternetPassword = internetPassword;
        this.Publisher = publisher;
        this.PublisherDatabase = publisherDatabase;
        this.PublisherLogin = publisherLogin;
        this.PublisherPassword = publisherPassword;
        this.Publication = publication;
        this.Subscriber = subscriber;
        this.SubscriberConnectionString = subscriberConnectionString;
        NativeMethods.DllAddRef();
        this.isNativeAssemblyReleased = 0;
      }
    }

    public SqlCeReplication(
      string internetUrl,
      string internetLogin,
      string internetPassword,
      string publisher,
      string publisherDatabase,
      string publication,
      string subscriber,
      string subscriberConnectionString)
    {
      NativeMethods.LoadNativeBinaries();
      this.pIReplication = new IntPtr(0);
      IntPtr pCreationIError = new IntPtr(0);
      int hr = NativeMethods.uwrepl_Replication(ref this.pIReplication, ref pCreationIError);
      if (NativeMethods.Failed(hr))
      {
        SqlCeException sqlCeException = SqlCeException.FillErrorInformation(hr, pCreationIError);
        int num = IntPtr.Zero != pCreationIError ? (int) NativeMethods.uwutil_ReleaseCOMPtr(pCreationIError) : throw sqlCeException;
      }
      else
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ErrorPointer(this.pIReplication, ref this.pIErrors));
        this.InternetUrl = internetUrl;
        this.InternetLogin = internetLogin;
        this.InternetPassword = internetPassword;
        this.Publisher = publisher;
        this.PublisherDatabase = publisherDatabase;
        this.Publication = publication;
        this.Subscriber = subscriber;
        this.SubscriberConnectionString = subscriberConnectionString;
        this.PublisherSecurityMode = SecurityType.NTAuthentication;
        NativeMethods.DllAddRef();
        this.isNativeAssemblyReleased = 0;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~SqlCeReplication() => this.Dispose(false);

    private void Dispose(bool disposing)
    {
      if (IntPtr.Zero != this.pIReplication)
      {
        int num = (int) NativeMethods.uwutil_ReleaseCOMPtr(this.pIReplication);
        this.pIReplication = IntPtr.Zero;
      }
      if (IntPtr.Zero != this.pIErrors)
      {
        int num = (int) NativeMethods.uwutil_ReleaseCOMPtr(this.pIErrors);
        this.pIErrors = IntPtr.Zero;
      }
      GC.KeepAlive((object) this.pIReplication);
      GC.KeepAlive((object) this.pIErrors);
      if (Interlocked.Exchange(ref this.isNativeAssemblyReleased, 1) != 0)
        return;
      NativeMethods.DllRelease();
    }

    public short PostSyncCleanup
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        short iCleanupType = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_PostSyncCleanup(this.pIReplication, ref iCleanupType));
        return iCleanupType;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PostSyncCleanup(this.pIReplication, value));
      }
    }

    public string Distributor
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int distributor = NativeMethods.uwrepl_get_Distributor(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, distributor);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_Distributor(this.pIReplication, value));
      }
    }

    public NetworkType DistributorNetwork
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NetworkType DistributorNetwork = NetworkType.DefaultNetwork;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_DistributorNetwork(this.pIReplication, ref DistributorNetwork));
        return DistributorNetwork;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DistributorNetwork(this.pIReplication, value));
      }
    }

    public string DistributorAddress
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int distributorAddress = NativeMethods.uwrepl_get_DistributorAddress(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, distributorAddress);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DistributorAddress(this.pIReplication, value));
      }
    }

    public SecurityType DistributorSecurityMode
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        SecurityType DistributorSecurityMode = SecurityType.DBAuthentication;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_DistributorSecurityMode(this.pIReplication, ref DistributorSecurityMode));
        return DistributorSecurityMode;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DistributorSecurityMode(this.pIReplication, value));
      }
    }

    public string DistributorLogin
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int distributorLogin = NativeMethods.uwrepl_get_DistributorLogin(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, distributorLogin);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DistributorLogin(this.pIReplication, value));
      }
    }

    public string DistributorPassword
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int distributorPassword = NativeMethods.uwrepl_get_DistributorPassword(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, distributorPassword);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DistributorPassword(this.pIReplication, value));
      }
    }

    public ExchangeType ExchangeType
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        ExchangeType ExchangeType = ExchangeType.BiDirectional;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ExchangeType(this.pIReplication, ref ExchangeType));
        return ExchangeType;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ExchangeType(this.pIReplication, value));
      }
    }

    public string HostName
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int hostName = NativeMethods.uwrepl_get_HostName(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, hostName);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_HostName(this.pIReplication, value));
      }
    }

    public string InternetUrl
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int internetUrl = NativeMethods.uwrepl_get_InternetUrl(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_InternetUrl(this.pIReplication, value));
      }
    }

    public string InternetLogin
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int internetLogin = NativeMethods.uwrepl_get_InternetLogin(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_InternetLogin(this.pIReplication, value));
      }
    }

    public string InternetPassword
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int internetPassword = NativeMethods.uwrepl_get_InternetPassword(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_InternetPassword(this.pIReplication, value));
      }
    }

    public string InternetProxyServer
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int internetProxyServer = NativeMethods.uwrepl_get_InternetProxyServer(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_InternetProxyServer(this.pIReplication, value));
      }
    }

    public string InternetProxyLogin
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int internetProxyLogin = NativeMethods.uwrepl_get_InternetProxyLogin(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_InternetProxyLogin(this.pIReplication, value));
      }
    }

    public string InternetProxyPassword
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int internetProxyPassword = NativeMethods.uwrepl_get_InternetProxyPassword(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_InternetProxyPassword(this.pIReplication, value));
      }
    }

    public short LoginTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        ushort LoginTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_LoginTimeout(this.pIReplication, ref LoginTimeout));
        return (short) LoginTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_LoginTimeout(this.pIReplication, (ushort) value));
      }
    }

    public string ProfileName
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int profileName = NativeMethods.uwrepl_get_ProfileName(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, profileName);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ProfileName(this.pIReplication, value));
      }
    }

    public string Publisher
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int publisher = NativeMethods.uwrepl_get_Publisher(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, publisher);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_Publisher(this.pIReplication, value));
      }
    }

    public NetworkType PublisherNetwork
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NetworkType PublisherNetwork = NetworkType.DefaultNetwork;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_PublisherNetwork(this.pIReplication, ref PublisherNetwork));
        return PublisherNetwork;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PublisherNetwork(this.pIReplication, value));
      }
    }

    public string PublisherAddress
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int publisherAddress = NativeMethods.uwrepl_get_PublisherAddress(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, publisherAddress);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PublisherAddress(this.pIReplication, value));
      }
    }

    public SecurityType PublisherSecurityMode
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        SecurityType PublisherSecurityMode = SecurityType.DBAuthentication;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_PublisherSecurityMode(this.pIReplication, ref PublisherSecurityMode));
        return PublisherSecurityMode;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PublisherSecurityMode(this.pIReplication, value));
      }
    }

    public string PublisherLogin
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int publisherLogin = NativeMethods.uwrepl_get_PublisherLogin(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, publisherLogin);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PublisherLogin(this.pIReplication, value));
      }
    }

    public string PublisherPassword
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int publisherPassword = NativeMethods.uwrepl_get_PublisherPassword(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, publisherPassword);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PublisherPassword(this.pIReplication, value));
      }
    }

    public string PublisherDatabase
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int publisherDatabase = NativeMethods.uwrepl_get_PublisherDatabase(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, publisherDatabase);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_PublisherDatabase(this.pIReplication, value));
      }
    }

    public string Publication
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int publication = NativeMethods.uwrepl_get_Publication(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, publication);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_Publication(this.pIReplication, value));
      }
    }

    public int PublisherChanges
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int PublisherChanges = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_PublisherChanges(this.pIReplication, ref PublisherChanges));
        return PublisherChanges;
      }
    }

    public int PublisherConflicts
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int PublisherConflicts = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_PublisherConflicts(this.pIReplication, ref PublisherConflicts));
        return PublisherConflicts;
      }
    }

    public short QueryTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        ushort QueryTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_QueryTimeout(this.pIReplication, ref QueryTimeout));
        return (short) QueryTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_QueryTimeout(this.pIReplication, (ushort) value));
      }
    }

    public string Subscriber
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int subscriber = NativeMethods.uwrepl_get_Subscriber(this.pIReplication, ref rbz);
        try
        {
          NativeMethods.CheckHRESULT(this.pIErrors, subscriber);
          return Marshal.PtrToStringUni(rbz);
        }
        finally
        {
          NativeMethods.uwutil_SysFreeString(rbz);
        }
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_Subscriber(this.pIReplication, value));
      }
    }

    public string SubscriberConnectionString
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        IntPtr rbz = new IntPtr(0);
        int connectionString = NativeMethods.uwrepl_get_SubscriberConnectionString(this.pIReplication, ref rbz);
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
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        SqlCeConnectionStringBuilder connectionStringBuilder = new SqlCeConnectionStringBuilder(value);
        connectionStringBuilder.DataSource = ConStringUtil.ReplaceDataDirectory(connectionStringBuilder.DataSource);
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_SubscriberConnectionString(this.pIReplication, connectionStringBuilder.OledbConnectionString));
      }
    }

    public int SubscriberChanges
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int SubscriberChanges = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_SubscriberChanges(this.pIReplication, ref SubscriberChanges));
        return SubscriberChanges;
      }
    }

    public int SubscriberConflicts
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int SubscriberConflicts = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_SubscriberConflicts(this.pIReplication, ref SubscriberConflicts));
        return SubscriberConflicts;
      }
    }

    public ValidateType Validate
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        ValidateType Validate = ValidateType.NoValidation;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_Validate(this.pIReplication, ref Validate));
        return Validate;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_Validate(this.pIReplication, value));
      }
    }

    public int ConnectTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int connectTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ConnectTimeout(this.pIReplication, ref connectTimeout));
        return connectTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ConnectTimeout(this.pIReplication, value));
      }
    }

    public int SendTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int SendTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_SendTimeout(this.pIReplication, ref SendTimeout));
        return SendTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_SendTimeout(this.pIReplication, value));
      }
    }

    public int ReceiveTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int ReceiveTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ReceiveTimeout(this.pIReplication, ref ReceiveTimeout));
        return ReceiveTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ReceiveTimeout(this.pIReplication, value));
      }
    }

    private int DataSendTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int DataSendTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_DataSendTimeout(this.pIReplication, ref DataSendTimeout));
        return DataSendTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DataSendTimeout(this.pIReplication, value));
      }
    }

    private int DataReceiveTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int DataReceiveTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_DataReceiveTimeout(this.pIReplication, ref DataReceiveTimeout));
        return DataReceiveTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_DataReceiveTimeout(this.pIReplication, value));
      }
    }

    private int ControlSendTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int ControlSendTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ControlSendTimeout(this.pIReplication, ref ControlSendTimeout));
        return ControlSendTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ControlSendTimeout(this.pIReplication, value));
      }
    }

    private int ControlReceiveTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        int ControlReceiveTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ControlReceiveTimeout(this.pIReplication, ref ControlReceiveTimeout));
        return ControlReceiveTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ControlReceiveTimeout(this.pIReplication, value));
      }
    }

    public short ConnectionRetryTimeout
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        ushort ConnectionRetryTimeout = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ConnectionRetryTimeout(this.pIReplication, ref ConnectionRetryTimeout));
        return (short) ConnectionRetryTimeout;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ConnectionRetryTimeout(this.pIReplication, (ushort) value));
      }
    }

    public short CompressionLevel
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        ushort CompressionLevel = 0;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_CompressionLevel(this.pIReplication, ref CompressionLevel));
        return (short) CompressionLevel;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_CompressionLevel(this.pIReplication, (ushort) value));
      }
    }

    public bool ConnectionManager
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        bool ConnectionManager = false;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_ConnectionManager(this.pIReplication, ref ConnectionManager));
        return ConnectionManager;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_ConnectionManager(this.pIReplication, value));
      }
    }

    public SnapshotTransferType SnapshotTransferType
    {
      get
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        SnapshotTransferType SnapshotTransferType = SnapshotTransferType.UNC;
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_get_SnapshotTransferType(this.pIReplication, ref SnapshotTransferType));
        return SnapshotTransferType;
      }
      set
      {
        if (IntPtr.Zero == this.pIReplication)
          throw new ObjectDisposedException(nameof (SqlCeReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_put_SnapshotTransferType(this.pIReplication, value));
      }
    }

    public void AddSubscription(AddOption addOption)
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_AddSubscription(this.pIReplication, addOption));
    }

    public void DropSubscription(DropOption dropOption)
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_DropSubscription(this.pIReplication, dropOption));
    }

    public void ReinitializeSubscription(bool uploadBeforeReinitialize)
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_ReinitializeSubscription(this.pIReplication, uploadBeforeReinitialize));
    }

    public void Synchronize()
    {
      bool flag = false;
      SqlCeException sqlCeException = (SqlCeException) null;
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      try
      {
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_Initialize(this.pIReplication));
        NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_Run(this.pIReplication));
      }
      catch (SqlCeException ex)
      {
        flag = true;
        sqlCeException = ex;
      }
      finally
      {
        int hr = NativeMethods.uwrepl_Terminate(this.pIReplication);
        if (flag)
          throw sqlCeException;
        NativeMethods.CheckHRESULT(this.pIErrors, hr);
      }
    }

    public IAsyncResult BeginSynchronize(AsyncCallback onSyncCompletion, object state)
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      SyncAsyncResult syncAsyncResult = new SyncAsyncResult(this, onSyncCompletion, state);
      new Thread(new ThreadStart(syncAsyncResult.SyncThread)).Start();
      return (IAsyncResult) syncAsyncResult;
    }

    public IAsyncResult BeginSynchronize(
      AsyncCallback onSyncCompletion,
      OnStartTableUpload onStartTableUpload,
      OnStartTableDownload onStartTableDownload,
      OnSynchronization onSynchronization,
      object state)
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      SyncAsyncResult syncAsyncResult = new SyncAsyncResult(this, onSyncCompletion, onStartTableUpload, onStartTableDownload, onSynchronization, state);
      new Thread(new ThreadStart(syncAsyncResult.SyncThread)).Start();
      return (IAsyncResult) syncAsyncResult;
    }

    public void EndSynchronize(IAsyncResult ar)
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      Exception exception = ((SyncAsyncResult) ar).GetException();
      if (exception != null)
        throw exception;
    }

    public void CancelSynchronize()
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_Cancel(this.pIReplication));
    }

    public bool LoadProperties()
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      bool PasswordsLoaded = true;
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_LoadProperties(this.pIReplication, ref PasswordsLoaded));
      return PasswordsLoaded;
    }

    public void SaveProperties()
    {
      if (IntPtr.Zero == this.pIReplication)
        throw new ObjectDisposedException(nameof (SqlCeReplication));
      NativeMethods.CheckHRESULT(this.pIErrors, NativeMethods.uwrepl_SaveProperties(this.pIReplication));
    }
  }
}
