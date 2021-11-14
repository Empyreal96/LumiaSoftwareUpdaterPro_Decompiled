// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenByRefreshTokenHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenByRefreshTokenHandler : AcquireTokenHandlerBase
  {
    private readonly string refreshToken;

    public AcquireTokenByRefreshTokenHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      ClientKey clientKey,
      string refreshToken,
      bool callSync)
      : base(authenticator, tokenCache, resource ?? "null_resource_as_optional", clientKey, TokenSubjectType.UserPlusClient, callSync)
    {
      if (string.IsNullOrWhiteSpace(refreshToken))
        throw new ArgumentNullException(nameof (refreshToken));
      if (!string.IsNullOrWhiteSpace(resource) && this.Authenticator.AuthorityType != AuthorityType.AAD)
        throw new ArgumentException("This authority does not support refresh token for multiple resources. Pass null as a resource", nameof (resource));
      this.refreshToken = refreshToken;
      this.LoadFromCache = false;
      this.StoreToCache = false;
      this.SupportADFS = true;
    }

    protected override async Task<AuthenticationResult> SendTokenRequestAsync() => await this.SendTokenRequestByRefreshTokenAsync(this.refreshToken);

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
    {
    }
  }
}
