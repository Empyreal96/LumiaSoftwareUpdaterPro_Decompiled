// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.JsonWebToken
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class JsonWebToken
  {
    private const int MaxTokenLength = 65536;
    private readonly JsonWebToken.JWTPayload payload;

    public JsonWebToken(ClientAssertionCertificate certificate, string audience)
    {
      DateTime webTokenValidFrom = NetworkPlugin.RequestCreationHelper.GetJsonWebTokenValidFrom();
      DateTime time = webTokenValidFrom + TimeSpan.FromSeconds(600.0);
      this.payload = new JsonWebToken.JWTPayload()
      {
        Audience = audience,
        Issuer = certificate.ClientId,
        ValidFrom = DateTimeHelper.ConvertToTimeT(webTokenValidFrom),
        ValidTo = DateTimeHelper.ConvertToTimeT(time),
        Subject = certificate.ClientId
      };
      this.payload.JwtIdentifier = NetworkPlugin.RequestCreationHelper.GetJsonWebTokenId();
    }

    public ClientAssertion Sign(ClientAssertionCertificate credential)
    {
      string message = this.Encode(credential);
      if (65536 < message.Length)
        throw new AdalException("encoded_token_too_long");
      return new ClientAssertion(this.payload.Issuer, message + "." + JsonWebToken.UrlEncodeSegment(credential.Sign(message)));
    }

    private static string EncodeSegment(string segment) => JsonWebToken.UrlEncodeSegment(Encoding.UTF8.GetBytes(segment));

    private static string UrlEncodeSegment(byte[] segment) => Base64UrlEncoder.Encode(segment);

    private static string EncodeToJson<T>(T toEncode)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (T)).WriteObject((Stream) memoryStream, (object) toEncode);
        return Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int) memoryStream.Position);
      }
    }

    private static string EncodeHeaderToJson(ClientAssertionCertificate credential) => JsonWebToken.EncodeToJson<JsonWebToken.JWTHeaderWithCertificate>(new JsonWebToken.JWTHeaderWithCertificate(credential));

    private string Encode(ClientAssertionCertificate credential) => JsonWebToken.EncodeSegment(JsonWebToken.EncodeHeaderToJson(credential)) + "." + JsonWebToken.EncodeSegment(this.EncodePayloadToJson());

    private string EncodePayloadToJson() => JsonWebToken.EncodeToJson<JsonWebToken.JWTPayload>(this.payload);

    [DataContract]
    internal class JWTHeader
    {
      protected ClientAssertionCertificate Credential { get; private set; }

      public JWTHeader(ClientAssertionCertificate credential) => this.Credential = credential;

      [DataMember(Name = "typ")]
      public static string Type
      {
        get => "JWT";
        set
        {
        }
      }

      [DataMember(Name = "alg")]
      public string Algorithm
      {
        get => this.Credential != null ? "RS256" : "none";
        set
        {
        }
      }
    }

    [DataContract]
    internal class JWTPayload
    {
      [DataMember(Name = "aud")]
      public string Audience { get; set; }

      [DataMember(Name = "iss")]
      public string Issuer { get; set; }

      [DataMember(Name = "nbf")]
      public long ValidFrom { get; set; }

      [DataMember(Name = "exp")]
      public long ValidTo { get; set; }

      [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "sub")]
      public string Subject { get; set; }

      [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "jti")]
      public string JwtIdentifier { get; set; }
    }

    [DataContract]
    internal sealed class JWTHeaderWithCertificate : JsonWebToken.JWTHeader
    {
      public JWTHeaderWithCertificate(ClientAssertionCertificate credential)
        : base(credential)
      {
      }

      [DataMember(Name = "x5t")]
      public string X509CertificateThumbprint
      {
        get => Base64UrlEncoder.Encode(this.Credential.Certificate.GetCertHash());
        set
        {
        }
      }
    }
  }
}
