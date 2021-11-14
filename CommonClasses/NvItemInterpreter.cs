// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemInterpreter
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.LsuPro
{
  public class NvItemInterpreter
  {
    public string Intrepret(NvItemInformation data, string converter)
    {
      if (!(converter == "booleangroup2bitmaskbandpref"))
      {
        if (!(converter == "booleangroup2bitmask"))
          return !(converter == "bool2str") ? (converter == "bundle2commalist" ? string.Empty : string.Empty) : (data.GetMembers()[0].Values[0].ToString() == "1" ? "True" : "False");
        IList<NvItemMember> members = data.GetMembers();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (object obj in (IEnumerable<object>) members[0].Values)
        {
          string str = this.Binary(obj.ToString(), obj.GetType());
          while (str.Length < 8)
            str += "0";
          stringBuilder.Append(str);
        }
        return stringBuilder.ToString();
      }
      IList<NvItemMember> members1 = data.GetMembers();
      StringBuilder stringBuilder1 = new StringBuilder();
      foreach (object obj in (IEnumerable<object>) members1[1].Values)
      {
        string str = this.Binary(obj.ToString(), obj.GetType());
        while (str.Length < 8)
          str += "0";
        stringBuilder1.Append(str);
      }
      return stringBuilder1.ToString();
    }

    private string Binary(string value, Type type) => type.ToString().Contains(".U") ? this.GetBinaryRepresentation(ulong.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture)) : this.GetBinaryRepresentation(long.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture));

    private string GetBinaryRepresentation(ulong number)
    {
      string str = string.Empty + (object) (number % 2UL);
      if (number >= 2UL)
        str += this.GetBinaryRepresentation(number / 2UL);
      return str;
    }

    private string GetBinaryRepresentation(long number)
    {
      string str = string.Empty + (object) (number % 2L);
      if (number >= 2L)
        str += this.GetBinaryRepresentation(number / 2L);
      return str;
    }
  }
}
