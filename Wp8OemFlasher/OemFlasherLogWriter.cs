// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OemFlasherLogWriter
// Assembly: Wp8OemFlasher, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0F564F-0EF5-4D78-8BB5-4C7A3BFE4321
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8OemFlasher.dll

using FFUComponents;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.LsuPro
{
  internal class OemFlasherLogWriter : IDisposable
  {
    private readonly object syncObject = new object();
    private StreamWriter logWriter;
    private bool disposed;

    public void StartLogging()
    {
      lock (this.syncObject)
      {
        try
        {
          string logFilePath = TraceWriter.GenerateLogFilePath("ffutool");
          this.logWriter = new StreamWriter(logFilePath)
          {
            AutoFlush = true
          };
          this.LogInformationEntry("Logging to file '{0}'", (object) logFilePath);
        }
        catch (Exception ex)
        {
          object[] objArray = new object[0];
          Tracer.Error(ex, "Failed to open log file", objArray);
        }
      }
    }

    public void StopLogging()
    {
      lock (this.syncObject)
      {
        try
        {
          if (this.logWriter == null)
            return;
          this.logWriter.Close();
          this.logWriter = (StreamWriter) null;
        }
        catch (Exception ex)
        {
          object[] objArray = new object[0];
          Tracer.Error(ex, "Failed to close log file", objArray);
        }
      }
    }

    public void LogInformationEntry(string format, params object[] args)
    {
      Tracer.Information(format, args);
      this.WriteToFile(format, args);
    }

    public void LogInformationEntryInLogFile(string format, params object[] args) => this.WriteToFile(format, args);

    public void LogWarningEntry(string format, params object[] args)
    {
      Tracer.Warning(format, args);
      this.WriteToFile(format, args);
    }

    public void LogErrorEntry(Exception exception, string format, params object[] args)
    {
      Tracer.Error(exception, format, args);
      this.WriteToFile(format, args);
      for (; exception != null; exception = exception.InnerException)
      {
        if (exception is FFUException)
        {
          FFUException ffuException = exception as FFUException;
          this.WriteToFile("{0}/{1} : {2}", (object) ffuException.DeviceFriendlyName, (object) ffuException.DeviceUniqueID, (object) ffuException.ToString());
        }
        else
          this.WriteToFile("{0}", (object) exception.ToString());
      }
    }

    private void WriteToFile(string format, params object[] args)
    {
      lock (this.syncObject)
      {
        try
        {
          if (this.logWriter == null)
            return;
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0} : ", (object) DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture));
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, args);
          this.PrintToFile(stringBuilder.ToString());
        }
        catch (Exception ex)
        {
        }
      }
    }

    private void PrintToFile(string str) => this.logWriter.WriteLine(str);

    ~OemFlasherLogWriter() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      int num = disposing ? 1 : 0;
      if (this.logWriter != null)
      {
        this.logWriter.Close();
        this.logWriter = (StreamWriter) null;
      }
      this.disposed = true;
    }
  }
}
