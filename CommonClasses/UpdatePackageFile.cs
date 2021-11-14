// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackageFile
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;

namespace Microsoft.LsuPro
{
  public class UpdatePackageFile
  {
    public UpdatePackageFile(
      string productType,
      string fileName,
      string relativePath,
      long fileSize,
      DateTime timestamp)
    {
      this.ProductType = productType;
      this.FileName = fileName ?? string.Empty;
      this.RelativePath = relativePath ?? string.Empty;
      this.FileSize = fileSize;
      this.Timestamp = timestamp;
    }

    public string ProductType { get; private set; }

    public string FileName { get; private set; }

    public string RelativePath { get; private set; }

    public long FileSize { get; private set; }

    public DateTime Timestamp { get; private set; }
  }
}
