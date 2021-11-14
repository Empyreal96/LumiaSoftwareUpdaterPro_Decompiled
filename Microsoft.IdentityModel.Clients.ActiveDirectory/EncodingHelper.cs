// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.EncodingHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class EncodingHelper
  {
    public static void AddKeyValueStringsWithUrlEncoding(
      StringBuilder messageBuilder,
      Dictionary<string, string> keyValuePairs)
    {
      foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
        EncodingHelper.AddKeyValueString(messageBuilder, EncodingHelper.UrlEncode(keyValuePair.Key), EncodingHelper.UrlEncode(keyValuePair.Value));
    }

    public static void AddStringWithUrlEncoding(
      StringBuilder messageBuilder,
      string key,
      char[] value)
    {
      char[] chars = (char[]) null;
      try
      {
        chars = EncodingHelper.UrlEncode(value);
        EncodingHelper.AddKeyValueString(messageBuilder, EncodingHelper.UrlEncode(key), chars);
      }
      finally
      {
        chars.SecureClear();
      }
    }

    public static void AddKeyValueString(StringBuilder messageBuilder, string key, string value) => EncodingHelper.AddKeyValueString(messageBuilder, key, value.ToCharArray());

    public static Dictionary<string, string> ParseKeyValueList(
      string input,
      char delimiter,
      bool urlDecode,
      CallState callState)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string splitWithQuote in EncodingHelper.SplitWithQuotes(input, delimiter))
      {
        List<string> stringList = EncodingHelper.SplitWithQuotes(splitWithQuote, '=');
        if (stringList.Count == 2 && !string.IsNullOrWhiteSpace(stringList[0]) && !string.IsNullOrWhiteSpace(stringList[1]))
        {
          string message1 = stringList[0];
          string message2 = stringList[1];
          if (urlDecode)
          {
            message1 = EncodingHelper.UrlDecode(message1);
            message2 = EncodingHelper.UrlDecode(message2);
          }
          string lower = message1.Trim().PlatformSpecificToLower();
          string str = message2.Trim().Trim('"').Trim();
          if (dictionary.ContainsKey(lower))
            Logger.Warning(callState, "Key/value pair list contains redundant key '{0}'.", (object) lower);
          dictionary[lower] = str;
        }
      }
      return dictionary;
    }

    public static byte[] ToByteArray(this StringBuilder stringBuilder)
    {
      if (stringBuilder == null)
        return (byte[]) null;
      UTF8Encoding utF8Encoding = new UTF8Encoding();
      char[] chArray = new char[stringBuilder.Length];
      try
      {
        stringBuilder.CopyTo(0, chArray, 0, stringBuilder.Length);
        return utF8Encoding.GetBytes(chArray);
      }
      finally
      {
        chArray.SecureClear();
      }
    }

    public static void SecureClear(this StringBuilder stringBuilder)
    {
      if (stringBuilder == null)
        return;
      for (int index = 0; index < stringBuilder.Length; ++index)
        stringBuilder[index] = char.MinValue;
      stringBuilder.Length = 0;
    }

    public static void SecureClear(this byte[] bytes)
    {
      if (bytes == null)
        return;
      for (int index = 0; index < bytes.Length; ++index)
        bytes[index] = (byte) 0;
    }

    public static void SecureClear(this char[] chars)
    {
      if (chars == null)
        return;
      for (int index = 0; index < chars.Length; ++index)
        chars[index] = char.MinValue;
    }

    internal static string Base64Encode(string input)
    {
      string str = string.Empty;
      if (!string.IsNullOrEmpty(input))
        str = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
      return str;
    }

    internal static string Base64Decode(string encodedString)
    {
      string str = (string) null;
      if (!string.IsNullOrEmpty(encodedString))
      {
        byte[] bytes = Convert.FromBase64String(encodedString);
        str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
      }
      return str;
    }

    internal static char[] UrlEncode(char[] message)
    {
      if (message == null)
        return (char[]) null;
      char[] array = new char[message.Length * 2];
      int num = 0;
      char[] chArray = new char[1];
      foreach (char ch in message)
      {
        chArray[0] = ch;
        char[] charArray = EncodingHelper.UrlEncode(new string(chArray)).ToCharArray();
        charArray.CopyTo((Array) array, num);
        if (num + charArray.Length > array.Length)
          Array.Resize<char>(ref array, message.Length * 2);
        num += charArray.Length;
      }
      Array.Resize<char>(ref array, num);
      return array;
    }

    internal static List<string> SplitWithQuotes(string input, char delimiter)
    {
      List<string> stringList = new List<string>();
      if (string.IsNullOrWhiteSpace(input))
        return stringList;
      int startIndex = 0;
      bool flag = false;
      for (int index = 0; index < input.Length; ++index)
      {
        if ((int) input[index] == (int) delimiter && !flag)
        {
          string str = input.Substring(startIndex, index - startIndex);
          if (!string.IsNullOrWhiteSpace(str.Trim()))
            stringList.Add(str);
          startIndex = index + 1;
        }
        else if (input[index] == '"')
          flag = !flag;
      }
      string str1 = input.Substring(startIndex);
      if (!string.IsNullOrWhiteSpace(str1.Trim()))
        stringList.Add(str1);
      return stringList;
    }

    private static void AddKeyValueString(StringBuilder messageBuilder, string key, char[] value)
    {
      string str = messageBuilder.Length == 0 ? string.Empty : "&";
      messageBuilder.AppendFormat("{0}{1}=", (object) str, (object) key);
      messageBuilder.Append(value);
    }

    public static char[] ToCharArray(this SecureString secureString)
    {
      char[] chArray = new char[secureString.Length];
      IntPtr coTaskMemUnicode = Marshal.SecureStringToCoTaskMemUnicode(secureString);
      for (int index = 0; index < secureString.Length; ++index)
        chArray[index] = (char) Marshal.ReadInt16(coTaskMemUnicode, index * 2);
      Marshal.ZeroFreeCoTaskMemUnicode(coTaskMemUnicode);
      return chArray;
    }

    public static string UrlEncode(string message)
    {
      if (string.IsNullOrEmpty(message))
        return message;
      message = Uri.EscapeDataString(message);
      message = message.Replace("%20", "+");
      return message;
    }

    public static string UrlDecode(string message)
    {
      if (string.IsNullOrEmpty(message))
        return message;
      message = message.Replace("+", "%20");
      message = Uri.UnescapeDataString(message);
      return message;
    }
  }
}
