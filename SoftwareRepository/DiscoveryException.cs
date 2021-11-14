// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.DiscoveryException
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Runtime.Serialization;

namespace SoftwareRepository
{
  [Serializable]
  public class DiscoveryException : Exception
  {
    public DiscoveryException()
    {
    }

    public DiscoveryException(string message)
      : base(message)
    {
    }

    public DiscoveryException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected DiscoveryException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
