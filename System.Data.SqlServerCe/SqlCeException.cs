// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeException
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Data.Common;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Data.SqlServerCe
{
  [Serializable]
  public class SqlCeException : DbException
  {
    private SqlCeErrorCollection _errors;
    private string _customMessage;

    static SqlCeException() => KillBitHelper.ThrowIfKillBitIsSet();

    private SqlCeException()
      : this(string.Empty)
    {
    }

    private SqlCeException(string msg)
      : base(msg)
    {
      this.Errors = new SqlCeErrorCollection();
    }

    private SqlCeException(string msg, Exception inner)
      : base(msg, inner)
    {
      this.Errors = new SqlCeErrorCollection();
    }

    internal SqlCeException(SqlCeErrorCollection errors)
      : this(string.Empty)
    {
      this.Errors = errors;
    }

    protected SqlCeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Errors = info != null ? (SqlCeErrorCollection) info.GetValue("__Errors__", typeof (SqlCeErrorCollection)) : throw new ArgumentNullException(nameof (info));
    }

    [SecurityCritical]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      base.GetObjectData(info, context);
      info.AddValue("__Errors__", (object) this.Errors);
    }

    public SqlCeErrorCollection Errors
    {
      get
      {
        if (this._errors == null)
          this._errors = new SqlCeErrorCollection();
        return this._errors;
      }
      private set
      {
        this._errors = value;
        this._customMessage = this.BuildExceptionMessage();
      }
    }

    public new int HResult => this.Errors.Count > 0 ? this.Errors[0].HResult : -1;

    public int NativeError => this.Errors.Count > 0 ? this.Errors[0].NativeError : -1;

    public override string Message => !string.IsNullOrEmpty(this._customMessage) ? this._customMessage : base.Message;

    public override string Source => this.Errors.Count > 0 ? this.Errors[0].Source : string.Empty;

    [SecurityCritical]
    internal static SqlCeException FillErrorInformation(int hr, IntPtr pIError)
    {
      SqlCeErrorCollection errors = new SqlCeErrorCollection();
      errors.FillErrorInformation(hr, pIError);
      return SqlCeException.CreateException(errors);
    }

    [SecurityCritical]
    internal static SqlCeException FillErrorCollection(int hr, IntPtr pISSCEErrors)
    {
      SqlCeErrorCollection errors = new SqlCeErrorCollection();
      errors.FillErrorCollection(hr, pISSCEErrors);
      return SqlCeException.CreateException(errors);
    }

    internal static SqlCeException CreateException(SqlCeErrorCollection errors)
    {
      switch (errors[0].NativeError)
      {
        case 25090:
          return (SqlCeException) new SqlCeLockTimeoutException(errors);
        case 25126:
          return (SqlCeException) new SqlCeTransactionInProgressException(errors);
        case 25138:
          return (SqlCeException) new SqlCeInvalidDatabaseFormatException(errors);
        default:
          return new SqlCeException(errors);
      }
    }

    internal static SqlCeException CreateException(string message) => new SqlCeException(message);

    internal static SqlCeException CreateException(string message, Exception inner) => new SqlCeException(message, inner);

    private string BuildExceptionMessage()
    {
      if (this.Errors.Count <= 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.Errors.Count; ++index)
      {
        if (index > 0)
          stringBuilder.Append("; ");
        stringBuilder.Append(this.Errors[index].Message);
      }
      return stringBuilder.ToString();
    }
  }
}
