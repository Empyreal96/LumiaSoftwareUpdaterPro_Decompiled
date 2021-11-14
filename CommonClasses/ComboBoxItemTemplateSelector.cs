// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ComboBoxItemTemplateSelector
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.LsuPro
{
  public class ComboBoxItemTemplateSelector : DataTemplateSelector
  {
    public DataTemplate SelectedItemTemplate { get; set; }

    public DataTemplate DropDownItemTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container) => this.GetVisualParent(container) != null ? this.DropDownItemTemplate : this.SelectedItemTemplate;

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private ComboBoxItem GetVisualParent(DependencyObject dependencyObject)
    {
      while (true)
      {
        switch (dependencyObject)
        {
          case null:
          case ComboBoxItem _:
            goto label_3;
          default:
            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            continue;
        }
      }
label_3:
      return dependencyObject as ComboBoxItem;
    }
  }
}
