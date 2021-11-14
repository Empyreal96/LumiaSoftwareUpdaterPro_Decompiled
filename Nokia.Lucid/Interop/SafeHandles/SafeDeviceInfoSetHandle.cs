// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.SafeHandles.SafeDeviceInfoSetHandle
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace Nokia.Lucid.Interop.SafeHandles
{
  public sealed class SafeDeviceInfoSetHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    private SafeDeviceInfoSetHandle()
      : base(true)
    {
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    protected override bool ReleaseHandle() => SetupApiNativeMethods.SetupDiDestroyDeviceInfoList(this.handle);
  }
}
