// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Extensions
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using Newtonsoft.Json;
using System;

namespace SoftwareRepository
{
  public static class Extensions
  {
    public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);

    public static string ToSpeedFormat(this double speed) => string.Format("{0}/s", (object) Extensions.ByteSizeConverter((long) Math.Round(speed)));

    private static string ByteSizeConverter(long size)
    {
      string str = size >= 1024L ? (size >= 1048576L ? (size >= 1073741824L ? (size >= 1099511627776L ? string.Format("{0:F2} TiB", (object) (1.0 * (double) size / 1099511627776.0)) : string.Format("{0:F2} GiB", (object) (1.0 * (double) size / 1073741824.0))) : string.Format("{0:F2} MiB", (object) (1.0 * (double) size / 1048576.0))) : string.Format("{0:F2} KiB", (object) (1.0 * (double) size / 1024.0))) : string.Format("{0} B", (object) size);
      return size >= 1024L ? string.Format("{0} ({1} B)", (object) str, (object) size) : str;
    }
  }
}
