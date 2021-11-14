// Decompiled with JetBrains decompiler
// Type: Ionic.ComparisonOperator
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System.ComponentModel;

namespace Ionic
{
  internal enum ComparisonOperator
  {
    [Description(">")] GreaterThan,
    [Description(">=")] GreaterThanOrEqualTo,
    [Description("<")] LesserThan,
    [Description("<=")] LesserThanOrEqualTo,
    [Description("=")] EqualTo,
    [Description("!=")] NotEqualTo,
  }
}
