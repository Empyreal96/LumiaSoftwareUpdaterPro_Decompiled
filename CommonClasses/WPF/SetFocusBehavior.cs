// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.WPF.SetFocusBehavior
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.LsuPro.WPF
{
  public static class SetFocusBehavior
  {
    public static readonly DependencyProperty SetFocusProperty = DependencyProperty.RegisterAttached("SetFocus", typeof (bool), typeof (SetFocusBehavior), new PropertyMetadata((object) false, new PropertyChangedCallback(SetFocusBehavior.OnSetFocusPropertyChanged)));

    public static bool GetSetFocus(DependencyObject dependencyObject) => (bool) dependencyObject.GetValue(SetFocusBehavior.SetFocusProperty);

    public static void SetSetFocus(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(SetFocusBehavior.SetFocusProperty, (object) value);

    private static void OnSetFocusPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(dependencyObject is Control control) || !(e.NewValue is bool) || !(bool) e.NewValue)
        return;
      control.Focus();
    }
  }
}
