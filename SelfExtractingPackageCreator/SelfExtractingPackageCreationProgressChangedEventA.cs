// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SelfExtractingPackageCreationProgressChangedEventArgs
// Assembly: SelfExtractingPackageCreator, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 15D44AF4-CB22-48AD-A5ED-B49E916EE3E9
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SelfExtractingPackageCreator.dll

using System;

namespace Microsoft.LsuPro
{
  public class SelfExtractingPackageCreationProgressChangedEventArgs : EventArgs
  {
    public SelfExtractingPackageCreationProgressChangedEventArgs(string message, int percentage)
    {
      this.ProgressMessage = message;
      this.ProgressPercentage = percentage;
    }

    public string ProgressMessage { get; set; }

    public int ProgressPercentage { get; set; }
  }
}
