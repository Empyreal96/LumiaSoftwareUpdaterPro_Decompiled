// Decompiled with JetBrains decompiler
// Type: FFUComponents.FlashingHost
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Diagnostics.Eventing;

namespace FFUComponents
{
  public class FlashingHost : IDisposable
  {
    internal EventProviderVersionTwo m_provider = new EventProviderVersionTwo(new Guid("fb961307-bc64-4de4-8828-81d583524da0"));
    private Guid FlashId = new Guid("80ada65c-a7fa-49f8-a2ed-f67790c8f016");
    private Guid DeviceStatusChangeId = new Guid("3a02d575-c63d-4a76-9adf-9b6b736c66dc");
    private Guid TransferId = new Guid("211e6307-fd7c-49f9-a4db-d9ae5a4adb22");
    private Guid BootmeId = new Guid("a0cd9e55-fb70-452f-ac50-2eb82d2984b5");
    private Guid SkipId = new Guid("4979cb5a-17d4-47c6-9ac6-e97446bd74f4");
    private Guid ResetId = new Guid("768fda16-a5c7-44cf-8a47-03580b28538d");
    private Guid RebootId = new Guid("850eedee-9b52-4171-af6f-73c34d84a893");
    private Guid ReconnectId = new Guid("1a80ed37-3a4f-4b81-a466-accb411f96e1");
    private Guid ConnectId = new Guid("bebe24cb-92b1-40ca-843a-f2f9f0cab947");
    private Guid FileReadId = new Guid("d875a842-f690-40bf-880a-16e7d2a88d85");
    private Guid MutexWaitId = new Guid("3120aadc-6b30-4509-bedf-9696c78ddd9c");
    private Guid MassStorageId = new Guid("1b67e5c6-caab-4424-8d24-5c2c258aff5f");
    private Guid StreamClearId = new Guid("d32ce88a-c858-4ed1-86ac-764c58bf2599");
    private Guid ClearIdId = new Guid("3aa9618a-8ac9-4386-b524-c32f4326e59e");
    private Guid WimId = new Guid("0a86e459-1f85-459f-a9da-dca82415c492");
    private Guid WimTransferId = new Guid("53874dd6-905f-4a4c-ac66-5dadb02f4ce8");
    private Guid WimPacketId = new Guid("6f4a3de2-cddd-40d5-829d-861ccbcaff4d");
    protected EventDescriptor Flash_Start;
    protected EventDescriptor Flash_Stop;
    protected EventDescriptor Device_Attach;
    protected EventDescriptor Device_Detach;
    protected EventDescriptor Device_Remove;
    protected EventDescriptor Flash_Error;
    protected EventDescriptor Flash_Timeout;
    protected EventDescriptor TransferException;
    protected EventDescriptor ReconnectIOException;
    protected EventDescriptor ReconnectWin32Exception;
    protected EventDescriptor ReadBootmeIOException;
    protected EventDescriptor ReadBootmeWin32Exception;
    protected EventDescriptor SkipIOException;
    protected EventDescriptor SkipWin32Exception;
    protected EventDescriptor WriteSkipFailed;
    protected EventDescriptor USBResetWin32Exception;
    protected EventDescriptor RebootIOException;
    protected EventDescriptor RebootWin32Exception;
    protected EventDescriptor ConnectWin32Exception;
    protected EventDescriptor ThreadException;
    protected EventDescriptor FileRead_Start;
    protected EventDescriptor FileRead_Stop;
    protected EventDescriptor WaitAbandoned;
    protected EventDescriptor MutexTimeout;
    protected EventDescriptor ConnectNotifyException;
    protected EventDescriptor DisconnectNotifyException;
    protected EventDescriptor InitNotifyException;
    protected EventDescriptor MassStorageIOException;
    protected EventDescriptor MassStorageWin32Exception;
    protected EventDescriptor StreamClearStart;
    protected EventDescriptor StreamClearStop;
    protected EventDescriptor StreamClearPushWin32Exception;
    protected EventDescriptor StreamClearPullWin32Exception;
    protected EventDescriptor StreamClearIOException;
    protected EventDescriptor ClearIdIOException;
    protected EventDescriptor ClearIdWin32Exception;
    protected EventDescriptor WimSuccess;
    protected EventDescriptor WimError;
    protected EventDescriptor WimIOException;
    protected EventDescriptor WimWin32Exception;
    protected EventDescriptor WimTransferStart;
    protected EventDescriptor WimTransferStop;
    protected EventDescriptor WimPacketStart;
    protected EventDescriptor WimPacketStop;
    protected EventDescriptor WimGetStatus;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.m_provider.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public FlashingHost()
    {
      this.Flash_Start = new EventDescriptor(0, (byte) 0, (byte) 0, (byte) 4, (byte) 1, 1, 0L);
      this.Flash_Stop = new EventDescriptor(1, (byte) 0, (byte) 0, (byte) 4, (byte) 2, 1, 0L);
      this.Device_Attach = new EventDescriptor(2, (byte) 0, (byte) 0, (byte) 4, (byte) 10, 2, 0L);
      this.Device_Detach = new EventDescriptor(3, (byte) 0, (byte) 0, (byte) 4, (byte) 11, 2, 0L);
      this.Device_Remove = new EventDescriptor(4, (byte) 0, (byte) 0, (byte) 4, (byte) 12, 2, 0L);
      this.Flash_Error = new EventDescriptor(5, (byte) 0, (byte) 0, (byte) 2, (byte) 0, 1, 0L);
      this.Flash_Timeout = new EventDescriptor(6, (byte) 0, (byte) 0, (byte) 2, (byte) 0, 1, 0L);
      this.TransferException = new EventDescriptor(7, (byte) 0, (byte) 0, (byte) 2, (byte) 0, 3, 0L);
      this.ReconnectIOException = new EventDescriptor(8, (byte) 0, (byte) 0, (byte) 2, (byte) 13, 8, 0L);
      this.ReconnectWin32Exception = new EventDescriptor(9, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 8, 0L);
      this.ReadBootmeIOException = new EventDescriptor(10, (byte) 0, (byte) 0, (byte) 2, (byte) 13, 4, 0L);
      this.ReadBootmeWin32Exception = new EventDescriptor(11, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 4, 0L);
      this.SkipIOException = new EventDescriptor(12, (byte) 0, (byte) 0, (byte) 2, (byte) 0, 5, 0L);
      this.SkipWin32Exception = new EventDescriptor(13, (byte) 0, (byte) 0, (byte) 2, (byte) 0, 5, 0L);
      this.WriteSkipFailed = new EventDescriptor(14, (byte) 0, (byte) 0, (byte) 2, (byte) 15, 5, 0L);
      this.USBResetWin32Exception = new EventDescriptor(15, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 6, 0L);
      this.RebootIOException = new EventDescriptor(16, (byte) 0, (byte) 0, (byte) 2, (byte) 13, 7, 0L);
      this.RebootWin32Exception = new EventDescriptor(17, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 7, 0L);
      this.ConnectWin32Exception = new EventDescriptor(18, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 9, 0L);
      this.ThreadException = new EventDescriptor(19, (byte) 0, (byte) 0, (byte) 2, (byte) 15, 2, 0L);
      this.FileRead_Start = new EventDescriptor(20, (byte) 0, (byte) 0, (byte) 4, (byte) 1, 10, 0L);
      this.FileRead_Stop = new EventDescriptor(21, (byte) 0, (byte) 0, (byte) 4, (byte) 2, 10, 0L);
      this.WaitAbandoned = new EventDescriptor(22, (byte) 0, (byte) 0, (byte) 2, (byte) 2, 11, 0L);
      this.MutexTimeout = new EventDescriptor(23, (byte) 0, (byte) 0, (byte) 2, (byte) 2, 11, 0L);
      this.ConnectNotifyException = new EventDescriptor(24, (byte) 0, (byte) 0, (byte) 3, (byte) 10, 2, 0L);
      this.DisconnectNotifyException = new EventDescriptor(25, (byte) 0, (byte) 0, (byte) 3, (byte) 12, 2, 0L);
      this.InitNotifyException = new EventDescriptor(26, (byte) 0, (byte) 0, (byte) 3, (byte) 10, 2, 0L);
      this.MassStorageIOException = new EventDescriptor(27, (byte) 0, (byte) 0, (byte) 2, (byte) 13, 12, 0L);
      this.MassStorageWin32Exception = new EventDescriptor(28, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 12, 0L);
      this.StreamClearStart = new EventDescriptor(29, (byte) 0, (byte) 0, (byte) 4, (byte) 1, 13, 0L);
      this.StreamClearStop = new EventDescriptor(30, (byte) 0, (byte) 0, (byte) 4, (byte) 2, 13, 0L);
      this.StreamClearPushWin32Exception = new EventDescriptor(31, (byte) 0, (byte) 0, (byte) 4, (byte) 14, 13, 0L);
      this.StreamClearPullWin32Exception = new EventDescriptor(32, (byte) 0, (byte) 0, (byte) 4, (byte) 14, 13, 0L);
      this.StreamClearIOException = new EventDescriptor(33, (byte) 0, (byte) 0, (byte) 4, (byte) 13, 13, 0L);
      this.ClearIdIOException = new EventDescriptor(34, (byte) 0, (byte) 0, (byte) 2, (byte) 13, 14, 0L);
      this.ClearIdWin32Exception = new EventDescriptor(35, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 14, 0L);
      this.WimSuccess = new EventDescriptor(36, (byte) 0, (byte) 0, (byte) 4, (byte) 16, 15, 0L);
      this.WimError = new EventDescriptor(37, (byte) 0, (byte) 0, (byte) 2, (byte) 16, 15, 0L);
      this.WimIOException = new EventDescriptor(38, (byte) 0, (byte) 0, (byte) 2, (byte) 13, 15, 0L);
      this.WimWin32Exception = new EventDescriptor(39, (byte) 0, (byte) 0, (byte) 2, (byte) 14, 15, 0L);
      this.WimTransferStart = new EventDescriptor(40, (byte) 0, (byte) 0, (byte) 4, (byte) 1, 16, 0L);
      this.WimTransferStop = new EventDescriptor(41, (byte) 0, (byte) 0, (byte) 4, (byte) 2, 16, 0L);
      this.WimPacketStart = new EventDescriptor(42, (byte) 0, (byte) 0, (byte) 4, (byte) 1, 17, 0L);
      this.WimPacketStop = new EventDescriptor(43, (byte) 0, (byte) 0, (byte) 4, (byte) 2, 17, 0L);
      this.WimGetStatus = new EventDescriptor(44, (byte) 0, (byte) 0, (byte) 4, (byte) 1, 15, 0L);
    }

