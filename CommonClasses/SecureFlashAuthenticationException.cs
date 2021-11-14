// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SecureFlashAuthenticationException
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro
{
  [Serializable]
  public class SecureFlashAuthenticationException : FlashException
  {
    public SecureFlashAuthenticationException(uint errorCode)
      : base(errorCode)
    {
    }

    public SecureFlashAuthenticationException(uint errorCode, string message)
      : base(errorCode, message)
    {
    }

    public SecureFlashAuthenticationException(
      uint errorCode,
      string message,
      Exception innerException)
      : base(errorCode, message, innerException)
    {
    }
  }
}
