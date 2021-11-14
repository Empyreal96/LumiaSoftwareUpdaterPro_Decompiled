// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.KeyValuePairsEnumerator`2
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Collections.Generic;

namespace System.Data.SqlServerCe
{
  internal class KeyValuePairsEnumerator<TKey, TValue> : IEnumerator
  {
    private IEnumerator<KeyValuePair<TKey, TValue>> m_collection;

    static KeyValuePairsEnumerator() => KillBitHelper.ThrowIfKillBitIsSet();

    public KeyValuePairsEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator) => this.m_collection = enumerator;

    public void Reset() => this.m_collection.Reset();

    public bool MoveNext() => this.m_collection.MoveNext();

    public object Current => (object) this.m_collection.Current.Value;
  }
}
