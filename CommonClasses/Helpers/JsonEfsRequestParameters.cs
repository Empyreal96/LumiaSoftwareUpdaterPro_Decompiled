// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.JsonEfsRequestParameters
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.LsuPro.Helpers
{
  public class JsonEfsRequestParameters
  {
    public JsonEfsRequestParameters(
      string messageVersion,
      string filePath,
      IList<byte> data,
      string itemType)
    {
      this.MessageVersion = messageVersion;
      this.FilePath = filePath;
      this.Data = data;
      this.ItemType = itemType;
    }

    [JsonProperty("messageversion")]
    public string MessageVersion { get; set; }

    [JsonProperty("filepath")]
    public string FilePath { get; set; }

    [JsonProperty("data")]
    public IList<byte> Data { get; set; }

    [JsonProperty("itemtype")]
    public string ItemType { get; set; }
  }
}
