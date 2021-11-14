// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ResultSetChangedEventArgs
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal class ResultSetChangedEventArgs : EventArgs
  {
    private RowView rowView;
    private ChangeType changeType;

    static ResultSetChangedEventArgs() => KillBitHelper.ThrowIfKillBitIsSet();

    public ResultSetChangedEventArgs(ChangeType type, RowView view)
    {
      this.rowView = view;
      this.changeType = type;
    }

    internal RowView RowView => this.rowView;

    internal ChangeType ChangeType => this.changeType;
  }
}
