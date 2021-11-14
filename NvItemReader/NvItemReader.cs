// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemReaderNamespace.NvItemReader
// Assembly: NvItemReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F6886691-C1CC-4CFC-8E60-297EAF353617
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemReader.exe

using Microsoft.LsuPro.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nokia.Lucid.DeviceInformation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.NvItemReaderNamespace
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class NvItemReader
  {
    private readonly string ncsdSupportedVersionForIndexedItems = "1.18";
    private readonly Collection<string> indexedItems = new Collection<string>((IList<string>) new string[2]
    {
      "57",
      "848"
    });
    private CommandLineParser commandLineParser;
    private Nokia.Lucid.UsbDeviceIo.UsbDeviceIo deviceIo;
    private bool readNvItems;
    private bool writeNvItems;
    private string ncsdVersion;
    private int maxretries;
    private int timeout;
    private string tempDirectory = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), nameof (NvItemReader));
    private string filePath;
    private bool forceExit;
    private TaskHelper waitForCloseSignalTask;
    private CancellationTokenSource waitForCloseSignalTaskCancellationTokenSource;
    private CancellationToken waitForCloseSignalTaskCancellationToken;
    private TaskHelper wachdogTask;
    private CancellationTokenSource wachdogTaskCancellationTokenSource;
    private CancellationToken wachdogTaskCancellationToken;

    public NvItemReader(string[] args)
    {
      this.commandLineParser = new CommandLineParser(args);
      this.waitForCloseSignalTaskCancellationTokenSource = new CancellationTokenSource();
      this.waitForCloseSignalTaskCancellationToken = this.waitForCloseSignalTaskCancellationTokenSource.Token;
      this.wachdogTaskCancellationTokenSource = new CancellationTokenSource();
      this.wachdogTaskCancellationToken = this.wachdogTaskCancellationTokenSource.Token;
    }

    public string PortId { get; private set; }

    public string DevicePath { get; private set; }

    public string NvItemFilePath { get; private set; }

    public bool ParseArguments()
    {
      if (this.commandLineParser.ParseArguments() > 0 && this.commandLineParser.SwitchIsSet("help"))
      {
        NvItemReader.PrintHelp();
        return false;
      }
      this.PortId = this.commandLineParser.GetOptionValue("port");
      this.DevicePath = this.commandLineParser.GetOptionValue("path");
      this.NvItemFilePath = this.commandLineParser.GetOptionValue("nvitemfile");
      this.readNvItems = this.commandLineParser.SwitchIsSet("readnvitems");
      this.writeNvItems = this.commandLineParser.SwitchIsSet("writenvitems");
      this.maxretries = string.IsNullOrEmpty(this.commandLineParser.GetOptionValue("retries")) ? 3 : Convert.ToInt32(this.commandLineParser.GetOptionValue("retries"), (IFormatProvider) CultureInfo.InvariantCulture);
      this.timeout = string.IsNullOrEmpty(this.commandLineParser.GetOptionValue("timeout")) ? 1000 : Convert.ToInt32(this.commandLineParser.GetOptionValue("timeout"), (IFormatProvider) CultureInfo.InvariantCulture);
      return !this.writeNvItems || !string.IsNullOrWhiteSpace(this.NvItemFilePath);
    }

    public void PerformOperation()
    {
      try
      {
        this.DeleteCommunicationFile();
        this.StartWaitForCloseSignalTask();
        this.StartWatchdogTask();
        if (string.IsNullOrEmpty(this.DevicePath))
        {
          DeviceInfo deviceInfo1 = new DeviceInfoSet().EnumeratePresentDevices().Where<DeviceInfo>((Func<DeviceInfo, bool>) (deviceInfo => (uint) deviceInfo.DeviceType > 0U)).FirstOrDefault<DeviceInfo>((Func<DeviceInfo, bool>) (deviceInfo => deviceInfo.Path.Contains("0fd3b15c-d457-45d8-a779-c2b2c9f9d0fd")));
          if (deviceInfo1 != null)
          {
            this.DevicePath = deviceInfo1.Path;
            Console.WriteLine("Device detected");
          }
          Tracer.Information("No -path parameter given. Autodetecting first device.");
        }
        this.deviceIo = new Nokia.Lucid.UsbDeviceIo.UsbDeviceIo(this.DevicePath);
        if (this.writeNvItems)
        {
          Tracer.Information("Writing NV Items to device {0}, {1}", (object) this.DevicePath, (object) this.NvItemFilePath);
          this.WriteNvItems(this.NvItemFilePath);
        }
        else
        {
          if (!this.readNvItems)
            return;
          Tracer.Information("Reading NV Items from device {0}, {1}", (object) this.DevicePath, (object) this.NvItemFilePath);
          this.ReadNvItems(this.NvItemFilePath);
        }
      }
      catch (ArgumentNullException ex)
      {
        this.PrintConsoleError("No NvItem file specified", (Exception) ex, this.DevicePath, true);
        throw;
      }
      catch (Exception ex)
      {
        this.PrintConsoleError("Exception caught", ex, this.DevicePath, true);
        throw;
      }
      finally
      {
        this.DeleteCommunicationFile();
        if (this.deviceIo != null)
        {
          this.deviceIo.Dispose();
          this.deviceIo = (Nokia.Lucid.UsbDeviceIo.UsbDeviceIo) null;
        }
      }
    }

    private void ReadNvItems(string nvItemFilePath)
    {
      int num = 0;
      List<string> stringList = this.ReadJsonMessagesFromNviFile(nvItemFilePath);
      this.ncsdVersion = this.GetNcsdVersion();
      Tracer.Information("Reading NV items (device path {0})", (object) this.DevicePath);
      foreach (string jsonMessage in stringList)
      {
        if (this.forceExit)
        {
          Tracer.Information("Force exit.");
          return;
        }
        string result = string.Empty;
        string nvId = string.Empty;
        this.ReadJsonMessage(jsonMessage, out result, out nvId);
        Thread.Sleep(10);
        Tracer.Information("Operation: ReadingNvItems, Data: {0}, Progress: {1}", (object) result, (object) (num * 100 / stringList.Count));
        this.PrintConsoleProgress("ReadingNvItems", result, num * 100 / stringList.Count, this.DevicePath, nvId);
        ++num;
      }
      Tracer.Information("Operation: ReadingNvItems, Data: Success, Progress: 100");
      this.PrintConsoleProgress("ReadingNvItems", string.Empty, 100, this.DevicePath, string.Empty);
    }

    private void ReadJsonMessage(string jsonMessage, out string result, out string nvId)
    {
      int maxretries = this.maxretries;
      result = string.Empty;
      nvId = string.Empty;
      JsonRpcRequest jsonRpcRequest1 = JsonConvert.DeserializeObject<JsonRpcRequest>(jsonMessage);
      string strB = this.NormalizedVersion(this.ncsdSupportedVersionForIndexedItems);
      int num = string.Compare(this.NormalizedVersion(this.ncsdVersion), strB, StringComparison.CurrentCulture);
      for (int index = 1; index <= maxretries; ++index)
      {
        try
        {
          if (this.forceExit)
          {
            Tracer.Information("Force exit.");
            break;
          }
          if (string.Compare(jsonRpcRequest1.Method, "WriteNVData", StringComparison.CurrentCulture) == 0)
          {
            JsonRequestParameters requestParameters = JsonConvert.DeserializeObject<JsonRequestParameters>(JsonConvert.SerializeObject(jsonRpcRequest1.Parameters));
            nvId = requestParameters.Id;
            JsonRpcRequest jsonRpcRequest2;
            if (this.indexedItems.Contains(nvId) && num >= 0)
            {
              if (string.IsNullOrEmpty(requestParameters.SubscriptionId))
                jsonRpcRequest2 = new JsonRpcRequest("ReadNVData")
                {
                  Parameters = (object) new NvIndexedItemRead(Convert.ToInt32(requestParameters.Id, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32((object) requestParameters.NvData[0], (IFormatProvider) CultureInfo.InvariantCulture)),
                  ID = (object) 0
                };
              else
                jsonRpcRequest2 = new JsonRpcRequest("ReadNVData")
                {
                  Parameters = (object) new NvIndexedItemReadSubscriptionId(Convert.ToInt32(requestParameters.Id, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32((object) requestParameters.NvData[0], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(requestParameters.SubscriptionId, (IFormatProvider) CultureInfo.InvariantCulture)),
                  ID = (object) 0
                };
            }
            else if (string.IsNullOrEmpty(requestParameters.SubscriptionId))
              jsonRpcRequest2 = new JsonRpcRequest("ReadNVData")
              {
                Parameters = (object) new NvItemRead(Convert.ToInt32(requestParameters.Id, (IFormatProvider) CultureInfo.InvariantCulture)),
                ID = (object) 0
              };
            else
              jsonRpcRequest2 = new JsonRpcRequest("ReadNVData")
              {
                Parameters = (object) new NvItemReadSubscriptionId(Convert.ToInt32(requestParameters.Id, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(requestParameters.SubscriptionId, (IFormatProvider) CultureInfo.InvariantCulture)),
                ID = (object) 0
              };
            NvItemReadResponse itemReadResponse = JsonConvert.DeserializeObject<NvItemReadResponse>(JsonConvert.SerializeObject((object) JsonConvert.DeserializeObject<JsonRpcResponse>(this.SendAndReceiveMessage(JsonConvert.SerializeObject((object) jsonRpcRequest2)).Replace(" ", string.Empty))));
            JsonRpcRequest jsonRpcRequest3 = new JsonRpcRequest("WriteNVData")
            {
              Parameters = (object) new JsonRequestParameters(requestParameters.MessageVersion, requestParameters.Id, requestParameters.SubscriptionId, itemReadResponse.Result.NVData)
            };
            result = JsonConvert.SerializeObject((object) jsonRpcRequest3);
            break;
          }
          if (string.Compare(jsonRpcRequest1.Method, "WriteEFSData", StringComparison.CurrentCulture) == 0)
          {
            JsonEfsRequestParameters requestParameters = JsonConvert.DeserializeObject<JsonEfsRequestParameters>(JsonConvert.SerializeObject(jsonRpcRequest1.Parameters));
            JsonRpcRequest jsonRpcRequest2 = new JsonRpcRequest("ReadEFSData")
            {
              Parameters = (object) new EfsDataRead(requestParameters.FilePath),
              ID = (object) 0
            };
            nvId = requestParameters.FilePath;
            EfsReadResponse efsReadResponse = JsonConvert.DeserializeObject<EfsReadResponse>(JsonConvert.SerializeObject((object) JsonConvert.DeserializeObject<JsonRpcResponse>(this.SendAndReceiveMessage(JsonConvert.SerializeObject((object) jsonRpcRequest2)))));
            JsonRpcRequest jsonRpcRequest3 = new JsonRpcRequest("WriteEFSData")
            {
              Parameters = (object) new JsonEfsRequestParameters(requestParameters.MessageVersion, requestParameters.FilePath, efsReadResponse.Result.EfsData, requestParameters.ItemType)
            };
            result = JsonConvert.SerializeObject((object) jsonRpcRequest3);
            break;
          }
        }
        catch (Exception ex)
        {
          if (string.Compare(jsonRpcRequest1.Method, "WriteNVData", StringComparison.CurrentCulture) == 0)
          {
            JsonRequestParameters requestParameters = JsonConvert.DeserializeObject<JsonRequestParameters>(JsonConvert.SerializeObject(jsonRpcRequest1.Parameters));
            nvId = requestParameters.Id;
          }
          if (string.Compare(jsonRpcRequest1.Method, "WriteEFSData", StringComparison.CurrentCulture) == 0)
          {
            JsonEfsRequestParameters requestParameters = JsonConvert.DeserializeObject<JsonEfsRequestParameters>(JsonConvert.SerializeObject(jsonRpcRequest1.Parameters));
            nvId = requestParameters.FilePath;
          }
          Tracer.Warning("Failed to read: {0} retry {1} on {2}", (object) jsonMessage, (object) index, (object) maxretries);
          if (ex.Message.Contains("-2080335463"))
          {
            this.PrintConsoleError("Reading NV Item not allowed", ex, this.DevicePath, nvId, false);
            break;
          }
          if (index == maxretries)
          {
            this.PrintConsoleError("Failed to read NV Item after all retries", ex, this.DevicePath, nvId, false);
            break;
          }
          Tracer.Information("Retrying after delay");
          Thread.Sleep(this.timeout);
        }
      }
    }

    private void WriteNvItems(string nvItemFilePath)
    {
      int num = 0;
      List<string> stringList = this.ReadJsonMessagesFromNviFile(nvItemFilePath);
      Tracer.Information("Writing NV items (device path {0})", (object) this.DevicePath);
      try
      {
        this.UnlockNvSpc();
      }
      catch (Exception ex)
      {
        this.PrintConsoleError("UnlockNvSpc failed", ex, this.DevicePath, false);
      }
      try
      {
        this.SetSecurityMode();
      }
      catch (Exception ex)
      {
        this.PrintConsoleError("SetSecurityMode failed", ex, this.DevicePath, false);
      }
      foreach (string jsonMessage in stringList)
      {
        if (this.forceExit)
        {
          Tracer.Information("Force exit.");
          return;
        }
        string result = string.Empty;
        string nvId = string.Empty;
        this.WriteJsonMessage(jsonMessage, out result, out nvId);
        Thread.Sleep(10);
        Tracer.Information("Operation: WritingNvItems progress: {0}, Data: {1}, Result: {2}", (object) (num * 100 / stringList.Count), (object) jsonMessage, (object) result.Replace("\n", string.Empty));
        this.PrintConsoleProgress("WritingNvItems", string.Empty, num * 100 / stringList.Count, this.DevicePath, nvId);
        ++num;
      }
      Tracer.Information("Operation: WritingNvItems, Data: Success, Progress: 100, DevicePath: {0}", (object) this.DevicePath);
      this.PrintConsoleProgress("WritingNvItems", string.Empty, 100, this.DevicePath, string.Empty);
    }

    private void WriteJsonMessage(string jsonMessage, out string result, out string nvId)
    {
      int maxretries = this.maxretries;
      nvId = string.Empty;
      result = string.Empty;
      for (int index = 1; index <= maxretries; ++index)
      {
        try
        {
          if (this.forceExit)
          {
            Tracer.Information("Force exit.");
            break;
          }
          result = this.SendAndReceiveMessage(jsonMessage);
          break;
        }
        catch (Exception ex)
        {
          Tracer.Warning("Failed to write: {0} retry {1} on {2}", (object) jsonMessage, (object) index, (object) maxretries);
          JsonRpcRequest jsonRpcRequest = JsonConvert.DeserializeObject<JsonRpcRequest>(jsonMessage);
          if (string.Compare(jsonRpcRequest.Method, "WriteNVData", StringComparison.CurrentCulture) == 0)
          {
            JsonRequestParameters requestParameters = JsonConvert.DeserializeObject<JsonRequestParameters>(JsonConvert.SerializeObject(jsonRpcRequest.Parameters));
            nvId = requestParameters.Id;
          }
          if (string.Compare(jsonRpcRequest.Method, "WriteEFSData", StringComparison.CurrentCulture) == 0)
          {
            JsonEfsRequestParameters requestParameters = JsonConvert.DeserializeObject<JsonEfsRequestParameters>(JsonConvert.SerializeObject(jsonRpcRequest.Parameters));
            nvId = requestParameters.FilePath;
          }
          if (ex.Message.Contains("-2080335463"))
          {
            this.PrintConsoleError("Writing NV Item not allowed", ex, this.DevicePath, nvId, false);
            break;
          }
          if (index == maxretries)
          {
            this.PrintConsoleError("Failed to write NV Item after all retries", ex, this.DevicePath, nvId, false);
            break;
          }
          Tracer.Information("Retrying after delay");
          Thread.Sleep(this.timeout);
        }
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void PrintConsoleProgress(
      string operation,
      string data,
      int progress,
      string devicePath,
      string nvId)
    {
      Console.WriteLine("Operation: {0}, Data: {1}, Progress: {2}%, DevicePath: {3}, NvId: {4}", (object) operation, (object) data, (object) progress, (object) devicePath, (object) nvId);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void PrintConsoleError(
      string message,
      Exception exception,
      string portId,
      string nvitemId,
      bool error)
    {
      string str1 = string.Empty;
      if (exception != null)
        str1 = exception.Message.Replace('\n', char.MinValue).Replace('\r', char.MinValue);
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      object[] objArray = new object[5]
      {
        error ? (object) "Error" : (object) "Warning",
        (object) message,
        (object) str1,
        (object) portId,
        null
      };
      string str2;
      if (!string.IsNullOrEmpty(nvitemId))
        str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ", NvItemId: {0}", (object) nvitemId);
      else
        str2 = string.Empty;
      objArray[4] = (object) str2;
      string str3 = string.Format((IFormatProvider) invariantCulture, "{0}: {1}, {2}, PortId: {3}{4}", objArray);
      Tracer.Error("PrintConsoleError: {0}", (object) str3);
      Tracer.Error(exception, "Nv Item reader exception");
      Console.WriteLine(str3);
    }

    private void PrintConsoleError(string message, Exception exception, string portId, bool error) => this.PrintConsoleError(message, exception, portId, string.Empty, error);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private List<string> ReadJsonMessagesFromNviFile(string nviFile)
    {
      List<string> stringList = new List<string>();
      if (string.IsNullOrWhiteSpace(nviFile))
        throw new ArgumentNullException(nameof (nviFile));
      using (StreamReader streamReader = new StreamReader(nviFile))
      {
        while (true)
        {
          string str1 = streamReader.ReadLine();
          if (str1 != null)
          {
            string str2 = str1.Replace(" ", string.Empty);
            stringList.Add(str2);
          }
          else
            break;
        }
      }
      return stringList;
    }

    private void SetSecurityMode() => Tracer.Information("SetSecurityMode: {0}", (object) JsonConvert.SerializeObject((object) JsonConvert.DeserializeObject<JsonRpcResponse>(this.SendAndReceiveMessage("{\"jsonrpc\": \"2.0\", \"method\": \"SetSecurityMode\", \"params\": {\"SecurityMode\":\"Maintenance\"}}"))));

    private void UnlockNvSpc() => Tracer.Information("UnlockNvSpc: {0}", (object) JsonConvert.SerializeObject((object) JsonConvert.DeserializeObject<JsonRpcResponse>(this.SendAndReceiveMessage("{\"jsonrpc\": \"2.0\", \"method\": \"UnlockNvSpc\", \"params\": {\"MessageVersion\": 0, \"CodeString\": \"000000\"}, \"id\": 0}"))));

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string NormalizedVersion(string version)
    {
      string[] array = version.Split('.');
      Array.Resize<string>(ref array, 5);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,10}{1,10}{2,10}{3,10}{4,10}", (object[]) array);
    }

    private string GetNcsdVersion()
    {
      Tracer.Information("Reading NCSD version from device: {0}", (object) this.DevicePath);
      return this.GetNcsdVersionValue(this.SendAndReceiveMessage("{\"jsonrpc\": \"2.0\", \"id\": 5,\"method\": \"GetVersion\", \"params\": {\"MessageVersion\": 0}}"));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string GetNcsdVersionValue(string line)
    {
      JsonRpcResponse jsonRpcResponse;
      try
      {
        jsonRpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(line);
      }
      catch (JsonReaderException ex)
      {
        Tracer.Information("Unable to parse jsonresponse from output line {0}, exception {1}", (object) line, (object) ex);
        return string.Empty;
      }
      try
      {
        if (jsonRpcResponse.Error != null)
          return string.Empty;
        return ((JToken) jsonRpcResponse.Result).Root.ToString().Split(':')[4].Trim('}').Replace("\"", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
      }
      catch (Exception ex)
      {
        Tracer.Information("Unable to parse any value from output line {0}, exception {1}", (object) line, (object) ex);
        return string.Empty;
      }
    }

    private string SendAndReceiveMessage(string message)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(message);
      this.deviceIo.Send(bytes, (uint) bytes.Length);
      byte[] receivedData;
      int num = (int) this.deviceIo.Receive(out receivedData, TimeSpan.FromSeconds(5.0));
      return Encoding.Default.GetString(receivedData);
    }

    internal static void PrintHelp()
    {
      Console.WriteLine("NvItemReader.exe");
      Console.WriteLine("Parameters");
      Console.WriteLine("-path=[device]   Device path for the device that is connected.");
      Console.WriteLine("-port=[port]   Usb port id where the device is connected.");
      Console.WriteLine("-nvitemfile=[nvitemfilepath]     NvItem file path.");
      Console.WriteLine("-retries=[maxretries]     Max retries if operation failed. Default is 3.");
      Console.WriteLine("-timeout=[timeout]     Retries timeout in milisecond. Default is 1000ms.");
      Console.WriteLine("-help            Shows help.");
      Console.WriteLine("-readnvitems     Read Nv Items specified in file.");
      Console.WriteLine("-writenvitems    Write Nv Items from file.");
      Console.WriteLine("                 NvItemReader does not change device mode. It reads or writes only Nv Items specified in file.");
      Console.WriteLine("                 in the mode that device is currently in.");
    }

    private void DeleteCommunicationFile()
    {
      this.filePath = Path.Combine(this.tempDirectory, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NvItemReader_{0}.txt", (object) this.PortId.Replace(':', '.')));
      if (!File.Exists(this.filePath))
        return;
      File.Delete(this.filePath);
    }

    private void StartWaitForCloseSignalTask()
    {
      this.waitForCloseSignalTask = new TaskHelper((Action) (() => this.WaitForCloseSignalTask()), this.waitForCloseSignalTaskCancellationToken);
      this.waitForCloseSignalTask.Start();
      this.waitForCloseSignalTask.ContinueWith((Action<object>) (t =>
      {
        if (this.waitForCloseSignalTask.Exception == null)
          return;
        foreach (object innerException in this.waitForCloseSignalTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void WaitForCloseSignalTask()
    {
      this.filePath = Path.Combine(this.tempDirectory, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NvItemReader_{0}.txt", (object) this.PortId.Replace(':', '.')));
      do
      {
        if (!File.Exists(this.filePath))
        {
          Thread.Sleep(500);
        }
        else
        {
          string str;
          try
          {
            StreamReaderHelper streamReaderHelper = new StreamReaderHelper(this.filePath);
            str = streamReaderHelper.ReadLine();
            streamReaderHelper.Close();
          }
          catch (Exception ex)
          {
            object[] objArray = new object[0];
            Tracer.Information(ex, "WaitForCloseSignalTask: Exception", objArray);
            Thread.Sleep(500);
            goto label_10;
          }
          if (!string.IsNullOrEmpty(str) && str == "CloseNvItemReader")
          {
            Tracer.Information("WaitForCloseSignalTask: cancelling tasks");
            if (this.wachdogTaskCancellationTokenSource != null)
              this.wachdogTaskCancellationTokenSource.Cancel();
            this.forceExit = true;
            this.DeleteCommunicationFile();
            break;
          }
          Thread.Sleep(500);
        }
label_10:;
      }
      while (!this.waitForCloseSignalTaskCancellationToken.IsCancellationRequested);
    }

    private void StartWatchdogTask()
    {
      this.wachdogTask = new TaskHelper((Action) (() => this.WatchdogTask()), this.wachdogTaskCancellationToken);
      this.wachdogTask.Start();
      this.wachdogTask.ContinueWith((Action<object>) (t =>
      {
        if (this.wachdogTask.Exception == null)
          return;
        foreach (object innerException in this.wachdogTask.Exception.InnerExceptions)
          Tracer.Error(innerException.ToString());
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void WatchdogTask()
    {
      while (this.CheckIfLsuProIsRunning())
      {
        Thread.Sleep(1000);
        if (this.wachdogTaskCancellationToken.IsCancellationRequested)
          return;
      }
      Tracer.Information("WatchdogTask: Lsu Pro was closed.");
      if (this.waitForCloseSignalTaskCancellationTokenSource != null)
        this.waitForCloseSignalTaskCancellationTokenSource.Cancel();
      this.forceExit = true;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private bool CheckIfLsuProIsRunning()
    {
      try
      {
        Process[] processesByName1 = ProcessHelper.GetProcessesByName("LumiaSoftwareUpdaterPro");
        if (processesByName1.Length != 0)
          return !processesByName1[0].HasExited;
        Process[] processesByName2 = ProcessHelper.GetProcessesByName("LUMIAS~1");
        if (processesByName2.Length != 0)
          return !processesByName2[0].HasExited;
        Process[] processesByName3 = ProcessHelper.GetProcessesByName("LumiaSoftwareUpdaterPro.vshost");
        if (processesByName3.Length != 0)
          return !processesByName3[0].HasExited;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Information(ex, "CheckIfLsuProIsRunning: exception occured.", objArray);
        return false;
      }
      return false;
    }
  }
}
