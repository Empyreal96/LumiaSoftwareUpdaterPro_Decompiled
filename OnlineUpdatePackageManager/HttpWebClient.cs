// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.HttpWebClient
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.LsuPro
{
  public class HttpWebClient : WebClient
  {
    public HttpWebClient()
    {
      this.AllowAutoRedirect = true;
      this.RequestCookieContainer = new CookieContainer();
      this.Timeout = 60000;
      this.EnableCompression = true;
      HttpWebClient.IgnoreCertificateValidationErrors = System.IO.File.Exists("ignorecertificatevalidationerrors.txt") || System.IO.File.Exists(Path.Combine(SpecialFolders.Bin, "ignorecertificatevalidationerrors.txt")) || System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "ignorecertificatevalidationerrors.txt"));
      Tracer.Information("Certificate validation errors are {0}ignored", HttpWebClient.IgnoreCertificateValidationErrors ? (object) string.Empty : (object) "not ");
      ServicePointManager.Expect100Continue = false;
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(HttpWebClient.ServerCertificateValidationCallback);
    }

    public static bool ServerCertificateValidationCallback(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      if (sslPolicyErrors == SslPolicyErrors.None)
        return true;
      Tracer.Warning("Certificate validation error: {0}", (object) sslPolicyErrors);
      return HttpWebClient.IgnoreCertificateValidationErrors;
    }

    public static bool IgnoreCertificateValidationErrors { get; set; }

    public HttpWebRequest Request { get; private set; }

    public HttpWebResponse Response { get; private set; }

    public bool AllowAutoRedirect { get; set; }

    public CookieContainer RequestCookieContainer { get; set; }

    public int Timeout { get; set; }

    public string Method { get; set; }

    public bool EnableCompression { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
      this.Request = base.GetWebRequest(address) as HttpWebRequest;
      if (this.Request == null)
        return (WebRequest) null;
      if (!string.IsNullOrEmpty(this.Method))
        this.Request.Method = this.Method;
      this.Request.Timeout = this.Timeout;
      this.Request.ReadWriteTimeout = this.Timeout;
      this.Request.AllowAutoRedirect = this.AllowAutoRedirect;
      this.Request.CookieContainer = this.RequestCookieContainer;
      if (this.EnableCompression)
        this.Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      return (WebRequest) this.Request;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
      try
      {
        this.Response = base.GetWebResponse((WebRequest) this.Request) as HttpWebResponse;
      }
      catch (WebException ex)
      {
        if (ex.Response == null)
          throw;
        else
          this.Response = ex.Response as HttpWebResponse;
      }
      return (WebResponse) this.Response;
    }

    protected override WebResponse GetWebResponse(
      WebRequest request,
      IAsyncResult result)
    {
      try
      {
        this.Response = base.GetWebResponse(request, result) as HttpWebResponse;
      }
      catch (WebException ex)
      {
        if (ex.Response == null)
          throw;
        else
          this.Response = ex.Response as HttpWebResponse;
      }
      return (WebResponse) this.Response;
    }

    public void AddRequestCookie(Cookie cookie) => this.RequestCookieContainer.Add(cookie);

    public Cookie GetResponseCookie(string cookieName) => this.Response == null ? (Cookie) null : this.Response.Cookies[cookieName];
  }
}
