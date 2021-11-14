// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.VersionComparer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro
{
  public class VersionComparer : IComparer<string>
  {
    public int Compare(string x, string y)
    {
      switch (VersionComparer.CompareVersions(x, y))
      {
        case VersionComparerResult.FirstIsGreater:
          return 1;
        case VersionComparerResult.SecondIsGreater:
          return -1;
        case VersionComparerResult.NumbersAreEqual:
          return 0;
        default:
          return 0;
      }
    }

    public static VersionComparerResult CompareVersions(string x, string y)
    {
      if (!string.IsNullOrWhiteSpace(x) && string.IsNullOrWhiteSpace(y))
        return VersionComparerResult.FirstIsGreater;
      if (string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
        return VersionComparerResult.SecondIsGreater;
      if (string.IsNullOrWhiteSpace(x) && string.IsNullOrWhiteSpace(y))
        return VersionComparerResult.NumbersAreEqual;
      x = x.Trim();
      y = y.Trim();
      uint[] versionParts1 = new uint[0];
      int num1 = VersionComparer.TrySplitVersionString(x, out versionParts1);
      if (-1 == num1)
        return VersionComparerResult.UnableToCompare;
      uint[] versionParts2 = new uint[0];
      int num2 = VersionComparer.TrySplitVersionString(y, out versionParts2);
      if (-1 == num2)
        return VersionComparerResult.UnableToCompare;
      int num3 = num1;
      if (num1 > num2)
        num3 = num2;
      for (int index = 0; index < num3; ++index)
      {
        if (versionParts1[index] > versionParts2[index])
          return VersionComparerResult.FirstIsGreater;
        if (versionParts1[index] < versionParts2[index])
          return VersionComparerResult.SecondIsGreater;
      }
      if (num1 == num2)
        return VersionComparerResult.NumbersAreEqual;
      return num1 <= num2 ? VersionComparerResult.SecondIsGreater : VersionComparerResult.FirstIsGreater;
    }

    private static int TrySplitVersionString(string version, out uint[] versionParts)
    {
      string[] strArray = version.Split('.');
      int length = strArray.Length;
      versionParts = new uint[length];
      for (int index = 0; index < length; ++index)
      {
        if (!uint.TryParse(strArray[index], out versionParts[index]))
          return -1;
      }
      return length;
    }
  }
}
