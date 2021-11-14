// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.Toolkit
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Management;
using System.Reflection;
using System.Threading;

namespace Microsoft.LsuPro.Helpers
{
  public static class Toolkit
  {
    private const string ThorVersionSupportingSafePointReporting = "1.8.2.5";

    public static int CompareTypeDesignators(string t1, string t2)
    {
      try
      {
        return Convert.ToInt32(t1.Substring(t1.IndexOf('-') + 1), (IFormatProvider) CultureInfo.InvariantCulture).CompareTo(Convert.ToInt32(t2.Substring(t2.IndexOf('-') + 1), (IFormatProvider) CultureInfo.InvariantCulture));
      }
      catch (Exception ex)
      {
        return string.Compare(t1, t2, StringComparison.OrdinalIgnoreCase);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "reviewed")]
    public static int GetMedianValue(List<int> listOfValues)
    {
      listOfValues.Sort();
      int index = listOfValues.Count / 2;
      int num = listOfValues[index];
      if (listOfValues.Count % 2 == 0 && listOfValues.Count >= 2)
        num = (listOfValues[index - 1] + listOfValues[index]) / 2;
      return num;
    }

    public static int CheckUriCriticality(
      FlashException exception,
      int fallbackUri,
      string thorVersion)
    {
      try
      {
        int criticalUri = Toolkit.GetCriticalUri(fallbackUri);
        if (exception == null)
          return fallbackUri;
        switch ((Thor2ExitCode) exception.ErrorCode)
        {
          case Thor2ExitCode.Thor2UnexpectedExit:
            return criticalUri;
          case Thor2ExitCode.MsgUnableToSendOrReceiveMessageDuringSffuProgramming:
            switch (VersionComparer.CompareVersions(thorVersion, "1.8.2.5"))
            {
              case VersionComparerResult.FirstIsGreater:
              case VersionComparerResult.NumbersAreEqual:
                return exception.SafePointReached ? fallbackUri : criticalUri;
              default:
                return criticalUri;
            }
          default:
            return exception.SafePointReached ? fallbackUri : criticalUri;
        }
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Failed to check failure criticality", objArray);
        return fallbackUri;
      }
    }

    public static bool CancellableWait(int seconds, CancellationToken cancellationToken)
    {
      for (int index = 0; index < seconds; ++index)
      {
        if (cancellationToken.IsCancellationRequested)
          return false;
        Thread.Sleep(1000);
      }
      return true;
    }

    public static string GetCleanSdCardSize(string sizeInBytesValue)
    {
      long result = 0;
      return long.TryParse(sizeInBytesValue, out result) && result > 0L ? Toolkit.GetCleanSdCardSize(result) : string.Empty;
    }

    public static string GetCleanSdCardSize(long sizeInBytes)
    {
      double a;
      string str;
      if (sizeInBytes <= 536870912L)
      {
        a = (double) sizeInBytes / Math.Pow(2.0, 20.0);
        str = "MB";
      }
      else if (sizeInBytes <= 549755813888L)
      {
        a = (double) sizeInBytes / Math.Pow(2.0, 30.0);
        str = "GB";
      }
      else
      {
        a = (double) sizeInBytes / Math.Pow(2.0, 40.0);
        str = "TB";
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) Math.Pow(2.0, Math.Ceiling(Math.Log(a, 2.0))), (object) str);
    }

    public static string GetFormattedStringForBytes(
      long bytes,
      bool showMbFraction = false,
      bool convertToGb = false)
    {
      string str;
      if (bytes >= 1048576L)
      {
        if (convertToGb && bytes >= 1073741824L)
        {
          str = string.Format((IFormatProvider) NumberFormatInfo.InvariantInfo, "{0:F2} GB", (object) ((double) bytes / 1024.0 / 1024.0 / 1024.0));
        }
        else
        {
          double num = (double) bytes / 1024.0 / 1024.0;
          str = string.Format((IFormatProvider) NumberFormatInfo.InvariantInfo, showMbFraction ? "{0:F1} MB" : "{0:F0} MB", (object) num);
        }
      }
      else if (bytes >= 1024L)
        str = string.Format((IFormatProvider) NumberFormatInfo.InvariantInfo, "{0:F0} KB", (object) ((double) bytes / 1024.0));
      else
        str = string.Format((IFormatProvider) NumberFormatInfo.InvariantInfo, "{0} B", (object) bytes);
      return str;
    }

