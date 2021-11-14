// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ViewModelBase
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.LsuPro
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    private bool closeWindow;
    private readonly Dispatcher dispatcher = Application.Current == null ? Dispatcher.CurrentDispatcher : Application.Current.Dispatcher;

    public event PropertyChangedEventHandler PropertyChanged;

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "reviewed")]
    protected virtual void OnPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler == null)
        return;
      string propertyName = ViewModelBase.GetPropertyName<T>(propertyExpression);
      this.InvokeIfRequired((Action) (() => handler((object) this, new PropertyChangedEventArgs(propertyName))));
    }

    private static string GetPropertyName<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
    {
      if (propertyExpression == null)
        throw new ArgumentNullException(nameof (propertyExpression));
      if (!(propertyExpression.Body is MemberExpression body))
        throw new ArgumentException("Expression is not a member access expression.", nameof (propertyExpression));
      PropertyInfo member = body.Member as PropertyInfo;
      if (member == (PropertyInfo) null)
        throw new ArgumentException("The member access expression does not access a property.", nameof (propertyExpression));
      if (member.GetGetMethod(true).IsStatic)
        throw new ArgumentException("The referenced property is a static property.", nameof (propertyExpression));
      return body.Member.Name;
    }

    public bool CloseWindow
    {
      get => this.closeWindow;
      set
      {
        this.closeWindow = value;
        this.OnPropertyChanged<bool>((System.Linq.Expressions.Expression<Func<bool>>) (() => this.CloseWindow));
      }
    }

    protected void InvokeIfRequired(Action action)
    {
      if (this.dispatcher.Thread != Thread.CurrentThread)
        this.dispatcher.Invoke(DispatcherPriority.DataBind, (Delegate) action);
      else
        action();
    }
  }
}
