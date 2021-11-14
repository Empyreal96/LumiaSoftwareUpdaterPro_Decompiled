// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalError
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public static class AdalError
  {
    public const string Unknown = "unknown_error";
    public const string InvalidArgument = "invalid_argument";
    public const string AuthenticationFailed = "authentication_failed";
    public const string AuthenticationCanceled = "authentication_canceled";
    public const string UnauthorizedResponseExpected = "unauthorized_response_expected";
    public const string AuthorityNotInValidList = "authority_not_in_valid_list";
    public const string AuthorityValidationFailed = "authority_validation_failed";
    public const string AssemblyLoadFailed = "assembly_load_failed";
    public const string InvalidOwnerWindowType = "invalid_owner_window_type";
    public const string MultipleTokensMatched = "multiple_matching_tokens_detected";
    public const string InvalidAuthorityType = "invalid_authority_type";
    public const string InvalidCredentialType = "invalid_credential_type";
    public const string InvalidServiceUrl = "invalid_service_url";
    public const string FailedToAcquireTokenSilently = "failed_to_acquire_token_silently";
    public const string CertificateKeySizeTooSmall = "certificate_key_size_too_small";
    public const string IdentityProtocolLoginUrlNull = "identity_protocol_login_url_null";
    public const string IdentityProtocolMismatch = "identity_protocol_mismatch";
    public const string EmailAddressSuffixMismatch = "email_address_suffix_mismatch";
    public const string IdentityProviderRequestFailed = "identity_provider_request_failed";
    public const string StsTokenRequestFailed = "sts_token_request_failed";
    public const string EncodedTokenTooLong = "encoded_token_too_long";
    public const string ServiceUnavailable = "service_unavailable";
    public const string ServiceReturnedError = "service_returned_error";
    public const string FederatedServiceReturnedError = "federated_service_returned_error";
    public const string StsMetadataRequestFailed = "sts_metadata_request_failed";
    public const string NoDataFromSts = "no_data_from_sts";
    public const string UserMismatch = "user_mismatch";
    public const string UnknownUserType = "unknown_user_type";
    public const string UnknownUser = "unknown_user";
    public const string UserRealmDiscoveryFailed = "user_realm_discovery_failed";
    public const string AccessingWsMetadataExchangeFailed = "accessing_ws_metadata_exchange_failed";
    public const string ParsingWsMetadataExchangeFailed = "parsing_ws_metadata_exchange_failed";
    public const string WsTrustEndpointNotFoundInMetadataDocument = "wstrust_endpoint_not_found";
    public const string ParsingWsTrustResponseFailed = "parsing_wstrust_response_failed";
    public const string NetworkNotAvailable = "network_not_available";
    public const string AuthenticationUiFailed = "authentication_ui_failed";
    public const string UserInteractionRequired = "user_interaction_required";
    public const string PasswordRequiredForManagedUserError = "password_required_for_managed_user";
    public const string GetUserNameFailed = "get_user_name_failed";
    public const string MissingFederationMetadataUrl = "missing_federation_metadata_url";
    public const string FailedToRefreshToken = "failed_to_refresh_token";
    public const string IntegratedAuthFailed = "integrated_authentication_failed";
    public const string DuplicateQueryParameter = "duplicate_query_parameter";
  }
}
