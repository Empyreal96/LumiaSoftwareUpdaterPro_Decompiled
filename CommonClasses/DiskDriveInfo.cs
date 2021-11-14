// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DiskDriveInfo
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.LsuPro
{
  public static class DiskDriveInfo
  {
    public static int GetTotalFreeSpaceMb(string directory) => (int) ((double) DiskDriveInfo.GetTotalFreeSpace(directory) / 1024.0 / 1024.0);

    public static long GetTotalFreeSpace(string directory)
    {
      DriveInfo driveInfo = DiskDriveInfo.GetDriveInfo(directory);
      if (driveInfo != null)
        return driveInfo.TotalFreeSpace;
      ulong lpFreeBytesAvailable;
      return !DiskDriveInfo.GetDiskFreeSpaceEx(directory, out lpFreeBytesAvailable, out ulong _, out ulong _) ? -1L : (long) lpFreeBytesAvailable;
    }

    public static int GetNumberOfMegabytesInDirectory(string directory) => (int) ((double) DiskDriveInfo.GetNumberOfBytesInDirectory(directory) / 1024.0 / 1024.0);

    public static long GetNumberOfBytesInDirectory(string directory)
    {
      long num = 0;
      try
      {
        FileInfo[] accessibleSubdirs = DirectoryHelper.GetFileInfosFromAllAccessibleSubdirs(directory, "*");
        num += ((IEnumerable<FileInfo>) accessibleSubdirs).Select<FileInfo, FileInfoHelper>((Func<FileInfo, FileInfoHelper>) (fileInfo => new FileInfoHelper(fileInfo.FullName))).Select<FileInfoHelper, long>((Func<FileInfoHelper, long>) (tmp => tmp.Length)).Sum();
      }
      catch (Exception ex)
      {
        Tracer.Information("Cannot calculate folder size: {0}", (object) ex.Message);
      }
      return num;
    }

    public static bool DirectoryIsOnNetworkDrive(string directory)
    {
      DriveInfo driveInfo = DiskDriveInfo.GetDriveInfo(directory);
      return driveInfo == null || driveInfo.DriveType == DriveType.Network;
    }

    public static DriveInfo GetDriveInfo(string directory)
    {
      DriveInfo[] drives = DriveInfo.GetDrives();
      string rootDir = Path.GetPathRoot(directory);
      return rootDir == null ? (DriveInfo) null : ((IEnumerable<DriveInfo>) drives).FirstOrDefault<DriveInfo>((Func<DriveInfo, bool>) (drive => rootDir.Equals(drive.Name, StringComparison.OrdinalIgnoreCase)));
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetDiskFreeSpaceEx(
      string lpDirectoryName,
      out ulong lpFreeBytesAvailable,
      out ulong lpTotalNumberOfBytes,
      out ulong lpTotalNumberOfFreeBytes);
  }
}
