// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.NotificationMessage`1
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

namespace GalaSoft.MvvmLight.Messaging
{
  public class NotificationMessage<T> : GenericMessage<T>
  {
    public NotificationMessage(T content, string notification)
      : base(content)
    {
      this.Notification = notification;
    }

    public NotificationMessage(object sender, T content, string notification)
      : base(sender, content)
    {
      this.Notification = notification;
    }

    public NotificationMessage(object sender, object target, T content, string notification)
      : base(sender, target, content)
    {
      this.Notification = notification;
    }

    public string Notification { get; private set; }
  }
}
