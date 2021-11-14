// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.BadPasswordException
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Ionic.Zip
{
  [Guid("ebc25cf6-9120-4283-b972-0e5520d0000B")]
  [Serializable]
  public class BadPasswordException : ZipException
  {
    public BadPasswordException()
    {
    }

    public BadPasswordException(string message)
      : base(message)
    {
    }

    public BadPasswordException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected BadPasswordException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
