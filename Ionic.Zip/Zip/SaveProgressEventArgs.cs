// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.SaveProgressEventArgs
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.Zip
{
  public class SaveProgressEventArgs : ZipProgressEventArgs
  {
    private int _entriesSaved;

    internal SaveProgressEventArgs(
      string archiveName,
      bool before,
      int entriesTotal,
      int entriesSaved,
      ZipEntry entry)
      : base(archiveName, before ? ZipProgressEventType.Saving_BeforeWriteEntry : ZipProgressEventType.Saving_AfterWriteEntry)
    {
      this.EntriesTotal = entriesTotal;
      this.CurrentEntry = entry;
      this._entriesSaved = entriesSaved;
    }

    internal SaveProgressEventArgs()
    {
    }

    internal SaveProgressEventArgs(string archiveName, ZipProgressEventType flavor)
      : base(archiveName, flavor)
    {
    }

    internal static SaveProgressEventArgs ByteUpdate(
      string archiveName,
      ZipEntry entry,
      long bytesXferred,
      long totalBytes)
    {
      SaveProgressEventArgs progressEventArgs = new SaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_EntryBytesRead);
      progressEventArgs.ArchiveName = archiveName;
      progressEventArgs.CurrentEntry = entry;
      progressEventArgs.BytesTransferred = bytesXferred;
      progressEventArgs.TotalBytesToTransfer = totalBytes;
      return progressEventArgs;
    }

    internal static SaveProgressEventArgs Started(string archiveName) => new SaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_Started);

    internal static SaveProgressEventArgs Completed(string archiveName) => new SaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_Completed);

    public int EntriesSaved => this._entriesSaved;
  }
}
