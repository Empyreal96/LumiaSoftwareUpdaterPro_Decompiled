// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyFlashCompletedEventArgs
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class EmergencyFlashCompletedEventArgs : EventArgs
  {
    public EmergencyFlashType EmergencyFlashType { get; internal set; }

    public bool Success { get; internal set; }

    public Thor2ExitCode ExitCode { get; internal set; }
  }
}