    public bool EventWriteFlash_Start(
      Guid DeviceId,
      string DeviceFriendlyName,
      string AssemblyFileVersion)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEventWithString(ref this.Flash_Start, DeviceId, DeviceFriendlyName, AssemblyFileVersion);
    }

    public bool EventWriteFlash_Stop(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Flash_Stop, DeviceId, DeviceFriendlyName);

    public bool EventWriteDevice_Attach(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Device_Attach, DeviceId, DeviceFriendlyName);

    public bool EventWriteDevice_Detach(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Device_Detach, DeviceId, DeviceFriendlyName);

    public bool EventWriteDevice_Remove(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Device_Remove, DeviceId, DeviceFriendlyName);

    public bool EventWriteFlash_Error(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Flash_Error, DeviceId, DeviceFriendlyName);

    public bool EventWriteFlash_Timeout(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Flash_Timeout, DeviceId, DeviceFriendlyName);

    public bool EventWriteTransferException(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.TransferException, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteReconnectIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.ReconnectIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteReconnectWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ReconnectWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteReadBootmeIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.ReadBootmeIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteReadBootmeWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ReadBootmeWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteSkipIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.SkipIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteSkipWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.SkipWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteWriteSkipFailed(Guid DeviceId, string DeviceFriendlyName, int ErrorCode) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WriteSkipFailed, DeviceId, DeviceFriendlyName, ErrorCode);

