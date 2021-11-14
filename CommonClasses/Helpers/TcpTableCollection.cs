// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.TcpTableCollection
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.LsuPro.Helpers
{
  public class TcpTableCollection : IEnumerable<TcpRow>, IEnumerable
  {
    private IEnumerable<TcpRow> tcpRows;

    public TcpTableCollection(IEnumerable<TcpRow> tcpRows) => this.tcpRows = tcpRows;

    public IEnumerable<TcpRow> Rows => this.tcpRows;

    public IEnumerator<TcpRow> GetEnumerator() => this.tcpRows.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.tcpRows.GetEnumerator();
  }
}
