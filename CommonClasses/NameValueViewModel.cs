// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NameValueViewModel
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Linq.Expressions;

namespace Microsoft.LsuPro
{
  public class NameValueViewModel : ViewModelBase
  {
    private string name;
    private object value;

    public string Name
    {
      get => this.name;
      set
      {
        if (!(value != this.name))
          return;
        this.name = value;
        this.OnPropertyChanged<string>((Expression<Func<string>>) (() => this.Name));
      }
    }

    public object Value
    {
      get => this.value;
      set
      {
        if (value == this.value)
          return;
        this.value = value;
        this.OnPropertyChanged<object>((Expression<Func<object>>) (() => this.Value));
      }
    }
  }
}
