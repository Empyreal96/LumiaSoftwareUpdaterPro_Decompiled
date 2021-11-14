// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Ioc.SimpleIoc
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GalaSoft.MvvmLight.Ioc
{
  public class SimpleIoc : ISimpleIoc, IServiceLocator, IServiceProvider
  {
    private readonly Dictionary<Type, ConstructorInfo> _constructorInfos = new Dictionary<Type, ConstructorInfo>();
    private readonly string _defaultKey = Guid.NewGuid().ToString();
    private readonly object[] _emptyArguments = new object[0];
    private readonly Dictionary<Type, Dictionary<string, Delegate>> _factories = new Dictionary<Type, Dictionary<string, Delegate>>();
    private readonly Dictionary<Type, Dictionary<string, object>> _instancesRegistry = new Dictionary<Type, Dictionary<string, object>>();
    private readonly Dictionary<Type, Type> _interfaceToClassMap = new Dictionary<Type, Type>();
    private readonly object _syncLock = new object();
    private static SimpleIoc _default;

    public static SimpleIoc Default => SimpleIoc._default ?? (SimpleIoc._default = new SimpleIoc());

    public bool ContainsCreated<TClass>() => this.ContainsCreated<TClass>((string) null);

    public bool ContainsCreated<TClass>(string key)
    {
      Type key1 = typeof (TClass);
      if (!this._instancesRegistry.ContainsKey(key1))
        return false;
      return string.IsNullOrEmpty(key) || this._instancesRegistry[key1].ContainsKey(key);
    }

    public bool IsRegistered<T>() => this._interfaceToClassMap.ContainsKey(typeof (T));

    public bool IsRegistered<T>(string key)
    {
      Type key1 = typeof (T);
      return this._interfaceToClassMap.ContainsKey(key1) && this._factories.ContainsKey(key1) && this._factories[key1].ContainsKey(key);
    }

    public void Register<TInterface, TClass>()
      where TInterface : class
      where TClass : class
    {
      this.Register<TInterface, TClass>(false);
    }

    public void Register<TInterface, TClass>(bool createInstanceImmediately)
      where TInterface : class
      where TClass : class
    {
      lock (this._syncLock)
      {
        Type type1 = typeof (TInterface);
        Type type2 = typeof (TClass);
        if (this._interfaceToClassMap.ContainsKey(type1))
        {
          if (this._interfaceToClassMap[type1] != type2)
            throw new InvalidOperationException(string.Format("There is already a class registered for {0}.", (object) type1.FullName));
        }
        else
        {
          this._interfaceToClassMap.Add(type1, type2);
          this._constructorInfos.Add(type2, this.GetConstructorInfo(type2));
        }
        Func<TInterface> factory = new Func<TInterface>(this.MakeInstance<TInterface>);
        this.DoRegister<TInterface>(type1, factory, this._defaultKey);
        if (!createInstanceImmediately)
          return;
        this.GetInstance<TInterface>();
      }
    }

    public void Register<TClass>() where TClass : class => this.Register<TClass>(false);

    public void Register<TClass>(bool createInstanceImmediately) where TClass : class
    {
      Type type = typeof (TClass);
      if (type.IsInterface)
        throw new ArgumentException("An interface cannot be registered alone.");
      lock (this._syncLock)
      {
        if (this._factories.ContainsKey(type) && this._factories[type].ContainsKey(this._defaultKey))
        {
          if (!this._constructorInfos.ContainsKey(type))
            throw new InvalidOperationException(string.Format("Class {0} is already registered.", (object) type));
        }
        else
        {
          if (!this._interfaceToClassMap.ContainsKey(type))
            this._interfaceToClassMap.Add(type, (Type) null);
          this._constructorInfos.Add(type, this.GetConstructorInfo(type));
          Func<TClass> factory = new Func<TClass>(this.MakeInstance<TClass>);
          this.DoRegister<TClass>(type, factory, this._defaultKey);
          if (!createInstanceImmediately)
            return;
          this.GetInstance<TClass>();
        }
      }
    }

    public void Register<TClass>(Func<TClass> factory) where TClass : class => this.Register<TClass>(factory, false);

    public void Register<TClass>(Func<TClass> factory, bool createInstanceImmediately) where TClass : class
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      lock (this._syncLock)
      {
        Type type = typeof (TClass);
        if (this._factories.ContainsKey(type) && this._factories[type].ContainsKey(this._defaultKey))
          throw new InvalidOperationException(string.Format("There is already a factory registered for {0}.", (object) type.FullName));
        if (!this._interfaceToClassMap.ContainsKey(type))
          this._interfaceToClassMap.Add(type, (Type) null);
        this.DoRegister<TClass>(type, factory, this._defaultKey);
        if (!createInstanceImmediately)
          return;
        this.GetInstance<TClass>();
      }
    }

    public void Register<TClass>(Func<TClass> factory, string key) where TClass : class => this.Register<TClass>(factory, key, false);

    public void Register<TClass>(Func<TClass> factory, string key, bool createInstanceImmediately) where TClass : class
    {
      lock (this._syncLock)
      {
        Type type = typeof (TClass);
        if (this._factories.ContainsKey(type) && this._factories[type].ContainsKey(key))
          throw new InvalidOperationException(string.Format("There is already a factory registered for {0} with key {1}.", (object) type.FullName, (object) key));
        if (!this._interfaceToClassMap.ContainsKey(type))
          this._interfaceToClassMap.Add(type, (Type) null);
        this.DoRegister<TClass>(type, factory, key);
        if (!createInstanceImmediately)
          return;
        this.GetInstance<TClass>(key);
      }
    }

    public void Reset()
    {
      this._interfaceToClassMap.Clear();
      this._instancesRegistry.Clear();
      this._constructorInfos.Clear();
      this._factories.Clear();
    }

    public void Unregister<TClass>() where TClass : class
    {
      lock (this._syncLock)
      {
        Type key1 = typeof (TClass);
        Type key2;
        if (this._interfaceToClassMap.ContainsKey(key1))
        {
          Type type = this._interfaceToClassMap[key1];
          if ((object) type == null)
            type = key1;
          key2 = type;
        }
        else
          key2 = key1;
        if (this._instancesRegistry.ContainsKey(key1))
          this._instancesRegistry.Remove(key1);
        if (this._interfaceToClassMap.ContainsKey(key1))
          this._interfaceToClassMap.Remove(key1);
        if (this._factories.ContainsKey(key1))
          this._factories.Remove(key1);
        if (!this._constructorInfos.ContainsKey(key2))
          return;
        this._constructorInfos.Remove(key2);
      }
    }

    public void Unregister<TClass>(TClass instance) where TClass : class
    {
      lock (this._syncLock)
      {
        Type key1 = typeof (TClass);
        if (!this._instancesRegistry.ContainsKey(key1))
          return;
        Dictionary<string, object> source = this._instancesRegistry[key1];
        List<KeyValuePair<string, object>> list = source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => pair.Value == (object) (TClass) instance)).ToList<KeyValuePair<string, object>>();
        for (int index = 0; index < list.Count<KeyValuePair<string, object>>(); ++index)
        {
          string key2 = list[index].Key;
          source.Remove(key2);
          if (this._factories.ContainsKey(key1) && this._factories[key1].ContainsKey(key2))
            this._factories[key1].Remove(key2);
        }
      }
    }

    public void Unregister<TClass>(string key) where TClass : class
    {
      lock (this._syncLock)
      {
        Type key1 = typeof (TClass);
        if (this._instancesRegistry.ContainsKey(key1))
        {
          Dictionary<string, object> source = this._instancesRegistry[key1];
          List<KeyValuePair<string, object>> list = source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => pair.Key == key)).ToList<KeyValuePair<string, object>>();
          for (int index = 0; index < list.Count<KeyValuePair<string, object>>(); ++index)
            source.Remove(list[index].Key);
        }
        if (!this._factories.ContainsKey(key1) || !this._factories[key1].ContainsKey(key))
          return;
        this._factories[key1].Remove(key);
      }
    }

    private object DoGetService(Type serviceType, string key)
    {
      lock (this._syncLock)
      {
        if (string.IsNullOrEmpty(key))
          key = this._defaultKey;
        Dictionary<string, object> dictionary;
        if (!this._instancesRegistry.ContainsKey(serviceType))
        {
          if (!this._interfaceToClassMap.ContainsKey(serviceType))
            throw new ActivationException(string.Format("Type not found in cache: {0}.", (object) serviceType.FullName));
          dictionary = new Dictionary<string, object>();
          this._instancesRegistry.Add(serviceType, dictionary);
        }
        else
          dictionary = this._instancesRegistry[serviceType];
        if (dictionary.ContainsKey(key))
          return dictionary[key];
        object obj = (object) null;
        if (this._factories.ContainsKey(serviceType))
        {
          if (this._factories[serviceType].ContainsKey(key))
          {
            obj = this._factories[serviceType][key].DynamicInvoke((object[]) null);
          }
          else
          {
            if (!this._factories[serviceType].ContainsKey(this._defaultKey))
              throw new ActivationException(string.Format("Type not found in cache without a key: {0}", (object) serviceType.FullName));
            obj = this._factories[serviceType][this._defaultKey].DynamicInvoke((object[]) null);
          }
        }
        dictionary.Add(key, obj);
        return obj;
      }
    }

    private void DoRegister<TClass>(Type classType, Func<TClass> factory, string key)
    {
      if (this._factories.ContainsKey(classType))
      {
        if (this._factories[classType].ContainsKey(key))
        {
          if (key == this._defaultKey)
            throw new InvalidOperationException(string.Format("Class {0} is already registered.", (object) classType.FullName));
          throw new InvalidOperationException(string.Format("Class {0} is already registered with key {1}.", (object) key));
        }
        this._factories[classType].Add(key, (Delegate) factory);
      }
      else
      {
        Dictionary<string, Delegate> dictionary = new Dictionary<string, Delegate>()
        {
          {
            key,
            (Delegate) factory
          }
        };
        this._factories.Add(classType, dictionary);
      }
    }

    private ConstructorInfo GetConstructorInfo(Type serviceType)
    {
      Type type1;
      if (this._interfaceToClassMap.ContainsKey(serviceType))
      {
        Type type2 = this._interfaceToClassMap[serviceType];
        if ((object) type2 == null)
          type2 = serviceType;
        type1 = type2;
      }
      else
        type1 = serviceType;
      ConstructorInfo[] constructors = type1.GetConstructors();
      if (constructors.Length <= 1)
        return constructors[0];
      ConstructorInfo constructorInfo = ((IEnumerable<ConstructorInfo>) constructors).Select(t => new
      {
        t = t,
        attribute = Attribute.GetCustomAttribute((MemberInfo) t, typeof (PreferredConstructorAttribute))
      }).Where(_param0 => _param0.attribute != null).Select(_param0 => _param0.t).FirstOrDefault<ConstructorInfo>();
      return !(constructorInfo == (ConstructorInfo) null) ? constructorInfo : throw new ActivationException("Cannot build instance: Multiple constructors found but none marked with PreferredConstructor.");
    }

    private TClass MakeInstance<TClass>()
    {
      Type type = typeof (TClass);
      ConstructorInfo constructorInfo = this._constructorInfos.ContainsKey(type) ? this._constructorInfos[type] : this.GetConstructorInfo(type);
      ParameterInfo[] parameters1 = constructorInfo.GetParameters();
      if (parameters1.Length == 0)
        return (TClass) constructorInfo.Invoke(this._emptyArguments);
      object[] parameters2 = new object[parameters1.Length];
      foreach (ParameterInfo parameterInfo in parameters1)
        parameters2[parameterInfo.Position] = this.GetService(parameterInfo.ParameterType);
      return (TClass) constructorInfo.Invoke(parameters2);
    }

    public IEnumerable<object> GetAllCreatedInstances(Type serviceType) => this._instancesRegistry.ContainsKey(serviceType) ? (IEnumerable<object>) this._instancesRegistry[serviceType].Values : (IEnumerable<object>) new List<object>();

    public IEnumerable<TService> GetAllCreatedInstances<TService>() => this.GetAllCreatedInstances(typeof (TService)).Select<object, TService>((Func<object, TService>) (instance => (TService) instance));

    public object GetService(Type serviceType) => this.DoGetService(serviceType, this._defaultKey);

    public IEnumerable<object> GetAllInstances(Type serviceType)
    {
      lock (this._factories)
      {
        if (this._factories.ContainsKey(serviceType))
        {
          foreach (KeyValuePair<string, Delegate> keyValuePair in this._factories[serviceType])
            this.GetInstance(serviceType, keyValuePair.Key);
        }
      }
      return this._instancesRegistry.ContainsKey(serviceType) ? (IEnumerable<object>) this._instancesRegistry[serviceType].Values : (IEnumerable<object>) new List<object>();
    }

    public IEnumerable<TService> GetAllInstances<TService>() => this.GetAllInstances(typeof (TService)).Select<object, TService>((Func<object, TService>) (instance => (TService) instance));

    public object GetInstance(Type serviceType) => this.DoGetService(serviceType, this._defaultKey);

    public object GetInstance(Type serviceType, string key) => this.DoGetService(serviceType, key);

    public TService GetInstance<TService>() => (TService) this.DoGetService(typeof (TService), this._defaultKey);

    public TService GetInstance<TService>(string key) => (TService) this.DoGetService(typeof (TService), key);
  }
}
