// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.PropertyKey
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.DeviceInformation
{
  public struct PropertyKey : IEquatable<PropertyKey>
  {
    private readonly Guid category;
    private readonly int propertyId;

    public PropertyKey(
      int a,
      short b,
      short c,
      byte d,
      byte e,
      byte f,
      byte g,
      byte h,
      byte i,
      byte j,
      byte k,
      int propertyId)
    {
      this.category = new Guid(a, b, c, d, e, f, g, h, i, j, k);
      this.propertyId = propertyId;
    }

    [CLSCompliant(false)]
    public PropertyKey(
      uint a,
      ushort b,
      ushort c,
      byte d,
      byte e,
      byte f,
      byte g,
      byte h,
      byte i,
      byte j,
      byte k,
      int propertyId)
    {
      this.category = new Guid(a, b, c, d, e, f, g, h, i, j, k);
      this.propertyId = propertyId;
    }

    public PropertyKey(Guid category, int propertyId)
    {
      this.category = category;
      this.propertyId = propertyId;
    }

    public Guid Category => this.category;

    public int PropertyId => this.propertyId;

    public static bool operator ==(PropertyKey left, PropertyKey right) => object.Equals((object) left, (object) right);

    public static bool operator !=(PropertyKey left, PropertyKey right) => !object.Equals((object) left, (object) right);

    public override int GetHashCode() => this.category.GetHashCode() ^ this.propertyId.GetHashCode();

    public override bool Equals(object obj) => obj is PropertyKey other && this.Equals(other);

    public bool Equals(PropertyKey other) => this.category == other.category && this.propertyId == other.propertyId;

    public override string ToString() => string.Format("{{{0}}}[{1}]", (object) this.category, (object) this.propertyId);
  }
}
