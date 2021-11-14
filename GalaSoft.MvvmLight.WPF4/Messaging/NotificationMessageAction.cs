// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.NotificationMessageAction
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;

namespace GalaSoft.MvvmLight.Messaging
{
  public class NotificationMessageAction : NotificationMessageWithCallback
  {
    public NotificationMessageAction(string notification, Action callback)
      : base(notification, (Delegate) callback)
    {
    }

    public NotificationMessageAction(object sender, string notification, Action callback)
      : base(sender, notification, (Delegate) callback)
    {
    }

    public NotificationMessageAction(
      object sender,
      object target,
      string notification,
      Action callback)
      : base(sender, target, notification, (Delegate) callback)
    {
    }

    public void Execute() => this.Execute();
  }
}
