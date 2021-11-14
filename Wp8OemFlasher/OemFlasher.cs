// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OemFlasher
// Assembly: Wp8OemFlasher, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0F564F-0EF5-4D78-8BB5-4C7A3BFE4321
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8OemFlasher.dll

using FFUComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class OemFlasher
  {
    private static uint noFlashableDeviceError = 90000;
    private double lastProgressPercentage;
    private Stopwatch stopwatch;
    private OemFlasherLogWriter logWriter;

    public event EventHandler<OemFlashStartedEventArgs> OemFlashStarted;

    public event EventHandler<OemFlashProgressEventArgs> OemFlashProgress;

    public event EventHandler<OemFlashCompletedEventArgs> OemFlashCompleted;

    public OemFlasher()
    {
      try
      {
        this.ElapsedMilliseconds = 0L;
        this.Version = OemFlasher.DetermineVersion();
      }
      catch (Exception ex)
      {
        this.Version = "unknown";
      }
    }

    public string Version { get; private set; }

    public long ElapsedMilliseconds { get; private set; }

    public void StartFlashing(string pathToFfu)
    {
      this.logWriter = (OemFlasherLogWriter) null;
      this.ElapsedMilliseconds = 0L;
      Task.Factory.StartNew((Action) (() => this.FlashDevice(pathToFfu)));
    }

    private void FlashDevice(string pathToFfu)
    {
      Exception exception = (Exception) null;
      bool flag = false;
      this.logWriter = new OemFlasherLogWriter();
      this.logWriter.StartLogging();
      this.logWriter.LogInformationEntry("Starting to flash '{0}'", (object) pathToFfu);
      this.logWriter.LogInformationEntry("FFUTool version is {0}.", (object) this.Version);
      this.SendStartedEvent();
      try
      {
        this.stopwatch = new Stopwatch();
        this.lastProgressPercentage = -1.0;
        FFUManager.Start();
        flag = true;
        ICollection<IFFUDevice> devices = (ICollection<IFFUDevice>) new List<IFFUDevice>();
        FFUManager.GetFlashableDevices(ref devices);
        using (IFFUDevice device = devices.FirstOrDefault<IFFUDevice>())
        {
          if (this.DeviceIsValid(device))
          {
            this.logWriter.LogInformationEntry("Found flashable device");
            this.logWriter.LogInformationEntry("Unique ID: {0}", (object) device.DeviceUniqueID);
            this.logWriter.LogInformationEntry("Friendly name: {0}", (object) device.DeviceFriendlyName);
            device.ProgressEvent += new EventHandler<ProgressEventArgs>(this.DeviceProgressEvent);
            this.stopwatch.Start();
            device.FlashFFUFile(pathToFfu);
            this.stopwatch.Stop();
            this.ElapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
            this.logWriter.LogInformationEntry("Flash operation finished successfully (elapsed time {0})", (object) TimeSpan.FromMilliseconds((double) this.ElapsedMilliseconds));
            device.ProgressEvent -= new EventHandler<ProgressEventArgs>(this.DeviceProgressEvent);
          }
          else
            this.ThrowNoFlashableDeviceException();
        }
      }
      catch (Exception ex)
      {
        this.logWriter.LogErrorEntry(ex, "Flashing failed");
        exception = ex;
      }
      finally
      {
        if (flag)
          FFUManager.Stop();
        this.logWriter.LogInformationEntry("Done");
        this.logWriter.StopLogging();
        this.logWriter.Dispose();
        this.SendCompletedEvent(new OemFlasherResult(exception));
      }
    }

    private void DeviceProgressEvent(object sender, ProgressEventArgs e)
    {
      this.logWriter.LogInformationEntryInLogFile("Flash progress {0} / {1}", (object) e.Position, (object) e.Length);
      double num = (double) e.Position * 100.0 / (double) e.Length;
      if (num - this.lastProgressPercentage <= 0.5 && num <= 99.0)
        return;
      long speed = (long) ((double) e.Position / this.stopwatch.Elapsed.TotalSeconds);
      this.SendProgressEvent(e.Position, e.Length, (int) num, speed);
      this.lastProgressPercentage = num;
    }

    private void SendStartedEvent()
    {
      EventHandler<OemFlashStartedEventArgs> oemFlashStarted = this.OemFlashStarted;
      if (oemFlashStarted == null)
        return;
      oemFlashStarted((object) this, new OemFlashStartedEventArgs());
    }

    private void SendProgressEvent(long position, long length, int percentage, long speed)
    {
      EventHandler<OemFlashProgressEventArgs> oemFlashProgress = this.OemFlashProgress;
      if (oemFlashProgress == null)
        return;
      oemFlashProgress((object) this, new OemFlashProgressEventArgs(position, length, percentage, speed));
    }

    private void SendCompletedEvent(OemFlasherResult result)
    {
      EventHandler<OemFlashCompletedEventArgs> oemFlashCompleted = this.OemFlashCompleted;
      if (oemFlashCompleted == null)
        return;
      oemFlashCompleted((object) this, new OemFlashCompletedEventArgs(result));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private bool DeviceIsValid(IFFUDevice device) => device != null;

    private void ThrowNoFlashableDeviceException()
    {
      this.logWriter.LogWarningEntry("No flashable device found");
      throw new FlashException(OemFlasher.noFlashableDeviceError, "No flashable device found", true);
    }

    private static string DetermineVersion()
    {
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      if (directoryName != null)
      {
        FileVersionInfo versionInfo = FileVersionInfoHelper.GetVersionInfo(Path.Combine(directoryName, "ffutool.exe"));
        if (versionInfo != null && !string.IsNullOrEmpty(versionInfo.FileVersion))
          return versionInfo.FileVersion;
      }
      return "unknown";
    }
  }
}
