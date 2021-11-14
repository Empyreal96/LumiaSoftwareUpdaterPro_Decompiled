// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.DropDownButton
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Microsoft.LsuPro
{
  public class DropDownButton : ToggleButton
  {
    private static readonly DependencyProperty DropDownProperty = DependencyProperty.Register(nameof (DropDown), typeof (ContextMenu), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

    public DropDownButton() => this.SetBinding(ToggleButton.IsCheckedProperty, (BindingBase) new Binding("DropDown.IsOpen")
    {
      Source = (object) this
    });

    public ContextMenu DropDown
    {
      get => (ContextMenu) this.GetValue(DropDownButton.DropDownProperty);
      set => this.SetValue(DropDownButton.DropDownProperty, (object) value);
    }

    protected override void OnClick()
    {
      if (this.DropDown == null)
        return;
      this.DropDown.PlacementTarget = (UIElement) this;
      this.DropDown.Placement = PlacementMode.Bottom;
      this.DropDown.IsOpen = true;
    }
  }
}
