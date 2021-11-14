// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FileStreamHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class FileStreamHelper
  {
    private Stream stream;

    public Stream CreateFileStream(
      string path,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      this.stream = (Stream) new FileStream(path, fileMode, fileAccess, fileShare);
      return this.stream;
    }

    public Stream CreateFileStream(string path, FileMode fileMode)
    {
      this.stream = (Stream) new FileStream(path, fileMode);
      return this.stream;
    }

    public Stream CreateFileStream(string path)
    {
      this.stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
      return this.stream;
    }

    public void Close() => this.stream.Close();
  }
}
