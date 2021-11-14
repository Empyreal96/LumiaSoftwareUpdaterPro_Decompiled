// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.JsonWebTokenConstants
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class JsonWebTokenConstants
  {
    public const uint JwtToAadLifetimeInSeconds = 600;
    public const string HeaderType = "JWT";

    internal class Algorithms
    {
      public const string RsaSha256 = "RS256";
      public const string None = "none";
    }

    internal class ReservedClaims
    {
      public const string Audience = "aud";
      public const string Issuer = "iss";
      public const string Subject = "sub";
      public const string NotBefore = "nbf";
      public const string ExpiresOn = "exp";
      public const string JwtIdentifier = "jti";
    }

    internal class ReservedHeaderParameters
    {
      public const string Algorithm = "alg";
      public const string Type = "typ";
      public const string X509CertificateThumbprint = "x5t";
    }
  }
}
