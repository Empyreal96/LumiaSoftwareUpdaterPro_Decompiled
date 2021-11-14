// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Helpers.WeakFunc`1
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Reflection;

namespace GalaSoft.MvvmLight.Helpers
{
  public class WeakFunc<TResult>
  {
    private Func<TResult> _staticFunc;

    protected MethodInfo Method { get; set; }

    public bool IsStatic => this._staticFunc != null;

    public virtual string MethodName => this._staticFunc != null ? this._staticFunc.Method.Name : this.Method.Name;

    protected WeakReference FuncReference { get; set; }

    protected WeakReference Reference { get; set; }

    protected WeakFunc()
    {
    }

    public WeakFunc(Func<TResult> func)
      : this(func.Target, func)
    {
    }

    public WeakFunc(object target, Func<TResult> func)
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

    public virtual bool IsAlive
    {
      get
      {
        if (this._staticFunc == null && this.Reference == null)
          return false;
        return this._staticFunc != null && this.Reference == null || this.Reference.IsAlive;
      }
    }

    public object Target => this.Reference == null ? (object) null : this.Reference.Target;

    protected object FuncTarget => this.FuncReference == null ? (object) null : this.FuncReference.Target;

    public TResult Execute()
    {
      if (this._staticFunc != null)
        return this._staticFunc();
      return this.IsAlive && this.Method != (MethodInfo) null && this.FuncReference != null ? (TResult) this.Method.Invoke(this.FuncTarget, (object[]) null) : default (TResult);
    }

    public void MarkForDeletion()
    {
      this.Reference = (WeakReference) null;
      this.FuncReference = (WeakReference) null;
      this.Method = (MethodInfo) null;
      this._staticFunc = (Func<TResult>) null;
    }
  }
}
