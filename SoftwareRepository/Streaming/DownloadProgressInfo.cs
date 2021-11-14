// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.DownloadProgressInfo
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Diagnostics.CodeAnalysis;

namespace SoftwareRepository.Streaming
{
  [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
  public struct DownloadProgressInfo
  {
    [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Performance")]
    public readonly long BytesReceived;
    [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Performance")]
    public readonly string FileName;
    [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Performance")]
    public readonly long TotalBytes;

    public DownloadProgressInfo(long bytesReceived, long totalBytes, string fileName)
    {
      this.BytesReceived = bytesReceived;
      this.TotalBytes = totalBytes;
      this.FileName = fileName;
    }
  }
}
