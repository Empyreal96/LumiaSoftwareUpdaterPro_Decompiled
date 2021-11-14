// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Authenticator
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class Authenticator
  {
    private const string TenantlessTenantName = "Common";
    private static readonly AuthenticatorTemplateList AuthenticatorTemplateList = new AuthenticatorTemplateList();
    private bool updatedFromTemplate;

    public Authenticator(string authority, bool validateAuthority)
    {
      this.Authority = Authenticator.CanonicalizeUri(authority);
      this.AuthorityType = Authenticator.DetectAuthorityType(this.Authority);
      this.ValidateAuthority = this.AuthorityType == AuthorityType.AAD || !validateAuthority ? validateAuthority : throw new ArgumentException("Authority validation is not supported for this type of authority", nameof (validateAuthority));
    }

    public string Authority { get; private set; }

    public AuthorityType AuthorityType { get; private set; }

    public bool ValidateAuthority { get; private set; }

    public bool IsTenantless { get; private set; }

    public string AuthorizationUri { get; set; }

    public string TokenUri { get; private set; }

    public string UserRealmUri { get; private set; }

    public string SelfSignedJwtAudience { get; private set; }

    public Guid CorrelationId { get; set; }

    public async Task UpdateFromTemplateAsync(CallState callState)
    {
      if (this.updatedFromTemplate)
        return;
      Uri authorityUri = new Uri(this.Authority);
      string host = authorityUri.Authority;
      string path = authorityUri.AbsolutePath.Substring(1);
      string tenant = path.Substring(0, path.IndexOf("/", StringComparison.Ordinal));
      AuthenticatorTemplate matchingTemplate = await Authenticator.AuthenticatorTemplateList.FindMatchingItemAsync(this.ValidateAuthority, host, tenant, callState);
      this.AuthorizationUri = matchingTemplate.AuthorizeEndpoint.Replace("{tenant}", tenant);
      this.TokenUri = matchingTemplate.TokenEndpoint.Replace("{tenant}", tenant);
      this.UserRealmUri = Authenticator.CanonicalizeUri(matchingTemplate.UserRealmEndpoint);
      this.IsTenantless = string.Compare(tenant, "Common", StringComparison.OrdinalIgnoreCase) == 0;
      this.SelfSignedJwtAudience = matchingTemplate.Issuer.Replace("{tenant}", tenant);
      this.updatedFromTemplate = true;
    }

    public void UpdateTenantId(string tenantId)
    {
      if (!this.IsTenantless || string.IsNullOrWhiteSpace(tenantId))
        return;
      this.Authority = Authenticator.ReplaceTenantlessTenant(this.Authority, tenantId);
      this.updatedFromTemplate = false;
    }

    internal static AuthorityType DetectAuthorityType(string authority)
    {
      if (string.IsNullOrWhiteSpace(authority))
        throw new ArgumentNullException(nameof (authority));
      Uri uri = Uri.IsWellFormedUriString(authority, UriKind.Absolute) ? new Uri(authority) : throw new ArgumentException("'authority' should be in Uri format", nameof (authority));
      string str = !(uri.Scheme != "https") ? uri.AbsolutePath.Substring(1) : throw new ArgumentException("'authority' should use the 'https' scheme", nameof (authority));
      if (string.IsNullOrWhiteSpace(str))
        throw new ArgumentException("'authority' Uri should have at least one segment in the path (i.e. https://<host>/<path>/...)", nameof (authority));
      return Authenticator.IsAdfsAuthority(str.Substring(0, str.IndexOf("/", StringComparison.Ordinal))) ? AuthorityType.ADFS : AuthorityType.AAD;
    }

    private static string CanonicalizeUri(string uri)
    {
      if (!string.IsNullOrWhiteSpace(uri) && !uri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        uri += "/";
      return uri;
    }

    private static string ReplaceTenantlessTenant(string authority, string tenantId) => new Regex(Regex.Escape("Common"), RegexOptions.IgnoreCase).Replace(authority, tenantId, 1);

    private static bool IsAdfsAuthority(string firstPath) => string.Compare(firstPath, "adfs", StringComparison.OrdinalIgnoreCase) == 0;
  }
}
