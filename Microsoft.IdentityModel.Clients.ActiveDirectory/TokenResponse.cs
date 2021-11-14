// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.TokenResponse
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [DataContract]
  internal class TokenResponse
  {
    private const string CorrelationIdClaim = "correlation_id";

    [DataMember(IsRequired = false, Name = "token_type")]
    public string TokenType { get; set; }

    [DataMember(IsRequired = false, Name = "access_token")]
    public string AccessToken { get; set; }

    [DataMember(IsRequired = false, Name = "refresh_token")]
    public string RefreshToken { get; set; }

    [DataMember(IsRequired = false, Name = "resource")]
    public string Resource { get; set; }

    [DataMember(IsRequired = false, Name = "id_token")]
    public string IdToken { get; set; }

    [DataMember(IsRequired = false, Name = "created_on")]
    public long CreatedOn { get; set; }

    [DataMember(IsRequired = false, Name = "expires_on")]
    public long ExpiresOn { get; set; }

    [DataMember(IsRequired = false, Name = "expires_in")]
    public long ExpiresIn { get; set; }

    [DataMember(IsRequired = false, Name = "error")]
    public string Error { get; set; }

    [DataMember(IsRequired = false, Name = "error_description")]
    public string ErrorDescription { get; set; }

    [DataMember(IsRequired = false, Name = "error_codes")]
    public string[] ErrorCodes { get; set; }

    [DataMember(IsRequired = false, Name = "correlation_id")]
    public string CorrelationId { get; set; }
  }
}
