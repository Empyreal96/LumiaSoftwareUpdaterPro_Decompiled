// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Network.JsonLocationInfo
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;

namespace Microsoft.LsuPro.Network
{
  public class JsonLocationInfo
  {
    [JsonProperty("country_name")]
    public string CountryName { get; set; }

    [JsonProperty("country_code")]
    public string CountryCode { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("ip")]
    public string Ip { get; set; }
  }
}
