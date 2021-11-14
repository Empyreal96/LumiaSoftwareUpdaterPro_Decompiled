// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.DownloadProgress`1
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;

namespace SoftwareRepository.Streaming
{
  public class DownloadProgress<T> : IProgress<T>
  {
    private readonly Action<T> action;

    public DownloadProgress(Action<T> action) => this.action = action != null ? action : throw new ArgumentNullException(nameof (action));

    public void Report(T value) => this.action(value);
  }
}
