// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.IpHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Microsoft.LsuPro.Helpers
{
  public static class IpHelper
  {
    public const string DllName = "iphlpapi.dll";
    public const int AfInet = 2;

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "reviewed", MessageId = "1#")]
    [DllImport("iphlpapi.dll", SetLastError = true)]
    public static extern uint GetExtendedTcpTable(
      IntPtr tcpTable,
      ref int tcpTableLength,
      [MarshalAs(UnmanagedType.Bool)] bool sort,
      int ipVersion,
      IpHelper.TcpTableType tcpTableType,
      int reserved);

    public enum TcpTableType
    {
      BasicListener,
      BasicConnections,
      BasicAll,
      OwnerPidListener,
      OwnerPidConnections,
      OwnerPidAll,
      OwnerModuleListener,
      OwnerModuleConnections,
      OwnerModuleAll,
    }

    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not needed")]
    public struct TcpTable
    {
      public uint Length;
      public IpHelper.TcpRow Row;
    }

    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not needed")]
    public struct TcpRow
    {
      public TcpState State;
      public uint LocalAddr;
      public byte LocalPort1;
      public byte LocalPort2;
      public byte LocalPort3;
      public byte LocalPort4;
      public uint RemoteAddr;
      public byte RemotePort1;
      public byte RemotePort2;
      public byte RemotePort3;
      public byte RemotePort4;
      public int OwningPid;
    }
  }
}
