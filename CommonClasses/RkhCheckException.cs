// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.RkhCheckException
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "reviewed")]
  [Serializable]
  public class RkhCheckException : FlashException
  {
    public RkhCheckException(
      RkhCheckException.RhkCheckIssue issue,
      uint errorCode,
      string errorMessage)
      : base(errorCode, errorMessage)
    {
      this.RkhFailureType = issue;
    }

    public RkhCheckException(
      RkhCheckException.RhkCheckIssue issue,
      uint errorCode,
      string message,
      Exception innerException)
      : base(errorCode, message, innerException)
    {
      this.RkhFailureType = issue;
    }

    public RkhCheckException.RhkCheckIssue RkhFailureType { get; private set; }

    public enum RhkCheckIssue
    {
      RkhMismatch,
      BootLoadersNotSigned,
      UefiNotSigned,
    }
  }
}
