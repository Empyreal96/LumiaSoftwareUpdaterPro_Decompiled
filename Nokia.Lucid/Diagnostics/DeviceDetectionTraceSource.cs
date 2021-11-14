// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.DeviceDetectionTraceSource
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.DeviceDetection;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;

namespace Nokia.Lucid.Diagnostics
{
  internal sealed class DeviceDetectionTraceSource : TraceSource
  {
    private const string TraceSourceName = "Nokia.Lucid.DeviceDetection";
    public static readonly DeviceDetectionTraceSource Instance = new DeviceDetectionTraceSource();

    private DeviceDetectionTraceSource()
      : base("Nokia.Lucid.DeviceDetection")
    {
    }

    public void FilterExpressionCompilation_Start(Expression filter) => this.TraceEvent(TraceEventType.Start, DeviceDetectionTraceEventId.FilterExpressionEvaluation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Compiling filter expression.\r\nExpression:\r\n{0}", (object) filter));

    public void FilterExpressionCompilation_Stop(Expression filter) => this.TraceEvent(TraceEventType.Stop, DeviceDetectionTraceEventId.FilterExpressionEvaluation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Compiled filter expression.\r\nExpression:\r\n{0}", (object) filter));

    public void FilterExpressionCompilation_Error(Expression filter, Exception exception) => this.TraceEvent(TraceEventType.Error, DeviceDetectionTraceEventId.FilterExpressionEvaluation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while compiling filter expression.\r\nExpression:\r\n{0}\r\nException:\r\n{1}", (object) filter, (object) exception));

    public void FilterExpressionEvaluation_Start(string devicePath) => this.TraceEvent(TraceEventType.Start, DeviceDetectionTraceEventId.FilterExpressionEvaluation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Evaluating filter expression for device path.\r\nPath: {0}", (object) devicePath));

    public void FilterExpressionEvaluation_Stop(string devicePath, bool matched)
    {
      string messageText;
      if (matched)
        messageText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Device path matches filter expression.\r\nPath: {0}", (object) devicePath);
      else
        messageText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Device path does not match filter expression, rejecting device notification.\r\nPath: {0}", (object) devicePath);
      this.TraceEvent(TraceEventType.Stop, DeviceDetectionTraceEventId.FilterExpressionEvaluation, messageText);
    }

    public void FilterExpressionEvaluation_Error(string devicePath, Exception exception) => this.TraceEvent(TraceEventType.Error, DeviceDetectionTraceEventId.FilterExpressionEvaluation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while evaluating filter expression for device path.\r\nPath: {0}\r\nException:\r\n{1}", (object) devicePath, (object) exception));

    public void DeviceChangeEvent_Start(
      DeviceChangeAction action,
      string devicePath,
      DeviceType deviceType)
    {
      this.TraceEvent(TraceEventType.Start, DeviceDetectionTraceEventId.DeviceChangeEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Raising device change event.\r\nAction: {0}\r\nPath: {1}\r\nType: {2}", (object) action, (object) devicePath, (object) deviceType));
    }

    public void DeviceChangeEvent_Stop(
      DeviceChangeAction action,
      string devicePath,
      DeviceType deviceType)
    {
      this.TraceEvent(TraceEventType.Stop, DeviceDetectionTraceEventId.DeviceChangeEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Raised device change event.\r\nAction: {0}\r\nPath: {1}\r\nType: {2}", (object) action, (object) devicePath, (object) deviceType));
    }

    public void DeviceChangeEvent_Error(
      DeviceChangeAction action,
      string devicePath,
      DeviceType deviceType,
      Exception exception)
    {
      this.TraceEvent(TraceEventType.Error, DeviceDetectionTraceEventId.DeviceChangeEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while raising device change event.\r\nAction: {0}\r\nPath: {1}\r\nType: {2}\r\nException:\r\n{3}", (object) action, (object) devicePath, (object) deviceType, (object) exception));
    }

    public void InvalidDeviceMapping(Guid classGuid, DeviceType deviceType)
    {
      string identifier;
      string str = KnownNames.TryGetInterfaceClassName(classGuid, out identifier) ? " (" + identifier + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Warning, DeviceDetectionTraceEventId.InvalidDeviceMapping, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid device interface class mapping.\r\nClass: {0}{1}\r\nType: {2}", (object) classGuid, (object) str, (object) deviceType));
    }

    private void TraceEvent(
      TraceEventType eventType,
      DeviceDetectionTraceEventId id,
      string messageText)
    {
      this.TraceEvent(eventType, (int) id, messageText);
    }
  }
}
