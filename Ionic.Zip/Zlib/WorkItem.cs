// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.WorkItem
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.Zlib
{
  internal class WorkItem
  {
    public byte[] buffer;
    public byte[] compressed;
    public int crc;
    public int index;
    public int ordinal;
    public int inputBytesAvailable;
    public int compressedBytesAvailable;
    public ZlibCodec compressor;

    public WorkItem(
      int size,
      CompressionLevel compressLevel,
      CompressionStrategy strategy,
      int ix)
    {
      this.buffer = new byte[size];
      this.compressed = new byte[size + (size / 32768 + 1) * 5 * 2];
      this.compressor = new ZlibCodec();
      this.compressor.InitializeDeflate(compressLevel, false);
      this.compressor.OutputBuffer = this.compressed;
      this.compressor.InputBuffer = this.buffer;
      this.index = ix;
    }
  }
}
