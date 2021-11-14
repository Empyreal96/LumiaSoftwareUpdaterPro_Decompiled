// Decompiled with JetBrains decompiler
// Type: FFUComponents.FindFileData
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System.Runtime.InteropServices;

namespace FFUComponents
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct FindFileData
  {
    public uint dwFileAttributes;
    public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
    public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
    public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
    public uint FileSizeHigh;
    public uint FileSizeLow;
    public uint Reserved0;
    public uint Reserved1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string FileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    public string Alternate;
  }
}
