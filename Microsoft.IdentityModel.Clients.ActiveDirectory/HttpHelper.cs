// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.HttpHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class HttpHelper
  {
    public static async Task<T> SendPostRequestAndDeserializeJsonResponseAsync<T>(
      string uri,
      RequestParameters requestParameters,
      CallState callState)
    {
      ClientMetrics clientMetrics = new ClientMetrics();
      T obj;
      try
      {
        IHttpWebRequest request = NetworkPlugin.HttpWebRequestFactory.Create(uri);
        request.ContentType = "application/x-www-form-urlencoded";
        HttpHelper.AddCorrelationIdHeadersToRequest(request, callState);
        AdalIdHelper.AddAsHeaders(request);
        clientMetrics.BeginClientMetricsRecord(request, callState);
        HttpHelper.SetPostRequest(request, requestParameters, callState);
        using (IHttpWebResponse response = await request.GetResponseSyncOrAsync(callState))
        {
          HttpHelper.VerifyCorrelationIdHeaderInReponse(response, callState);
          clientMetrics.SetLastError((string[]) null);
          obj = HttpHelper.DeserializeResponse<T>(response);
        }
      }
      catch (WebException ex)
      {
        TokenResponse tokenResponse = OAuth2Response.ReadErrorResponse(ex.Response);
        clientMetrics.SetLastError(tokenResponse?.ErrorCodes);
        throw new AdalServiceException(tokenResponse.Error, tokenResponse.ErrorDescription, tokenResponse.ErrorCodes, ex);
      }
      finally
      {
        clientMetrics.EndClientMetricsRecord("token", callState);
      }
      return obj;
    }

    public static void SetPostRequest(
      IHttpWebRequest request,
      RequestParameters requestParameters,
      CallState callState,
      Dictionary<string, string> headers = null)
    {
      request.Method = "POST";
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> header in headers)
          request.Headers[header.Key] = header.Value;
      }
      request.BodyParameters = requestParameters;
    }

    public static T DeserializeResponse<T>(IHttpWebResponse response)
    {
      DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (T));
      Stream responseStream = response.GetResponseStream();
      if (responseStream == null)
        return default (T);
      using (Stream stream = responseStream)
        return (T) contractJsonSerializer.ReadObject(stream);
    }

    public static string ReadStreamContent(Stream stream)
    {
      using (StreamReader streamReader = new StreamReader(stream))
        return streamReader.ReadToEnd();
    }

    public static string CheckForExtraQueryParameter(string url)
    {
      string environmentVariable = PlatformSpecificHelper.GetEnvironmentVariable("ExtraQueryParameter");
      string str = url.IndexOf('?') > 0 ? "&" : "?";
      if (!string.IsNullOrWhiteSpace(environmentVariable))
        url += str + environmentVariable;
      return url;
    }

    public static void AddCorrelationIdHeadersToRequest(
      IHttpWebRequest request,
      CallState callState)
    {
      if (callState == null || callState.CorrelationId == Guid.Empty)
        return;
      Dictionary<string, string> headers = new Dictionary<string, string>()
      {
        {
          "client-request-id",
          callState.CorrelationId.ToString()
        },
        {
          "return-client-request-id",
          "true"
        }
      };
      HttpHelper.AddHeadersToRequest(request, headers);
    }

    public static void VerifyCorrelationIdHeaderInReponse(
      IHttpWebResponse response,
      CallState callState)
    {
      if (callState == null || callState.CorrelationId == Guid.Empty)
        return;
      WebHeaderCollection headers = response.Headers;
      foreach (string allKey in headers.AllKeys)
      {
        string str = allKey.Trim();
        if (string.Compare(str, "client-request-id", StringComparison.OrdinalIgnoreCase) == 0)
        {
          string input = headers[str].Trim();
          Guid result;
          if (!Guid.TryParse(input, out result))
          {
            Logger.Warning(callState, "Returned correlation id '{0}' is not in GUID format.", (object) input);
            break;
          }
          if (!(result != callState.CorrelationId))
            break;
          Logger.Warning(callState, "Returned correlation id '{0}' does not match the sent correlation id '{1}'", (object) input, (object) callState.CorrelationId);
          break;
        }
      }
    }

    public static void AddHeadersToRequest(
      IHttpWebRequest request,
      Dictionary<string, string> headers)
    {
      if (headers == null)
        return;
      foreach (KeyValuePair<string, string> header in headers)
        request.Headers[header.Key] = header.Value;
    }
  }
}
