// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Wp8AdvancedModeChanger
// Assembly: Wp8AdvancedModeChanger, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 4898CE63-E10A-4A15-8755-B18DC0649425
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8AdvancedModeChanger.dll

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.LsuPro
{
  public class Wp8AdvancedModeChanger
  {
    private bool success;

    public string ErrorMessage { get; private set; }

    protected string BcdEditTool { get; set; }

    public bool ChangeModeTo(ConnectedDeviceMode mode)
    {
      Tracer.Information("ChangeModeTo: {0}", (object) mode);
      try
      {
        if (string.IsNullOrWhiteSpace(this.BcdEditTool))
        {
          this.ErrorMessage = "BcdEdit tool path must be set before running mode changer.";
          return false;
        }
        if (mode == ConnectedDeviceMode.Normal)
          this.SetToDefault();
        if (mode == ConnectedDeviceMode.QcomSerialComposite)
          this.SetToSerial();
        if (mode == ConnectedDeviceMode.QcomRmnetComposite)
          this.SetToRmNet();
        return true;
      }
      catch (DriveNotFoundException ex)
      {
        this.ErrorMessage = "Drive not found: " + ex.Message;
      }
      catch (FileNotFoundException ex)
      {
        this.ErrorMessage = "File not found " + ex.Message;
      }
      catch (Exception ex)
      {
        this.ErrorMessage = ex.Message;
      }
      return false;
    }

    public bool TryLocateBdcEditTool(string bcdEditPath)
    {
      try
      {
        this.BcdEditTool = this.FindBcdEditInPath(bcdEditPath);
        if (string.IsNullOrEmpty(this.BcdEditTool))
        {
          this.BcdEditTool = this.FindBcdEditInSystem();
          if (string.IsNullOrEmpty(this.BcdEditTool))
            return false;
        }
        return true;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Information(ex, "BcdEdit tool not found.", objArray);
      }
      return false;
    }

    private void SetToDefault()
    {
      Tracer.Information("Activating Default Retail SW USB Configuration (IpOverUsb, MTP and Nokia Care)");
      this.ExecuteUsbConfigurationChangeForMode(Wp8AdvancedModeChanger.DetectMassStorageDriveLetter(), "1");
    }

    private void ExecuteUsbConfigurationChangeForMode(string massStorageDrive, string action)
    {
      string path1 = string.Empty + Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      ProcessStartInfo processStartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(path1, "Wp8AdvancedModeChanger.bat")),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = path1
      };
      processStartInfo.EnvironmentVariables.Add("MASS_STORAGE_DRIVE", massStorageDrive);
      processStartInfo.EnvironmentVariables.Add("ACTION", action);
      processStartInfo.EnvironmentVariables.Add("BCDEDIT", this.BcdEditTool);
      processHelper.StartInfo = processStartInfo;
      Tracer.Information("ExecuteUsbConfigurationChangeForMode: {0}, Drive: {1}, Action: {2}, BcdEdit: {3}", (object) processHelper.StartInfo.FileName, (object) massStorageDrive, (object) action, (object) this.BcdEditTool);
      this.success = false;
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.ChangeUsbConfigurationProcessOnOutputDataReceived);
      processHelper.Start();
      processHelper.BeginOutputReadLine();
      processHelper.WaitForExit();
      if (processHelper.ExitCode != 0)
      {
        Tracer.Error("Configuration change execution failed.");
        throw new FileNotFoundException("bcdedit tool not found");
      }
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.ChangeUsbConfigurationProcessOnOutputDataReceived);
    }

    private void ChangeUsbConfigurationProcessOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs e)
    {
      if (e.Data == null)
      {
        Tracer.Information("No data received");
      }
      else
      {
        Tracer.Information(e.Data);
        if (e.Data.ToLowerInvariant().Contains("the operation completed successfully"))
          this.success = true;
        if (!e.Data.ToLowerInvariant().Contains("the boot configuration data store could not be opened") || this.success)
          return;
        Tracer.Error("Operation failed. Please run LSU Pro as administrator and try again.");
        this.ErrorMessage = "Please run LSU Pro as administrator and try again.";
      }
    }

    private void SetToSerial()
    {
      Tracer.Information("Activating Qualcomm Serial Composite USB Configuration (DIAG, MODEM, NMEA and TRACE)");
      this.ExecuteUsbConfigurationChangeForMode(Wp8AdvancedModeChanger.DetectMassStorageDriveLetter(), "2");
    }

    private void SetToRmNet()
    {
      Tracer.Information("Activating Qualcomm RmNet Composite USB Configuration (DIAG, NMEA, MODEM and RMNET)");
      this.ExecuteUsbConfigurationChangeForMode(Wp8AdvancedModeChanger.DetectMassStorageDriveLetter(), "3");
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string FindBcdEditInPath(string bdcEditSearchPath)
    {
      Tracer.Information("Detecting BCDedit tool from folder " + bdcEditSearchPath);
      if (!DirectoryHelper.DirectoryExist(bdcEditSearchPath))
      {
        Tracer.Information("Bcd edit tool not found in " + bdcEditSearchPath);
        return string.Empty;
      }
      string[] directories1 = DirectoryHelper.GetDirectories(bdcEditSearchPath, "bcdedit");
      string[] directories2 = DirectoryHelper.GetDirectories(bdcEditSearchPath, "x86");
      string[] directories3 = DirectoryHelper.GetDirectories(bdcEditSearchPath, "x64");
      string[] strArray = ((IEnumerable<string>) directories1).Any<string>() || ((IEnumerable<string>) directories2).Any<string>() || ((IEnumerable<string>) directories3).Any<string>() ? DirectoryHelper.GetFiles(bdcEditSearchPath, "bcdedit.exe", SearchOption.AllDirectories) : DirectoryHelper.GetFiles(bdcEditSearchPath, "bcdedit.exe", SearchOption.TopDirectoryOnly);
      if (!((IEnumerable<string>) strArray).Any<string>())
      {
        Tracer.Information("Bcd edit tool not found in " + bdcEditSearchPath);
        return string.Empty;
      }
      if (((IEnumerable<string>) strArray).Count<string>() <= 1)
        return strArray[0];
      return !("AMD64" == Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")) ? ((IEnumerable<string>) strArray).FirstOrDefault<string>((Func<string, bool>) (file => file.Contains("x86"))) : ((IEnumerable<string>) strArray).FirstOrDefault<string>((Func<string, bool>) (file => file.Contains("x64")));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Globalization", "CA1302:DoNotHardcodeLocaleSpecificStrings", Justification = "on purpose", MessageId = "System32")]
    private string FindBcdEditInSystem()
    {
      Tracer.Information("Detecting BCDedit tool in system folder");
      string path1 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Sysnative"), "bcdedit.exe");
      if (File.Exists(path1))
        return path1;
      string path2 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"), "bcdedit.exe");
      return File.Exists(path2) ? path2 : string.Empty;
    }

    private static string DetectMassStorageDriveLetter()
    {
      Tracer.Information("Detecting Mass Storage drive letter.");
      using (IEnumerator<string> enumerator = ((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).Where<DriveInfo>((Func<DriveInfo, bool>) (d => d.DriveType == DriveType.Fixed && d.IsReady && !d.Name.StartsWith("C:", StringComparison.OrdinalIgnoreCase))).Select<DriveInfo, string>((Func<DriveInfo, string>) (d => d.Name.Substring(0, 1))).ToList<string>().Where<string>((Func<string, bool>) (driveLetter => File.Exists(driveLetter + ":\\EFIESP\\efi\\Microsoft\\Boot\\BCD"))).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          string current = enumerator.Current;
          Tracer.Information("Mass Storage drive detected as drive {0}", (object) current.ToUpperInvariant());
          return current.ToUpperInvariant();
        }
      }
      Tracer.Error("Mass Storage drive was not detected.");
      throw new DriveNotFoundException("Mass Storage drive was not identified.");
    }
  }
}
