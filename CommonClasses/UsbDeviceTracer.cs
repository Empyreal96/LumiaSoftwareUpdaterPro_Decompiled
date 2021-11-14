// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.UsbDeviceTracer
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.LsuPro
{
  public static class UsbDeviceTracer
  {
    private const string ToolName = "UsbDeviceInfoReader.exe";

    public static int TraceUsbDevices()
    {
      try
      {
        string logFilePath = TraceWriter.GenerateLogFilePath("lumiausbdevices");
        int num = UsbDeviceTracer.RunUsbDeviceTracingTool(logFilePath);
        Tracer.Information("Connected Lumia USB devices traced to '{0}'", (object) logFilePath);
        return num;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Warning(ex, "Failed to trace Lumia USB devices", objArray);
      }
      return 1;
    }

    private static int RunUsbDeviceTracingTool(string targetFile)
    {
      string directoryName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
      if (directoryName != null)
      {
        Process process = new Process()
        {
          StartInfo = {
            FileName = Path.Combine(directoryName, "UsbDeviceInfoReader.exe"),
            CreateNoWindow = true,
            UseShellExecute = false,
            Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) targetFile)
          }
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
      }
      Tracer.Warning("Unable to determine working directory");
      return 1;
    }
  }
}
