// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FlashStateUpdatedEventArgs
// Assembly: Wp8DeviceUpdater, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 88C20D73-1EDE-4FB2-B734-F8968E9CB6A0
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceUpdater.dll

using System;

namespace Microsoft.LsuPro
{
  public class FlashStateUpdatedEventArgs : EventArgs
  {
    public FlashStateUpdatedEventArgs(FlashState flashState) => this.FlashState = flashState;

    public FlashState FlashState { get; private set; }
  }
}
