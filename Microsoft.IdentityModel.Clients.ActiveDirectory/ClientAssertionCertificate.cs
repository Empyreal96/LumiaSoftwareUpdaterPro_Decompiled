// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.ClientAssertionCertificate
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class ClientAssertionCertificate
  {
    public ClientAssertionCertificate(string clientId, X509Certificate2 certificate)
    {
      if (string.IsNullOrWhiteSpace(clientId))
        throw new ArgumentNullException(nameof (clientId));
      if (certificate == null)
        throw new ArgumentNullException(nameof (certificate));
      if (certificate.PublicKey.Key.KeySize < ClientAssertionCertificate.MinKeySizeInBits)
        throw new ArgumentOutOfRangeException(nameof (certificate), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The certificate used must have a key size of at least {0} bits", (object) ClientAssertionCertificate.MinKeySizeInBits));
      this.ClientId = clientId;
      this.Certificate = certificate;
    }

    public static int MinKeySizeInBits => 2048;

    public string ClientId { get; private set; }

    public X509Certificate2 Certificate { get; private set; }

    internal byte[] Sign(string message) => CryptographyHelper.SignWithCertificate(message, this.Certificate);
  }
}
