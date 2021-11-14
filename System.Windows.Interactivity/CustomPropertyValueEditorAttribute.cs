﻿// Decompiled with JetBrains decompiler
// Type: System.Windows.Interactivity.CustomPropertyValueEditorAttribute
// Assembly: System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: AAE9A92C-FB4A-4427-A2C1-2E6256CD1F02
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Windows.Interactivity.dll

namespace System.Windows.Interactivity
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class CustomPropertyValueEditorAttribute : Attribute
  {
    public CustomPropertyValueEditor CustomPropertyValueEditor { get; private set; }

    public CustomPropertyValueEditorAttribute(
      CustomPropertyValueEditor customPropertyValueEditor)
    {
      this.CustomPropertyValueEditor = customPropertyValueEditor;
    }
  }
}
