// Decompiled with JetBrains decompiler
// Type: Ionic.EnumUtil
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System;
using System.ComponentModel;

namespace Ionic
{
  internal sealed class EnumUtil
  {
    private EnumUtil()
    {
    }

    internal static string GetDescription(Enum value)
    {
      DescriptionAttribute[] customAttributes = (DescriptionAttribute[]) value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), false);
      return customAttributes.Length > 0 ? customAttributes[0].Description : value.ToString();
    }

    internal static object Parse(Type enumType, string stringRepresentation) => EnumUtil.Parse(enumType, stringRepresentation, false);

    internal static object Parse(Type enumType, string stringRepresentation, bool ignoreCase)
    {
      if (ignoreCase)
        stringRepresentation = stringRepresentation.ToLower();
      foreach (Enum @enum in Enum.GetValues(enumType))
      {
        string str = EnumUtil.GetDescription(@enum);
        if (ignoreCase)
          str = str.ToLower();
        if (str == stringRepresentation)
          return (object) @enum;
      }
      return Enum.Parse(enumType, stringRepresentation, ignoreCase);
    }
  }
}
