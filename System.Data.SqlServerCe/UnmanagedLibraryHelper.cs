// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.UnmanagedLibraryHelper
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Data.SqlServerCe
{
  internal sealed class UnmanagedLibraryHelper : IDisposable
  {
    private UnmanagedLibraryHelper.SafeLibraryHandle m_hLibrary;

    static UnmanagedLibraryHelper() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    public UnmanagedLibraryHelper(string fileName)
    {
      this.m_hLibrary = UnmanagedLibraryHelper.SafeLibrary_NativeMethods.LoadLibrary(fileName);
      if (!this.m_hLibrary.IsInvalid)
        return;
      Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    public TDelegate GetUnmanagedFunction<TDelegate>(string functionName) where TDelegate : class
    {
      IntPtr procAddress = UnmanagedLibraryHelper.SafeLibrary_NativeMethods.GetProcAddress(this.m_hLibrary, functionName);
      return procAddress == IntPtr.Zero ? default (TDelegate) : (TDelegate) (object) Marshal.GetDelegateForFunctionPointer(procAddress, typeof (TDelegate));
    }

    [SecurityCritical]
    public void Dispose()
    {
      if (this.m_hLibrary.IsClosed)
        return;
      this.m_hLibrary.Close();
    }

    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      static SafeLibraryHandle() => KillBitHelper.ThrowIfKillBitIsSet();

      [SecurityCritical]
      private SafeLibraryHandle()
        : base(true)
      {
      }

      [SecurityCritical]
      protected override bool ReleaseHandle() => UnmanagedLibraryHelper.SafeLibrary_NativeMethods.FreeLibrary(this.handle);
    }

    private static class SafeLibrary_NativeMethods
    {
      private const string LOADLIB = "kernel32.dll";

      static SafeLibrary_NativeMethods() => KillBitHelper.ThrowIfKillBitIsSet();

      [SuppressUnmanagedCodeSecurity]
      [SecurityCritical]
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
      internal static extern UnmanagedLibraryHelper.SafeLibraryHandle LoadLibrary(
        string fileName);

      [SecurityCritical]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [SuppressUnmanagedCodeSecurity]
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FreeLibrary(IntPtr hModule);

      [SecurityCritical]
      [SuppressUnmanagedCodeSecurity]
      [DllImport("kernel32.dll", BestFitMapping = false)]
      internal static extern IntPtr GetProcAddress(
        UnmanagedLibraryHelper.SafeLibraryHandle hModule,
        string procname);
    }
  }
}
