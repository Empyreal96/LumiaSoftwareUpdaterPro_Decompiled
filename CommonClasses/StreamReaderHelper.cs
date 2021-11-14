// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.StreamReaderHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.IO;

namespace Microsoft.LsuPro
{
  public class StreamReaderHelper
  {
    private readonly StreamReader stream;

    public StreamReaderHelper(string path) => this.stream = File.OpenText(path);

    public string ReadLine() => this.stream.ReadLine();

    public void Close() => this.stream.Close();
  }
}
