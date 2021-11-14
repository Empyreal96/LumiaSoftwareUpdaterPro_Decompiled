// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCacheNotificationArgs
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class TokenCacheNotificationArgs
  {
    public TokenCache TokenCache { get; internal set; }

    public string ClientId { get; internal set; }

    public string Resource { get; internal set; }

    public string UniqueId { get; internal set; }

    public string DisplayableId { get; internal set; }
  }
}
