// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ValueMapper
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Globalization;

namespace System.Data.SqlServerCe
{
  internal class ValueMapper
  {
    private static ValueMapper.ValueMapperDelegate _typeMapperInternal = Type.GetType("System.DateTimeOffset") == null ? new ValueMapper.ValueMapperDelegate(ValueMapper.GetMappedValueNET20RTM) : new ValueMapper.ValueMapperDelegate(ValueMapper.GetMappedValueNET20SP1);

    static ValueMapper() => KillBitHelper.ThrowIfKillBitIsSet();

    public static object GetMappedValue(SqlDbType paramType, object value) => ValueMapper._typeMapperInternal(paramType, value);

    private static object GetMappedValueNET20RTM(SqlDbType paramType, object value)
    {
      switch (value)
      {
        case null:
          return (object) null;
        case DateTime _:
          return ValueMapper.DateTimeMapper(paramType, value);
        case TimeSpan _:
          return ValueMapper.TimeSpanMapper(paramType, value);
        default:
          return value;
      }
    }

    private static object GetMappedValueNET20SP1(SqlDbType paramType, object value)
    {
      switch (value)
      {
        case null:
          return (object) null;
        case DateTime _:
          return ValueMapper.DateTimeMapper(paramType, value);
        case DateTimeOffset _:
          return ValueMapper.DateTimeOffsetMapper(paramType, value);
        case TimeSpan _:
          return ValueMapper.TimeSpanMapper(paramType, value);
        default:
          return value;
      }
    }

    private static object DateTimeMapper(SqlDbType paramType, object value) => paramType == SqlDbType.NChar || paramType == SqlDbType.NVarChar ? (object) ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss.fffffff", (IFormatProvider) CultureInfo.InvariantCulture) : value;

    private static object TimeSpanMapper(SqlDbType paramType, object value)
    {
      if (paramType != SqlDbType.NChar && paramType != SqlDbType.NVarChar)
        return value;
      TimeSpan timeSpan = (TimeSpan) value;
      return (object) DateTime.MinValue.Add(timeSpan).ToString("HH:mm:ss.fffffff");
    }

    private static object DateTimeOffsetMapper(SqlDbType paramType, object value) => paramType == SqlDbType.NChar || paramType == SqlDbType.NVarChar ? (object) ((DateTimeOffset) value).ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", (IFormatProvider) CultureInfo.InvariantCulture) : value;

    private delegate object ValueMapperDelegate(SqlDbType paramType, object value);
  }
}
