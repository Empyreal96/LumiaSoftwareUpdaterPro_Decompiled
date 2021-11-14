// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.PowerManagementHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.LsuPro.Helpers
{
  internal static class PowerManagementHelper
  {
    public static bool SetShutdownParametersForProcess(uint dwLevel, uint dwFlags) => PowerManagementHelper.SetProcessShutdownParameters(dwLevel, dwFlags);

    public static bool CreateShutdownBlockReason(IntPtr hwnd, string reason) => PowerManagementHelper.ShutdownBlockReasonCreate(hwnd, reason);

    public static bool DestroyShutdownBlockReason(IntPtr hwnd) => PowerManagementHelper.ShutdownBlockReasonDestroy(hwnd);

    public static bool ReadSystemPowerStatus(
      out PowerManagementHelper.SystemPowerStatus lpSystemPowerStatus)
    {
      return PowerManagementHelper.GetSystemPowerStatus(out lpSystemPowerStatus);
    }

    public static ExecutionStates SetThreadExecutionFlags(ExecutionStates flags) => PowerManagementHelper.SetThreadExecutionState(flags);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShutdownBlockReasonCreate(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShutdownBlockReasonDestroy(IntPtr hwnd);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern ExecutionStates SetThreadExecutionState(
      ExecutionStates flags);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetSystemPowerStatus(
      out PowerManagementHelper.SystemPowerStatus lpSystemPowerStatus);

    internal struct SystemPowerStatus
    {
      public byte AcLineStatus;
      public byte BatteryFlag;
      public byte BatteryLifePercent;
      public byte Reserved1;
      public int BatteryLifeTime;
      public int BatteryFullLifeTime;
    }
  }
}
