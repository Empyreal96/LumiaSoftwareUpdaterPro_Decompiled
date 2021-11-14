// Decompiled with JetBrains decompiler
// Type: Ionic.CompoundCriterion
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using Ionic.Zip;
using System;
using System.Text;

namespace Ionic
{
  internal class CompoundCriterion : SelectionCriterion
  {
    internal LogicalConjunction Conjunction;
    internal SelectionCriterion Left;
    private SelectionCriterion _Right;

    internal SelectionCriterion Right
    {
      get => this._Right;
      set
      {
        this._Right = value;
        if (value == null)
        {
          this.Conjunction = LogicalConjunction.NONE;
        }
        else
        {
          if (this.Conjunction != LogicalConjunction.NONE)
            return;
          this.Conjunction = LogicalConjunction.AND;
        }
      }
    }

    internal override bool Evaluate(string filename)
    {
      bool flag = this.Left.Evaluate(filename);
      switch (this.Conjunction)
      {
        case LogicalConjunction.AND:
          if (flag)
          {
            flag = this.Right.Evaluate(filename);
            break;
          }
          break;
        case LogicalConjunction.OR:
          if (!flag)
          {
            flag = this.Right.Evaluate(filename);
            break;
          }
          break;
        case LogicalConjunction.XOR:
          flag ^= this.Right.Evaluate(filename);
          break;
        default:
          throw new ArgumentException("Conjunction");
      }
      return flag;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(").Append(this.Left != null ? this.Left.ToString() : "null").Append(" ").Append(this.Conjunction.ToString()).Append(" ").Append(this.Right != null ? this.Right.ToString() : "null").Append(")");
      return stringBuilder.ToString();
    }

    internal override bool Evaluate(ZipEntry entry)
    {
      bool flag = this.Left.Evaluate(entry);
      switch (this.Conjunction)
      {
        case LogicalConjunction.AND:
          if (flag)
          {
            flag = this.Right.Evaluate(entry);
            break;
          }
          break;
        case LogicalConjunction.OR:
          if (!flag)
          {
            flag = this.Right.Evaluate(entry);
            break;
          }
          break;
        case LogicalConjunction.XOR:
          flag ^= this.Right.Evaluate(entry);
          break;
      }
      return flag;
    }
  }
}
