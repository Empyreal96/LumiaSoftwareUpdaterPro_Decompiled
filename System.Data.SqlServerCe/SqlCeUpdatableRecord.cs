// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeUpdatableRecord
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.SqlTypes;
using System.Globalization;

namespace System.Data.SqlServerCe
{
  public class SqlCeUpdatableRecord : IDataRecord
  {
    internal object[] values;
    private SqlMetaData[] sqlMetaDatas;
    private bool updatable;
    private bool[] boundFields;
    private string[] fieldNames;
    private FieldNameLookup fieldNameLookup;
    private int hiddenFieldCount;
    private ColumnUpdatedStatus[] columnsUpdated;
    private Type SqlBinaryType = typeof (SqlBinary);
    private Type SqlBooleanType = typeof (SqlBoolean);
    private Type SqlByteType = typeof (SqlByte);
    private Type SqlDateTimeType = typeof (SqlDateTime);
    private Type SqlDecimalType = typeof (SqlDecimal);
    private Type SqlDoubleType = typeof (SqlDouble);
    private Type SqlGuidType = typeof (SqlGuid);
    private Type SqlInt16Type = typeof (SqlInt16);
    private Type SqlInt32Type = typeof (SqlInt32);
    private Type SqlInt64Type = typeof (SqlInt64);
    private Type SqlMoneyType = typeof (SqlMoney);
    private Type SqlSingleType = typeof (SqlSingle);
    private Type SqlStringType = typeof (SqlString);

    static SqlCeUpdatableRecord() => KillBitHelper.ThrowIfKillBitIsSet();

    internal SqlCeUpdatableRecord(
      MetaData[] metaData,
      object[] values,
      bool updatable,
      int hiddenFieldCount,
      FieldNameLookup fieldNameLookup)
    {
      NativeMethods.LoadNativeBinaries();
      int length = metaData.Length;
      this.values = values;
      this.updatable = updatable;
      this.fieldNameLookup = fieldNameLookup;
      this.hiddenFieldCount = hiddenFieldCount;
      this.sqlMetaDatas = new SqlMetaData[length];
      this.columnsUpdated = new ColumnUpdatedStatus[length];
      this.fieldNames = new string[length];
      for (int index = 0; index < length; ++index)
      {
        this.sqlMetaDatas[index] = metaData[index].SqlMetaData;
        this.fieldNames[index] = metaData[index].baseColumnName;
        this.columnsUpdated[index] = ColumnUpdatedStatus.None;
      }
      if (this.fieldNameLookup == null)
        this.fieldNameLookup = new FieldNameLookup(this.fieldNames, -1);
      this.boundFields = new bool[length];
    }

    public int FieldCount => this.sqlMetaDatas.Length;

    public int HiddenFieldCount => this.hiddenFieldCount;

    public bool Updatable => this.updatable;

    internal ColumnUpdatedStatus[] ColumnsUpdatedStatus => this.columnsUpdated;

    public void SetDefault(int ordinal)
    {
      this.EnsureObjectArray(ordinal, nameof (SetDefault));
      this.columnsUpdated[ordinal] = ColumnUpdatedStatus.ServerDefault;
    }

    public int GetValues(object[] values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      int num1 = values.Length < this.sqlMetaDatas.Length ? values.Length : this.sqlMetaDatas.Length;
      int num2 = num1 < this.values.Length ? num1 : this.values.Length;
      for (int index = 0; index < num2; ++index)
      {
        object obj = this.values[index];
        values[index] = !(obj is SqlDecimal sqlDecimal2) ? obj : (object) (Decimal) sqlDecimal2;
      }
      return num2;
    }

    public int SetValues(object[] values)
    {
      this.EnsureObjectArray(values.Length, nameof (SetValues));
      for (int ordinal = 0; ordinal < values.Length; ++ordinal)
        this.SetValue(ordinal, values[ordinal]);
      return values.Length;
    }

    public string GetName(int ordinal) => this.fieldNames[ordinal];

