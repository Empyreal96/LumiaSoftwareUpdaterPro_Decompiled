// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.UsbDeviceIoTraceSource
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Nokia.Lucid.Diagnostics
{
  internal sealed class UsbDeviceIoTraceSource : TraceSource
  {
    private const string TraceSourceName = "Nokia.Lucid.UsbDeviceIo";
    public static readonly UsbDeviceIoTraceSource Instance = new UsbDeviceIoTraceSource();

    private UsbDeviceIoTraceSource()
      : base("Nokia.Lucid.UsbDeviceIo")
    {
    }

    public void DeviceIoInformation(string infoString) => this.TraceEvent(TraceEventType.Verbose, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Message: {0}", (object) infoString));

    public void DeviceIoError(Exception exception) => this.TraceEvent(TraceEventType.Error, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception:\r\n{0}", (object) exception));

    public void DeviceIoErrorWin32(Win32Exception exception) => this.TraceEvent(TraceEventType.Error, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StatusCode: {0}\r\nWin32Exception:\r\n{1}", (object) exception.NativeErrorCode, (object) exception));

    public void DeviceIoMessageOut(byte[] message)
    {
      if (this.Switch.Level == SourceLevels.Information)
        this.TraceEvent(TraceEventType.Information, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, ">> {0}", (object) this.FormatMessageTruncated(message)));
      if ((uint) this.Switch.Level < 31U)
        return;
      this.TraceEvent(TraceEventType.Information, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, ">> {0}", (object) this.FormatMessage(message)));
    }

    public void DeviceIoMessageIn(byte[] message)
    {
      if (this.Switch.Level == SourceLevels.Information)
        this.TraceEvent(TraceEventType.Information, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<< {0}", (object) this.FormatMessageTruncated(message)));
      if ((uint) this.Switch.Level < 31U)
        return;
      this.TraceEvent(TraceEventType.Information, UsbDeviceIoTraceEventId.GenericTrace, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<< {0}", (object) this.FormatMessage(message)));
    }

    private void TraceEvent(
      TraceEventType eventType,
      UsbDeviceIoTraceEventId id,
      string messageText)
    {
      this.TraceEvent(eventType, (int) id, messageText);
    }

    private string FormatMessage(byte[] message)
    {
      StringBuilder stringBuilder = new StringBuilder(message.Length * 4);
      foreach (byte num in message)
      {
        stringBuilder.Append(num.ToString("x2"));
        stringBuilder.Append(", ");
      }
      return stringBuilder.ToString();
    }

    private string FormatMessageTruncated(byte[] message)
    {
      if (message.Length <= 48)
        return this.FormatMessage(message);
      StringBuilder stringBuilder = new StringBuilder(250);
      for (int index = 0; index < message.Length; ++index)
      {
        stringBuilder.Append(message[index].ToString("x2"));
        stringBuilder.Append(", ");
        if (index == 31)
        {
          stringBuilder.Append("... ");
          index = message.Length - 17;
        }
      }
      stringBuilder.Append("(" + message.Length.ToString() + " bytes)");
      return stringBuilder.ToString();
    }
  }
}
