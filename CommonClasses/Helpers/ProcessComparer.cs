// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.ProcessComparer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.LsuPro.Helpers
{
  public class ProcessComparer : IEqualityComparer<Process>
  {
    public bool Equals(Process x, Process y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.MainModule.FileName == y.MainModule.FileName;
    }

    public int GetHashCode(Process obj) => obj == null || string.IsNullOrEmpty(obj.MainModule.FileName) ? 0 : obj.MainModule.FileName.GetHashCode();
  }
}
