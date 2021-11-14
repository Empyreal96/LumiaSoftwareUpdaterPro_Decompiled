// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.QuattroEmergencyFlashFiles
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

namespace Microsoft.LsuPro
{
  public class QuattroEmergencyFlashFiles
  {
    public EmergencyFileInfo ProgrammerFile { get; internal set; }

    public EmergencyFileInfo EdFile { get; internal set; }

    public bool IsValid => this.ProgrammerFile != null && this.EdFile != null && this.ProgrammerFile.IsValid && this.EdFile.IsValid;
  }
}
