// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.DynamicProperty
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

namespace System.Linq.Expressions
{
  public class DynamicProperty
  {
    private string name;
    private Type type;

    public DynamicProperty(string name, Type type)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      this.name = name;
      this.type = type;
    }

    public string Name => this.name;

    public Type Type => this.type;
  }
}
