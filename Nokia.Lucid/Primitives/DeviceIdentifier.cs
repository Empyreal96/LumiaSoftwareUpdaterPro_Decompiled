// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Primitives.DeviceIdentifier
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nokia.Lucid.Primitives
{
  public sealed class DeviceIdentifier
  {
    private static readonly Regex IdentifierPattern = new Regex("\\bVID_(?<Vid>[0-9A-Z]{4})&PID_(?<Pid>[0-9A-Z]{4})(?:&MI_(?<Mi>\\d{2}))?.*(?<Guid>{[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly string value;
    private readonly string vid;
    private readonly string pid;
    private readonly int? mi;
    private readonly System.Guid guid;

    private DeviceIdentifier(string value, string vid, string pid, int? mi, System.Guid guid)
    {
      this.value = value;
      this.vid = vid;
      this.pid = pid;
      this.mi = mi;
      this.guid = guid;
    }

    public string Value => this.value;

    public static DeviceIdentifier Parse(string value)
    {
      DeviceIdentifier result;
      if (!DeviceIdentifier.TryParse(value, out result))
        throw new FormatException();
      return result;
    }

    public static bool TryParse(string value, out DeviceIdentifier result)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      string vid;
      string pid;
      int? mi;
      System.Guid guid;
      if (!DeviceIdentifier.TryParse(value, out vid, out pid, out mi, out guid))
      {
        result = (DeviceIdentifier) null;
        return false;
      }
      result = new DeviceIdentifier(value, vid, pid, mi, guid);
      return true;
    }

    public bool Vid(string value) => string.Equals(this.vid, value, StringComparison.OrdinalIgnoreCase);

    public bool Vid(params string[] values) => values == null || values.Length == 0 || ((IEnumerable<string>) values).Any<string>((Func<string, bool>) (s => string.Equals(this.vid, s, StringComparison.OrdinalIgnoreCase)));

    public bool Pid(string value) => string.Equals(this.pid, value, StringComparison.OrdinalIgnoreCase);

    public bool Pid(params string[] values) => values == null || values.Length == 0 || ((IEnumerable<string>) values).Any<string>((Func<string, bool>) (s => string.Equals(this.pid, s, StringComparison.OrdinalIgnoreCase)));

    public bool MI(int value) => !this.mi.HasValue || this.mi.Value == value;

    public bool MI(params int[] values) => !this.mi.HasValue || values == null || values.Length == 0 || ((IEnumerable<int>) values).Any<int>((Func<int, bool>) (n => this.mi.Value == n));

    public bool Guid(System.Guid value) => this.guid.Equals(value);

    public bool Guid(params System.Guid[] values) => values == null || values.Length == 0 || ((IEnumerable<System.Guid>) values).Any<System.Guid>((Func<System.Guid, bool>) (s => this.guid.Equals(s)));

    public bool Matches(string vid, string pid, System.Guid guid, int mi) => this.Vid(vid) && this.Pid(pid) && this.Guid(guid) && this.MI(mi);

    public bool Matches(string vid, string pid, System.Guid guid, params int[] mi) => this.Vid(vid) && this.Pid(pid) && this.Guid(guid) && this.MI(mi);

    public bool Matches(string identifier)
    {
      string vid;
      string pid;
      int? mi;
      System.Guid guid;
      return DeviceIdentifier.TryParse(identifier, out vid, out pid, out mi, out guid) && this.Vid(vid) && this.Pid(pid) && (!mi.HasValue || this.MI(mi.Value)) && this.Guid(guid);
    }

    public override string ToString() => this.value;

    private static bool TryParse(
      string value,
      out string vid,
      out string pid,
      out int? mi,
      out System.Guid guid)
    {
      Match match = DeviceIdentifier.IdentifierPattern.Match(value);
      if (!match.Success)
      {
        vid = (string) null;
        pid = (string) null;
        mi = new int?();
        guid = System.Guid.Empty;
        return false;
      }
      vid = match.Groups["Vid"].Value;
      pid = match.Groups["Pid"].Value;
      guid = new System.Guid(match.Groups["Guid"].Value);
      Group group = match.Groups["Mi"];
      mi = group.Success ? new int?(int.Parse(group.Value, NumberStyles.None, (IFormatProvider) NumberFormatInfo.InvariantInfo)) : new int?();
      return true;
    }
  }
}
