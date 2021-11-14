// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Command.RelayCommand`1
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using GalaSoft.MvvmLight.Helpers;
using System;
using System.Windows.Input;

namespace GalaSoft.MvvmLight.Command
{
  public class RelayCommand<T> : ICommand
  {
    private readonly WeakAction<T> _execute;
    private readonly WeakFunc<T, bool> _canExecute;

    public RelayCommand(Action<T> execute)
      : this(execute, (Func<T, bool>) null)
    {
    }

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
    {
      this._execute = execute != null ? new WeakAction<T>(execute) : throw new ArgumentNullException(nameof (execute));
      if (canExecute == null)
        return;
      this._canExecute = new WeakFunc<T, bool>(canExecute);
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (this._canExecute == null)
          return;
        CommandManager.RequerySuggested += value;
      }
      remove
      {
        if (this._canExecute == null)
          return;
        CommandManager.RequerySuggested -= value;
      }
    }

    public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

    public bool CanExecute(object parameter)
    {
      if (this._canExecute == null)
        return true;
      if (!this._canExecute.IsStatic && !this._canExecute.IsAlive)
        return false;
      return parameter == null && typeof (T).IsValueType ? this._canExecute.Execute(default (T)) : this._canExecute.Execute((T) parameter);
    }

    public virtual void Execute(object parameter)
    {
      object parameter1 = parameter;
      if (parameter != null && parameter.GetType() != typeof (T) && parameter is IConvertible)
        parameter1 = Convert.ChangeType(parameter, typeof (T), (IFormatProvider) null);
      if (!this.CanExecute(parameter1) || this._execute == null || !this._execute.IsStatic && !this._execute.IsAlive)
        return;
      if (parameter1 == null)
      {
        if (typeof (T).IsValueType)
          this._execute.Execute(default (T));
        else
          this._execute.Execute((T) parameter1);
      }
      else
        this._execute.Execute((T) parameter1);
    }
  }
}
