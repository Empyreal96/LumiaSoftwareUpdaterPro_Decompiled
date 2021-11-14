// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.Kernel32NativeMethods
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.Interop
{
  internal static class Kernel32NativeMethods
  {
    private const string Kernel32DllName = "kernel32.dll";

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern WNDPROC GetProcAddress(IntPtr hModule, string lpProcName);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("kernel32.dll")]
    public static extern int GetCurrentThreadId();
  }
}
