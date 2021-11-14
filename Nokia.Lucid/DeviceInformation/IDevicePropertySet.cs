// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.IDevicePropertySet
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System.Collections.Generic;

namespace Nokia.Lucid.DeviceInformation
{
  public interface IDevicePropertySet
  {
    IEnumerable<PropertyKey> EnumeratePropertyKeys();

    object ReadProperty(PropertyKey key, IPropertyValueFormatter formatter);

    bool TryReadProperty(PropertyKey key, IPropertyValueFormatter formatter, out object value);
  }
}
