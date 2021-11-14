// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.HttpWebResponseWrapper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.IO;
using System.Net;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class HttpWebResponseWrapper : IHttpWebResponse, IDisposable
  {
    private WebResponse response;

    public HttpWebResponseWrapper(WebResponse response) => this.response = response;

    public HttpStatusCode StatusCode => !(this.response is HttpWebResponse response) ? HttpStatusCode.NotImplemented : response.StatusCode;

    public WebHeaderCollection Headers => this.response.Headers;

    public Stream GetResponseStream() => this.response.GetResponseStream();

    public void Close() => PlatformSpecificHelper.CloseHttpWebResponse(this.response);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.response == null)
        return;
      this.response.Dispose();
      this.response = (WebResponse) null;
    }
  }
}
