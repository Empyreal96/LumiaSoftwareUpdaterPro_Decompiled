// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.WPF.FilteredComboBox
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.LsuPro.WPF
{
  public class FilteredComboBox : ComboBox
  {
    public static readonly DependencyProperty MinimumStringLengthForFilteringProperty = DependencyProperty.Register(nameof (MinimumStringLengthForFiltering), typeof (int), typeof (FilteredComboBox), (PropertyMetadata) new UIPropertyMetadata((object) 3));
    private string currentFilterText = string.Empty;
    private string previousFilter;
    private int rememberSelectedIndex;

    public string CurrentFilterText
    {
      get => this.currentFilterText;
      private set => this.currentFilterText = value;
    }

    public FilteredComboBox() => this.IsTextSearchEnabled = false;

    public int MinimumStringLengthForFiltering
    {
      get => (int) this.GetValue(FilteredComboBox.MinimumStringLengthForFilteringProperty);
      set => this.SetValue(FilteredComboBox.MinimumStringLengthForFilteringProperty, (object) value);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.EditableTextBox = this.GetTemplateChild("PART_EditableTextBox") as TextBox;
    }

    protected TextBox EditableTextBox { get; set; }

    protected Predicate<object> OriginalFilterPredicate { get; set; }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      if (newValue != null)
      {
        ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) newValue);
        this.OriginalFilterPredicate = defaultView.Filter;
        defaultView.Filter += new Predicate<object>(this.FilterPredicate);
      }
      if (oldValue != null)
        CollectionViewSource.GetDefaultView((object) oldValue).Filter -= new Predicate<object>(this.FilterPredicate);
      base.OnItemsSourceChanged(oldValue, newValue);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      if (e.Key != Key.Up && e.Key != Key.Down)
      {
        if (e.Key == Key.Tab || e.Key == Key.Return)
          this.ClearFilter();
        else if (e.Key == Key.Escape)
        {
          this.ClearFilter();
          this.RefreshFilter();
          this.SelectedIndex = this.rememberSelectedIndex;
        }
        else if (this.Text != this.previousFilter)
        {
          if (this.Text.Length >= this.MinimumStringLengthForFiltering)
          {
            this.CurrentFilterText = this.Text;
            this.RefreshFilter();
            this.IsDropDownOpen = true;
            this.EditableTextBox.SelectionStart = int.MaxValue;
          }
          else
            this.CurrentFilterText = string.Empty;
        }
      }
      base.OnKeyUp(e);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Tab || e.Key == Key.Return)
        this.IsDropDownOpen = false;
      else if (e.Key == Key.Escape)
      {
        this.IsDropDownOpen = false;
        this.Text = string.Empty;
      }
      else
      {
        if (e.Key == Key.Down)
          this.IsDropDownOpen = true;
        if (this.SelectedIndex >= 0)
          this.rememberSelectedIndex = this.SelectedIndex;
        this.SelectedIndex = -1;
      }
      base.OnPreviewKeyDown(e);
      this.previousFilter = this.Text;
    }

    protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      this.ClearFilter();
      base.OnPreviewLostKeyboardFocus(e);
    }

    private void ClearFilter()
    {
      this.CurrentFilterText = string.Empty;
      this.previousFilter = string.Empty;
      this.RefreshFilter();
    }

    private void RefreshFilter()
    {
      if (this.ItemsSource == null)
        return;
      CollectionViewSource.GetDefaultView((object) this.ItemsSource).Refresh();
    }

    protected virtual bool FilterPredicate(object o)
    {
      if (o == null)
        return false;
      return string.IsNullOrWhiteSpace(this.Text) || o.ToString().ToLowerInvariant().Contains(this.Text.ToLowerInvariant());
    }
  }
}
