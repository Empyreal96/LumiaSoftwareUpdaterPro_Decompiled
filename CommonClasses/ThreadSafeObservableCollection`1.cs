// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ThreadSafeObservableCollection`1
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.LsuPro
{
  public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
  {
    private readonly Dispatcher dispatcher;
    private object lockObject = new object();

    public ThreadSafeObservableCollection() => this.dispatcher = Application.Current == null ? Dispatcher.CurrentDispatcher : Application.Current.Dispatcher;

    public void AddRange(IEnumerable<T> list) => this.AddRange(list, false, true);

    public void ClearAndAddRange(IEnumerable<T> list) => this.AddRange(list, true, true);

    private void AddRange(IEnumerable<T> list, bool clearFirst, bool invokeIfRequired)
    {
      if (list == null)
        return;
      if (invokeIfRequired)
      {
        this.InvokeIfRequired((Action) (() => this.AddRange(list, clearFirst, false)));
      }
      else
      {
        lock (this.lockObject)
        {
          this.CheckReentrancy();
          if (clearFirst)
            this.Items.Clear();
          foreach (T obj in list)
            this.Items.Add(obj);
          this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
      }
    }

    public new void Add(T item) => this.InvokeIfRequired((Action) (() => base.Add(item)));

    public new void Clear() => this.InvokeIfRequired((Action) (() => base.Clear()));

    public new void Insert(int index, T item) => this.InvokeIfRequired((Action) (() => base.Insert(index, item)));

    public new void RemoveAt(int index) => this.InvokeIfRequired((Action) (() => base.RemoveAt(index)));

    public new bool Remove(T item)
    {
      bool res = false;
      this.InvokeIfRequired((Action) (() => res = base.Remove(item)));
      return res;
    }

    public new int IndexOf(T item)
    {
      int res = 0;
      this.InvokeIfRequired((Action) (() => res = base.IndexOf(item)));
      return res;
    }

    public new bool Contains(T item)
    {
      bool res = false;
      this.InvokeIfRequired((Action) (() => res = base.Contains(item)));
      return res;
    }

    public new void SetItem(int index, T item) => this.InvokeIfRequired((Action) (() => base.SetItem(index, item)));

    public new T this[int index]
    {
      get
      {
        T res = default (T);
        this.InvokeIfRequired((Action) (() => res = base[index]));
        return res;
      }
      set => this.InvokeIfRequired((Action) (() => base[index] = value));
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => this.InvokeIfRequired((Action) (() => base.OnCollectionChanged(e)));

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) => this.InvokeIfRequired((Action) (() => base.OnPropertyChanged(e)));

    private void InvokeIfRequired(Action action)
    {
      if (this.dispatcher.Thread != Thread.CurrentThread)
        this.dispatcher.Invoke(DispatcherPriority.Normal, (Delegate) action);
      else
        action();
    }
  }
}
