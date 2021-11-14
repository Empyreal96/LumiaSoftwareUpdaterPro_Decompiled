// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult
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
  [DataContract]
  public sealed class AuthenticationResult
  {
    private const string Oauth2AuthorizationHeader = "Bearer ";

    internal AuthenticationResult(
      string accessTokenType,
      string accessToken,
      string refreshToken,
      DateTimeOffset expiresOn)
    {
      this.AccessTokenType = accessTokenType;
      this.AccessToken = accessToken;
      this.RefreshToken = refreshToken;
      this.ExpiresOn = (DateTimeOffset) DateTime.SpecifyKind(expiresOn.DateTime, DateTimeKind.Utc);
    }

    [DataMember]
    public string AccessTokenType { get; private set; }

    [DataMember]
    public string AccessToken { get; internal set; }

    [DataMember]
    public string RefreshToken { get; internal set; }

    [DataMember]
    public DateTimeOffset ExpiresOn { get; internal set; }

    [DataMember]
    public string TenantId { get; private set; }

    [DataMember]
    public UserInfo UserInfo { get; internal set; }

    [DataMember]
    public string IdToken { get; internal set; }

    [DataMember]
    public bool IsMultipleResourceRefreshToken { get; internal set; }

    public static AuthenticationResult Deserialize(string serializedObject)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedObject)))
        return (AuthenticationResult) new DataContractJsonSerializer(typeof (AuthenticationResult)).ReadObject((Stream) memoryStream);
    }

    public string CreateAuthorizationHeader() => "Bearer " + this.AccessToken;

    public string Serialize()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (AuthenticationResult)).WriteObject((Stream) memoryStream, (object) this);
        return Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int) memoryStream.Position);
      }
    }

    internal void UpdateTenantAndUserInfo(string tenantId, string idToken, UserInfo userInfo)
    {
      this.TenantId = tenantId;
      this.IdToken = idToken;
      if (userInfo == null)
        return;
      this.UserInfo = new UserInfo(userInfo);
    }

    internal string Resource { get; set; }
  }
}
