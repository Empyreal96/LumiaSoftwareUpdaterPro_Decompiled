// Decompiled with JetBrains decompiler
// Type: Ionic.TypeCriterion
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Ionic
{
  internal class TypeCriterion : SelectionCriterion
  {
    private char ObjectType;
    internal ComparisonOperator Operator;

    internal string AttributeString
    {
      get => this.ObjectType.ToString();
      set => this.ObjectType = value.Length == 1 && (value[0] == 'D' || value[0] == 'F') ? value[0] : throw new ArgumentException("Specify a single character: either D or F");
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("type ").Append(EnumUtil.GetDescription((Enum) this.Operator)).Append(" ").Append(this.AttributeString);
      return stringBuilder.ToString();
    }

    internal override bool Evaluate(string filename)
    {
      bool flag = this.ObjectType == 'D' ? Directory.Exists(filename) : File.Exists(filename);
      if (this.Operator != ComparisonOperator.EqualTo)
        flag = !flag;
      return flag;
    }

    internal override bool Evaluate(ZipEntry entry)
    {
      bool flag = this.ObjectType == 'D' ? entry.IsDirectory : !entry.IsDirectory;
      if (this.Operator != ComparisonOperator.EqualTo)
        flag = !flag;
      return flag;
    }
  }
}
