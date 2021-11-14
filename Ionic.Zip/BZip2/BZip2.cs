// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.BZip2
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

namespace Ionic.BZip2
{
  internal static class BZip2
  {
    public static readonly int BlockSizeMultiple = 100000;
    public static readonly int MinBlockSize = 1;
    public static readonly int MaxBlockSize = 9;
    public static readonly int MaxAlphaSize = 258;
    public static readonly int MaxCodeLength = 23;
    public static readonly char RUNA = char.MinValue;
    public static readonly char RUNB = '\u0001';
    public static readonly int NGroups = 6;
    public static readonly int G_SIZE = 50;
    public static readonly int N_ITERS = 4;
    public static readonly int MaxSelectors = 2 + 900000 / Ionic.BZip2.BZip2.G_SIZE;
    public static readonly int NUM_OVERSHOOT_BYTES = 20;
    internal static readonly int QSORT_STACK_SIZE = 1000;

    internal static T[][] InitRectangularArray<T>(int d1, int d2)
    {
      T[][] objArray = new T[d1][];
      for (int index = 0; index < d1; ++index)
        objArray[index] = new T[d2];
      return objArray;
    }
  }
}
