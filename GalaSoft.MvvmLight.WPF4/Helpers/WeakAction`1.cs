// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Helpers.WeakAction`1
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Reflection;

namespace GalaSoft.MvvmLight.Helpers
{
  public class WeakAction<T> : WeakAction, IExecuteWithObject
  {
    private Action<T> _staticAction;

    public override string MethodName => this._staticAction != null ? this._staticAction.Method.Name : this.Method.Name;

    public override bool IsAlive
    {
      get
      {
        if (this._staticAction == null && this.Reference == null)
          return false;
        if (this._staticAction == null)
          return this.Reference.IsAlive;
        return this.Reference == null || this.Reference.IsAlive;
      }
    }

    public WeakAction(Action<T> action)
      : this(action.Target, action)
    {
    }

    public WeakAction(object target, Action<T> action)
    {
      if (action.Method.IsStatic)
      {
        this._staticAction = action;
        if (target == null)
          return;
        this.Reference = new WeakReference(target);
      }
      else
      {
        this.Method = action.Method;
        this.ActionReference = new WeakReference(action.Target);
        this.Reference = new WeakReference(target);
      }
    }

    public new void Execute() => this.Execute(default (T));

    public void Execute(T parameter)
    {
      if (this._staticAction != null)
      {
        this._staticAction(parameter);
      }
      else
      {
        if (!this.IsAlive || !(this.Method != (MethodInfo) null) || this.ActionReference == null)
          return;
        this.Method.Invoke(this.ActionTarget, new object[1]
        {
          (object) parameter
        });
      }
    }

    public void ExecuteWithObject(object parameter) => this.Execute((T) parameter);

    public new void MarkForDeletion()
    {
      this._staticAction = (Action<T>) null;
      base.MarkForDeletion();
    }
  }
}
