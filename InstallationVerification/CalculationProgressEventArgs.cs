// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.CalculationProgressEventArgs
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class CalculationProgressEventArgs : EventArgs
  {
    public CalculationProgressEventArgs(int progressPercentage)
    {
      this.ProgressPercentage = progressPercentage <= 100 && progressPercentage >= 0 ? progressPercentage : throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Progress went beyond constraints ({0}%", (object) progressPercentage));
      this.Cancel = false;
    }

    public int ProgressPercentage { get; private set; }

    public bool Cancel { get; set; }
  }
}
