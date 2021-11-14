// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemDescriptionParser
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.LsuPro
{
  internal class NvItemDescriptionParser
  {
    private readonly NvItemJsonConfigurationParser jsonItemDefinitionFile;
    private readonly XmlDocument confmlDocument;
    private readonly XmlNamespaceManager namespaces;
    private Dictionary<string, NvItemViewConfiguration> itemViewConfigurations;

    internal NvItemDescriptionParser(string jsonData, XmlDocument confmlDocument)
    {
      this.jsonItemDefinitionFile = new NvItemJsonConfigurationParser(jsonData);
      this.confmlDocument = confmlDocument;
      this.namespaces = new XmlNamespaceManager(this.confmlDocument.NameTable);
      this.namespaces.AddNamespace("def", "urn:x-customization-configuration");
      this.Initialized = false;
    }

    internal bool Initialized { get; private set; }

    internal void Initialize()
    {
      this.itemViewConfigurations = new Dictionary<string, NvItemViewConfiguration>();
      foreach (string listItemId in this.jsonItemDefinitionFile.ListItemIds())
        this.itemViewConfigurations.Add(listItemId, this.ParseXmlDataTo(this.jsonItemDefinitionFile.GetItemConfiguration(listItemId)));
      foreach (string listEfsItemPath in this.jsonItemDefinitionFile.ListEfsItemPaths())
        this.itemViewConfigurations.Add(listEfsItemPath, this.ParseXmlDataTo(this.jsonItemDefinitionFile.GetItemConfiguration(listEfsItemPath)));
      this.Initialized = true;
    }

    internal IList<int> ListItemsThatHaveDescription()
    {
      if (!this.Initialized)
        throw new InvalidOperationException("Object not initialized");
      List<int> intList = new List<int>();
      foreach (KeyValuePair<string, NvItemViewConfiguration> viewConfiguration in this.itemViewConfigurations)
      {
        int result;
        if (int.TryParse(viewConfiguration.Key, out result) && viewConfiguration.Value.HasItemDescription)
          intList.Add(result);
      }
      return (IList<int>) intList;
    }

    internal IList<string> ListEfsItemsThatHaveDescription()
    {
      if (!this.Initialized)
        throw new InvalidOperationException("Object not initialized");
      IList<string> stringList = (IList<string>) new List<string>();
      foreach (KeyValuePair<string, NvItemViewConfiguration> viewConfiguration in this.itemViewConfigurations)
      {
        if (!int.TryParse(viewConfiguration.Key, out int _) && viewConfiguration.Value.HasItemDescription)
          stringList.Add(viewConfiguration.Key);
      }
      return stringList;
    }

    internal IList<int> ListItemsThatHaveValueDescription()
    {
      if (!this.Initialized)
        throw new InvalidOperationException("Object not initialized");
      IList<int> intList = (IList<int>) new List<int>();
      foreach (KeyValuePair<string, NvItemViewConfiguration> viewConfiguration in this.itemViewConfigurations)
      {
        int result;
        if (int.TryParse(viewConfiguration.Key, out result) && viewConfiguration.Value.HasValueDescriptions)
          intList.Add(result);
      }
      return intList;
    }

    internal List<string> ListEfsItemsThatHaveValueDescription()
    {
      if (!this.Initialized)
        throw new InvalidOperationException("Object not initialized");
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, NvItemViewConfiguration> viewConfiguration in this.itemViewConfigurations)
      {
        if (!int.TryParse(viewConfiguration.Key, out int _) && viewConfiguration.Value.HasValueDescriptions)
          stringList.Add(viewConfiguration.Key);
      }
      return stringList;
    }

    internal NvItemViewConfiguration GetConfigurationForItem(string itemId)
    {
      if (!this.Initialized)
        throw new InvalidOperationException("Object not initialized");
      return this.itemViewConfigurations[itemId];
    }

    internal bool HasConfigurationForItem(string itemId) => this.itemViewConfigurations.ContainsKey(itemId);

    private NvItemViewConfiguration ParseXmlDataTo(
      NvItemJsonConfiguration itemJsonConfiguration)
    {
      NvItemViewConfiguration viewConfiguration = new NvItemViewConfiguration(itemJsonConfiguration);
      XmlNode xmlNode = (XmlNode) null;
      if (!string.IsNullOrEmpty(itemJsonConfiguration.XpathToItemDescription))
      {
        xmlNode = this.confmlDocument.SelectSingleNode(itemJsonConfiguration.XpathToItemDescription, this.namespaces);
        if (xmlNode != null)
        {
          viewConfiguration.HasItemDescription = true;
          viewConfiguration.Description = xmlNode.InnerText.Replace("\t", string.Empty);
        }
      }
      IDictionary<int, string> dictionary1 = (IDictionary<int, string>) new Dictionary<int, string>();
      IDictionary<int, string> dictionary2 = (IDictionary<int, string>) new Dictionary<int, string>();
      if (itemJsonConfiguration.XpathToItemValueDescriptions != null)
      {
        foreach (KeyValuePair<int, string> valueDescription in itemJsonConfiguration.XpathToItemValueDescriptions)
        {
          xmlNode = this.confmlDocument.SelectSingleNode(valueDescription.Value, this.namespaces);
          if (xmlNode != null)
          {
            dictionary1.Add(valueDescription.Key, xmlNode.InnerText.Replace("\t", string.Empty));
            dictionary2.Add(valueDescription.Key, xmlNode.Attributes["name"].Value);
          }
        }
        if (dictionary1.Count > 0)
        {
          viewConfiguration.HasValueDescriptions = true;
          viewConfiguration.ValueDescriptions = dictionary1;
          viewConfiguration.ValueDescriptionValueNames = dictionary2;
        }
      }
      viewConfiguration.ItemName = xmlNode == null || xmlNode.Attributes == null ? itemJsonConfiguration.ItemIdentifier : xmlNode.Attributes["name"].Value;
      return viewConfiguration;
    }
  }
}
