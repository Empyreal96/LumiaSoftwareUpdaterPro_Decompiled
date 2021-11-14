// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.MessageTraceEventId
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

namespace Nokia.Lucid.Diagnostics
{
  internal enum MessageTraceEventId
  {
    WindowMessage = 1,
    DeviceNotification = 2,
    ThreadMessage = 3,
    MessageLoopEnter = 4,
    MessageLoopExit = 5,
    MessageDispatch = 6,
    MessageLoopExitRequest = 7,
    MessageWindowCreation = 8,
    MessageWindowProcAttach = 9,
    MessageWindowCloseRequest = 10, // 0x0000000A
    DeviceNotificationRegistration = 11, // 0x0000000B
    DeviceNotificationUnregistration = 12, // 0x0000000C
    DeviceNotificationProcessing = 13, // 0x0000000D
    MessageWindowStatusChange = 14, // 0x0000000E
    ThreadExceptionDelegation = 15, // 0x0000000F
  }
}
