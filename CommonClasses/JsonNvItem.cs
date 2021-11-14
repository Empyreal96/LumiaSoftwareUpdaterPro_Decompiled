// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.JsonNvItem
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;

namespace Microsoft.LsuPro
{
  public class JsonNvItem
  {
    [JsonProperty("method")]
    public string Method { get; set; }

    [JsonProperty("params")]
    public NvItemParameters Parameters { get; set; }
  }
}
