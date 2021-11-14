// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.TcpServer.MessageIoEventArgs
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.TcpServer
{
  public class MessageIoEventArgs : EventArgs
  {
    public MessageIoEventArgs(ref byte[] data) => this.Data = data;

    public byte[] Data { get; private set; }
  }
}
