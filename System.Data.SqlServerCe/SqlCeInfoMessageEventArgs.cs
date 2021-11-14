// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeInfoMessageEventArgs
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Security;

namespace System.Data.SqlServerCe
{
  public sealed class SqlCeInfoMessageEventArgs : EventArgs
  {
    private object src;
    private SqlCeErrorCollection errors;

    static SqlCeInfoMessageEventArgs() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal SqlCeInfoMessageEventArgs(int hr, IntPtr pError, object src)
    {
      this.src = src;
      this.errors = new SqlCeErrorCollection();
      this.errors.FillErrorInformation(hr, pError);
    }

    public SqlCeErrorCollection Errors => this.errors;

    public string Message => this.Errors[0].Message;

    public override string ToString() => this.Message;
  }
}
