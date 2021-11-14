// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.CommonUtils
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.IO;

namespace System.Data.SqlServerCe
{
  internal static class CommonUtils
  {
    private const string DataDirectoryMacro = "|DataDirectory|";
    private const string DataDirectory = "DataDirectory";

    public static string ReplaceDataDirectory(string inputString)
    {
      string str = inputString.Trim();
      if (!string.IsNullOrEmpty(inputString) && inputString.StartsWith("|DataDirectory|", StringComparison.InvariantCultureIgnoreCase))
      {
        string path1 = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
        if (string.IsNullOrEmpty(path1))
          path1 = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
        if (string.IsNullOrEmpty(path1))
          path1 = string.Empty;
        int length = "|DataDirectory|".Length;
        if (inputString.Length > "|DataDirectory|".Length && '\\' == inputString["|DataDirectory|".Length])
          ++length;
        str = Path.Combine(path1, inputString.Substring(length));
      }
      return str;
    }

    static CommonUtils() => KillBitHelper.ThrowIfKillBitIsSet();
  }
}
