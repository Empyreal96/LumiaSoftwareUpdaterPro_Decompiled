// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeUtil
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlServerCe
{
  internal static class SqlCeUtil
  {
    internal const string ProviderName = "SQL Server Compact ADO.NET Data Provider";
    internal const string ProductName = "Microsoft SQL Server Compact";
    internal const string ProductRootRegKey = "Software\\Microsoft\\Microsoft SQL Server Compact Edition\\v4.0";
    internal const string Product35RootRegKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Compact Edition\\v3.5";
    internal const string ProductProxyPortsRegKey = "Software\\Microsoft\\Windows CE Services\\ProxyPorts";
    internal const string NetCFKey = "Software\\Microsoft\\.NetCompactFramework\\Installer\\Assemblies\\Global";
    internal const string ProductServicingFile = "Microsoft.SqlServer.Compact.400.{0}.bc";
    internal const string CRTAssemblyFolder = "Microsoft.VC90.CRT";
    internal const string CRTDllName = "msvcr90.dll";
    internal const string CRTCertificatePublicKey = "3082010a0282010100bd72b489e71c9f85c774b8605c03363d9cfd997a9a294622b0a78753edee463ac75b050b57a8b7ca05ccd34c77477085b3e5cbdf67e7a3fd742793679fd78a034430c6f7c9bac93a1d0856444f17080df9b41968aa241cfb055785e9c54e072137a7ebce2c2fb642cd2105a7d6e6d32857c71b7ace293607cd9e55ccbbf122eba823a40d29c2fbd0c35a3e633dc72c490b7b7985f088ef71bd435ae3a3b30df355fb25e0e220d3e79a5e94a5332d287f571b556a0c3244ef666c6ff0389cef02ad9aa1dd9807100e3c1869e2794e4614e0b98cd0756d9cac009c2d42f551b85af4784583e92e7c2bbb5dcd196128ad94430ac56a42ffb532aea42922de16e8d30203010001";
    internal const string ModuleStorageEngine = "sqlcese40.dll";
    internal const string ModuleStorageEngineSys = "sqlcese40.sys.dll";
    internal const string ModuleQueryProcessor = "sqlceqp40.dll";
    internal const string ModuleClientAgent = "sqlceca40.dll";
    internal const string ModuleOleDbProvider = "sqlceoledb40.dll";
    internal const string ModuleManagedExtentions = "sqlceme40.dll";
    internal const string ModuleDbCompact = "sqlcecompact40.dll";
    internal const string ModuleErrRes = "sqlceer40{0}.dll";
    internal const string ModuleServerAgent = "sqlcesa40.dll";
    internal const string ModuleReplicationProvider = "sqlcerp40.dll";
    internal const string ModuleTdsServer = "tdsserver40.exe";
    internal const string Hash_i386 = "1zLdCHyo0bBvEohtwYF7tLbd5cy/4cgOK2yiOA7hvYY=";
    internal const string Hash_amd64 = "nkxsb8fu5OHOJarhFN40NLkxICSRxQSYq5hH5XzAHYA=";
    private static bool? isWebHosted = new bool?();

    [SecurityCritical]
    internal static RegistryKey RegistryOpenSubKey(
      RegistryKey rootKey,
      string subKeyName,
      bool writable)
    {
      new RegistryPermission(RegistryPermissionAccess.Read, rootKey.ToString() + "\\" + subKeyName).Assert();
      RegistryKey registryKey = rootKey.OpenSubKey(subKeyName, writable);
      CodeAccessPermission.RevertAssert();
      return registryKey;
    }

    [SecurityCritical]
    internal static RegistryKey RegistryOpenProductRootKey(
      RegistryKey sysRootKey,
      bool writable)
    {
      return SqlCeUtil.RegistryOpenSubKey(sysRootKey, "Software\\Microsoft\\Microsoft SQL Server Compact Edition\\v4.0", writable);
    }

    [SecurityCritical]
    internal static RegistryKey RegistryOpenProductRootSubKey(
      RegistryKey sysRootKey,
      string subKeyName,
      bool writable)
    {
      RegistryKey rootKey = SqlCeUtil.RegistryOpenSubKey(sysRootKey, "Software\\Microsoft\\Microsoft SQL Server Compact Edition\\v4.0", writable);
      RegistryKey registryKey = (RegistryKey) null;
      if (rootKey != null)
      {
        registryKey = SqlCeUtil.RegistryOpenSubKey(rootKey, subKeyName, writable);
        rootKey.Close();
      }
      return registryKey;
    }

    internal static RegistryKey RegistryCreateProductRootKey(RegistryKey sysRootKey) => sysRootKey.CreateSubKey("Software\\Microsoft\\Microsoft SQL Server Compact Edition\\v4.0");

    internal static RegistryKey RegistryCreateProductRootSubKey(
      RegistryKey sysRootKey,
      string subKeyName)
    {
      RegistryKey productRootKey = SqlCeUtil.RegistryCreateProductRootKey(sysRootKey);
      RegistryKey registryKey = (RegistryKey) null;
      if (productRootKey != null)
      {
        registryKey = productRootKey.CreateSubKey(subKeyName);
        productRootKey.Close();
      }
      return registryKey;
    }

    [SecurityCritical]
    internal static string GetModuleInstallPath(string moduleName)
    {
      string empty = string.Empty;
      new RegistryPermission(RegistryPermissionAccess.Read, "\\Computer\\HKEY_LOCAL_MACHINE").Assert();
      RegistryKey registryKey = SqlCeUtil.RegistryOpenProductRootKey(Registry.LocalMachine, false);
      CodeAccessPermission.RevertAssert();
      if (registryKey == null)
        return string.Empty;
      string name = "NativeDir";
      new RegistryPermission(RegistryPermissionAccess.Read, registryKey.ToString() + "\\" + name).Assert();
      string path1 = (string) registryKey.GetValue(name);
      registryKey.Close();
      CodeAccessPermission.RevertAssert();
      return string.IsNullOrEmpty(path1) ? string.Empty : Path.Combine(path1, moduleName);
    }

    internal static bool IsWebHosted
    {
      get
      {
        if (!SqlCeUtil.isWebHosted.HasValue)
          SqlCeUtil.isWebHosted = new bool?(SqlCeUtil.FindIfWebHosted());
        return SqlCeUtil.isWebHosted.Value;
      }
    }

    private static bool FindIfWebHosted()
    {
      flag7 = false;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (assembly.FullName.StartsWith("System.Web", StringComparison.Ordinal) && assembly.GlobalAssemblyCache)
        {
          Type type = assembly.GetType("System.Web.Hosting.HostingEnvironment");
          if (type != null)
          {
            PropertyInfo property = type.GetProperty("IsHosted", typeof (bool));
            if (property != null)
            {
              object obj = property.GetValue((object) type, (object[]) null);
              if (obj == null || !(obj is bool flag7))
                break;
              break;
            }
            break;
          }
          break;
        }
      }
      return flag7;
    }

    [SecurityCritical]
    internal static void DemandForPermission(string filename, FileIOPermissionAccess permissions)
    {
      string fullPath = Path.GetFullPath(filename);
      new FileIOPermission(permissions, fullPath).Demand();
    }

    static SqlCeUtil() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal static Exception CreateException(IntPtr pError, int hr)
    {
      Exception exception = (Exception) null;
      if (NativeMethods.Failed(hr))
        exception = !(IntPtr.Zero == pError) ? (Exception) SqlCeException.FillErrorInformation(hr, pError) : Marshal.GetExceptionForHR(hr);
      return exception;
    }
  }
}
