// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackageVariantVersionComparer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro
{
  public class UpdatePackageVariantVersionComparer : IEqualityComparer<UpdatePackage>
  {
    public bool Equals(UpdatePackage x, UpdatePackage y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.VariantVersion == y.VariantVersion;
    }

    public int GetHashCode(UpdatePackage obj) => obj == null || obj.VariantVersion == null ? 0 : obj.VariantVersion.GetHashCode();
  }
}
