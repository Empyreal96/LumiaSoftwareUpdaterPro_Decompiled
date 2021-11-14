// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.TcpRow
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;

namespace Microsoft.LsuPro.Helpers
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public class TcpRow
  {
    private IPEndPoint localEndPoint;
    private IPEndPoint remoteEndPoint;
    private TcpState state;
    private int processId;

    public TcpRow(IpHelper.TcpRow tcpRow)
    {
      this.state = tcpRow.State;
      this.processId = tcpRow.OwningPid;
      int port1 = ((int) tcpRow.LocalPort1 << 8) + (int) tcpRow.LocalPort2 + ((int) tcpRow.LocalPort3 << 24) + ((int) tcpRow.LocalPort4 << 16);
      this.localEndPoint = new IPEndPoint((long) tcpRow.LocalAddr, port1);
      int port2 = ((int) tcpRow.RemotePort1 << 8) + (int) tcpRow.RemotePort2 + ((int) tcpRow.RemotePort3 << 24) + ((int) tcpRow.RemotePort4 << 16);
      this.remoteEndPoint = new IPEndPoint((long) tcpRow.RemoteAddr, port2);
    }

    public IPEndPoint LocalEndPoint => this.localEndPoint;

    public IPEndPoint RemoteEndPoint => this.remoteEndPoint;

    public TcpState State => this.state;

    public int ProcessId => this.processId;
  }
}
