// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.InstallationVerification
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;

namespace Microsoft.LsuPro
{
  public class InstallationVerification
  {
    private readonly Collection<FileEntry> fileEntries = new Collection<FileEntry>();
    private readonly Collection<DirectoryEntry> directoryEntries = new Collection<DirectoryEntry>();
    private readonly HashCalculator hashCalculator = new HashCalculator();
    private string installationSnapshotFile = "InstallationSnapshot.xml";

    public Collection<FileEntry> FileEntries => this.fileEntries;

    public Collection<DirectoryEntry> DirectoryEntries => this.directoryEntries;

    public void InstallationSnapshot(string installationScript, string applicationVersion)
    {
      this.CreateInstallationSnapshot(installationScript);
      this.WriteInstallationSnapshot(applicationVersion);
    }

    public int VerifyInstallation(string installationSnapshot)
    {
      this.ReadInstallationSnapshot(installationSnapshot);
      return this.CompareInstallationWithSnapshot();
    }

    private int CompareInstallationWithSnapshot()
    {
      bool flag1 = false;
      int num = 0;
      string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      foreach (DirectoryEntry directoryEntry in this.DirectoryEntries)
      {
        if (!Directory.Exists(Path.Combine(folderPath, directoryEntry.Name)))
        {
          flag1 = true;
          num = 1;
          Trace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Directory not exist: {0}", (object) Path.Combine(folderPath, directoryEntry.Name)));
        }
        else if (directoryEntry.GenericAll.ToLowerInvariant() == "yes" && directoryEntry.User.ToLowerInvariant() == "everyone")
        {
          DirectorySecurity accessControl = new DirectoryInfo(Path.Combine(folderPath, directoryEntry.Name)).GetAccessControl(AccessControlSections.Access);
          AuthorizationRuleCollection accessRules = accessControl.GetAccessRules(true, true, typeof (SecurityIdentifier));
          bool flag2 = false;
          IEnumerator enumerator = accessRules.GetEnumerator();
          try
          {
            if (enumerator.MoveNext())
            {
              FileSystemAccessRule current = (FileSystemAccessRule) enumerator.Current;
              if (current.AccessControlType == AccessControlType.Allow)
              {
                if ((current.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData)
                  flag2 = true;
              }
            }
          }
          finally
          {
            if (enumerator is IDisposable disposable6)
              disposable6.Dispose();
          }
          foreach (AccessRule accessRule in (ReadOnlyCollectionBase) accessControl.GetAccessRules(true, true, typeof (NTAccount)))
          {
            if (accessRule.AccessControlType == AccessControlType.Deny)
            {
              flag2 = false;
              break;
            }
          }
          if (!flag2)
          {
            flag1 = true;
            num = 1;
            Trace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User has no proper rights to directory: {0}", (object) Path.Combine(folderPath, directoryEntry.Name)));
          }
        }
      }
      if (!flag1)
        Trace.Info("Directory structure is OK.");
      bool flag3 = false;
      foreach (FileEntry fileEntry in this.FileEntries)
      {
        if (!File.Exists(Path.Combine(folderPath, fileEntry.Name)))
        {
          flag3 = true;
          num = 1;
          Trace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File not exist: {0}", (object) Path.Combine(folderPath, fileEntry.Name)));
        }
        else if (!fileEntry.Name.Contains("InstallationSnapshot.xml") && HashCalculator.Crc32BytesToString(this.hashCalculator.CalculateCrc32(Path.Combine(folderPath, fileEntry.Name))) != fileEntry.Crc)
        {
          flag3 = true;
          num = 1;
          Trace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Wrong file CRC: {0}", (object) Path.Combine(folderPath, fileEntry.Name)));
        }
      }
      if (!flag3)
        Trace.Info("All files are OK.");
      return num;
    }

    private void ReadInstallationSnapshot(string installationSnapshot)
    {
      this.fileEntries.Clear();
      this.directoryEntries.Clear();
      if (!string.IsNullOrEmpty(installationSnapshot))
        this.installationSnapshotFile = installationSnapshot;
      using (Stream input = (Stream) new FileStream(this.installationSnapshotFile, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (XmlReader reader = XmlReader.Create(input))
        {
          XmlDocument xmlDocument = new XmlDocument()
          {
            XmlResolver = (XmlResolver) null
          };
          xmlDocument.Load(reader);
          foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("Directory"))
          {
            if (xmlNode.Attributes != null)
            {
              string empty1 = string.Empty;
              string empty2 = string.Empty;
              string empty3 = string.Empty;
              foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
              {
                if (attribute.Name == "Name")
                  empty1 = attribute.Value;
                if (attribute.Name == "User")
                  empty2 = attribute.Value;
                if (attribute.Name == "GenericAll")
                  empty3 = attribute.Value;
              }
              this.directoryEntries.Add(new DirectoryEntry(empty1, empty2, empty3));
            }
          }
          foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("File"))
          {
            if (xmlNode.Attributes != null)
            {
              string empty1 = string.Empty;
              string empty2 = string.Empty;
              foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
              {
                if (attribute.Name == "Name")
                  empty1 = attribute.Value;
                if (attribute.Name == "Crc")
                  empty2 = attribute.Value;
              }
              this.fileEntries.Add(new FileEntry(empty1, empty2));
            }
          }
        }
      }
    }

