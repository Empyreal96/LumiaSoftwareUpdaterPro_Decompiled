// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.PathConversionException
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "reviewed")]
  [Serializable]
  public class PathConversionException : Exception
  {
    public PathConversionException(string path) => this.Path = path;

    public PathConversionException(string path, string message)
      : base(message)
    {
      this.Path = path;
    }

    public PathConversionException(string path, string message, Exception innerException)
      : base(message, innerException)
    {
      this.Path = path;
    }

    public string Path { get; private set; }
  }
}
