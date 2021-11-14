// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemViewConfiguration
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.LsuPro
{
  internal class NvItemViewConfiguration
  {
    private NvItemJsonConfiguration jsonConfig;

    internal NvItemViewConfiguration(NvItemJsonConfiguration itemJsonConfiguration)
    {
      this.jsonConfig = itemJsonConfiguration;
      this.Converter = itemJsonConfiguration.Convert;
    }

    internal NvItemInformation ItemInformation { get; set; }

    internal string ItemName { get; set; }

    internal string Converter { get; set; }

    internal string Description { get; set; }

    internal IDictionary<int, string> ValueDescriptions { get; set; }

    internal bool HasItemDescription { get; set; }

    internal bool HasValueDescriptions { get; set; }

    internal IDictionary<int, string> ValueDescriptionValueNames { get; set; }

    internal bool ConvertNeededForItem => !string.IsNullOrWhiteSpace(this.Converter);

    internal Dictionary<int, bool> ConvertItem()
    {
      Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
      string str = new NvItemInterpreter().Intrepret(this.ItemInformation, this.Converter);
      if (!string.IsNullOrEmpty(str))
      {
        foreach (int num in this.jsonConfig.XpathToItemValueDescriptions.Keys.ToList<int>())
        {
          bool flag = str.Length > num && str[num].Equals('1');
          dictionary.Add(num, flag);
        }
      }
      return dictionary;
    }
  }
}
