// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.WPF.FilteredSoftwareVersionComboBox
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

namespace Microsoft.LsuPro.WPF
{
  public class FilteredSoftwareVersionComboBox : FilteredComboBox
  {
    protected override bool FilterPredicate(object o)
    {
      bool flag = false;
      if (this.OriginalFilterPredicate != null)
        flag = this.OriginalFilterPredicate(o);
      if (string.IsNullOrWhiteSpace(this.CurrentFilterText) || !flag)
        return flag;
      return o is UpdatePackageSw updatePackageSw && (this.SelectedItem is UpdatePackageSw selectedItem && this.CurrentFilterText == selectedItem.SoftwareVersion || updatePackageSw.SoftwareVersion.ToLowerInvariant().Contains(this.CurrentFilterText.ToLowerInvariant()));
    }
  }
}
