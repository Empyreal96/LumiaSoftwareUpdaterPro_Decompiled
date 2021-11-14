// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ConStringUtil
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Data.SqlServerCe
{
  internal sealed class ConStringUtil
  {
    public const string EngineDefault = "Engine Default";
    public const string PlatformDefault = "Platform Default";
    public const string ReadOnly = "Read Only";
    public const string ReadWrite = "Read Write";
    public const string Exclusive = "Exclusive";
    public const string SharedRead = "Shared Read";
    private const int _encryptionModeOptions = 2;
    private const char _equalSign = '=';
    private const char _semiColon = ';';
    private const char _singleQuote = '\'';
    private const char _doubleQuote = '"';
    private const char _space = ' ';
    private const char _tab = '\t';
    private const char _backslash = '\\';
    private static readonly Hashtable _encryptionModeMapping = new Hashtable(2)
    {
      [(object) "Platform Default"] = (object) 1,
      [(object) "Engine Default"] = (object) 2
    };
    private static readonly Hashtable _connectionSynonymMapping = KeywordMapper.KeywordSynonymsMapping;

    public static int MapEncryptionMode(string value) => value != null && ConStringUtil._encryptionModeMapping.ContainsKey((object) value) ? (int) ConStringUtil._encryptionModeMapping[(object) value] : 0;

    public static string RemoveKeyValuesFromString(string conString, string removeKey)
    {
      string conString1 = conString;
      int index = 0;
      int length = 0;
      ConStringUtil.SkipWhiteSpace(conString1, ref index);
      while (index < conString1.Length)
      {
        if (conString1[index] == ';')
        {
          ++index;
          length = index;
          ConStringUtil.SkipWhiteSpace(conString1, ref index);
        }
        else
        {
          int num = conString1.IndexOf('=', index);
          string lower = conString1.Substring(index, num - index).TrimEnd((char[]) null).ToLower(CultureInfo.InvariantCulture);
          index = num + 1;
          if (removeKey == (string) ConStringUtil._connectionSynonymMapping[(object) lower.ToLower(CultureInfo.InvariantCulture)])
          {
            string str = conString1.Substring(0, length);
            ConStringUtil.SkipValue(conString1, ref index);
            if (index < conString1.Length && conString1[index] == ';')
              ++index;
            conString1 = str + conString1.Substring(index);
            index = length;
            ConStringUtil.SkipWhiteSpace(conString1, ref index);
          }
          else
          {
            ConStringUtil.SkipValue(conString1, ref index);
            if (index < conString1.Length && conString1[index] == ';')
            {
              ++index;
              length = index;
              ConStringUtil.SkipWhiteSpace(conString1, ref index);
            }
          }
        }
      }
      return conString1;
    }

    public static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
      Dictionary<string, string> values = new Dictionary<string, string>();
      switch (connectionString)
      {
        case "":
        case null:
          return values;
        default:
          ConStringUtil.ParseStringIntoTable(connectionString, values);
          goto case "";
      }
    }

    public static string ReplaceDataDirectory(string inputString) => CommonUtils.ReplaceDataDirectory(inputString);

    public static string MapToOledbConnectionString(string conString)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string key = string.Empty;
      string empty = string.Empty;
      int currentPosition = 0;
      int vallength = 0;
      bool isempty = false;
      char[] charArray = conString.ToCharArray();
      char[] valuebuf = new char[conString.Length];
      while (currentPosition < conString.Length)
      {
        currentPosition = ConStringUtil.GetKeyValuePair(charArray, currentPosition, out key, valuebuf, out vallength, out isempty);
        if (!isempty)
        {
          string str = new string(valuebuf, 0, vallength);
          if (KeywordMapper.OledbAdoDotNetKeywordsMapping.ContainsKey(key))
            key = KeywordMapper.OledbAdoDotNetKeywordsMapping[key];
          stringBuilder.Append(key);
          stringBuilder.Append("=");
          stringBuilder.Append(str);
          stringBuilder.Append(";");
        }
      }
      return stringBuilder.ToString();
    }

    private static void SkipValue(string conString, ref int index)
    {
      ConStringUtil.SkipWhiteSpace(conString, ref index);
      if (index == conString.Length)
        return;
      char ch = conString[index];
      switch (ch)
      {
        case '"':
        case '\'':
          do
          {
            int num = conString.IndexOf(ch, index + 1);
            index = num + 1;
          }
          while (index < conString.Length && (int) conString[index] == (int) ch);
          ConStringUtil.SkipWhiteSpace(conString, ref index);
          break;
        case ';':
          break;
        default:
          index = conString.IndexOf(';', index + 1);
          if (index != -1)
            break;
          index = conString.Length;
          break;
      }
    }

    private static void SkipWhiteSpace(string conString, ref int index)
    {
      while (index < conString.Length)
      {
        switch (conString[index])
        {
          case '\t':
          case ' ':
            ++index;
            continue;
          default:
            return;
        }
      }
    }

    private static void ParseStringIntoTable(string conString, Dictionary<string, string> values)
    {
      int currentPosition = 0;
      int vallength = 0;
      bool isempty = false;
      string key = string.Empty;
      string empty = string.Empty;
      char[] charArray = conString.ToCharArray();
      char[] valuebuf = new char[conString.Length];
      while (currentPosition < conString.Length)
      {
        currentPosition = ConStringUtil.GetKeyValuePair(charArray, currentPosition, out key, valuebuf, out vallength, out isempty);
        if (!isempty)
        {
          string str = new string(valuebuf, 0, vallength);
          if (!ConStringUtil.InsertKeyValue(values, ref key, str))
            throw new ArgumentException(Res.GetString("SQL_InvalidConStringOption", (object) key));
        }
      }
    }

    private static Exception ConnectionStringSyntax(int index, char[] connectionString) => (Exception) new ArgumentException(Res.GetString("ADP_ConnectionStringSyntax", (object) index));

    private static int GetKeyValuePair(
      char[] connectionString,
      int currentPosition,
      out string key,
      char[] valuebuf,
      out int vallength,
      out bool isempty)
    {
      ConStringUtil.ParserState parserState = ConStringUtil.ParserState.NothingYet;
      int bufPosition = 0;
      int index = currentPosition;
      key = (string) null;
      vallength = -1;
      isempty = false;
      char minValue = char.MinValue;
      if (connectionString.Length >= int.MaxValue)
        throw new OverflowException();
      for (; currentPosition < connectionString.Length; ++currentPosition)
      {
        minValue = connectionString[currentPosition];
        switch (parserState)
        {
          case ConStringUtil.ParserState.NothingYet:
            if (';' != minValue && !char.IsWhiteSpace(minValue))
            {
              index = currentPosition;
              if (minValue == char.MinValue)
              {
                parserState = ConStringUtil.ParserState.NullTermination;
                break;
              }
              if (char.IsControl(minValue))
                throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
              parserState = ConStringUtil.ParserState.Key;
              bufPosition = 0;
              goto default;
            }
            else
              break;
          case ConStringUtil.ParserState.Key:
            if ('=' == minValue)
            {
              parserState = ConStringUtil.ParserState.KeyEqual;
              break;
            }
            if (!char.IsWhiteSpace(minValue) && char.IsControl(minValue))
              throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
            goto default;
          case ConStringUtil.ParserState.KeyEqual:
            if ('=' == minValue)
            {
              parserState = ConStringUtil.ParserState.Key;
              goto default;
            }
            else
            {
              key = ConStringUtil.GetKey(valuebuf, bufPosition);
              bufPosition = 0;
              parserState = ConStringUtil.ParserState.KeyEnd;
              goto case ConStringUtil.ParserState.KeyEnd;
            }
          case ConStringUtil.ParserState.KeyEnd:
            if (!char.IsWhiteSpace(minValue))
            {
              if ('\'' == minValue)
              {
                parserState = ConStringUtil.ParserState.SingleQuoteValue;
                break;
              }
              if ('"' == minValue)
              {
                parserState = ConStringUtil.ParserState.DoubleQuoteValue;
                break;
              }
              if (';' != minValue && minValue != char.MinValue)
              {
                if (char.IsControl(minValue))
                  throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
                parserState = ConStringUtil.ParserState.UnquotedValue;
                goto default;
              }
              else
                goto label_59;
            }
            else
              break;
          case ConStringUtil.ParserState.UnquotedValue:
            if (char.IsWhiteSpace(minValue) || !char.IsControl(minValue) && ';' != minValue)
              goto default;
            else
              goto label_59;
          case ConStringUtil.ParserState.DoubleQuoteValue:
            if ('"' == minValue)
            {
              parserState = ConStringUtil.ParserState.DoubleQuoteValueQuote;
              break;
            }
            if (minValue == char.MinValue)
              throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
            goto default;
          case ConStringUtil.ParserState.DoubleQuoteValueQuote:
            if ('"' == minValue)
            {
              parserState = ConStringUtil.ParserState.DoubleQuoteValue;
              goto default;
            }
            else
            {
              parserState = ConStringUtil.ParserState.DoubleQuoteValueEnd;
              goto case ConStringUtil.ParserState.DoubleQuoteValueEnd;
            }
          case ConStringUtil.ParserState.DoubleQuoteValueEnd:
            if (!char.IsWhiteSpace(minValue))
            {
              if (';' != minValue)
              {
                if (minValue != char.MinValue)
                  throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
                parserState = ConStringUtil.ParserState.NullTermination;
                break;
              }
              goto label_59;
            }
            else
              break;
          case ConStringUtil.ParserState.SingleQuoteValue:
            if ('\'' == minValue)
            {
              parserState = ConStringUtil.ParserState.SingleQuoteValueQuote;
              break;
            }
            if (minValue == char.MinValue)
              throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
            goto default;
          case ConStringUtil.ParserState.SingleQuoteValueQuote:
            if ('\'' == minValue)
            {
              parserState = ConStringUtil.ParserState.SingleQuoteValue;
              goto default;
            }
            else
            {
              parserState = ConStringUtil.ParserState.SingleQuoteValueEnd;
              goto case ConStringUtil.ParserState.SingleQuoteValueEnd;
            }
          case ConStringUtil.ParserState.SingleQuoteValueEnd:
            if (!char.IsWhiteSpace(minValue))
            {
              if (';' != minValue)
              {
                if (minValue != char.MinValue)
                  throw ConStringUtil.ConnectionStringSyntax(currentPosition, connectionString);
                parserState = ConStringUtil.ParserState.NullTermination;
                break;
              }
              goto label_59;
            }
            else
              break;
          case ConStringUtil.ParserState.NullTermination:
            if (minValue != char.MinValue && !char.IsWhiteSpace(minValue))
              throw ConStringUtil.ConnectionStringSyntax(index, connectionString);
            break;
          default:
            valuebuf[bufPosition++] = minValue;
            break;
        }
      }
      if (ConStringUtil.ParserState.KeyEqual == parserState)
      {
        key = ConStringUtil.GetKey(valuebuf, bufPosition);
        bufPosition = 0;
      }
      if (ConStringUtil.ParserState.Key == parserState || ConStringUtil.ParserState.DoubleQuoteValue == parserState || ConStringUtil.ParserState.SingleQuoteValue == parserState)
        throw ConStringUtil.ConnectionStringSyntax(index, connectionString);
label_59:
      if (ConStringUtil.ParserState.UnquotedValue == parserState)
      {
        bufPosition = ConStringUtil.TrimWhiteSpace(valuebuf, bufPosition);
        if ('\'' == valuebuf[bufPosition - 1] || '"' == valuebuf[bufPosition - 1])
          throw ConStringUtil.ConnectionStringSyntax(currentPosition - 1, connectionString);
      }
      else if (ConStringUtil.ParserState.KeyEqual != parserState && ConStringUtil.ParserState.KeyEnd != parserState)
        isempty = 0 == bufPosition;
      if (';' == minValue && currentPosition < connectionString.Length)
        ++currentPosition;
      vallength = bufPosition;
      return currentPosition;
    }

    private static string GetKey(char[] valuebuf, int bufPosition)
    {
      bufPosition = ConStringUtil.TrimWhiteSpace(valuebuf, bufPosition);
      byte[] bytes = Encoding.Unicode.GetBytes(valuebuf, 0, bufPosition);
      return Encoding.Unicode.GetString(bytes, 0, bytes.Length).ToLower(CultureInfo.InvariantCulture);
    }

    private static int TrimWhiteSpace(char[] valuebuf, int bufPosition)
    {
      while (0 < bufPosition && char.IsWhiteSpace(valuebuf[bufPosition - 1]))
        --bufPosition;
      return bufPosition;
    }

    private static bool InsertKeyValue(
      Dictionary<string, string> values,
      ref string key,
      string value)
    {
      bool flag = true;
      if (string.Empty == value)
        value = (string) null;
      if (ConStringUtil._connectionSynonymMapping.ContainsKey((object) key))
      {
        key = (string) ConStringUtil._connectionSynonymMapping[(object) key];
        if (values.ContainsKey(key))
          values.Remove(key);
        values.Add(key, value);
      }
      else
        flag = false;
      return flag;
    }

    private enum ParserState
    {
      NothingYet = 1,
      Key = 2,
      KeyEqual = 3,
      KeyEnd = 4,
      UnquotedValue = 5,
      DoubleQuoteValue = 6,
      DoubleQuoteValueQuote = 7,
      DoubleQuoteValueEnd = 8,
      SingleQuoteValue = 9,
      SingleQuoteValueQuote = 10, // 0x0000000A
      SingleQuoteValueEnd = 11, // 0x0000000B
      NullTermination = 12, // 0x0000000C
    }
  }
}
