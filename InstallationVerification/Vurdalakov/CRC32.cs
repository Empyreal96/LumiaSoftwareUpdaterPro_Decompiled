// Decompiled with JetBrains decompiler
// Type: Vurdalakov.CRC32
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;
using System.Security.Cryptography;

namespace Vurdalakov
{
  public abstract class CRC32 : HashAlgorithm
  {
    public CRC32() => this.HashSizeValue = 32;

    public uint Crc32Hash { get; protected set; }

    public static CRC32 Create() => (CRC32) new CRC32Managed();

    public static CRC32 Create(uint polynomial) => (CRC32) new CRC32Managed(polynomial);

    public static CRC32 Create(string hashName) => throw new NotImplementedException();
  }
}
