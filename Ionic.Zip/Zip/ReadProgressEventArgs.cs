// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.ReadProgressEventArgs
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.Zip
{
  public class ReadProgressEventArgs : ZipProgressEventArgs
  {
    internal ReadProgressEventArgs()
    {
    }

    private ReadProgressEventArgs(string archiveName, ZipProgressEventType flavor)
      : base(archiveName, flavor)
    {
    }

    internal static ReadProgressEventArgs Before(
      string archiveName,
      int entriesTotal)
    {
      ReadProgressEventArgs progressEventArgs = new ReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_BeforeReadEntry);
      progressEventArgs.EntriesTotal = entriesTotal;
      return progressEventArgs;
    }

    internal static ReadProgressEventArgs After(
      string archiveName,
      ZipEntry entry,
      int entriesTotal)
    {
      ReadProgressEventArgs progressEventArgs = new ReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_AfterReadEntry);
      progressEventArgs.EntriesTotal = entriesTotal;
      progressEventArgs.CurrentEntry = entry;
      return progressEventArgs;
    }

    internal static ReadProgressEventArgs Started(string archiveName) => new ReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_Started);

    internal static ReadProgressEventArgs ByteUpdate(
      string archiveName,
      ZipEntry entry,
      long bytesXferred,
      long totalBytes)
    {
      ReadProgressEventArgs progressEventArgs = new ReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_ArchiveBytesRead);
      progressEventArgs.CurrentEntry = entry;
      progressEventArgs.BytesTransferred = bytesXferred;
      progressEventArgs.TotalBytesToTransfer = totalBytes;
      return progressEventArgs;
    }

    internal static ReadProgressEventArgs Completed(string archiveName) => new ReadProgressEventArgs(archiveName, ZipProgressEventType.Reading_Completed);
  }
}
