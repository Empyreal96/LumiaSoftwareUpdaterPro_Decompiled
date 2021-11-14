// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.CryptographyHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class CryptographyHelper
  {
    public static byte[] SignWithCertificate(string message, X509Certificate2 x509Certificate)
    {
      RSACryptoServiceProvider asymmetricAlgorithm = new X509AsymmetricSecurityKey(x509Certificate).GetAsymmetricAlgorithm("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", true) as RSACryptoServiceProvider;
      RSACryptoServiceProvider cryptoServiceProvider = (RSACryptoServiceProvider) null;
      try
      {
        cryptoServiceProvider = CryptographyHelper.GetCryptoProviderForSha256(asymmetricAlgorithm);
        using (SHA256Cng shA256Cng = new SHA256Cng())
          return cryptoServiceProvider.SignData(Encoding.UTF8.GetBytes(message), (object) shA256Cng);
      }
      finally
      {
        if (cryptoServiceProvider != null && !object.ReferenceEquals((object) asymmetricAlgorithm, (object) cryptoServiceProvider))
          cryptoServiceProvider.Dispose();
      }
    }

    public static byte[] SignWithSymmetricKey(string message, byte[] key)
    {
      using (HMAC hmac = HMAC.Create("HMACSHA256"))
      {
        hmac.Key = key;
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
      }
    }

    private static RSACryptoServiceProvider GetCryptoProviderForSha256(
      RSACryptoServiceProvider rsaProvider)
    {
      if (rsaProvider.CspKeyContainerInfo.ProviderType == 24)
        return rsaProvider;
      CspParameters parameters = new CspParameters()
      {
        ProviderType = 24,
        KeyContainerName = rsaProvider.CspKeyContainerInfo.KeyContainerName,
        KeyNumber = (int) rsaProvider.CspKeyContainerInfo.KeyNumber
      };
      if (rsaProvider.CspKeyContainerInfo.MachineKeyStore)
        parameters.Flags = CspProviderFlags.UseMachineKeyStore;
      parameters.Flags |= CspProviderFlags.UseExistingKey;
      return new RSACryptoServiceProvider(parameters);
    }
  }
}
