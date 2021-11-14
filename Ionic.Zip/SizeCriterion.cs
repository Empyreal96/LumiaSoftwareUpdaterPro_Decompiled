// Decompiled with JetBrains decompiler
// Type: Ionic.SizeCriterion
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Ionic
{
  internal class SizeCriterion : SelectionCriterion
  {
    internal ComparisonOperator Operator;
    internal long Size;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("size ").Append(EnumUtil.GetDescription((Enum) this.Operator)).Append(" ").Append(this.Size.ToString());
      return stringBuilder.ToString();
    }

    internal override bool Evaluate(string filename) => this._Evaluate(new FileInfo(filename).Length);

    private bool _Evaluate(long Length)
    {
      switch (this.Operator)
      {
        case ComparisonOperator.GreaterThan:
          return Length > this.Size;
        case ComparisonOperator.GreaterThanOrEqualTo:
          return Length >= this.Size;
        case ComparisonOperator.LesserThan:
          return Length < this.Size;
        case ComparisonOperator.LesserThanOrEqualTo:
          return Length <= this.Size;
        case ComparisonOperator.EqualTo:
          return Length == this.Size;
        case ComparisonOperator.NotEqualTo:
          return Length != this.Size;
        default:
          throw new ArgumentException("Operator");
      }
    }

    internal override bool Evaluate(ZipEntry entry) => this._Evaluate(entry.UncompressedSize);
  }
}
