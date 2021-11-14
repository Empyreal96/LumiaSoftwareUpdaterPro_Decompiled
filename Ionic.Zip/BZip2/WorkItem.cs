// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.WorkItem
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System.IO;

namespace Ionic.BZip2
{
  internal class WorkItem
  {
    public int index;
    public MemoryStream ms;
    public int ordinal;
    public BitWriter bw;

    public BZip2Compressor Compressor { get; private set; }

    public WorkItem(int ix, int blockSize)
    {
      this.ms = new MemoryStream();
      this.bw = new BitWriter((Stream) this.ms);
      this.Compressor = new BZip2Compressor(this.bw, blockSize);
      this.index = ix;
    }
  }
}
