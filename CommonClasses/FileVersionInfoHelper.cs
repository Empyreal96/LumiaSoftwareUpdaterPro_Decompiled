// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FileVersionInfoHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Diagnostics;

namespace Microsoft.LsuPro
{
  public static class FileVersionInfoHelper
  {
    public static FileVersionInfo GetVersionInfo(string fileName) => FileVersionInfo.GetVersionInfo(fileName);
  }
}
