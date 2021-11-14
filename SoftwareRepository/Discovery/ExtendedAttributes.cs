// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Discovery.ExtendedAttributes
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Runtime.Serialization;

namespace SoftwareRepository.Discovery
{
  [Serializable]
  public class ExtendedAttributes : ISerializable
  {
    public System.Collections.Generic.Dictionary<string, string> Dictionary;

    public ExtendedAttributes() => this.Dictionary = new System.Collections.Generic.Dictionary<string, string>();

    protected ExtendedAttributes(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        return;
      this.Dictionary = new System.Collections.Generic.Dictionary<string, string>();
      foreach (SerializationEntry serializationEntry in info)
        this.Dictionary.Add(serializationEntry.Name, (string) serializationEntry.Value);
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        return;
      foreach (string key in this.Dictionary.Keys)
        info.AddValue(key, (object) this.Dictionary[key]);
    }
  }
}
