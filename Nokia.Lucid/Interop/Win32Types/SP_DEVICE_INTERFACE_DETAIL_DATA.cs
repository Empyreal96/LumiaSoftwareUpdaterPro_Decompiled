// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.Win32Types.SP_DEVICE_INTERFACE_DETAIL_DATA
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System.Runtime.InteropServices;

namespace Nokia.Lucid.Interop.Win32Types
{
  [BestFitMapping(false, ThrowOnUnmappableChar = true)]
  [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
  internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
  {
    public int cbSize;
    private char DevicePath;
  }
}
