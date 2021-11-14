// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OperationStateUpdatedEventArgs
// Assembly: NvItemModule, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0B184167-245E-49B5-887C-F5F0E401EE86
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemModule.dll

using System;
using System.Collections.ObjectModel;

namespace Microsoft.LsuPro
{
  public class OperationStateUpdatedEventArgs : EventArgs
  {
    public OperationStateUpdatedEventArgs(OperationState flashState)
    {
      this.FlashState = flashState;
      this.Error = (Collection<NvItemException>) null;
    }

    public OperationStateUpdatedEventArgs(
      OperationState flashState,
      Collection<NvItemException> warnings)
    {
      this.FlashState = flashState;
      this.Error = warnings;
    }

    public Collection<NvItemException> Error { get; private set; }

    public OperationState FlashState { get; private set; }
  }
}
