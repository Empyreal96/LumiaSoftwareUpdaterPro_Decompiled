// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.ObservableObject
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GalaSoft.MvvmLight
{
  public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public event PropertyChangingEventHandler PropertyChanging;

    protected PropertyChangedEventHandler PropertyChangedHandler => this.PropertyChanged;

    protected PropertyChangingEventHandler PropertyChangingHandler => this.PropertyChanging;

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName)
    {
      Type type = this.GetType();
      if (!string.IsNullOrEmpty(propertyName) && type.GetProperty(propertyName) == (PropertyInfo) null && (!(this is ICustomTypeDescriptor customTypeDescriptor) || !customTypeDescriptor.GetProperties().Cast<PropertyDescriptor>().Any<PropertyDescriptor>((Func<PropertyDescriptor, bool>) (property => property.Name == propertyName))))
        throw new ArgumentException("Property not found", propertyName);
    }

    protected virtual void RaisePropertyChanging(string propertyName)
    {
      PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
      if (propertyChanging == null)
        return;
      propertyChanging((object) this, new PropertyChangingEventArgs(propertyName));
    }

    protected virtual void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
    {
      PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
      if (propertyChanging == null)
        return;
      string propertyName = this.GetPropertyName<T>(propertyExpression);
      propertyChanging((object) this, new PropertyChangingEventArgs(propertyName));
    }

    protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      string propertyName = this.GetPropertyName<T>(propertyExpression);
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
    {
      if (propertyExpression == null)
        throw new ArgumentNullException(nameof (propertyExpression));
      if (!(propertyExpression.Body is MemberExpression body))
        throw new ArgumentException("Invalid argument", nameof (propertyExpression));
      PropertyInfo member = body.Member as PropertyInfo;
      return !(member == (PropertyInfo) null) ? member.Name : throw new ArgumentException("Argument is not a property", nameof (propertyExpression));
    }

    protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
    {
      if (EqualityComparer<T>.Default.Equals(field, newValue))
        return false;
      this.RaisePropertyChanging<T>(propertyExpression);
      field = newValue;
      this.RaisePropertyChanged<T>(propertyExpression);
      return true;
    }

    protected bool Set<T>(string propertyName, ref T field, T newValue)
    {
      if (EqualityComparer<T>.Default.Equals(field, newValue))
        return false;
      this.RaisePropertyChanging(propertyName);
      field = newValue;
      this.RaisePropertyChanged(propertyName);
      return true;
    }
  }
}
