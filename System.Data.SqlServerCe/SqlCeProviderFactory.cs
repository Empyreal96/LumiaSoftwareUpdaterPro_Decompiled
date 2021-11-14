// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeProviderFactory
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.Common;
using System.Security;

namespace System.Data.SqlServerCe
{
  public sealed class SqlCeProviderFactory : DbProviderFactory, IServiceProvider
  {
    public static readonly SqlCeProviderFactory Instance = new SqlCeProviderFactory();

    static SqlCeProviderFactory() => KillBitHelper.ThrowIfKillBitIsSet();

    public override DbCommand CreateCommand() => (DbCommand) new SqlCeCommand();

    public override DbCommandBuilder CreateCommandBuilder() => (DbCommandBuilder) new SqlCeCommandBuilder();

    public override DbConnection CreateConnection() => (DbConnection) new SqlCeConnection();

    public override DbDataAdapter CreateDataAdapter() => (DbDataAdapter) new SqlCeDataAdapter();

    public override DbParameter CreateParameter() => (DbParameter) new SqlCeParameter();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => (DbConnectionStringBuilder) new SqlCeConnectionStringBuilder();

    [SecurityCritical]
    [SecurityTreatAsSafe]
    object IServiceProvider.GetService(Type serviceType)
    {
      object obj = (object) null;
      if (serviceType == ExtensionMethods.SystemDataCommonDbProviderServices_Type)
        obj = ExtensionMethods.SystemDataSqlServerCeSqlCeProviderServices_Instance();
      return obj;
    }
  }
}
