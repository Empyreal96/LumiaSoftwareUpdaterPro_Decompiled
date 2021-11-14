// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.IMessenger
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;

namespace GalaSoft.MvvmLight.Messaging
{
  public interface IMessenger
  {
    void Register<TMessage>(object recipient, Action<TMessage> action);

    void Register<TMessage>(object recipient, object token, Action<TMessage> action);

    void Register<TMessage>(
      object recipient,
      object token,
      bool receiveDerivedMessagesToo,
      Action<TMessage> action);

    void Register<TMessage>(
      object recipient,
      bool receiveDerivedMessagesToo,
      Action<TMessage> action);

    void Send<TMessage>(TMessage message);

    void Send<TMessage, TTarget>(TMessage message);

    void Send<TMessage>(TMessage message, object token);

    void Unregister(object recipient);

    void Unregister<TMessage>(object recipient);

    void Unregister<TMessage>(object recipient, object token);

    void Unregister<TMessage>(object recipient, Action<TMessage> action);

    void Unregister<TMessage>(object recipient, object token, Action<TMessage> action);
  }
}
