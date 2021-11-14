// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalErrorMessage
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class AdalErrorMessage
  {
    public const string AccessingMetadataDocumentFailed = "Accessing WS metadata exchange failed";
    public const string AssemblyLoadFailedTemplate = "Loading an assembly required for interactive user authentication failed. Make sure assembly '{0}' exists";
    public const string AuthenticationUiFailed = "The browser based authentication dialog failed to complete";
    public const string AuthorityInvalidUriFormat = "'authority' should be in Uri format";
    public const string AuthorityNotInValidList = "'authority' is not in the list of valid addresses";
    public const string AuthorityValidationFailed = "Authority validation failed";
    public const string AuthorityUriInsecure = "'authority' should use the 'https' scheme";
    public const string AuthorityUriInvalidPath = "'authority' Uri should have at least one segment in the path (i.e. https://<host>/<path>/...)";
    public const string AuthorizationServerInvalidResponse = "The authorization server returned an invalid response";
    public const string CertificateKeySizeTooSmallTemplate = "The certificate used must have a key size of at least {0} bits";
    public const string EmailAddressSuffixMismatch = "No identity provider email address suffix matches the provided address";
    public const string EncodedTokenTooLong = "Encoded token size is beyond the upper limit";
    public const string FailedToAcquireTokenSilently = "Failed to acquire token silently. Call method AcquireToken";
    public const string FailedToRefreshToken = "Failed to refresh token";
    public const string FederatedServiceReturnedErrorTemplate = "Federated service at {0} returned error: {1}";
    public const string IdentityProtocolLoginUrlNull = "The LoginUrl property in identityProvider cannot be null";
    public const string IdentityProtocolMismatch = "No identity provider matches the requested protocol";
    public const string IdentityProviderRequestFailed = "Token request to identity provider failed. Check InnerException for more details";
    public const string InvalidArgumentLength = "Parameter has invalid length";
    public const string InvalidAuthenticateHeaderFormat = "Invalid authenticate header format";
    public const string InvalidAuthorityTypeTemplate = "This method overload is not supported by '{0}'";
    public const string InvalidCredentialType = "Invalid credential type";
    public const string InvalidFormatParameterTemplate = "Parameter '{0}' has invalid format";
    public const string InvalidTokenCacheKeyFormat = "Invalid token cache key format";
    public const string MissingAuthenticateHeader = "WWW-Authenticate header was expected in the response";
    public const string MultipleTokensMatched = "The cache contains multiple tokens satisfying the requirements. Call AcquireToken again providing more requirements (e.g. UserId)";
    public const string NetworkIsNotAvailable = "The network is down so authentication cannot proceed";
    public const string NoDataFromSTS = "No data received from security token service";
    public const string NullParameterTemplate = "Parameter '{0}' cannot be null";
    public const string ParsingMetadataDocumentFailed = "Parsing WS metadata exchange failed";
    public const string ParsingWsTrustResponseFailed = "Parsing WS-Trust response failed";
    public const string PasswordRequiredForManagedUserError = "Password is required for managed user";
    public const string RedirectUriContainsFragment = "'redirectUri' must NOT include a fragment component";
    public const string ServiceReturnedError = "Service returned error. Check InnerException for more details";
    public const string StsMetadataRequestFailed = "Metadata request to Access Control service failed. Check InnerException for more details";
    public const string StsTokenRequestFailed = "Token request to security token service failed.  Check InnerException for more details";
    public const string UnauthorizedHttpStatusCodeExpected = "Unauthorized Http Status Code (401) was expected in the response";
    public const string UnauthorizedResponseExpected = "Unauthorized http response (status code 401) was expected";
    public const string UnexpectedAuthorityValidList = "Unexpected list of valid addresses";
    public const string Unknown = "Unknown error";
    public const string UnknownUser = "Could not identify logged in user";
    public const string UnknownUserType = "Unknown User Type";
    public const string UnsupportedAuthorityValidation = "Authority validation is not supported for this type of authority";
    public const string UnsupportedMultiRefreshToken = "This authority does not support refresh token for multiple resources. Pass null as a resource";
    public const string AuthenticationCanceled = "User canceled authentication";
    public const string UserMismatch = "User '{0}' returned by service does not match user '{1}' in the request";
    public const string UserCredentialAssertionTypeEmpty = "credential.AssertionType cannot be empty";
    public const string UserInteractionRequired = "One of two conditions was encountered: 1. The PromptBehavior.Never flag was passed, but the constraint could not be honored, because user interaction was required. 2. An error occurred during a silent web authentication that prevented the http authentication flow from completing in a short enough time frame";
    public const string UserRealmDiscoveryFailed = "User realm discovery failed";
    public const string WsTrustEndpointNotFoundInMetadataDocument = "WS-Trust endpoint not found in metadata document";
    public const string GetUserNameFailed = "Failed to get user name";
    public const string MissingFederationMetadataUrl = "Federation Metadata Url is missing for federated user. This user type is unsupported.";
    public const string SpecifyAnyUser = "If you do not need access token for any specific user, pass userId=UserIdentifier.AnyUser instead of userId=null.";
    public const string IntegratedAuthFailed = "Integrated authentication failed. You may try an alternative authentication method";
    public const string DuplicateQueryParameterTemplate = "Duplicate query parameter '{0}' in extraQueryParameters";
  }
}
