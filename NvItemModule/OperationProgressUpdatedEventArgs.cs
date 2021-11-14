// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OperationProgressUpdatedEventArgs
// Assembly: NvItemModule, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0B184167-245E-49B5-887C-F5F0E401EE86
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemModule.dll

using System;

namespace Microsoft.LsuPro
{
  public class OperationProgressUpdatedEventArgs : EventArgs
  {
    public OperationProgressUpdatedEventArgs(int progress, string data, string nvId)
    {
      this.Progress = progress;
      this.Data = data;
      this.NvId = nvId;
    }

    public int Progress { get; private set; }

    public string Data { get; private set; }

    public string NvId { get; private set; }
  }
}
