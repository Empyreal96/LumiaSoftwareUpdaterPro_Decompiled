// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.VariantSelectionCollectionViewSource
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.LsuPro
{
  public class VariantSelectionCollectionViewSource : CollectionViewSource
  {
    private bool attached;

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property != CollectionViewSource.ViewProperty || this.View == null || this.attached)
        return;
      this.attached = true;
      this.View.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, args) => this.OnPropertyChanged(new DependencyPropertyChangedEventArgs(CollectionViewSource.ViewProperty, (object) args.OldItems, (object) args.NewItems)));
    }
  }
}
