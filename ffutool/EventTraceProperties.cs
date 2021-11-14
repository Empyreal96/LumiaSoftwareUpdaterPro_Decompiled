// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.EventTraceProperties
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.ImageTools
{
  [Serializable]
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct EventTraceProperties
  {
    public const int MaxLoggerNameLength = 260;
    [NonSerialized]
    public EventTraceProperties.EventTracePropertiesCore CoreProperties;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    private string loggerName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    private string logFileName;

    internal static EventTraceProperties CreateProperties(
      string sessionName,
      string logFilePath,
      LoggingModeConstant logMode)
    {
      uint num = (uint) Marshal.SizeOf(typeof (EventTraceProperties));
      EventTraceProperties eventTraceProperties = new EventTraceProperties()
      {
        CoreProperties = {
          Wnode = new EventTraceProperties.WNodeHeader()
        }
      };
      eventTraceProperties.CoreProperties.Wnode.BufferSize = num;
      eventTraceProperties.CoreProperties.Wnode.Flags = 131072U;
      eventTraceProperties.CoreProperties.Wnode.Guid = Guid.NewGuid();
      eventTraceProperties.CoreProperties.BufferSize = 64U;
      eventTraceProperties.CoreProperties.MinimumBuffers = 5U;
      eventTraceProperties.CoreProperties.MaximumBuffers = 200U;
      eventTraceProperties.CoreProperties.FlushTimer = 0U;
      eventTraceProperties.CoreProperties.LogFileMode = logMode;
      if (logFilePath != null && logFilePath.Length < 260)
        eventTraceProperties.logFileName = logFilePath;
      eventTraceProperties.CoreProperties.LogFileNameOffset = (uint) (int) Marshal.OffsetOf(typeof (EventTraceProperties), "logFileName");
      if (sessionName != null && sessionName.Length < 260)
        eventTraceProperties.loggerName = sessionName;
      eventTraceProperties.CoreProperties.LoggerNameOffset = (uint) (int) Marshal.OffsetOf(typeof (EventTraceProperties), "loggerName");
      return eventTraceProperties;
    }

    internal static EventTraceProperties CreateProperties() => EventTraceProperties.CreateProperties((string) null, (string) null, (LoggingModeConstant) 0);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WNodeHeader
    {
      public uint BufferSize;
      public uint ProviderId;
      public ulong HistoricalContext;
      public long TimeStamp;
      public Guid Guid;
      public uint ClientContext;
      public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EventTracePropertiesCore
    {
      public EventTraceProperties.WNodeHeader Wnode;
      public uint BufferSize;
      public uint MinimumBuffers;
      public uint MaximumBuffers;
      public uint MaximumFileSize;
      public LoggingModeConstant LogFileMode;
      public uint FlushTimer;
      public uint EnableFlags;
      public int AgeLimit;
      public uint NumberOfBuffers;
      public uint FreeBuffers;
      public uint EventsLost;
      public uint BuffersWritten;
      public uint LogBuffersLost;
      public uint RealTimeBuffersLost;
      public IntPtr LoggerThreadId;
      public uint LogFileNameOffset;
      public uint LoggerNameOffset;
    }
  }
}
