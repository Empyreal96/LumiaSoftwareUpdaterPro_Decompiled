// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.PiaReaderOperationState
// Assembly: PiaReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 27422629-2045-43F0-B2EB-AE7A366F98F1
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\PiaReader.dll

namespace Microsoft.LsuPro
{
  public enum PiaReaderOperationState
  {
    Undefined,
    DetectingDevice,
    DeviceDetected,
    SwitchingToPiaStarted,
    SwitchingToPiaCompleted,
    ReadingPiaStarted,
    ReadingPiaCompleted,
    SwitchingToFlashModeStarted,
    SwitchingToFlashModeCompleted,
  }
}
