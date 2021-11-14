// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeDelegatedTransaction
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Transactions;

namespace System.Data.SqlServerCe
{
  internal sealed class SqlCeDelegatedTransaction : 
    IPromotableSinglePhaseNotification,
    ITransactionPromoter
  {
    private SqlCeConnection _connection;
    private System.Data.IsolationLevel seIsolationLevel;
    private SqlCeTransaction _sqlCeTransaction;
    private Transaction _atomicTransaction;

    private void CloseDelegatedConnection(SqlCeConnection connection) => connection.Close();

    static SqlCeDelegatedTransaction() => KillBitHelper.ThrowIfKillBitIsSet();

    internal SqlCeDelegatedTransaction(SqlCeConnection connection, Transaction tx)
    {
      this._connection = connection;
      this._atomicTransaction = tx;
      System.Transactions.IsolationLevel isolationLevel = tx.IsolationLevel;
      switch (isolationLevel)
      {
        case System.Transactions.IsolationLevel.Serializable:
          this.seIsolationLevel = System.Data.IsolationLevel.Serializable;
          break;
        case System.Transactions.IsolationLevel.RepeatableRead:
          this.seIsolationLevel = System.Data.IsolationLevel.RepeatableRead;
          break;
        case System.Transactions.IsolationLevel.ReadCommitted:
          this.seIsolationLevel = System.Data.IsolationLevel.ReadCommitted;
          break;
        case System.Transactions.IsolationLevel.ReadUncommitted:
          this.seIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
          break;
        case System.Transactions.IsolationLevel.Unspecified:
          this.seIsolationLevel = System.Data.IsolationLevel.Unspecified;
          break;
        default:
          throw new ArgumentException(Res.GetString("ADP_InvalidIsolationLevel", (object) isolationLevel.ToString()));
      }
    }

    internal Transaction Transaction => this._atomicTransaction;

    public void Initialize() => this._sqlCeTransaction = this._connection.BeginTransaction(this.seIsolationLevel);

    internal static TransactionPromotionException PromotionFailed(
      Exception inner)
    {
      return new TransactionPromotionException();
    }

    public byte[] Promote() => throw SqlCeDelegatedTransaction.PromotionFailed((Exception) null);

    public void Rollback(SinglePhaseEnlistment enlistment)
    {
      if (this._sqlCeTransaction == null)
      {
        enlistment.Aborted();
      }
      else
      {
        if (this._connection.HasOpenedCursors(this._sqlCeTransaction))
          this._connection.DisposeSqlCeDataRdr(this._sqlCeTransaction);
        this._sqlCeTransaction.Rollback();
        this._connection.DelegatedTransaction = (SqlCeDelegatedTransaction) null;
        if (this._connection.State == ConnectionState.Closed)
          this.CloseDelegatedConnection(this._connection);
        enlistment.Aborted();
      }
    }

    public void SinglePhaseCommit(SinglePhaseEnlistment enlistment)
    {
      if (this._sqlCeTransaction == null)
      {
        enlistment.Committed();
      }
      else
      {
        if (this._connection.HasOpenedCursors(this._sqlCeTransaction))
          this._connection.DisposeSqlCeDataRdr(this._sqlCeTransaction);
        this._sqlCeTransaction.Commit();
        this._connection.DelegatedTransaction = (SqlCeDelegatedTransaction) null;
        if (this._connection.State == ConnectionState.Closed)
          this.CloseDelegatedConnection(this._connection);
        enlistment.Committed();
      }
    }

    public SqlCeTransaction SqlCeTransaction
    {
      get => this._sqlCeTransaction;
      set => this._sqlCeTransaction = value;
    }
  }
}
