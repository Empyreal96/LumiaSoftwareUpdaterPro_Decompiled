// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.RobustTrace
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Primitives;
using System;

namespace Nokia.Lucid.Diagnostics
{
  internal static class RobustTrace
  {
    public static void Trace(Action trace)
    {
      try
      {
        trace();
      }
      catch (Exception ex)
      {
        if (!ExceptionServices.IsCriticalException(ex))
          return;
        throw;
      }
    }

    public static void Trace<TArg>(Action<TArg> trace, TArg arg)
    {
      try
      {
        trace(arg);
      }
      catch (Exception ex)
      {
        if (!ExceptionServices.IsCriticalException(ex))
          return;
        throw;
      }
    }

    public static void Trace<TArg0, TArg1>(Action<TArg0, TArg1> trace, TArg0 arg0, TArg1 arg1)
    {
      try
      {
        trace(arg0, arg1);
      }
      catch (Exception ex)
      {
        if (!ExceptionServices.IsCriticalException(ex))
          return;
        throw;
      }
    }

    public static void Trace<TArg0, TArg1, TArg2>(
      Action<TArg0, TArg1, TArg2> trace,
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2)
    {
      try
      {
        trace(arg0, arg1, arg2);
      }
      catch (Exception ex)
      {
        if (!ExceptionServices.IsCriticalException(ex))
          return;
        throw;
      }
    }

    public static void Trace<TArg0, TArg1, TArg2, TArg3>(
      Action<TArg0, TArg1, TArg2, TArg3> trace,
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3)
    {
      try
      {
        trace(arg0, arg1, arg2, arg3);
      }
      catch (Exception ex)
      {
        if (!ExceptionServices.IsCriticalException(ex))
          return;
        throw;
      }
    }

    public static void Trace<TArg0, TArg1, TArg2, TArg3, TArg4>(
      Action<TArg0, TArg1, TArg2, TArg3, TArg4> trace,
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4)
    {
      try
      {
        trace(arg0, arg1, arg2, arg3, arg4);
      }
      catch (Exception ex)
      {
        if (!ExceptionServices.IsCriticalException(ex))
          return;
        throw;
      }
    }
  }
}
