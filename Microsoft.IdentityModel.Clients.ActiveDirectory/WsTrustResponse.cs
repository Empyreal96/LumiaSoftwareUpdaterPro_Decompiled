// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.WsTrustResponse
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class WsTrustResponse
  {
    public const string Saml1Assertion = "urn:oasis:names:tc:SAML:1.0:assertion";

    public string Token { get; private set; }

    public string TokenType { get; private set; }

    public static WsTrustResponse CreateFromResponse(Stream responseStream) => WsTrustResponse.CreateFromResponseDocument(WsTrustResponse.ReadDocumentFromResponse(responseStream));

    public static string ReadErrorResponse(XDocument responseDocument, CallState callState)
    {
      string str = (string) null;
      try
      {
        XElement xelement1 = responseDocument.Descendants(XmlNamespace.SoapEnvelope + "Body").FirstOrDefault<XElement>();
        if (xelement1 != null)
        {
          XElement xelement2 = xelement1.Elements(XmlNamespace.SoapEnvelope + "Fault").FirstOrDefault<XElement>();
          if (xelement2 != null)
          {
            XElement xelement3 = xelement2.Elements(XmlNamespace.SoapEnvelope + "Reason").FirstOrDefault<XElement>();
            if (xelement3 != null)
            {
              XElement xelement4 = xelement3.Elements(XmlNamespace.SoapEnvelope + "Text").FirstOrDefault<XElement>();
              if (xelement4 != null)
              {
                using (XmlReader reader = xelement4.CreateReader())
                {
                  int content = (int) reader.MoveToContent();
                  str = reader.ReadInnerXml();
                }
              }
            }
          }
        }
      }
      catch (XmlException ex)
      {
        throw new AdalException("parsing_wstrust_response_failed", (Exception) ex);
      }
      return str;
    }

    internal static XDocument ReadDocumentFromResponse(Stream responseStream)
    {
      try
      {
        return XDocument.Load(responseStream, LoadOptions.None);
      }
      catch (XmlException ex)
      {
        throw new AdalException("parsing_wstrust_response_failed", (Exception) ex);
      }
    }

    internal static WsTrustResponse CreateFromResponseDocument(
      XDocument responseDocument)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      try
      {
        if (responseDocument.Descendants(XmlNamespace.Trust + "RequestSecurityTokenResponseCollection").FirstOrDefault<XElement>() != null)
        {
          foreach (XElement descendant in responseDocument.Descendants(XmlNamespace.Trust + "RequestSecurityTokenResponse"))
          {
            XElement xelement1 = descendant.Elements(XmlNamespace.Trust + "TokenType").FirstOrDefault<XElement>();
            if (xelement1 != null)
            {
              XElement xelement2 = descendant.Elements(XmlNamespace.Trust + "RequestedSecurityToken").FirstOrDefault<XElement>();
              if (xelement2 != null)
                dictionary.Add(xelement1.Value, xelement2.FirstNode.ToString(SaveOptions.DisableFormatting));
            }
          }
        }
      }
      catch (XmlException ex)
      {
        throw new AdalException("parsing_wstrust_response_failed", (Exception) ex);
      }
      if (dictionary.Count == 0)
        throw new AdalException("parsing_wstrust_response_failed");
      string key = dictionary.ContainsKey("urn:oasis:names:tc:SAML:1.0:assertion") ? "urn:oasis:names:tc:SAML:1.0:assertion" : dictionary.Keys.First<string>();
      return new WsTrustResponse()
      {
        TokenType = key,
        Token = dictionary[key]
      };
    }
  }
}
