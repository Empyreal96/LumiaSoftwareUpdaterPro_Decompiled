// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemParameters
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.LsuPro
{
  public class NvItemParameters
  {
    [JsonProperty("FilePath", NullValueHandling = NullValueHandling.Ignore)]
    public string FilePath { get; set; }

    [JsonProperty("ItemType", NullValueHandling = NullValueHandling.Ignore)]
    public string ItemType { get; set; }

    [JsonProperty("Data", NullValueHandling = NullValueHandling.Ignore)]
    public IList<byte> Data { get; set; }

    [JsonProperty("ID", NullValueHandling = NullValueHandling.Ignore)]
    public int Id { get; set; }

    [JsonProperty("SubscriptionId", NullValueHandling = NullValueHandling.Ignore)]
    public string SubscriptionId { get; set; }

    [JsonProperty("NVData", NullValueHandling = NullValueHandling.Ignore)]
    public IList<byte> NvData { get; set; }
  }
}
