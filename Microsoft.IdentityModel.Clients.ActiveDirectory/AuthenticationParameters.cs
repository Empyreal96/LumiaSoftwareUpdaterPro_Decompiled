// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationParameters
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class AuthenticationParameters
  {
    private const string AuthenticateHeader = "WWW-Authenticate";
    private const string Bearer = "bearer";
    private const string AuthorityKey = "authorization_uri";
    private const string ResourceKey = "resource_id";

    public string Authority { get; set; }

    public string Resource { get; set; }

    public static AuthenticationParameters CreateFromResponseAuthenticateHeader(
      string authenticateHeader)
    {
      authenticateHeader = !string.IsNullOrWhiteSpace(authenticateHeader) ? authenticateHeader.Trim() : throw new ArgumentNullException(nameof (authenticateHeader));
      if (!authenticateHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase) || authenticateHeader.Length < "bearer".Length + 2 || !char.IsWhiteSpace(authenticateHeader["bearer".Length]))
      {
        ArgumentException argumentException = new ArgumentException("Invalid authenticate header format", nameof (authenticateHeader));
        Logger.Error((CallState) null, (Exception) argumentException);
        throw argumentException;
      }
      authenticateHeader = authenticateHeader.Substring("bearer".Length).Trim();
      Dictionary<string, string> keyValueList = EncodingHelper.ParseKeyValueList(authenticateHeader, ',', false, (CallState) null);
      AuthenticationParameters authenticationParameters = new AuthenticationParameters();
      string str;
      keyValueList.TryGetValue("authorization_uri", out str);
      authenticationParameters.Authority = str;
      keyValueList.TryGetValue("resource_id", out str);
      authenticationParameters.Resource = str;
      return authenticationParameters;
    }

    private static async Task<AuthenticationParameters> CreateFromResourceUrlCommonAsync(
      Uri resourceUrl)
    {
      CallState callState = new CallState(Guid.NewGuid(), false);
      if (resourceUrl == (Uri) null)
        throw new ArgumentNullException(nameof (resourceUrl));
      IHttpWebResponse response = (IHttpWebResponse) null;
      AuthenticationParameters authParams;
      try
      {
        IHttpWebRequest request = NetworkPlugin.HttpWebRequestFactory.Create(resourceUrl.AbsoluteUri);
        request.ContentType = "application/x-www-form-urlencoded";
        response = await request.GetResponseSyncOrAsync(callState);
        AdalException ex = new AdalException("unauthorized_response_expected");
        Logger.Error((CallState) null, (Exception) ex);
        throw ex;
      }
      catch (WebException ex)
      {
        response = NetworkPlugin.HttpWebRequestFactory.CreateResponse(ex.Response);
        if (response == null)
        {
          AdalServiceException serviceException = new AdalServiceException("Unauthorized Http Status Code (401) was expected in the response", ex);
          Logger.Error((CallState) null, (Exception) serviceException);
          throw serviceException;
        }
        authParams = AuthenticationParameters.CreateFromUnauthorizedResponseCommon(response);
      }
      finally
      {
        response?.Close();
      }
      return authParams;
    }

    private static AuthenticationParameters CreateFromUnauthorizedResponseCommon(
      IHttpWebResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      if (response.StatusCode == HttpStatusCode.Unauthorized)
      {
        if (((IEnumerable<string>) response.Headers.AllKeys).Contains<string>("WWW-Authenticate"))
          return AuthenticationParameters.CreateFromResponseAuthenticateHeader(response.Headers["WWW-Authenticate"]);
        ArgumentException argumentException = new ArgumentException("WWW-Authenticate header was expected in the response", nameof (response));
        Logger.Error((CallState) null, (Exception) argumentException);
        throw argumentException;
      }
      ArgumentException argumentException1 = new ArgumentException("Unauthorized Http Status Code (401) was expected in the response", nameof (response));
      Logger.Error((CallState) null, (Exception) argumentException1);
      throw argumentException1;
    }

    public static async Task<AuthenticationParameters> CreateFromResourceUrlAsync(
      Uri resourceUrl)
    {
      return await AuthenticationParameters.CreateFromResourceUrlCommonAsync(resourceUrl);
    }

    public static AuthenticationParameters CreateFromUnauthorizedResponse(
      HttpWebResponse response)
    {
      return AuthenticationParameters.CreateFromUnauthorizedResponseCommon(NetworkPlugin.HttpWebRequestFactory.CreateResponse((WebResponse) response));
    }
  }
}
