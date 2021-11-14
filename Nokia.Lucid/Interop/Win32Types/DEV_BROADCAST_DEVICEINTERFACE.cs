// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.Win32Types.DEV_BROADCAST_DEVICEINTERFACE
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.Interop.Win32Types
{
  [BestFitMapping(false, ThrowOnUnmappableChar = true)]
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  internal struct DEV_BROADCAST_DEVICEINTERFACE
  {
    private const int MAX_PATH = 260;
    public int dbcc_size;
    public int dbcc_devicetype;
    public int dbcc_reserved;
    public Guid dbcc_classguid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string dbcc_name;
  }
}
