// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [Serializable]
  public class AdalException : Exception
  {
    public AdalException()
      : base("Unknown error")
    {
      this.ErrorCode = "unknown_error";
    }

    public AdalException(string errorCode)
      : base(AdalException.GetErrorMessage(errorCode))
    {
      this.ErrorCode = errorCode;
    }

    public AdalException(string errorCode, string message)
      : base(message)
    {
      this.ErrorCode = errorCode;
    }

    public AdalException(string errorCode, Exception innerException)
      : base(AdalException.GetErrorMessage(errorCode), innerException)
    {
      this.ErrorCode = errorCode;
    }

    public AdalException(string errorCode, string message, Exception innerException)
      : base(message, innerException)
    {
      this.ErrorCode = errorCode;
    }

    public string ErrorCode { get; private set; }

    public override string ToString() => base.ToString() + string.Format("\n\tErrorCode: {0}", (object) this.ErrorCode);

    protected AdalException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ErrorCode = info.GetString(nameof (ErrorCode));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("ErrorCode", (object) this.ErrorCode);
      base.GetObjectData(info, context);
    }

    private static string GetErrorMessage(string errorCode)
    {
      string str = (string) null;
      switch (errorCode)
      {
        case "invalid_credential_type":
          str = "Invalid credential type";
          break;
        case "identity_protocol_login_url_null":
          str = "The LoginUrl property in identityProvider cannot be null";
          break;
        case "identity_protocol_mismatch":
          str = "No identity provider matches the requested protocol";
          break;
        case "email_address_suffix_mismatch":
          str = "No identity provider email address suffix matches the provided address";
          break;
        case "identity_provider_request_failed":
          str = "Token request to identity provider failed. Check InnerException for more details";
          break;
      }
      if (str == null)
      {
        switch (errorCode)
        {
          case "sts_token_request_failed":
            str = "Token request to security token service failed.  Check InnerException for more details";
            break;
          case "encoded_token_too_long":
            str = "Encoded token size is beyond the upper limit";
            break;
          case "sts_metadata_request_failed":
            str = "Metadata request to Access Control service failed. Check InnerException for more details";
            break;
          case "authority_not_in_valid_list":
            str = "'authority' is not in the list of valid addresses";
            break;
          case "unknown_user_type":
            str = "Unknown User Type";
            break;
        }
      }
      if (str == null)
      {
        switch (errorCode)
        {
          case "unknown_user":
            str = "Could not identify logged in user";
            break;
          case "user_realm_discovery_failed":
            str = "User realm discovery failed";
            break;
          case "accessing_ws_metadata_exchange_failed":
            str = "Accessing WS metadata exchange failed";
            break;
          case "parsing_ws_metadata_exchange_failed":
            str = "Parsing WS metadata exchange failed";
            break;
        }
      }
      if (str == null)
      {
        switch (errorCode)
        {
          case "wstrust_endpoint_not_found":
            str = "WS-Trust endpoint not found in metadata document";
            break;
          case "parsing_wstrust_response_failed":
            str = "Parsing WS-Trust response failed";
            break;
          case "authentication_canceled":
            str = "User canceled authentication";
            break;
          case "network_not_available":
            str = "The network is down so authentication cannot proceed";
            break;
        }
      }
      if (str == null)
      {
        switch (errorCode)
        {
          case "authentication_ui_failed":
            str = "The browser based authentication dialog failed to complete";
            break;
          case "user_interaction_required":
            str = "One of two conditions was encountered: 1. The PromptBehavior.Never flag was passed, but the constraint could not be honored, because user interaction was required. 2. An error occurred during a silent web authentication that prevented the http authentication flow from completing in a short enough time frame";
            break;
          case "missing_federation_metadata_url":
            str = "Federation Metadata Url is missing for federated user. This user type is unsupported.";
            break;
          case "integrated_authentication_failed":
            str = "Integrated authentication failed. You may try an alternative authentication method";
            break;
        }
      }
      if (str == null)
      {
        switch (errorCode)
        {
          case "unauthorized_response_expected":
            str = "Unauthorized http response (status code 401) was expected";
            break;
          case "multiple_matching_tokens_detected":
            str = "The cache contains multiple tokens satisfying the requirements. Call AcquireToken again providing more requirements (e.g. UserId)";
            break;
          case "password_required_for_managed_user":
            str = "Password is required for managed user";
            break;
          case "get_user_name_failed":
            str = "Failed to get user name";
            break;
          default:
            str = "Unknown error";
            break;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", (object) errorCode, (object) str);
    }

    internal enum ErrorFormat
    {
      Json,
      Other,
    }
  }
}
