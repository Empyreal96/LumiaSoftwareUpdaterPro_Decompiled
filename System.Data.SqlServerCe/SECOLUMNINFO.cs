// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SECOLUMNINFO
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Runtime.InteropServices;

namespace System.Data.SqlServerCe
{
  [StructLayout(LayoutKind.Sequential)]
  internal class SECOLUMNINFO
  {
    public IntPtr pwszColumn = IntPtr.Zero;
    public uint cchMax;
    public SETYPE type;
    public uint ulSize;
    public uint bPrecision;
    public uint bScale;
    public long lSeed;
    public long lStep;
    public int fIsFixed;
    public int fIsNullable;
    public int fIsIdentity;
    public int fIsRowGuid;
    public int fIsWriteable;
    public int fIsSystem;
    public int fIsRowVersion;
    public long lIdentityMin;
    public long lIdentityMax;
    public long lIdentityNext;
    public int fMaybeNull;
    public IntPtr pwszBaseTable = IntPtr.Zero;
    public uint cchMaxBaseTable;
    public IntPtr pwszBaseColumn = IntPtr.Zero;
    public uint cchMaxBaseColumn;
    public uint dwCedbPropId;
    public IntPtr pwszDefaultExpr = IntPtr.Zero;
    public uint cchMaxDefaultExpr;
    public int fHasDefault;
    public int fIsExpression;
    public int fCompressed;
    public long lIdentityMinEx;
    public long lIdentityMaxEx;
    public int fUseExRange;
    public int fUseOverflowRange;

    static SECOLUMNINFO() => KillBitHelper.ThrowIfKillBitIsSet();
  }
}
