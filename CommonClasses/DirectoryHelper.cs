// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DirectoryHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.LsuPro
{
  public static class DirectoryHelper
  {
    public static string[] GetFiles(string directory, string searchPattern, SearchOption option) => Directory.GetFiles(directory, searchPattern, option);

    public static string[] GetFiles(string directory) => Directory.GetFiles(directory);

    public static string[] GetDirectories(
      string directory,
      string searchPattern,
      SearchOption option)
    {
      return Directory.GetDirectories(directory, searchPattern, option);
    }

    public static string[] GetDirectories(string directory, string searchPattern) => Directory.GetDirectories(directory, searchPattern);

    public static string[] GetDirectories(string directory) => Directory.GetDirectories(directory);

    public static bool DirectoryExist(string path) => Directory.Exists(path);

    public static void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public static void Delete(string path, bool recursive) => Directory.Delete(path, recursive);

    public static FileInfo[] GetFileInfos(
      string directoryName,
      string searchPattern,
      SearchOption searchOption)
    {
      return new DirectoryInfo(directoryName).GetFiles(searchPattern, searchOption);
    }

    public static FileInfo[] GetFileInfosFromAllAccessibleSubdirs(
      string directoryName,
      string searchPattern)
    {
      DirectoryInfo root = new DirectoryInfo(directoryName);
      List<FileInfo> fileInfoList = new List<FileInfo>();
      string searchPattern1 = searchPattern;
      ref List<FileInfo> local = ref fileInfoList;
      DirectoryHelper.RecursiveDirectoryScan(root, searchPattern1, ref local);
      return fileInfoList.ToArray();
    }

    public static bool CanCreateFile(string directoryName)
    {
      try
      {
        if (!DirectoryHelper.DirectoryExist(directoryName))
          DirectoryHelper.CreateDirectory(directoryName);
        string path;
        do
        {
          path = Path.Combine(directoryName, Path.GetRandomFileName());
        }
        while (File.Exists(path));
        using (File.Open(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
          ;
        try
        {
          File.Delete(path);
        }
        catch (Exception ex)
        {
          Tracer.Warning("Cannot delete temporary file '{0}': {1}", (object) path, (object) ex.Message);
        }
      }
      catch (Exception ex)
      {
        Tracer.Error(ex, "Cannot create temporary file in directory '{0}': {1}", (object) directoryName, (object) ex.Message);
        return false;
      }
      return true;
    }

    private static void RecursiveDirectoryScan(
      DirectoryInfo root,
      string searchPattern,
      ref List<FileInfo> fileInfos)
    {
      FileInfo[] fileInfoArray = (FileInfo[]) null;
      try
      {
        fileInfoArray = root.GetFiles(searchPattern);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Failed to retrieve files from directory", objArray);
      }
      if (fileInfoArray == null)
        return;
      fileInfos.AddRange((IEnumerable<FileInfo>) fileInfoArray);
      foreach (DirectoryInfo directory in root.GetDirectories())
        DirectoryHelper.RecursiveDirectoryScan(directory, searchPattern, ref fileInfos);
    }
  }
}
