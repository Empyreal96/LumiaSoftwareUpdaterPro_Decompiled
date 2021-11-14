// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeConnectionStringBuilder
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Security;

namespace System.Data.SqlServerCe
{
  [DefaultProperty("DataSource")]
  [TypeConverter(typeof (SqlCeConnectionStringBuilder.SqlCeConnectionStringBuilderConverter))]
  public sealed class SqlCeConnectionStringBuilder : DbConnectionStringBuilder
  {
    private static readonly string[] _validKeywords = KeywordMapper.ValidKeywords;
    private static readonly Dictionary<string, Keywords> _keywords = KeywordMapper.KeywordsDictionary;
    private int _autoshrinkThreshold = 60;
    private bool _caseSensitive;
    private string _dataSource = "";
    private int _defaultLockEscalation = 100;
    private int _defaultLockTimeout = 5000;
    private bool _encrypt;
    private string _encryptionMode;
    private bool _enlist = true;
    private int _fileAccessRetryTimeout;
    private string _fileMode = "Read Write";
    private int _flushInterval = 10;
    private int _initialLcid = -1;
    private int _maxBufferSize = 4096;
    private int _maxDatabaseSize = 256;
    private string _password = "";
    private bool _persistSecurityInfo;
    private int _tempFileMaxSize = 256;
    private string _tempFilePath;

    public SqlCeConnectionStringBuilder()
      : this((string) null)
    {
    }

    public SqlCeConnectionStringBuilder(string connectionString)
    {
      if (!ADP.IsEmpty(connectionString))
        this.ConnectionString = connectionString;
      this.BrowsableConnectionString = false;
    }

    public override void Clear()
    {
      base.Clear();
      for (int index = 0; index < SqlCeConnectionStringBuilder._validKeywords.Length; ++index)
        this.Reset((Keywords) index);
    }

    public override bool ContainsKey(string keyword)
    {
      ADP.CheckArgumentNull((object) keyword, nameof (keyword));
      return SqlCeConnectionStringBuilder._keywords.ContainsKey(keyword);
    }

    public override bool ShouldSerialize(string keyword)
    {
      ADP.CheckArgumentNull((object) keyword, nameof (keyword));
      Keywords keywords;
      return SqlCeConnectionStringBuilder._keywords.TryGetValue(keyword, out keywords) && base.ShouldSerialize(SqlCeConnectionStringBuilder._validKeywords[(int) keywords]);
    }

    public override bool TryGetValue(string keyword, out object value)
    {
      Keywords index;
      if (SqlCeConnectionStringBuilder._keywords.TryGetValue(keyword, out index))
      {
        value = this.GetAt(index);
        return true;
      }
      value = (object) null;
      return false;
    }

    public override bool Remove(string keyword)
    {
      ADP.CheckArgumentNull((object) keyword, nameof (keyword));
      Keywords index;
      if (!SqlCeConnectionStringBuilder._keywords.TryGetValue(keyword, out index) || !base.Remove(SqlCeConnectionStringBuilder._validKeywords[(int) index]))
        return false;
      this.Reset(index);
      return true;
    }

