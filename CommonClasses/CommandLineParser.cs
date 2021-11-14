// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.CommandLineParser
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.LsuPro
{
  public class CommandLineParser
  {
    private readonly Dictionary<string, string> options = new Dictionary<string, string>();
    private readonly List<string> filenames = new List<string>();
    private string[] args;
    private char[] prefixes;
    private char valueSeparators;

    public CommandLineParser(string[] args, char[] prefixes = null, char[] valueSeparators = null)
    {
      this.args = args;
      if (prefixes == null)
        this.prefixes = new char[2]{ '/', '-' };
      if (valueSeparators != null)
        return;
      this.valueSeparators = '=';
    }

    public string ExecutablePath { get; private set; }

    public string FileName => !string.IsNullOrWhiteSpace(this.filenames[0]) ? this.filenames[0] : string.Empty;

    public int ParseArguments()
    {
      if (this.args == null || this.args.Length < 1)
        return 0;
      int num = 0;
      if (this.args[0].EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
      {
        this.ExecutablePath = Path.GetDirectoryName(this.args[0]);
        this.args[0] = string.Empty;
      }
      foreach (string str1 in ((IEnumerable<string>) this.args).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))))
      {
        if (str1.IndexOfAny(this.prefixes) != 0)
        {
          this.filenames.Add(str1);
          ++num;
        }
        else if (str1.IndexOf(this.valueSeparators) > 0)
        {
          string str2 = str1.Remove(str1.IndexOf(this.valueSeparators));
          string str3 = str1.Remove(0, str1.IndexOf(this.valueSeparators) + 1);
          this.options.Add(str2.TrimStart(this.prefixes).ToLower(CultureInfo.InvariantCulture), str3);
          ++num;
        }
        else
        {
          this.options.Add(str1.TrimStart(this.prefixes).ToLower(CultureInfo.InvariantCulture), string.Empty);
          ++num;
        }
      }
      return num;
    }

    public string GetOptionValue(string optionName) => !this.options.ContainsKey(optionName.ToLower(CultureInfo.InvariantCulture)) ? string.Empty : this.options[optionName.ToLower(CultureInfo.InvariantCulture)];

    public bool SwitchIsSet(string switchName) => this.options.ContainsKey(switchName.ToLower(CultureInfo.InvariantCulture)) && !this.options[switchName.ToLower(CultureInfo.InvariantCulture)].Equals("false");

    public string GetOptionValue(string optionName, string defaultValue) => !this.options.ContainsKey(optionName.ToLower(CultureInfo.InvariantCulture)) ? defaultValue : this.options[optionName.ToLower(CultureInfo.InvariantCulture)];
  }
}
