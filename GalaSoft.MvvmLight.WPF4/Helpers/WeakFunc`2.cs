// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Helpers.WeakFunc`2
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Reflection;

namespace GalaSoft.MvvmLight.Helpers
{
  public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
  {
    private Func<T, TResult> _staticFunc;

    public override string MethodName => this._staticFunc != null ? this._staticFunc.Method.Name : this.Method.Name;

    public override bool IsAlive
    {
      get
      {
        if (this._staticFunc == null && this.Reference == null)
          return false;
        if (this._staticFunc == null)
          return this.Reference.IsAlive;
        return this.Reference == null || this.Reference.IsAlive;
      }
    }

    public WeakFunc(Func<T, TResult> func)
      : this(func.Target, func)
    {
    }

    public WeakFunc(object target, Func<T, TResult> func)
    {
      if (func.Method.IsStatic)
      {
        this._staticFunc = func;
        if (target == null)
          return;
        this.Reference = new WeakReference(target);
      }
      else
      {
        this.Method = func.Method;
        this.FuncReference = new WeakReference(func.Target);
        this.Reference = new WeakReference(target);
      }
    }

    public new TResult Execute() => this.Execute(default (T));

    public TResult Execute(T parameter)
    {
      if (this._staticFunc != null)
        return this._staticFunc(parameter);
      if (!this.IsAlive || !(this.Method != (MethodInfo) null) || this.FuncReference == null)
        return default (TResult);
      return (TResult) this.Method.Invoke(this.FuncTarget, new object[1]
      {
        (object) parameter
      });
    }

    public object ExecuteWithObject(object parameter) => (object) this.Execute((T) parameter);

    public new void MarkForDeletion()
    {
      this._staticFunc = (Func<T, TResult>) null;
      base.MarkForDeletion();
    }
  }
}
