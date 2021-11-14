// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.DialogMessage
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Windows;

namespace GalaSoft.MvvmLight.Messaging
{
  public class DialogMessage : GenericMessage<string>
  {
    public DialogMessage(string content, Action<MessageBoxResult> callback)
      : base(content)
    {
      this.Callback = callback;
    }

    public DialogMessage(object sender, string content, Action<MessageBoxResult> callback)
      : base(sender, content)
    {
      this.Callback = callback;
    }

    public DialogMessage(
      object sender,
      object target,
      string content,
      Action<MessageBoxResult> callback)
      : base(sender, target, content)
    {
      this.Callback = callback;
    }

    public MessageBoxButton Button { get; set; }

    public Action<MessageBoxResult> Callback { get; private set; }

    public string Caption { get; set; }

    public MessageBoxResult DefaultResult { get; set; }

    public MessageBoxImage Icon { get; set; }

    public MessageBoxOptions Options { get; set; }

    public void ProcessCallback(MessageBoxResult result)
    {
      if (this.Callback == null)
        return;
      this.Callback(result);
    }
  }
}
