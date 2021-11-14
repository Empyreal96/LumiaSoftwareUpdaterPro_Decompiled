// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Cryptography.CalculationProgressEventArgs
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Globalization;

namespace Microsoft.LsuPro.Cryptography
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
