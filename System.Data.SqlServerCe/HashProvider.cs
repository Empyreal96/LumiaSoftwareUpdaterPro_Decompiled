// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.HashProvider
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace System.Data.SqlServerCe
{
  internal class HashProvider
  {
    static HashProvider() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal static bool MatchHash(string filePath)
    {
      string[] strArray = new string[2]
      {
        "1zLdCHyo0bBvEohtwYF7tLbd5cy/4cgOK2yiOA7hvYY=",
        "nkxsb8fu5OHOJarhFN40NLkxICSRxQSYq5hH5XzAHYA="
      };
      byte[] hash = HashProvider.CalculateHash(filePath);
      if (IntPtr.Size == 8)
      {
        strArray[0] = "nkxsb8fu5OHOJarhFN40NLkxICSRxQSYq5hH5XzAHYA=";
        strArray[1] = "1zLdCHyo0bBvEohtwYF7tLbd5cy/4cgOK2yiOA7hvYY=";
      }
      foreach (string s in strArray)
      {
        if (HashProvider.ByteArrayEqual(Convert.FromBase64String(s), hash))
          return true;
      }
      return false;
    }

    private static bool ByteArrayEqual(byte[] array1, byte[] array2)
    {
      if (array1.Length != array2.Length)
        return false;
      for (int index = 0; index < array1.Length; ++index)
      {
        if ((int) array1[index] != (int) array2[index])
          return false;
      }
      return true;
    }

    [SecurityCritical]
    internal static byte[] CalculateHash(string filePath)
    {
      using (SHA256 shA256 = (SHA256) new SqlCeSHA256())
      {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
          return shA256.ComputeHash((Stream) fileStream);
      }
    }
  }
}
