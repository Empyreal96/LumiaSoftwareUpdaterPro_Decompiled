// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.IPropertyValueFormatter
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

namespace Nokia.Lucid.DeviceInformation
{
  public interface IPropertyValueFormatter
  {
    object ReadFrom(byte[] buffer, int index, int count, PropertyType propertyType);

    bool TryReadFrom(
      byte[] buffer,
      int index,
      int count,
      PropertyType propertyType,
      out object value);
  }
}
