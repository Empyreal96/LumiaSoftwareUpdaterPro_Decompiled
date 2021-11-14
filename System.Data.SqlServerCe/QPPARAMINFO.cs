// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.QPPARAMINFO
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Runtime.InteropServices;

namespace System.Data.SqlServerCe
{
  [StructLayout(LayoutKind.Sequential)]
  internal class QPPARAMINFO
  {
    public IntPtr pwszParam = IntPtr.Zero;
    public uint cchMax;
    public uint iOrdinal;
    public QPPARAMTYPE paramType;
    public SETYPE type;
    public uint ulSize;
    public byte bPrecision;
    public byte bScale;
    public short padding;
    public int fIsTyped;

    static QPPARAMINFO() => KillBitHelper.ThrowIfKillBitIsSet();
  }
}
