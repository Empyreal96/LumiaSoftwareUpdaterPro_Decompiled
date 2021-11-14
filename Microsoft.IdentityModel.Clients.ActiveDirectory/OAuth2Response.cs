// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.OAuth2Response
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class OAuth2Response
  {
    public static AuthenticationResult ParseTokenResponse(
      TokenResponse tokenResponse,
      CallState callState)
    {
      if (tokenResponse.AccessToken != null)
      {
        DateTimeOffset expiresOn = (DateTimeOffset) (DateTime.UtcNow + TimeSpan.FromSeconds((double) tokenResponse.ExpiresIn));
        AuthenticationResult authenticationResult = new AuthenticationResult(tokenResponse.TokenType, tokenResponse.AccessToken, tokenResponse.RefreshToken, expiresOn)
        {
          Resource = tokenResponse.Resource
        };
        IdToken idToken = OAuth2Response.ParseIdToken(tokenResponse.IdToken);
        if (idToken != null)
        {
          string tenantId = idToken.TenantId;
          string str1 = (string) null;
          string str2 = (string) null;
          if (!string.IsNullOrWhiteSpace(idToken.ObjectId))
            str1 = idToken.ObjectId;
          else if (!string.IsNullOrWhiteSpace(idToken.Subject))
            str1 = idToken.Subject;
          if (!string.IsNullOrWhiteSpace(idToken.UPN))
            str2 = idToken.UPN;
          else if (!string.IsNullOrWhiteSpace(idToken.Email))
            str2 = idToken.Email;
          string givenName = idToken.GivenName;
          string familyName = idToken.FamilyName;
          string str3 = idToken.IdentityProvider ?? idToken.Issuer;
          DateTimeOffset? nullable = new DateTimeOffset?();
          if (idToken.PasswordExpiration > 0L)
            nullable = new DateTimeOffset?((DateTimeOffset) (DateTime.UtcNow + TimeSpan.FromSeconds((double) idToken.PasswordExpiration)));
          Uri uri = (Uri) null;
          if (!string.IsNullOrEmpty(idToken.PasswordChangeUrl))
            uri = new Uri(idToken.PasswordChangeUrl);
          authenticationResult.UpdateTenantAndUserInfo(tenantId, tokenResponse.IdToken, new UserInfo()
          {
            UniqueId = str1,
            DisplayableId = str2,
            GivenName = givenName,
            FamilyName = familyName,
            IdentityProvider = str3,
            PasswordExpiresOn = nullable,
            PasswordChangeUrl = uri
          });
        }
        return authenticationResult;
      }
      if (tokenResponse.Error != null)
        throw new AdalServiceException(tokenResponse.Error, tokenResponse.ErrorDescription);
      throw new AdalServiceException("unknown_error", "Unknown error");
    }

    public static AuthorizationResult ParseAuthorizeResponse(
      string webAuthenticationResult,
      CallState callState)
    {
      AuthorizationResult authorizationResult = (AuthorizationResult) null;
      string query = new Uri(webAuthenticationResult).Query;
      if (!string.IsNullOrWhiteSpace(query))
      {
        Dictionary<string, string> keyValueList = EncodingHelper.ParseKeyValueList(query.Substring(1), '&', true, callState);
        authorizationResult = !keyValueList.ContainsKey("code") ? (!keyValueList.ContainsKey("error") ? new AuthorizationResult("authentication_failed", "The authorization server returned an invalid response") : new AuthorizationResult(keyValueList["error"], keyValueList.ContainsKey("error_description") ? keyValueList["error_description"] : (string) null)) : new AuthorizationResult(keyValueList["code"]);
      }
      return authorizationResult;
    }

    public static TokenResponse ReadErrorResponse(WebResponse response)
    {
      if (response == null)
        return new TokenResponse()
        {
          Error = "service_returned_error",
          ErrorDescription = "Service returned error. Check InnerException for more details"
        };
      Stream responseStream = response.GetResponseStream();
      if (responseStream == null)
        return new TokenResponse()
        {
          Error = "unknown_error",
          ErrorDescription = "Unknown error"
        };
      TokenResponse tokenResponse;
      try
      {
        tokenResponse = (TokenResponse) new DataContractJsonSerializer(typeof (TokenResponse)).ReadObject(responseStream);
        responseStream.Position = 0L;
      }
      catch (SerializationException ex)
      {
        responseStream.Position = 0L;
        tokenResponse = new TokenResponse()
        {
          Error = ((HttpWebResponse) response).StatusCode == HttpStatusCode.ServiceUnavailable ? "service_unavailable" : "unknown_error",
          ErrorDescription = HttpHelper.ReadStreamContent(responseStream)
        };
      }
      return tokenResponse;
    }

    private static IdToken ParseIdToken(string idToken)
    {
      IdToken idToken1 = (IdToken) null;
      if (!string.IsNullOrWhiteSpace(idToken))
      {
        string[] strArray = idToken.Split('.');
        if (strArray.Length == 3)
        {
          try
          {
            using (MemoryStream memoryStream = new MemoryStream(Base64UrlEncoder.DecodeBytes(strArray[1])))
              idToken1 = (IdToken) new DataContractJsonSerializer(typeof (IdToken)).ReadObject((Stream) memoryStream);
          }
          catch (SerializationException ex)
          {
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      return idToken1;
    }
  }
}
