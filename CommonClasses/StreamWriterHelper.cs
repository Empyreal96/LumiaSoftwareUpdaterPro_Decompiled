// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.StreamWriterHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class StreamWriterHelper
  {
    private readonly StreamWriter stream;

    public StreamWriterHelper(string path) => this.stream = new StreamWriter(path);

    public void WriteLine(string line) => this.stream.WriteLine(line);

    public void Flush() => this.stream.Flush();

    public void Close() => this.stream.Close();
  }
}
