// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Reporting.DownloadReport
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoftwareRepository.Reporting
{
  [DataContract]
  internal class DownloadReport
  {
    [DataMember(Name = "api-version")]
    internal string ApiVersion { get; set; }

    [DataMember(Name = "id")]
    internal string Id { get; set; }

    [DataMember(Name = "fileName")]
    internal string FileName { get; set; }

    [DataMember(Name = "url")]
    internal List<string> Url { get; set; }

    [DataMember(Name = "status")]
    internal int Status { get; set; }

    [DataMember(Name = "time")]
    internal long Time { get; set; }

    [DataMember(Name = "size")]
    internal long Size { get; set; }

    [DataMember(Name = "connections")]
    internal int Connections { get; set; }
  }
}
