// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.Primitives.ThreadAffinity
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Properties;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Nokia.Lucid.DeviceDetection.Primitives
{
  internal static class ThreadAffinity
  {
    [ThreadStatic]
    private static int level;

    public static bool IsThreadAffine => ThreadAffinity.level > 0;

    internal static int Level => ThreadAffinity.level;

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    public static void BeginThreadAffinity(ref bool success)
    {
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        if (ThreadAffinity.level == 0)
          Thread.BeginThreadAffinity();
        ++ThreadAffinity.level;
        success = true;
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    public static void EndThreadAffinity()
    {
      if (ThreadAffinity.level <= 0)
        throw new InvalidOperationException(Resources.InvalidOperationException_MessageText_CouldNotEndThreadAffinity);
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        if (ThreadAffinity.level == 1)
          Thread.EndThreadAffinity();
        --ThreadAffinity.level;
      }
    }
  }
}
