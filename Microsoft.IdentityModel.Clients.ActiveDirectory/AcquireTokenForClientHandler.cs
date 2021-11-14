// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenForClientHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenForClientHandler : AcquireTokenHandlerBase
  {
    public AcquireTokenForClientHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      ClientKey clientKey,
      bool callSync)
      : base(authenticator, tokenCache, resource, clientKey, TokenSubjectType.Client, callSync)
    {
      this.SupportADFS = true;
    }

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters) => requestParameters["grant_type"] = "client_credentials";
  }
}
