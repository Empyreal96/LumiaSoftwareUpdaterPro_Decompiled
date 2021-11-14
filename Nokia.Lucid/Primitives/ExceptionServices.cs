// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Primitives.ExceptionServices
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Threading;

namespace Nokia.Lucid.Primitives
{
  internal static class ExceptionServices
  {
    public static bool IsCriticalException(Exception exception)
    {
      switch (exception)
      {
        case ThreadAbortException _:
        case StackOverflowException _:
          return true;
        default:
          return exception is OutOfMemoryException;
      }
    }
  }
}
