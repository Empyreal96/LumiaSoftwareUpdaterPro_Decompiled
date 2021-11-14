// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.DownloadException
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SoftwareRepository
{
  [Serializable]
  public class DownloadException : Exception
  {
    public const int Undefined = 0;
    public const int DownloadInterrupted = 507;
    public const int DownloadInterruptedDiskFull = 508;
    public const int FileIntegrityError = 417;
    public const int FileNotFound = 404;
    public const int IncorrectFileSize = 412;
    public const int RequestTimeout = 408;
    public const int UnknownError = 999;

    public DownloadException() => this.StatusCode = 0;

    public DownloadException(int statusCode) => this.StatusCode = statusCode;

    public DownloadException(string message)
      : base(message)
    {
      this.StatusCode = 0;
    }

    public DownloadException(int statusCode, string message)
      : base(message)
    {
      this.StatusCode = statusCode;
    }

    public DownloadException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.StatusCode = 0;
    }

    public DownloadException(int statusCode, string message, Exception innerException)
      : base(message, innerException)
    {
      this.StatusCode = statusCode;
    }

    protected DownloadException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public int StatusCode { get; set; }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
  }
}
