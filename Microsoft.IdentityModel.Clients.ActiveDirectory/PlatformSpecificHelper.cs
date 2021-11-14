// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.PlatformSpecificHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class PlatformSpecificHelper
  {
    public static string GetProductName() => ".NET";

    public static string GetEnvironmentVariable(string variable)
    {
      string environmentVariable = Environment.GetEnvironmentVariable(variable);
      return string.IsNullOrWhiteSpace(environmentVariable) ? (string) null : environmentVariable;
    }

    public static string PlatformSpecificToLower(this string input) => input.ToLower(CultureInfo.InvariantCulture);

    public static string GetUserPrincipalName()
    {
      uint userNameSize = 0;
      PlatformSpecificHelper.NativeMethods.GetUserNameEx(8, (StringBuilder) null, ref userNameSize);
      StringBuilder userName = userNameSize != 0U ? new StringBuilder((int) userNameSize) : throw new AdalException("get_user_name_failed", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
      return PlatformSpecificHelper.NativeMethods.GetUserNameEx(8, userName, ref userNameSize) ? userName.ToString() : throw new AdalException("get_user_name_failed", (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }

    public static string CreateSha256Hash(string input)
    {
      using (SHA256Cng shA256Cng = new SHA256Cng())
      {
        UTF8Encoding utF8Encoding = new UTF8Encoding();
        return Convert.ToBase64String(shA256Cng.ComputeHash(utF8Encoding.GetBytes(input)));
      }
    }

    public static void CloseHttpWebResponse(WebResponse response) => response.Close();

    private static class NativeMethods
    {
      [DllImport("secur32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.U1)]
      public static extern bool GetUserNameEx(
        int nameFormat,
        StringBuilder userName,
        ref uint userNameSize);
    }
  }
}
