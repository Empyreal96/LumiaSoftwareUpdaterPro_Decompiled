// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvUpdaterLogPollerNamespace.NvUpdaterLogPoller
// Assembly: NvUpdaterLogPoller, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: BEB2359E-8F80-4FFF-A3A2-60B1221F2B0A
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvUpdaterLogPoller.exe

using Microsoft.LsuPro.Helpers;
using Microsoft.LumiaConnectivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nokia.Lucid.DeviceInformation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.LsuPro.NvUpdaterLogPollerNamespace
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class NvUpdaterLogPoller
  {
    private const string GetSupportedLogFiles = "{\"jsonrpc\" : \"2.0\",\"id\" : 0,\"method\" : \"GetSupportedLogFiles\",\"params\" :{\"MessageVersion\" : 0}}";
    private const string GetLogFileSize = "{\"jsonrpc\" : \"2.0\",\"id\" : 0,\"method\" : \"ReadLogFileSize\",\"params\" :{\"MessageVersion\" : 0, \"LogFile\":\"NviUpdater\"}} ";
    private CommandLineParser commandLineParser;
    private Nokia.Lucid.UsbDeviceIo.UsbDeviceIo deviceIo;
    private bool commandLineCall;

    public NvUpdaterLogPoller(string[] args) => this.commandLineParser = new CommandLineParser(args);

    public string DevicePath { get; private set; }

    public string PortId { get; private set; }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    public void PrintHelp()
    {
      Console.WriteLine("NvUpdaterLogPoller.exe");
      Console.WriteLine("Parameters");
      Console.WriteLine("-path=[device]   Device path for the device that is connected.");
      Console.WriteLine("-id=[portId]     PortId, used only for labeling output.");
      Console.WriteLine("-help            Shows help");
      Console.WriteLine("-f               Work without device path.");
    }

    public bool ParseArguments()
    {
      if (this.commandLineParser.ParseArguments() > 0 && this.commandLineParser.SwitchIsSet("help"))
      {
        this.PrintHelp();
        return false;
      }
      this.DevicePath = this.commandLineParser.GetOptionValue("path");
      Tracer.Information("Trying to poll NvUpdater log from device path {0}", (object) this.DevicePath);
      this.PortId = this.commandLineParser.GetOptionValue("id");
      Tracer.Information("PortId {0}", (object) this.PortId);
      this.commandLineCall = this.commandLineParser.SwitchIsSet("f");
      return true;
    }

    public void TryReadDeviceLog()
    {
      try
      {
        if (string.IsNullOrEmpty(this.DevicePath))
        {
          if (!this.commandLineCall)
          {
            Tracer.Error("Device path is not set. NvUpdaterLogPoller requires DevicePath.");
            Console.WriteLine("NvUpdaterLogPoller requires DevicePath as parameter. If you are calling from console, add -f flag to the call.");
            throw new InvalidOperationException("DevicePath needs to be set when calling NvUpdaterLogPoller.");
          }
          DeviceInfo propertySet = new DeviceInfoSet()
          {
            DeviceTypeMap = UsbDeviceScanner.SupportedDevicesMap,
            Filter = UsbDeviceScanner.GetSupportedVidAndPidExpression()
          }.EnumeratePresentDevices().Last<DeviceInfo>((Func<DeviceInfo, bool>) (deviceInfo => (uint) deviceInfo.DeviceType > 0U));
          if (propertySet != null)
          {
            this.DevicePath = propertySet.Path;
            Console.WriteLine("Device detected");
          }
          Tracer.Information("No -path parameter given. Autodetecting device.");
          LucidConnectivityHelper.ParseTypeDesignatorAndSalesName(propertySet.ReadBusReportedDeviceDescription(), out string _, out string _);
        }
        this.deviceIo = new Nokia.Lucid.UsbDeviceIo.UsbDeviceIo(this.DevicePath);
        Tracer.Information("Start log reading.");
        while (!this.PollForNvLogFileAvailable())
          Thread.Sleep(1000);
        int logFileSize = this.ReadLogFileSize();
        Tracer.Information("Log file available in device.");
        Console.WriteLine("Reading log file");
        this.ReadLogFileFromDevice(logFileSize);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "NvUpdaterLogReaderError", objArray);
        throw;
      }
      finally
      {
        if (this.deviceIo != null)
        {
          this.deviceIo.Dispose();
          this.deviceIo = (Nokia.Lucid.UsbDeviceIo.UsbDeviceIo) null;
        }
      }
    }

    internal static string GetPropertyValue(string line)
    {
      string str = string.Empty;
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
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Json response contains error {0}", jsonRpcResponse.Error));
        str = ((JToken) jsonRpcResponse.Result).Root.ToString().Split(':')[1].Trim('}').Replace("\"", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
      }
      catch (Exception ex)
      {
        Tracer.Information("Unable to parse any value from output line {0}, exception {1}", (object) line, (object) ex);
      }
      return str;
    }

    private int ReadLogFileSize()
    {
      int result = 0;
      int num;
      do
      {
        num = result;
        Thread.Sleep(500);
        if (int.TryParse(NvUpdaterLogPoller.GetPropertyValue(this.SendAndReceiveMessage("{\"jsonrpc\" : \"2.0\",\"id\" : 0,\"method\" : \"ReadLogFileSize\",\"params\" :{\"MessageVersion\" : 0, \"LogFile\":\"NviUpdater\"}} ")), out result))
          Tracer.Information("Log size {0}", (object) result);
        else
          num = -1;
      }
      while (num != result);
      return result;
    }

    private void ReadLogFileFromDevice(int logFileSize)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      int offset = 0;
      do
      {
        int readSegmentSize = Math.Min(logFileSize - offset, 8192);
        if (readSegmentSize < 8192)
          flag = false;
        Tracer.Information("Reading {0} bytes", (object) readSegmentSize);
        string str = this.ReadLogContentInBase64Format(readSegmentSize, offset);
        stringBuilder.Append(this.ConvertToUtf8String(str.Substring(0, str.LastIndexOf(','))));
        offset += readSegmentSize;
        if (stringBuilder.Length >= logFileSize - 5)
          flag = false;
      }
      while (flag);
      this.WriteLogToFile(stringBuilder.ToString());
    }

    private string ReadLogContentInBase64Format(int readSegmentSize, int offset = 0)
    {
      string message1 = "{\"jsonrpc\" : \"2.0\",\"id\" : 0,\"method\" : \"ReadLogFile\",\"params\" :{\"MessageVersion\" : 0, \"LogFile\":\"NviUpdater\", \"ReadSize\":" + (object) readSegmentSize + ", \"ReadOffset\":" + (object) offset + "}}";
      Tracer.Information("Read command: {0}", (object) message1);
      string message2 = this.SendAndReceiveMessage(message1);
      Tracer.Information("NvUpdater data read from device.");
      return NvUpdaterLogPoller.GetPropertyValue(message2);
    }

    private void WriteLogToFile(string logContentInUtf8)
    {
      string logFilePath = TraceWriter.GenerateLogFilePath("nvupdater");
      Tracer.Information("Writing NvUpdater log to file {0}", (object) logFilePath);
      string[] contents = logContentInUtf8.Split(new char[1]
      {
        '\n'
      }, StringSplitOptions.RemoveEmptyEntries);
      File.WriteAllLines(logFilePath, contents);
      Console.WriteLine("NvUpdaterLog written to file:{0}, port:{1}", (object) logFilePath, (object) this.PortId);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string ConvertToUtf8String(string base64Log) => Encoding.UTF8.GetString(Convert.FromBase64String(base64Log));

    private bool PollForNvLogFileAvailable() => this.SendAndReceiveMessage("{\"jsonrpc\" : \"2.0\",\"id\" : 0,\"method\" : \"GetSupportedLogFiles\",\"params\" :{\"MessageVersion\" : 0}}").Contains("NviUpdater");

    private string SendAndReceiveMessage(string message)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(message);
      this.deviceIo.Send(bytes, (uint) bytes.Length);
      byte[] receivedData;
      int num = (int) this.deviceIo.Receive(out receivedData, TimeSpan.FromSeconds(5.0));
      return Encoding.Default.GetString(receivedData);
    }
  }
}
