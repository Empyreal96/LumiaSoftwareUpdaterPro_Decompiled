// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenHandlerBase
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal abstract class AcquireTokenHandlerBase
  {
    protected const string NullResource = "null_resource_as_optional";
    protected static readonly Task CompletedTask = (Task) Task.FromResult<bool>(false);
    private readonly TokenCache tokenCache;

    protected AcquireTokenHandlerBase(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      ClientKey clientKey,
      TokenSubjectType subjectType,
      bool callSync)
    {
      this.Authenticator = authenticator;
      this.CallState = AcquireTokenHandlerBase.CreateCallState(this.Authenticator.CorrelationId, callSync);
      Logger.Information(this.CallState, string.Format("=== Token Acquisition started:\n\tAuthority: {0}\n\tResource: {1}\n\tClientId: {2}\n\tCacheType: {3}\n\tAuthentication Target: {4}\n\t", (object) authenticator.Authority, (object) resource, (object) clientKey.ClientId, tokenCache != null ? (object) (tokenCache.GetType().FullName + string.Format(" ({0} items)", (object) tokenCache.Count)) : (object) "null", (object) subjectType));
      this.tokenCache = tokenCache;
      if (string.IsNullOrWhiteSpace(resource))
      {
        ArgumentNullException argumentNullException = new ArgumentNullException(nameof (resource));
        Logger.Error(this.CallState, (Exception) argumentNullException);
        throw argumentNullException;
      }
      this.Resource = resource != "null_resource_as_optional" ? resource : (string) null;
      this.ClientKey = clientKey;
      this.TokenSubjectType = subjectType;
      this.LoadFromCache = tokenCache != null;
      this.StoreToCache = tokenCache != null;
      this.SupportADFS = false;
    }

    internal CallState CallState { get; set; }

    protected bool SupportADFS { get; set; }

    protected Authenticator Authenticator { get; private set; }

    protected string Resource { get; set; }

    protected ClientKey ClientKey { get; private set; }

    protected TokenSubjectType TokenSubjectType { get; private set; }

    protected string UniqueId { get; set; }

    protected string DisplayableId { get; set; }

    protected UserIdentifierType UserIdentifierType { get; set; }

    protected bool LoadFromCache { get; set; }

    protected bool StoreToCache { get; set; }

    public async Task<AuthenticationResult> RunAsync()
    {
      bool notifiedBeforeAccessCache = false;
      AuthenticationResult authenticationResult;
      try
      {
        await this.PreRunAsync();
        AuthenticationResult result = (AuthenticationResult) null;
        if (this.LoadFromCache)
        {
          this.NotifyBeforeAccessCache();
          notifiedBeforeAccessCache = true;
          result = this.tokenCache.LoadFromCache(this.Authenticator.Authority, this.Resource, this.ClientKey.ClientId, this.TokenSubjectType, this.UniqueId, this.DisplayableId, this.CallState);
          if (result != null && result.AccessToken == null && result.RefreshToken != null)
          {
            result = await this.RefreshAccessTokenAsync(result);
            if (result != null)
              this.tokenCache.StoreToCache(result, this.Authenticator.Authority, this.Resource, this.ClientKey.ClientId, this.TokenSubjectType, this.CallState);
          }
        }
        if (result == null)
        {
          await this.PreTokenRequest();
          result = await this.SendTokenRequestAsync();
          this.PostTokenRequest(result);
          if (this.StoreToCache)
          {
            if (!notifiedBeforeAccessCache)
            {
              this.NotifyBeforeAccessCache();
              notifiedBeforeAccessCache = true;
            }
            this.tokenCache.StoreToCache(result, this.Authenticator.Authority, this.Resource, this.ClientKey.ClientId, this.TokenSubjectType, this.CallState);
          }
        }
        await this.PostRunAsync(result);
        authenticationResult = result;
      }
      catch (Exception ex)
      {
        Logger.Error(this.CallState, ex);
        throw;
      }
      finally
      {
        if (notifiedBeforeAccessCache)
          this.NotifyAfterAccessCache();
      }
      return authenticationResult;
    }

    public static CallState CreateCallState(Guid correlationId, bool callSync)
    {
      correlationId = correlationId != Guid.Empty ? correlationId : Guid.NewGuid();
      return new CallState(correlationId, callSync);
    }

    protected virtual Task PostRunAsync(AuthenticationResult result)
    {
      this.LogReturnedToken(result);
      return AcquireTokenHandlerBase.CompletedTask;
    }

    protected virtual async Task PreRunAsync()
    {
      await this.Authenticator.UpdateFromTemplateAsync(this.CallState);
      this.ValidateAuthorityType();
    }

    protected virtual Task PreTokenRequest() => AcquireTokenHandlerBase.CompletedTask;

    protected virtual void PostTokenRequest(AuthenticationResult result) => this.Authenticator.UpdateTenantId(result.TenantId);

    protected abstract void AddAditionalRequestParameters(RequestParameters requestParameters);

    protected virtual async Task<AuthenticationResult> SendTokenRequestAsync()
    {
      RequestParameters requestParameters = new RequestParameters(this.Resource, this.ClientKey);
      this.AddAditionalRequestParameters(requestParameters);
      return await this.SendHttpMessageAsync(requestParameters);
    }

    protected async Task<AuthenticationResult> SendTokenRequestByRefreshTokenAsync(
      string refreshToken)
    {
      RequestParameters requestParameters = new RequestParameters(this.Resource, this.ClientKey);
      requestParameters["grant_type"] = "refresh_token";
      requestParameters["refresh_token"] = refreshToken;
      return await this.SendHttpMessageAsync(requestParameters);
    }

    private async Task<AuthenticationResult> RefreshAccessTokenAsync(
      AuthenticationResult result)
    {
      AuthenticationResult newResult = (AuthenticationResult) null;
      if (this.Resource != null)
      {
        Logger.Verbose(this.CallState, "Refreshing access token...");
        try
        {
          newResult = await this.SendTokenRequestByRefreshTokenAsync(result.RefreshToken);
          this.Authenticator.UpdateTenantId(result.TenantId);
          if (newResult.IdToken == null)
            newResult.UpdateTenantAndUserInfo(result.TenantId, result.IdToken, result.UserInfo);
        }
        catch (AdalException ex)
        {
          if (ex is AdalServiceException serviceException3 && serviceException3.ErrorCode == "invalid_request")
            throw new AdalServiceException("failed_to_refresh_token", "Failed to refresh token. " + serviceException3.Message, serviceException3.ServiceErrorCodes, (WebException) serviceException3.InnerException);
          newResult = (AuthenticationResult) null;
        }
      }
      return newResult;
    }

    private async Task<AuthenticationResult> SendHttpMessageAsync(
      RequestParameters requestParameters)
    {
      string uri = HttpHelper.CheckForExtraQueryParameter(this.Authenticator.TokenUri);
      TokenResponse tokenResponse = await HttpHelper.SendPostRequestAndDeserializeJsonResponseAsync<TokenResponse>(uri, requestParameters, this.CallState);
      AuthenticationResult result = OAuth2Response.ParseTokenResponse(tokenResponse, this.CallState);
      if (result.RefreshToken == null && requestParameters.ContainsKey("refresh_token"))
      {
        result.RefreshToken = requestParameters["refresh_token"];
        Logger.Verbose(this.CallState, "Refresh token was missing from the token refresh response, so the refresh token in the request is returned instead");
      }
      result.IsMultipleResourceRefreshToken = !string.IsNullOrWhiteSpace(result.RefreshToken) && !string.IsNullOrWhiteSpace(tokenResponse.Resource);
      return result;
    }

    private void NotifyBeforeAccessCache() => this.tokenCache.OnBeforeAccess(new TokenCacheNotificationArgs()
    {
      TokenCache = this.tokenCache,
      Resource = this.Resource,
      ClientId = this.ClientKey.ClientId,
      UniqueId = this.UniqueId,
      DisplayableId = this.DisplayableId
    });

    private void NotifyAfterAccessCache() => this.tokenCache.OnAfterAccess(new TokenCacheNotificationArgs()
    {
      TokenCache = this.tokenCache,
      Resource = this.Resource,
      ClientId = this.ClientKey.ClientId,
      UniqueId = this.UniqueId,
      DisplayableId = this.DisplayableId
    });

    private void LogReturnedToken(AuthenticationResult result)
    {
      if (result.AccessToken == null)
        return;
      Logger.Information(this.CallState, "=== Token Acquisition finished successfully. An access token was retuned:\n\tAccess Token Hash: {0}\n\tRefresh Token Hash: {1}\n\tExpiration Time: {2}\n\tUser Hash: {3}\n\t", (object) PlatformSpecificHelper.CreateSha256Hash(result.AccessToken), (object) (result.RefreshToken == null ? "[No Refresh Token]" : PlatformSpecificHelper.CreateSha256Hash(result.RefreshToken)), (object) result.ExpiresOn, result.UserInfo != null ? (object) PlatformSpecificHelper.CreateSha256Hash(result.UserInfo.UniqueId) : (object) "null");
    }

    private void ValidateAuthorityType()
    {
      if (!this.SupportADFS && this.Authenticator.AuthorityType == AuthorityType.ADFS)
        throw new AdalException("invalid_authority_type", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "This method overload is not supported by '{0}'", (object) this.Authenticator.Authority));
    }
  }
}
