// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.NvItemReadResponse
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro.Helpers
{
  public class NvItemReadResponse
  {
    [JsonProperty("jsonrpc")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public string JsonRpc => "2.0";

    [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
    public NvReadData Result { get; set; }

    [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
    public object Error { get; set; }

    [JsonProperty("id")]
    public object ID { get; set; }
  }
}
