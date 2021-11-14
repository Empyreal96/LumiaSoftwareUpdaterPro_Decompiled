// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FileSystemSecurity
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.LsuPro
{
  public static class FileSystemSecurity
  {
    private static FileSystemAccessRule everyoneGroupAccessRule;

    public static void AddEveryoneGroupToDirectory(string directoryName)
    {
      try
      {
        DirectorySecurity accessControl = Directory.GetAccessControl(directoryName);
        accessControl.AddAccessRule(FileSystemSecurity.GetEveryoneGroupAccessRule());
        Directory.SetAccessControl(directoryName, accessControl);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Error modifying directory access rights", objArray);
      }
    }

    public static void AddEveryoneGroupToFile(string fileName)
    {
      try
      {
        FileSecurity accessControl = File.GetAccessControl(fileName);
        accessControl.AddAccessRule(FileSystemSecurity.GetEveryoneGroupAccessRule());
        File.SetAccessControl(fileName, accessControl);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Error modifying file access rights", objArray);
      }
    }

    private static FileSystemAccessRule GetEveryoneGroupAccessRule()
    {
      if (FileSystemSecurity.everyoneGroupAccessRule != null)
        return FileSystemSecurity.everyoneGroupAccessRule;
      FileSystemSecurity.everyoneGroupAccessRule = new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, (SecurityIdentifier) null).Translate(typeof (NTAccount)), FileSystemRights.FullControl, AccessControlType.Allow);
      return FileSystemSecurity.everyoneGroupAccessRule;
    }
  }
}
