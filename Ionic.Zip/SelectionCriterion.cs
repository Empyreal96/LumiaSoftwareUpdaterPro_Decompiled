// Decompiled with JetBrains decompiler
// Type: Ionic.SelectionCriterion
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using Ionic.Zip;
using System.Diagnostics;

namespace Ionic
{
  internal abstract class SelectionCriterion
  {
    internal virtual bool Verbose { get; set; }

    internal abstract bool Evaluate(string filename);

    [Conditional("SelectorTrace")]
    protected static void CriterionTrace(string format, params object[] args)
    {
    }

    internal abstract bool Evaluate(ZipEntry entry);
  }
}
