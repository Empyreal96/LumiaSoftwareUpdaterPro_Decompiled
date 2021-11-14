// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.AddProgressEventArgs
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.Zip
{
  public class AddProgressEventArgs : ZipProgressEventArgs
  {
    internal AddProgressEventArgs()
    {
    }

    private AddProgressEventArgs(string archiveName, ZipProgressEventType flavor)
      : base(archiveName, flavor)
    {
    }

    internal static AddProgressEventArgs AfterEntry(
      string archiveName,
      ZipEntry entry,
      int entriesTotal)
    {
      AddProgressEventArgs progressEventArgs = new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_AfterAddEntry);
      progressEventArgs.EntriesTotal = entriesTotal;
      progressEventArgs.CurrentEntry = entry;
      return progressEventArgs;
    }

    internal static AddProgressEventArgs Started(string archiveName) => new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Started);

    internal static AddProgressEventArgs Completed(string archiveName) => new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Completed);
  }
}
