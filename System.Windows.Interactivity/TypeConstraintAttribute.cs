﻿// Decompiled with JetBrains decompiler
// Type: System.Windows.Interactivity.TypeConstraintAttribute
// Assembly: System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: AAE9A92C-FB4A-4427-A2C1-2E6256CD1F02
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Windows.Interactivity.dll

namespace System.Windows.Interactivity
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class TypeConstraintAttribute : Attribute
  {
    public Type Constraint { get; private set; }

    public TypeConstraintAttribute(Type constraint) => this.Constraint = constraint;
  }
}
