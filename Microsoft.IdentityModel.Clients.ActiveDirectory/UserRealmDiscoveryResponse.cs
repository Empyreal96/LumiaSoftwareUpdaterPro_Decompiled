// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.UserRealmDiscoveryResponse
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [DataContract]
  internal sealed class UserRealmDiscoveryResponse
  {
    [DataMember(Name = "ver")]
    public string Version { get; set; }

    [DataMember(Name = "account_type")]
    public string AccountType { get; set; }

    [DataMember(Name = "federation_protocol")]
    public string FederationProtocol { get; set; }

    [DataMember(Name = "federation_metadata_url")]
    public string FederationMetadataUrl { get; set; }

    [DataMember(Name = "federation_active_auth_url")]
    public string FederationActiveAuthUrl { get; set; }

    internal static async Task<UserRealmDiscoveryResponse> CreateByDiscoveryAsync(
      string userRealmUri,
      string userName,
      CallState callState)
    {
      string userRealmEndpoint = userRealmUri;
      ref UserRealmDiscoveryResponse.\u003CCreateByDiscoveryAsync\u003Ed__0 local = this;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      local.\u003CuserRealmEndpoint\u003E5__1 = local.\u003CuserRealmEndpoint\u003E5__1 + userName + "?api-version=1.0";
      userRealmEndpoint = HttpHelper.CheckForExtraQueryParameter(userRealmEndpoint);
      Logger.Information(callState, "Sending user realm discovery request to '{0}'", (object) userRealmEndpoint);
      ClientMetrics clientMetrics = new ClientMetrics();
      UserRealmDiscoveryResponse userRealmResponse;
      try
      {
        IHttpWebRequest request = NetworkPlugin.HttpWebRequestFactory.Create(userRealmEndpoint);
        request.Method = "GET";
        request.Accept = "application/json";
        HttpHelper.AddCorrelationIdHeadersToRequest(request, callState);
        AdalIdHelper.AddAsHeaders(request);
        clientMetrics.BeginClientMetricsRecord(request, callState);
        using (IHttpWebResponse response = await request.GetResponseSyncOrAsync(callState))
        {
          HttpHelper.VerifyCorrelationIdHeaderInReponse(response, callState);
          userRealmResponse = HttpHelper.DeserializeResponse<UserRealmDiscoveryResponse>(response);
          clientMetrics.SetLastError((string[]) null);
        }
      }
      catch (WebException ex)
      {
        AdalServiceException serviceException = new AdalServiceException("user_realm_discovery_failed", ex);
        clientMetrics.SetLastError(new string[1]
        {
          serviceException.StatusCode.ToString()
        });
        throw serviceException;
      }
      finally
      {
        clientMetrics.EndClientMetricsRecord("user_realm", callState);
      }
      return userRealmResponse;
    }
  }
}
