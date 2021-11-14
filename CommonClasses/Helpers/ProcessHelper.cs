// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.ProcessHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.LsuPro.Helpers
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class ProcessHelper
  {
    private Process process;

    public ProcessHelper()
    {
      this.process = new Process();
      this.process.OutputDataReceived += new DataReceivedEventHandler(this.ProcessOnOutputDataReceived);
      this.process.ErrorDataReceived += new DataReceivedEventHandler(this.ProcessOnErrorDataReceived);
      this.process.Exited += new EventHandler(this.ProcessOnExited);
    }

    public event DataReceivedEventHandler OutputDataReceived;

    public event DataReceivedEventHandler ErrorDataReceived;

    public bool EnableRaisingEvents
    {
      get => this.process.EnableRaisingEvents;
      set => this.process.EnableRaisingEvents = value;
    }

    public ProcessStartInfo StartInfo
    {
      get => this.process.StartInfo;
      set => this.process.StartInfo = value;
    }

    public int Id => this.process.Id;

    public bool HasExited => this.process.HasExited;

    public int ExitCode => this.process.ExitCode;

    public StreamWriter StandardInput => this.process.StandardInput;

    public static Process GetProcessById(int processId) => Process.GetProcessById(processId);

    public static Process[] GetProcessesByName(string processName) => Process.GetProcessesByName(processName);

    public bool Start()
    {
      Tracer.Information("ProcessHelper start called {0}", (object) this.process.StartInfo.FileName);
      return this.process.Start();
    }

    public void Dispose() => this.process.Dispose();

    public void WaitForExit() => this.process.WaitForExit();

    public void WaitForExit(int milliseconds) => this.process.WaitForExit(milliseconds);

    public void BeginOutputReadLine() => this.process.BeginOutputReadLine();

    public void Kill() => this.process.Kill();

    private void ProcessOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (this.OutputDataReceived == null)
        return;
      this.OutputDataReceived(sender, dataReceivedEventArgs);
    }

    private void ProcessOnErrorDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (this.ErrorDataReceived == null)
        return;
      this.ErrorDataReceived(sender, dataReceivedEventArgs);
    }

    private void ProcessOnExited(object sender, EventArgs eventArgs) => Tracer.Information("Process exited");
  }
}
