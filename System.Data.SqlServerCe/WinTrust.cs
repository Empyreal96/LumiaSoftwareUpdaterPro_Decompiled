// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.WinTrust
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace System.Data.SqlServerCe
{
  internal sealed class WinTrust
  {
    private const string WINTRUST_ACTION_GENERIC_VERIFY_V2 = "{00AAC56B-CD44-11D0-8CC2-00C04FC295EE}";
    private readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    private UnmanagedLibraryHelper winTrustModuleHelper;
    [SecurityCritical]
    private WinTrust.delegate_WinVerifyTrust WinVerifyTrust;

    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32.dll")]
    private static extern uint GetSystemDirectory([Out] StringBuilder lpBuffer, uint uSize);

    static WinTrust() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal WinTrust()
    {
      StringBuilder lpBuffer = new StringBuilder();
      int systemDirectory = (int) WinTrust.GetSystemDirectory(lpBuffer, 256U);
      this.winTrustModuleHelper = new UnmanagedLibraryHelper(Path.Combine(lpBuffer.ToString(), "wintrust.dll"));
      this.WinVerifyTrust = this.winTrustModuleHelper.GetUnmanagedFunction<WinTrust.delegate_WinVerifyTrust>(nameof (WinVerifyTrust));
    }

    [SecurityCritical]
    public bool VerifyEmbeddedSignature(string filePath) => this.WinVerifyTrust(this.INVALID_HANDLE_VALUE, new Guid("{00AAC56B-CD44-11D0-8CC2-00C04FC295EE}"), new WinTrust.WinTrustData(filePath)) == WinTrust.WinVerifyTrustResult.Success;

    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
    private delegate WinTrust.WinVerifyTrustResult delegate_WinVerifyTrust(
      IntPtr hwnd,
      [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID,
      WinTrust.WinTrustData pWVTData);

    private enum WinVerifyTrustResult : uint
    {
      Success = 0,
      ProviderUnknown = 2148204545, // 0x800B0001
      ActionUnknown = 2148204546, // 0x800B0002
      SubjectFormUnknown = 2148204547, // 0x800B0003
      SubjectNotTrusted = 2148204548, // 0x800B0004
    }

    private enum WinTrustDataUIChoice : uint
    {
      All = 1,
      None = 2,
      NoBad = 3,
      NoGood = 4,
    }

    private enum WinTrustDataRevocationChecks : uint
    {
      None,
      WholeChain,
    }

    private enum WinTrustDataChoice : uint
    {
      File = 1,
      Catalog = 2,
      Blob = 3,
      Signer = 4,
      Certificate = 5,
    }

    private enum WinTrustDataStateAction : uint
    {
      Ignore,
      Verify,
      Close,
      AutoCache,
      AutoCacheFlush,
    }

    [Flags]
    private enum WinTrustDataProvFlags : uint
    {
      UseIe4TrustFlag = 1,
      NoIe4ChainFlag = 2,
      NoPolicyUsageFlag = 4,
      RevocationCheckNone = 16, // 0x00000010
      RevocationCheckEndCert = 32, // 0x00000020
      RevocationCheckChain = 64, // 0x00000040
      RevocationCheckChainExcludeRoot = 128, // 0x00000080
      SaferFlag = 256, // 0x00000100
      HashOnlyFlag = 512, // 0x00000200
      UseDefaultOsverCheck = 1024, // 0x00000400
      LifetimeSigningFlag = 2048, // 0x00000800
      CacheOnlyUrlRetrieval = 4096, // 0x00001000
    }

    private enum WinTrustDataUIContext : uint
    {
      Execute,
      Install,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private class WinTrustFileInfo
    {
      private uint structSize = (uint) Marshal.SizeOf(typeof (WinTrust.WinTrustFileInfo));
      private IntPtr pszFilePath;
      private IntPtr hFile = IntPtr.Zero;
      private IntPtr pgKnownSubject = IntPtr.Zero;

      static WinTrustFileInfo() => KillBitHelper.ThrowIfKillBitIsSet();

      [SecurityCritical]
      public WinTrustFileInfo(string _filePath) => this.pszFilePath = Marshal.StringToCoTaskMemAuto(_filePath);

      [SecurityCritical]
      ~WinTrustFileInfo() => Marshal.FreeCoTaskMem(this.pszFilePath);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private class WinTrustData
    {
      private uint structSize = (uint) Marshal.SizeOf(typeof (WinTrust.WinTrustData));
      private IntPtr policyCallbackData = IntPtr.Zero;
      private IntPtr SIPClientData = IntPtr.Zero;
      private WinTrust.WinTrustDataUIChoice UIChoice = WinTrust.WinTrustDataUIChoice.None;
      private WinTrust.WinTrustDataRevocationChecks revocationChecks;
      private WinTrust.WinTrustDataChoice unionChoice = WinTrust.WinTrustDataChoice.File;
      private IntPtr fileInfoPtr;
      private WinTrust.WinTrustDataStateAction stateAction;
      private IntPtr stateData = IntPtr.Zero;
      private string URLReference;
      private WinTrust.WinTrustDataProvFlags provFlags = WinTrust.WinTrustDataProvFlags.SaferFlag;
      private WinTrust.WinTrustDataUIContext UIContext;

      static WinTrustData() => KillBitHelper.ThrowIfKillBitIsSet();

      [SecurityCritical]
      public WinTrustData(string _fileName)
      {
        WinTrust.WinTrustFileInfo winTrustFileInfo = new WinTrust.WinTrustFileInfo(_fileName);
        this.fileInfoPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (WinTrust.WinTrustFileInfo)));
        Marshal.StructureToPtr((object) winTrustFileInfo, this.fileInfoPtr, false);
      }

      [SecurityCritical]
      ~WinTrustData() => Marshal.FreeCoTaskMem(this.fileInfoPtr);
    }
  }
}
