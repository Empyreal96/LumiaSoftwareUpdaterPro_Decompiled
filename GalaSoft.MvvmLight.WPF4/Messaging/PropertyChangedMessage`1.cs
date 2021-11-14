// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

namespace GalaSoft.MvvmLight.Messaging
{
  public class PropertyChangedMessage<T> : PropertyChangedMessageBase
  {
    public PropertyChangedMessage(object sender, T oldValue, T newValue, string propertyName)
      : base(sender, propertyName)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public PropertyChangedMessage(T oldValue, T newValue, string propertyName)
      : base(propertyName)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public PropertyChangedMessage(
      object sender,
      object target,
      T oldValue,
      T newValue,
      string propertyName)
      : base(sender, target, propertyName)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public T NewValue { get; private set; }

    public T OldValue { get; private set; }
  }
}
