// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.CommandLineParser
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

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
    private char[] valueSeparators;

    public CommandLineParser(string[] args, char[] prefixes = null, char[] valueSeparators = null)
    {
      this.args = args;
      if (prefixes == null)
        this.prefixes = new char[2]{ '/', '-' };
      if (valueSeparators != null)
        return;
      this.valueSeparators = new char[1]{ '=' };
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
      foreach (string str in ((IEnumerable<string>) this.args).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))))
      {
        if (str.IndexOfAny(this.prefixes) < 0)
        {
          this.filenames.Add(str);
          ++num;
        }
        else if (str.IndexOfAny(this.valueSeparators) > 0)
        {
          string[] strArray = str.Split(this.valueSeparators);
          this.options.Add(strArray[0].TrimStart(this.prefixes).ToLower(CultureInfo.InvariantCulture), strArray[1]);
          ++num;
        }
        else
        {
          this.options.Add(str.TrimStart(this.prefixes).ToLower(CultureInfo.InvariantCulture), string.Empty);
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
