// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyFileScanner
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.LsuPro
{
  public static class EmergencyFileScanner
  {
    public static Dictionary<string, AlphaCollinsEmergencyFlashFiles> GetAlphaCollinsEmergencyFlashFiles(
      string directory)
    {
      Dictionary<string, AlphaCollinsEmergencyFlashFiles> dictionary = new Dictionary<string, AlphaCollinsEmergencyFlashFiles>();
      string str1 = Path.Combine(directory, "Products");
      if (Directory.Exists(str1))
      {
        foreach (string str2 in EmergencyFileScanner.GetFilesForFlashType(str1, EmergencyFlashType.AlphaCollins))
        {
          EmergencyFileInfo emergencyFileInfo = new EmergencyFileInfo(str2);
          if (emergencyFileInfo.IsValid)
          {
            if (emergencyFileInfo.FileType == EmergencyFileType.AlphaCollinsHexFile)
            {
              if (dictionary.ContainsKey(emergencyFileInfo.ProductType))
              {
                dictionary[emergencyFileInfo.ProductType].AddHexFile(str2);
              }
              else
              {
                AlphaCollinsEmergencyFlashFiles emergencyFlashFiles = new AlphaCollinsEmergencyFlashFiles();
                emergencyFlashFiles.AddHexFile(str2);
                dictionary.Add(emergencyFileInfo.ProductType, emergencyFlashFiles);
              }
            }
            else if (emergencyFileInfo.FileType == EmergencyFileType.AlphaCollinsMbnFile)
            {
              if (dictionary.ContainsKey(emergencyFileInfo.ProductType))
              {
                dictionary[emergencyFileInfo.ProductType].AddMbnFile(str2);
              }
              else
              {
                AlphaCollinsEmergencyFlashFiles emergencyFlashFiles = new AlphaCollinsEmergencyFlashFiles();
                emergencyFlashFiles.AddMbnFile(str2);
                dictionary.Add(emergencyFileInfo.ProductType, emergencyFlashFiles);
              }
            }
          }
        }
      }
      return dictionary;
    }

    public static Dictionary<string, QuattroEmergencyFlashFiles> GetQuattroEmergencyFlashFiles(
      string directory)
    {
      Dictionary<string, QuattroEmergencyFlashFiles> dictionary = new Dictionary<string, QuattroEmergencyFlashFiles>();
      string str = Path.Combine(directory, "Products");
      if (Directory.Exists(str))
      {
        foreach (string path in EmergencyFileScanner.GetFilesForFlashType(str, EmergencyFlashType.Quattro))
        {
          EmergencyFileInfo emergencyFileInfo = new EmergencyFileInfo(path);
          if (emergencyFileInfo.IsValid)
          {
            if (emergencyFileInfo.FileType == EmergencyFileType.QuattroMbnFile || emergencyFileInfo.FileType == EmergencyFileType.QuattroEdeFile)
            {
              if (dictionary.ContainsKey(emergencyFileInfo.ProductType))
              {
                dictionary[emergencyFileInfo.ProductType].ProgrammerFile = emergencyFileInfo;
              }
              else
              {
                QuattroEmergencyFlashFiles emergencyFlashFiles = new QuattroEmergencyFlashFiles()
                {
                  ProgrammerFile = new EmergencyFileInfo(path)
                };
                dictionary.Add(emergencyFileInfo.ProductType, emergencyFlashFiles);
              }
            }
            else if (emergencyFileInfo.FileType == EmergencyFileType.QuattroEdFile || emergencyFileInfo.FileType == EmergencyFileType.QuattroEdpFile)
            {
              if (dictionary.ContainsKey(emergencyFileInfo.ProductType))
              {
                dictionary[emergencyFileInfo.ProductType].EdFile = emergencyFileInfo;
              }
              else
              {
                QuattroEmergencyFlashFiles emergencyFlashFiles = new QuattroEmergencyFlashFiles()
                {
                  EdFile = emergencyFileInfo
                };
                dictionary.Add(emergencyFileInfo.ProductType, emergencyFlashFiles);
              }
            }
          }
        }
      }
      return dictionary;
    }

    private static IEnumerable<string> GetFilesForFlashType(
      string packageDir,
      EmergencyFlashType flashType)
    {
      return (IEnumerable<string>) ((IEnumerable<string>) DirectoryHelper.GetFiles(packageDir, "*.*", SearchOption.AllDirectories)).Where<string>((Func<string, bool>) (file => EmergencyFileScanner.EmergencyFlashTypeBasedOnName(file) == flashType)).ToList<string>();
    }

    private static EmergencyFlashType EmergencyFlashTypeBasedOnName(string file)
    {
      switch (EmergencyFileInfo.GetFileTypeFromName(file))
      {
        case EmergencyFileType.AlphaCollinsHexFile:
        case EmergencyFileType.AlphaCollinsMbnFile:
          return EmergencyFlashType.AlphaCollins;
        case EmergencyFileType.QuattroMbnFile:
        case EmergencyFileType.QuattroEdFile:
        case EmergencyFileType.QuattroEdeFile:
        case EmergencyFileType.QuattroEdpFile:
          return EmergencyFlashType.Quattro;
        default:
          return EmergencyFlashType.Unknown;
      }
    }
  }
}
