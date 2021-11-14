// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCacheItem
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class TokenCacheItem
  {
    internal TokenCacheItem(TokenCacheKey key, AuthenticationResult result)
    {
      this.Authority = key.Authority;
      this.Resource = key.Resource;
      this.ClientId = key.ClientId;
      this.TokenSubjectType = key.TokenSubjectType;
      this.UniqueId = key.UniqueId;
      this.DisplayableId = key.DisplayableId;
      this.TenantId = result.TenantId;
      this.ExpiresOn = result.ExpiresOn;
      this.IsMultipleResourceRefreshToken = result.IsMultipleResourceRefreshToken;
      this.AccessToken = result.AccessToken;
      this.RefreshToken = result.RefreshToken;
      this.IdToken = result.IdToken;
      if (result.UserInfo == null)
        return;
      this.FamilyName = result.UserInfo.FamilyName;
      this.GivenName = result.UserInfo.GivenName;
      this.IdentityProvider = result.UserInfo.IdentityProvider;
    }

    public string Authority { get; private set; }

    public string ClientId { get; internal set; }

    public DateTimeOffset ExpiresOn { get; internal set; }

    public string FamilyName { get; internal set; }

    public string GivenName { get; internal set; }

    public string IdentityProvider { get; internal set; }

    public bool IsMultipleResourceRefreshToken { get; internal set; }

    public string Resource { get; internal set; }

    public string TenantId { get; internal set; }

    public string UniqueId { get; internal set; }

    public string DisplayableId { get; internal set; }

    public string AccessToken { get; internal set; }

    public string RefreshToken { get; internal set; }

    public string IdToken { get; internal set; }

    internal TokenSubjectType TokenSubjectType { get; set; }

    internal bool Match(TokenCacheKey key) => key.Authority == this.Authority && key.ResourceEquals(this.Resource) && (key.ClientIdEquals(this.ClientId) && key.TokenSubjectType == this.TokenSubjectType) && key.UniqueId == this.UniqueId && key.DisplayableIdEquals(this.DisplayableId);
  }
}
