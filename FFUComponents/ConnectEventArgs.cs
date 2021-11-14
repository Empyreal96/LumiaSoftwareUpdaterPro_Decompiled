// Decompiled with JetBrains decompiler
// Type: FFUComponents.ConnectEventArgs
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;

namespace FFUComponents
{
  public class ConnectEventArgs : EventArgs
  {
    private IFFUDevice device;

    private ConnectEventArgs()
    {
    }

    public ConnectEventArgs(IFFUDevice device) => this.device = device;

    public IFFUDevice Device => this.device;
  }
}
