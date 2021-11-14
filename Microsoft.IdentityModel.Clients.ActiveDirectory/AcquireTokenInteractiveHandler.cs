// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenInteractiveHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenInteractiveHandler : AcquireTokenHandlerBase
  {
    internal AuthorizationResult authorizationResult;
    private Uri redirectUri;
    private string redirectUriRequestParameter;
    private PromptBehavior promptBehavior;
    private readonly string extraQueryParameters;
    private readonly IWebUI webUi;
    private readonly UserIdentifier userId;

    public AcquireTokenInteractiveHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      string clientId,
      Uri redirectUri,
      PromptBehavior promptBehavior,
      UserIdentifier userId,
      string extraQueryParameters,
      IWebUI webUI,
      bool callSync)
      : base(authenticator, tokenCache, resource, new ClientKey(clientId), TokenSubjectType.User, callSync)
    {
      if (redirectUri == (Uri) null)
        throw new ArgumentNullException(nameof (redirectUri));
      this.redirectUri = string.IsNullOrWhiteSpace(redirectUri.Fragment) ? redirectUri : throw new ArgumentException("'redirectUri' must NOT include a fragment component", nameof (redirectUri));
      this.SetRedirectUriRequestParameter();
      this.userId = userId != null ? userId : throw new ArgumentNullException(nameof (userId), "If you do not need access token for any specific user, pass userId=UserIdentifier.AnyUser instead of userId=null.");
      this.promptBehavior = promptBehavior;
      if (!string.IsNullOrEmpty(extraQueryParameters) && extraQueryParameters[0] == '&')
        extraQueryParameters = extraQueryParameters.Substring(1);
      this.extraQueryParameters = extraQueryParameters;
      this.webUi = webUI;
      this.UniqueId = userId.UniqueId;
      this.DisplayableId = userId.DisplayableId;
      this.UserIdentifierType = userId.Type;
      this.LoadFromCache = tokenCache != null && this.promptBehavior != PromptBehavior.Always && this.promptBehavior != PromptBehavior.RefreshSession;
      this.SupportADFS = true;
    }

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
    {
      requestParameters["grant_type"] = "authorization_code";
      requestParameters["code"] = this.authorizationResult.Code;
      requestParameters["redirect_uri"] = this.redirectUriRequestParameter;
    }

    protected override void PostTokenRequest(AuthenticationResult result)
    {
      base.PostTokenRequest(result);
      if (this.DisplayableId == null && this.UniqueId == null || this.UserIdentifierType == UserIdentifierType.OptionalDisplayableId)
        return;
      string str1 = result.UserInfo == null || result.UserInfo.UniqueId == null ? "NULL" : result.UserInfo.UniqueId;
      string str2 = result.UserInfo != null ? result.UserInfo.DisplayableId : "NULL";
      if (this.UserIdentifierType == UserIdentifierType.UniqueId && string.Compare(str1, this.UniqueId, StringComparison.Ordinal) != 0)
        throw new AdalUserMismatchException(this.UniqueId, str1);
      if (this.UserIdentifierType == UserIdentifierType.RequiredDisplayableId && string.Compare(str2, this.DisplayableId, StringComparison.OrdinalIgnoreCase) != 0)
        throw new AdalUserMismatchException(this.DisplayableId, str2);
    }

    private Uri CreateAuthorizationUri(bool includeFormsAuthParam)
    {
      string loginHint = (string) null;
      if (!this.userId.IsAnyUser && (this.userId.Type == UserIdentifierType.OptionalDisplayableId || this.userId.Type == UserIdentifierType.RequiredDisplayableId))
        loginHint = this.userId.Id;
      return new Uri(HttpHelper.CheckForExtraQueryParameter(new Uri(new Uri(this.Authenticator.AuthorizationUri), "?" + (object) this.CreateAuthorizationRequest(loginHint, includeFormsAuthParam)).AbsoluteUri));
    }

    private RequestParameters CreateAuthorizationRequest(
      string loginHint,
      bool includeFormsAuthParam)
    {
      RequestParameters parameters = new RequestParameters(this.Resource, this.ClientKey);
      parameters["response_type"] = "code";
      parameters["redirect_uri"] = this.redirectUriRequestParameter;
      if (!string.IsNullOrWhiteSpace(loginHint))
        parameters["login_hint"] = loginHint;
      if (this.CallState != null && this.CallState.CorrelationId != Guid.Empty)
        parameters["client-request-id"] = this.CallState.CorrelationId.ToString();
      if (this.promptBehavior == PromptBehavior.Always)
        parameters["prompt"] = "login";
      else if (this.promptBehavior == PromptBehavior.RefreshSession)
        parameters["prompt"] = "refresh_session";
      else if (this.promptBehavior == PromptBehavior.Never)
        parameters["prompt"] = "attempt_none";
      if (includeFormsAuthParam)
        parameters["amr_values"] = "pwd";
      AdalIdHelper.AddAsQueryParameters(parameters);
      if (!string.IsNullOrWhiteSpace(this.extraQueryParameters))
      {
        foreach (KeyValuePair<string, string> keyValue in EncodingHelper.ParseKeyValueList(this.extraQueryParameters, '&', false, this.CallState))
        {
          if (parameters.ContainsKey(keyValue.Key))
            throw new AdalException("duplicate_query_parameter", string.Format("Duplicate query parameter '{0}' in extraQueryParameters", (object) keyValue.Key));
        }
        parameters.ExtraQueryParameter = this.extraQueryParameters;
      }
      return parameters;
    }

    private void VerifyAuthorizationResult()
    {
      if (this.promptBehavior == PromptBehavior.Never && this.authorizationResult.Error == "login_required")
        throw new AdalException("user_interaction_required");
      if (this.authorizationResult.Status != AuthorizationStatus.Success)
        throw new AdalServiceException(this.authorizationResult.Error, this.authorizationResult.ErrorDescription);
    }

    protected override Task PreTokenRequest()
    {
      base.PreTokenRequest();
      this.AcquireAuthorization();
      this.VerifyAuthorizationResult();
      return AcquireTokenHandlerBase.CompletedTask;
    }

    internal void AcquireAuthorization()
    {
      Action action = (Action) (() => this.authorizationResult = OAuth2Response.ParseAuthorizeResponse(this.webUi.Authenticate(this.CreateAuthorizationUri(AcquireTokenInteractiveHandler.IncludeFormsAuthParams()), this.redirectUri), this.CallState));
      if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
      {
        using (StaTaskScheduler staTaskScheduler = new StaTaskScheduler(1))
          Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, (TaskScheduler) staTaskScheduler).Wait();
      }
      else
        action();
    }

    internal static bool IncludeFormsAuthParams() => AcquireTokenInteractiveHandler.IsUserLocal() && AcquireTokenInteractiveHandler.IsDomainJoined();

    internal async Task<Uri> CreateAuthorizationUriAsync(Guid correlationId)
    {
      this.CallState.CorrelationId = correlationId;
      await this.Authenticator.UpdateFromTemplateAsync(this.CallState);
      return this.CreateAuthorizationUri(false);
    }

    private static bool IsDomainJoined()
    {
      bool flag = false;
      IntPtr domain = IntPtr.Zero;
      try
      {
        AcquireTokenInteractiveHandler.NativeMethods.NetJoinStatus status = AcquireTokenInteractiveHandler.NativeMethods.NetJoinStatus.NetSetupUnknownStatus;
        int joinInformation = AcquireTokenInteractiveHandler.NativeMethods.NetGetJoinInformation((string) null, out domain, out status);
        if (domain != IntPtr.Zero)
          AcquireTokenInteractiveHandler.NativeMethods.NetApiBufferFree(domain);
        flag = joinInformation == 0 && status == AcquireTokenInteractiveHandler.NativeMethods.NetJoinStatus.NetSetupDomainName;
      }
      catch (Exception ex)
      {
      }
      finally
      {
        IntPtr zero = IntPtr.Zero;
      }
      return flag;
    }

    private static bool IsUserLocal() => WindowsIdentity.GetCurrent().Name.Split('\\')[0].ToUpperInvariant().Equals(Environment.MachineName.ToUpperInvariant());

    private void SetRedirectUriRequestParameter() => this.redirectUriRequestParameter = this.redirectUri.AbsoluteUri;

    private static class NativeMethods
    {
      public const int ErrorSuccess = 0;

      [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern int NetGetJoinInformation(
        string server,
        out IntPtr domain,
        out AcquireTokenInteractiveHandler.NativeMethods.NetJoinStatus status);

      [DllImport("Netapi32.dll")]
      public static extern int NetApiBufferFree(IntPtr Buffer);

      public enum NetJoinStatus
      {
        NetSetupUnknownStatus,
        NetSetupUnjoined,
        NetSetupWorkgroupName,
        NetSetupDomainName,
      }
    }
  }
}
