// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.ManagedIpHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.LsuPro.Helpers
{
  public static class ManagedIpHelper
  {
    public static TcpTableCollection GetExtendedTcpTable(bool sorted)
    {
      List<TcpRow> tcpRowList = new List<TcpRow>();
      IntPtr num = IntPtr.Zero;
      int tcpTableLength = 0;
      if (IpHelper.GetExtendedTcpTable(num, ref tcpTableLength, sorted, 2, IpHelper.TcpTableType.OwnerPidAll, 0) != 0U)
      {
        try
        {
          num = Marshal.AllocHGlobal(tcpTableLength);
          if (IpHelper.GetExtendedTcpTable(num, ref tcpTableLength, true, 2, IpHelper.TcpTableType.OwnerPidAll, 0) == 0U)
          {
            IpHelper.TcpTable structure = (IpHelper.TcpTable) Marshal.PtrToStructure(num, typeof (IpHelper.TcpTable));
            IntPtr ptr = (IntPtr) ((long) num + (long) Marshal.SizeOf((object) structure.Length));
            for (int index = 0; (long) index < (long) structure.Length; ++index)
            {
              tcpRowList.Add(new TcpRow((IpHelper.TcpRow) Marshal.PtrToStructure(ptr, typeof (IpHelper.TcpRow))));
              ptr = (IntPtr) ((long) ptr + (long) Marshal.SizeOf(typeof (IpHelper.TcpRow)));
            }
          }
        }
        finally
        {
          if (num != IntPtr.Zero)
            Marshal.FreeHGlobal(num);
        }
      }
      return new TcpTableCollection((IEnumerable<TcpRow>) tcpRowList);
    }
  }
}
