// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemDisplayInformation
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.ObjectModel;

namespace Microsoft.LsuPro
{
  public class NvItemDisplayInformation : ViewModelBase
  {
    public NvItemDisplayInformation(NvItemDisplayInformation information)
    {
      this.Id = information.Id;
      this.Identifier = information.Identifier;
      this.SubscriptionId = information.SubscriptionId;
      this.IsEfsItem = information.IsEfsItem;
      this.FileRawData = information.FileRawData;
      this.IsEqual = information.IsEqual;
      this.Name = information.Name;
      this.DisplayName = information.DisplayName;
      this.Description = information.Description;
      this.FileValueDescription = information.FileValueDescription;
      this.DeviceValueDescription = information.DeviceValueDescription;
      this.StringForSearching = information.StringForSearching;
      this.DeviceRawDataCollection = information.DeviceRawDataCollection;
      this.DataType = information.DataType;
      this.DeviceDataType = information.DeviceDataType;
      this.FileBitmaskStates = new Collection<ComplexValue>();
      foreach (ComplexValue fileBitmaskState in information.FileBitmaskStates)
        this.FileBitmaskStates.Add(new ComplexValue(fileBitmaskState.BitmaskState, fileBitmaskState.ValueName, fileBitmaskState.ValueDescription));
      this.DeviceBitmaskStates = information.DeviceBitmaskStates;
    }

    public NvItemDisplayInformation(
      int id,
      string identifier,
      string subscriptionId,
      bool efsItem,
      Collection<byte> fileRawData)
    {
      this.Id = id;
      this.Identifier = identifier;
      this.SubscriptionId = subscriptionId;
      this.IsEfsItem = efsItem;
      this.FileRawData = fileRawData;
    }

    public NvItemDisplayInformation(
      int id,
      string identifier,
      string subscriptionId,
      string name,
      string displayName,
      string description,
      bool efsItem,
      Collection<byte> fileRawData,
      DataType dataType,
      Collection<ComplexValue> fileBitmaskStates,
      string valueForSearching)
    {
      this.Id = id;
      this.Identifier = identifier;
      this.SubscriptionId = subscriptionId;
      this.Name = name;
      this.DisplayName = displayName;
      this.Description = description;
      this.IsEfsItem = efsItem;
      this.FileRawData = fileRawData;
      this.DataType = dataType;
      this.FileBitmaskStates = fileBitmaskStates;
      this.StringForSearching = valueForSearching;
    }

    public int Id { get; set; }

    public string Identifier { get; internal set; }

    public string SubscriptionId { get; internal set; }

    public bool IsEqual { get; set; }

    public string Name { get; internal set; }

    public string DisplayName { get; internal set; }

    public string Description { get; internal set; }

    public string FileValueDescription { get; set; }

    public string DeviceValueDescription { get; set; }

    public bool IsEfsItem { get; internal set; }

    public string StringForSearching { get; set; }

    public Collection<byte> FileRawData { get; internal set; }

    public object DeviceRawDataCollection { get; set; }

    public DataType DataType { get; internal set; }

    public DataType DeviceDataType { get; set; }

    public Collection<ComplexValue> FileBitmaskStates { get; internal set; }

    public Collection<ComplexValue> DeviceBitmaskStates { get; set; }

    public bool CanEdit => this.IsEfsItem && this.Identifier.Equals("/nv/item_files/modem/lte/rrc/cap/fgi");
  }
}
