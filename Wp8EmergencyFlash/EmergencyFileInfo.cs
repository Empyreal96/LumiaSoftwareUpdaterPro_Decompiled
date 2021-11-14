// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.EmergencyFileInfo
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

using System;
using System.IO;

namespace Microsoft.LsuPro
{
  public class EmergencyFileInfo
  {
    public static EmergencyFileType GetFileTypeFromName(string file)
    {
      string extension = Path.GetExtension(file);
      if (extension == null)
        return EmergencyFileType.Ignored;
      string lowerInvariant = extension.ToLowerInvariant();
      if (lowerInvariant == ".hex")
        return EmergencyFileType.AlphaCollinsHexFile;
      if (lowerInvariant == ".mbn")
        return EmergencyFileInfo.DetermineMbnType(file);
      if (lowerInvariant == ".ed")
        return EmergencyFileType.QuattroEdFile;
      if (lowerInvariant == ".ede")
        return EmergencyFileType.QuattroEdeFile;
      return lowerInvariant == ".edp" ? EmergencyFileType.QuattroEdpFile : EmergencyFileType.Ignored;
    }

    public EmergencyFileInfo(string path)
    {
      this.FileInfo = new FileInfo(path);
      this.FileType = EmergencyFileInfo.GetFileTypeFromName(path);
      this.ProductType = EmergencyFileInfo.GetTypeDesignatorFromPath(path);
      if (this.FileType == EmergencyFileType.AlphaCollinsHexFile || this.FileType == EmergencyFileType.AlphaCollinsMbnFile)
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(path);
        this.Version = withoutExtension != null ? withoutExtension.Substring(withoutExtension.LastIndexOf("_v", StringComparison.OrdinalIgnoreCase) + 2) : string.Empty;
      }
      else
        this.Version = "0.0";
    }

    public bool IsValid
    {
      get
      {
        bool flag = false;
        switch (this.FileType)
        {
          case EmergencyFileType.AlphaCollinsHexFile:
          case EmergencyFileType.AlphaCollinsMbnFile:
            flag = EmergencyFileInfo.DirectoryIsAlphaCollinsEmergencyDir(this.Directory);
            break;
          case EmergencyFileType.QuattroMbnFile:
          case EmergencyFileType.QuattroEdFile:
          case EmergencyFileType.QuattroEdeFile:
          case EmergencyFileType.QuattroEdpFile:
            flag = EmergencyFileInfo.DirectoryIsQuattroEmergencyDir(this.Directory);
            break;
        }
        return ((!this.Exists ? 0 : (!string.IsNullOrEmpty(this.ProductType) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    public bool Exists => this.FileInfo.Exists;

    public string Name => this.FileInfo.Name;

    public string FullPathAndName => this.FileInfo.FullName;

    public string Directory => this.FileInfo.DirectoryName;

    public string ProductType { get; private set; }

    public string Version { get; private set; }

    public EmergencyFileType FileType { get; private set; }

    internal FileInfo FileInfo { get; private set; }

    private static EmergencyFileType DetermineMbnType(string file)
    {
      string directoryName = Path.GetDirectoryName(file);
      if (EmergencyFileInfo.DirectoryIsAlphaCollinsEmergencyDir(directoryName))
        return EmergencyFileType.AlphaCollinsMbnFile;
      return EmergencyFileInfo.DirectoryIsQuattroEmergencyDir(directoryName) ? EmergencyFileType.QuattroMbnFile : EmergencyFileType.Ignored;
    }

    private static bool DirectoryIsAlphaCollinsEmergencyDir(string dir) => DirectoryHelper.DirectoryExist(dir) && (uint) DirectoryHelper.GetFiles(dir, "*.hex", SearchOption.TopDirectoryOnly).Length > 0U;

    private static bool DirectoryIsQuattroEmergencyDir(string dir) => (uint) DirectoryHelper.GetFiles(dir, "*.ede", SearchOption.TopDirectoryOnly).Length > 0U;

    private static string GetTypeDesignatorFromPath(string path)
    {
      string fileName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path)));
      if (fileName != null)
      {
        string upperInvariant = fileName.ToUpperInvariant();
        if (upperInvariant.Contains("-"))
          return upperInvariant;
      }
      return string.Empty;
    }
  }
}
