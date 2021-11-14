// Decompiled with JetBrains decompiler
// Type: FFUComponents.UsbEventWatcherForm
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FFUComponents
{
  internal class UsbEventWatcherForm : Form
  {
    private IUsbEventSink eventSink;
    private Guid ifGuid;
    private Guid classGuid;
    private IntPtr notificationHandle;

    public UsbEventWatcherForm(IUsbEventSink sink, Guid devClass, Guid devIf)
    {
      this.eventSink = sink;
      this.ifGuid = devIf;
      this.classGuid = devClass;
      this.notificationHandle = IntPtr.Zero;
      this.InitializeComponent();
    }

    private void DiscoverSimpleIODevices()
    {
      foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(string.Format("SELECT PnPDeviceId FROM Win32_PnPEntity where ClassGuid ='{0:B}'", (object) this.classGuid)).Get())
        this.NotifyConnect(managementBaseObject["PnPDeviceId"] as string);
    }

    private void NotifyConnect(string pnpId)
    {
      try
      {
        if (pnpId == null)
          return;
        this.eventSink.OnDeviceConnect(this.GetUsbDevicePath(pnpId));
      }
      catch (Exception ex)
      {
        FFUManager.HostLogger.EventWriteInitNotifyException(pnpId, ex.ToString());
      }
    }

    private unsafe string GetUsbDevicePath(string pnpId)
    {
      IntPtr classDevs = NativeMethods.SetupDiGetClassDevs(ref this.ifGuid, pnpId, 0, 18);
      if (IntPtr.Zero == classDevs)
        throw new Win32Exception(Marshal.GetLastWin32Error());
      int memberIndex = 0;
      int requiredSize = 0;
      DeviceInterfaceData deviceInterfaceData = new DeviceInterfaceData()
      {
        Size = Marshal.SizeOf(typeof (DeviceInterfaceData))
      };
      if (!NativeMethods.SetupDiEnumDeviceInterfaces(classDevs, 0, ref this.ifGuid, memberIndex, ref deviceInterfaceData))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      if (!NativeMethods.SetupDiGetDeviceInterfaceDetail(classDevs, ref deviceInterfaceData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero))
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (lastWin32Error != 122)
          throw new Win32Exception(lastWin32Error);
      }
      DeviceInterfaceDetailData* deviceInterfaceDetailData = (DeviceInterfaceDetailData*) (void*) Marshal.AllocHGlobal(requiredSize);
      try
      {
        deviceInterfaceDetailData->Size = IntPtr.Size != 4 ? 8 : 6;
        DeviceInformationData deviceInfoData = new DeviceInformationData()
        {
          Size = Marshal.SizeOf(typeof (DeviceInformationData))
        };
        if (!NativeMethods.SetupDiGetDeviceInterfaceDetail(classDevs, ref deviceInterfaceData, deviceInterfaceDetailData, requiredSize, ref requiredSize, ref deviceInfoData))
          throw new Win32Exception(Marshal.GetLastWin32Error());
        return Marshal.PtrToStringAuto(new IntPtr((void*) &deviceInterfaceDetailData->DevicePath));
      }
      finally
      {
        Marshal.FreeHGlobal((IntPtr) (void*) deviceInterfaceDetailData);
      }
    }

    protected override void WndProc(ref Message message)
    {
      base.WndProc(ref message);
      if (message.Msg != 537)
        return;
      this.HandleDeviceChangeMessage(ref message);
    }

    private void HandleDeviceChangeMessage(ref Message message)
    {
      switch (message.WParam.ToInt32())
      {
        case 32768:
          if (Marshal.ReadInt32(message.LParam, 4) != 5)
            break;
          DevBroadcastDeviceInterface structure1 = (DevBroadcastDeviceInterface) Marshal.PtrToStructure(message.LParam, typeof (DevBroadcastDeviceInterface));
          try
          {
            this.eventSink.OnDeviceConnect(structure1.Name);
            break;
          }
          catch (Exception ex)
          {
            FFUManager.HostLogger.EventWriteConnectNotifyException(structure1.Name, ex.ToString());
            break;
          }
        case 32772:
          if (Marshal.ReadInt32(message.LParam, 4) != 5)
            break;
          DevBroadcastDeviceInterface structure2 = (DevBroadcastDeviceInterface) Marshal.PtrToStructure(message.LParam, typeof (DevBroadcastDeviceInterface));
          try
          {
            this.eventSink.OnDeviceDisconnect(structure2.Name);
            break;
          }
          catch (Exception ex)
          {
            FFUManager.HostLogger.EventWriteDisconnectNotifyException(structure2.Name, ex.ToString());
            break;
          }
      }
    }

    private IntPtr RegisterDeviceNotification(Guid ifGuid)
    {
      IntPtr zero = IntPtr.Zero;
      DevBroadcastDeviceInterface broadcastDeviceInterface = new DevBroadcastDeviceInterface()
      {
        Size = Marshal.SizeOf(typeof (DevBroadcastDeviceInterface)),
        DeviceType = 5,
        ClassGuid = ifGuid
      };
      IntPtr num = Marshal.AllocHGlobal(broadcastDeviceInterface.Size);
      try
      {
        Marshal.StructureToPtr((object) broadcastDeviceInterface, num, true);
        return NativeMethods.RegisterDeviceNotification(this.Handle, num, 0);
      }
      finally
      {
        Marshal.FreeHGlobal(num);
      }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      IntPtr hNotification = this.RegisterDeviceNotification(this.ifGuid);
      IntPtr num = Interlocked.CompareExchange(ref this.notificationHandle, hNotification, IntPtr.Zero);
      if (IntPtr.Zero != num)
        NativeMethods.UnregisterDeviceNotification(hNotification);
      else
        this.DiscoverSimpleIODevices();
      base.OnHandleCreated(e);
    }

    protected override void OnClosed(EventArgs e)
    {
      IntPtr hNotification = Interlocked.CompareExchange(ref this.notificationHandle, IntPtr.Zero, this.notificationHandle);
      if (IntPtr.Zero != hNotification)
        NativeMethods.UnregisterDeviceNotification(hNotification);
      base.OnClosed(e);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.ClientSize = new Size(0, 0);
      this.WindowState = FormWindowState.Minimized;
      this.Visible = false;
      this.Name = nameof (UsbEventWatcherForm);
      this.ShowInTaskbar = false;
      this.MaximizeBox = false;
      this.MaximumSize = new Size(1, 1);
      IntPtr num = NativeMethods.SetParent(this.Handle, (IntPtr) -3L);
      if (IntPtr.Zero == num)
        throw new Win32Exception(Marshal.GetLastWin32Error());
      this.ResumeLayout(false);
    }
  }
}
