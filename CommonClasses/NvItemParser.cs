// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemParser
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.LsuPro
{
  public class NvItemParser
  {
    private bool initialized;
    private XmlDocument definitionXml;

    public NvItemParser() => this.initialized = false;

    ~NvItemParser()
    {
      if (!this.initialized)
        return;
      this.UnloadDefinition();
    }

    public bool IsInitialized() => this.initialized;

    public void LoadDefinition(Stream definitionStream)
    {
      this.definitionXml = new XmlDocument();
      this.definitionXml.Load(definitionStream);
      this.initialized = true;
    }

    public void UnloadDefinition()
    {
      this.definitionXml = (XmlDocument) null;
      this.initialized = false;
    }

    [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Justification = "reviewed", MessageId = "System.Xml.XmlNode")]
    public XmlNode ConvertResponseDataToXmlNode(int id, byte[] data)
    {
      if (!this.initialized)
        throw new InvalidOperationException("NvItemParser needs to have definition specified.");
      int index1 = 0;
      XmlNode xmlNode1 = this.definitionXml.SelectSingleNode("//NvItem[@id=" + (object) id + "]");
      XmlNode xmlNode2;
      if (xmlNode1 == null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index2 = 0; index2 < data.Length; ++index2)
        {
          if (index2 != 0)
            stringBuilder.Append(" ");
          stringBuilder.Append(data[index2]);
        }
        string xml = "<NvItem id=\"" + (object) id + "\" name=\"\"><Member type=\"uint8\" sizeOf=\"" + (object) data.Length + "\" name=\"\">" + (object) stringBuilder + "</Member></NvItem>";
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        xmlNode2 = xmlDocument.DocumentElement.Clone();
      }
      else
      {
        xmlNode2 = xmlNode1.Clone();
        foreach (XmlNode childNode in xmlNode2.ChildNodes)
        {
          if (childNode.Attributes != null)
          {
            int num = int.Parse(childNode.Attributes["sizeOf"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
            for (int index2 = 0; index2 < num; ++index2)
            {
              if (index1 >= data.Length)
              {
                List<byte> byteList = new List<byte>((IEnumerable<byte>) data);
                byteList.AddRange((IEnumerable<byte>) new byte[8]);
                data = byteList.ToArray();
              }
              childNode.InnerText = index2 != 0 ? childNode.InnerText + " " + ValueConverter.ConvertByteArrayToNumberString(childNode.Attributes["type"].Value, data, ref index1) : ValueConverter.ConvertByteArrayToNumberString(childNode.Attributes["type"].Value, data, ref index1);
            }
          }
        }
      }
      return xmlNode2;
    }

    [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Justification = "reviewed", MessageId = "System.Xml.XmlNode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public byte[] ConvertByteArrayFromXmlNode(XmlNode node)
    {
      List<byte> byteList = new List<byte>();
      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (childNode.Attributes != null)
        {
          string type = childNode.Attributes["type"].Value;
          int num = int.Parse(childNode.Attributes["sizeOf"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          string[] strArray = childNode.InnerText.Split(' ');
          for (int index = 0; index < num; ++index)
            byteList.AddRange((IEnumerable<byte>) ValueConverter.ConvertNumberStringToByteList(type, strArray[index]));
        }
      }
      return byteList.ToArray();
    }

    [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Justification = "reviewed", MessageId = "System.Xml.XmlNode")]
    public string ConvertJsonMessageFromXmlNode(XmlNode node)
    {
      byte[] numArray = this.ConvertByteArrayFromXmlNode(node);
      StringBuilder stringBuilder = new StringBuilder("[");
      for (int index = 0; index < numArray.Length; ++index)
      {
        if (index != 0)
          stringBuilder.Append(", ");
        stringBuilder.Append(numArray[index]);
      }
      stringBuilder.Append("]");
      if (node.Name == "NvItem")
        return "{\"jsonrpc\": \"2.0\", \"method\": \"WriteNVData\", \"params\": {\"MessageVersion\": 0, \"ID\": " + node.Attributes["id"].Value + ", \"NVData\": " + (object) stringBuilder + "}, \"id\": 0}";
      string str = "Item";
      if (numArray.Length > 106)
        str = "Data";
      return "{\"jsonrpc\": \"2.0\", \"method\": \"WriteEFSData\", \"params\": {\"MessageVersion\": 0, \"FilePath\":\"" + node.Attributes["fullpathname"].Value + "\", \"Data\":" + (object) stringBuilder + ", \"ItemType\":\"" + str + "\"}, \"id\": 0}";
    }

    public NvItemInformation ConvertResponseDataToNvItemInformation(
      int id,
      byte[] data)
    {
      NvItemInformation informationFromXmlNode = NvItemParser.GenerateNvItemInformationFromXmlNode(this.ConvertResponseDataToXmlNode(id, data));
      informationFromXmlNode.RawData = data;
      return informationFromXmlNode;
    }

    public NvItemInformation ConvertResponseDataToNvItemInformation(
      string efsItemPath,
      byte[] data)
    {
      NvItemInformation informationFromXmlNode = NvItemParser.GenerateNvItemInformationFromXmlNode(this.ConvertResponseDataToXmlNode(efsItemPath, data));
      informationFromXmlNode.RawData = data;
      return informationFromXmlNode;
    }

    public NvItemInformation ConvertJsonMessageToNvItemInformation(
      string jsonMessage)
    {
      XmlNode xmlNode = this.ConvertJsonMessageToXmlNode(jsonMessage);
      JsonNvItem jsonNvItem = JsonConvert.DeserializeObject<JsonNvItem>(jsonMessage);
      NvItemInformation informationFromXmlNode = NvItemParser.GenerateNvItemInformationFromXmlNode(xmlNode);
      informationFromXmlNode.RawData = !string.IsNullOrWhiteSpace(jsonNvItem.Parameters.FilePath) ? jsonNvItem.Parameters.Data.ToArray<byte>() : jsonNvItem.Parameters.NvData.ToArray<byte>();
      informationFromXmlNode.SubscriptionId = jsonNvItem.Parameters.SubscriptionId;
      return informationFromXmlNode;
    }

    [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Justification = "reviewed", MessageId = "System.Xml.XmlNode")]
    public XmlNode ConvertJsonMessageToXmlNode(string jsonMessage)
    {
      JsonNvItem jsonNvItem = JsonConvert.DeserializeObject<JsonNvItem>(jsonMessage);
      return jsonNvItem.Method != "WriteEFSData" ? this.ConvertResponseDataToXmlNode(jsonNvItem.Parameters.Id, jsonNvItem.Parameters.NvData.ToArray<byte>()) : this.ConvertResponseDataToXmlNode(jsonNvItem.Parameters.FilePath, jsonNvItem.Parameters.Data.ToArray<byte>());
    }

    [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Justification = "reviewed", MessageId = "System.Xml.XmlNode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public XmlNode ConvertResponseDataToXmlNode(string efsItemPath, byte[] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < data.Length; ++index)
      {
        if (index != 0)
          stringBuilder.Append(" ");
        stringBuilder.Append(data[index]);
      }
      string xml = "<NvEfsItem fullpathname=\"" + efsItemPath + "\" name=\"" + efsItemPath + "\"><Member type=\"uint8\" sizeOf=\"" + (object) data.Length + "\" name=\"\">" + (object) stringBuilder + "</Member></NvEfsItem>";
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xml);
      return xmlDocument.DocumentElement.Clone();
    }

    private static NvItemInformation GenerateNvItemInformationFromXmlNode(
      XmlNode xmlNode)
    {
      NvItemInformation nvItemInformation = new NvItemInformation();
      if (xmlNode.Name == "NvItem")
      {
        nvItemInformation.Identifier = xmlNode.SelectSingleNode("@id").InnerText;
        nvItemInformation.Name = xmlNode.SelectSingleNode("@name").InnerText;
        nvItemInformation.DisplayName = nvItemInformation.Name;
        nvItemInformation.IsEfsItem = false;
      }
      else
      {
        nvItemInformation.Identifier = xmlNode.SelectSingleNode("@fullpathname").InnerText;
        nvItemInformation.Name = xmlNode.SelectSingleNode("@fullpathname").InnerText;
        nvItemInformation.DisplayName = nvItemInformation.Name;
        nvItemInformation.IsEfsItem = true;
      }
      foreach (XmlNode selectNode in xmlNode.SelectNodes("Member"))
      {
        string innerText1 = selectNode.SelectSingleNode("@type").InnerText;
        int int32 = Convert.ToInt32(selectNode.SelectSingleNode("@sizeOf").InnerText, (IFormatProvider) CultureInfo.InvariantCulture);
        string innerText2 = selectNode.SelectSingleNode("@name").InnerText;
        string innerText3 = selectNode.InnerText;
        List<object> values = new List<object>();
        string[] tokens = (string[]) null;
        Type type = (Type) null;
        if (int32 > 0)
          tokens = innerText3.Split(' ');
        if (tokens.Length == int32)
          type = NvItemParser.ConvertMembersToRealValues(innerText1.ToLowerInvariant(), tokens, values);
        NvItemMember member = new NvItemMember()
        {
          Name = innerText2,
          NvItemType = type,
          Values = (IList<object>) values
        };
        nvItemInformation.AddMember(member);
      }
      return nvItemInformation;
    }

    private static Type ConvertMembersToRealValues(
      string type,
      string[] tokens,
      List<object> values)
    {
      Type type1 = (Type) null;
      if (type == "int8")
      {
        type1 = typeof (sbyte);
        foreach (string token in tokens)
          values.Add((object) Convert.ToSByte(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "uint8")
      {
        type1 = typeof (byte);
        foreach (string token in tokens)
          values.Add((object) Convert.ToByte(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "int16")
      {
        type1 = typeof (short);
        foreach (string token in tokens)
          values.Add((object) Convert.ToInt16(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "uint16")
      {
        type1 = typeof (ushort);
        foreach (string token in tokens)
          values.Add((object) Convert.ToUInt16(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "int32")
      {
        type1 = typeof (int);
        foreach (string token in tokens)
          values.Add((object) Convert.ToInt32(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "uint32")
      {
        type1 = typeof (uint);
        foreach (string token in tokens)
          values.Add((object) Convert.ToUInt32(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "int64")
      {
        type1 = typeof (long);
        foreach (string token in tokens)
          values.Add((object) Convert.ToInt64(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (type == "uint64")
      {
        type1 = typeof (ulong);
        foreach (string token in tokens)
          values.Add((object) Convert.ToUInt64(token, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      return type1;
    }
  }
}
