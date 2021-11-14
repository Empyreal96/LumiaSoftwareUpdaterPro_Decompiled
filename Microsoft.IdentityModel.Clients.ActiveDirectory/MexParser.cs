// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.MexParser
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class MexParser
  {
    private const string WsTrustSoapTransport = "http://schemas.xmlsoap.org/soap/http";

    public static async Task<Uri> FetchWsTrustAddressFromMexAsync(
      string federationMetadataUrl,
      UserAuthType userAuthType,
      CallState callState)
    {
      XDocument mexDocument = await MexParser.FetchMexAsync(federationMetadataUrl, callState);
      return MexParser.ExtractWsTrustAddressFromMex(mexDocument, userAuthType, callState);
    }

    internal static async Task<XDocument> FetchMexAsync(
      string federationMetadataUrl,
      CallState callState)
    {
      XDocument mexDocument;
      try
      {
        IHttpWebRequest request = NetworkPlugin.HttpWebRequestFactory.Create(federationMetadataUrl);
        request.Method = "GET";
        request.ContentType = "application/soap+xml";
        using (IHttpWebResponse response = await request.GetResponseSyncOrAsync(callState))
          mexDocument = XDocument.Load(response.GetResponseStream(), LoadOptions.None);
      }
      catch (WebException ex)
      {
        throw new AdalServiceException("accessing_ws_metadata_exchange_failed", ex);
      }
      catch (XmlException ex)
      {
        throw new AdalException("parsing_ws_metadata_exchange_failed", (Exception) ex);
      }
      return mexDocument;
    }

    internal static Uri ExtractWsTrustAddressFromMex(
      XDocument mexDocument,
      UserAuthType userAuthType,
      CallState callState)
    {
      try
      {
        Dictionary<string, MexPolicy> dictionary1 = MexParser.ReadPolicies((XContainer) mexDocument);
        Dictionary<string, MexPolicy> dictionary2 = MexParser.ReadPolicyBindings((XContainer) mexDocument, (IReadOnlyDictionary<string, MexPolicy>) dictionary1);
        MexParser.SetPolicyEndpointAddresses((XContainer) mexDocument, (IReadOnlyDictionary<string, MexPolicy>) dictionary2);
        Random random = new Random();
        MexPolicy mexPolicy = dictionary1.Values.Where<MexPolicy>((Func<MexPolicy, bool>) (p => p.Url != (Uri) null && p.AuthType == userAuthType)).OrderBy<MexPolicy, int>((Func<MexPolicy, int>) (p => random.Next())).FirstOrDefault<MexPolicy>();
        if (mexPolicy != null)
          return mexPolicy.Url;
        if (userAuthType == UserAuthType.IntegratedAuth)
          throw new AdalException("integrated_authentication_failed", (Exception) new AdalException("wstrust_endpoint_not_found"));
        throw new AdalException("wstrust_endpoint_not_found");
      }
      catch (XmlException ex)
      {
        throw new AdalException("parsing_ws_metadata_exchange_failed", (Exception) ex);
      }
    }

    private static Dictionary<string, MexPolicy> ReadPolicies(
      XContainer mexDocument)
    {
      Dictionary<string, MexPolicy> dictionary = new Dictionary<string, MexPolicy>();
      foreach (XElement element in mexDocument.Elements().First<XElement>().Elements(XmlNamespace.Wsp + "Policy"))
      {
        XElement xelement1 = element.Elements(XmlNamespace.Wsp + "ExactlyOne").FirstOrDefault<XElement>();
        if (xelement1 != null)
        {
          foreach (XElement descendant in xelement1.Descendants(XmlNamespace.Wsp + "All"))
          {
            if (descendant.Elements(XmlNamespace.Http + "NegotiateAuthentication").FirstOrDefault<XElement>() != null)
              MexParser.AddPolicy((IDictionary<string, MexPolicy>) dictionary, element, UserAuthType.IntegratedAuth);
            XElement xelement2 = descendant.Elements(XmlNamespace.Sp + "SignedEncryptedSupportingTokens").FirstOrDefault<XElement>();
            if (xelement2 != null)
            {
              XElement xelement3 = xelement2.Elements(XmlNamespace.Wsp + "Policy").FirstOrDefault<XElement>();
              if (xelement3 != null)
              {
                XElement xelement4 = xelement3.Elements(XmlNamespace.Sp + "UsernameToken").FirstOrDefault<XElement>();
                if (xelement4 != null)
                {
                  XElement xelement5 = xelement4.Elements(XmlNamespace.Wsp + "Policy").FirstOrDefault<XElement>();
                  if (xelement5 != null && xelement5.Elements(XmlNamespace.Sp + "WssUsernameToken10").FirstOrDefault<XElement>() != null)
                    MexParser.AddPolicy((IDictionary<string, MexPolicy>) dictionary, element, UserAuthType.UsernamePassword);
                }
              }
            }
          }
        }
      }
      return dictionary;
    }

    private static Dictionary<string, MexPolicy> ReadPolicyBindings(
      XContainer mexDocument,
      IReadOnlyDictionary<string, MexPolicy> policies)
    {
      Dictionary<string, MexPolicy> dictionary = new Dictionary<string, MexPolicy>();
      foreach (XElement element1 in mexDocument.Elements().First<XElement>().Elements(XmlNamespace.Wsdl + "binding"))
      {
        foreach (XElement element2 in element1.Elements(XmlNamespace.Wsp + "PolicyReference"))
        {
          XAttribute xattribute1 = element2.Attribute((XName) "URI");
          if (xattribute1 != null && policies.ContainsKey(xattribute1.Value))
          {
            XAttribute xattribute2 = element1.Attribute((XName) "name");
            if (xattribute2 != null)
            {
              XElement xelement1 = element1.Elements(XmlNamespace.Wsdl + "operation").FirstOrDefault<XElement>();
              if (xelement1 != null)
              {
                XElement xelement2 = xelement1.Elements(XmlNamespace.Soap12 + "operation").FirstOrDefault<XElement>();
                if (xelement2 != null)
                {
                  XAttribute xattribute3 = xelement2.Attribute((XName) "soapAction");
                  if (xattribute3 != null && string.Compare(XmlNamespace.Issue.ToString(), xattribute3.Value, StringComparison.OrdinalIgnoreCase) == 0)
                  {
                    XElement xelement3 = element1.Elements(XmlNamespace.Soap12 + "binding").FirstOrDefault<XElement>();
                    if (xelement3 != null)
                    {
                      XAttribute xattribute4 = xelement3.Attribute((XName) "transport");
                      if (xattribute4 != null && string.Compare("http://schemas.xmlsoap.org/soap/http", xattribute4.Value, StringComparison.OrdinalIgnoreCase) == 0)
                        dictionary.Add(xattribute2.Value, policies[xattribute1.Value]);
                    }
                  }
                }
              }
            }
          }
        }
      }
      return dictionary;
    }

    private static void SetPolicyEndpointAddresses(
      XContainer mexDocument,
      IReadOnlyDictionary<string, MexPolicy> bindings)
    {
      foreach (XElement element in mexDocument.Elements().First<XElement>().Elements(XmlNamespace.Wsdl + "service").First<XElement>().Elements(XmlNamespace.Wsdl + "port"))
      {
        XAttribute xattribute = element.Attribute((XName) "binding");
        if (xattribute != null)
        {
          string[] strArray = xattribute.Value.Split(new char[1]
          {
            ':'
          }, 2);
          if (strArray.Length >= 2 && bindings.ContainsKey(strArray[1]))
          {
            XElement xelement1 = element.Elements(XmlNamespace.Wsa10 + "EndpointReference").FirstOrDefault<XElement>();
            if (xelement1 != null)
            {
              XElement xelement2 = xelement1.Elements(XmlNamespace.Wsa10 + "Address").FirstOrDefault<XElement>();
              if (xelement2 != null && Uri.IsWellFormedUriString(xelement2.Value, UriKind.Absolute))
                bindings[strArray[1]].Url = new Uri(xelement2.Value);
            }
          }
        }
      }
    }

    private static void AddPolicy(
      IDictionary<string, MexPolicy> policies,
      XElement policy,
      UserAuthType policyAuthType)
    {
      if ((policy.Descendants(XmlNamespace.Sp + "TransportBinding").FirstOrDefault<XElement>() ?? policy.Descendants(XmlNamespace.Sp2005 + "TransportBinding").FirstOrDefault<XElement>()) == null)
        return;
      XAttribute xattribute = policy.Attribute(XmlNamespace.Wsu + "Id");
      if (xattribute == null)
        return;
      policies.Add("#" + xattribute.Value, new MexPolicy()
      {
        Id = xattribute.Value,
        AuthType = policyAuthType
      });
    }
  }
}
