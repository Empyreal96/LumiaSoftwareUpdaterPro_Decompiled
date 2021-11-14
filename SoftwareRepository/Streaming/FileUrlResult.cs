// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.FileUrlResult
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using SoftwareRepository.Discovery;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace SoftwareRepository.Streaming
{
  [DataContract]
  internal class FileUrlResult
  {
    [DataMember(Name = "url")]
    internal string Url { get; set; }

    [DataMember(Name = "alternateUrl")]
    internal List<string> AlternateUrl { get; set; }

    [DataMember(Name = "fileSize")]
    internal long FileSize { get; set; }

    [DataMember(Name = "checksum")]
    internal List<SoftwareFileChecksum> Checksum { get; set; }

    internal HttpStatusCode StatusCode { get; set; }

    internal List<string> GetFileUrls()
    {
      List<string> stringList = new List<string>();
      if (!string.IsNullOrEmpty(this.Url))
        stringList.Add(this.Url);
      if (this.AlternateUrl != null)
      {
        foreach (string str in this.AlternateUrl)
        {
          if (!string.IsNullOrEmpty(str))
            stringList.Add(str);
        }
      }
      return stringList;
    }
  }
}
