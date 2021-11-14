// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemInformation
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro
{
  public class NvItemInformation
  {
    private List<NvItemMember> members;

    public NvItemInformation()
    {
      this.Identifier = string.Empty;
      this.Name = string.Empty;
      this.members = new List<NvItemMember>();
      this.SubscriptionId = string.Empty;
    }

    public NvItemInformation(NvItemInformation other)
    {
      this.Identifier = other.Identifier;
      this.Name = other.Name;
      this.Description = other.Description;
      this.RawData = other.RawData;
      this.members = other.members;
    }

    public string Identifier { get; internal set; }

    public string SubscriptionId { get; internal set; }

    public ItemType NvItemType => !this.IsEfsItem ? ItemType.NV : ItemType.EFS;

    public string Name { get; internal set; }

    public string DisplayName { get; internal set; }

    public string Description { get; internal set; }

    public byte[] RawData { get; internal set; }

    public bool IsEfsItem { get; internal set; }

    public IDictionary<int, bool> BitmaskStates { get; internal set; }

    public bool HasValueDescriptions { get; internal set; }

    public IDictionary<int, string> ValueDescriptionValueNames { get; internal set; }

    public IDictionary<int, string> ValueDescriptions { get; internal set; }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is NvItemInformation nvItemInformation) || this.Identifier != nvItemInformation.Identifier)
        return false;
      if (this.RawData == null && nvItemInformation.RawData == null)
        return true;
      if (this.RawData.Length != nvItemInformation.RawData.Length)
        return false;
      for (int index = 0; index < this.RawData.Length; ++index)
      {
        if (!this.RawData[index].Equals(nvItemInformation.RawData[index]))
          return false;
      }
      return true;
    }

    public override int GetHashCode() => this.Identifier.GetHashCode();

    public IList<NvItemMember> GetMembers() => (IList<NvItemMember>) this.members;

    internal void AddMember(NvItemMember member) => this.members.Add(member);
  }
}
