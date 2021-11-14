// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.HashSafeHandle
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
  internal sealed class HashSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    static HashSafeHandle() => KillBitHelper.ThrowIfKillBitIsSet();

    private HashSafeHandle()
      : base(true)
    {
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    [SuppressUnmanagedCodeSecurity]
    [DllImport("advapi32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptDestroyHash(IntPtr hHash);

    protected override bool ReleaseHandle() => HashSafeHandle.CryptDestroyHash(this.handle);
  }
}
