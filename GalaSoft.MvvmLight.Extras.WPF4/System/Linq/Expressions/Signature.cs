// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.Signature
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System.Collections.Generic;

namespace System.Linq.Expressions
{
  internal class Signature : IEquatable<Signature>
  {
    public DynamicProperty[] properties;
    public int hashCode;

    public Signature(IEnumerable<DynamicProperty> properties)
    {
      this.properties = properties.ToArray<DynamicProperty>();
      this.hashCode = 0;
      foreach (DynamicProperty property in properties)
        this.hashCode ^= property.Name.GetHashCode() ^ property.Type.GetHashCode();
    }

    public override int GetHashCode() => this.hashCode;

    public override bool Equals(object obj) => obj is Signature && this.Equals((Signature) obj);

    public bool Equals(Signature other)
    {
      if (this.properties.Length != other.properties.Length)
        return false;
      for (int index = 0; index < this.properties.Length; ++index)
      {
        if (this.properties[index].Name != other.properties[index].Name || this.properties[index].Type != other.properties[index].Type)
          return false;
      }
      return true;
    }
  }
}
