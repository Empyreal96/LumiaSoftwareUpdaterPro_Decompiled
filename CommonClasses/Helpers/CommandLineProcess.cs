// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.CommandLineProcess
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.LsuPro.Helpers
{
  public class CommandLineProcess
  {
    private StringBuilder outputData;

    public string ExecutablePath { get; set; }

    public string Arguments { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "reviewed", MessageId = "1#")]
    public int ExecuteSync(out string output, ref int processId)
    {
      this.outputData = new StringBuilder();
      Process process = new Process();
      try
      {
        process.StartInfo.FileName = this.ExecutablePath;
        process.StartInfo.Arguments = this.Arguments;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.OutputDataReceived += new DataReceivedEventHandler(this.OnOutputDataReceiced);
        process.Start();
        processId = process.Id;
        process.BeginOutputReadLine();
        process.WaitForExit();
        process.OutputDataReceived -= new DataReceivedEventHandler(this.OnOutputDataReceiced);
      }
      catch (Exception ex)
      {
        output = string.Empty;
        return -1;
      }
      output = this.outputData.ToString();
      return process.ExitCode;
    }

    private void OnOutputDataReceiced(object sender, DataReceivedEventArgs e)
    {
      if (e.Data == null)
        return;
      this.outputData.AppendLine(e.Data);
    }
  }
}
