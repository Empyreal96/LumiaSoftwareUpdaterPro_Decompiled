// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.Streamer
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SoftwareRepository.Streaming
{
  public abstract class Streamer : IDisposable
  {
    private Stream Stream;
    private byte[] Metadata;

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Spelled correctly.", MessageId = "GetStreamInternal")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Can throw.")]
    public Stream GetStream()
    {
      if (this.Stream != null)
        return this.Stream;
      Stream streamInternal = this.GetStreamInternal();
      if (streamInternal == null)
        throw new InvalidOperationException("GetStreamInternal() returned null");
      if (!streamInternal.CanWrite)
        throw new InvalidOperationException("Stream returned by GetStreamInternal() must support writing");
      if (!streamInternal.CanSeek)
        throw new InvalidOperationException("Stream returned by GetStreamInternal() must support seeking");
      return this.Stream = streamInternal;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual void SetMetadata(byte[] metadata) => this.Metadata = metadata;

    public virtual byte[] GetMetadata() => this.Metadata;

    public virtual void ClearMetadata() => this.Metadata = (byte[]) null;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Implementors can employ arbitrarily complex logic.")]
    protected abstract Stream GetStreamInternal();

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.Stream == null)
        return;
      this.Stream.Dispose();
      this.Stream = (Stream) null;
    }
  }
}
