// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCacheKey
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal sealed class TokenCacheKey
  {
    internal TokenCacheKey(
      string authority,
      string resource,
      string clientId,
      TokenSubjectType tokenSubjectType,
      UserInfo userInfo)
      : this(authority, resource, clientId, tokenSubjectType, userInfo?.UniqueId, userInfo?.DisplayableId)
    {
    }

    internal TokenCacheKey(
      string authority,
      string resource,
      string clientId,
      TokenSubjectType tokenSubjectType,
      string uniqueId,
      string displayableId)
    {
      this.Authority = authority;
      this.Resource = resource;
      this.ClientId = clientId;
      this.TokenSubjectType = tokenSubjectType;
      this.UniqueId = uniqueId;
      this.DisplayableId = displayableId;
    }

    public string Authority { get; private set; }

    public string Resource { get; internal set; }

    public string ClientId { get; private set; }

    public string UniqueId { get; private set; }

    public string DisplayableId { get; private set; }

    public TokenSubjectType TokenSubjectType { get; private set; }

    public override bool Equals(object obj) => obj is TokenCacheKey other && this.Equals(other);

    public bool Equals(TokenCacheKey other)
    {
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      return other != null && other.Authority == this.Authority && (this.ResourceEquals(other.Resource) && this.ClientIdEquals(other.ClientId)) && (other.UniqueId == this.UniqueId && this.DisplayableIdEquals(other.DisplayableId)) && other.TokenSubjectType == this.TokenSubjectType;
    }

    public override int GetHashCode() => (this.Authority + ":::" + this.Resource.ToLower() + ":::" + this.ClientId.ToLower() + ":::" + this.UniqueId + ":::" + (this.DisplayableId != null ? (object) this.DisplayableId.ToLower() : (object) (string) null) + ":::" + (object) (int) this.TokenSubjectType).GetHashCode();

    internal bool ResourceEquals(string otherResource) => string.Compare(otherResource, this.Resource, StringComparison.OrdinalIgnoreCase) == 0;

    internal bool ClientIdEquals(string otherClientId) => string.Compare(otherClientId, this.ClientId, StringComparison.OrdinalIgnoreCase) == 0;

    internal bool DisplayableIdEquals(string otherDisplayableId) => string.Compare(otherDisplayableId, this.DisplayableId, StringComparison.OrdinalIgnoreCase) == 0;
  }
}
