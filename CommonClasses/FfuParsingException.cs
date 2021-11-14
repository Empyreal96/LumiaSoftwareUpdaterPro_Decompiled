// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FfuParsingException
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "reviewed")]
  [Serializable]
  public class FfuParsingException : FlashException
  {
    public FfuParsingException(
      uint errorCode,
      string message,
      FfuParsingException.FfuParsingFailureType failureType,
      string expectedCrc,
      string actualCrc)
      : base(errorCode, message)
    {
      this.FailureType = failureType;
      this.ExpectedHash = expectedCrc;
      this.ActualHash = actualCrc;
    }

    public FfuParsingException(
      uint errorCode,
      string message,
      FfuParsingException.FfuParsingFailureType failureType,
      string actualMd5)
      : base(errorCode, message)
    {
      this.FailureType = failureType;
      this.ExpectedHash = string.Empty;
      this.ActualHash = actualMd5;
    }

    public FfuParsingException.FfuParsingFailureType FailureType { get; private set; }

    public string ExpectedHash { get; private set; }

    public string ActualHash { get; private set; }

    public enum FfuParsingFailureType
    {
      CrcMismatch,
      Generic,
    }
  }
}
