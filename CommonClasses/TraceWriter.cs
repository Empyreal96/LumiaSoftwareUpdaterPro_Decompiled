// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.TraceWriter
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Ionic.Zip;
using Microsoft.LsuPro.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.LsuPro
{
  public class TraceWriter
  {
    private object syncObject = new object();
    private TextWriter textWriter;
    private static DateTime sessionStart;
    private static string mainLogFileName;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "reviewed")]
    static TraceWriter() => TraceWriter.Instance = new TraceWriter(TraceWriter.GenerateLogFilePath(TraceWriter.GeneratePrefix()));

    private TraceWriter(string logFilePath) => this.textWriter = (TextWriter) new StreamWriter((Stream) new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
    {
      AutoFlush = true
    };

    public static TraceWriter Instance { get; private set; }

    private static string GetLogDirectoryPath()
    {
      string logs = SpecialFolders.Logs;
      if (!Directory.Exists(logs))
        Directory.CreateDirectory(logs);
      return logs;
    }

    public static string GenerateLogFilePath(string prefix)
    {
      DateTime now = DateTime.Now;
      Random random = new Random();
      string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:yyyyMMdd}_{0:HHmm}_{0:ssfff}_{1}_{2}.log", (object) now, (object) prefix, (object) random.Next(100000, 999999));
      if (prefix.Equals("lsupro"))
      {
        TraceWriter.sessionStart = now.AddMinutes(-1.0);
        TraceWriter.mainLogFileName = path2;
      }
      return Path.Combine(TraceWriter.GetLogDirectoryPath(), path2);
    }

    public static string CollectSessionLogFiles(string zipFileName = null)
    {
      try
      {
        if (string.IsNullOrEmpty(TraceWriter.mainLogFileName))
          throw new InvalidOperationException("Can't generate zip file for non-LSU Pro process");
        string logDirectoryPath = TraceWriter.GetLogDirectoryPath();
        if (string.IsNullOrEmpty(zipFileName))
        {
          zipFileName = Path.ChangeExtension(TraceWriter.mainLogFileName, "zip");
          zipFileName = Path.Combine(logDirectoryPath, zipFileName);
        }
        Tracer.Information("Generating ZIP file: {0}", (object) Path.GetFileName(zipFileName));
        if (File.Exists(zipFileName))
        {
          Tracer.Information("Deleting ZIP file: {0}", (object) zipFileName);
          File.Delete(zipFileName);
        }
        using (ZipFile zipFile = new ZipFile(zipFileName))
        {
          foreach (string file in DirectoryHelper.GetFiles(logDirectoryPath, "*.log", SearchOption.AllDirectories))
          {
            try
            {
              if (DateTime.Compare(DateTime.ParseExact(PathHelper.GetFileNameWithoutExtension(file).Substring(0, 13), "yyyyMMdd_HHmm", (IFormatProvider) CultureInfo.InvariantCulture), TraceWriter.sessionStart) >= 0)
              {
                Tracer.Information("Adding file to ZIP: {0}", (object) Path.GetFileName(file));
                zipFile.AddFile(file);
              }
            }
            catch (Exception ex)
            {
              Tracer.Error("Problem with adding log to zip: {0}", (object) ex.Message);
            }
          }
          try
          {
            List<string> list = ((IEnumerable<string>) DirectoryHelper.GetFiles(logDirectoryPath, "*aduservice*.log", SearchOption.AllDirectories)).OrderByDescending<string, string>((Func<string, string>) (date => date), (IComparer<string>) new DateComparer()).ToList<string>();
            zipFile.AddFile(list[0], string.Empty);
          }
          catch (Exception ex)
          {
            Tracer.Warning(ex, "Cannot ad ADu service log to zip: {0}", (object) ex.Message);
          }
          zipFile.Save();
        }
        Tracer.Information("ZIP file generated OK: {0}", (object) zipFileName);
        return zipFileName;
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "Creation of ZIP file failed: {0}", (object) ex.Message);
        throw;
      }
    }

    public static string CollectSessionLogFiles(string zipFileName, DateTime dateTime)
    {
      try
      {
        if (string.IsNullOrEmpty(TraceWriter.mainLogFileName))
          throw new InvalidOperationException("Can't generate zip file for non-LSU Pro process");
        string logDirectoryPath = TraceWriter.GetLogDirectoryPath();
        if (string.IsNullOrEmpty(zipFileName))
        {
          zipFileName = Path.ChangeExtension(TraceWriter.mainLogFileName, "zip");
          zipFileName = Path.Combine(logDirectoryPath, zipFileName);
        }
        Tracer.Information("Generating ZIP file: {0}", (object) Path.GetFileName(zipFileName));
        if (File.Exists(zipFileName))
        {
          Tracer.Information("Deleting ZIP file: {0}", (object) zipFileName);
          File.Delete(zipFileName);
        }
        using (ZipFile zipFile = new ZipFile(zipFileName))
        {
          foreach (string file in DirectoryHelper.GetFiles(logDirectoryPath, "*.log", SearchOption.AllDirectories))
          {
            try
            {
              DateTime exact = DateTime.ParseExact(PathHelper.GetFileNameWithoutExtension(file).Substring(0, 13), "yyyyMMdd_HHmm", (IFormatProvider) CultureInfo.InvariantCulture);
              if (DateTime.Compare(exact, dateTime) <= 0)
              {
                if (DateTime.Compare(exact, dateTime.AddHours(-4.0)) >= 0)
                {
                  Tracer.Information("Adding file to ZIP: {0}", (object) Path.GetFileName(file));
                  zipFile.AddFile(file);
                }
              }
            }
            catch (Exception ex)
            {
              Tracer.Error("Problem with adding log to zip: {0}", (object) ex.Message);
            }
          }
          try
          {
            List<string> list = ((IEnumerable<string>) DirectoryHelper.GetFiles(logDirectoryPath, "*aduservice*.log", SearchOption.AllDirectories)).OrderByDescending<string, string>((Func<string, string>) (date => date), (IComparer<string>) new DateComparer()).ToList<string>();
            zipFile.AddFile(list[0], string.Empty);
          }
          catch (Exception ex)
          {
            Tracer.Warning(ex, "Cannot ad ADu service log to zip: {0}", (object) ex.Message);
          }
          zipFile.Save();
        }
        Tracer.Information("ZIP file generated OK: {0}", (object) zipFileName);
        return zipFileName;
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "Creation of ZIP file failed: {0}", (object) ex.Message);
        throw;
      }
    }

    public static void CleanupLogFiles()
    {
      Tracer.Information("Old log files cleanup started");
      TraceWriter.CleanupLogFiles("*.log");
      TraceWriter.CleanupLogFiles("*.zip");
      Tracer.Information("Old log files cleanup finished");
    }

    private static void CleanupLogFiles(string searchPattern)
    {
      try
      {
        DateTime dateTime = DateTime.Now.AddDays(-14.0);
        foreach (string file in DirectoryHelper.GetFiles(SpecialFolders.Logs, searchPattern, SearchOption.AllDirectories))
        {
          try
          {
            if (DateTime.ParseExact(PathHelper.GetFileNameWithoutExtension(file).Remove(13), "yyyyMMdd_HHmm", (IFormatProvider) CultureInfo.InvariantCulture) < dateTime)
              File.Delete(file);
          }
          catch (Exception ex)
          {
            Tracer.Warning("Failed deleting file {0}: {1}", (object) file, (object) ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning("Failed deleting files: {0}", (object) ex.Message);
      }
    }

    public void Write(string line)
    {
      try
      {
        lock (this.syncObject)
        {
          if (this.textWriter == null)
            return;
          this.textWriter.WriteLine(line);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("\n\nTraceWriter failed:\n\n" + ex.ToString() + "\n\n");
      }
    }

    private static string GeneratePrefix()
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
      if (withoutExtension.StartsWith("LumiaSoftwareUpdaterPro", StringComparison.OrdinalIgnoreCase))
        return "lsupro";
      if (withoutExtension.StartsWith("DeviceInfoReader", StringComparison.OrdinalIgnoreCase))
        return "devinfo";
      if (withoutExtension.StartsWith("DeviceModeChanger", StringComparison.OrdinalIgnoreCase))
        return "devmode";
      if (withoutExtension.StartsWith("NvItemReader", StringComparison.OrdinalIgnoreCase))
        return "nvitemreader";
      if (withoutExtension.StartsWith("LsuProInstaller", StringComparison.OrdinalIgnoreCase))
        return "lsuproinstaller";
      if (withoutExtension.StartsWith("Typemock", StringComparison.OrdinalIgnoreCase))
        return "typemock";
      if (withoutExtension.StartsWith("ContinuousUpdater", StringComparison.OrdinalIgnoreCase))
        return "continuousupdater";
      if (withoutExtension.StartsWith("InstallationVerification", StringComparison.OrdinalIgnoreCase))
        return "installationverification";
      return withoutExtension.StartsWith("NvUpdaterLogPoller", StringComparison.OrdinalIgnoreCase) ? "nvpoll" : "unknown";
    }
  }
}
