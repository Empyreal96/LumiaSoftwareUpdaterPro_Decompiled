// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlMetaData
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.SqlTypes;
using System.Globalization;

namespace System.Data.SqlServerCe
{
  public sealed class SqlMetaData
  {
    private const long x_lMax = -1;
    private const long x_lServerMaxUnicode = 4000;
    private const long x_lServerMaxANSI = 8000;
    private const long x_lServerMaxBinary = 8000;
    private const SqlCompareOptions x_eDefaultStringCompareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
    private string m_strName;
    private long m_lMaxLength;
    private SqlDbType m_sqlDbType;
    private byte m_bPrecision;
    private byte m_bScale;
    private long m_lLocale;
    private SqlCompareOptions m_eCompareOptions;
    private bool m_bPartialLength;
    private static byte[] __maxLenFromPrecision = new byte[38]
    {
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 9,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 13,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17,
      (byte) 17
    };
    private static readonly DateTime x_dtSmallMax = new DateTime(2079, 6, 6, 23, 59, 29, 998);
    private static readonly DateTime x_dtSmallMin = new DateTime(1899, 12, 31, 23, 59, 29, 999);
    private static readonly SqlMoney x_smSmallMax = new SqlMoney(214748.3647M);
    private static readonly SqlMoney x_smSmallMin = new SqlMoney(-214748.3648M);
    private static DbType[] sxm_rgSqlDbTypeToDbType = new DbType[30]
    {
      DbType.Int64,
      DbType.Binary,
      DbType.Boolean,
      DbType.AnsiString,
      DbType.DateTime,
      DbType.Decimal,
      DbType.Double,
      DbType.Binary,
      DbType.Int32,
      DbType.Currency,
      DbType.String,
      DbType.String,
      DbType.String,
      DbType.Single,
      DbType.Guid,
      DbType.DateTime,
      DbType.Int16,
      DbType.Currency,
      DbType.AnsiString,
      DbType.Binary,
      DbType.Byte,
      DbType.Binary,
      DbType.AnsiString,
      DbType.Object,
      DbType.Object,
      DbType.Xml,
      DbType.String,
      DbType.String,
      DbType.String,
      DbType.Object
    };
    internal static SqlMetaData[] sxm_rgDefaults = new SqlMetaData[30]
    {
      new SqlMetaData("bigint", SqlDbType.BigInt, 8L, (byte) 19, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("binary", SqlDbType.Binary, 1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("bit", SqlDbType.Bit, 1L, (byte) 1, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("char", SqlDbType.Char, 1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("datetime", SqlDbType.DateTime, 8L, (byte) 23, (byte) 3, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("decimal", SqlDbType.Decimal, 9L, (byte) 18, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("float", SqlDbType.Float, 8L, (byte) 53, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("image", SqlDbType.Image, -1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("int", SqlDbType.Int, 4L, (byte) 10, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("money", SqlDbType.Money, 8L, (byte) 19, (byte) 4, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("nchar", SqlDbType.NChar, 1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("ntext", SqlDbType.NText, -1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("nvarchar", SqlDbType.NVarChar, 4000L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("real", SqlDbType.Real, 4L, (byte) 24, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("uniqueidentifier", SqlDbType.UniqueIdentifier, 16L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("smalldatetime", SqlDbType.SmallDateTime, 4L, (byte) 16, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("smallint", SqlDbType.SmallInt, 2L, (byte) 5, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("smallmoney", SqlDbType.SmallMoney, 4L, (byte) 10, (byte) 4, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("text", SqlDbType.Text, -1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("rowversion", SqlDbType.Timestamp, 8L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("tinyint", SqlDbType.TinyInt, 1L, (byte) 3, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("varbinary", SqlDbType.VarBinary, 8000L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.None, false),
      new SqlMetaData("varchar", SqlDbType.VarChar, 8000L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("sql_variant", SqlDbType.Variant, 8016L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.None, false),
      null,
      null,
      new SqlMetaData("nvarchar", SqlDbType.NVarChar, 1L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("nvarchar", SqlDbType.NVarChar, 4000L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      new SqlMetaData("nvarchar", SqlDbType.NVarChar, 4000L, (byte) 0, (byte) 0, 0L, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth, false),
      null
    };

    static SqlMetaData() => KillBitHelper.ThrowIfKillBitIsSet();

    public SqlMetaData(string name, SqlDbType dbType) => this.Construct(name, dbType);

    public SqlMetaData(string name, SqlDbType dbType, long maxLength) => this.Construct(name, dbType, maxLength);

    public SqlMetaData(string name, SqlDbType dbType, byte precision, byte scale) => this.Construct(name, dbType, precision, scale);

    public SqlMetaData(
      string name,
      SqlDbType dbType,
      long maxLength,
      long locale,
      SqlCompareOptions compareOptions)
    {
      this.Construct(name, dbType, maxLength, locale, compareOptions);
    }

    public SqlMetaData(
      string name,
      SqlDbType dbType,
      long maxLength,
      byte precision,
      byte scale,
      long localeId,
      SqlCompareOptions compareOptions,
      Type userDefinedType)
    {
      switch (dbType)
      {
        case SqlDbType.BigInt:
        case SqlDbType.Bit:
        case SqlDbType.DateTime:
        case SqlDbType.Float:
        case SqlDbType.Image:
        case SqlDbType.Int:
        case SqlDbType.Money:
        case SqlDbType.Real:
        case SqlDbType.UniqueIdentifier:
        case SqlDbType.SmallDateTime:
        case SqlDbType.SmallInt:
        case SqlDbType.SmallMoney:
        case SqlDbType.Timestamp:
        case SqlDbType.TinyInt:
        case SqlDbType.Xml:
          this.Construct(name, dbType);
          break;
        case SqlDbType.Binary:
        case SqlDbType.VarBinary:
          this.Construct(name, dbType, maxLength);
          break;
        case SqlDbType.Char:
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.NVarChar:
        case SqlDbType.Text:
        case SqlDbType.VarChar:
          this.Construct(name, dbType, maxLength, localeId, compareOptions);
          break;
        case SqlDbType.Decimal:
          this.Construct(name, dbType, precision, scale);
          break;
        case SqlDbType.Variant:
          this.Construct(name, dbType);
          break;
        default:
          throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) dbType.ToString()));
      }
    }

    private SqlMetaData(
      string name,
      SqlDbType sqlDbType,
      long maxLength,
      byte precision,
      byte scale,
      long localeId,
      SqlCompareOptions compareOptions,
      bool partialLength)
    {
      this.AssertNameIsValid(name);
      this.m_strName = name;
      this.m_sqlDbType = sqlDbType;
      this.m_lMaxLength = maxLength;
      this.m_bPrecision = precision;
      this.m_bScale = scale;
      this.m_lLocale = localeId;
      this.m_eCompareOptions = compareOptions;
      this.m_bPartialLength = partialLength;
    }

    public SqlCompareOptions CompareOptions => this.m_eCompareOptions;

    public DbType DbType => SqlMetaData.sxm_rgSqlDbTypeToDbType[(int) this.m_sqlDbType];

    public long LocaleId => this.m_lLocale;

    public static long Max => -1;

    public long MaxLength => this.m_lMaxLength;

    public string Name => this.m_strName;

    public byte Precision => this.m_bPrecision;

    public byte Scale => this.m_bScale;

    public SqlDbType SqlDbType => this.m_sqlDbType;

    public string TypeName => SqlMetaData.sxm_rgDefaults[(int) this.SqlDbType].Name;

    internal bool IsPartialLength => this.m_bPartialLength;

    private void Construct(string name, SqlDbType dbType)
    {
      this.AssertNameIsValid(name);
      if (SqlDbType.Binary == dbType || SqlDbType.Char == dbType || (SqlDbType.NChar == dbType || SqlDbType.NVarChar == dbType) || (SqlDbType.VarBinary == dbType || SqlDbType.VarChar == dbType || SqlDbType.BigInt > dbType))
        throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) dbType.ToString()));
      this.SetDefaultsForType(dbType);
      if (SqlDbType.NText == dbType || SqlDbType.Text == dbType)
        this.m_lLocale = (long) CultureInfo.CurrentCulture.LCID;
      this.m_strName = name;
    }

    private void Construct(string name, SqlDbType dbType, long maxLength)
    {
      this.AssertNameIsValid(name);
      long num = 0;
      if (SqlDbType.Char == dbType)
      {
        if (maxLength > 8000L || maxLength < 0L)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
        num = (long) CultureInfo.CurrentCulture.LCID;
      }
      else if (SqlDbType.VarChar == dbType)
      {
        if ((maxLength > 8000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
        num = (long) CultureInfo.CurrentCulture.LCID;
      }
      else if (SqlDbType.NChar == dbType)
      {
        if (maxLength > 4000L || maxLength < 0L)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
        num = (long) CultureInfo.CurrentCulture.LCID;
      }
      else if (SqlDbType.NVarChar == dbType)
      {
        if ((maxLength > 4000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
        num = (long) CultureInfo.CurrentCulture.LCID;
      }
      else if (SqlDbType.NText == dbType || SqlDbType.Text == dbType)
      {
        if (SqlMetaData.Max != maxLength)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
        num = (long) CultureInfo.CurrentCulture.LCID;
      }
      else if (SqlDbType.Binary == dbType)
      {
        if (maxLength > 8000L || maxLength < 0L)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else if (SqlDbType.VarBinary == dbType)
      {
        if ((maxLength > 8000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else if (SqlDbType.Image == dbType)
      {
        if (SqlMetaData.Max != maxLength)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else
        throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) dbType.ToString()));
      this.SetDefaultsForType(dbType);
      this.m_strName = name;
      this.m_lMaxLength = maxLength;
      this.m_lLocale = num;
    }

    private void Construct(
      string name,
      SqlDbType dbType,
      long maxLength,
      long locale,
      SqlCompareOptions compareOptions)
    {
      this.AssertNameIsValid(name);
      if (SqlDbType.Char == dbType)
      {
        if (maxLength > 8000L || maxLength < 0L)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else if (SqlDbType.VarChar == dbType)
      {
        if ((maxLength > 8000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else if (SqlDbType.NChar == dbType)
      {
        if (maxLength > 4000L || maxLength < 0L)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else if (SqlDbType.NVarChar == dbType)
      {
        if ((maxLength > 4000L || maxLength < 0L) && maxLength != SqlMetaData.Max)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else if (SqlDbType.NText == dbType || SqlDbType.Text == dbType)
      {
        if (SqlMetaData.Max != maxLength)
          throw new ArgumentException(Res.GetString("ADP_InvalidDataLength2", (object) maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (maxLength));
      }
      else
        throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) dbType.ToString()));
      if (SqlCompareOptions.BinarySort != compareOptions && (~(SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreNonSpace | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth) & compareOptions) != SqlCompareOptions.None)
        throw new ArgumentException(compareOptions.ToString());
      this.SetDefaultsForType(dbType);
      this.m_strName = name;
      this.m_lMaxLength = maxLength;
      this.m_lLocale = locale;
      this.m_eCompareOptions = compareOptions;
    }

    private void Construct(string name, SqlDbType dbType, byte precision, byte scale)
    {
      this.AssertNameIsValid(name);
      if (SqlDbType.Decimal != dbType)
        throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) dbType.ToString()));
      if ((int) precision > (int) SqlDecimal.MaxPrecision || (int) scale > (int) precision)
        throw new ArgumentException(Res.GetString("SQL_PrecisionValueOutOfRange", (object) precision));
      if ((int) scale > (int) SqlDecimal.MaxScale)
        throw new ArgumentException(Res.GetString("SQL_ScaleValueOutOfRange", (object) scale));
      this.SetDefaultsForType(SqlDbType.Decimal);
      this.m_strName = name;
      this.m_bPrecision = precision;
      this.m_bScale = scale;
      this.m_lMaxLength = (long) SqlMetaData.__maxLenFromPrecision[(int) precision - 1];
    }

    private void AssertNameIsValid(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
    }

    public short Adjust(short value)
    {
      if (SqlDbType.SmallInt != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public int Adjust(int value)
    {
      if (SqlDbType.Int != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public long Adjust(long value)
    {
      if (this.SqlDbType != SqlDbType.BigInt)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public float Adjust(float value)
    {
      if (SqlDbType.Real != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public double Adjust(double value)
    {
      if (SqlDbType.Float != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public string Adjust(string value)
    {
      if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
      {
        if (value != null && (long) value.Length < this.MaxLength)
          value = value.PadRight((int) this.MaxLength);
      }
      else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && (SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType))
        SqlMetaData.ThrowInvalidType();
      if (value == null)
        return (string) null;
      if ((long) value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
        value = value.Remove((int) this.MaxLength, (int) ((long) value.Length - this.MaxLength));
      return value;
    }

    public Decimal Adjust(Decimal value)
    {
      if (SqlDbType.Decimal != this.SqlDbType && SqlDbType.Money != this.SqlDbType && SqlDbType.SmallMoney != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      if (SqlDbType.Decimal == this.SqlDbType)
        return this.InternalAdjustSqlDecimal(new SqlDecimal(value)).Value;
      this.VerifyMoneyRange(new SqlMoney(value));
      return value;
    }

    public DateTime Adjust(DateTime value)
    {
      if (SqlDbType.DateTime != this.SqlDbType && SqlDbType.SmallDateTime != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      this.VerifyDateTimeRange(value);
      return value;
    }

    public Guid Adjust(Guid value)
    {
      if (SqlDbType.UniqueIdentifier != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlBoolean Adjust(SqlBoolean value)
    {
      if (SqlDbType.Bit != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlByte Adjust(SqlByte value)
    {
      if (SqlDbType.TinyInt != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlInt16 Adjust(SqlInt16 value)
    {
      if (SqlDbType.SmallInt != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlInt32 Adjust(SqlInt32 value)
    {
      if (SqlDbType.Int != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlInt64 Adjust(SqlInt64 value)
    {
      if (this.SqlDbType != SqlDbType.BigInt)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlSingle Adjust(SqlSingle value)
    {
      if (SqlDbType.Real != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlDouble Adjust(SqlDouble value)
    {
      if (SqlDbType.Float != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlMoney Adjust(SqlMoney value)
    {
      if (SqlDbType.Money != this.SqlDbType && SqlDbType.SmallMoney != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      if (!value.IsNull)
        this.VerifyMoneyRange(value);
      return value;
    }

    public SqlDateTime Adjust(SqlDateTime value)
    {
      if (SqlDbType.DateTime != this.SqlDbType && SqlDbType.SmallDateTime != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      if (!value.IsNull)
        this.VerifyDateTimeRange(value.Value);
      return value;
    }

    public SqlDecimal Adjust(SqlDecimal value)
    {
      if (SqlDbType.Decimal != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return this.InternalAdjustSqlDecimal(value);
    }

    public SqlString Adjust(SqlString value)
    {
      if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
      {
        if (!value.IsNull && (long) value.Value.Length < this.MaxLength)
          return new SqlString(value.Value.PadRight((int) this.MaxLength));
      }
      else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && (SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType))
        SqlMetaData.ThrowInvalidType();
      if (value.IsNull || (long) value.Value.Length <= this.MaxLength || SqlMetaData.Max == this.MaxLength)
        return value;
      value = new SqlString(value.Value.Remove((int) this.MaxLength, (int) ((long) value.Value.Length - this.MaxLength)));
      return value;
    }

    public SqlBinary Adjust(SqlBinary value)
    {
      if (SqlDbType.Binary == this.SqlDbType || SqlDbType.Timestamp == this.SqlDbType)
      {
        if (!value.IsNull && (long) value.Length < this.MaxLength)
        {
          byte[] numArray1 = value.Value;
          byte[] numArray2 = new byte[this.MaxLength];
          Array.Copy((Array) numArray1, (Array) numArray2, numArray1.Length);
          Array.Clear((Array) numArray2, numArray1.Length, numArray2.Length - numArray1.Length);
          return new SqlBinary(numArray2);
        }
      }
      else if (SqlDbType.VarBinary != this.SqlDbType && SqlDbType.Image != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      if (value.IsNull || (long) value.Length <= this.MaxLength || SqlMetaData.Max == this.MaxLength)
        return value;
      byte[] numArray3 = value.Value;
      byte[] numArray4 = new byte[this.MaxLength];
      Array.Copy((Array) numArray3, (Array) numArray4, (int) this.MaxLength);
      value = new SqlBinary(numArray4);
      return value;
    }

    public SqlGuid Adjust(SqlGuid value)
    {
      if (SqlDbType.UniqueIdentifier != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public SqlChars Adjust(SqlChars value)
    {
      if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
      {
        if (value != null && !value.IsNull)
        {
          long length = value.Length;
          if (length < this.MaxLength)
          {
            if (value.MaxLength < this.MaxLength)
            {
              char[] buffer = new char[(int) this.MaxLength];
              Array.Copy((Array) value.Buffer, (Array) buffer, (int) length);
              value = new SqlChars(buffer);
            }
            char[] buffer1 = value.Buffer;
            for (long index = length; index < this.MaxLength; ++index)
              buffer1[index] = ' ';
            value.SetLength(this.MaxLength);
            return value;
          }
        }
      }
      else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && (SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType))
        SqlMetaData.ThrowInvalidType();
      if (value == null || value.IsNull || (value.Length <= this.MaxLength || SqlMetaData.Max == this.MaxLength))
        return value;
      value.SetLength(this.MaxLength);
      return value;
    }

    public SqlBytes Adjust(SqlBytes value)
    {
      if (SqlDbType.Binary == this.SqlDbType || SqlDbType.Timestamp == this.SqlDbType)
      {
        if (value != null && !value.IsNull)
        {
          int length = (int) value.Length;
          if ((long) length < this.MaxLength)
          {
            if (value.MaxLength < this.MaxLength)
            {
              byte[] buffer = new byte[this.MaxLength];
              Array.Copy((Array) value.Buffer, (Array) buffer, length);
              value = new SqlBytes(buffer);
            }
            byte[] buffer1 = value.Buffer;
            Array.Clear((Array) buffer1, length, buffer1.Length - length);
            value.SetLength(this.MaxLength);
            return value;
          }
        }
      }
      else if (SqlDbType.VarBinary != this.SqlDbType && SqlDbType.Image != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      if (value == null || value.IsNull || (value.Length <= this.MaxLength || SqlMetaData.Max == this.MaxLength))
        return value;
      value.SetLength(this.MaxLength);
      return value;
    }

    public object Adjust(object value)
    {
      if (value == null)
        return (object) null;
      Type type = value.GetType();
      switch (Type.GetTypeCode(type))
      {
        case TypeCode.Empty:
          throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) TypeCode.Empty));
        case TypeCode.Object:
          if (type == typeof (byte[]))
          {
            value = (object) this.Adjust((byte[]) value);
            goto case TypeCode.DBNull;
          }
          else if (type == typeof (char[]))
          {
            value = (object) this.Adjust((char[]) value);
            goto case TypeCode.DBNull;
          }
          else if (type == typeof (Guid))
          {
            value = (object) this.Adjust((Guid) value);
            goto case TypeCode.DBNull;
          }
          else
          {
            if (type == typeof (object))
              throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) TypeCode.UInt64));
            if (type == typeof (SqlBinary))
            {
              value = (object) this.Adjust((SqlBinary) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlBoolean))
            {
              value = (object) this.Adjust((SqlBoolean) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlByte))
            {
              value = (object) this.Adjust((SqlByte) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlDateTime))
            {
              value = (object) this.Adjust((SqlDateTime) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlDouble))
            {
              value = (object) this.Adjust((SqlDouble) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlGuid))
            {
              value = (object) this.Adjust((SqlGuid) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlInt16))
            {
              value = (object) this.Adjust((SqlInt16) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlInt32))
            {
              value = (object) this.Adjust((SqlInt32) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlInt64))
            {
              value = (object) this.Adjust((SqlInt64) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlMoney))
            {
              value = (object) this.Adjust((SqlMoney) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlDecimal))
            {
              value = (object) this.Adjust((SqlDecimal) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlSingle))
            {
              value = (object) this.Adjust((SqlSingle) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlString))
            {
              value = (object) this.Adjust((SqlString) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlChars))
            {
              value = (object) this.Adjust((SqlChars) value);
              goto case TypeCode.DBNull;
            }
            else if (type == typeof (SqlBytes))
            {
              value = (object) this.Adjust((SqlBytes) value);
              goto case TypeCode.DBNull;
            }
            else
              throw new Exception(Res.GetString("ADP_InvalidDataType", (object) type));
          }
        case TypeCode.DBNull:
          return value;
        case TypeCode.Boolean:
          value = (object) this.Adjust((bool) value);
          goto case TypeCode.DBNull;
        case TypeCode.Char:
          value = (object) this.Adjust((char) value);
          goto case TypeCode.DBNull;
        case TypeCode.SByte:
          throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) TypeCode.SByte));
        case TypeCode.Byte:
          value = (object) this.Adjust((byte) value);
          goto case TypeCode.DBNull;
        case TypeCode.Int16:
          value = (object) this.Adjust((short) value);
          goto case TypeCode.DBNull;
        case TypeCode.UInt16:
          throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) TypeCode.UInt16));
        case TypeCode.Int32:
          value = (object) this.Adjust((int) value);
          goto case TypeCode.DBNull;
        case TypeCode.UInt32:
          throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) TypeCode.UInt32));
        case TypeCode.Int64:
          value = (object) this.Adjust((long) value);
          goto case TypeCode.DBNull;
        case TypeCode.UInt64:
          throw new ArgumentException(Res.GetString("ADP_InvalidDataType", (object) TypeCode.UInt64));
        case TypeCode.Single:
          value = (object) this.Adjust((float) value);
          goto case TypeCode.DBNull;
        case TypeCode.Double:
          value = (object) this.Adjust((double) value);
          goto case TypeCode.DBNull;
        case TypeCode.Decimal:
          value = (object) this.Adjust((Decimal) value);
          goto case TypeCode.DBNull;
        case TypeCode.DateTime:
          value = (object) this.Adjust((DateTime) value);
          goto case TypeCode.DBNull;
        case TypeCode.String:
          value = (object) this.Adjust((string) value);
          goto case TypeCode.DBNull;
        default:
          throw new Exception(Res.GetString("ADP_InvalidDataType", (object) type));
      }
    }

    public bool Adjust(bool value)
    {
      if (SqlDbType.Bit != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public byte Adjust(byte value)
    {
      if (SqlDbType.TinyInt != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public byte[] Adjust(byte[] value)
    {
      if (SqlDbType.Binary == this.SqlDbType || SqlDbType.Timestamp == this.SqlDbType)
      {
        if (value != null && (long) value.Length < this.MaxLength)
        {
          byte[] numArray = new byte[this.MaxLength];
          Array.Copy((Array) value, (Array) numArray, value.Length);
          Array.Clear((Array) numArray, value.Length, numArray.Length - value.Length);
          return numArray;
        }
      }
      else if (SqlDbType.VarBinary != this.SqlDbType && SqlDbType.Image != this.SqlDbType)
        SqlMetaData.ThrowInvalidType();
      if (value == null)
        return (byte[]) null;
      if ((long) value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
      {
        byte[] numArray = new byte[this.MaxLength];
        Array.Copy((Array) value, (Array) numArray, (int) this.MaxLength);
        value = numArray;
      }
      return value;
    }

    public char Adjust(char value)
    {
      if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
      {
        if (1L != this.MaxLength)
          SqlMetaData.ThrowInvalidType();
      }
      else if (1L > this.MaxLength || SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && (SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType))
        SqlMetaData.ThrowInvalidType();
      return value;
    }

    public char[] Adjust(char[] value)
    {
      if (SqlDbType.Char == this.SqlDbType || SqlDbType.NChar == this.SqlDbType)
      {
        if (value != null)
        {
          long length = (long) value.Length;
          if (length < this.MaxLength)
          {
            char[] chArray = new char[(int) this.MaxLength];
            Array.Copy((Array) value, (Array) chArray, (int) length);
            for (long index = length; index < (long) chArray.Length; ++index)
              chArray[index] = ' ';
            return chArray;
          }
        }
      }
      else if (SqlDbType.VarChar != this.SqlDbType && SqlDbType.NVarChar != this.SqlDbType && (SqlDbType.Text != this.SqlDbType && SqlDbType.NText != this.SqlDbType))
        SqlMetaData.ThrowInvalidType();
      if (value == null)
        return (char[]) null;
      if ((long) value.Length > this.MaxLength && SqlMetaData.Max != this.MaxLength)
      {
        char[] chArray = new char[this.MaxLength];
        Array.Copy((Array) value, (Array) chArray, (int) this.MaxLength);
        value = chArray;
      }
      return value;
    }

    private static void ThrowInvalidType() => throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "invalid type"));

    private void VerifyDateTimeRange(DateTime value)
    {
      if (SqlDbType.SmallDateTime != this.SqlDbType || !(SqlMetaData.x_dtSmallMax < value) && !(SqlMetaData.x_dtSmallMin > value))
        return;
      SqlMetaData.ThrowInvalidType();
    }

    private void VerifyMoneyRange(SqlMoney value)
    {
      if (SqlDbType.SmallMoney != this.SqlDbType || !(SqlMetaData.x_smSmallMax < value).Value && !(SqlMetaData.x_smSmallMin > value).Value)
        return;
      SqlMetaData.ThrowInvalidType();
    }

    private SqlDecimal InternalAdjustSqlDecimal(SqlDecimal value) => !value.IsNull && ((int) value.Precision != (int) this.Precision || (int) value.Scale != (int) this.Scale) ? SqlDecimal.ConvertToPrecScale(value, (int) this.Precision, (int) this.Scale) : value;

    private void SetDefaultsForType(SqlDbType dbType)
    {
      if (SqlDbType.BigInt > dbType || SqlDbType.Udt < dbType)
        return;
      SqlMetaData sxmRgDefault = SqlMetaData.sxm_rgDefaults[(int) dbType];
      this.m_sqlDbType = dbType;
      this.m_lMaxLength = sxmRgDefault.MaxLength;
      this.m_bPrecision = sxmRgDefault.Precision;
      this.m_bScale = sxmRgDefault.Scale;
      this.m_lLocale = sxmRgDefault.LocaleId;
      this.m_eCompareOptions = sxmRgDefault.CompareOptions;
    }
  }
}
