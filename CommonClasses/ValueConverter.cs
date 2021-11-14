// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ValueConverter
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public static class ValueConverter
  {
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "reviewed", MessageId = "2#")]
    public static string ConvertByteArrayToNumberString(string type, byte[] values, ref int index)
    {
      string str = string.Empty;
      if (type == "uint8")
      {
        str = ValueConverter.ConvertUInt8(values, index);
        ++index;
      }
      if (type == "int8")
      {
        str = ValueConverter.ConvertInt8(values, index);
        ++index;
      }
      if (type == "uint16")
      {
        str = ValueConverter.ConvertUInt16(values, index);
        index += 2;
      }
      if (type == "int16")
      {
        str = ValueConverter.ConvertInt16(values, index);
        index += 2;
      }
      if (type == "uint32")
      {
        str = ValueConverter.ConvertUInt32(values, index);
        index += 4;
      }
      if (type == "int32")
      {
        str = ValueConverter.ConvertInt32(values, index);
        index += 4;
      }
      if (type == "uint64")
      {
        str = ValueConverter.ConvertUInt64(values, index);
        index += 8;
      }
      if (type == "int64")
      {
        str = ValueConverter.ConvertInt64(values, index);
        index += 8;
      }
      return str;
    }

    public static byte[] ConvertNumberStringToByteList(string type, string value)
    {
      List<byte> byteList = new List<byte>();
      if (type == "uint8")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromUInt8(value));
      if (type == "int8")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromInt8(value));
      if (type == "uint16")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromUInt16(value));
      if (type == "int16")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromInt16(value));
      if (type == "uint32")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromUInt32(value));
      if (type == "int32")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromInt32(value));
      if (type == "uint64")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromUInt64(value));
      if (type == "int64")
        byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertFromInt64(value));
      return byteList.ToArray();
    }

    private static string ConvertInt8(byte[] values, int index) => string.Empty + (object) sbyte.Parse(values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ConvertInt16(byte[] values, int index) => string.Empty + (object) short.Parse(values[index + 1].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ConvertInt32(byte[] values, int index) => string.Empty + (object) int.Parse(values[index + 3].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 2].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 1].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ConvertInt64(byte[] values, int index) => string.Empty + (object) long.Parse(values[index + 7].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 6].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 5].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 4].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 3].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 2].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 1].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ConvertUInt8(byte[] values, int index) => string.Empty + (object) values[index];

    private static string ConvertUInt16(byte[] values, int index) => string.Empty + (object) ushort.Parse(values[index + 1].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ConvertUInt32(byte[] values, int index) => string.Empty + (object) uint.Parse(values[index + 3].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 2].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 1].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static string ConvertUInt64(byte[] values, int index) => string.Empty + (object) ulong.Parse(values[index + 7].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 6].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 5].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 4].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 3].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 2].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index + 1].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture) + values[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);

    private static List<byte> ConvertFromUInt64(string valuesString)
    {
      string str = ulong.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X16", (IFormatProvider) CultureInfo.InvariantCulture);
      return new List<byte>()
      {
        byte.Parse(str.Substring(14, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(12, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(10, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(8, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(6, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    private static List<byte> ConvertFromUInt32(string valuesString)
    {
      string str = uint.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture);
      return new List<byte>()
      {
        byte.Parse(str.Substring(6, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    private static List<byte> ConvertFromUInt16(string valuesString)
    {
      string str = ushort.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture);
      return new List<byte>()
      {
        byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    private static List<byte> ConvertFromUInt8(string valuesString) => new List<byte>()
    {
      byte.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture)
    };

    private static List<byte> ConvertFromInt64(string valuesString)
    {
      string str = long.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X16", (IFormatProvider) CultureInfo.InvariantCulture);
      return new List<byte>()
      {
        byte.Parse(str.Substring(14, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(12, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(10, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(8, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(6, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    private static List<byte> ConvertFromInt32(string valuesString)
    {
      string str = int.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture);
      return new List<byte>()
      {
        byte.Parse(str.Substring(6, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    private static List<byte> ConvertFromInt16(string valuesString)
    {
      string str = short.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture);
      return new List<byte>()
      {
        byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture),
        byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    private static List<byte> ConvertFromInt8(string valuesString) => new List<byte>()
    {
      byte.Parse(sbyte.Parse(valuesString, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture)
    };
  }
}
