// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.StringExtensions
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.LsuPro
{
  public static class StringExtensions
  {
    public static string EncodeAsFileName(this string fileName, string replacement = "_") => Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", replacement);

    public static string EncodeAsPathName(this string pathName, string replacement = "_") => Regex.Replace(pathName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", replacement);
  }
}
