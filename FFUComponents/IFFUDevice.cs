// Decompiled with JetBrains decompiler
// Type: FFUComponents.IFFUDevice
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  public interface IFFUDevice : IDisposable
  {
    string DeviceFriendlyName { get; }

    Guid DeviceUniqueID { get; }

    Guid SerialNumber { get; }

    void FlashFFUFile(string ffuFilePath);

    event EventHandler<ProgressEventArgs> ProgressEvent;

    bool WriteWim(string wimPath);

    bool EndTransfer();

    bool SkipTransfer();

    bool Reboot();

    bool EnterMassStorage();

    bool ClearIdOverride();

    bool GetDiskInfo(out uint blockSize, out ulong lastBlock);

    void ReadDisk(ulong diskOffset, byte[] buffer, int offset, int count);

    void WriteDisk(ulong diskOffset, byte[] buffer, int offset, int count);
  }
}
