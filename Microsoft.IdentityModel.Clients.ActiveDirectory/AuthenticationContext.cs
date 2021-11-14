// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;
using System;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class AuthenticationContext
  {
    internal Authenticator Authenticator;
    private object ownerWindow;

    static AuthenticationContext() => Logger.Information((CallState) null, string.Format("ADAL {0} with assembly version '{1}', file version '{2}' and informational version '{3}' is running...", (object) PlatformSpecificHelper.GetProductName(), (object) AdalIdHelper.GetAdalVersion(), (object) AdalIdHelper.GetAssemblyFileVersion(), (object) AdalIdHelper.GetAssemblyInformationalVersion()));

    public AuthenticationContext(string authority)
      : this(authority, AuthorityValidationType.NotProvided, TokenCache.DefaultShared)
    {
    }

    public AuthenticationContext(string authority, bool validateAuthority)
      : this(authority, validateAuthority ? AuthorityValidationType.True : AuthorityValidationType.False, TokenCache.DefaultShared)
    {
    }

    public AuthenticationContext(string authority, TokenCache tokenCache)
      : this(authority, AuthorityValidationType.NotProvided, tokenCache)
    {
    }

    public AuthenticationContext(string authority, bool validateAuthority, TokenCache tokenCache)
      : this(authority, validateAuthority ? AuthorityValidationType.True : AuthorityValidationType.False, tokenCache)
    {
    }

    private AuthenticationContext(
      string authority,
      AuthorityValidationType validateAuthority,
      TokenCache tokenCache)
    {
      this.Authenticator = new Authenticator(authority, validateAuthority != AuthorityValidationType.False);
      this.TokenCache = tokenCache;
    }

    public string Authority => this.Authenticator.Authority;

    public bool ValidateAuthority => this.Authenticator.ValidateAuthority;

    public TokenCache TokenCache { get; private set; }

    public Guid CorrelationId
    {
      get => this.Authenticator.CorrelationId;
      set => this.Authenticator.CorrelationId = value;
    }

    private async Task<AuthenticationResult> AcquireTokenCommonAsync(
      string resource,
      string clientId,
      UserCredential userCredential,
      bool callSync = false)
    {
      AcquireTokenNonInteractiveHandler handler = new AcquireTokenNonInteractiveHandler(this.Authenticator, this.TokenCache, resource, clientId, userCredential, callSync);
      return await handler.RunAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenCommonAsync(
      string resource,
      string clientId,
      UserAssertion userAssertion,
      bool callSync = false)
    {
      AcquireTokenNonInteractiveHandler handler = new AcquireTokenNonInteractiveHandler(this.Authenticator, this.TokenCache, resource, clientId, userAssertion, callSync);
      return await handler.RunAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenCommonAsync(
      string resource,
      string clientId,
      Uri redirectUri,
      PromptBehavior promptBehavior,
      UserIdentifier userId,
      string extraQueryParameters = null,
      bool callSync = false)
    {
      AcquireTokenInteractiveHandler handler = new AcquireTokenInteractiveHandler(this.Authenticator, this.TokenCache, resource, clientId, redirectUri, promptBehavior, userId, extraQueryParameters, this.CreateWebAuthenticationDialog(promptBehavior), callSync);
      return await handler.RunAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenByRefreshTokenCommonAsync(
      string refreshToken,
      ClientKey clientKey,
      string resource,
      bool callSync = false)
    {
      AcquireTokenByRefreshTokenHandler handler = new AcquireTokenByRefreshTokenHandler(this.Authenticator, this.TokenCache, resource, clientKey, refreshToken, callSync);
      return await handler.RunAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenSilentCommonAsync(
      string resource,
      ClientKey clientKey,
      UserIdentifier userId,
      bool callSync = false)
    {
      AcquireTokenSilentHandler handler = new AcquireTokenSilentHandler(this.Authenticator, this.TokenCache, resource, clientKey, userId, callSync);
      return await handler.RunAsync();
    }

    public object OwnerWindow
    {
      get => this.ownerWindow;
      set
      {
        WebUIFactory.ThrowIfUIAssemblyUnavailable();
        this.ownerWindow = value;
      }
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      string clientId,
      UserCredential userCredential)
    {
      return await this.AcquireTokenCommonAsync(resource, clientId, userCredential);
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      string clientId,
      UserAssertion userAssertion)
    {
      return await this.AcquireTokenCommonAsync(resource, clientId, userAssertion);
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      ClientCredential clientCredential)
    {
      return await this.AcquireTokenForClientCommonAsync(resource, new ClientKey(clientCredential));
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      ClientAssertionCertificate clientCertificate)
    {
      return await this.AcquireTokenForClientCommonAsync(resource, new ClientKey(clientCertificate, this.Authenticator));
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      ClientAssertion clientAssertion)
    {
      return await this.AcquireTokenForClientCommonAsync(resource, new ClientKey(clientAssertion));
    }

    public async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientCredential clientCredential)
    {
      return await this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCredential), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientCredential clientCredential,
      string resource)
    {
      return await this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCredential), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertion clientAssertion)
    {
      return await this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientAssertion), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertion clientAssertion,
      string resource)
    {
      return await this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientAssertion), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertionCertificate clientCertificate)
    {
      return await this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCertificate, this.Authenticator), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertionCertificate clientCertificate,
      string resource)
    {
      return await this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCertificate, this.Authenticator), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      string clientId)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientId), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      string clientId,
      string resource)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientId), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      ClientCredential clientCredential)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCredential), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      ClientCredential clientCredential,
      string resource)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCredential), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      ClientAssertion clientAssertion)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientAssertion), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      ClientAssertion clientAssertion,
      string resource)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientAssertion), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      ClientAssertionCertificate clientCertificate)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCertificate, this.Authenticator), (string) null);
    }

    public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(
      string refreshToken,
      ClientAssertionCertificate clientCertificate,
      string resource)
    {
      return await this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCertificate, this.Authenticator), resource);
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      ClientCredential clientCredential,
      UserAssertion userAssertion)
    {
      return await this.AcquireTokenOnBehalfCommonAsync(resource, new ClientKey(clientCredential), userAssertion);
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      ClientAssertionCertificate clientCertificate,
      UserAssertion userAssertion)
    {
      return await this.AcquireTokenOnBehalfCommonAsync(resource, new ClientKey(clientCertificate, this.Authenticator), userAssertion);
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string resource,
      ClientAssertion clientAssertion,
      UserAssertion userAssertion)
    {
      return await this.AcquireTokenOnBehalfCommonAsync(resource, new ClientKey(clientAssertion), userAssertion);
    }

    public async Task<AuthenticationResult> AcquireTokenSilentAsync(
      string resource,
      string clientId)
    {
      return await this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientId), UserIdentifier.AnyUser);
    }

    public async Task<AuthenticationResult> AcquireTokenSilentAsync(
      string resource,
      string clientId,
      UserIdentifier userId)
    {
      return await this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientId), userId);
    }

    public async Task<AuthenticationResult> AcquireTokenSilentAsync(
      string resource,
      ClientCredential clientCredential,
      UserIdentifier userId)
    {
      return await this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientCredential), userId);
    }

    public async Task<AuthenticationResult> AcquireTokenSilentAsync(
      string resource,
      ClientAssertionCertificate clientCertificate,
      UserIdentifier userId)
    {
      return await this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientCertificate, this.Authenticator), userId);
    }

    public async Task<AuthenticationResult> AcquireTokenSilentAsync(
      string resource,
      ClientAssertion clientAssertion,
      UserIdentifier userId)
    {
      return await this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientAssertion), userId);
    }

    public AuthenticationResult AcquireToken(
      string resource,
      string clientId,
      UserCredential userCredential)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenCommonAsync(resource, clientId, userCredential, true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      string clientId,
      UserAssertion userAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenCommonAsync(resource, clientId, userAssertion, true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      ClientCredential clientCredential)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenForClientCommonAsync(resource, new ClientKey(clientCredential), true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      ClientAssertionCertificate clientCertificate)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenForClientCommonAsync(resource, new ClientKey(clientCertificate, this.Authenticator), true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      ClientAssertion clientAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenForClientCommonAsync(resource, new ClientKey(clientAssertion), true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      string clientId,
      Uri redirectUri)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenCommonAsync(resource, clientId, redirectUri, PromptBehavior.Auto, UserIdentifier.AnyUser, callSync: true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      string clientId,
      Uri redirectUri,
      PromptBehavior promptBehavior)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenCommonAsync(resource, clientId, redirectUri, promptBehavior, UserIdentifier.AnyUser, callSync: true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      string clientId,
      Uri redirectUri,
      PromptBehavior promptBehavior,
      UserIdentifier userId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenCommonAsync(resource, clientId, redirectUri, promptBehavior, userId, callSync: true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      string clientId,
      Uri redirectUri,
      PromptBehavior promptBehavior,
      UserIdentifier userId,
      string extraQueryParameters)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenCommonAsync(resource, clientId, redirectUri, promptBehavior, userId, extraQueryParameters, true));
    }

    public AuthenticationResult AcquireTokenByAuthorizationCode(
      string authorizationCode,
      Uri redirectUri,
      ClientCredential clientCredential)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCredential), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByAuthorizationCode(
      string authorizationCode,
      Uri redirectUri,
      ClientCredential clientCredential,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCredential), resource, true));
    }

    public AuthenticationResult AcquireTokenByAuthorizationCode(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertion clientAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientAssertion), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByAuthorizationCode(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertion clientAssertion,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientAssertion), resource, true));
    }

    public AuthenticationResult AcquireTokenByAuthorizationCode(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertionCertificate clientCertificate)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCertificate, this.Authenticator), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByAuthorizationCode(
      string authorizationCode,
      Uri redirectUri,
      ClientAssertionCertificate clientCertificate,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByAuthorizationCodeCommonAsync(authorizationCode, redirectUri, new ClientKey(clientCertificate, this.Authenticator), resource, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      string clientId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientId), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      string clientId,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientId), resource, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      ClientCredential clientCredential)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCredential), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      ClientCredential clientCredential,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCredential), resource, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      ClientAssertion clientAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientAssertion), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      ClientAssertion clientAssertion,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientAssertion), resource, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      ClientAssertionCertificate clientCertificate)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCertificate, this.Authenticator), (string) null, true));
    }

    public AuthenticationResult AcquireTokenByRefreshToken(
      string refreshToken,
      ClientAssertionCertificate clientCertificate,
      string resource)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenByRefreshTokenCommonAsync(refreshToken, new ClientKey(clientCertificate, this.Authenticator), resource, true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      ClientCredential clientCredential,
      UserAssertion userAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenOnBehalfCommonAsync(resource, new ClientKey(clientCredential), userAssertion, true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      ClientAssertionCertificate clientCertificate,
      UserAssertion userAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenOnBehalfCommonAsync(resource, new ClientKey(clientCertificate, this.Authenticator), userAssertion, true));
    }

    public AuthenticationResult AcquireToken(
      string resource,
      ClientAssertion clientAssertion,
      UserAssertion userAssertion)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenOnBehalfCommonAsync(resource, new ClientKey(clientAssertion), userAssertion, true));
    }

    public AuthenticationResult AcquireTokenSilent(
      string resource,
      string clientId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientId), UserIdentifier.AnyUser, true));
    }

    public AuthenticationResult AcquireTokenSilent(
      string resource,
      string clientId,
      UserIdentifier userId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientId), userId, true));
    }

    public AuthenticationResult AcquireTokenSilent(
      string resource,
      ClientCredential clientCredential,
      UserIdentifier userId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientCredential), userId, true));
    }

    public AuthenticationResult AcquireTokenSilent(
      string resource,
      ClientAssertionCertificate clientCertificate,
      UserIdentifier userId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientCertificate, this.Authenticator), userId, true));
    }

    public AuthenticationResult AcquireTokenSilent(
      string resource,
      ClientAssertion clientAssertion,
      UserIdentifier userId)
    {
      return AuthenticationContext.RunAsyncTask<AuthenticationResult>(this.AcquireTokenSilentCommonAsync(resource, new ClientKey(clientAssertion), userId, true));
    }

    public Uri GetAuthorizationRequestURL(
      string resource,
      string clientId,
      Uri redirectUri,
      UserIdentifier userId,
      string extraQueryParameters)
    {
      return AuthenticationContext.RunAsyncTask<Uri>(new AcquireTokenInteractiveHandler(this.Authenticator, this.TokenCache, resource, clientId, redirectUri, PromptBehavior.Auto, userId, extraQueryParameters, (IWebUI) null, true).CreateAuthorizationUriAsync(this.CorrelationId));
    }

    private async Task<AuthenticationResult> AcquireTokenByAuthorizationCodeCommonAsync(
      string authorizationCode,
      Uri redirectUri,
      ClientKey clientKey,
      string resource,
      bool callSync = false)
    {
      AcquireTokenByAuthorizationCodeHandler handler = new AcquireTokenByAuthorizationCodeHandler(this.Authenticator, this.TokenCache, resource, clientKey, authorizationCode, redirectUri, callSync);
      return await handler.RunAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenForClientCommonAsync(
      string resource,
      ClientKey clientKey,
      bool callSync = false)
    {
      AcquireTokenForClientHandler handler = new AcquireTokenForClientHandler(this.Authenticator, this.TokenCache, resource, clientKey, callSync);
      return await handler.RunAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenOnBehalfCommonAsync(
      string resource,
      ClientKey clientKey,
      UserAssertion userAssertion,
      bool callSync = false)
    {
      AcquireTokenOnBehalfHandler handler = new AcquireTokenOnBehalfHandler(this.Authenticator, this.TokenCache, resource, clientKey, userAssertion, callSync);
      return await handler.RunAsync();
    }

    private static T RunAsyncTask<T>(Task<T> task)
    {
      try
      {
        return task.Result;
      }
      catch (AggregateException ex)
      {
        Exception exception = ex.InnerExceptions[0];
        exception = exception is AggregateException ? ((AggregateException) exception).InnerExceptions[0] : throw exception;
      }
    }

    internal IWebUI CreateWebAuthenticationDialog(PromptBehavior promptBehavior) => NetworkPlugin.WebUIFactory.Create(promptBehavior, this.ownerWindow);
  }
}
