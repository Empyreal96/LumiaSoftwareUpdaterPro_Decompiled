// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.ClientAssertion
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class ClientAssertion
  {
    public ClientAssertion(string clientId, string assertion)
    {
      if (string.IsNullOrWhiteSpace(clientId))
        throw new ArgumentNullException(nameof (clientId));
      if (string.IsNullOrWhiteSpace(assertion))
        throw new ArgumentNullException(nameof (assertion));
      this.ClientId = clientId;
      this.AssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
      this.Assertion = assertion;
    }

    public string ClientId { get; private set; }

    public string Assertion { get; private set; }

    public string AssertionType { get; private set; }
  }
}
