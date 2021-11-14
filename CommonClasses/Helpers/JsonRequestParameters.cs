// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.JsonRequestParameters
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.LsuPro.Helpers
{
  public class JsonRequestParameters
  {
    public JsonRequestParameters(
      string messageVersion,
      string id,
      string subscriptionId,
      IList<byte> nvData)
    {
      this.MessageVersion = messageVersion;
      this.Id = id;
      this.SubscriptionId = subscriptionId;
      this.NvData = nvData;
    }

    [JsonProperty("messageversion")]
    public string MessageVersion { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("subscriptionid")]
    public string SubscriptionId { get; set; }

    [JsonProperty("nvdata")]
    public IList<byte> NvData { get; set; }
  }
}
