// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.IdToken
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [DataContract]
  internal class IdToken
  {
    [DataMember(IsRequired = false, Name = "oid")]
    public string ObjectId { get; set; }

    [DataMember(IsRequired = false, Name = "sub")]
    public string Subject { get; set; }

    [DataMember(IsRequired = false, Name = "tid")]
    public string TenantId { get; set; }

    [DataMember(IsRequired = false, Name = "upn")]
    public string UPN { get; set; }

    [DataMember(IsRequired = false, Name = "given_name")]
    public string GivenName { get; set; }

    [DataMember(IsRequired = false, Name = "family_name")]
    public string FamilyName { get; set; }

    [DataMember(IsRequired = false, Name = "email")]
    public string Email { get; set; }

    [DataMember(IsRequired = false, Name = "pwd_exp")]
    public long PasswordExpiration { get; set; }

    [DataMember(IsRequired = false, Name = "pwd_url")]
    public string PasswordChangeUrl { get; set; }

    [DataMember(IsRequired = false, Name = "idp")]
    public string IdentityProvider { get; set; }

    [DataMember(IsRequired = false, Name = "iss")]
    public string Issuer { get; set; }
  }
}
