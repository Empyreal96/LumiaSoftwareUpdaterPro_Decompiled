// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.WebUI
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;
using System.Net.NetworkInformation;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal abstract class WebUI : IWebUI
  {
    protected Uri RequestUri { get; private set; }

    protected Uri CallbackUri { get; private set; }

    public object OwnerWindow { get; set; }

    public string Authenticate(Uri requestUri, Uri callbackUri)
    {
      this.RequestUri = requestUri;
      this.CallbackUri = callbackUri;
      WebUI.ThrowOnNetworkDown();
      return this.OnAuthenticate();
    }

    private static void ThrowOnNetworkDown()
    {
      if (!NetworkInterface.GetIsNetworkAvailable())
        throw new AdalException("network_not_available");
    }

    protected abstract string OnAuthenticate();
  }
}
