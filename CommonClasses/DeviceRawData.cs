// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DeviceRawData
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.LsuPro
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class DeviceRawData
  {
    public DeviceRawData(byte data, bool isEqual)
    {
      this.Data = data;
      this.IsEqual = isEqual;
    }

    public byte Data { get; private set; }

    public bool IsEqual { get; private set; }

    public override string ToString() => this.Data.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
