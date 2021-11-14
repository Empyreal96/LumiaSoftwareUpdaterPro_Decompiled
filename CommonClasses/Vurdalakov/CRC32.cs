// Decompiled with JetBrains decompiler
// Type: Vurdalakov.CRC32
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Vurdalakov
{
  public abstract class CRC32 : HashAlgorithm
  {
    protected CRC32() => this.HashSizeValue = 32;

    public uint CRC32Hash { get; protected set; }

    public static CRC32 Create() => (CRC32) new CRC32Managed();

    public static CRC32 Create(uint polynomial) => (CRC32) new CRC32Managed(polynomial);

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "on purpose", MessageId = "hashName")]
    public static CRC32 Create(string hashName) => throw new NotImplementedException();
  }
}
