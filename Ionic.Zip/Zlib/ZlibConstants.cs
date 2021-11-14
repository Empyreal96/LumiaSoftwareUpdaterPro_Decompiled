// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.ZlibConstants
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.Zlib
{
  public static class ZlibConstants
  {
    public const int WindowBitsMax = 15;
    public const int WindowBitsDefault = 15;
    public const int Z_OK = 0;
    public const int Z_STREAM_END = 1;
    public const int Z_NEED_DICT = 2;
    public const int Z_STREAM_ERROR = -2;
    public const int Z_DATA_ERROR = -3;
    public const int Z_BUF_ERROR = -5;
    public const int WorkingBufferSizeDefault = 16384;
    public const int WorkingBufferSizeMin = 1024;
  }
}
