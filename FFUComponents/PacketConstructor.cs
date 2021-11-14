// Decompiled with JetBrains decompiler
// Type: FFUComponents.PacketConstructor
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.IO;

namespace FFUComponents
{
  internal class PacketConstructor : IDisposable
  {
    private const int cbData = 262144;
    private int packetNumber;

    public Stream DataStream { internal get; set; }

    public long PacketDataLength => 262144;

    public long Position => this.DataStream.Position;

    public long Length => this.DataStream.Length;

    public long RemainingData => this.DataStream.Length - this.DataStream.Position;

    public void Reset()
    {
      this.DataStream.Seek(0L, SeekOrigin.Begin);
      this.packetNumber = 0;
    }

    public PacketConstructor() => this.packetNumber = 0;

    public unsafe byte[] GetNextPacket()
    {
      byte[] buffer = new byte[262156];
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = (byte) 0;
      int num = this.DataStream.Read(buffer, 0, 262144);
      int index1 = 262144;
      byte[] bytes1 = BitConverter.GetBytes(num);
      bytes1.CopyTo((Array) buffer, index1);
      int index2 = index1 + bytes1.Length;
      byte[] bytes2 = BitConverter.GetBytes(this.packetNumber++);
      bytes2.CopyTo((Array) buffer, index2);
      int index3 = index2 + bytes2.Length;
      uint checksum;
      fixed (byte* lpBuffer = buffer)
        checksum = Crc32.GetChecksum(0U, lpBuffer, (uint) (buffer.Length - 4));
      BitConverter.GetBytes(checksum).CopyTo((Array) buffer, index3);
      return buffer;
    }

    public byte[] GetZeroLengthPacket()
    {
      this.DataStream.Seek(0L, SeekOrigin.End);
      return this.GetNextPacket();
    }

    private void Dispose(bool fDisposing)
    {
      if (!fDisposing || this.DataStream == null)
        return;
      this.DataStream.Dispose();
      this.DataStream = (Stream) null;
    }

    public void Dispose() => this.Dispose(true);
  }
}
