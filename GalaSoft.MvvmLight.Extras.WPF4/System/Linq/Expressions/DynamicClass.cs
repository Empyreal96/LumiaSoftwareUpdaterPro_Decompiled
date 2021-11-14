// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.DynamicClass
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System.Reflection;
using System.Text;

namespace System.Linq.Expressions
{
  public abstract class DynamicClass
  {
    public override string ToString()
    {
      PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      for (int index = 0; index < properties.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(", ");
        stringBuilder.Append(properties[index].Name);
        stringBuilder.Append("=");
        stringBuilder.Append(properties[index].GetValue((object) this, (object[]) null));
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
