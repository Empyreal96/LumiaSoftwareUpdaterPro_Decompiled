// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemMember
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.LsuPro
{
  public class NvItemMember
  {
    public string Name { get; internal set; }

    public Type NvItemType { get; internal set; }

    public IList<object> Values { get; internal set; }

    public string StringRepresentation
    {
      get
      {
        string empty = string.Empty;
        if (this.NvItemType == typeof (byte))
        {
          foreach (byte num in (IEnumerable<object>) this.Values)
          {
            if (num != (byte) 0)
              empty += ((char) num).ToString();
          }
        }
        return empty;
      }
    }

    public override bool Equals(object obj) => obj != null && obj is NvItemMember nvItemMember && (this.Values.Count == nvItemMember.Values.Count && this.Name == nvItemMember.Name) && this.NvItemType == nvItemMember.NvItemType && !this.Values.Except<object>((IEnumerable<object>) nvItemMember.Values).Any<object>();

    public override int GetHashCode() => this.Name.GetHashCode() ^ this.Values.Count;
  }
}
