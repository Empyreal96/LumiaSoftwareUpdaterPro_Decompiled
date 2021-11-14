// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.GenericMessage`1
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

namespace GalaSoft.MvvmLight.Messaging
{
  public class GenericMessage<T> : MessageBase
  {
    public GenericMessage(T content) => this.Content = content;

    public GenericMessage(object sender, T content)
      : base(sender)
    {
      this.Content = content;
    }

    public GenericMessage(object sender, object target, T content)
      : base(sender, target)
    {
      this.Content = content;
    }

    public T Content { get; protected set; }
  }
}
