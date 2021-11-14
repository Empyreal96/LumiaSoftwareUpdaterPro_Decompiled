// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Messaging.Messenger
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace GalaSoft.MvvmLight.Messaging
{
  public class Messenger : IMessenger
  {
    private static readonly object CreationLock = new object();
    private static IMessenger _defaultInstance;
    private readonly object _registerLock = new object();
    private Dictionary<Type, List<Messenger.WeakActionAndToken>> _recipientsOfSubclassesAction;
    private Dictionary<Type, List<Messenger.WeakActionAndToken>> _recipientsStrictAction;
    private bool _isCleanupRegistered;

    public static IMessenger Default
    {
      get
      {
        if (Messenger._defaultInstance == null)
        {
          lock (Messenger.CreationLock)
          {
            if (Messenger._defaultInstance == null)
              Messenger._defaultInstance = (IMessenger) new Messenger();
          }
        }
        return Messenger._defaultInstance;
      }
    }

    public virtual void Register<TMessage>(object recipient, Action<TMessage> action) => this.Register<TMessage>(recipient, (object) null, false, action);

    public virtual void Register<TMessage>(
      object recipient,
      bool receiveDerivedMessagesToo,
      Action<TMessage> action)
    {
      this.Register<TMessage>(recipient, (object) null, receiveDerivedMessagesToo, action);
    }

    public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action) => this.Register<TMessage>(recipient, token, false, action);

    public virtual void Register<TMessage>(
      object recipient,
      object token,
      bool receiveDerivedMessagesToo,
      Action<TMessage> action)
    {
      lock (this._registerLock)
      {
        Type key = typeof (TMessage);
        Dictionary<Type, List<Messenger.WeakActionAndToken>> dictionary;
        if (receiveDerivedMessagesToo)
        {
          if (this._recipientsOfSubclassesAction == null)
            this._recipientsOfSubclassesAction = new Dictionary<Type, List<Messenger.WeakActionAndToken>>();
          dictionary = this._recipientsOfSubclassesAction;
        }
        else
        {
          if (this._recipientsStrictAction == null)
            this._recipientsStrictAction = new Dictionary<Type, List<Messenger.WeakActionAndToken>>();
          dictionary = this._recipientsStrictAction;
        }
        lock (dictionary)
        {
          List<Messenger.WeakActionAndToken> weakActionAndTokenList;
          if (!dictionary.ContainsKey(key))
          {
            weakActionAndTokenList = new List<Messenger.WeakActionAndToken>();
            dictionary.Add(key, weakActionAndTokenList);
          }
          else
            weakActionAndTokenList = dictionary[key];
          WeakAction<TMessage> weakAction = new WeakAction<TMessage>(recipient, action);
          Messenger.WeakActionAndToken weakActionAndToken = new Messenger.WeakActionAndToken()
          {
            Action = (WeakAction) weakAction,
            Token = token
          };
          weakActionAndTokenList.Add(weakActionAndToken);
        }
      }
      this.RequestCleanup();
    }

    public virtual void Send<TMessage>(TMessage message) => this.SendToTargetOrType<TMessage>(message, (Type) null, (object) null);

    public virtual void Send<TMessage, TTarget>(TMessage message) => this.SendToTargetOrType<TMessage>(message, typeof (TTarget), (object) null);

    public virtual void Send<TMessage>(TMessage message, object token) => this.SendToTargetOrType<TMessage>(message, (Type) null, token);

    public virtual void Unregister(object recipient)
    {
      Messenger.UnregisterFromLists(recipient, this._recipientsOfSubclassesAction);
      Messenger.UnregisterFromLists(recipient, this._recipientsStrictAction);
    }

    public virtual void Unregister<TMessage>(object recipient) => this.Unregister<TMessage>(recipient, (object) null, (Action<TMessage>) null);

    public virtual void Unregister<TMessage>(object recipient, object token) => this.Unregister<TMessage>(recipient, token, (Action<TMessage>) null);

    public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action) => this.Unregister<TMessage>(recipient, (object) null, action);

    public virtual void Unregister<TMessage>(
      object recipient,
      object token,
      Action<TMessage> action)
    {
      Messenger.UnregisterFromLists<TMessage>(recipient, token, action, this._recipientsStrictAction);
      Messenger.UnregisterFromLists<TMessage>(recipient, token, action, this._recipientsOfSubclassesAction);
      this.RequestCleanup();
    }

    public static void OverrideDefault(IMessenger newMessenger) => Messenger._defaultInstance = newMessenger;

    public static void Reset() => Messenger._defaultInstance = (IMessenger) null;

    public void ResetAll() => Messenger.Reset();

    private static void CleanupList(
      IDictionary<Type, List<Messenger.WeakActionAndToken>> lists)
    {
      if (lists == null)
        return;
      lock (lists)
      {
        List<Type> typeList = new List<Type>();
        foreach (KeyValuePair<Type, List<Messenger.WeakActionAndToken>> list in (IEnumerable<KeyValuePair<Type, List<Messenger.WeakActionAndToken>>>) lists)
        {
          List<Messenger.WeakActionAndToken> weakActionAndTokenList = new List<Messenger.WeakActionAndToken>();
          foreach (Messenger.WeakActionAndToken weakActionAndToken in list.Value)
          {
            if (weakActionAndToken.Action == null || !weakActionAndToken.Action.IsAlive)
              weakActionAndTokenList.Add(weakActionAndToken);
          }
          foreach (Messenger.WeakActionAndToken weakActionAndToken in weakActionAndTokenList)
            list.Value.Remove(weakActionAndToken);
          if (list.Value.Count == 0)
            typeList.Add(list.Key);
        }
        foreach (Type key in typeList)
          lists.Remove(key);
      }
    }

    private static void SendToList<TMessage>(
      TMessage message,
      IEnumerable<Messenger.WeakActionAndToken> list,
      Type messageTargetType,
      object token)
    {
      if (list == null)
        return;
      foreach (Messenger.WeakActionAndToken weakActionAndToken in list.Take<Messenger.WeakActionAndToken>(list.Count<Messenger.WeakActionAndToken>()).ToList<Messenger.WeakActionAndToken>())
      {
        if (weakActionAndToken.Action is IExecuteWithObject action1 && weakActionAndToken.Action.IsAlive && weakActionAndToken.Action.Target != null && (messageTargetType == (Type) null || weakActionAndToken.Action.Target.GetType() == messageTargetType || messageTargetType.IsAssignableFrom(weakActionAndToken.Action.Target.GetType())) && (weakActionAndToken.Token == null && token == null || weakActionAndToken.Token != null && weakActionAndToken.Token.Equals(token)))
          action1.ExecuteWithObject((object) message);
      }
    }

    private static void UnregisterFromLists(
      object recipient,
      Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
    {
      if (recipient == null || lists == null || lists.Count == 0)
        return;
      lock (lists)
      {
        foreach (Type key in lists.Keys)
        {
          foreach (Messenger.WeakActionAndToken weakActionAndToken in lists[key])
          {
            IExecuteWithObject action = (IExecuteWithObject) weakActionAndToken.Action;
            if (action != null && recipient == action.Target)
              action.MarkForDeletion();
          }
        }
      }
    }

    private static void UnregisterFromLists<TMessage>(
      object recipient,
      object token,
      Action<TMessage> action,
      Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
    {
      Type key = typeof (TMessage);
      if (recipient == null || lists == null || (lists.Count == 0 || !lists.ContainsKey(key)))
        return;
      lock (lists)
      {
        foreach (Messenger.WeakActionAndToken weakActionAndToken in lists[key])
        {
          if (weakActionAndToken.Action is WeakAction<TMessage> action3 && recipient == action3.Target && (action == null || action.Method.Name == action3.MethodName) && (token == null || token.Equals(weakActionAndToken.Token)))
            weakActionAndToken.Action.MarkForDeletion();
        }
      }
    }

    public void RequestCleanup()
    {
      if (this._isCleanupRegistered)
        return;
      Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) new Action(this.Cleanup), DispatcherPriority.ApplicationIdle, (object[]) null);
      this._isCleanupRegistered = true;
    }

    public void Cleanup()
    {
      Messenger.CleanupList((IDictionary<Type, List<Messenger.WeakActionAndToken>>) this._recipientsOfSubclassesAction);
      Messenger.CleanupList((IDictionary<Type, List<Messenger.WeakActionAndToken>>) this._recipientsStrictAction);
      this._isCleanupRegistered = false;
    }

    private void SendToTargetOrType<TMessage>(
      TMessage message,
      Type messageTargetType,
      object token)
    {
      Type type1 = typeof (TMessage);
      if (this._recipientsOfSubclassesAction != null)
      {
        foreach (Type type2 in this._recipientsOfSubclassesAction.Keys.Take<Type>(this._recipientsOfSubclassesAction.Count<KeyValuePair<Type, List<Messenger.WeakActionAndToken>>>()).ToList<Type>())
        {
          List<Messenger.WeakActionAndToken> weakActionAndTokenList = (List<Messenger.WeakActionAndToken>) null;
          if (type1 == type2 || type1.IsSubclassOf(type2) || type2.IsAssignableFrom(type1))
          {
            lock (this._recipientsOfSubclassesAction)
              weakActionAndTokenList = this._recipientsOfSubclassesAction[type2].Take<Messenger.WeakActionAndToken>(this._recipientsOfSubclassesAction[type2].Count<Messenger.WeakActionAndToken>()).ToList<Messenger.WeakActionAndToken>();
          }
          Messenger.SendToList<TMessage>(message, (IEnumerable<Messenger.WeakActionAndToken>) weakActionAndTokenList, messageTargetType, token);
        }
      }
      if (this._recipientsStrictAction != null)
      {
        lock (this._recipientsStrictAction)
        {
          if (this._recipientsStrictAction.ContainsKey(type1))
          {
            List<Messenger.WeakActionAndToken> list = this._recipientsStrictAction[type1].Take<Messenger.WeakActionAndToken>(this._recipientsStrictAction[type1].Count<Messenger.WeakActionAndToken>()).ToList<Messenger.WeakActionAndToken>();
            Messenger.SendToList<TMessage>(message, (IEnumerable<Messenger.WeakActionAndToken>) list, messageTargetType, token);
          }
        }
      }
      this.RequestCleanup();
    }

    private struct WeakActionAndToken
    {
      public WeakAction Action;
      public object Token;
    }
  }
}
