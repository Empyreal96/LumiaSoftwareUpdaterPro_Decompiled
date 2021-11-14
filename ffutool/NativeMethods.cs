// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.NativeMethods
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.ImageTools
{
  internal static class NativeMethods
  {
    [DllImport("advapi32.dll", EntryPoint = "StartTraceW", CharSet = CharSet.Unicode)]
    internal static extern int StartTrace(
      out ulong traceHandle,
      [MarshalAs(UnmanagedType.LPWStr), In] string sessionName,
      [In, Out] ref EventTraceProperties eventTraceProperties);

    internal static int EnableTraceEx2(
      ulong traceHandle,
      Guid providerId,
      uint controlCode,
      TraceLevel traceLevel = TraceLevel.Verbose,
      ulong matchAnyKeyword = 18446744073709551615,
      ulong matchAllKeyword = 0,
      uint timeout = 0)
    {
      return NativeMethods.EnableTraceEx2(traceHandle, ref providerId, controlCode, traceLevel, matchAnyKeyword, matchAllKeyword, timeout, IntPtr.Zero);
    }

    [DllImport("advapi32.dll")]
    internal static extern int StopTrace(
      [In] ulong traceHandle,
      [MarshalAs(UnmanagedType.LPWStr), In] string sessionName,
      out EventTraceProperties eventTraceProperties);

    [DllImport("advapi32.dll")]
    private static extern int EnableTraceEx2(
      [In] ulong traceHandle,
      [In] ref Guid providerId,
      [In] uint controlCode,
      [In] TraceLevel traceLevel,
      [In] ulong matchAnyKeyword,
      [In] ulong matchAllKeyword,
      [In] uint timeout,
      [In] IntPtr enableTraceParameters);
  }
}
