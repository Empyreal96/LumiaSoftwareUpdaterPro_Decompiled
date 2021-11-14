// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.ZipConstants
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.Zip
{
  internal static class ZipConstants
  {
    public const uint PackedToRemovableMedia = 808471376;
    public const uint Zip64EndOfCentralDirectoryRecordSignature = 101075792;
    public const uint Zip64EndOfCentralDirectoryLocatorSignature = 117853008;
    public const uint EndOfCentralDirectorySignature = 101010256;
    public const int ZipEntrySignature = 67324752;
    public const int ZipEntryDataDescriptorSignature = 134695760;
    public const int SplitArchiveSignature = 134695760;
    public const int ZipDirEntrySignature = 33639248;
    public const int AesKeySize = 192;
    public const int AesBlockSize = 128;
    public const ushort AesAlgId128 = 26126;
    public const ushort AesAlgId192 = 26127;
    public const ushort AesAlgId256 = 26128;
  }
}
