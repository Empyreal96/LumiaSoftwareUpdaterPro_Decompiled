// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Command.RelayCommand
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using GalaSoft.MvvmLight.Helpers;
using System;
using System.Windows.Input;

namespace GalaSoft.MvvmLight.Command
{
  public class RelayCommand : ICommand
  {
    private readonly WeakAction _execute;
    private readonly WeakFunc<bool> _canExecute;

    public RelayCommand(Action execute)
      : this(execute, (Func<bool>) null)
    {
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
    {
      this._execute = execute != null ? new WeakAction(execute) : throw new ArgumentNullException(nameof (execute));
      if (canExecute == null)
        return;
      this._canExecute = new WeakFunc<bool>(canExecute);
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
      return (this._canExecute.IsStatic || this._canExecute.IsAlive) && this._canExecute.Execute();
    }

    public virtual void Execute(object parameter)
    {
      if (!this.CanExecute(parameter) || this._execute == null || !this._execute.IsStatic && !this._execute.IsAlive)
        return;
      this._execute.Execute();
    }
  }
}
