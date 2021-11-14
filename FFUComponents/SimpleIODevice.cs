// Decompiled with JetBrains decompiler
// Type: FFUComponents.SimpleIODevice
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using FFUComponents.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFUComponents
{
  internal class SimpleIODevice : IFFUDeviceInternal, IFFUDevice, IDisposable
  {
    private const int SimpleIOTransferSize = 16376;
    private const int maxResets = 3;
    private volatile bool fConnected;
    private volatile bool fOperationStarted;
    private DTSFUsbStream usbStream;
    private MemoryStream memStm;
    private AutoResetEvent connectEvent;
    private PacketConstructor packets;
    private FlashingHost hostLogger;
    private FlashingDeviceLogger deviceLogger;
    private long curPosition;
    private Mutex syncMutex;
    private string usbDevicePath;
    private object pathSync;
    private string errInfo;
    private int resetCount;
    private int diskTransferSize;
    private uint diskBlockSize;
    private ulong diskLastBlock;
    private long lastProgress;
    private bool forceClearOnReconnect;
    private Guid serialNumber = Guid.Empty;
    private bool serialNumberChecked;

    public string DeviceFriendlyName { get; private set; }

    public Guid DeviceUniqueID { get; private set; }

    public Guid SerialNumber
    {
      get
      {
        if (!this.serialNumberChecked)
        {
          this.serialNumberChecked = true;
          this.serialNumber = this.GetSerialNumberFromDevice();
        }
        return this.serialNumber;
      }
    }

    public string UsbDevicePath
    {
      get => this.usbDevicePath;
      private set
      {
        lock (this.pathSync)
        {
          if (this.syncMutex != null)
          {
            this.syncMutex.Close();
            this.syncMutex = (Mutex) null;
          }
          this.syncMutex = new Mutex(false, "Global\\FFU_Mutex_" + this.GetPnPIdFromDevicePath(value).Replace('\\', '_'));
          this.usbDevicePath = value;
        }
      }
    }

    public event EventHandler<ProgressEventArgs> ProgressEvent;

    private bool AcquirePathMutex()
    {
      TimeSpan remaining = new TimeoutHelper(TimeSpan.FromMinutes(2.0)).Remaining;
      if (remaining <= TimeSpan.Zero)
      {
        this.hostLogger.EventWriteMutexTimeout(this.DeviceUniqueID, this.DeviceFriendlyName);
        return false;
      }
      try
      {
        if (this.syncMutex.WaitOne(remaining, false))
          return true;
        this.hostLogger.EventWriteMutexTimeout(this.DeviceUniqueID, this.DeviceFriendlyName);
        return false;
      }
      catch (AbandonedMutexException ex)
      {
        this.hostLogger.EventWriteWaitAbandoned(this.DeviceUniqueID, this.DeviceFriendlyName);
        return true;
      }
    }

    private void ReleasePathMutex() => this.syncMutex.ReleaseMutex();

    private void InitFlashingStream()
    {
      bool flag = false;
      lock (this.pathSync)
      {
        if (!this.AcquirePathMutex())
          throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format("Failed to acquire the device's flashing mutex."));
        try
        {
          if (this.usbStream != null)
            this.usbStream.Dispose();
          this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMinutes(1.0));
          this.usbStream.WriteByte((byte) 2);
        }
        catch (IOException ex)
        {
          flag = true;
        }
        catch (Win32Exception ex)
        {
          flag = true;
          if (ex.NativeErrorCode == 31)
            this.forceClearOnReconnect = false;
        }
        finally
        {
          this.ReleasePathMutex();
        }
      }
      if (!flag)
        return;
      this.WaitForReconnect();
    }

    private Stream GetBufferedFileStream(string path) => (Stream) new BufferedStream((Stream) new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), 5242880);

    private Stream GetStringStream(string src)
    {
      MemoryStream memoryStream = new MemoryStream();
      new BinaryWriter((Stream) memoryStream, Encoding.BigEndianUnicode).Write(src);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      return (Stream) memoryStream;
    }

    private void SendPacket(byte[] packet)
    {
      bool flag = false;
      while (!flag)
      {
        try
        {
          for (int offset = 0; offset < packet.Length; offset += 16376)
            this.usbStream.Write(packet, offset, Math.Min(16376, packet.Length - offset));
          flag = this.WaitForAck();
        }
        catch (Win32Exception ex)
        {
          this.hostLogger.EventWriteTransferException(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
          long position = this.packets.Position;
          this.WaitForReconnect();
          if (position != this.packets.Position)
            break;
        }
      }
    }

    private bool WaitForAck()
    {
      while (true)
      {
        byte[] numArray = new byte[16376];
        this.usbStream.Read(numArray, 0, numArray.Length);
        switch (numArray[0])
        {
          case 3:
            goto label_1;
          case 5:
            this.deviceLogger.LogDeviceEvent(numArray, this.DeviceUniqueID, this.DeviceFriendlyName, out this.errInfo);
            continue;
          case 6:
            goto label_3;
          default:
            goto label_4;
        }
      }
label_1:
      return true;
label_3:
      this.usbStream.Dispose();
      this.usbStream = (DTSFUsbStream) null;
      this.fConnected = false;
      this.hostLogger.EventWriteFlash_Error(this.DeviceUniqueID, this.DeviceFriendlyName);
      throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format("Failed to flash with device error {0}.", (object) this.errInfo));
label_4:
      return false;
    }

    private bool WaitForEndResponse() => this.WaitForAck();

    private bool WriteSkip(DTSFUsbStream skipStream)
    {
      skipStream.WriteByte((byte) 7);
      int ErrorCode = skipStream.ReadByte();
      if (ErrorCode == 3)
        return true;
      this.hostLogger.EventWriteWriteSkipFailed(this.DeviceUniqueID, this.DeviceFriendlyName, ErrorCode);
      return false;
    }

    private void WaitForReconnect()
    {
      this.hostLogger.EventWriteDevice_Detach(this.DeviceUniqueID, this.DeviceFriendlyName);
      if (!this.DoWaitForDevice())
      {
        this.hostLogger.EventWriteFlash_Timeout(this.DeviceUniqueID, this.DeviceFriendlyName);
        throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, "Unable to reconnect to device due to timeout.");
      }
      if (0L == this.curPosition && this.resetCount < 3)
      {
        this.packets.Reset();
        ++this.resetCount;
      }
      if ((ulong) (this.packets.Position - this.curPosition) > (ulong) this.packets.PacketDataLength)
        throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format("Resumed flashing a device from a different point than when disconnect occurred. Old position: 0x{0:X}, new position: 0x{1:X}.", (object) this.packets.Position, (object) this.curPosition));
      this.usbStream.WriteByte((byte) 2);
      this.hostLogger.EventWriteDevice_Attach(this.DeviceUniqueID, this.DeviceFriendlyName);
    }

    private bool DoWaitForDevice()
    {
      bool flag = false;
      if (this.usbStream != null)
      {
        this.usbStream.Dispose();
        this.usbStream = (DTSFUsbStream) null;
      }
      this.connectEvent.WaitOne(30000, false);
      lock (this.pathSync)
      {
        if (!this.AcquirePathMutex())
          throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format("Failed to acquire the device's flashing mutex."));
        try
        {
          bool clearOnReconnect = this.forceClearOnReconnect;
          this.forceClearOnReconnect = true;
          if (clearOnReconnect)
          {
            using (DTSFUsbStream clearStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMilliseconds(100.0)))
              this.ClearJunkDataFromStream(clearStream);
          }
          this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMinutes(1.0));
          this.ReadBootmeFromStream(this.usbStream);
          flag = true;
        }
        catch (IOException ex)
        {
          this.hostLogger.EventWriteReconnectIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
        }
        catch (Win32Exception ex)
        {
          this.hostLogger.EventWriteReconnectWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
        }
        finally
        {
          this.ReleasePathMutex();
        }
      }
      return flag;
    }

    private void ClearJunkDataFromStream(DTSFUsbStream clearStream)
    {
      this.hostLogger.EventWriteStreamClearStart(this.DeviceUniqueID, this.DeviceFriendlyName);
      try
      {
        clearStream.PipeReset();
        for (int index1 = 0; index1 < 3; ++index1)
        {
          byte[] buffer = new byte[16376];
          for (int index2 = 0; index2 < 17; ++index2)
          {
            try
            {
              clearStream.Write(buffer, 0, buffer.Length);
            }
            catch (Win32Exception ex)
            {
              this.hostLogger.EventWriteStreamClearPushWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.ErrorCode);
            }
          }
          for (int index2 = 0; index2 < 5; ++index2)
          {
            try
            {
              clearStream.Read(buffer, 0, buffer.Length);
            }
            catch (Win32Exception ex)
            {
              this.hostLogger.EventWriteStreamClearPullWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.ErrorCode);
            }
          }
        }
        clearStream.PipeReset();
      }
      catch (IOException ex)
      {
        this.hostLogger.EventWriteStreamClearIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
        this.connectEvent.WaitOne(5000, false);
      }
      Thread.Sleep(TimeSpan.FromSeconds(1.0));
      this.hostLogger.EventWriteStreamClearStop(this.DeviceUniqueID, this.DeviceFriendlyName);
    }

    private string GetPnPIdFromDevicePath() => this.GetPnPIdFromDevicePath(this.UsbDevicePath);

    private string GetPnPIdFromDevicePath(string path)
    {
      string str = path.Replace('#', '\\').Substring(4);
      return str.Remove(str.IndexOf('\\', 22));
    }

    private void TransferPackets()
    {
      while (this.packets.RemainingData > 0L)
      {
        this.hostLogger.EventWriteFileRead_Start(this.DeviceUniqueID, this.DeviceFriendlyName);
        byte[] nextPacket = this.packets.GetNextPacket();
        this.hostLogger.EventWriteFileRead_Stop(this.DeviceUniqueID, this.DeviceFriendlyName);
        this.SendPacket(nextPacket);
        if (this.ProgressEvent != null && (this.packets.Position - this.lastProgress > 1048576L || this.packets.Position == this.packets.Length))
        {
          this.lastProgress = this.packets.Position;
          ProgressEventArgs args = new ProgressEventArgs(this.packets.Position, this.packets.Length);
          Task.Factory.StartNew((Action) (() => this.ProgressEvent((object) this, args)));
        }
      }
      if (this.packets.Length % this.packets.PacketDataLength != 0L)
        return;
      this.SendPacket(this.packets.GetZeroLengthPacket());
    }

    public void FlashFFUFile(string ffuFilePath)
    {
      if (this.curPosition != 0L)
        throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, "Attempting to flash a device which has already received data.");
      this.lastProgress = 0L;
      this.fConnected = true;
      this.fOperationStarted = true;
      try
      {
        this.packets.DataStream = this.GetBufferedFileStream(ffuFilePath);
        this.InitFlashingStream();
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyVersionAttribute), false);
        if (customAttributes.Length > 0)
          this.hostLogger.EventWriteFlash_Start(this.DeviceUniqueID, this.DeviceFriendlyName, string.Format("Module version ID: {0}", (object) ((AssemblyVersionAttribute) customAttributes[0]).ToString()));
        this.TransferPackets();
        this.WaitForEndResponse();
        this.hostLogger.EventWriteFlash_Stop(this.DeviceUniqueID, this.DeviceFriendlyName);
      }
      finally
      {
        this.fConnected = false;
        FFUManager.DisconnectDevice(this.DeviceUniqueID);
      }
    }

    public void FlashDataFile(string path)
    {
      string fileName = Path.GetFileName(path);
      this.InitFlashingStream();
      this.usbStream.WriteByte((byte) 9);
      this.packets.DataStream = this.GetStringStream(fileName);
      this.TransferPackets();
      this.WaitForEndResponse();
      this.packets.DataStream = this.GetBufferedFileStream(path);
      this.TransferPackets();
      this.WaitForEndResponse();
    }

    private bool HasWimHeader(Stream wimStream)
    {
      byte[] numArray = new byte[8]
      {
        (byte) 77,
        (byte) 83,
        (byte) 87,
        (byte) 73,
        (byte) 77,
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      byte[] buffer = new byte[numArray.Length];
      long position = wimStream.Position;
      wimStream.Read(buffer, 0, buffer.Length);
      wimStream.Position = position;
      return ((IEnumerable<byte>) numArray).SequenceEqual<byte>((IEnumerable<byte>) buffer);
    }

    private void WriteWim(Stream sdiStream, Stream wimStream)
    {
      bool flag = this.HasWimHeader(wimStream);
      byte[] buffer1 = new byte[12];
      uint num1 = 0;
      if (flag)
        num1 = (uint) sdiStream.Length;
      BitConverter.GetBytes(num1).CopyTo((Array) buffer1, 0);
      BitConverter.GetBytes((uint) wimStream.Length).CopyTo((Array) buffer1, 4);
      BitConverter.GetBytes(16376).CopyTo((Array) buffer1, 8);
      byte[] buffer2 = new byte[16376];
      Stream[] streamArray;
      if (flag)
        streamArray = new Stream[2]{ sdiStream, wimStream };
      else
        streamArray = new Stream[1]{ wimStream };
      this.usbStream.WriteByte((byte) 16);
      this.usbStream.Write(buffer1, 0, buffer1.Length);
      foreach (Stream stream in streamArray)
      {
        this.hostLogger.EventWriteWimTransferStart(this.DeviceUniqueID, this.DeviceFriendlyName);
        while (stream.Position < stream.Length)
        {
          int num2 = stream.Read(buffer2, 0, 16376);
          this.hostLogger.EventWriteWimPacketStart(this.DeviceUniqueID, this.DeviceFriendlyName, num2);
          this.usbStream.Write(buffer2, 0, num2);
          this.hostLogger.EventWriteWimPacketStop(this.DeviceUniqueID, this.DeviceFriendlyName, 0);
        }
        this.hostLogger.EventWriteWimTransferStop(this.DeviceUniqueID, this.DeviceFriendlyName);
      }
    }

    private bool ReadStatus()
    {
      this.hostLogger.EventWriteWimGetStatus(this.DeviceUniqueID, this.DeviceFriendlyName);
      byte[] buffer = new byte[4];
      this.usbStream.Read(buffer, 0, buffer.Length);
      int int32 = BitConverter.ToInt32(buffer, 0);
      bool flag = int32 >= 0;
      if (flag)
      {
        this.hostLogger.EventWriteWimSuccess(this.DeviceUniqueID, this.DeviceFriendlyName, int32);
        return flag;
      }
      this.hostLogger.EventWriteWimError(this.DeviceUniqueID, this.DeviceFriendlyName, int32);
      throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format("Device returned WIM boot failure status code 0x{0:x}.", (object) int32));
    }

    public bool WriteWim(string wimPath)
    {
      bool flag = false;
      lock (this.pathSync)
      {
        if (this.fConnected || !this.AcquirePathMutex())
          return false;
        this.fOperationStarted = true;
        try
        {
          using (Stream bufferedFileStream = this.GetBufferedFileStream(wimPath))
          {
            using (Stream sdiStream = (Stream) new MemoryStream(Resources.bootsdi))
            {
              using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
              {
                this.usbStream.SetShortPacketTerminate();
                try
                {
                  this.WriteWim(sdiStream, bufferedFileStream);
                }
                catch (Win32Exception ex)
                {
                  this.hostLogger.EventWriteWimWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
                }
                this.usbStream.SetTransferTimeout(TimeSpan.FromSeconds(15.0));
                flag = this.ReadStatus();
              }
            }
          }
        }
        catch (IOException ex)
        {
          this.hostLogger.EventWriteWimIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
        }
        catch (Win32Exception ex)
        {
          this.hostLogger.EventWriteWimWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
        }
        finally
        {
          this.ReleasePathMutex();
        }
      }
      return flag;
    }

    private void ReadDiskInfo(out int transferSize, out uint blockSize, out ulong lastBlock)
    {
      this.usbStream.WriteByte((byte) 12);
      byte[] buffer = new byte[16];
      this.usbStream.Read(buffer, 0, buffer.Length);
      int startIndex1 = 0;
      transferSize = BitConverter.ToInt32(buffer, startIndex1);
      int startIndex2 = startIndex1 + 4;
      blockSize = BitConverter.ToUInt32(buffer, startIndex2);
      int startIndex3 = startIndex2 + 4;
      lastBlock = BitConverter.ToUInt64(buffer, startIndex3);
      int num = startIndex3 + 8;
    }

    public bool GetDiskInfo(out uint blockSize, out ulong lastBlock)
    {
      bool flag = false;
      blockSize = 0U;
      lastBlock = 0UL;
      lock (this.pathSync)
      {
        if (!this.fConnected)
        {
          if (this.AcquirePathMutex())
          {
            try
            {
              using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
              {
                this.ReadDiskInfo(out this.diskTransferSize, out this.diskBlockSize, out this.diskLastBlock);
                flag = true;
                goto label_15;
              }
            }
            catch (IOException ex)
            {
              goto label_15;
            }
            catch (Win32Exception ex)
            {
              goto label_15;
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        return false;
      }
label_15:
      blockSize = this.diskBlockSize;
      lastBlock = this.diskLastBlock;
      return flag;
    }

    private void ReadDataToBuffer(ulong diskOffset, byte[] buffer, int offset, int count)
    {
      ulong num = (ulong) count;
      this.usbStream.WriteByte((byte) 13);
      byte[] buffer1 = new byte[16];
      BitConverter.GetBytes(diskOffset).CopyTo((Array) buffer1, 0);
      BitConverter.GetBytes(num).CopyTo((Array) buffer1, 8);
      this.usbStream.Write(buffer1, 0, buffer1.Length);
      int offset1 = offset;
      int count1;
      for (int index = offset + count; offset1 < index; offset1 += count1)
      {
        count1 = this.diskTransferSize;
        if (count1 > index - offset1)
          count1 = index - offset1;
        this.usbStream.Read(buffer, offset1, count1);
        if (count1 % 512 == 0)
          this.usbStream.ReadByte();
      }
    }

    private void WriteDataFromBuffer(ulong diskOffset, byte[] buffer, int offset, int count)
    {
      ulong num = (ulong) count;
      this.usbStream.WriteByte((byte) 14);
      byte[] buffer1 = new byte[16];
      BitConverter.GetBytes(diskOffset).CopyTo((Array) buffer1, 0);
      BitConverter.GetBytes(num).CopyTo((Array) buffer1, 8);
      this.usbStream.Write(buffer1, 0, buffer1.Length);
      int offset1 = offset;
      int count1;
      for (int index = offset + count; offset1 < index; offset1 += count1)
      {
        count1 = this.diskTransferSize;
        if (count1 > index - offset1)
          count1 = index - offset1;
        this.usbStream.Write(buffer, offset1, count1);
        if (count1 % 512 == 0)
        {
          byte[] buffer2 = new byte[0];
          this.usbStream.Write(buffer2, 0, buffer2.Length);
        }
      }
      byte[] buffer3 = new byte[8];
      this.usbStream.Read(buffer3, 0, buffer3.Length);
      if ((long) count != (long) BitConverter.ToUInt64(buffer3, 0))
        throw new FFUDeviceDiskWriteException((IFFUDevice) this, "Unable to complete write.", (Exception) null);
    }

    public void ReadDisk(ulong diskOffset, byte[] buffer, int offset, int count)
    {
      lock (this.pathSync)
      {
        if (this.diskTransferSize <= 0 || this.fConnected || !this.AcquirePathMutex())
          throw new FFUDeviceNotReadyException((IFFUDevice) this);
        ulong num = (this.diskLastBlock + 1UL) * (ulong) this.diskBlockSize;
        if (count > 0 && diskOffset < num)
        {
          if ((long) num - (long) diskOffset >= (long) count)
          {
            try
            {
              using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
              {
                this.ReadDataToBuffer(diskOffset, buffer, offset, count);
                return;
              }
            }
            catch (IOException ex)
            {
              throw new FFUDeviceNotReadyException((IFFUDevice) this);
            }
            catch (Win32Exception ex)
            {
              throw new FFUDeviceDiskReadException((IFFUDevice) this, "An error occurred during a USB transfer.", (Exception) ex);
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        throw new FFUDeviceDiskReadException((IFFUDevice) this, "Unable to read requested region", (Exception) null);
      }
    }

    public void WriteDisk(ulong diskOffset, byte[] buffer, int offset, int count)
    {
      lock (this.pathSync)
      {
        if (this.diskTransferSize <= 0 || this.fConnected || !this.AcquirePathMutex())
          throw new FFUDeviceNotReadyException((IFFUDevice) this);
        ulong num = (this.diskLastBlock + 1UL) * (ulong) this.diskBlockSize;
        if (count > 0 && diskOffset < num)
        {
          if ((long) num - (long) diskOffset >= (long) count)
          {
            try
            {
              using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
              {
                this.WriteDataFromBuffer(diskOffset, buffer, offset, count);
                return;
              }
            }
            catch (IOException ex)
            {
              throw new FFUDeviceNotReadyException((IFFUDevice) this);
            }
            catch (Win32Exception ex)
            {
              throw new FFUDeviceDiskWriteException((IFFUDevice) this, "An error occurred during a USB transfer.", (Exception) ex);
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        throw new FFUDeviceDiskReadException((IFFUDevice) this, "Unable to read requested region", (Exception) null);
      }
    }

    public bool SkipTransfer()
    {
      bool flag = false;
      lock (this.pathSync)
      {
        if (this.curPosition != 0L || this.fConnected || !this.AcquirePathMutex())
          return false;
        this.fOperationStarted = true;
        try
        {
          using (DTSFUsbStream skipStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
            flag = this.WriteSkip(skipStream);
        }
        catch (IOException ex)
        {
          this.hostLogger.EventWriteSkipIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
        }
        catch (Win32Exception ex)
        {
          this.hostLogger.EventWriteSkipWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
        }
        finally
        {
          this.ReleasePathMutex();
        }
      }
      return flag;
    }

    public bool EndTransfer()
    {
      bool flag = false;
      if (this.curPosition == 0L)
        return true;
      lock (this.pathSync)
      {
        if (!this.fConnected)
        {
          if (this.AcquirePathMutex())
          {
            try
            {
              using (DTSFUsbStream idStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
              {
                idStream.WriteByte((byte) 8);
                byte[] buffer = new byte[16376];
                do
                {
                  idStream.Read(buffer, 0, buffer.Length);
                }
                while (buffer[0] == (byte) 5);
                if (buffer[0] == (byte) 6)
                {
                  this.ReadBootmeFromStream(idStream);
                  if (this.curPosition == 0L)
                  {
                    flag = true;
                    goto label_21;
                  }
                  else
                    goto label_21;
                }
                else
                  goto label_21;
              }
            }
            catch (IOException ex)
            {
              goto label_21;
            }
            catch (Win32Exception ex)
            {
              goto label_21;
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        return false;
      }
label_21:
      return flag;
    }

    public bool Reboot()
    {
      bool flag = false;
      lock (this.pathSync)
      {
        if (!this.fConnected)
        {
          if (this.AcquirePathMutex())
          {
            try
            {
              using (DTSFUsbStream dtsfUsbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
              {
                dtsfUsbStream.WriteByte((byte) 10);
                flag = true;
                goto label_15;
              }
            }
            catch (IOException ex)
            {
              this.hostLogger.EventWriteRebootIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
              goto label_15;
            }
            catch (Win32Exception ex)
            {
              this.hostLogger.EventWriteRebootWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
              goto label_15;
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        return false;
      }
label_15:
      return flag;
    }

    public bool EnterMassStorage()
    {
      bool flag = false;
      lock (this.pathSync)
      {
        if (!this.fConnected)
        {
          if (this.AcquirePathMutex())
          {
            try
            {
              using (DTSFUsbStream dtsfUsbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
              {
                dtsfUsbStream.WriteByte((byte) 11);
                if (dtsfUsbStream.ReadByte() == 3)
                {
                  flag = true;
                  goto label_16;
                }
                else
                  goto label_16;
              }
            }
            catch (IOException ex)
            {
              this.hostLogger.EventWriteMassStorageIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
              goto label_16;
            }
            catch (Win32Exception ex)
            {
              this.hostLogger.EventWriteMassStorageWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
              goto label_16;
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        return false;
      }
label_16:
      return flag;
    }

    public bool ClearIdOverride()
    {
      bool flag = false;
      lock (this.pathSync)
      {
        if (!this.fConnected)
        {
          if (this.AcquirePathMutex())
          {
            try
            {
              using (DTSFUsbStream idStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
              {
                idStream.WriteByte((byte) 15);
                if (idStream.ReadByte() == 3)
                {
                  flag = true;
                  this.ReadBootmeFromStream(idStream);
                  goto label_16;
                }
                else
                  goto label_16;
              }
            }
            catch (IOException ex)
            {
              this.hostLogger.EventWriteClearIdIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
              goto label_16;
            }
            catch (Win32Exception ex)
            {
              this.hostLogger.EventWriteClearIdWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
              goto label_16;
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        return false;
      }
label_16:
      return flag;
    }

    public bool OnConnect(SimpleIODevice device)
    {
      if (device.UsbDevicePath != this.UsbDevicePath)
        this.UsbDevicePath = device.UsbDevicePath;
      if (this.fConnected)
      {
        this.connectEvent.Set();
        return true;
      }
      return this.ReadBootme();
    }

    public bool IsConnected() => this.fConnected || this.ReadBootme();

    public bool NeedsTimer() => !this.fOperationStarted;

    public bool OnDisconnect() => false;

    private void ReadBootmeFromStream(DTSFUsbStream idStream)
    {
      idStream.WriteByte((byte) 1);
      BinaryReader binaryReader = new BinaryReader((Stream) idStream);
      this.curPosition = binaryReader.ReadInt64();
      Guid guid = new Guid(binaryReader.ReadBytes(sizeof (Guid)));
      if (Guid.Empty != guid)
        throw new Win32Exception(1167);
      this.DeviceUniqueID = new Guid(binaryReader.ReadBytes(sizeof (Guid)));
      this.DeviceFriendlyName = binaryReader.ReadString();
    }

    private bool ReadBootme()
    {
      bool flag = false;
      for (int index = 0; index < 3; ++index)
      {
        lock (this.pathSync)
        {
          if (this.syncMutex != null)
          {
            if (this.AcquirePathMutex())
            {
              try
              {
                if (index > 0)
                {
                  using (DTSFUsbStream clearStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMilliseconds(100.0)))
                    this.ClearJunkDataFromStream(clearStream);
                }
                using (DTSFUsbStream idStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(2.0)))
                {
                  this.ReadBootmeFromStream(idStream);
                  flag = true;
                  break;
                }
              }
              catch (IOException ex)
              {
                this.hostLogger.EventWriteReadBootmeIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
                continue;
              }
              catch (Win32Exception ex)
              {
                this.hostLogger.EventWriteReadBootmeWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
                continue;
              }
              finally
              {
                this.ReleasePathMutex();
              }
            }
          }
          return false;
        }
      }
      return flag;
    }

    private Guid GetSerialNumberFromDevice()
    {
      Guid guid = Guid.Empty;
      lock (this.pathSync)
      {
        if (this.syncMutex != null)
        {
          if (this.AcquirePathMutex())
          {
            try
            {
              using (DTSFUsbStream dtsfUsbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
              {
                byte[] numArray = new byte[16];
                dtsfUsbStream.WriteByte((byte) 17);
                dtsfUsbStream.Read(numArray, 0, numArray.Length);
                guid = new Guid(numArray);
                goto label_15;
              }
            }
            catch (IOException ex)
            {
              goto label_15;
            }
            catch (Win32Exception ex)
            {
              goto label_15;
            }
            finally
            {
              this.ReleasePathMutex();
            }
          }
        }
        return guid;
      }
label_15:
      return guid;
    }

    public SimpleIODevice(string devicePath)
    {
      this.fConnected = false;
      this.fOperationStarted = false;
      this.forceClearOnReconnect = true;
      this.usbStream = (DTSFUsbStream) null;
      this.memStm = new MemoryStream();
      this.connectEvent = new AutoResetEvent(false);
      this.pathSync = new object();
      this.UsbDevicePath = devicePath;
      this.hostLogger = FFUManager.HostLogger;
      this.deviceLogger = FFUManager.DeviceLogger;
      this.packets = new PacketConstructor();
      this.DeviceUniqueID = Guid.Empty;
      this.DeviceFriendlyName = "";
      this.resetCount = 0;
      this.diskTransferSize = 0;
      this.diskBlockSize = 0U;
      this.diskLastBlock = 0UL;
    }

    private void Dispose(bool fDisposing)
    {
      if (!fDisposing)
        return;
      FFUManager.DisconnectDevice(this);
      if (this.usbStream != null)
      {
        this.usbStream.Dispose();
        this.usbStream = (DTSFUsbStream) null;
        this.fConnected = false;
      }
      if (this.memStm != null)
      {
        this.memStm.Dispose();
        this.memStm = (MemoryStream) null;
      }
      if (this.syncMutex != null)
      {
        this.syncMutex.Close();
        this.syncMutex = (Mutex) null;
      }
      if (this.packets == null)
        return;
      this.packets.Dispose();
      this.packets = (PacketConstructor) null;
    }

    public void Dispose() => this.Dispose(true);

    private enum SioOpcode : byte
    {
      SioId = 1,
      SioFlash = 2,
      SioAck = 3,
      SioNack = 4,
      SioLog = 5,
      SioErr = 6,
      SioSkip = 7,
      SioReset = 8,
      SioFile = 9,
      SioReboot = 10, // 0x0A
      SioMassStorage = 11, // 0x0B
      SioGetDiskInfo = 12, // 0x0C
      SioReadDisk = 13, // 0x0D
      SioWriteDisk = 14, // 0x0E
      SioClearIdOverride = 15, // 0x0F
      SioWim = 16, // 0x10
      SioSerialNumber = 17, // 0x11
    }
  }
}
