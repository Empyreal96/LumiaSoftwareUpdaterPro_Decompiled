// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.WindowBase
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.ComponentModel;
using System.Windows;

namespace Microsoft.LsuPro
{
  public class WindowBase : Window
  {
    public WindowBase() => this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChanged);

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (!(this.DataContext is ViewModelBase dataContext))
        return;
      dataContext.PropertyChanged += new PropertyChangedEventHandler(this.OnDataContextPropertyChanged);
    }

    private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!e.PropertyName.Equals("CloseWindow"))
        return;
      if (this.DataContext is ViewModelBase dataContext)
      {
        this.DialogResult = new bool?(dataContext.CloseWindow);
        dataContext.PropertyChanged -= new PropertyChangedEventHandler(this.OnDataContextPropertyChanged);
      }
      this.Close();
      this.DataContextChanged -= new DependencyPropertyChangedEventHandler(this.OnDataContextChanged);
    }
  }
}
