// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ResultSetEnumerator
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;

namespace System.Data.SqlServerCe
{
  public sealed class ResultSetEnumerator : IEnumerator
  {
    internal SqlCeResultSet _resultset;
    internal SqlCeUpdatableRecord _current;

    static ResultSetEnumerator() => KillBitHelper.ThrowIfKillBitIsSet();

    public ResultSetEnumerator(SqlCeResultSet resultSet) => this._resultset = resultSet != null ? resultSet : throw new ArgumentNullException("resultset");

    object IEnumerator.Current => (object) this._current;

    public SqlCeUpdatableRecord Current => this._current;

    public bool MoveNext()
    {
      this._current = (SqlCeUpdatableRecord) null;
      if (!this._resultset.Read())
        return false;
      this._current = this._resultset.GetCurrentRecord();
      return true;
    }

    public void Reset()
    {
      this._current = (SqlCeUpdatableRecord) null;
      this._resultset.ReadFirst();
      this._resultset.ReadPrevious();
    }
  }
}
