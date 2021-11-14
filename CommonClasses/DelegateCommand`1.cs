// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DelegateCommand`1
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Windows.Input;

namespace Microsoft.LsuPro
{
  public class DelegateCommand<T> : ICommand
  {
    private readonly Action<T> executeAction;
    private readonly Predicate<T> canExecutePredicate;

    public DelegateCommand(Action<T> executeAction)
      : this(executeAction, (Predicate<T>) null)
    {
    }

    public DelegateCommand(Action<T> executeAction, Predicate<T> canExecute)
      : this(executeAction, canExecute, string.Empty)
    {
    }

    public DelegateCommand(
      Action<T> executeAction,
      Predicate<T> canExecute,
      string delegateCommandLabel)
    {
      this.executeAction = executeAction;
      this.canExecutePredicate = canExecute;
      this.Label = delegateCommandLabel;
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (this.canExecutePredicate == null)
          return;
        CommandManager.RequerySuggested += value;
      }
      remove
      {
        if (this.canExecutePredicate == null)
          return;
        CommandManager.RequerySuggested -= value;
      }
    }

    public string Label { get; set; }

    public void Execute(object parameter) => this.executeAction((T) parameter);

    public bool CanExecute(object parameter) => this.canExecutePredicate == null || this.canExecutePredicate((T) parameter);
  }
}
