// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.LsuProInstallerNamespace.LsuProInstaller
// Assembly: LsuProInstaller, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0D77AD-19D4-4325-A92F-5D3ED484BABF
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LsuProInstaller.exe

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.LsuPro.LsuProInstallerNamespace
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class LsuProInstaller
  {
    private CommandLineParser commandLineParser;
    private bool start;

    public LsuProInstaller(string[] args) => this.commandLineParser = new CommandLineParser(args);

    public string UpdateFilePath { get; private set; }

    public bool ParseArguments()
    {
      if (this.commandLineParser.ParseArguments() > 0 && this.commandLineParser.SwitchIsSet("help"))
      {
        this.PrintHelp();
        return false;
      }
      this.UpdateFilePath = this.commandLineParser.GetOptionValue("updatefile");
      this.start = this.commandLineParser.SwitchIsSet("start");
      return true;
    }

    public void PerformOperation()
    {
      this.WaitUntillLsuProClose();
      using (Mutex mutex = new Mutex(false, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Global\\{{{0}}}", (object) nameof (LsuProInstaller))))
      {
        MutexAccessRule rule = new MutexAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.WorldSid, (SecurityIdentifier) null), MutexRights.FullControl, AccessControlType.Allow);
        MutexSecurity mutexSecurity = new MutexSecurity();
        mutexSecurity.AddAccessRule(rule);
        mutex.SetAccessControl(mutexSecurity);
        bool flag = false;
        try
        {
          try
          {
            flag = mutex.WaitOne(10, false);
            if (!flag)
              throw new TimeoutException("Timeout waiting for exclusive access");
          }
          catch (AbandonedMutexException ex)
          {
            Console.WriteLine("Abandoned mutex: {0}", (object) ex.Message);
            flag = true;
          }
          this.RunLsuProInstaller();
        }
        finally
        {
          if (flag)
            mutex.ReleaseMutex();
        }
      }
      if (!this.start)
        return;
      this.RunLsuProApp();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void RunLsuProApp()
    {
      Console.WriteLine("RunLsuProApp: Start LsuPro.");
      string path1 = PathHelper.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Lumia Software Updater Pro\\Bin");
      Process process = new Process()
      {
        EnableRaisingEvents = true
      };
      process.StartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) PathHelper.Combine(path1, "LumiaSoftwareUpdaterPro.exe")),
        UseShellExecute = false,
        RedirectStandardError = false,
        RedirectStandardOutput = false,
        CreateNoWindow = true,
        WorkingDirectory = path1
      };
      process.Start();
    }

    private void RunLsuProInstaller()
    {
      Console.WriteLine("RunLsuProInstaller: run installer");
      Process process = new Process()
      {
        EnableRaisingEvents = true
      };
      ProcessStartInfo processStartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "msiexec.exe"),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/qb /i \"{0}\" /l*v installlog.txt", (object) this.UpdateFilePath),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = PathHelper.GetDirectoryName(this.UpdateFilePath)
      };
      process.EnableRaisingEvents = true;
      process.StartInfo = processStartInfo;
      process.OutputDataReceived += new DataReceivedEventHandler(this.ProcessOnOutputDataReceived);
      process.ErrorDataReceived += new DataReceivedEventHandler(this.ProcessOnErrorDataReceived);
      process.Start();
      process.BeginOutputReadLine();
      process.WaitForExit(300000);
      process.OutputDataReceived -= new DataReceivedEventHandler(this.ProcessOnOutputDataReceived);
      process.ErrorDataReceived -= new DataReceivedEventHandler(this.ProcessOnErrorDataReceived);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void WaitUntillLsuProClose()
    {
      try
      {
        Console.WriteLine("WaitUntillLsuProClose: waiting for close LsuPro");
        Process[] processesByName = Process.GetProcessesByName("LumiaSoftwareUpdaterPro");
        int num = 0;
        do
        {
          Thread.Sleep(1000);
          ++num;
          if (num > 60)
            throw new TimeoutException("Timeout waiting for close LsuPro.");
        }
        while (!processesByName[0].HasExited);
      }
      catch (Exception ex)
      {
        Console.WriteLine("WaitUntillLsuProClose: {0}", (object) ex.Message);
      }
    }

    private void ProcessOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (dataReceivedEventArgs == null || dataReceivedEventArgs.Data == null)
        return;
      Console.WriteLine("ProcessOnOutputDataReceived: {0}", (object) dataReceivedEventArgs.Data);
    }

    private void ProcessOnErrorDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (dataReceivedEventArgs == null || dataReceivedEventArgs.Data == null)
        return;
      Console.WriteLine("ProcessOnErrorDataReceived: {0}", (object) dataReceivedEventArgs.Data);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void PrintHelp()
    {
      Console.WriteLine("LsuProInstaller.exe");
      Console.WriteLine("Parameters");
      Console.WriteLine("-updatefile=[updatefilepath]     Update file path.");
      Console.WriteLine("-start                           Start LSU Pro automatically after update.");
    }
  }
}
