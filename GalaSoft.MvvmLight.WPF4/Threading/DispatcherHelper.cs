// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Threading.DispatcherHelper
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Windows.Threading;

namespace GalaSoft.MvvmLight.Threading
{
  public static class DispatcherHelper
  {
    public static Dispatcher UIDispatcher { get; private set; }

    public static void CheckBeginInvokeOnUI(Action action)
    {
      if (DispatcherHelper.UIDispatcher.CheckAccess())
        action();
      else
        DispatcherHelper.UIDispatcher.BeginInvoke((Delegate) action);
    }

    public static void InvokeAsync(Action action) => DispatcherHelper.UIDispatcher.BeginInvoke((Delegate) action);

    public static void Initialize()
    {
      if (DispatcherHelper.UIDispatcher != null && DispatcherHelper.UIDispatcher.Thread.IsAlive)
        return;
      DispatcherHelper.UIDispatcher = Dispatcher.CurrentDispatcher;
    }
  }
}
