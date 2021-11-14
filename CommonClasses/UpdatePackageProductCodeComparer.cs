// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UpdatePackageProductCodeComparer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class UpdatePackageProductCodeComparer : IEqualityComparer<UpdatePackage>
  {
    public bool Equals(UpdatePackage x, UpdatePackage y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.ProductCode == y.ProductCode;
    }

    public int GetHashCode(UpdatePackage obj) => obj == null || obj.ProductCode == null ? 0 : obj.ProductCode.GetHashCode();
  }
}
