// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.FieldNameLookup
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Globalization;

namespace System.Data.SqlServerCe
{
  internal sealed class FieldNameLookup
  {
    private Hashtable _fieldNameLookup;
    private string[] _fieldNames;
    private CompareInfo _compareInfo;
    private int _defaultLocaleID;

    static FieldNameLookup() => KillBitHelper.ThrowIfKillBitIsSet();

    public FieldNameLookup(string[] fieldNames, int defaultLocaleID)
    {
      this._fieldNames = fieldNames != null ? fieldNames : throw new ArgumentNullException(nameof (fieldNames));
      this._defaultLocaleID = defaultLocaleID;
    }

    public FieldNameLookup(IDataReader reader, int defaultLocaleID)
    {
      int fieldCount = reader.FieldCount;
      string[] strArray = new string[fieldCount];
      for (int i = 0; i < fieldCount; ++i)
        strArray[i] = reader.GetName(i);
      this._fieldNames = strArray;
      this._defaultLocaleID = defaultLocaleID;
    }

    public int GetOrdinal(string fieldName)
    {
      int num = fieldName != null ? this.IndexOf(fieldName) : throw new ArgumentNullException(nameof (fieldName));
      return -1 != num ? num : throw new IndexOutOfRangeException(fieldName);
    }

    public int IndexOfName(string fieldName)
    {
      if (this._fieldNameLookup == null)
        this.GenerateLookup();
      object obj = this._fieldNameLookup[(object) fieldName];
      return obj == null ? -1 : (int) obj;
    }

    public int IndexOf(string fieldName)
    {
      if (this._fieldNameLookup == null)
        this.GenerateLookup();
      object obj = this._fieldNameLookup[(object) fieldName];
      int num;
      if (obj != null)
      {
        num = (int) obj;
      }
      else
      {
        num = this.LinearIndexOf(fieldName, CompareOptions.IgnoreCase);
        if (-1 == num)
          num = this.LinearIndexOf(fieldName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
      }
      return num;
    }

    private int LinearIndexOf(string fieldName, CompareOptions compareOptions)
    {
      CompareInfo compareInfo = this._compareInfo;
      if (compareInfo == null)
      {
        if (-1 != this._defaultLocaleID)
          compareInfo = CompareInfo.GetCompareInfo(this._defaultLocaleID);
        if (compareInfo == null)
          compareInfo = CultureInfo.InvariantCulture.CompareInfo;
        this._compareInfo = compareInfo;
      }
      int length = this._fieldNames.Length;
      for (int index = 0; index < length; ++index)
      {
        if (compareInfo.Compare(fieldName, this._fieldNames[index], compareOptions) == 0)
        {
          this._fieldNameLookup[(object) fieldName] = (object) index;
          return index;
        }
      }
      return -1;
    }

    private void GenerateLookup()
    {
      int length = this._fieldNames.Length;
      Hashtable hashtable = new Hashtable(length);
      for (int index = length - 1; 0 <= index; --index)
      {
        string fieldName = this._fieldNames[index];
        hashtable[(object) fieldName] = (object) index;
      }
      this._fieldNameLookup = hashtable;
    }
  }
}