    public bool EventWriteUSBResetWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.USBResetWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteRebootIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.RebootIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteRebootWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.RebootWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteConnectWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ConnectWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteThreadException(string String) => this.m_provider.WriteEvent(ref this.ThreadException, String);

    public bool EventWriteFileRead_Start(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.FileRead_Start, DeviceId, DeviceFriendlyName);

    public bool EventWriteFileRead_Stop(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.FileRead_Stop, DeviceId, DeviceFriendlyName);

    public bool EventWriteWaitAbandoned(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WaitAbandoned, DeviceId, DeviceFriendlyName);

    public bool EventWriteMutexTimeout(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.MutexTimeout, DeviceId, DeviceFriendlyName);

    public bool EventWriteConnectNotifyException(string DevicePath, string Exception) => !this.m_provider.IsEnabled() || this.m_provider.TemplateNotifyException(ref this.ConnectNotifyException, DevicePath, Exception);

    public bool EventWriteDisconnectNotifyException(string DevicePath, string Exception) => !this.m_provider.IsEnabled() || this.m_provider.TemplateNotifyException(ref this.DisconnectNotifyException, DevicePath, Exception);

    public bool EventWriteInitNotifyException(string DevicePath, string Exception) => !this.m_provider.IsEnabled() || this.m_provider.TemplateNotifyException(ref this.InitNotifyException, DevicePath, Exception);

