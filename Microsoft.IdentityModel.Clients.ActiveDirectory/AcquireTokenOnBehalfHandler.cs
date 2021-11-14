// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenOnBehalfHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenOnBehalfHandler : AcquireTokenHandlerBase
  {
    private readonly UserAssertion userAssertion;

    public AcquireTokenOnBehalfHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      ClientKey clientKey,
      UserAssertion userAssertion,
      bool callSync)
      : base(authenticator, tokenCache, resource, clientKey, TokenSubjectType.UserPlusClient, callSync)
    {
      this.userAssertion = userAssertion != null ? userAssertion : throw new ArgumentNullException(nameof (userAssertion));
      this.DisplayableId = userAssertion.UserName;
      this.SupportADFS = true;
    }

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
    {
      requestParameters["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer";
      requestParameters["assertion"] = this.userAssertion.Assertion;
      requestParameters["requested_token_use"] = "on_behalf_of";
      requestParameters["scope"] = "openid";
    }
  }
}