    public static string GetFormattedStringForTime(int durationInSeconds)
    {
      string str;
      if (durationInSeconds >= 86400)
      {
        int num1 = durationInSeconds / 24 / 60 / 60;
        int num2 = (durationInSeconds - num1 * 86400) / 60 / 60;
        int num3 = (durationInSeconds - num1 * 86400 - num2 * 3600) / 60;
        int num4 = (durationInSeconds - num1 * 86400 - num2 * 3600 - num3 * 60) % 60;
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} day{1}, {2} hour{3}, {4} minute{5}, {6} second{7}", (object) num1, num1 == 1 ? (object) string.Empty : (object) "s", (object) num2, num2 == 1 ? (object) string.Empty : (object) "s", (object) num3, num3 == 1 ? (object) string.Empty : (object) "s", (object) num4, num4 == 1 ? (object) string.Empty : (object) "s");
      }
      else if (durationInSeconds >= 3600)
      {
        int num1 = durationInSeconds / 60 / 60;
        int num2 = (durationInSeconds - num1 * 3600) / 60;
        int num3 = (durationInSeconds - num1 * 3600 - num2 * 60) % 60;
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} hour{1}, {2} minute{3}, {4} second{5}", (object) num1, num1 == 1 ? (object) string.Empty : (object) "s", (object) num2, num2 == 1 ? (object) string.Empty : (object) "s", (object) num3, num3 == 1 ? (object) string.Empty : (object) "s");
      }
      else if (durationInSeconds >= 60)
      {
        int num1 = durationInSeconds / 60;
        int num2 = durationInSeconds % 60;
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} minute{1}, {2} second{3}", (object) num1, num1 == 1 ? (object) string.Empty : (object) "s", (object) num2, num2 == 1 ? (object) string.Empty : (object) "s");
      }
      else
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} second{1}", (object) durationInSeconds, durationInSeconds == 1 ? (object) string.Empty : (object) "s");
      return str;
    }

    public static void ParseDetailedProgress(
      string data,
      out long transferredBytes,
      out long totalBytes,
      out double transferSpeed)
    {
      transferredBytes = 0L;
      totalBytes = 0L;
      transferSpeed = 0.0;
      if (data.Length <= 19)
        return;
      data = data.Substring(19);
      if (data.Contains("("))
      {
        string[] strArray = data.Split('(');
        if (strArray.Length != 2)
          return;
        Toolkit.ParseProgress(strArray[0], out transferredBytes, out totalBytes);
        Toolkit.ParseSpeed(strArray[1], out transferSpeed);
      }
      else
        Toolkit.ParseProgress(data, out transferredBytes, out totalBytes);
    }

    public static Exception CreateEmptyFileOfSpecificSize(string path, long totalSize)
    {
      try
      {
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
          fileStream.SetLength(totalSize);
        return (Exception) null;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    public static bool ExceptionIsOutOfDiskSpaceError(Exception exception) => exception is IOException && (exception.GetHResult() & (int) ushort.MaxValue) == 112;

    public static string GetExceptionAsStringWithoutStackTrace(Exception exception)
    {
      if (exception != null)
      {
        string[] strArray = exception.ToString().Split(new string[1]
        {
          Environment.NewLine
        }, StringSplitOptions.None);
        if (strArray.Length != 0)
          return strArray[0];
      }
      return string.Empty;
    }

    public static bool IsEmergencyFlashDriverInstalled()
    {
      Tracer.Information("Checking emergency driver installation");
      if (Toolkit.InfFilesContainGuidString("{71DE994D-8B7C-43DB-A27E-2AE7CD579A0C}"))
      {
        Tracer.Information("Nokia driver installed");
        return true;
      }
      foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher("SELECT * FROM Win32_SystemDriver").Get())
      {
        string str1 = managementBaseObject.GetPropertyValue("Name").ToString();
        string str2 = managementBaseObject.GetPropertyValue("Description").ToString();
        if (str1 == "qcusbser" && str2 == "Qualcomm USB Device for Legacy Serial Communication")
        {
          Tracer.Information("QCOMM driver installed");
          return true;
        }
      }
      return false;
    }

    public static string DetermineThor2Version()
    {
      try
      {
        string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        if (directoryName != null)
        {
          FileVersionInfo versionInfo = FileVersionInfoHelper.GetVersionInfo(Path.Combine(directoryName, "thor2.exe"));
          if (versionInfo != null)
          {
            Tracer.Information("THOR2 version is {0}", (object) versionInfo.ProductVersion);
            return versionInfo.ProductVersion;
          }
          Tracer.Error("Could not determine THOR2 version. FileVersionInfo was null.");
          return "NA";
        }
        Tracer.Error("Could not determine THOR2 version. Working directory was null.");
        return "NA";
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Could not determine THOR2 version", objArray);
        return "NA";
      }
    }

    public static bool InfFilesContainGuidString(string guid)
    {
      string[] files = DirectoryHelper.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "inf\\oem*.inf", SearchOption.TopDirectoryOnly);
      Array.Reverse((Array) files);
      foreach (string path in files)
      {
        using (StreamReader streamReader = File.OpenText(path))
        {
          if (streamReader.ReadToEnd().Contains(guid))
            return true;
        }
      }
      return false;
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "on purpose", MessageId = "System.Int64.TryParse(System.String,System.Int64@)")]
    private static void ParseProgress(string data, out long transferredBytes, out long totalBytes)
    {
      transferredBytes = 0L;
      totalBytes = 0L;
      string[] strArray = data.Split('/');
      if (strArray.Length != 2)
        return;
      long.TryParse(strArray[0], out transferredBytes);
      long.TryParse(strArray[1], out totalBytes);
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "on purpose", MessageId = "System.Double.TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.Double@)")]
    private static void ParseSpeed(string data, out double transferSpeed) => double.TryParse(data.Substring(0, data.IndexOf("MB", StringComparison.OrdinalIgnoreCase)).Trim(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out transferSpeed);

    private static int GetCriticalUri(int uri)
    {
      if (uri >= 100000 && uri < 101000)
        return 100027;
      if (uri >= 102000 && uri < 103000)
        return 102027;
      return uri >= 103000 && uri < 104000 ? 103027 : uri;
    }
  }
}
