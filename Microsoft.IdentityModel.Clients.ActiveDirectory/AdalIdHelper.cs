// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalIdHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AdalIdHelper
  {
    public static string GetProcessorArchitecture() => AdalIdHelper.NativeMethods.GetProcessorArchitecture();

    public static void AddAsQueryParameters(RequestParameters parameters) => NetworkPlugin.RequestCreationHelper.AddAdalIdParameters((IDictionary<string, string>) parameters);

    public static void AddAsHeaders(IHttpWebRequest request)
    {
      Dictionary<string, string> headers = new Dictionary<string, string>();
      NetworkPlugin.RequestCreationHelper.AddAdalIdParameters((IDictionary<string, string>) headers);
      HttpHelper.AddHeadersToRequest(request, headers);
    }

    public static string GetAdalVersion() => typeof (AdalIdHelper).GetTypeInfo().Assembly.GetName().Version.ToString();

    public static string GetAssemblyFileVersion() => typeof (AdalIdHelper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

    public static string GetAssemblyInformationalVersion()
    {
      AssemblyInformationalVersionAttribute customAttribute = typeof (AdalIdHelper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
      return customAttribute == null ? string.Empty : customAttribute.InformationalVersion;
    }

    private static class NativeMethods
    {
      private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;
      private const int PROCESSOR_ARCHITECTURE_ARM = 5;
      private const int PROCESSOR_ARCHITECTURE_IA64 = 6;
      private const int PROCESSOR_ARCHITECTURE_INTEL = 0;

      [DllImport("kernel32.dll")]
      private static extern void GetNativeSystemInfo(
        ref AdalIdHelper.NativeMethods.SYSTEM_INFO lpSystemInfo);

      public static string GetProcessorArchitecture()
      {
        try
        {
          AdalIdHelper.NativeMethods.SYSTEM_INFO lpSystemInfo = new AdalIdHelper.NativeMethods.SYSTEM_INFO();
          AdalIdHelper.NativeMethods.GetNativeSystemInfo(ref lpSystemInfo);
          switch (lpSystemInfo.wProcessorArchitecture)
          {
            case 0:
              return "x86";
            case 5:
              return "ARM";
            case 6:
            case 9:
              return "x64";
            default:
              return "Unknown";
          }
        }
        catch
        {
          return "Unknown";
        }
      }

      private struct SYSTEM_INFO
      {
        public short wProcessorArchitecture;
        public short wReserved;
        public int dwPageSize;
        public IntPtr lpMinimumApplicationAddress;
        public IntPtr lpMaximumApplicationAddress;
        public IntPtr dwActiveProcessorMask;
        public int dwNumberOfProcessors;
        public int dwProcessorType;
        public int dwAllocationGranularity;
        public short wProcessorLevel;
        public short wProcessorRevision;
      }
    }
  }
}
