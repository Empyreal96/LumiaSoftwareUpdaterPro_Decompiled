// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.VplHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Xml;

namespace Microsoft.LsuPro
{
  public class VplHelper
  {
    private FileStreamHelper fileStreamHelper;

    public VplHelper()
      : this(new FileStreamHelper())
    {
    }

    public VplHelper(FileStreamHelper fileStreamHelper) => this.fileStreamHelper = fileStreamHelper;

    public string GetProductTypeFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "TypeDesignator")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetProductCodeFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "ProductCode")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetVariantGroupFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "VariantGroup")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetBuildTypeFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "BuildType")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetAkVersionFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName.ToLowerInvariant() == "akversion")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetOsVersionFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "os_version" || xmlReader.LocalName.ToLowerInvariant() == "osversion")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetBspVersionFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName.ToLowerInvariant() == "bspversion" || xmlReader.LocalName.ToLowerInvariant() == "bsp_version")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetPackageUsePurposeFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "PackageUse")
                return xmlReader.GetAttribute("purpose");
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetPackageUseTypeFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "PackageUse")
                return xmlReader.GetAttribute("releaseType");
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetSoftwareVersionFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "SwVersion")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetVariantVersionFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "VariantVersion")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetVariantDescriptionFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileStream))
          {
            while (xmlReader.Read())
            {
              if (xmlReader.LocalName == "Description")
                return xmlReader.ReadElementContentAsString();
            }
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
    }

    public string GetFfuFileFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader reader = XmlReader.Create(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(reader);
            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("File"))
            {
              string empty = string.Empty;
              string innerText;
              try
              {
                innerText = xmlNode.SelectSingleNode("FileSubType").InnerText;
              }
              catch (Exception ex)
              {
                continue;
              }
              if (string.Compare(innerText, "WindowsPhone", StringComparison.OrdinalIgnoreCase) == 0)
              {
                string path = Path.Combine(Path.GetDirectoryName(vplFilePath), xmlNode.SelectSingleNode("Name").InnerText);
                return File.Exists(path) ? path : string.Empty;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
      return string.Empty;
    }

    public byte[] GetCrcForFileInVpl(string vplFilePath, string fileName)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader reader = XmlReader.Create(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(reader);
            foreach (XmlNode node in xmlDocument.GetElementsByTagName("File"))
            {
              string empty = string.Empty;
              string innerText1;
              try
              {
                innerText1 = node.SelectSingleNode("FileSubType").InnerText;
              }
              catch (Exception ex)
              {
                continue;
              }
              if (string.Compare(innerText1, "WindowsPhone", StringComparison.OrdinalIgnoreCase) == 0)
              {
                string innerText2 = node.SelectSingleNode("Name").InnerText;
                if (fileName == innerText2)
                  return this.GetExpectedFileHash(node);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return (byte[]) null;
      }
      return (byte[]) null;
    }

    public string GetCustomerNviFileFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader reader = XmlReader.Create(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(reader);
            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("File"))
            {
              string empty = string.Empty;
              string innerText;
              try
              {
                innerText = xmlNode.SelectSingleNode("FileSubType").InnerText;
              }
              catch (Exception ex)
              {
                continue;
              }
              if (string.Compare(innerText, "CustomerNvItems", StringComparison.OrdinalIgnoreCase) == 0)
              {
                string path = Path.Combine(Path.GetDirectoryName(vplFilePath), xmlNode.SelectSingleNode("Name").InnerText);
                return File.Exists(path) ? path : string.Empty;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
      return string.Empty;
    }

    public string GetRetailModeNviFileFromVpl(string vplFilePath)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplFilePath))
        {
          using (XmlReader reader = XmlReader.Create(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(reader);
            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("File"))
            {
              string empty = string.Empty;
              string innerText;
              try
              {
                innerText = xmlNode.SelectSingleNode("FileSubType").InnerText;
              }
              catch (Exception ex)
              {
                continue;
              }
              if (string.Compare(innerText, "RetailModeNvItems", StringComparison.OrdinalIgnoreCase) == 0)
              {
                string path = Path.Combine(Path.GetDirectoryName(vplFilePath), xmlNode.SelectSingleNode("Name").InnerText);
                return File.Exists(path) ? path : string.Empty;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return string.Empty;
      }
      return string.Empty;
    }

    public bool CheckIfAllMandatoryFilesAreOnDisc(string vplPathFile)
    {
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplPathFile))
        {
          using (XmlReader reader = XmlReader.Create(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(reader);
            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("File"))
            {
              if (string.Compare(xmlNode.SelectSingleNode("Optional").InnerText, "false", StringComparison.OrdinalIgnoreCase) == 0 && !File.Exists(Path.Combine(Path.GetDirectoryName(vplPathFile), xmlNode.SelectSingleNode("Name").InnerText)))
                return false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return false;
      }
      return !(this.GetVariantGroupFromVpl(vplPathFile).ToLowerInvariant() == "enosw");
    }

    public IList<string> GetFileList(string vplPathFile)
    {
      List<string> stringList = new List<string>();
      try
      {
        using (Stream fileStream = this.fileStreamHelper.CreateFileStream(vplPathFile))
        {
          using (XmlReader reader = XmlReader.Create(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = (XmlResolver) null;
            xmlDocument.Load(reader);
            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("File"))
            {
              string path = Path.Combine(Path.GetDirectoryName(vplPathFile), xmlNode.SelectSingleNode("Name").InnerText);
              if (File.Exists(path))
                stringList.Add(path);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex.Message);
        return (IList<string>) null;
      }
      return (IList<string>) stringList;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private byte[] GetExpectedFileHash(XmlNode node)
    {
      try
      {
        string innerText = node.SelectSingleNode("Crc").InnerText;
        if (string.IsNullOrEmpty(innerText))
          return (byte[]) null;
        byte[] numArray = SoapHexBinary.Parse(innerText).Value;
        if (BitConverter.IsLittleEndian)
          Array.Reverse((Array) numArray, 0, numArray.Length);
        return numArray;
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }
  }
}
