// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Helpers.WeakAction
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Reflection;

namespace GalaSoft.MvvmLight.Helpers
{
  public class WeakAction
  {
    private Action _staticAction;

    protected MethodInfo Method { get; set; }

    public virtual string MethodName => this._staticAction != null ? this._staticAction.Method.Name : this.Method.Name;

    protected WeakReference ActionReference { get; set; }

    protected WeakReference Reference { get; set; }

    public bool IsStatic => this._staticAction != null;

    protected WeakAction()
    {
    }

    public WeakAction(Action action)
      : this(action.Target, action)
    {
    }

    public WeakAction(object target, Action action)
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

    public virtual bool IsAlive
    {
      get
      {
        if (this._staticAction == null && this.Reference == null)
          return false;
        return this._staticAction != null && this.Reference == null || this.Reference.IsAlive;
      }
    }

    public object Target => this.Reference == null ? (object) null : this.Reference.Target;

    protected object ActionTarget => this.ActionReference == null ? (object) null : this.ActionReference.Target;

    public void Execute()
    {
      if (this._staticAction != null)
      {
        this._staticAction();
      }
      else
      {
        object actionTarget = this.ActionTarget;
        if (!this.IsAlive || !(this.Method != (MethodInfo) null) || (this.ActionReference == null || actionTarget == null))
          return;
        this.Method.Invoke(this.ActionTarget, (object[]) null);
      }
    }

    public void MarkForDeletion()
    {
      this.Reference = (WeakReference) null;
      this.ActionReference = (WeakReference) null;
      this.Method = (MethodInfo) null;
      this._staticAction = (Action) null;
    }
  }
}
