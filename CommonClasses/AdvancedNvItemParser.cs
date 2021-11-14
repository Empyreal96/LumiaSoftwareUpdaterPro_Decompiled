// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.AdvancedNvItemParser
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.LsuPro
{
  public class AdvancedNvItemParser
  {
    private string nvSettingsJsonFile;
    private XmlDocument confmlFile;
    private NvItemParser itemParser;
    private NvItemDescriptionParser descriptionParser;
    private bool basicParserInitialized;
    private bool descriptionParserInitialized;

    public AdvancedNvItemParser()
    {
      this.itemParser = new NvItemParser();
      this.basicParserInitialized = false;
      this.descriptionParserInitialized = false;
    }

    public void LoadDefinition(string definitionFilePath)
    {
      FileStream fileStream = new FileStream(definitionFilePath, FileMode.Open, FileAccess.Read);
      this.itemParser.LoadDefinition((Stream) fileStream);
      fileStream.Close();
      this.basicParserInitialized = true;
    }

    public void LoadConfiguration(string nvSettingsJsonFilePath, string confMlRootFilePath)
    {
      TextReader textReader = (TextReader) new StreamReader(nvSettingsJsonFilePath);
      this.nvSettingsJsonFile = textReader.ReadToEnd();
      textReader.Close();
      string xml = XInclude.MergeFiles(confMlRootFilePath);
      this.confmlFile = new XmlDocument();
      this.confmlFile.LoadXml(xml);
      this.descriptionParser = new NvItemDescriptionParser(this.nvSettingsJsonFile, this.confmlFile);
      this.descriptionParser.Initialize();
      this.descriptionParserInitialized = true;
    }

    public NvItemInformation ParseItemInformation(string message)
    {
      if (!this.basicParserInitialized)
        throw new InvalidOperationException("LoadDefinition has not been called");
      return this.ParseAdvancedInfo(this.itemParser.ConvertJsonMessageToNvItemInformation(message));
    }

    public NvItemInformation ParseItemInformation(int nvIdentifier, byte[] data)
    {
      if (!this.basicParserInitialized)
        throw new InvalidOperationException("LoadDefinition has not been called");
      return this.ParseAdvancedInfo(this.itemParser.ConvertResponseDataToNvItemInformation(nvIdentifier, data));
    }

    public NvItemInformation ParseItemInformation(string efsItemPath, byte[] data)
    {
      if (!this.basicParserInitialized)
        throw new InvalidOperationException("LoadDefinition has not been called");
      return this.ParseAdvancedInfo(this.itemParser.ConvertResponseDataToNvItemInformation(efsItemPath, data));
    }

    private NvItemInformation ParseAdvancedInfo(NvItemInformation nvItemInfo)
    {
      if (this.descriptionParserInitialized && this.descriptionParser.HasConfigurationForItem(nvItemInfo.Identifier))
      {
        NvItemViewConfiguration configurationForItem = this.descriptionParser.GetConfigurationForItem(nvItemInfo.Identifier);
        if (configurationForItem.HasItemDescription)
          nvItemInfo.DisplayName = configurationForItem.ItemName;
        nvItemInfo.Description = string.IsNullOrEmpty(configurationForItem.Description) ? (string) null : configurationForItem.Description.Replace("\t", string.Empty);
        nvItemInfo.HasValueDescriptions = configurationForItem.HasValueDescriptions;
        nvItemInfo.ValueDescriptionValueNames = configurationForItem.ValueDescriptionValueNames;
        nvItemInfo.ValueDescriptions = configurationForItem.ValueDescriptions;
        if (configurationForItem.ConvertNeededForItem)
        {
          configurationForItem.ItemInformation = nvItemInfo;
          if (!(configurationForItem.Converter == "bool2str") && !(configurationForItem.Converter == "bundle2commalist"))
            nvItemInfo.BitmaskStates = (IDictionary<int, bool>) configurationForItem.ConvertItem();
        }
        else
          nvItemInfo.BitmaskStates = (IDictionary<int, bool>) null;
      }
      return nvItemInfo;
    }
  }
}
