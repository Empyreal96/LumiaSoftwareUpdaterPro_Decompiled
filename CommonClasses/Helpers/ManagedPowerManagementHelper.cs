// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.ManagedPowerManagementHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.LsuPro.Helpers
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class ManagedPowerManagementHelper
  {
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Justification = "reviewed", MessageId = "flags")]
    public static bool SetProcessShutdownParameters(uint level, uint flags) => PowerManagementHelper.SetShutdownParametersForProcess(level, flags);

    public static bool CreateShutdownBlockReason(IntPtr hwnd, string reason) => PowerManagementHelper.CreateShutdownBlockReason(hwnd, reason);

    public static bool DestroyShutdownBlockReason(IntPtr hwnd) => PowerManagementHelper.DestroyShutdownBlockReason(hwnd);

    public static bool SetThreadExecutionState(
      ExecutionStates executionState,
      out ExecutionStates previousExecutionState)
    {
      previousExecutionState = PowerManagementHelper.SetThreadExecutionFlags(executionState);
      return previousExecutionState > (ExecutionStates) 0;
    }

    public static SystemPowerState GetSystemPowerStatus()
    {
      SystemPowerState systemPowerState = new SystemPowerState();
      PowerManagementHelper.SystemPowerStatus lpSystemPowerStatus;
      if (!PowerManagementHelper.ReadSystemPowerStatus(out lpSystemPowerStatus))
        throw new InvalidDataException("Could not get system power status");
      switch (lpSystemPowerStatus.AcLineStatus)
      {
        case 0:
          systemPowerState.OnBatteryPower = new bool?(true);
          break;
        case 1:
          systemPowerState.OnBatteryPower = new bool?(false);
          break;
        default:
          systemPowerState.OnBatteryPower = new bool?();
          break;
      }
      systemPowerState.RemainingBatteryLifetime = lpSystemPowerStatus.BatteryLifeTime != -1 ? TimeSpan.FromSeconds((double) lpSystemPowerStatus.BatteryLifeTime) : TimeSpan.MaxValue;
      systemPowerState.FullBatteryLifetime = lpSystemPowerStatus.BatteryFullLifeTime != -1 ? TimeSpan.FromSeconds((double) lpSystemPowerStatus.BatteryFullLifeTime) : TimeSpan.MaxValue;
      systemPowerState.RemainingBatteryPercentage = lpSystemPowerStatus.BatteryLifePercent != byte.MaxValue ? (int) lpSystemPowerStatus.BatteryLifePercent : int.MaxValue;
      systemPowerState.BatteryChargeStatus = (BatteryChargeStatus) lpSystemPowerStatus.BatteryFlag;
      return systemPowerState;
    }
  }
}
