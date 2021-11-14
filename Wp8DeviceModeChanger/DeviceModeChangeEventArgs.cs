// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceModeChangeEventArgs
// Assembly: Wp8DeviceModeChanger, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 4F0CEEBE-2E94-4BD1-9254-36B88BE96C0D
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceModeChanger.dll

using System;

namespace Microsoft.LsuPro
{
  public class DeviceModeChangeEventArgs : EventArgs
  {
    public DeviceModeChangeEventArgs(string message) => this.Message = message;

    public string Message { get; private set; }
  }
}
