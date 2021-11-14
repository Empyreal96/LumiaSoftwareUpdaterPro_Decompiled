// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FocusFirstBehavior
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Windows;
using System.Windows.Input;

namespace Microsoft.LsuPro
{
  public static class FocusFirstBehavior
  {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof (bool), typeof (FrameworkElement), new PropertyMetadata((object) false, new PropertyChangedCallback(FocusFirstBehavior.OnIsEnabledPropertyChanged)));

    public static bool GetIsEnabled(DependencyObject dependencyObject) => (bool) dependencyObject.GetValue(FocusFirstBehavior.IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(FocusFirstBehavior.IsEnabledProperty, (object) value);

    private static void OnIsEnabledPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
      if (frameworkElement == null || !(e.NewValue is bool) || !(bool) e.NewValue)
        return;
      frameworkElement.Loaded += (RoutedEventHandler) ((sender, args) => frameworkElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)));
    }
  }
}
