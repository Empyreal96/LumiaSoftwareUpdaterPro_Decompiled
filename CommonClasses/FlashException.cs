// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FlashException
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "reviewed")]
  [Serializable]
  public class FlashException : Exception
  {
    public FlashException(uint errorCode)
    {
      this.ErrorCode = errorCode;
      this.SafePointReached = true;
    }

    public FlashException(uint errorCode, string message)
      : base(message)
    {
      this.ErrorCode = errorCode;
      this.SafePointReached = true;
    }

    public FlashException(uint errorCode, string message, bool safePointReached)
      : base(message)
    {
      this.ErrorCode = errorCode;
      this.SafePointReached = safePointReached;
    }

    public FlashException(uint errorCode, string message, Exception innerException)
      : base(message, innerException)
    {
      this.ErrorCode = errorCode;
    }

    public uint ErrorCode { get; private set; }

    public bool SafePointReached { get; private set; }
  }
}
