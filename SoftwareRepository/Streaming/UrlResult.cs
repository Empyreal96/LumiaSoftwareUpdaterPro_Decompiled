// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.UrlResult
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System.Runtime.Serialization;

namespace SoftwareRepository.Streaming
{
  [DataContract]
  public class UrlResult
  {
    [DataMember(Name = "fileUrl")]
    public string FileUrl { get; set; }

    [DataMember(Name = "isSelected")]
    public bool IsSelected { get; set; }

    [DataMember(Name = "testSpeed")]
    public double TestSpeed { get; set; }

    [DataMember(Name = "displayTestSpeed")]
    public string DisplayTestSpeed
    {
      get => this.TestSpeed.ToSpeedFormat();
      private set
      {
      }
    }

    [DataMember(Name = "error")]
    public string Error { get; set; }

    [DataMember(Name = "bytesRead")]
    public long BytesRead { get; set; }
  }
}
