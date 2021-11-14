// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenNonInteractiveHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenNonInteractiveHandler : AcquireTokenHandlerBase
  {
    private readonly UserCredential userCredential;
    private UserAssertion userAssertion;

    public AcquireTokenNonInteractiveHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      string clientId,
      UserCredential userCredential,
      bool callSync)
      : base(authenticator, tokenCache, resource, new ClientKey(clientId), TokenSubjectType.User, callSync)
    {
      this.userCredential = userCredential != null ? userCredential : throw new ArgumentNullException(nameof (userCredential));
    }

    public AcquireTokenNonInteractiveHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      string clientId,
      UserAssertion userAssertion,
      bool callSync)
      : base(authenticator, tokenCache, resource, new ClientKey(clientId), TokenSubjectType.User, callSync)
    {
      if (userAssertion == null)
        throw new ArgumentNullException(nameof (userAssertion));
      this.userAssertion = !string.IsNullOrWhiteSpace(userAssertion.AssertionType) ? userAssertion : throw new ArgumentException("credential.AssertionType cannot be empty", nameof (userAssertion));
    }

    protected override async Task PreRunAsync()
    {
      await base.PreRunAsync();
      if (this.userCredential != null)
      {
        if (string.IsNullOrWhiteSpace(this.userCredential.UserName))
        {
          this.userCredential.UserName = PlatformSpecificHelper.GetUserPrincipalName();
          if (string.IsNullOrWhiteSpace(this.userCredential.UserName))
          {
            Logger.Information(this.CallState, "Could not find UPN for logged in user");
            throw new AdalException("unknown_user");
          }
          Logger.Verbose(this.CallState, "Logged in user with hash '{0}' detected", (object) PlatformSpecificHelper.CreateSha256Hash(this.userCredential.UserName));
        }
        this.DisplayableId = this.userCredential.UserName;
      }
      else
      {
        if (this.userAssertion == null)
          return;
        this.DisplayableId = this.userAssertion.UserName;
      }
    }

    protected override async Task PreTokenRequest()
    {
      await base.PreTokenRequest();
      if (this.userAssertion != null)
        return;
      UserRealmDiscoveryResponse userRealmResponse = await UserRealmDiscoveryResponse.CreateByDiscoveryAsync(this.Authenticator.UserRealmUri, this.userCredential.UserName, this.CallState);
      Logger.Information(this.CallState, "User with hash '{0}' detected as '{1}'", (object) PlatformSpecificHelper.CreateSha256Hash(this.userCredential.UserName), (object) userRealmResponse.AccountType);
      if (string.Compare(userRealmResponse.AccountType, "federated", StringComparison.OrdinalIgnoreCase) == 0)
      {
        if (string.IsNullOrWhiteSpace(userRealmResponse.FederationMetadataUrl))
          throw new AdalException("missing_federation_metadata_url");
        Uri wsTrustUrl = await MexParser.FetchWsTrustAddressFromMexAsync(userRealmResponse.FederationMetadataUrl, this.userCredential.UserAuthType, this.CallState);
        Logger.Information(this.CallState, "WS-Trust endpoint '{0}' fetched from MEX at '{1}'", (object) wsTrustUrl, (object) userRealmResponse.FederationMetadataUrl);
        WsTrustResponse wsTrustResponse = await WsTrustRequest.SendRequestAsync(wsTrustUrl, this.userCredential, this.CallState);
        Logger.Information(this.CallState, "Token of type '{0}' acquired from WS-Trust endpoint", (object) wsTrustResponse.TokenType);
        this.userAssertion = new UserAssertion(wsTrustResponse.Token, wsTrustResponse.TokenType == "urn:oasis:names:tc:SAML:1.0:assertion" ? "urn:ietf:params:oauth:grant-type:saml1_1-bearer" : "urn:ietf:params:oauth:grant-type:saml2-bearer");
      }
      else
      {
        if (string.Compare(userRealmResponse.AccountType, "managed", StringComparison.OrdinalIgnoreCase) != 0)
          throw new AdalException("unknown_user_type");
        if (this.userCredential.PasswordToCharArray() == null)
          throw new AdalException("password_required_for_managed_user");
      }
    }

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
    {
      if (this.userAssertion != null)
      {
        requestParameters["grant_type"] = this.userAssertion.AssertionType;
        requestParameters["assertion"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.userAssertion.Assertion));
      }
      else
      {
        requestParameters["grant_type"] = "password";
        requestParameters["username"] = this.userCredential.UserName;
        if (this.userCredential.SecurePassword != null)
          requestParameters.AddSecureParameter("password", this.userCredential.SecurePassword);
        else
          requestParameters["password"] = this.userCredential.Password;
      }
      requestParameters["scope"] = "openid";
    }
  }
}
