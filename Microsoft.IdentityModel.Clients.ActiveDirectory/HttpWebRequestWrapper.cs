// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.HttpWebRequestWrapper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class HttpWebRequestWrapper : IHttpWebRequest
  {
    private readonly HttpWebRequest request;
    private int timeoutInMilliSeconds = 30000;

    public HttpWebRequestWrapper(string uri) => this.request = (HttpWebRequest) WebRequest.Create(uri);

    public RequestParameters BodyParameters { get; set; }

    public string Accept
    {
      set => this.request.Accept = value;
    }

    public string ContentType
    {
      set => this.request.ContentType = value;
    }

    public string Method
    {
      set => this.request.Method = value;
    }

    public bool UseDefaultCredentials
    {
      set => this.request.UseDefaultCredentials = value;
    }

    public WebHeaderCollection Headers => this.request.Headers;

    public int TimeoutInMilliSeconds
    {
      set => this.timeoutInMilliSeconds = value;
    }

    public async Task<IHttpWebResponse> GetResponseSyncOrAsync(
      CallState callState)
    {
      if (this.BodyParameters != null)
      {
        using (Stream stream = await this.GetRequestStreamSyncOrAsync(callState))
          this.BodyParameters.WriteToStream(stream);
      }
      if (callState != null && callState.CallSync)
      {
        this.request.Timeout = this.timeoutInMilliSeconds;
        return NetworkPlugin.HttpWebRequestFactory.CreateResponse(this.request.GetResponse());
      }
      Task<WebResponse> getResponseTask = this.request.GetResponseAsync();
      ThreadPool.RegisterWaitForSingleObject(((IAsyncResult) getResponseTask).AsyncWaitHandle, (WaitOrTimerCallback) ((state, timedOut) =>
      {
        if (!timedOut)
          return;
        ((WebRequest) state).Abort();
      }), (object) this.request, this.timeoutInMilliSeconds, true);
      return NetworkPlugin.HttpWebRequestFactory.CreateResponse(await getResponseTask);
    }

    public async Task<Stream> GetRequestStreamSyncOrAsync(CallState callState) => callState != null && callState.CallSync ? this.request.GetRequestStream() : await this.request.GetRequestStreamAsync();
  }
}
