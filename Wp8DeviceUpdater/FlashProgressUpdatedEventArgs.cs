// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FlashProgressUpdatedEventArgs
// Assembly: Wp8DeviceUpdater, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 88C20D73-1EDE-4FB2-B734-F8968E9CB6A0
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceUpdater.dll

using System;

namespace Microsoft.LsuPro
{
  public class FlashProgressUpdatedEventArgs : EventArgs
  {
    public FlashProgressUpdatedEventArgs(
      int progress,
      long transferredBytes,
      long totalBytes,
      double megabytesPerSecond)
    {
      this.Progress = progress;
      this.TransferredBytes = transferredBytes;
      this.TotalBytes = totalBytes;
      this.MegabytesPerSecond = megabytesPerSecond;
    }

    public int Progress { get; private set; }

    public long TransferredBytes { get; private set; }

    public long TotalBytes { get; private set; }

    public double MegabytesPerSecond { get; private set; }
  }
}
