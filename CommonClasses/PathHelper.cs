// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.PathHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.IO;
using System.Text;

namespace Microsoft.LsuPro
{
  public static class PathHelper
  {
    public static string GetDirectoryName(string path) => Path.GetDirectoryName(path);

    public static string GetFileName(string path) => Path.GetFileName(path);

    public static string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);

    public static string Combine(string path1, string path2) => Path.Combine(path1, path2);

    public static bool CanCreateValidAnsiPathFromUnicode(string path)
    {
      Encoding unicode = Encoding.Unicode;
      Encoding defaultEncoding = PathHelper.GetDefaultEncoding();
      byte[] bytes1 = defaultEncoding.GetBytes(path);
      byte[] bytes2 = Encoding.Convert(defaultEncoding, unicode, bytes1);
      string str = unicode.GetString(bytes2);
      return path == str;
    }

    private static Encoding GetDefaultEncoding() => Encoding.Default;
  }
}
