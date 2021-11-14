// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemJsonConfigurationParser
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.LsuPro
{
  internal class NvItemJsonConfigurationParser
  {
    private readonly Dictionary<string, NvItemJsonConfiguration> parsedItemConfigurations;
    private readonly string jsonFileContent;
    private List<string> jsonNvItemIdentifiers = new List<string>();
    private JObject nvitemJobject;

    internal NvItemJsonConfigurationParser(string jsonData)
    {
      this.jsonFileContent = jsonData;
      this.parsedItemConfigurations = new Dictionary<string, NvItemJsonConfiguration>();
      this.ParseNuageJsonConfigurationContent();
    }

    internal List<string> ListItemIds()
    {
      List<string> stringList = new List<string>();
      foreach (string key in this.parsedItemConfigurations.Keys)
      {
        if (int.TryParse(key, out int _))
          stringList.Add(key);
      }
      return stringList;
    }

    internal List<string> ListEfsItemPaths()
    {
      List<string> stringList = new List<string>();
      foreach (string key in this.parsedItemConfigurations.Keys)
      {
        if (!int.TryParse(key, out int _))
          stringList.Add(key);
      }
      return stringList;
    }

    internal Dictionary<int, string> GetXpathForRetrievingValueDescriptionsForItem(
      string itemId)
    {
      return this.parsedItemConfigurations[itemId].XpathToItemValueDescriptions;
    }

    internal string GetXpathForRetrievingItemDescription(string itemPath) => this.parsedItemConfigurations[itemPath].XpathToItemDescription;

    internal NvItemJsonConfiguration GetItemConfiguration(string itemId) => this.parsedItemConfigurations[itemId];

    private void ParseNuageJsonConfigurationContent()
    {
      this.nvitemJobject = JObject.Parse(JObject.Parse(this.jsonFileContent)["nv_items"].ToString());
      this.jsonNvItemIdentifiers = ((IEnumerable<KeyValuePair<string, JToken>>) this.nvitemJobject.ToArray<KeyValuePair<string, JToken>>()).Select<KeyValuePair<string, JToken>, string>((Func<KeyValuePair<string, JToken>, string>) (keyValuePair => keyValuePair.Key)).ToList<string>();
      foreach (string nvItemIdentifier in this.jsonNvItemIdentifiers)
      {
        this.parsedItemConfigurations.Add(nvItemIdentifier, new NvItemJsonConfiguration(nvItemIdentifier));
        JToken jtoken = (JToken) JObject.Parse(this.nvitemJobject[nvItemIdentifier].ToString());
        if (jtoken[(object) "convert"] != null)
          this.parsedItemConfigurations[nvItemIdentifier].Convert = jtoken[(object) "convert"].ToString();
        if (jtoken[(object) "confml_Value"] != null)
        {
          string itemRef = jtoken[(object) "confml_Value"].ToString();
          this.HandleValueOrValueList(nvItemIdentifier, itemRef);
        }
        if (jtoken[(object) "confml_Value_list"] != null)
        {
          string itemRef = jtoken[(object) "confml_Value_list"].ToString();
          this.HandleValueOrValueList(nvItemIdentifier, itemRef);
        }
        Dictionary<int, string> dictionary = new Dictionary<int, string>();
        if (jtoken[(object) "confml_Value_group"] != null)
        {
          foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) JObject.Parse(jtoken[(object) "confml_Value_group"].ToString()))
          {
            string str1 = keyValuePair.Value.ToString();
            string str2 = str1.Substring(str1.LastIndexOf(".", StringComparison.Ordinal) + 1);
            dictionary.Add(int.Parse(keyValuePair.Key, (IFormatProvider) CultureInfo.InvariantCulture), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//def:feature[@ref=\"NVSettings\"]/def:setting[@ref=\"{0}\"]", (object) str2));
          }
          this.parsedItemConfigurations[nvItemIdentifier].XpathToItemValueDescriptions = dictionary;
        }
      }
    }

    private void HandleValueOrValueList(string knownItem, string itemRef)
    {
      string[] strArray = itemRef.Substring(itemRef.LastIndexOf(":", StringComparison.Ordinal) + 1).Split('.');
      if (strArray.Length == 2)
      {
        string str1 = strArray[0];
        string str2 = strArray[1];
        this.parsedItemConfigurations[knownItem].XpathToItemDescription = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//def:feature[@ref=\"{0}\"]/def:setting[@ref=\"{1}\"]", (object) str1, (object) str2);
      }
      else
      {
        itemRef = itemRef.Substring(itemRef.LastIndexOf(".", StringComparison.Ordinal) + 1);
        this.parsedItemConfigurations[knownItem].XpathToItemDescription = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//def:feature[@ref=\"NVSettings\"]/def:setting[@ref=\"{0}\"]", (object) itemRef);
      }
    }
  }
}
