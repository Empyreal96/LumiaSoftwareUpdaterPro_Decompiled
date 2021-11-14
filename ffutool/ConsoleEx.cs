// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.ConsoleEx
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using FFUComponents;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.ImageTools
{
  internal class ConsoleEx
  {
    private static ConsoleEx instance;
    private static object syncRoot = new object();
    private Dictionary<Guid, Tuple<int, ProgressReporter>> deviceRows;
    private int cursorTop;
    private int lastcursorTop;
    private bool error;
    private readonly int RESEVERED_LINES = 30;

    public static ConsoleEx Instance
    {
      get
      {
        if (ConsoleEx.instance == null)
        {
          lock (ConsoleEx.syncRoot)
          {
            if (ConsoleEx.instance == null)
              ConsoleEx.instance = new ConsoleEx();
          }
        }
        return ConsoleEx.instance;
      }
    }

    public void Initialize(ICollection<IFFUDevice> devices)
    {
      int height = devices.Count * 6 + 100;
      if (Console.BufferHeight < height)
        Console.SetBufferSize(Console.BufferWidth, height);
      this.deviceRows = new Dictionary<Guid, Tuple<int, ProgressReporter>>();
      int num = 0;
      foreach (IFFUDevice device in (IEnumerable<IFFUDevice>) devices)
      {
        Console.WriteLine(Resources.DEVICE_NO, (object) num);
        Console.WriteLine(Resources.NAME, (object) device.DeviceFriendlyName);
        Console.WriteLine(Resources.ID, (object) device.DeviceUniqueID);
        this.deviceRows[device.DeviceUniqueID] = new Tuple<int, ProgressReporter>(num, new ProgressReporter());
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        ++num;
      }
      for (int index = 0; index < this.RESEVERED_LINES; ++index)
        Console.WriteLine();
      this.lastcursorTop = Console.CursorTop - this.RESEVERED_LINES;
      this.cursorTop = this.lastcursorTop - devices.Count * 6;
      foreach (IFFUDevice device in (IEnumerable<IFFUDevice>) devices)
        this.UpdateStatus(device, DeviceStatus.CONNECTED, (object) null);
      this.error = false;
    }

    public void UpdateProgress(ProgressEventArgs progress)
    {
      Tuple<int, ProgressReporter> deviceRow = this.deviceRows[progress.Device.DeviceUniqueID];
      string progressDisplay = deviceRow.Item2.CreateProgressDisplay(progress.Position, progress.Length);
      lock (ConsoleEx.syncRoot)
        this.WriteLine(progressDisplay, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceProgress), true);
    }

    public void UpdateStatus(IFFUDevice device, DeviceStatus status, object data)
    {
      Tuple<int, ProgressReporter> deviceRow = this.deviceRows[device.DeviceUniqueID];
      lock (ConsoleEx.syncRoot)
      {
        switch (status)
        {
          case DeviceStatus.CONNECTED:
            this.WriteLine(Resources.STATUS_CONNECTED, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceStatus), true);
            break;
          case DeviceStatus.FLASHING:
            this.WriteLine(Resources.STATUS_FLASHING, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceStatus), true);
            break;
          case DeviceStatus.BOOTING_WIM:
            this.WriteLine(Resources.STATUS_BOOTING_TO_WIM, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceStatus), true);
            break;
          case DeviceStatus.DONE:
            this.WriteLine(Resources.STATUS_DONE, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceStatus), true);
            break;
          case DeviceStatus.EXCEPTION:
            Exception exception = (Exception) data;
            this.WriteLine(Resources.STATUS_ERROR, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceStatus), true);
            if (!this.error)
            {
              Console.SetCursorPosition(0, this.lastcursorTop);
              Console.WriteLine(Resources.ERRORS);
              this.lastcursorTop = Console.CursorTop;
              this.error = true;
            }
            Console.SetCursorPosition(0, this.lastcursorTop);
            Console.WriteLine(Resources.DEVICE_NO, (object) deviceRow.Item1);
            Console.WriteLine(exception.Message);
            Console.WriteLine();
            this.lastcursorTop = Console.CursorTop;
            break;
          case DeviceStatus.ERROR:
          case DeviceStatus.MESSAGE:
            this.WriteLine(data as string, this.GetDeviceCursorPosition(deviceRow.Item1, DeviceStatusPosition.DeviceStatus), true);
            break;
          default:
            throw new Exception(Resources.ERROR_UNEXPECTED_DEVICESTATUS);
        }
        Console.SetCursorPosition(0, this.lastcursorTop);
      }
    }

    private int GetDeviceCursorPosition(int index, DeviceStatusPosition position) => (int) (this.cursorTop + 6 * index + position);

    private void WriteLine(string text, int row, bool clear)
    {
      if (clear)
      {
        string str = new string(' ', Console.WindowWidth);
        Console.SetCursorPosition(0, row);
        Console.WriteLine(str);
      }
      Console.SetCursorPosition(0, row);
      Console.WriteLine(text);
    }
  }
}
