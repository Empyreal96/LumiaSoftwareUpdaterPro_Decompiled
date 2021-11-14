// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.PropertyValueFormatter
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Properties;
using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Text;

namespace Nokia.Lucid.DeviceInformation
{
  public sealed class PropertyValueFormatter : IPropertyValueFormatter
  {
    public static readonly PropertyValueFormatter Default = new PropertyValueFormatter();

    public object ReadFrom(byte[] buffer, int index, int count, PropertyType propertyType)
    {
      object obj;
      if (!this.TryReadFrom(buffer, index, count, propertyType, out obj))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NotSupportedException_MessageFormat_PropertyTypeNotSupported, (object) propertyType));
      return obj;
    }

    public bool TryReadFrom(
      byte[] buffer,
      int index,
      int count,
      PropertyType propertyType,
      out object value)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      switch (propertyType)
      {
        case PropertyType.Null:
          value = (object) null;
          return true;
        case PropertyType.Int32:
        case PropertyType.UInt32:
        case PropertyType.PropertyType:
        case PropertyType.Win32Error:
        case PropertyType.NTStatus:
          value = (object) BitConverter.ToInt32(buffer, index);
          return true;
        case PropertyType.Guid:
          value = (object) PropertyValueFormatter.ReadGuid(buffer, index, count);
          return true;
        case PropertyType.FileTime:
          value = (object) PropertyValueFormatter.ReadFileTime(buffer, index);
          return true;
        case PropertyType.Boolean:
          value = (object) PropertyValueFormatter.ReadBoolean(buffer, index);
          return true;
        case PropertyType.String:
        case PropertyType.SecurityDescriptorString:
        case PropertyType.StringIndirect:
          value = (object) PropertyValueFormatter.ReadUnicodeString(buffer, index, count);
          return true;
        case PropertyType.SecurityDescriptor:
          value = (object) PropertyValueFormatter.ReadSecurityDescriptor(buffer, index);
          return true;
        case PropertyType.SByteArray:
        case PropertyType.ByteArray:
          value = (object) PropertyValueFormatter.ReadByteArray(buffer, index, count);
          return true;
        case PropertyType.StringList:
          value = (object) PropertyValueFormatter.ReadUnicodeStringArray(buffer, index, count);
          return true;
        default:
          value = (object) null;
          return false;
      }
    }

    private static byte[] ReadByteArray(byte[] buffer, int index, int count)
    {
      byte[] numArray = new byte[count];
      Array.Copy((Array) buffer, index, (Array) numArray, 0, numArray.Length);
      return numArray;
    }

    private static string[] ReadUnicodeStringArray(byte[] buffer, int index, int count) => Encoding.Unicode.GetString(buffer, index, count).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);

    private static DateTime ReadFileTime(byte[] buffer, int index) => DateTime.FromFileTimeUtc(BitConverter.ToInt64(buffer, index));

    private static Guid ReadGuid(byte[] buffer, int index, int count)
    {
      if (index == 0 && count == buffer.Length)
        return new Guid(buffer);
      byte[] b = new byte[count];
      Array.Copy((Array) buffer, index, (Array) b, 0, b.Length);
      return new Guid(b);
    }

    private static bool ReadBoolean(byte[] buffer, int index) => buffer[index] != (byte) 0;

    private static string ReadUnicodeString(byte[] buffer, int index, int count) => Encoding.Unicode.GetString(buffer, index, count).TrimEnd(new char[1]);

    private static GenericSecurityDescriptor ReadSecurityDescriptor(
      byte[] buffer,
      int index)
    {
      return (GenericSecurityDescriptor) new RawSecurityDescriptor(buffer, index);
    }
  }
}
