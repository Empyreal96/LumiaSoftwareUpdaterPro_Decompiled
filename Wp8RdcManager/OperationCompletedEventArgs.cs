// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OperationCompletedEventArgs
// Assembly: Wp8RdcManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FA7F490B-384D-4433-AD97-E6E4DA27B1A0
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8RdcManager.dll

using System;

namespace Microsoft.LsuPro
{
  public class OperationCompletedEventArgs : EventArgs
  {
    public OperationCompletedEventArgs(string operationName) => this.OperationName = operationName;

    public string OperationName { get; set; }
  }
}
