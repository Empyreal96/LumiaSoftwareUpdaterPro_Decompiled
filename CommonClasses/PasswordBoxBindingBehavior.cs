// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.PasswordBoxBindingBehavior
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.LsuPro
{
  public static class PasswordBoxBindingBehavior
  {
    public static readonly DependencyProperty BindProperty = DependencyProperty.RegisterAttached("OnBindPropertyChanged", typeof (bool), typeof (PasswordBoxBindingBehavior), new PropertyMetadata((object) false, new PropertyChangedCallback(PasswordBoxBindingBehavior.OnBindPropertyChanged)));
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof (string), typeof (PasswordBoxBindingBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, new PropertyChangedCallback(PasswordBoxBindingBehavior.OnPasswordPropertyChanged)));
    private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof (bool), typeof (PasswordBoxBindingBehavior));

    public static bool GetBind(DependencyObject dependencyObject) => (bool) dependencyObject.GetValue(PasswordBoxBindingBehavior.BindProperty);

    public static void SetBind(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(PasswordBoxBindingBehavior.BindProperty, (object) value);

    private static void OnBindPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is PasswordBox passwordBox) || !(e.NewValue is bool))
        return;
      if ((bool) e.OldValue)
        passwordBox.PasswordChanged -= new RoutedEventHandler(PasswordBoxBindingBehavior.OnPasswordChanged);
      if (!(bool) e.NewValue)
        return;
      passwordBox.PasswordChanged += new RoutedEventHandler(PasswordBoxBindingBehavior.OnPasswordChanged);
    }

    public static string GetPassword(DependencyObject dependencyObject) => dependencyObject.GetValue(PasswordBoxBindingBehavior.PasswordProperty) as string;

    public static void SetPassword(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(PasswordBoxBindingBehavior.PasswordProperty, (object) value);

    private static void OnPasswordPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is PasswordBox passwordBox) || !(e.NewValue is string))
        return;
      passwordBox.PasswordChanged -= new RoutedEventHandler(PasswordBoxBindingBehavior.OnPasswordChanged);
      if (!PasswordBoxBindingBehavior.GetIsUpdating((DependencyObject) passwordBox))
        passwordBox.Password = e.NewValue as string;
      passwordBox.PasswordChanged += new RoutedEventHandler(PasswordBoxBindingBehavior.OnPasswordChanged);
    }

    private static bool GetIsUpdating(DependencyObject dependencyObject) => (bool) dependencyObject.GetValue(PasswordBoxBindingBehavior.IsUpdatingProperty);

    private static void SetIsUpdating(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(PasswordBoxBindingBehavior.IsUpdatingProperty, (object) value);

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
      if (!(sender is PasswordBox passwordBox))
        return;
      PasswordBoxBindingBehavior.SetIsUpdating((DependencyObject) passwordBox, true);
      PasswordBoxBindingBehavior.SetPassword((DependencyObject) passwordBox, passwordBox.Password);
      PasswordBoxBindingBehavior.SetIsUpdating((DependencyObject) passwordBox, false);
    }
  }
}
