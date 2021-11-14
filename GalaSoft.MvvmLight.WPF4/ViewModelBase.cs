// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.ViewModelBase
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace GalaSoft.MvvmLight
{
  public abstract class ViewModelBase : ObservableObject, ICleanup
  {
    private static bool? _isInDesignMode;
    private IMessenger _messengerInstance;

    public ViewModelBase()
      : this((IMessenger) null)
    {
    }

    public ViewModelBase(IMessenger messenger) => this.MessengerInstance = messenger;

    public bool IsInDesignMode => ViewModelBase.IsInDesignModeStatic;

    public static bool IsInDesignModeStatic
    {
      get
      {
        if (!ViewModelBase._isInDesignMode.HasValue)
          ViewModelBase._isInDesignMode = new bool?((bool) DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof (FrameworkElement)).Metadata.DefaultValue);
        return ViewModelBase._isInDesignMode.Value;
      }
    }

    protected IMessenger MessengerInstance
    {
      get => this._messengerInstance ?? Messenger.Default;
      set => this._messengerInstance = value;
    }

    public virtual void Cleanup() => this.MessengerInstance.Unregister((object) this);

    protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName) => this.MessengerInstance.Send<PropertyChangedMessage<T>>(new PropertyChangedMessage<T>((object) this, oldValue, newValue, propertyName));

    protected virtual void RaisePropertyChanged<T>(
      string propertyName,
      T oldValue,
      T newValue,
      bool broadcast)
    {
      if (string.IsNullOrEmpty(propertyName))
        throw new ArgumentException("This method cannot be called with an empty string", nameof (propertyName));
      this.RaisePropertyChanged(propertyName);
      if (!broadcast)
        return;
      this.Broadcast<T>(oldValue, newValue, propertyName);
    }

    protected virtual void RaisePropertyChanged<T>(
      System.Linq.Expressions.Expression<Func<T>> propertyExpression,
      T oldValue,
      T newValue,
      bool broadcast)
    {
      PropertyChangedEventHandler propertyChangedHandler = this.PropertyChangedHandler;
      if (propertyChangedHandler == null && !broadcast)
        return;
      string propertyName = this.GetPropertyName<T>(propertyExpression);
      if (propertyChangedHandler != null)
        propertyChangedHandler((object) this, new PropertyChangedEventArgs(propertyName));
      if (!broadcast)
        return;
      this.Broadcast<T>(oldValue, newValue, propertyName);
    }

    protected bool Set<T>(
      System.Linq.Expressions.Expression<Func<T>> propertyExpression,
      ref T field,
      T newValue,
      bool broadcast)
    {
      if (EqualityComparer<T>.Default.Equals(field, newValue))
        return false;
      this.RaisePropertyChanging<T>(propertyExpression);
      T oldValue = field;
      field = newValue;
      this.RaisePropertyChanged<T>(propertyExpression, oldValue, field, broadcast);
      return true;
    }

    protected bool Set<T>(string propertyName, ref T field, T newValue, bool broadcast)
    {
      if (EqualityComparer<T>.Default.Equals(field, newValue))
        return false;
      this.RaisePropertyChanging(propertyName);
      T oldValue = field;
      field = newValue;
      this.RaisePropertyChanged<T>(propertyName, oldValue, field, broadcast);
      return true;
    }
  }
}
