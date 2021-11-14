// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.NotificationMessageWithCallback
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;

namespace GalaSoft.MvvmLight.Messaging
{
  public class NotificationMessageWithCallback : NotificationMessage
  {
    private readonly Delegate _callback;

    public NotificationMessageWithCallback(string notification, Delegate callback)
      : base(notification)
    {
      NotificationMessageWithCallback.CheckCallback(callback);
      this._callback = callback;
    }

    public NotificationMessageWithCallback(object sender, string notification, Delegate callback)
      : base(sender, notification)
    {
      NotificationMessageWithCallback.CheckCallback(callback);
      this._callback = callback;
    }

    public NotificationMessageWithCallback(
      object sender,
      object target,
      string notification,
      Delegate callback)
      : base(sender, target, notification)
    {
      NotificationMessageWithCallback.CheckCallback(callback);
      this._callback = callback;
    }

    public virtual object Execute(params object[] arguments) => this._callback.DynamicInvoke(arguments);

    private static void CheckCallback(Delegate callback)
    {
      if ((object) callback == null)
        throw new ArgumentNullException(nameof (callback), "Callback may not be null");
    }
  }
}
