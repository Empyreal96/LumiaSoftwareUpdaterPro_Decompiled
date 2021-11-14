// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.FlashParam
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using FFUComponents;
using System.Threading;

namespace Microsoft.Windows.ImageTools
{
  internal class FlashParam
  {
    public IFFUDevice Device;
    public string FfuFilePath;
    public string WimPath;
    public AutoResetEvent WaitHandle;
    public int Result;
  }
}
