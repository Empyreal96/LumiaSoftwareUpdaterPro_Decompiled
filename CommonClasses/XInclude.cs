// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.XInclude
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.LsuPro
{
  public class XInclude
  {
    private XInclude()
    {
    }

    public static string MergeFiles(string inputFileName) => new XInclude().MergeXmlFiles(inputFileName);

    private string MergeXmlFiles(string inputFileName) => this.WriteXmlDocumentToString(this.LoadXmlDocument(inputFileName, true));

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string WriteXmlDocumentToString(XmlDocument xmlDocument)
    {
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter w = XmlWriter.Create((TextWriter) stringWriter, new XmlWriterSettings()
        {
          Indent = true
        }))
        {
          xmlDocument.WriteTo(w);
          w.Flush();
          return stringWriter.GetStringBuilder().ToString();
        }
      }
    }

    private XmlDocument LoadXmlDocument(string fileName, bool rootDocument)
    {
      XmlDocument xmlDocument1 = new XmlDocument();
      try
      {
        string end;
        using (StreamReader streamReader = new StreamReader(fileName))
          end = streamReader.ReadToEnd();
        xmlDocument1.LoadXml(end);
      }
      catch
      {
        if (!rootDocument)
          return xmlDocument1;
        throw;
      }
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument1.NameTable);
      nsmgr.AddNamespace("xinclude", "http://www.w3.org/2001/XInclude");
      foreach (XmlNode selectNode in xmlDocument1.SelectNodes("//xinclude:include", nsmgr))
      {
        XmlAttribute attribute = selectNode.Attributes["parse"];
        if (attribute != null && !attribute.Value.Equals("xml"))
          throw new NotSupportedException("Only 'xml' value of 'parse' attribute is supported");
        string str = (selectNode.Attributes["href"] ?? throw new NotSupportedException("'href' attribute is mandadtory")).Value;
        if (!Path.IsPathRooted(str))
          str = Path.Combine(Path.GetDirectoryName(fileName), str);
        XmlDocument xmlDocument2 = this.LoadXmlDocument(str, false);
        XmlNode oldChild = selectNode;
        XmlNode node = (XmlNode) null;
        if (xmlDocument2.HasChildNodes)
        {
          node = xmlDocument2.FirstChild;
          while (node != null && node.NodeType != XmlNodeType.Element)
            node = node.FirstChild == null ? node.NextSibling : node.FirstChild;
        }
        selectNode.ParentNode.InsertBefore((XmlNode) xmlDocument1.CreateComment(selectNode.OuterXml), selectNode);
        if (node != null)
        {
          XmlNode newChild = xmlDocument1.ImportNode(node, true);
          selectNode.ParentNode.ReplaceChild(newChild, oldChild);
        }
      }
      return xmlDocument1;
    }
  }
}
