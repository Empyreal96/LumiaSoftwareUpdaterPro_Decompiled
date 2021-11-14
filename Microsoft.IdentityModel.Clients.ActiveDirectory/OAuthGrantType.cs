// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.OAuthGrantType
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class OAuthGrantType
  {
    public const string AuthorizationCode = "authorization_code";
    public const string RefreshToken = "refresh_token";
    public const string ClientCredentials = "client_credentials";
    public const string Saml11Bearer = "urn:ietf:params:oauth:grant-type:saml1_1-bearer";
    public const string Saml20Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
    public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
    public const string Password = "password";
  }
}
