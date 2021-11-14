// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.ZipEntryTimestamp
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System;

namespace Ionic.Zip
{
  [Flags]
  public enum ZipEntryTimestamp
  {
    None = 0,
    DOS = 1,
    Windows = 2,
    Unix = 4,
    InfoZip1 = 8,
  }
}
