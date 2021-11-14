// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UnfinishedDownloadsEventArgs
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro
{
  public class UnfinishedDownloadsEventArgs : EventArgs
  {
    public UnfinishedDownloadsEventArgs(UnfinishedDownloadsInfo unfinishedDownloadsInfo) => this.UnfinishedDownloads = unfinishedDownloadsInfo;

    public UnfinishedDownloadsInfo UnfinishedDownloads { get; private set; }
  }
}
