// Decompiled with JetBrains decompiler
// Type: Ionic.AttributesCriterion
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Ionic
{
  internal class AttributesCriterion : SelectionCriterion
  {
    private FileAttributes _Attributes;
    internal ComparisonOperator Operator;

    internal string AttributeString
    {
      get
      {
        string str = "";
        if ((this._Attributes & FileAttributes.Hidden) != (FileAttributes) 0)
          str += "H";
        if ((this._Attributes & FileAttributes.System) != (FileAttributes) 0)
          str += "S";
        if ((this._Attributes & FileAttributes.ReadOnly) != (FileAttributes) 0)
          str += "R";
        if ((this._Attributes & FileAttributes.Archive) != (FileAttributes) 0)
          str += "A";
        if ((this._Attributes & FileAttributes.ReparsePoint) != (FileAttributes) 0)
          str += "L";
        if ((this._Attributes & FileAttributes.NotContentIndexed) != (FileAttributes) 0)
          str += "I";
        return str;
      }
      set
      {
        this._Attributes = FileAttributes.Normal;
        foreach (char ch in value.ToUpper())
        {
          switch (ch)
          {
            case 'A':
              if ((this._Attributes & FileAttributes.Archive) != (FileAttributes) 0)
                throw new ArgumentException(string.Format("Repeated flag. ({0})", (object) ch), nameof (value));
              this._Attributes |= FileAttributes.Archive;
              break;
            case 'H':
              if ((this._Attributes & FileAttributes.Hidden) != (FileAttributes) 0)
                throw new ArgumentException(string.Format("Repeated flag. ({0})", (object) ch), nameof (value));
              this._Attributes |= FileAttributes.Hidden;
              break;
            case 'I':
              if ((this._Attributes & FileAttributes.NotContentIndexed) != (FileAttributes) 0)
                throw new ArgumentException(string.Format("Repeated flag. ({0})", (object) ch), nameof (value));
              this._Attributes |= FileAttributes.NotContentIndexed;
              break;
            case 'L':
              if ((this._Attributes & FileAttributes.ReparsePoint) != (FileAttributes) 0)
                throw new ArgumentException(string.Format("Repeated flag. ({0})", (object) ch), nameof (value));
              this._Attributes |= FileAttributes.ReparsePoint;
              break;
            case 'R':
              if ((this._Attributes & FileAttributes.ReadOnly) != (FileAttributes) 0)
                throw new ArgumentException(string.Format("Repeated flag. ({0})", (object) ch), nameof (value));
              this._Attributes |= FileAttributes.ReadOnly;
              break;
            case 'S':
              if ((this._Attributes & FileAttributes.System) != (FileAttributes) 0)
                throw new ArgumentException(string.Format("Repeated flag. ({0})", (object) ch), nameof (value));
              this._Attributes |= FileAttributes.System;
              break;
            default:
              throw new ArgumentException(value);
          }
        }
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("attributes ").Append(EnumUtil.GetDescription((Enum) this.Operator)).Append(" ").Append(this.AttributeString);
      return stringBuilder.ToString();
    }

    private bool _EvaluateOne(FileAttributes fileAttrs, FileAttributes criterionAttrs) => (this._Attributes & criterionAttrs) != criterionAttrs || (fileAttrs & criterionAttrs) == criterionAttrs;

    internal override bool Evaluate(string filename) => Directory.Exists(filename) ? this.Operator != ComparisonOperator.EqualTo : this._Evaluate(File.GetAttributes(filename));

    private bool _Evaluate(FileAttributes fileAttrs)
    {
      bool flag = this._EvaluateOne(fileAttrs, FileAttributes.Hidden);
      if (flag)
        flag = this._EvaluateOne(fileAttrs, FileAttributes.System);
      if (flag)
        flag = this._EvaluateOne(fileAttrs, FileAttributes.ReadOnly);
      if (flag)
        flag = this._EvaluateOne(fileAttrs, FileAttributes.Archive);
      if (flag)
        flag = this._EvaluateOne(fileAttrs, FileAttributes.NotContentIndexed);
      if (flag)
        flag = this._EvaluateOne(fileAttrs, FileAttributes.ReparsePoint);
      if (this.Operator != ComparisonOperator.EqualTo)
        flag = !flag;
      return flag;
    }

    internal override bool Evaluate(ZipEntry entry) => this._Evaluate(entry.Attributes);
  }
}
