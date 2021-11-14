// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UsbHostControllerInfo
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "not needed")]
  public class UsbHostControllerInfo : IComparable<UsbHostControllerInfo>
  {
    public string Name { get; internal set; }

    public string DriverVersion { get; internal set; }

    [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", Justification = "it is OK", MessageId = "System.String.Compare(System.String,System.String,System.Globalization.CultureInfo,System.Globalization.CompareOptions)")]
    public int CompareTo(UsbHostControllerInfo other) => other != null ? string.Compare(this.Name, other.Name, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase) : 1;
  }
}
