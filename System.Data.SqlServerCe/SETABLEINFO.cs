// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SETABLEINFO
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Runtime.InteropServices;

namespace System.Data.SqlServerCe
{
  [StructLayout(LayoutKind.Sequential)]
  internal class SETABLEINFO
  {
    public IntPtr pwszTable = IntPtr.Zero;
    public uint cchMax;
    public bool fIsSystem;
    public bool fReadOnly;
    public ulong uhVersion;
    public bool fTemporary;
    public bool fOrdered;
    public long lNextIdentity;
    public bool fIdentityOverflow;
    public ushort wTracking;
    public int lTableNick;
    public uint dwCedbType;
    public int hBookmark;
    public uint cPages;
    public uint cLvPages;
    public uint dwGrantedOps;
    public bool fHasDefaults;
    public bool fCompressed;
    public uint cStatMods;

    static SETABLEINFO() => KillBitHelper.ThrowIfKillBitIsSet();
  }
}
