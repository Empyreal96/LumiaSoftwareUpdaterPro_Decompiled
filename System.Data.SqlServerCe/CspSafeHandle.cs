// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.CspSafeHandle
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Data.SqlServerCe
{
  [SecurityCritical(SecurityCriticalScope.Everything)]
  internal sealed class CspSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    static CspSafeHandle() => KillBitHelper.ThrowIfKillBitIsSet();

    private CspSafeHandle()
      : base(true)
    {
    }

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("advapi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

    protected override bool ReleaseHandle() => CspSafeHandle.CryptReleaseContext(this.handle, 0);
  }
}
