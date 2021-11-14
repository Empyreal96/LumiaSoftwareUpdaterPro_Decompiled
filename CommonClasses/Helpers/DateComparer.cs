// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.DateComparer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.LsuPro.Helpers
{
  public class DateComparer : IComparer<string>
  {
    public int Compare(string x, string y) => DateTime.ParseExact(PathHelper.GetFileNameWithoutExtension(x).Remove(13), "yyyyMMdd_HHmm", (IFormatProvider) CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(PathHelper.GetFileNameWithoutExtension(y).Remove(13), "yyyyMMdd_HHmm", (IFormatProvider) CultureInfo.InvariantCulture));
  }
}
