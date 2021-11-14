// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.MessageTraceSource
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.DeviceDetection.Primitives;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Nokia.Lucid.Diagnostics
{
  internal sealed class MessageTraceSource : TraceSource
  {
    private const string TraceSourceName = "Nokia.Lucid.Messages";
    public static readonly MessageTraceSource Instance = new MessageTraceSource();

    private MessageTraceSource()
      : base("Nokia.Lucid.Messages")
    {
    }

    public void MessageLoopEnter_StartStop()
    {
      this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageLoopEnter, "Entering message loop.");
      this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageLoopEnter, "Entered message loop.");
    }

    public void MessageDispatch_Start(
      IntPtr windowHandle,
      int message,
      IntPtr wParam,
      IntPtr lParam)
    {
      string messageName = KnownNames.GetMessageName(message);
      this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageDispatch, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Dispatching message to window.\r\nHWND: 0x{0:x4}\r\nMessage: 0x{1:x4} ({2})\r\nWPARAM: 0x{3:x4}\r\nLPARAM: 0x{4:x4}", (object) windowHandle.ToInt64(), (object) message, (object) messageName, (object) wParam.ToInt64(), (object) lParam.ToInt64()));
    }

    public void MessageDispatch_Stop(
      IntPtr windowHandle,
      int message,
      IntPtr wParam,
      IntPtr lParam)
    {
      string messageName = KnownNames.GetMessageName(message);
      this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageDispatch, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Dispatched message to window.\r\nHWND: 0x{0:x4}\r\nMessage: 0x{1:x4} ({2})\r\nWPARAM: 0x{3:x4}\r\nLPARAM: 0x{4:x4}", (object) windowHandle.ToInt64(), (object) message, (object) messageName, (object) wParam.ToInt64(), (object) lParam.ToInt64()));
    }

    public void MessageDispatch_Error(
      IntPtr windowHandle,
      int message,
      IntPtr wParam,
      IntPtr lParam,
      Exception exception)
    {
      string messageName = KnownNames.GetMessageName(message);
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.MessageDispatch, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while dispatching message to window, suppressing exception.\r\nHWND: 0x{0:x4}\r\nMessage: 0x{1:x4} ({2})\r\nWPARAM: 0x{3:x4}\r\nLPARAM: 0x{4:x4}\r\nException:\r\n{5}", (object) windowHandle.ToInt64(), (object) message, (object) messageName, (object) wParam.ToInt64(), (object) lParam.ToInt64(), (object) exception));
    }

    public void MessageLoopExit_Start() => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageLoopExit, "Exiting message loop.");

    public void MessageLoopExit_Stop() => this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageLoopExit, "Exited message loop.");

    public void ThreadMessage(int message, IntPtr wParam, IntPtr lParam)
    {
      string messageName = KnownNames.GetMessageName(message);
      this.TraceEvent(TraceEventType.Warning, 3, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Received thread message, discarding.\r\nMessage: 0x{0:x4} ({1})\r\nWPARAM: 0x{2:x4}\r\nLPARAM: 0x{3:x4}", (object) message, (object) messageName, (object) wParam.ToInt64(), (object) lParam.ToInt64()));
    }

    public void WindowMessage(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam)
    {
      string messageName = KnownNames.GetMessageName(message);
      this.TraceEvent(TraceEventType.Verbose, 1, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Received window message.\r\nHWND: 0x{0:x4}\r\nMessage: 0x{1:x4} ({2})\r\nWPARAM: 0x{3:x4}\r\nLPARAM: 0x{4:x4}", (object) windowHandle.ToInt64(), (object) message, (object) messageName, (object) wParam.ToInt64(), (object) lParam.ToInt64()));
    }

    public void DeviceNotification(IntPtr windowHandle, int eventType, int deviceType)
    {
      string identifier1;
      string str1 = KnownNames.TryGetEventTypeName(eventType, out identifier1) ? " (" + identifier1 + ")" : string.Empty;
      string identifier2;
      string str2 = KnownNames.TryGetDeviceTypeName(deviceType, out identifier2) ? " (" + identifier2 + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Verbose, MessageTraceEventId.DeviceNotification, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Received device notification.\r\nHWND: 0x{0:x4}\r\nEvent: 0x{1:x4}{2}\r\nHeader: 0x{3:x4}{4}", (object) windowHandle.ToInt64(), (object) eventType, (object) str1, (object) deviceType, (object) str2));
    }

    public void MessageWindowCreation_Start(string windowClass) => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageWindowCreation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Creating message window.\r\nClass: {0}", (object) windowClass));

    public void MessageWindowCreation_Stop(string windowClass, IntPtr windowHandle) => this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageWindowCreation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Created message window and acquired HWND handle.\r\nClass: {0}\r\nHWND: 0x{1:x4}", (object) windowClass, (object) windowHandle.ToInt64()));

    public void MessageWindowCreation_Error(string windowClass, int errorCode, string errorMessage) => this.TraceEvent(TraceEventType.Error, MessageTraceEventId.MessageWindowCreation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not create message window.\r\nClass: {0}\r\nError: 0x{1:x4} ({2})", (object) windowClass, (object) errorCode, (object) errorMessage));

    public void MessageWindowProcAttach_Start(IntPtr windowHandle) => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageWindowProcAttach, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Attaching window proc to message window.\r\nHWND: 0x{0:x4}", (object) windowHandle.ToInt64()));

    public void MessageWindowProcAttach_Stop(IntPtr windowHandle) => this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageWindowProcAttach, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Attached window proc to message window.\r\nHWND: 0x{0:x4}", (object) windowHandle.ToInt64()));

    public void MessageWindowProcAttach_Error(
      IntPtr windowHandle,
      int errorCode,
      string errorMessage)
    {
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.MessageWindowProcAttach, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not attach window proc to message window.\r\nHWND: {0}\r\nError: 0x{1:x4} ({2})", (object) windowHandle, (object) errorCode, (object) errorMessage));
    }

    public void MessageLoopExitRequest_Start() => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageLoopExitRequest, "Posting WM_QUIT to the message queue.");

    public void MessageLoopExitRequest_Stop() => this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageLoopExitRequest, "Posted WM_QUIT to the message queue.");

    public void MessageWindowCloseRequest_Start(IntPtr windowHandle) => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageWindowCloseRequest, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Posting WM_CLOSE to message queue.\r\nHWND: 0x{0:x4}", (object) windowHandle.ToInt64()));

    public void MessageWindowCloseRequest_Stop(IntPtr windowHandle) => this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageWindowCloseRequest, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Posted WM_CLOSE to message queue.\r\nHWND: 0x{0:x4}", (object) windowHandle.ToInt64()));

    public void MessageWindowCloseRequest_Error(
      IntPtr windowHandle,
      int errorCode,
      string errorText)
    {
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.MessageWindowCloseRequest, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not post WM_CLOSE to message queue.\r\nHWND: 0x{0:x4}\r\nError: 0x{1:x4} ({2})", (object) windowHandle.ToInt64(), (object) errorCode, (object) errorText));
    }

    public void DeviceNotificationRegistration_Start(IntPtr windowHandle, Guid interfaceClass)
    {
      string identifier;
      string str = KnownNames.TryGetInterfaceClassName(interfaceClass, out identifier) ? " (" + identifier + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Start, MessageTraceEventId.DeviceNotificationRegistration, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Registering device notification.\r\nHWND: 0x{0:x4}\r\nClass: {1}{2}", (object) windowHandle.ToInt64(), (object) interfaceClass, (object) str));
    }

    public void DeviceNotificationRegistration_Stop(
      IntPtr windowHandle,
      Guid interfaceClass,
      IntPtr devNotifyHandle)
    {
      string identifier;
      string str = KnownNames.TryGetInterfaceClassName(interfaceClass, out identifier) ? " (" + identifier + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.DeviceNotificationRegistration, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Registered device notification and acquired HDEVNOTIFY handle.\r\nHWND: 0x{0:x4}\r\nClass: {1}{2}\r\nHDEVNOTIFY: 0x{3:x4}", (object) windowHandle.ToInt64(), (object) interfaceClass, (object) str, (object) devNotifyHandle.ToInt64()));
    }

    public void DeviceNotificationRegistration_Error(
      IntPtr windowHandle,
      Guid interfaceClass,
      int errorCode,
      string errorMessage)
    {
      string identifier;
      string str = KnownNames.TryGetInterfaceClassName(interfaceClass, out identifier) ? " (" + identifier + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.DeviceNotificationRegistration, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not register device notification, deferring exception.\r\nHWND: 0x{0:x4}\r\nClass: {1}{2}\r\nError: 0x{3:x4} ({4})", (object) windowHandle.ToInt64(), (object) interfaceClass, (object) str, (object) errorCode, (object) errorMessage));
    }

    public void DeviceNotificationUnregistration_Start(IntPtr windowHandle, IntPtr devNotifyHandle) => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.DeviceNotificationUnregistration, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unregistering device notification.\r\nHWND: 0x{0:x4}\r\nHDEVNOTIFY: 0x{1:x4}", (object) windowHandle.ToInt64(), (object) devNotifyHandle.ToInt64()));

    public void DeviceNotificationUnregistration_Stop(IntPtr windowHandle, IntPtr devNotifyHandle) => this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.DeviceNotificationUnregistration, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unregistered device notification.\r\nHWND: 0x{0:x4}\r\nHDEVNOTIFY: 0x{1:x4}", (object) windowHandle.ToInt64(), (object) devNotifyHandle.ToInt64()));

    public void DeviceNotificationUnregistration_Error(
      IntPtr windowHandle,
      IntPtr devNotifyHandle,
      int errorCode,
      string errorMessage)
    {
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.DeviceNotificationUnregistration, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not unregister device notification, deferring exception.\r\nHWND: 0x{0:x4}\r\nHDEVNOTIFY: 0x{1:x4}\r\nError: 0x{2:x4} ({3})", (object) windowHandle.ToInt64(), (object) devNotifyHandle.ToInt64(), (object) errorCode, (object) errorMessage));
    }

    public void DeviceNotificationProcessing_Start(
      IntPtr windowHandle,
      string devicePath,
      Guid interfaceClass,
      int eventType)
    {
      string identifier1;
      string str1 = KnownNames.TryGetInterfaceClassName(interfaceClass, out identifier1) ? " (" + identifier1 + ")" : string.Empty;
      string identifier2;
      string str2 = KnownNames.TryGetEventTypeName(eventType, out identifier2) ? " (" + identifier2 + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Start, MessageTraceEventId.DeviceNotificationProcessing, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Processing device notification.\r\nHWND: 0x{0:x4}\r\nPath: {1}\r\nClass: {2}{3}\r\nEvent: 0x{4:x4}{5}", (object) windowHandle.ToInt64(), (object) devicePath, (object) interfaceClass, (object) str1, (object) eventType, (object) str2));
    }

    public void DeviceNotificationProcessing_Stop(
      IntPtr windowHandle,
      string devicePath,
      Guid interfaceClass,
      int eventType)
    {
      string identifier1;
      string str1 = KnownNames.TryGetInterfaceClassName(interfaceClass, out identifier1) ? " (" + identifier1 + ")" : string.Empty;
      string identifier2;
      string str2 = KnownNames.TryGetEventTypeName(eventType, out identifier2) ? " (" + identifier2 + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.DeviceNotificationProcessing, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Processed device notification.\r\nHWND: 0x{0:x4}\r\nPath: {1}\r\nClass: {2}{3}\r\nEvent: 0x{4:x4}{5}", (object) windowHandle.ToInt64(), (object) devicePath, (object) interfaceClass, (object) str1, (object) eventType, (object) str2));
    }

    public void DeviceNotificationProcessing_Error(
      IntPtr windowHandle,
      string devicePath,
      Guid interfaceClass,
      int eventType,
      Exception exception)
    {
      string identifier1;
      string str1 = KnownNames.TryGetInterfaceClassName(interfaceClass, out identifier1) ? " (" + identifier1 + ")" : string.Empty;
      string identifier2;
      string str2 = KnownNames.TryGetEventTypeName(eventType, out identifier2) ? " (" + identifier2 + ")" : string.Empty;
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.DeviceNotificationProcessing, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while processing device notification.\r\nHWND: 0x{0:x4}\r\nPath: {1}\r\nClass: {2}{3}\r\nEvent: 0x{4:x4}{5}\r\nException:\r\n{6}", (object) windowHandle.ToInt64(), (object) devicePath, (object) interfaceClass, (object) str1, (object) eventType, (object) str2, (object) exception));
    }

    public void ThreadExceptionDelegation_Start(IntPtr windowHandle, Exception threadException) => this.TraceEvent(TraceEventType.Start, MessageTraceEventId.ThreadExceptionDelegation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Delegating thread exception to thread exception handler.\r\nHWND: 0x{0:x4}\r\nThread exception:\r\n{1}", (object) windowHandle.ToInt64(), (object) threadException));

    public void ThreadExceptionDelegation_Error(
      IntPtr windowHandle,
      Exception threadException,
      Exception exception)
    {
      this.TraceEvent(TraceEventType.Error, MessageTraceEventId.ThreadExceptionDelegation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while delegating thread exception to thread exception handler, deferring thread exception.\r\nHWND: 0x{0:x4}\r\nThread exception:\r\n{1}\r\nException:\r\n{2}", (object) windowHandle.ToInt64(), (object) threadException, (object) exception));
    }

    public void ThreadExceptionDelegation_Stop(
      IntPtr windowHandle,
      bool handled,
      Exception threadException)
    {
      string messageText;
      if (handled)
        messageText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Thread exception handled by thread exception handler, suppressing thread exception.\r\nHWND: 0x{0:x4}\r\nThread exception:\r\n{1}", (object) windowHandle.ToInt64(), (object) threadException);
      else
        messageText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Thread exception not handled by thread exception handler, deferring thread exception.\r\nHWND: 0x{0:x4}\r\nThread exception:\r\n{1}", (object) windowHandle.ToInt64(), (object) threadException);
      this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.ThreadExceptionDelegation, messageText);
    }

    public void MessageWindowStatusChange_Start(
      IntPtr windowHandle,
      MessageWindowStatus oldStatus,
      MessageWindowStatus newStatus)
    {
      this.TraceEvent(TraceEventType.Start, MessageTraceEventId.MessageWindowStatusChange, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Changing message window status.\r\nHWND: 0x{0:x4}\r\nOld status: {1}\r\nNew status: {2}", (object) windowHandle.ToInt64(), (object) oldStatus, (object) newStatus));
    }

    public void MessageWindowStatusChange_Stop(
      IntPtr windowHandle,
      MessageWindowStatus oldStatus,
      MessageWindowStatus newStatus)
    {
      this.TraceEvent(TraceEventType.Stop, MessageTraceEventId.MessageWindowStatusChange, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Changed message window status.\r\nHWND: 0x{0:x4}\r\nOld status: {1}\r\nNew status: {2}", (object) windowHandle.ToInt64(), (object) oldStatus, (object) newStatus));
    }

    private void TraceEvent(TraceEventType eventType, MessageTraceEventId id, string messageText) => this.TraceEvent(eventType, (int) id, messageText);
  }
}
