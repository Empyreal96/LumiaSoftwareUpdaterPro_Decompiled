// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.ClientKey
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class ClientKey
  {
    public ClientKey(string clientId)
    {
      this.ClientId = !string.IsNullOrWhiteSpace(clientId) ? clientId : throw new ArgumentNullException(nameof (clientId));
      this.HasCredential = false;
    }

    public string ClientId { get; private set; }

    public bool HasCredential { get; private set; }

    public ClientKey(ClientCredential clientCredential)
    {
      this.Credential = clientCredential != null ? clientCredential : throw new ArgumentNullException(nameof (clientCredential));
      this.ClientId = clientCredential.ClientId;
      this.HasCredential = true;
    }

    public ClientKey(ClientAssertionCertificate clientCertificate, Authenticator authenticator)
    {
      this.Authenticator = authenticator;
      this.Certificate = clientCertificate != null ? clientCertificate : throw new ArgumentNullException(nameof (clientCertificate));
      this.ClientId = clientCertificate.ClientId;
      this.HasCredential = true;
    }

    public ClientKey(ClientAssertion clientAssertion)
    {
      this.Assertion = clientAssertion != null ? clientAssertion : throw new ArgumentNullException(nameof (clientAssertion));
      this.ClientId = clientAssertion.ClientId;
      this.HasCredential = true;
    }

    public ClientCredential Credential { get; private set; }

    public ClientAssertionCertificate Certificate { get; private set; }

    public ClientAssertion Assertion { get; private set; }

    public Authenticator Authenticator { get; private set; }
  }
}
