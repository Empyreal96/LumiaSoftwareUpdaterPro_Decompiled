// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Command.EventToCommand
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System;
using System.Windows;
using System.Windows.Input;

namespace GalaSoft.MvvmLight.Command
{
  public class EventToCommand : System.Windows.Interactivity.TriggerAction<DependencyObject>
  {
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof (CommandParameter), typeof (object), typeof (EventToCommand), new PropertyMetadata((object) null, (PropertyChangedCallback) ((s, e) =>
    {
      if (!(s is EventToCommand eventToCommand2) || eventToCommand2.AssociatedObject == null)
        return;
      eventToCommand2.EnableDisableElement();
    })));
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof (Command), typeof (ICommand), typeof (EventToCommand), new PropertyMetadata((object) null, (PropertyChangedCallback) ((s, e) => EventToCommand.OnCommandChanged(s as EventToCommand, e))));
    public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register(nameof (MustToggleIsEnabled), typeof (bool), typeof (EventToCommand), new PropertyMetadata((object) false, (PropertyChangedCallback) ((s, e) =>
    {
      if (!(s is EventToCommand eventToCommand2) || eventToCommand2.AssociatedObject == null)
        return;
      eventToCommand2.EnableDisableElement();
    })));
    private object _commandParameterValue;
    private bool? _mustToggleValue;

    public ICommand Command
    {
      get => (ICommand) this.GetValue(EventToCommand.CommandProperty);
      set => this.SetValue(EventToCommand.CommandProperty, (object) value);
    }

    public object CommandParameter
    {
      get => this.GetValue(EventToCommand.CommandParameterProperty);
      set => this.SetValue(EventToCommand.CommandParameterProperty, value);
    }

    public object CommandParameterValue
    {
      get => this._commandParameterValue ?? this.CommandParameter;
      set
      {
        this._commandParameterValue = value;
        this.EnableDisableElement();
      }
    }

    public bool MustToggleIsEnabled
    {
      get => (bool) this.GetValue(EventToCommand.MustToggleIsEnabledProperty);
      set => this.SetValue(EventToCommand.MustToggleIsEnabledProperty, (object) value);
    }

    public bool MustToggleIsEnabledValue
    {
      get => this._mustToggleValue.HasValue ? this._mustToggleValue.Value : this.MustToggleIsEnabled;
      set
      {
        this._mustToggleValue = new bool?(value);
        this.EnableDisableElement();
      }
    }

    protected override void OnAttached()
    {
      base.OnAttached();
      this.EnableDisableElement();
    }

    private FrameworkElement GetAssociatedObject() => this.AssociatedObject as FrameworkElement;

    private ICommand GetCommand() => this.Command;

    public bool PassEventArgsToCommand { get; set; }

    public void Invoke() => this.Invoke((object) null);

    protected override void Invoke(object parameter)
    {
      if (this.AssociatedElementIsDisabled())
        return;
      ICommand command = this.GetCommand();
      object parameter1 = this.CommandParameterValue;
      if (parameter1 == null && this.PassEventArgsToCommand)
        parameter1 = parameter;
      if (command == null || !command.CanExecute(parameter1))
        return;
      command.Execute(parameter1);
    }

    private static void OnCommandChanged(
      EventToCommand element,
      DependencyPropertyChangedEventArgs e)
    {
      if (element == null)
        return;
      if (e.OldValue != null)
        ((ICommand) e.OldValue).CanExecuteChanged -= new EventHandler(element.OnCommandCanExecuteChanged);
      ICommand newValue = (ICommand) e.NewValue;
      if (newValue != null)
        newValue.CanExecuteChanged += new EventHandler(element.OnCommandCanExecuteChanged);
      element.EnableDisableElement();
    }

    private bool AssociatedElementIsDisabled()
    {
      FrameworkElement associatedObject = this.GetAssociatedObject();
      if (this.AssociatedObject == null)
        return true;
      return associatedObject != null && !associatedObject.IsEnabled;
    }

    private void EnableDisableElement()
    {
      FrameworkElement associatedObject = this.GetAssociatedObject();
      if (associatedObject == null)
        return;
      ICommand command = this.GetCommand();
      if (!this.MustToggleIsEnabledValue || command == null)
        return;
      associatedObject.IsEnabled = command.CanExecute(this.CommandParameterValue);
    }

    private void OnCommandCanExecuteChanged(object sender, EventArgs e) => this.EnableDisableElement();
  }
}
