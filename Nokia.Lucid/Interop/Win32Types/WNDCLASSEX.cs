// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Interop.Win32Types.WNDCLASSEX
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.Interop.Win32Types
{
  [BestFitMapping(false, ThrowOnUnmappableChar = true)]
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  internal struct WNDCLASSEX
  {
    public int cbSize;
    public int style;
    public WNDPROC lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    public IntPtr lpszMenuName;
    public string lpszClassName;
    public IntPtr hIconSm;
  }
}
