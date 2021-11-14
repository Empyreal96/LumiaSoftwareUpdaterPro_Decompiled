// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.LoggingModeConstant
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using System;

namespace Microsoft.Windows.ImageTools
{
  [Flags]
  internal enum LoggingModeConstant : uint
  {
    PrivateLoggerMode = 2048, // 0x00000800
    PrivateInProc = 131072, // 0x00020000
  }
}
