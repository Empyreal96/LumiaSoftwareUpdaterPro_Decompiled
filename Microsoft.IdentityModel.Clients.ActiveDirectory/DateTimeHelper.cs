// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.DateTimeHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Globalization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class DateTimeHelper
  {
    public static long ConvertToTimeT(DateTime time)
    {
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      return (long) (time - dateTime).TotalSeconds;
    }

    public static DateTime ConvertFromTimeT(long seconds) => DateTime.SpecifyKind(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double) seconds), DateTimeKind.Utc);

    public static string BuildTimeString(DateTime utcTime) => utcTime.ToString("yyyy-MM-ddTHH:mm:ss.068Z", (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
