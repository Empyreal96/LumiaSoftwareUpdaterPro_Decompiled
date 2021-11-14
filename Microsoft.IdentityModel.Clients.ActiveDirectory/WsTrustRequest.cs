// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.WsTrustRequest
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class WsTrustRequest
  {
    private const int MaxExpectedMessageSize = 1024;
    private const string WsTrustEnvelopeTemplate = "<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope' xmlns:a='http://www.w3.org/2005/08/addressing' xmlns:u='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>\r\n              <s:Header>\r\n              <a:Action s:mustUnderstand='1'>http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>\r\n              <a:messageID>urn:uuid:{0}</a:messageID>\r\n              <a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo>\r\n              <a:To s:mustUnderstand='1'>{1}</a:To>\r\n              {2}\r\n              </s:Header>\r\n              <s:Body>\r\n              <trust:RequestSecurityToken xmlns:trust='http://docs.oasis-open.org/ws-sx/ws-trust/200512'>\r\n              <wsp:AppliesTo xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy'>\r\n              <a:EndpointReference>\r\n              <a:Address>{3}</a:Address>\r\n              </a:EndpointReference>\r\n              </wsp:AppliesTo>\r\n              <trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>\r\n              <trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>\r\n              </trust:RequestSecurityToken>\r\n              </s:Body>\r\n              </s:Envelope>";
    private const string DefaultAppliesTo = "urn:federation:MicrosoftOnline";

    public static async Task<WsTrustResponse> SendRequestAsync(
      Uri url,
      UserCredential credential,
      CallState callState)
    {
      IHttpWebRequest request = NetworkPlugin.HttpWebRequestFactory.Create(url.AbsoluteUri);
      request.ContentType = "application/soap+xml; charset=utf-8";
      if (credential.UserAuthType == UserAuthType.IntegratedAuth)
        WsTrustRequest.SetKerberosOption(request);
      StringBuilder messageBuilder = WsTrustRequest.BuildMessage("urn:federation:MicrosoftOnline", url.AbsoluteUri, credential);
      Dictionary<string, string> headers = new Dictionary<string, string>()
      {
        {
          "SOAPAction",
          XmlNamespace.Issue.ToString()
        }
      };
      WsTrustResponse wstResponse;
      try
      {
        HttpHelper.SetPostRequest(request, new RequestParameters(messageBuilder), callState, headers);
        IHttpWebResponse response = await request.GetResponseSyncOrAsync(callState);
        wstResponse = WsTrustResponse.CreateFromResponse(response.GetResponseStream());
      }
      catch (WebException ex1)
      {
        string str;
        try
        {
          str = WsTrustResponse.ReadErrorResponse(WsTrustResponse.ReadDocumentFromResponse(ex1.Response.GetResponseStream()), callState);
        }
        catch (AdalException ex2)
        {
          str = "See inner exception for detail.";
        }
        throw new AdalServiceException("federated_service_returned_error", string.Format("Federated service at {0} returned error: {1}", (object) url, (object) str), (string[]) null, ex1);
      }
      return wstResponse;
    }

    private static void SetKerberosOption(IHttpWebRequest request) => request.UseDefaultCredentials = true;

    private static StringBuilder BuildMessage(
      string appliesTo,
      string resource,
      UserCredential credential)
    {
      StringBuilder stringBuilder1 = WsTrustRequest.BuildSecurityHeader(credential);
      string str = Guid.NewGuid().ToString();
      StringBuilder stringBuilder2 = new StringBuilder(1024);
      stringBuilder2.AppendFormat("<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope' xmlns:a='http://www.w3.org/2005/08/addressing' xmlns:u='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>\r\n              <s:Header>\r\n              <a:Action s:mustUnderstand='1'>http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>\r\n              <a:messageID>urn:uuid:{0}</a:messageID>\r\n              <a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo>\r\n              <a:To s:mustUnderstand='1'>{1}</a:To>\r\n              {2}\r\n              </s:Header>\r\n              <s:Body>\r\n              <trust:RequestSecurityToken xmlns:trust='http://docs.oasis-open.org/ws-sx/ws-trust/200512'>\r\n              <wsp:AppliesTo xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy'>\r\n              <a:EndpointReference>\r\n              <a:Address>{3}</a:Address>\r\n              </a:EndpointReference>\r\n              </wsp:AppliesTo>\r\n              <trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>\r\n              <trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>\r\n              </trust:RequestSecurityToken>\r\n              </s:Body>\r\n              </s:Envelope>", (object) str, (object) resource, (object) stringBuilder1, (object) appliesTo);
      stringBuilder1.SecureClear();
      return stringBuilder2;
    }

    private static StringBuilder BuildSecurityHeader(UserCredential credential)
    {
      StringBuilder stringBuilder1 = new StringBuilder(1024);
      if (credential.UserAuthType == UserAuthType.UsernamePassword)
      {
        StringBuilder stringBuilder2 = new StringBuilder(1024);
        string str1 = Guid.NewGuid().ToString();
        stringBuilder2.AppendFormat("<o:UsernameToken u:Id='uuid-{0}'><o:Username>{1}</o:Username><o:Password>", (object) str1, (object) credential.UserName);
        char[] chars = (char[]) null;
        try
        {
          chars = credential.PasswordToCharArray();
          stringBuilder2.Append(chars);
        }
        finally
        {
          chars.SecureClear();
        }
        stringBuilder2.AppendFormat("</o:Password></o:UsernameToken>");
        DateTime utcNow = DateTime.UtcNow;
        string str2 = DateTimeHelper.BuildTimeString(utcNow);
        string str3 = DateTimeHelper.BuildTimeString(utcNow.AddMinutes(10.0));
        stringBuilder1.AppendFormat("<o:Security s:mustUnderstand='1' xmlns:o='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'><u:Timestamp u:Id='_0'><u:Created>{0}</u:Created><u:Expires>{1}</u:Expires></u:Timestamp>{2}</o:Security>", (object) str2, (object) str3, (object) stringBuilder2);
        stringBuilder2.SecureClear();
      }
      return stringBuilder1;
    }
  }
}