    public bool EventWriteMassStorageIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.MassStorageIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteMassStorageWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.MassStorageWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteStreamClearStart(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.StreamClearStart, DeviceId, DeviceFriendlyName);

    public bool EventWriteStreamClearStop(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.StreamClearStop, DeviceId, DeviceFriendlyName);

    public bool EventWriteStreamClearPushWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.StreamClearPushWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteStreamClearPullWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.StreamClearPullWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteStreamClearIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.StreamClearIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteClearIdIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.ClearIdIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteClearIdWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ClearIdWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteWimSuccess(Guid DeviceId, string DeviceFriendlyName, int ErrorCode) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimSuccess, DeviceId, DeviceFriendlyName, ErrorCode);

    public bool EventWriteWimError(Guid DeviceId, string DeviceFriendlyName, int ErrorCode) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimError, DeviceId, DeviceFriendlyName, ErrorCode);

    public bool EventWriteWimIOException(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimIOException, DeviceId, DeviceFriendlyName);

    public bool EventWriteWimWin32Exception(
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
    }

    public bool EventWriteWimTransferStart(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimTransferStart, DeviceId, DeviceFriendlyName);

    public bool EventWriteWimTransferStop(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimTransferStop, DeviceId, DeviceFriendlyName);

    public bool EventWriteWimPacketStart(
      Guid DeviceId,
      string DeviceFriendlyName,
      int TransferSize)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEventWithSize(ref this.WimPacketStart, DeviceId, DeviceFriendlyName, TransferSize);
    }

    public bool EventWriteWimPacketStop(Guid DeviceId, string DeviceFriendlyName, int ErrorCode) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimPacketStop, DeviceId, DeviceFriendlyName, ErrorCode);

    public bool EventWriteWimGetStatus(Guid DeviceId, string DeviceFriendlyName) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimGetStatus, DeviceId, DeviceFriendlyName);
  }
}
