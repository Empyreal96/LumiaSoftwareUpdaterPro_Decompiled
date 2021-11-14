// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenByAuthorizationCodeHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenByAuthorizationCodeHandler : AcquireTokenHandlerBase
  {
    private readonly string authorizationCode;
    private readonly Uri redirectUri;

    public AcquireTokenByAuthorizationCodeHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      ClientKey clientKey,
      string authorizationCode,
      Uri redirectUri,
      bool callSync)
      : base(authenticator, tokenCache, resource ?? "null_resource_as_optional", clientKey, TokenSubjectType.UserPlusClient, callSync)
    {
      this.authorizationCode = !string.IsNullOrWhiteSpace(authorizationCode) ? authorizationCode : throw new ArgumentNullException(nameof (authorizationCode));
      this.redirectUri = !(redirectUri == (Uri) null) ? redirectUri : throw new ArgumentNullException(nameof (redirectUri));
      this.LoadFromCache = false;
      this.SupportADFS = true;
    }

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
    {
      requestParameters["grant_type"] = "authorization_code";
      requestParameters["code"] = this.authorizationCode;
      requestParameters["redirect_uri"] = this.redirectUri.AbsoluteUri;
    }

    protected override void PostTokenRequest(AuthenticationResult result)
    {
      base.PostTokenRequest(result);
      this.UniqueId = result.UserInfo == null ? (string) null : result.UserInfo.UniqueId;
      this.DisplayableId = result.UserInfo == null ? (string) null : result.UserInfo.DisplayableId;
      if (result.Resource != null)
      {
        this.Resource = result.Resource;
        Logger.Verbose(this.CallState, "Resource value in the token response was used for storing tokens in the cache");
      }
      this.StoreToCache = this.StoreToCache && this.Resource != null;
    }
  }
}
