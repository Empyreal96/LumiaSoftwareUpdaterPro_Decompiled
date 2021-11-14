// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.UrlSelectionResult
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoftwareRepository.Streaming
{
  [DataContract]
  public class UrlSelectionResult
  {
    public UrlSelectionResult() => this.UrlResults = new List<UrlResult>();

    [DataMember(Name = "urlResults")]
    public List<UrlResult> UrlResults { get; set; }

    [DataMember(Name = "testBytes")]
    public long TestBytes { get; set; }
  }
}
