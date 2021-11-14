// Decompiled with JetBrains decompiler
// Type: Vurdalakov.CRC32Managed
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;

namespace Vurdalakov
{
  public class CRC32Managed : CRC32
  {
    private uint[] crc32Table = new uint[256];
    private uint crc32Result;

    public CRC32Managed()
      : this(3988292384U)
    {
    }

    public CRC32Managed(uint polynomial)
    {
      for (uint index1 = 0; index1 < 256U; ++index1)
      {
        uint num = index1;
        for (int index2 = 8; index2 > 0; --index2)
        {
          if (((int) num & 1) == 1)
            num = num >> 1 ^ polynomial;
          else
            num >>= 1;
        }
        this.crc32Table[(int) index1] = num;
      }
      this.Initialize();
    }

    public override bool CanReuseTransform => true;

    public override bool CanTransformMultipleBlocks => true;

    public override void Initialize() => this.crc32Result = uint.MaxValue;

    protected override void HashCore(byte[] array, int start, int size)
    {
      int num = start + size;
      for (int index = start; index < num; ++index)
        this.crc32Result = this.crc32Result >> 8 ^ this.crc32Table[(int) array[index] ^ (int) this.crc32Result & (int) byte.MaxValue];
    }

    protected override byte[] HashFinal()
    {
      this.crc32Result = ~this.crc32Result;
      this.Crc32Hash = this.crc32Result;
      this.HashValue = BitConverter.GetBytes(this.crc32Result);
      return this.HashValue;
    }
  }
}
