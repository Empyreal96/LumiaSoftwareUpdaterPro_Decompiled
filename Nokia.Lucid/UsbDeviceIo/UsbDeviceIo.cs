// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.UsbDeviceIo.UsbDeviceIo
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Nokia.Lucid.UsbDeviceIo
{
  public class UsbDeviceIo : IDisposable
  {
    private const int DefaultMaxItemCount = 1000;
    private readonly WinUsbIo winUsbIo;
    private readonly BlockingCollection<byte[]> receiveQueue;
    private readonly Task receiverTask;
    private readonly CancellationTokenSource cancelReceiver;

    public UsbDeviceIo(string devicePath)
      : this(devicePath, 1000)
    {
    }

    public UsbDeviceIo(string devicePath, int maxItemCount)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.UsbDeviceIo(string devicePath)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        try
        {
          this.winUsbIo = new WinUsbIo(devicePath);
          this.winUsbIo.Open();
          this.winUsbIo.SetPipePolicy(WinUsbIo.PIPE_TYPE.PipeTypeBulkOut, WinUsbIo.POLICY_TYPE.SHORT_PACKET_TERMINATE, 1U);
          this.MaxItemCount = maxItemCount;
          this.receiveQueue = new BlockingCollection<byte[]>(this.MaxItemCount);
          this.cancelReceiver = new CancellationTokenSource();
          CancellationToken token = this.cancelReceiver.Token;
          this.receiverTask = Task.Factory.StartNew((Action) (() => this.Receiver(token)), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        catch (Exception ex)
        {
          RobustTrace.Trace<Exception>(new Action<Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), ex);
          throw;
        }
      }
    }

    public event EventHandler<OnReceivedEventArgs> OnReceived;

    public event EventHandler<OnSendingEventArgs> OnSending;

    public int MaxItemCount { get; private set; }

    public void Dispose()
    {
      using (EntryExitLogger.Log("UsbDeviceIo.Dispose()", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
    }

    public void Send(byte[] dataToSend, uint length)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.Send(byte[] dataToSend, uint length)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        try
        {
          this.HandleOnSending(new OnSendingEventArgs(dataToSend));
          int num = (int) this.winUsbIo.Write(dataToSend, length);
          RobustTrace.Trace<byte[]>(new Action<byte[]>(UsbDeviceIoTraceSource.Instance.DeviceIoMessageOut), dataToSend);
        }
        catch (Exception ex)
        {
          RobustTrace.Trace<Exception>(new Action<Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), ex);
          throw;
        }
      }
    }

    public uint Receive(out byte[] receivedData, TimeSpan receiveTimeout)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.Receive(ref byte[] receivedData, TimeSpan receiveTimeout)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        try
        {
          if (!this.receiveQueue.TryTake(out receivedData, receiveTimeout))
            throw new TimeoutException("receive operation timed out");
          return (uint) receivedData.Length;
        }
        catch (Exception ex)
        {
          RobustTrace.Trace<Exception>(new Action<Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), ex);
          throw;
        }
      }
    }

    private static byte[] GetNewBuffer(uint length) => new byte[(IntPtr) length];

    private void Receiver(CancellationToken ct)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.Receiver(CancellationToken ct)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        byte[] data = new byte[new IntPtr(1048576)];
        while (!ct.IsCancellationRequested)
        {
          try
          {
            uint length = this.winUsbIo.Read(data, 1048576U);
            if (length > 0U)
            {
              byte[] newBuffer = Nokia.Lucid.UsbDeviceIo.UsbDeviceIo.GetNewBuffer(length);
              Buffer.BlockCopy((Array) data, 0, (Array) newBuffer, 0, (int) length);
              if (this.OnReceived != null)
                this.HandleOnReceived(new OnReceivedEventArgs(newBuffer));
              else if (!this.receiveQueue.TryAdd(newBuffer, 500))
              {
                RobustTrace.Trace<Exception>(new Action<Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), new Exception("Messages will be lost until there is free space in buffer"));
                continue;
              }
              RobustTrace.Trace<byte[]>(new Action<byte[]>(UsbDeviceIoTraceSource.Instance.DeviceIoMessageIn), newBuffer);
            }
          }
          catch (Win32Exception ex)
          {
            if (ex.NativeErrorCode == 31)
            {
              RobustTrace.Trace<string>(new Action<string>(UsbDeviceIoTraceSource.Instance.DeviceIoInformation), ex.Message);
              break;
            }
            if (ex.NativeErrorCode == 995)
              RobustTrace.Trace<string>(new Action<string>(UsbDeviceIoTraceSource.Instance.DeviceIoInformation), ex.Message);
            else
              RobustTrace.Trace<Win32Exception>(new Action<Win32Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoErrorWin32), ex);
          }
          catch (Exception ex)
          {
            RobustTrace.Trace<Exception>(new Action<Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), ex);
          }
        }
      }
    }

    private void Dispose(bool disposing)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.Dispose(bool disposing)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        try
        {
          if (disposing)
          {
            this.cancelReceiver.Cancel();
            if (!this.receiverTask.Wait(10))
            {
              try
              {
                this.winUsbIo.CancelIo();
              }
              catch (Win32Exception ex)
              {
                RobustTrace.Trace<Win32Exception>(new Action<Win32Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoErrorWin32), ex);
              }
              this.receiverTask.Wait(1000);
            }
            if (this.receiverTask.IsCompleted)
              this.receiverTask.Dispose();
            this.cancelReceiver.Dispose();
            this.receiveQueue.Dispose();
          }
        }
        catch (AggregateException ex)
        {
          foreach (Exception innerException in ex.InnerExceptions)
            Console.WriteLine("msg: " + innerException.Message);
        }
        catch (Exception ex)
        {
          RobustTrace.Trace<Exception>(new Action<Exception>(UsbDeviceIoTraceSource.Instance.DeviceIoError), ex);
        }
        this.winUsbIo.Dispose();
      }
    }

    private void HandleOnReceived(OnReceivedEventArgs e)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.HandleOnReceived(OnReceivedEventArgs e)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        EventHandler<OnReceivedEventArgs> onReceived = this.OnReceived;
        if (onReceived == null)
          return;
        onReceived((object) this, e);
      }
    }

    private void HandleOnSending(OnSendingEventArgs e)
    {
      using (EntryExitLogger.Log("UsbDeviceIo.HandleOnSending(OnSendingEventArgs e)", (TraceSource) UsbDeviceIoTraceSource.Instance))
      {
        EventHandler<OnSendingEventArgs> onSending = this.OnSending;
        if (onSending == null)
          return;
        onSending((object) this, e);
      }
    }
  }
}
