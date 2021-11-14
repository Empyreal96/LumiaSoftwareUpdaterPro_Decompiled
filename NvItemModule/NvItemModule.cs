// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemModule
// Assembly: NvItemModule, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0B184167-245E-49B5-887C-F5F0E401EE86
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemModule.dll

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class NvItemModule
  {
    private bool writeNvItems;
    private bool operationInProgress;
    private Collection<NvItemException> warnings = new Collection<NvItemException>();
    private Collection<NvItemException> errors = new Collection<NvItemException>();

    public event EventHandler<OperationProgressUpdatedEventArgs> OperationProgressUpdated;

    public event EventHandler<OperationStateUpdatedEventArgs> OperationStateUpdated;

    private string PortId { get; set; }

    private string DevicePath { get; set; }

    private string NvItemFile { get; set; }

    public int NvItemReaderProcessId { get; private set; }

    public bool CanStartNvItemOperation => !this.operationInProgress;

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public Collection<string> ReadNvItemsFromFile(string nvItemFile)
    {
      Collection<string> collection = new Collection<string>();
      if (string.IsNullOrWhiteSpace(nvItemFile))
        throw new ArgumentNullException(nameof (nvItemFile));
      using (StreamReader streamReader = new StreamReader(nvItemFile))
      {
        while (true)
        {
          string str1 = streamReader.ReadLine();
          if (str1 != null)
          {
            string str2 = str1.Replace(" ", string.Empty);
            collection.Add(str2);
          }
          else
            break;
        }
      }
      return collection;
    }

    public void ReadNvItems(string nvItemFile, ConnectedDevice device)
    {
      this.writeNvItems = false;
      this.NvItemFile = nvItemFile;
      this.PortId = device.PortId;
      this.warnings.Clear();
      this.errors.Clear();
      Tracer.Information("Starting to read NvItems on port {0} with file {1}", (object) this.PortId, (object) this.NvItemFile);
      TaskHelper taskHelper = new TaskHelper((Action) (() =>
      {
        bool flag = false;
        int num = 0;
        do
        {
          if (device.DeviceReady)
          {
            flag = true;
            this.DevicePath = device.DevicePath;
            this.ExecuteNvItemOperation();
          }
          else
          {
            if (num > 50)
              flag = true;
            ++num;
            Thread.Sleep(100);
          }
        }
        while (!flag);
        Tracer.Information("NvItem reading on port {0} completed", (object) this.PortId);
      }));
      taskHelper.Start();
      taskHelper.ContinueWith((Action<object>) (t =>
      {
        if (taskHelper.Exception == null)
          return;
        foreach (object innerException in taskHelper.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    public void WriteNvItems(string nvItemFile, ConnectedDevice device)
    {
      this.writeNvItems = true;
      this.NvItemFile = nvItemFile;
      this.PortId = device.PortId;
      this.warnings.Clear();
      this.errors.Clear();
      Tracer.Information("Starting to write NvItems on port {0} with file {1}", (object) this.PortId, (object) this.NvItemFile);
      TaskHelper taskHelper = new TaskHelper((Action) (() =>
      {
        bool flag = false;
        int num = 0;
        do
        {
          if (device.DeviceReady)
          {
            flag = true;
            this.DevicePath = device.DevicePath;
            this.ExecuteNvItemOperation();
          }
          else
          {
            if (num > 50)
              flag = true;
            ++num;
            Thread.Sleep(100);
          }
        }
        while (!flag);
        Tracer.Information("NvItem writing on port {0} completed", (object) this.PortId);
      }));
      taskHelper.Start();
      taskHelper.ContinueWith((Action<object>) (t =>
      {
        if (taskHelper.Exception == null)
          return;
        foreach (object innerException in taskHelper.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void ExecuteNvItemOperation()
    {
      this.operationInProgress = true;
      this.OnOperationStateUpdated(new OperationStateUpdatedEventArgs(OperationState.OperationStarted));
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      ProcessHelper processHelper = new ProcessHelper()
      {
        EnableRaisingEvents = true
      };
      ProcessStartInfo processStartInfo = new ProcessStartInfo()
      {
        FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) Path.Combine(directoryName, "NvItemReader.exe")),
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-port={0} -nvitemfile=\"{1}\" -path=\"{2}\"", (object) this.PortId, (object) this.NvItemFile, (object) this.DevicePath),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = directoryName
      };
      if (this.writeNvItems)
      {
        processStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -writenvitems", (object) processStartInfo.Arguments);
        this.OnOperationStateUpdated(new OperationStateUpdatedEventArgs(OperationState.WritingNvItems));
      }
      else
      {
        processStartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -readnvitems", (object) processStartInfo.Arguments);
        this.OnOperationStateUpdated(new OperationStateUpdatedEventArgs(OperationState.ReadingNvItems));
      }
      processHelper.StartInfo = processStartInfo;
      processHelper.OutputDataReceived += new DataReceivedEventHandler(this.ProcessOnOutputDataReceived);
      Tracer.Information("Starting process");
      processHelper.Start();
      Tracer.Information("Process started");
      processHelper.BeginOutputReadLine();
      this.NvItemReaderProcessId = processHelper.Id;
      Tracer.Information("Waiting process {0} to exit", (object) processHelper.Id);
      processHelper.WaitForExit();
      processHelper.OutputDataReceived -= new DataReceivedEventHandler(this.ProcessOnOutputDataReceived);
      Tracer.Information("Process terminated, exit code {0}", (object) processHelper.ExitCode);
      if (processHelper.ExitCode < 0)
        ReportSender.SaveReportAsync(new ReportDetails()
        {
          Uri = 104909L,
          UriDescription = "NvItemReader crash"
        }, DateTime.Now);
      this.operationInProgress = false;
      if (this.errors.Count != 0)
        this.OnOperationStateUpdated(new OperationStateUpdatedEventArgs(OperationState.OperationError, this.errors));
      else if (this.warnings.Count != 0)
        this.OnOperationStateUpdated(new OperationStateUpdatedEventArgs(OperationState.OperationCompletedWithWarnings, this.warnings));
      else
        this.OnOperationStateUpdated(new OperationStateUpdatedEventArgs(OperationState.OperationCompleted));
    }

    private void ProcessOnOutputDataReceived(
      object sender,
      DataReceivedEventArgs dataReceivedEventArgs)
    {
      try
      {
        if (string.IsNullOrEmpty(dataReceivedEventArgs.Data))
          return;
        Tracer.Information("NvItemReader output [{0}]:{1}", (object) this.PortId, (object) dataReceivedEventArgs.Data);
        if (dataReceivedEventArgs.Data.Contains("Progress:"))
        {
          string data = string.Empty;
          if (!this.writeNvItems)
          {
            int startIndex = dataReceivedEventArgs.Data.IndexOf("Data:", StringComparison.OrdinalIgnoreCase) + 6;
            int length = dataReceivedEventArgs.Data.IndexOf(", Progress:", StringComparison.OrdinalIgnoreCase) - startIndex;
            if (length > 0)
              data = dataReceivedEventArgs.Data.Substring(startIndex, length).Replace(" ", string.Empty);
          }
          Regex regex = new Regex("(\\d+)(?=%)");
          string input = dataReceivedEventArgs.Data.Substring(dataReceivedEventArgs.Data.IndexOf("Progress:", StringComparison.OrdinalIgnoreCase) + 9, 5);
          if (!regex.IsMatch(input))
            return;
          string str = dataReceivedEventArgs.Data.Substring(dataReceivedEventArgs.Data.IndexOf("NvId:", StringComparison.OrdinalIgnoreCase) + 5);
          this.OnOperationProgressUpdated(new OperationProgressUpdatedEventArgs(Convert.ToInt32(regex.Match(dataReceivedEventArgs.Data).Value, (IFormatProvider) CultureInfo.InvariantCulture), data, str.Trim()));
        }
        else if (dataReceivedEventArgs.Data.Contains("Error:"))
        {
          int length = dataReceivedEventArgs.Data.IndexOf(", PortId:", StringComparison.OrdinalIgnoreCase);
          if (length <= 0)
            return;
          string message = dataReceivedEventArgs.Data.Substring(0, length);
          string nvItem = string.Empty;
          int num = dataReceivedEventArgs.Data.IndexOf(", NvItemId:", StringComparison.OrdinalIgnoreCase);
          if (num > 0)
            nvItem = dataReceivedEventArgs.Data.Substring(num + 11);
          this.errors.Add(new NvItemException(message, nvItem));
        }
        else
        {
          if (!dataReceivedEventArgs.Data.Contains("Warning:"))
            return;
          int length = dataReceivedEventArgs.Data.IndexOf(", PortId:", StringComparison.OrdinalIgnoreCase);
          if (length <= 0)
            return;
          string message = dataReceivedEventArgs.Data.Substring(0, length);
          string nvItem = string.Empty;
          int num = dataReceivedEventArgs.Data.IndexOf(", NvItemId:", StringComparison.OrdinalIgnoreCase);
          if (num > 0)
            nvItem = dataReceivedEventArgs.Data.Substring(num + 11);
          this.warnings.Add(new NvItemException(message, nvItem));
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning("Error parsing NvItemReader output. Unable to parse string: {0}, exception {1}", (object) dataReceivedEventArgs.Data, (object) ex);
      }
    }

    private void OnOperationProgressUpdated(OperationProgressUpdatedEventArgs e)
    {
      Tracer.Information("OnOperationProgressUpdated: {0}.", (object) e.Progress);
      EventHandler<OperationProgressUpdatedEventArgs> operationProgressUpdated = this.OperationProgressUpdated;
      if (operationProgressUpdated == null)
        return;
      operationProgressUpdated((object) this, e);
    }

    private void OnOperationStateUpdated(OperationStateUpdatedEventArgs e)
    {
      Tracer.Information("OnOperationStateUpdated: {0}.", (object) e.FlashState);
      EventHandler<OperationStateUpdatedEventArgs> operationStateUpdated = this.OperationStateUpdated;
      if (operationStateUpdated == null)
        return;
      operationStateUpdated((object) this, e);
    }
  }
}