    public object GetValue(int ordinal)
    {
      object obj = this.values[ordinal];
      return obj is SqlDecimal sqlDecimal ? (object) (Decimal) sqlDecimal : obj;
    }

    public string GetDataTypeName(int ordinal) => this.sqlMetaDatas[ordinal].TypeName;

    public Type GetFieldType(int ordinal) => SqlCeType.FromSqlDbType(this.sqlMetaDatas[ordinal].SqlDbType).clrType;

    public IDataReader GetData(int ordinal) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (GetData)));

    public int GetOrdinal(string name) => this.fieldNameLookup.GetOrdinal(name);

    public bool IsSetAsDefault(int ordinal) => ColumnUpdatedStatus.ServerDefault == this.columnsUpdated[ordinal];

    public object this[int ordinal]
    {
      get => this.GetValue(ordinal);
      set => this.SetValue(ordinal, value);
    }

    public object this[string name]
    {
      get => this.GetValue(this.GetOrdinal(name));
      set => this.SetValue(this.GetOrdinal(name), value);
    }

    public bool IsDBNull(int ordinal)
    {
      if (this.values == null)
        this.EnsureObjectArray(ordinal, nameof (IsDBNull));
      object obj = this.values[ordinal];
      if (obj is INullable)
        return ((INullable) obj).IsNull;
      return obj == null || obj == DBNull.Value;
    }

    private void EnsureObjectArray(int ordinal, string method)
    {
      if (ordinal > this.sqlMetaDatas.Length)
        throw new IndexOutOfRangeException(Res.GetString("SQLCE_OrdinalOutOfRange", (object) ordinal, (object) method));
      if (!this.Updatable)
        throw new InvalidOperationException(Res.GetString("SQL_SqlRecordReadOnly", (object) method));
      if (this.values == null)
        this.values = new object[this.sqlMetaDatas.Length];
      if (this.columnsUpdated != null)
        return;
      this.columnsUpdated = new ColumnUpdatedStatus[this.sqlMetaDatas.Length];
    }

    private void SetClrTypeValue(int ordinal, object value, string method)
    {
      this.EnsureObjectArray(ordinal, method);
      if (value == DBNull.Value)
      {
        this.values[ordinal] = (object) DBNull.Value;
        this.boundFields[ordinal] = false;
        this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
      }
      else
      {
        SqlDbType sqlDbType1 = SqlCeType.FromClrType(value).SqlDbType;
        SqlDbType sqlDbType2 = this.sqlMetaDatas[ordinal].SqlDbType;
        SqlCeType.ValidateDataConversion(sqlDbType2, sqlDbType1);
        switch (sqlDbType2)
        {
          case SqlDbType.BigInt:
            if (!(value is long))
            {
              value = (object) Convert.ToInt64(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.Binary:
          case SqlDbType.Image:
          case SqlDbType.Timestamp:
          case SqlDbType.VarBinary:
            if (!(value is byte[]))
              throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) typeof (byte[])));
            break;
          case SqlDbType.Bit:
            if (!(value is bool))
            {
              value = (object) Convert.ToBoolean(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.DateTime:
          case SqlDbType.SmallDateTime:
            if (!(value is DateTime))
            {
              value = (object) Convert.ToDateTime(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.Decimal:
            if (!(value is SqlDecimal))
            {
              value = (object) new SqlDecimal(Convert.ToDecimal(value, (IFormatProvider) CultureInfo.CurrentCulture));
              break;
            }
            break;
          case SqlDbType.Float:
            if (!(value is double))
            {
              value = (object) Convert.ToDouble(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.Int:
            if (!(value is int))
            {
              value = (object) Convert.ToInt32(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.Money:
          case SqlDbType.SmallMoney:
            if (!(value is Decimal))
            {
              value = (object) Convert.ToDecimal(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.NChar:
          case SqlDbType.NText:
          case SqlDbType.NVarChar:
            if (!(value is string))
            {
              value = (object) Convert.ToString(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.Real:
            if (!(value is float))
            {
              value = (object) Convert.ToSingle(value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.UniqueIdentifier:
            switch (value)
            {
              case Guid _:
                break;
              case string _:
                value = (object) new Guid((string) value);
                break;
              case byte[] _:
                value = (object) new Guid((byte[]) value);
                break;
              default:
                throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) SqlDbType.UniqueIdentifier));
            }
            break;
          case SqlDbType.SmallInt:
            if (!(value is short))
            {
              value = (object) Convert.ToInt16((object) (ushort) value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            break;
          case SqlDbType.TinyInt:
            if (value is string)
            {
              value = (object) Convert.ToByte((string) value, (IFormatProvider) CultureInfo.CurrentCulture);
              break;
            }
            try
            {
              value = (object) (byte) value;
              break;
            }
            catch (InvalidCastException ex)
            {
              throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) SqlDbType.TinyInt));
            }
        }
        this.values[ordinal] = value;
        this.boundFields[ordinal] = false;
        this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
      }
    }

    private void SetSqlTypeValue(int ordinal, INullable value, string method)
    {
      this.EnsureObjectArray(ordinal, method);
      if (value.IsNull)
      {
        this.values[ordinal] = (object) DBNull.Value;
      }
      else
      {
        object obj = (object) null;
        Type type = value.GetType();
        if (type == this.SqlBinaryType)
          obj = (object) ((SqlBinary) value).Value;
        else if (type == this.SqlBooleanType)
          obj = (object) ((SqlBoolean) value).Value;
        else if (type == this.SqlByteType)
          obj = (object) ((SqlByte) value).Value;
        else if (type == this.SqlDateTimeType)
          obj = (object) ((SqlDateTime) value).Value;
        else if (type == this.SqlDecimalType)
          obj = (object) (SqlDecimal) value;
        else if (type == this.SqlDoubleType)
          obj = (object) ((SqlDouble) value).Value;
        else if (type == this.SqlGuidType)
          obj = (object) ((SqlGuid) value).Value;
        else if (type == this.SqlInt16Type)
          obj = (object) ((SqlInt16) value).Value;
        else if (type == this.SqlInt32Type)
          obj = (object) ((SqlInt32) value).Value;
        else if (type == this.SqlInt64Type)
          obj = (object) ((SqlInt64) value).Value;
        else if (type == this.SqlMoneyType)
          obj = (object) ((SqlMoney) value).Value;
        else if (type == this.SqlSingleType)
          obj = (object) ((SqlSingle) value).Value;
        else if (type == this.SqlStringType)
          obj = (object) ((SqlString) value).Value;
        this.values[ordinal] = obj;
      }
      this.boundFields[ordinal] = false;
      this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
    }

    private void ValidateType(int ordinal, object value, SqlDbType type)
    {
      if (this.sqlMetaDatas[ordinal].SqlDbType != type)
        throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) this.sqlMetaDatas[ordinal].SqlDbType));
    }

    public bool GetBoolean(int ordinal) => !this.IsDBNull(ordinal) ? (bool) this.values[ordinal] : throw new SqlNullValueException();

    public byte GetByte(int ordinal) => !this.IsDBNull(ordinal) ? (byte) this.values[ordinal] : throw new SqlNullValueException();

    public long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
    {
      byte[] numArray = !this.IsDBNull(ordinal) ? (byte[]) this.values[ordinal] : throw new SqlNullValueException();
      if (buffer == null)
        return (long) numArray.Length;
      if ((int) dataIndex > numArray.Length || numArray.Length == 0)
        return 0;
      if (dataIndex < 0L)
        throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) buffer.Length, (object) dataIndex));
      if (bufferIndex < 0 || bufferIndex >= buffer.Length)
        throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidDestinationBufferIndex", (object) buffer.Length, (object) bufferIndex));
      int num1 = buffer.Length - bufferIndex;
      int num2 = (int) ((long) numArray.Length - dataIndex);
      if (length > num1 && num2 > num1)
        throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_TooManyBytesAvailable", (object) num2, (object) num1));
      int length1 = Math.Min(buffer.Length - bufferIndex, length);
      if (dataIndex > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) length1.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) dataIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if ((long) numArray.Length - dataIndex < (long) length1)
        length1 = numArray.Length - (int) dataIndex;
      if (length1 < 0)
        throw new IndexOutOfRangeException(Res.GetString("SQL_InvalidDataLength", (object) length.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      Array.Copy((Array) numArray, (int) dataIndex, (Array) buffer, bufferIndex, length1);
      return (long) length1;
    }

    public long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
    {
      char[] chArray = !this.IsDBNull(ordinal) ? ((string) this.values[ordinal]).ToCharArray() : throw new SqlNullValueException();
      if (buffer == null)
        return (long) chArray.Length;
      if ((int) dataIndex > chArray.Length || chArray.Length == 0)
        return 0;
      if (dataIndex < 0L)
        throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) buffer.Length, (object) dataIndex));
      if (bufferIndex < 0 || bufferIndex >= buffer.Length)
        throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidDestinationBufferIndex", (object) buffer.Length, (object) bufferIndex));
      int num1 = buffer.Length - bufferIndex;
      int num2 = (int) ((long) chArray.Length - dataIndex);
      if (length > num1 && num2 > num1)
        throw new ArgumentOutOfRangeException(Res.GetString("SQLCE_TooManyCharsAvailable", (object) num2, (object) num1));
      int length1 = Math.Min(buffer.Length - bufferIndex, length);
      if (dataIndex > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) length1.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) dataIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if ((long) chArray.Length - dataIndex < (long) length1)
        length1 = chArray.Length - (int) dataIndex;
      if (length1 < 0)
        throw new IndexOutOfRangeException(Res.GetString("SQL_InvalidDataLength", (object) length.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      Array.Copy((Array) chArray, (int) dataIndex, (Array) buffer, bufferIndex, length1);
      return (long) length1;
    }

    public char GetChar(int ordinal) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (GetChar)));

    public Guid GetGuid(int ordinal) => !this.IsDBNull(ordinal) ? (Guid) this.values[ordinal] : throw new SqlNullValueException();

    public short GetInt16(int ordinal) => !this.IsDBNull(ordinal) ? (short) this.values[ordinal] : throw new SqlNullValueException();

    public int GetInt32(int ordinal) => !this.IsDBNull(ordinal) ? (int) this.values[ordinal] : throw new SqlNullValueException();

    public long GetInt64(int ordinal) => !this.IsDBNull(ordinal) ? (long) this.values[ordinal] : throw new SqlNullValueException();

    public float GetFloat(int ordinal) => !this.IsDBNull(ordinal) ? (float) this.values[ordinal] : throw new SqlNullValueException();

    public double GetDouble(int ordinal) => !this.IsDBNull(ordinal) ? (double) this.values[ordinal] : throw new SqlNullValueException();

    public string GetString(int ordinal)
    {
      object obj = !this.IsDBNull(ordinal) ? this.values[ordinal] : throw new SqlNullValueException();
      return obj is char[] ? new string((char[]) obj) : (string) obj;
    }

    public Decimal GetDecimal(int ordinal) => !this.IsDBNull(ordinal) ? this.GetSqlDecimal(ordinal).Value : throw new SqlNullValueException();

    public DateTime GetDateTime(int ordinal) => !this.IsDBNull(ordinal) ? (DateTime) this.values[ordinal] : throw new SqlNullValueException();

    public SqlMetaData GetSqlMetaData(int ordinal) => this.sqlMetaDatas[ordinal];

    public SqlBoolean GetSqlBoolean(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlBoolean.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((bool) this.values[ordinal]);
      return new SqlBoolean((bool) this.values[ordinal]);
    }

    public SqlBinary GetSqlBinary(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlBinary.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((byte[]) this.values[ordinal]);
      return new SqlBinary((byte[]) this.values[ordinal]);
    }

    public SqlByte GetSqlByte(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlByte.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((byte) this.values[ordinal]);
      return new SqlByte((byte) this.values[ordinal]);
    }

    public SqlInt16 GetSqlInt16(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlInt16.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((short) this.values[ordinal]);
      return new SqlInt16((short) this.values[ordinal]);
    }

    public SqlInt32 GetSqlInt32(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlInt32.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((int) this.values[ordinal]);
      return new SqlInt32((int) this.values[ordinal]);
    }

    public SqlInt64 GetSqlInt64(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlInt64.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((long) this.values[ordinal]);
      return new SqlInt64((long) this.values[ordinal]);
    }

    public SqlSingle GetSqlSingle(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlSingle.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((float) this.values[ordinal]);
      return new SqlSingle((float) this.values[ordinal]);
    }

    public SqlDouble GetSqlDouble(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlDouble.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((double) this.values[ordinal]);
      return new SqlDouble((double) this.values[ordinal]);
    }

    public SqlString GetSqlString(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlString.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((string) this.values[ordinal]);
      return new SqlString((string) this.values[ordinal]);
    }

    public SqlMoney GetSqlMoney(int ordinal) => this.IsDBNull(ordinal) ? SqlMoney.Null : new SqlMoney(this.GetSqlDecimal(ordinal).Value);

    public SqlDecimal GetSqlDecimal(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlDecimal.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((SqlDecimal) this.values[ordinal]);
      return this.values[ordinal] is SqlDecimal ? (SqlDecimal) this.values[ordinal] : new SqlDecimal((Decimal) this.values[ordinal]);
    }

    public SqlDateTime GetSqlDateTime(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlDateTime.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((DateTime) this.values[ordinal]);
      return new SqlDateTime((DateTime) this.values[ordinal]);
    }

    public SqlGuid GetSqlGuid(int ordinal)
    {
      if (this.IsDBNull(ordinal))
        return SqlGuid.Null;
      if (this.boundFields[ordinal])
        this.values[ordinal] = (object) this.sqlMetaDatas[ordinal].Adjust((Guid) this.values[ordinal]);
      return new SqlGuid((Guid) this.values[ordinal]);
    }

    public SqlChars GetSqlCharsRef(int ordinal) => (SqlChars) this.values[ordinal];

    public SqlBytes GetSqlBytesRef(int ordinal) => (SqlBytes) this.values[ordinal];

    public void SetObjectRef(int ordinal, object value)
    {
      this.EnsureObjectArray(ordinal, nameof (SetObjectRef));
      switch (value)
      {
        case char[] _:
        case byte[] _:
          this.values[ordinal] = value;
          this.boundFields[ordinal] = true;
          this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_InvalidDataType", (object) value.GetType().ToString(), (object) nameof (SetObjectRef)));
      }
    }

    public void SetValue(int ordinal, object value)
    {
      this.EnsureObjectArray(ordinal, nameof (SetValue));
      if (value == null)
        this.values[ordinal] = (object) null;
      else if (value is INullable)
        this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetValue));
      else
        this.SetClrTypeValue(ordinal, value, nameof (SetValue));
    }

    public void SetBoolean(int ordinal, bool value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Bit);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetBoolean));
    }

    public void SetByte(int ordinal, byte value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.TinyInt);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetByte));
    }

    public void SetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
    {
      this.EnsureObjectArray(ordinal, nameof (SetBytes));
      SqlDbType sqlDbType = this.sqlMetaDatas[ordinal].SqlDbType;
      switch (sqlDbType)
      {
        case SqlDbType.Binary:
        case SqlDbType.Image:
        case SqlDbType.VarBinary:
          if (buffer == null)
          {
            this.values[ordinal] = (object) DBNull.Value;
            break;
          }
          if (length < 0)
            throw new IndexOutOfRangeException(Res.GetString("SQL_InvalidDataLength", (object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (dataIndex < 0L)
            throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) dataIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (bufferIndex < 0 || bufferIndex > buffer.Length || bufferIndex == buffer.Length && length != 0)
            throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidDestinationBufferIndex", (object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (dataIndex + (long) length > (long) int.MaxValue)
            throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) int.MaxValue.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) dataIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (buffer.Length - bufferIndex < length)
            length = buffer.Length - bufferIndex;
          byte[] numArray1 = this.IsDBNull(ordinal) ? new byte[0] : (byte[]) this.values[ordinal];
          int length1 = Math.Max((int) dataIndex + length, numArray1.Length);
          int maxLength = (int) this.sqlMetaDatas[ordinal].MaxLength;
          if (maxLength != -1 && length1 > maxLength)
            throw new InvalidOperationException(Res.GetString("ADP_TruncatedBytes", (object) maxLength));
          byte[] numArray2;
          if (length1 > numArray1.Length)
          {
            numArray2 = new byte[length1];
            for (int index = 0; index < numArray1.Length; ++index)
              numArray2[index] = numArray1[index];
          }
          else
            numArray2 = numArray1;
          for (int index = 0; index < length; ++index)
            numArray2[(long) index + dataIndex] = buffer[index + bufferIndex];
          this.values[ordinal] = (object) numArray2;
          this.boundFields[ordinal] = false;
          this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) buffer.GetType().ToString(), (object) this.fieldNames[ordinal], (object) sqlDbType.ToString()));
      }
    }

    public void SetChar(int ordinal, char value) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (SetChar)));

    public void SetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
    {
      this.EnsureObjectArray(ordinal, nameof (SetChars));
      SqlDbType sqlDbType = this.sqlMetaDatas[ordinal].SqlDbType;
      switch (sqlDbType)
      {
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.NVarChar:
          if (buffer == null)
          {
            this.values[ordinal] = (object) DBNull.Value;
            break;
          }
          if (length < 0)
            throw new IndexOutOfRangeException(Res.GetString("SQL_InvalidDataLength", (object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (dataIndex < 0L)
            throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) dataIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (bufferIndex < 0 || bufferIndex > buffer.Length || bufferIndex == buffer.Length && length != 0)
            throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidDestinationBufferIndex", (object) buffer.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (dataIndex + (long) length > (long) int.MaxValue)
            throw new ArgumentOutOfRangeException(Res.GetString("ADP_InvalidSourceBufferIndex", (object) int.MaxValue.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) dataIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (buffer.Length - bufferIndex < length)
            length = buffer.Length - bufferIndex;
          string str1 = this.IsDBNull(ordinal) ? string.Empty : (string) this.values[ordinal];
          int length1 = Math.Max((int) dataIndex + length, str1.Length);
          int maxLength = (int) this.sqlMetaDatas[ordinal].MaxLength;
          char[] chArray = new char[length1];
          for (int index = 0; index < str1.Length; ++index)
            chArray[index] = str1[index];
          for (int index = 0; index < length; ++index)
            chArray[(long) index + dataIndex] = buffer[index + bufferIndex];
          string str2 = new string(chArray);
          if (maxLength != -1 && length1 > maxLength)
            throw new InvalidOperationException(Res.GetString("ADP_TruncatedString", (object) maxLength, (object) length1, (object) str2));
          this.values[ordinal] = (object) str2;
          this.boundFields[ordinal] = false;
          this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) buffer.GetType().ToString(), (object) this.fieldNames[ordinal], (object) sqlDbType.ToString()));
      }
    }

    public void SetGuid(int ordinal, Guid value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.UniqueIdentifier);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetGuid));
    }

    public void SetInt16(int ordinal, short value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.SmallInt);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetInt16));
    }

    public void SetInt32(int ordinal, int value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Int);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetInt32));
    }

    public void SetInt64(int ordinal, long value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.BigInt);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetInt64));
    }

    public void SetFloat(int ordinal, float value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Real);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetFloat));
    }

    public void SetDouble(int ordinal, double value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Float);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetDouble));
    }

    public void SetString(int ordinal, string value)
    {
      SqlDbType sqlDbType = this.sqlMetaDatas[ordinal].SqlDbType;
      switch (sqlDbType)
      {
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.NVarChar:
          this.SetClrTypeValue(ordinal, (object) value, nameof (SetString));
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) sqlDbType));
      }
    }

    public void SetDecimal(int ordinal, Decimal value)
    {
      SqlDbType sqlDbType = this.sqlMetaDatas[ordinal].SqlDbType;
      switch (sqlDbType)
      {
        case SqlDbType.Decimal:
        case SqlDbType.Money:
          this.EnsureObjectArray(ordinal, nameof (SetDecimal));
          value = this.sqlMetaDatas[ordinal].Adjust(value);
          this.values[ordinal] = (object) new SqlDecimal(value);
          this.boundFields[ordinal] = false;
          this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) sqlDbType));
      }
    }

    public void SetDateTime(int ordinal, DateTime value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.DateTime);
      this.SetClrTypeValue(ordinal, (object) value, nameof (SetDateTime));
    }

    public void SetSqlBoolean(int ordinal, SqlBoolean value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Bit);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlBoolean));
    }

    public void SetSqlBinary(int ordinal, SqlBinary value)
    {
      SqlDbType sqlDbType = this.sqlMetaDatas[ordinal].SqlDbType;
      switch (sqlDbType)
      {
        case SqlDbType.Binary:
        case SqlDbType.Image:
        case SqlDbType.VarBinary:
          this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlBinary));
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) sqlDbType.ToString()));
      }
    }

    public void SetSqlByte(int ordinal, SqlByte value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.TinyInt);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlByte));
    }

    public void SetSqlInt16(int ordinal, SqlInt16 value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.SmallInt);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlInt16));
    }

    public void SetSqlInt32(int ordinal, SqlInt32 value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Int);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlInt32));
    }

    public void SetSqlInt64(int ordinal, SqlInt64 value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.BigInt);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlInt64));
    }

    public void SetSqlSingle(int ordinal, SqlSingle value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Real);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlSingle));
    }

    public void SetSqlDouble(int ordinal, SqlDouble value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Float);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlDouble));
    }

    public void SetSqlString(int ordinal, SqlString value)
    {
      SqlDbType sqlDbType = this.sqlMetaDatas[ordinal].SqlDbType;
      switch (sqlDbType)
      {
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.NVarChar:
          this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlString));
          break;
        default:
          throw new ArgumentException(Res.GetString("SQLCE_DataColumn_SetFailed", (object) value.GetType().ToString(), (object) this.fieldNames[ordinal], (object) sqlDbType));
      }
    }

    public void SetSqlMoney(int ordinal, SqlMoney value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.Money);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlMoney));
    }

    public void SetSqlDecimal(int ordinal, SqlDecimal value)
    {
      this.EnsureObjectArray(ordinal, nameof (SetSqlDecimal));
      this.ValidateType(ordinal, (object) value, SqlDbType.Decimal);
      if (value.IsNull)
      {
        this.values[ordinal] = (object) DBNull.Value;
      }
      else
      {
        value = this.sqlMetaDatas[ordinal].Adjust(value);
        int[] data = value.Data;
        this.values[ordinal] = (object) new SqlDecimal(value.Precision, value.Scale, value.IsPositive, data[0], data[1], data[2], data[3]);
      }
      this.boundFields[ordinal] = false;
      this.columnsUpdated[ordinal] = ColumnUpdatedStatus.NewValue;
    }

    public void SetSqlDateTime(int ordinal, SqlDateTime value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.DateTime);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlDateTime));
    }

    public void SetSqlGuid(int ordinal, SqlGuid value)
    {
      this.ValidateType(ordinal, (object) value, SqlDbType.UniqueIdentifier);
      this.SetSqlTypeValue(ordinal, (INullable) value, nameof (SetSqlGuid));
    }
  }
}
