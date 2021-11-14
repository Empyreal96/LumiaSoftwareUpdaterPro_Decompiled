// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Base64UrlEncoder
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class Base64UrlEncoder
  {
    private const char Base64PadCharacter = '=';
    private const char Base64Character62 = '+';
    private const char Base64Character63 = '/';
    private const char Base64UrlCharacter62 = '-';
    private const char Base64UrlCharacter63 = '_';
    private static readonly Encoding TextEncoding = Encoding.UTF8;
    private static readonly string DoubleBase64PadCharacter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{0}", (object) '=');

    public static string Encode(string arg) => arg != null ? Base64UrlEncoder.Encode(Base64UrlEncoder.TextEncoding.GetBytes(arg)) : throw new ArgumentNullException(nameof (arg));

    public static byte[] DecodeBytes(string arg)
    {
      string s = arg.Replace('-', '+').Replace('_', '/');
      switch (s.Length % 4)
      {
        case 0:
          return Convert.FromBase64String(s);
        case 2:
          s += Base64UrlEncoder.DoubleBase64PadCharacter;
          goto case 0;
        case 3:
          s += (string) (object) '=';
          goto case 0;
        default:
          throw new ArgumentException("Illegal base64url string!", nameof (arg));
      }
    }

    internal static string Encode(byte[] arg) => arg != null ? Convert.ToBase64String(arg).Split('=')[0].Replace('+', '-').Replace('/', '_') : throw new ArgumentNullException(nameof (arg));
  }
}
