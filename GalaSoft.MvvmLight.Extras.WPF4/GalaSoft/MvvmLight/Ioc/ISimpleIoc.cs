// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Ioc.ISimpleIoc
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using Microsoft.Practices.ServiceLocation;
using System;

namespace GalaSoft.MvvmLight.Ioc
{
  public interface ISimpleIoc : IServiceLocator, IServiceProvider
  {
    bool ContainsCreated<TClass>();

    bool ContainsCreated<TClass>(string key);

    bool IsRegistered<T>();

    bool IsRegistered<T>(string key);

    void Register<TInterface, TClass>()
      where TInterface : class
      where TClass : class;

    void Register<TInterface, TClass>(bool createInstanceImmediately)
      where TInterface : class
      where TClass : class;

    void Register<TClass>() where TClass : class;

    void Register<TClass>(bool createInstanceImmediately) where TClass : class;

    void Register<TClass>(Func<TClass> factory) where TClass : class;

    void Register<TClass>(Func<TClass> factory, bool createInstanceImmediately) where TClass : class;

    void Register<TClass>(Func<TClass> factory, string key) where TClass : class;

    void Register<TClass>(Func<TClass> factory, string key, bool createInstanceImmediately) where TClass : class;

    void Reset();

    void Unregister<TClass>() where TClass : class;

    void Unregister<TClass>(TClass instance) where TClass : class;

    void Unregister<TClass>(string key) where TClass : class;
  }
}
