// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.KillBitHelper
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using Microsoft.Win32;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlServerCe
{
  internal static class KillBitHelper
  {
    private static readonly string KillBitRegKey = "SOFTWARE\\Microsoft\\ASP.NET\\{0}\\Security\\DisableLoadList\\";
    private static readonly string SqlCeAssembly = "System.Data.SqlServerCe";
    private static readonly string FwdLink = "http://go.microsoft.com/fwlink/?LinkID=196096&clcid=0x409";
    private static bool isKillBitted = KillBitHelper.GetKillBit();

    [SecurityCritical]
    static KillBitHelper() => KillBitHelper.ThrowIfKillBitIsSet();

    internal static void ThrowIfKillBitIsSet()
    {
      if (KillBitHelper.isKillBitted)
        throw new SecurityException(string.Format(Res.GetString("SQLCE_KillBitted"), (object) "4.0.8876.1", (object) KillBitHelper.FwdLink));
    }

    [SecurityCritical]
    public static bool GetKillBit()
    {
      bool flag = false;
      Version version1 = Environment.Version;
      Version version2 = new Version("4.0.8876.1");
      Version version3 = new Version(version1.Major, version1.Minor, version1.Build, 0);
      RegistryKey localMachine = Registry.LocalMachine;
      string name = string.Format(KillBitHelper.KillBitRegKey, (object) version3.ToString());
      new RegistryPermission(RegistryPermissionAccess.Read, localMachine.ToString() + "\\" + name).Assert();
      RegistryKey registryKey1 = localMachine.OpenSubKey(name, false);
      if (registryKey1 != null)
      {
        foreach (string subKeyName in registryKey1.GetSubKeyNames())
        {
          if (subKeyName.StartsWith(KillBitHelper.SqlCeAssembly, StringComparison.OrdinalIgnoreCase))
          {
            RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName, false);
            if ((int) registryKey2.GetValue("Flags", (object) 0) != 0)
            {
              string version4 = (string) registryKey2.GetValue("FileVersion");
              if (!string.IsNullOrEmpty(version4))
              {
                Version version5 = new Version(version4);
                if (version5.Major == version2.Major && version5.Minor == version2.Minor && version5.Build >= version2.Build)
                {
                  flag = true;
                  break;
                }
              }
            }
          }
        }
      }
      CodeAccessPermission.RevertAssert();
      return flag;
    }
  }
}
