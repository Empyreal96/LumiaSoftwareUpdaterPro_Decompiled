// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Discovery.DiscoveryParameters
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoftwareRepository.Discovery
{
  [DataContract]
  public class DiscoveryParameters
  {
    public DiscoveryParameters()
      : this(DiscoveryCondition.Default)
    {
    }

    public DiscoveryParameters(DiscoveryCondition condition)
    {
      this.APIVersion = "1";
      this.Query = new DiscoveryQueryParameters();
      this.Condition = new List<string>();
      switch (condition)
      {
        case DiscoveryCondition.All:
          this.Condition.Add("all");
          break;
        case DiscoveryCondition.Latest:
          this.Condition.Add("latest");
          break;
        default:
          this.Condition.Add("default");
          break;
      }
      this.Response = new List<string>();
      this.Response.Add("default");
    }

    [DataMember(Name = "api-version")]
    public string APIVersion { get; set; }

    [DataMember(Name = "query")]
    public DiscoveryQueryParameters Query { get; set; }

    [DataMember(Name = "condition")]
    public List<string> Condition { get; set; }

    [DataMember(Name = "response")]
    public List<string> Response { get; set; }
  }
}
