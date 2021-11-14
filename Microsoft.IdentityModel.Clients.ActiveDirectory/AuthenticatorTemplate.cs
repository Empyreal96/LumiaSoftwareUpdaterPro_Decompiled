// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticatorTemplate
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [DataContract]
  internal class AuthenticatorTemplate
  {
    private const string AuthorizeEndpointTemplate = "https://{host}/{tenant}/oauth2/authorize";
    private const string MetadataTemplate = "{\"Host\":\"{host}\", \"Authority\":\"https://{host}/{tenant}/\", \"InstanceDiscoveryEndpoint\":\"https://{host}/common/discovery/instance\", \"AuthorizeEndpoint\":\"https://{host}/{tenant}/oauth2/authorize\", \"TokenEndpoint\":\"https://{host}/{tenant}/oauth2/token\", \"UserRealmEndpoint\":\"https://{host}/common/UserRealm\"}";

    public static AuthenticatorTemplate CreateFromHost(string host)
    {
      string s = "{\"Host\":\"{host}\", \"Authority\":\"https://{host}/{tenant}/\", \"InstanceDiscoveryEndpoint\":\"https://{host}/common/discovery/instance\", \"AuthorizeEndpoint\":\"https://{host}/{tenant}/oauth2/authorize\", \"TokenEndpoint\":\"https://{host}/{tenant}/oauth2/token\", \"UserRealmEndpoint\":\"https://{host}/common/UserRealm\"}".Replace("{host}", host);
      AuthenticatorTemplate authenticatorTemplate;
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
      {
        authenticatorTemplate = (AuthenticatorTemplate) new DataContractJsonSerializer(typeof (AuthenticatorTemplate)).ReadObject((Stream) memoryStream);
        authenticatorTemplate.Issuer = authenticatorTemplate.TokenEndpoint;
      }
      return authenticatorTemplate;
    }

    [DataMember]
    public string Host { get; internal set; }

    [DataMember]
    public string Issuer { get; internal set; }

    [DataMember]
    public string Authority { get; internal set; }

    [DataMember]
    public string InstanceDiscoveryEndpoint { get; internal set; }

    [DataMember]
    public string AuthorizeEndpoint { get; internal set; }

    [DataMember]
    public string TokenEndpoint { get; internal set; }

    [DataMember]
    public string UserRealmEndpoint { get; internal set; }

    public async Task VerifyAnotherHostByInstanceDiscoveryAsync(
      string host,
      string tenant,
      CallState callState)
    {
      string instanceDiscoveryEndpoint = this.InstanceDiscoveryEndpoint;
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003CinstanceDiscoveryEndpoint\u003E5__1 += "?api-version=1.0&authorization_endpoint=https://{host}/{tenant}/oauth2/authorize";
      instanceDiscoveryEndpoint = instanceDiscoveryEndpoint.Replace("{host}", host);
      instanceDiscoveryEndpoint = instanceDiscoveryEndpoint.Replace("{tenant}", tenant);
      instanceDiscoveryEndpoint = HttpHelper.CheckForExtraQueryParameter(instanceDiscoveryEndpoint);
      ClientMetrics clientMetrics = new ClientMetrics();
      try
      {
        IHttpWebRequest request = NetworkPlugin.HttpWebRequestFactory.Create(instanceDiscoveryEndpoint);
        request.Method = "GET";
        HttpHelper.AddCorrelationIdHeadersToRequest(request, callState);
        AdalIdHelper.AddAsHeaders(request);
        clientMetrics.BeginClientMetricsRecord(request, callState);
        using (IHttpWebResponse response = await request.GetResponseSyncOrAsync(callState))
        {
          HttpHelper.VerifyCorrelationIdHeaderInReponse(response, callState);
          AuthenticatorTemplate.InstanceDiscoveryResponse discoveryResponse = HttpHelper.DeserializeResponse<AuthenticatorTemplate.InstanceDiscoveryResponse>(response);
          clientMetrics.SetLastError((string[]) null);
          if (discoveryResponse.TenantDiscoveryEndpoint == null)
            throw new AdalException("authority_not_in_valid_list");
        }
      }
      catch (WebException ex)
      {
        TokenResponse tokenResponse = OAuth2Response.ReadErrorResponse(ex.Response);
        clientMetrics.SetLastError(tokenResponse?.ErrorCodes);
        if (tokenResponse.Error == "invalid_instance")
          throw new AdalServiceException("authority_not_in_valid_list", ex);
        throw new AdalServiceException("authority_validation_failed", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}. {1}: {2}", (object) "Authority validation failed", (object) tokenResponse.Error, (object) tokenResponse.ErrorDescription), tokenResponse.ErrorCodes, ex);
      }
      finally
      {
        clientMetrics.EndClientMetricsRecord("instance", callState);
      }
    }

    [DataContract]
    internal sealed class InstanceDiscoveryResponse
    {
      [DataMember(Name = "tenant_discovery_endpoint")]
      public string TenantDiscoveryEndpoint { get; set; }
    }
  }
}
