// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.WebBrowserNavigateErrorEventArgs
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System.ComponentModel;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  public class WebBrowserNavigateErrorEventArgs : CancelEventArgs
  {
    private readonly string targetFrameName;
    private readonly string url;
    private readonly int statusCode;
    private readonly object webBrowserActiveXInstance;

    public WebBrowserNavigateErrorEventArgs(
      string url,
      string targetFrameName,
      int statusCode,
      object webBrowserActiveXInstance)
    {
      this.url = url;
      this.targetFrameName = targetFrameName;
      this.statusCode = statusCode;
      this.webBrowserActiveXInstance = webBrowserActiveXInstance;
    }

    public string TargetFrameName => this.targetFrameName;

    public string Url => this.url;

    public int StatusCode => this.statusCode;

    public object WebBrowserActiveXInstance => this.webBrowserActiveXInstance;
  }
}
