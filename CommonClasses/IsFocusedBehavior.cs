// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.IsFocusedBehavior
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Windows;

namespace Microsoft.LsuPro
{
  public static class IsFocusedBehavior
  {
    public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof (bool), typeof (IsFocusedBehavior), new PropertyMetadata((object) false, new PropertyChangedCallback(IsFocusedBehavior.OnIsFocusedPropertyChanged)));

    public static bool GetIsFocused(DependencyObject dependencyObject) => (bool) dependencyObject.GetValue(IsFocusedBehavior.IsFocusedProperty);

    public static void SetIsFocused(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(IsFocusedBehavior.IsFocusedProperty, (object) value);

    private static void OnIsFocusedPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = (UIElement) dependencyObject;
      if (uiElement == null || !(e.NewValue is bool) || !(bool) e.NewValue)
        return;
      uiElement.Focus();
    }
  }
}