    public override object this[string keyword]
    {
      get => this.GetAt(SqlCeConnectionStringBuilder.GetIndex(keyword));
      set
      {
        if (value != null)
        {
          switch (SqlCeConnectionStringBuilder.GetIndex(keyword))
          {
            case Keywords.AutoshrinkThreshold:
              this.AutoshrinkThreshold = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.CaseSensitive:
              this.CaseSensitive = SqlCeConnectionStringBuilder.ConvertToBoolean(value);
              break;
            case Keywords.DataSource:
              this.DataSource = SqlCeConnectionStringBuilder.ConvertToString(value);
              break;
            case Keywords.DefaultLockEscalation:
              this.DefaultLockEscalation = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.DefaultLockTimeout:
              this.DefaultLockTimeout = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.Encrypt:
              this.Encrypt = SqlCeConnectionStringBuilder.ConvertToBoolean(value);
              break;
            case Keywords.EncryptionMode:
              this.EncryptionMode = SqlCeConnectionStringBuilder.ConvertToString(value);
              break;
            case Keywords.Enlist:
              this.Enlist = SqlCeConnectionStringBuilder.ConvertToBoolean(value);
              break;
            case Keywords.FileAccessRetryTimeout:
              this.FileAccessRetryTimeout = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.FileMode:
              this.FileMode = SqlCeConnectionStringBuilder.ConvertToString(value);
              break;
            case Keywords.FlushInterval:
              this.FlushInterval = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.InitialLcid:
              this.InitialLcid = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.MaxBufferSize:
              this.MaxBufferSize = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.MaxDatabaseSize:
              this.MaxDatabaseSize = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.Password:
              this.Password = SqlCeConnectionStringBuilder.ConvertToString(value);
              break;
            case Keywords.PersistSecurityInfo:
              this.PersistSecurityInfo = SqlCeConnectionStringBuilder.ConvertToBoolean(value);
              break;
            case Keywords.TempFileMaxSize:
              this.TempFileMaxSize = SqlCeConnectionStringBuilder.ConvertToInt32(value);
              break;
            case Keywords.TempFilePath:
              this.TempFilePath = SqlCeConnectionStringBuilder.ConvertToString(value);
              break;
            default:
              throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "ADP_KeywordNotSupported", (object) keyword));
          }
        }
        else
          this.Remove(keyword);
      }
    }

    [ResCategory("DataCategory_Advanced")]
    [DisplayName("Autoshrink Threshold")]
    [ResDescription("SqlCeConnectionString_AutoShrinkThreshold")]
    [RefreshProperties(RefreshProperties.All)]
    public int AutoshrinkThreshold
    {
      get => this._autoshrinkThreshold;
      set
      {
        if (0 > value || 100 < value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Autoshrink Threshold", (object) 0, (object) 100));
        this.SetValue("Autoshrink Threshold", value);
        this._autoshrinkThreshold = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [DisplayName("Case Sensitive")]
    [ResCategory("DataCategory_Initialization")]
    [Browsable(false)]
    [ResDescription("SqlCeConnectionString_CaseSensitive")]
    public bool CaseSensitive
    {
      get => this._caseSensitive;
      set
      {
        this.SetValue("Case Sensitive", value);
        this._caseSensitive = value;
      }
    }

    [DisplayName("Data Source")]
    [ResDescription("SqlCeConnectionString_DataSource")]
    [RefreshProperties(RefreshProperties.All)]
    [ResCategory("DataCategory_Source")]
    public string DataSource
    {
      get => this._dataSource;
      set
      {
        this.SetValue("Data Source", value);
        this._dataSource = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [DisplayName("Default Lock Escalation")]
    [ResCategory("DataCategory_Advanced")]
    [ResDescription("SqlCeConnectionString_DefaultLockEscalation")]
    public int DefaultLockEscalation
    {
      get => this._defaultLockEscalation;
      set
      {
        if (50 > value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Default Lock Escalation", (object) 50, (object) int.MaxValue));
        this.SetValue("Default Lock Escalation", value);
        this._defaultLockEscalation = value;
      }
    }

    [DisplayName("Default Lock Timeout")]
    [ResDescription("SqlCeConnectionString_DefaultLockTimeout")]
    [RefreshProperties(RefreshProperties.All)]
    [ResCategory("DataCategory_Advanced")]
    public int DefaultLockTimeout
    {
      get => this._defaultLockTimeout;
      set
      {
        if (0 > value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Default Lock Timeout", (object) 0, (object) int.MaxValue));
        this.SetValue("Default Lock Timeout", value);
        this._defaultLockTimeout = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [Browsable(false)]
    [DisplayName("Encrypt Database")]
    [ResCategory("DataCategory_Security")]
    [ResDescription("SqlCeConnectionString_Encrypt")]
    public bool Encrypt
    {
      get => this._encrypt;
      set
      {
        this.SetValue("Encrypt Database", value);
        this._encrypt = value;
      }
    }

    [ResCategory("DataCategory_Security")]
    [ResDescription("SqlCeConnectionString_EncryptionMode")]
    [RefreshProperties(RefreshProperties.All)]
    [TypeConverter(typeof (SqlCeConnectionStringBuilder.EncryptionModeConverter))]
    [Browsable(false)]
    [DisplayName("Encryption Mode")]
    public string EncryptionMode
    {
      get => this._encryptionMode;
      set
      {
        if (value.Equals("Engine Default", StringComparison.OrdinalIgnoreCase))
          value = "Engine Default";
        else
          value = value.Equals("Platform Default", StringComparison.OrdinalIgnoreCase) ? "Platform Default" : throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQL_InvalidConnectionOptionValue", (object) "Encryption Mode", (object) value));
        this.SetValue("Encryption Mode", value);
        this._encryptionMode = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [Browsable(false)]
    [ResCategory("DataCategory_Advanced")]
    [ResDescription("SqlCeConnectionString_Enlist")]
    [DisplayName("Enlist")]
    public bool Enlist
    {
      get => this._enlist;
      set
      {
        this.SetValue(nameof (Enlist), value);
        this._enlist = value;
      }
    }

    [DisplayName("File Access Retry Timeout")]
    [RefreshProperties(RefreshProperties.All)]
    [Browsable(false)]
    [ResCategory("DataCategory_Initialization")]
    [ResDescription("SqlCeConnectionString_FileAccessRetryTimeout")]
    public int FileAccessRetryTimeout
    {
      get => this._fileAccessRetryTimeout;
      set
      {
        if (0 > value || 30 < value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "File Access Retry Timeout", (object) 0, (object) 30));
        this.SetValue("File Access Retry Timeout", value);
        this._fileAccessRetryTimeout = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [Browsable(false)]
    [DisplayName("Mode")]
    [ResCategory("DataCategory_Source")]
    [ResDescription("SqlCeConnectionString_FileMode")]
    public string FileMode
    {
      get => this._fileMode;
      set
      {
        if (value.Equals("Read Only", StringComparison.OrdinalIgnoreCase))
          value = "Read Only";
        else if (value.Equals("Read Write", StringComparison.OrdinalIgnoreCase))
          value = "Read Write";
        else if (value.Equals("Shared Read", StringComparison.OrdinalIgnoreCase))
          value = "Shared Read";
        else
          value = value.Equals("Exclusive", StringComparison.OrdinalIgnoreCase) ? "Exclusive" : throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQL_InvalidConnectionOptionValue", (object) "Mode", (object) value));
        this.SetValue("Mode", value);
        this._fileMode = value;
      }
    }

    [ResCategory("DataCategory_Advanced")]
    [ResDescription("SqlCeConnectionString_FlushInterval")]
    [RefreshProperties(RefreshProperties.All)]
    [DisplayName("Flush Interval")]
    public int FlushInterval
    {
      get => this._flushInterval;
      set
      {
        if (1 > value || 1000 < value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Flush Interval", (object) 1, (object) 1000));
        this.SetValue("Flush Interval", value);
        this._flushInterval = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [Browsable(false)]
    [DisplayName("Locale Identifier")]
    [ResDescription("SqlCeConnectionString_Lcid")]
    [ResCategory("DataCategory_Initialization")]
    public int InitialLcid
    {
      get => this._initialLcid;
      set
      {
        if (value < 0)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Locale Identifier", (object) 0, (object) int.MaxValue));
        this.SetValue("Locale Identifier", value);
        this._initialLcid = value;
      }
    }

    [DisplayName("Max Buffer Size")]
    [ResCategory("DataCategory_Advanced")]
    [ResDescription("SqlCeConnectionString_MaxBufferSize")]
    [RefreshProperties(RefreshProperties.All)]
    public int MaxBufferSize
    {
      get => this._maxBufferSize;
      set
      {
        if (256 > value || 262140 < value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Max Buffer Size", (object) 256, (object) 262140));
        this.SetValue("Max Buffer Size", value);
        this._maxBufferSize = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [ResCategory("DataCategory_Source")]
    [ResDescription("SqlCeConnectionString_MaxDatabaseSize")]
    [DisplayName("Max Database Size")]
    public int MaxDatabaseSize
    {
      get => this._maxDatabaseSize;
      set
      {
        if (16 > value || 4091 < value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Max Database Size", (object) 16, (object) 4091));
        this.SetValue("Max Database Size", value);
        this._maxDatabaseSize = value;
      }
    }

    [ResCategory("DataCategory_Security")]
    [PasswordPropertyText(true)]
    [DisplayName("Password")]
    [ResDescription("SqlCeConnectionString_Password")]
    [RefreshProperties(RefreshProperties.All)]
    public string Password
    {
      get => this._password;
      set
      {
        this.SetValue(nameof (Password), value);
        this._password = value;
      }
    }

    [DisplayName("Persist Security Info")]
    [ResCategory("DataCategory_Security")]
    [ResDescription("SqlCeConnectionString_PersistSecurityInfo")]
    [RefreshProperties(RefreshProperties.All)]
    public bool PersistSecurityInfo
    {
      get => this._persistSecurityInfo;
      set
      {
        this.SetValue("Persist Security Info", value);
        this._persistSecurityInfo = value;
      }
    }

    [RefreshProperties(RefreshProperties.All)]
    [ResDescription("SqlCeConnectionString_TempFileMaxSize")]
    [DisplayName("Temp File Max Size")]
    [ResCategory("DataCategory_Advanced")]
    public int TempFileMaxSize
    {
      get => this._tempFileMaxSize;
      set
      {
        if (16 > value || 4091 < value)
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ArgumentOutOfRange", (object) "Temp File Max Size", (object) 16, (object) 4091));
        this.SetValue("Temp File Max Size", value);
        this._tempFileMaxSize = value;
      }
    }

    [DisplayName("Temp File Directory")]
    [ResDescription("SqlCeConnectionString_TempFilePath")]
    [RefreshProperties(RefreshProperties.All)]
    [ResCategory("DataCategory_Advanced")]
    public string TempFilePath
    {
      get => this._tempFilePath;
      set
      {
        this.SetValue("Temp File Directory", value);
        this._tempFilePath = value;
      }
    }

    public override ICollection Keys => (ICollection) new ReadOnlyCollection<string>((IList<string>) SqlCeConnectionStringBuilder._validKeywords);

    public override ICollection Values
    {
      get
      {
        object[] objArray = new object[SqlCeConnectionStringBuilder._validKeywords.Length];
        for (int index = 0; index < objArray.Length; ++index)
          objArray[index] = this.GetAt((Keywords) index);
        return (ICollection) new ReadOnlyCollection<object>((IList<object>) objArray);
      }
    }

    public override bool IsFixedSize => true;

    internal string OledbConnectionString => ConStringUtil.MapToOledbConnectionString(this.ConnectionString);

    private static bool ConvertToBoolean(object value)
    {
      if (value is bool flag)
        return flag;
      string x = (value as string).Trim();
      if (x != null)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(x, "true") || StringComparer.OrdinalIgnoreCase.Equals(x, "yes"))
          return true;
        return !StringComparer.OrdinalIgnoreCase.Equals(x, "false") && !StringComparer.OrdinalIgnoreCase.Equals(x, "no") && bool.Parse(x);
      }
      try
      {
        return ((IConvertible) value).ToBoolean((IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ConvertFailed", (object) value.GetType(), (object) typeof (bool)), (Exception) ex);
      }
    }

    private static int ConvertToInt32(object value)
    {
      try
      {
        return ((IConvertible) value).ToInt32((IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ConvertFailed", (object) value.GetType(), (object) typeof (int)), (Exception) ex);
      }
    }

    private static string ConvertToString(object value)
    {
      try
      {
        return ((IConvertible) value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_ConvertFailed", (object) value.GetType(), (object) typeof (string)), (Exception) ex);
      }
    }

    private object GetAt(Keywords index)
    {
      switch (index)
      {
        case Keywords.AutoshrinkThreshold:
          return (object) this.AutoshrinkThreshold;
        case Keywords.CaseSensitive:
          return (object) this.CaseSensitive;
        case Keywords.DataSource:
          return (object) this.DataSource;
        case Keywords.DefaultLockEscalation:
          return (object) this.DefaultLockEscalation;
        case Keywords.DefaultLockTimeout:
          return (object) this.DefaultLockTimeout;
        case Keywords.Encrypt:
          return (object) this.Encrypt;
        case Keywords.EncryptionMode:
          return (object) this.EncryptionMode;
        case Keywords.Enlist:
          return (object) this.Enlist;
        case Keywords.FileAccessRetryTimeout:
          return (object) this.FileAccessRetryTimeout;
        case Keywords.FileMode:
          return (object) this.FileMode;
        case Keywords.FlushInterval:
          return (object) this.FlushInterval;
        case Keywords.InitialLcid:
          return (object) this.InitialLcid;
        case Keywords.MaxBufferSize:
          return (object) this.MaxBufferSize;
        case Keywords.MaxDatabaseSize:
          return (object) this.MaxDatabaseSize;
        case Keywords.Password:
          return (object) this.Password;
        case Keywords.PersistSecurityInfo:
          return (object) this.PersistSecurityInfo;
        case Keywords.TempFileMaxSize:
          return (object) this.TempFileMaxSize;
        case Keywords.TempFilePath:
          return (object) this.TempFilePath;
        default:
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "ADP_KeywordNotSupported", (object) SqlCeConnectionStringBuilder._validKeywords[(int) index]));
      }
    }

    private static Keywords GetIndex(string keyword)
    {
      ADP.CheckArgumentNull((object) keyword, nameof (keyword));
      Keywords keywords;
      if (SqlCeConnectionStringBuilder._keywords.TryGetValue(keyword, out keywords))
        return keywords;
      throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "ADP_KeywordNotSupported", (object) keyword));
    }

    private void Reset(Keywords index)
    {
      switch (index)
      {
        case Keywords.AutoshrinkThreshold:
          this._autoshrinkThreshold = 60;
          break;
        case Keywords.CaseSensitive:
          this._caseSensitive = false;
          break;
        case Keywords.DataSource:
          this._dataSource = "";
          break;
        case Keywords.DefaultLockEscalation:
          this._defaultLockEscalation = 100;
          break;
        case Keywords.DefaultLockTimeout:
          this._defaultLockTimeout = 5000;
          break;
        case Keywords.Encrypt:
          this._encrypt = false;
          break;
        case Keywords.EncryptionMode:
          this._encryptionMode = (string) null;
          break;
        case Keywords.Enlist:
          this._enlist = true;
          break;
        case Keywords.FileAccessRetryTimeout:
          this._fileAccessRetryTimeout = 0;
          break;
        case Keywords.FileMode:
          this._fileMode = "Read Write";
          break;
        case Keywords.FlushInterval:
          this._flushInterval = 10;
          break;
        case Keywords.InitialLcid:
          this._initialLcid = -1;
          break;
        case Keywords.MaxBufferSize:
          this._maxBufferSize = 4096;
          break;
        case Keywords.MaxDatabaseSize:
          this._maxDatabaseSize = 256;
          break;
        case Keywords.Password:
          this._password = "";
          break;
        case Keywords.PersistSecurityInfo:
          this._persistSecurityInfo = false;
          break;
        case Keywords.TempFileMaxSize:
          this._tempFileMaxSize = 256;
          break;
        case Keywords.TempFilePath:
          this._tempFilePath = (string) null;
          break;
        default:
          throw new ArgumentException(Res.GetString(CultureInfo.CurrentCulture, "ADP_KeywordNotSupported", (object) SqlCeConnectionStringBuilder._validKeywords[(int) index]));
      }
    }

    private void SetValue(string keyword, bool value) => base[keyword] = (object) value.ToString((IFormatProvider) null);

    private void SetValue(string keyword, int value) => base[keyword] = (object) value.ToString((IFormatProvider) null);

    private void SetValue(string keyword, string value)
    {
      ADP.CheckArgumentNull((object) value, keyword);
      base[keyword] = (object) value;
    }

    internal sealed class SqlCeConnectionStringBuilderConverter : ExpandableObjectConverter
    {
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => typeof (InstanceDescriptor) == destinationType || base.CanConvertTo(context, destinationType);

      public override object ConvertTo(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value,
        Type destinationType)
      {
        if (destinationType == null)
          throw new ArgumentNullException(nameof (destinationType));
        return typeof (InstanceDescriptor) == destinationType && value is SqlCeConnectionStringBuilder options ? (object) SqlCeConnectionStringBuilder.SqlCeConnectionStringBuilderConverter.ConvertToInstanceDescriptor(options) : base.ConvertTo(context, culture, value, destinationType);
      }

      [SecurityTreatAsSafe]
      [SecurityCritical]
      private static InstanceDescriptor ConvertToInstanceDescriptor(
        SqlCeConnectionStringBuilder options)
      {
        Type[] types = new Type[1]{ typeof (string) };
        object[] objArray = new object[1]
        {
          (object) options.ConnectionString
        };
        return new InstanceDescriptor((MemberInfo) typeof (SqlCeConnectionStringBuilder).GetConstructor(types), (ICollection) objArray);
      }
    }

    private sealed class EncryptionModeConverter : TypeConverter
    {
      private TypeConverter.StandardValuesCollection _standardValues;

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

      public override TypeConverter.StandardValuesCollection GetStandardValues(
        ITypeDescriptorContext context)
      {
        if (this._standardValues == null)
          this._standardValues = new TypeConverter.StandardValuesCollection((ICollection) new List<string>()
          {
            "Engine Default",
            "Platform Default"
          });
        return this._standardValues;
      }
    }

    private sealed class FileModeConverter : TypeConverter
    {
      private TypeConverter.StandardValuesCollection _standardValues;

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

      public override TypeConverter.StandardValuesCollection GetStandardValues(
        ITypeDescriptorContext context)
      {
        if (this._standardValues == null)
          this._standardValues = new TypeConverter.StandardValuesCollection((ICollection) new List<string>()
          {
            "Read Write",
            "Read Only",
            "Exclusive",
            "Shared Read"
          });
        return this._standardValues;
      }
    }
  }
}