    private void CreateInstallationSnapshot(string installationScript)
    {
      this.fileEntries.Clear();
      this.directoryEntries.Clear();
      this.directoryEntries.Add(new DirectoryEntry("Microsoft", "Everyone", "yes"));
      using (Stream input = (Stream) new FileStream(installationScript, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (XmlReader reader = XmlReader.Create(input))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.XmlResolver = (XmlResolver) null;
          xmlDocument.Load(reader);
          foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("CreateFolder"))
          {
            if (xmlNode.HasChildNodes)
            {
              string empty1 = string.Empty;
              string empty2 = string.Empty;
              string name = string.Empty;
              if (xmlNode.FirstChild.Name == "util:PermissionEx" && xmlNode.FirstChild.Attributes != null)
              {
                foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.FirstChild.Attributes)
                {
                  if (attribute.Name == "User")
                    empty1 = attribute.Value;
                  if (attribute.Name == "GenericAll")
                    empty2 = attribute.Value;
                }
                if (xmlNode.ParentNode != null && xmlNode.ParentNode.Name == "Component")
                  name = this.GetParentDirectory(xmlNode.ParentNode.ParentNode);
                this.directoryEntries.Add(new DirectoryEntry(name, empty1, empty2));
              }
            }
          }
        }
      }
      using (Stream input = (Stream) new FileStream(installationScript, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (XmlReader reader = XmlReader.Create(input))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.XmlResolver = (XmlResolver) null;
          xmlDocument.Load(reader);
          foreach (XmlNode inputXmlNode in xmlDocument.GetElementsByTagName("Directory"))
          {
            if (inputXmlNode.Attributes != null)
            {
              foreach (XmlAttribute attribute in (XmlNamedNodeMap) inputXmlNode.Attributes)
              {
                this.AddFilesFromDirectoryKey(attribute, "Name", "Bin", inputXmlNode);
                this.AddFilesFromDirectoryKey(attribute, "Name", "Nvi", inputXmlNode);
              }
            }
          }
        }
      }
    }

    private string GetParentDirectory(XmlNode xmlNode)
    {
      string empty = string.Empty;
      if (xmlNode.Name == "Directory")
      {
        if (xmlNode.Attributes != null)
        {
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
          {
            if (attribute.Name == "Name")
              empty = attribute.Value;
          }
        }
        if (xmlNode.ParentNode != null && xmlNode.ParentNode.Name == "Directory" && !(empty == "Microsoft"))
          return Path.Combine(this.GetParentDirectory(xmlNode.ParentNode), empty);
      }
      return empty;
    }

    private void WriteInstallationSnapshot(string applicationVersion)
    {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.ConformanceLevel = ConformanceLevel.Fragment;
      using (Stream output = (Stream) new FileStream(this.installationSnapshotFile, FileMode.Create, FileAccess.Write, FileShare.Write))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
        {
          xmlWriter.WriteStartElement(nameof (InstallationVerification));
          xmlWriter.WriteStartElement("LsuProVersion");
          xmlWriter.WriteAttributeString("Version", applicationVersion);
          xmlWriter.WriteEndElement();
          xmlWriter.WriteStartElement("Directories");
          foreach (DirectoryEntry directoryEntry in this.DirectoryEntries)
          {
            xmlWriter.WriteStartElement("Directory");
            xmlWriter.WriteAttributeString("Name", directoryEntry.Name);
            xmlWriter.WriteAttributeString("User", directoryEntry.User);
            xmlWriter.WriteAttributeString("GenericAll", directoryEntry.GenericAll);
            xmlWriter.WriteEndElement();
          }
          xmlWriter.WriteEndElement();
          xmlWriter.WriteStartElement("Files");
          foreach (FileEntry fileEntry in this.FileEntries)
          {
            xmlWriter.WriteStartElement("File");
            xmlWriter.WriteAttributeString("Name", fileEntry.Name);
            xmlWriter.WriteAttributeString("Crc", fileEntry.Crc);
            xmlWriter.WriteEndElement();
          }
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndElement();
        }
      }
    }

    private void AddFilesFromDirectoryKey(
      XmlAttribute attribute,
      string attributeName,
      string attributeValue,
      XmlNode inputXmlNode)
    {
      if (!(attribute.Name == attributeName) || !(attribute.Value == attributeValue))
        return;
      foreach (XmlNode childNode1 in inputXmlNode.ChildNodes)
      {
        string str = Path.Combine("Microsoft\\Lumia Software Updater Pro", attribute.Value);
        if (childNode1.Name == "Directory")
        {
          string parentDirectory = Path.Combine(str, this.GetDirectoryNameFromAttribute(childNode1));
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.Name == "Directory")
            {
              parentDirectory = this.GetParentDirectory(childNode2);
              foreach (XmlNode childNode3 in childNode2.ChildNodes)
              {
                if (childNode3.Name == "Component")
                  this.AddFilesFromComponentKey(childNode3, parentDirectory);
              }
            }
            else if (childNode2.Name == "Component")
            {
              foreach (XmlNode childNode3 in childNode2.ChildNodes)
              {
                if (childNode3.Name == "Component")
                  this.AddFilesFromComponentKey(childNode3, parentDirectory);
                else if (childNode3.Name == "File")
                  this.AddFileFromFileKey(childNode3, parentDirectory);
              }
            }
          }
        }
        else if (childNode1.Name == "Component")
          this.AddFilesFromComponentKey(childNode1, str);
      }
    }

    private void AddFilesFromComponentKey(XmlNode xmlNode, string parentDirectory)
    {
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "File")
          this.AddFileFromFileKey(childNode, parentDirectory);
      }
    }

    private void AddFileFromFileKey(XmlNode xmlNode, string parentDirectory)
    {
      if (xmlNode.Attributes == null)
        return;
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
      {
        if (attribute.Name == "Name")
        {
          string fileFromAttribute = this.GetSourceFileFromAttribute(xmlNode);
          byte[] hash = (byte[]) null;
          if (!fileFromAttribute.Contains("InstallationSnapshot.xml"))
            hash = this.hashCalculator.CalculateCrc32(fileFromAttribute);
          this.fileEntries.Add(new FileEntry(Path.Combine(parentDirectory, attribute.Value), hash != null ? HashCalculator.Crc32BytesToString(hash) : string.Empty));
        }
      }
    }

    private string GetSourceFileFromAttribute(XmlNode xmlNode)
    {
      string empty = string.Empty;
      if (xmlNode.Attributes != null)
      {
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
        {
          if (attribute.Name == "Source")
          {
            empty = attribute.Value;
            break;
          }
        }
      }
      return empty;
    }

    private string GetDirectoryNameFromAttribute(XmlNode xmlNode)
    {
      string empty = string.Empty;
      if (xmlNode.Attributes != null)
      {
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
        {
          if (attribute.Name == "Name")
          {
            empty = attribute.Value;
            break;
          }
        }
      }
      return empty;
    }
  }
}
