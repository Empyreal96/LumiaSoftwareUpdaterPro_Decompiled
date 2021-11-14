// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OperationState
// Assembly: NvItemModule, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0B184167-245E-49B5-887C-F5F0E401EE86
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemModule.dll

namespace Microsoft.LsuPro
{
  public enum OperationState
  {
    Undefined,
    OperationStarted,
    WritingNvItems,
    ReadingNvItems,
    OperationCompleted,
    OperationError,
    OperationCanceled,
    OperationCompletedWithWarnings,
  }
}
