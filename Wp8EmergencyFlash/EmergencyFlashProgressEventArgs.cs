// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyFlashProgressEventArgs
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using System;

namespace Microsoft.LsuPro
{
  public class EmergencyFlashProgressEventArgs : EventArgs
  {
    public string ProgressInfoText { get; internal set; }

    public string RawData { get; internal set; }

    public bool IsIndeterminate { get; internal set; }

    public int Percentage { get; internal set; }

    public EmergencyFlashStage Stage { get; internal set; }
  }
}
