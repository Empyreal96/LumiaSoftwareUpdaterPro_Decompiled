// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Security;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class ClientCredential
  {
    public ClientCredential(string clientId, string clientSecret)
    {
      if (string.IsNullOrWhiteSpace(clientId))
        throw new ArgumentNullException(nameof (clientId));
      if (string.IsNullOrWhiteSpace(clientSecret))
        throw new ArgumentNullException(nameof (clientSecret));
      this.ClientId = clientId;
      this.ClientSecret = clientSecret;
    }

    public ClientCredential(string clientId, SecureString secureClientSecret)
    {
      this.ClientId = !string.IsNullOrWhiteSpace(clientId) ? clientId : throw new ArgumentNullException(nameof (clientId));
      this.SecureClientSecret = secureClientSecret;
    }

    internal SecureString SecureClientSecret { get; private set; }

    public string ClientId { get; private set; }

    internal string ClientSecret { get; private set; }
  }
}
