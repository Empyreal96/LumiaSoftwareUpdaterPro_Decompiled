// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.MetaData
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.SqlTypes;
using System.Security;

namespace System.Data.SqlServerCe
{
  internal class MetaData : IDisposable
  {
    internal uint ordinal;
    internal uint size;
    internal object value;
    internal bool isReadOnly;
    internal bool isRowVersion;
    internal bool isExpression;
    internal bool isIdentity;
    internal bool isUnique;
    internal bool isKey;
    internal bool isNullable;
    internal bool hasDefault;
    internal string baseTableName;
    internal string baseColumnName;
    internal SqlCeType typeMap;
    private SqlMetaData sqlMetaData;
    private object thisLock = new object();
    private bool isFinalized;

    public SqlMetaData SqlMetaData => this.sqlMetaData;

    public string ColumnName => this.sqlMetaData.Name;

    static MetaData() => KillBitHelper.ThrowIfKillBitIsSet();

    private MetaData()
    {
    }

    public MetaData(
      string name,
      SqlCeType typeMap,
      byte precision,
      byte scale,
      long maxLength,
      string databaseName,
      string schemaName)
    {
      this.typeMap = typeMap;
      SqlDbType sqlDbType = typeMap.sqlDbType;
      if (precision == (byte) 0)
        precision = typeMap.maxpre;
      if (0L == maxLength)
        maxLength = (long) typeMap.fixlen;
      if (scale == (byte) 0)
        scale = typeMap.scale;
      if (sqlDbType == SqlDbType.NText || sqlDbType == SqlDbType.Image)
        maxLength = -1L;
      if (sqlDbType == SqlDbType.NVarChar && maxLength > 4000L)
        maxLength = 4000L;
      else if (sqlDbType == SqlDbType.VarBinary && maxLength > 8000L)
        maxLength = 8000L;
      this.sqlMetaData = new SqlMetaData(name, sqlDbType, maxLength, precision, scale, 0L, SqlCompareOptions.None, (Type) null);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~MetaData() => this.Dispose(false);

    [SecurityCritical]
    [SecurityTreatAsSafe]
    private void Dispose(bool disposing)
    {
      lock (this.thisLock)
      {
        if (this.isFinalized)
          return;
        if (this.value != null && this.value is IntPtr && this.typeMap.isBLOB)
        {
          IntPtr ppUnknown = (IntPtr) this.value;
          if (IntPtr.Zero != ppUnknown)
          {
            NativeMethods.SafeRelease(ref ppUnknown);
            this.value = (object) null;
          }
        }
        if (disposing)
        {
          this.value = (object) null;
          this.sqlMetaData = (SqlMetaData) null;
          this.baseTableName = (string) null;
          this.baseColumnName = (string) null;
          this.typeMap = (SqlCeType) null;
        }
        if (disposing)
          return;
        this.isFinalized = true;
      }
    }
  }
}
