// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [Serializable]
  public class AdalServiceException : AdalException
  {
    public AdalServiceException(string errorCode, string message)
      : base(errorCode, message)
    {
    }

    public AdalServiceException(string errorCode, WebException innerException)
      : base(errorCode, (Exception) innerException)
    {
    }

    public AdalServiceException(
      string errorCode,
      string message,
      string[] serviceErrorCodes,
      WebException innerException)
      : base(errorCode, message, (Exception) innerException)
    {
      IHttpWebResponse response = NetworkPlugin.HttpWebRequestFactory.CreateResponse(innerException.Response);
      this.StatusCode = response != null ? (int) response.StatusCode : 0;
      this.ServiceErrorCodes = serviceErrorCodes;
    }

    public int StatusCode { get; set; }

    public string[] ServiceErrorCodes { get; set; }

    public override string ToString() => base.ToString() + string.Format("\n\tStatusCode: {0}", (object) this.StatusCode);

    protected AdalServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.StatusCode = info.GetInt32(nameof (StatusCode));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("StatusCode", this.StatusCode);
      base.GetObjectData(info, context);
    }
  }
}
