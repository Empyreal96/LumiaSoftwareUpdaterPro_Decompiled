// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.FFUTool
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using FFUComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.Windows.ImageTools
{
  public static class FFUTool
  {
    private static Regex flashParam = new Regex("[-/]flash$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex uefiFlashParam = new Regex("[-/]uefiflash$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex fastFlashParam = new Regex("[-/]fastflash$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex skipParam = new Regex("[-/]skip$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex listParam = new Regex("[-/]list$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex forceParam = new Regex("[-/]force$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex massParam = new Regex("[-/]massStorage$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex clearIdParam = new Regex("[-/]clearId$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex serParam = new Regex("[-/]serial$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex wimParam = new Regex("[-/]wim$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex setBootModeParam = new Regex("[-/]setBootMode$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static void Main(string[] args)
    {
      if (args.Length > 2 && FFUTool.forceParam.IsMatch(args[2]))
        Console.WriteLine(Resources.FORCE_OPTION_DEPRECATED);
      bool flag = false;
      string path = (string) null;
      string str1 = (string) null;
      uint num1 = 0;
      string str2 = (string) null;
      if (args.Length < 1 || !FFUTool.flashParam.IsMatch(args[0]) && !FFUTool.uefiFlashParam.IsMatch(args[0]) && (!FFUTool.fastFlashParam.IsMatch(args[0]) && !FFUTool.wimParam.IsMatch(args[0])) && (!FFUTool.skipParam.IsMatch(args[0]) && !FFUTool.listParam.IsMatch(args[0]) && (!FFUTool.massParam.IsMatch(args[0]) && !FFUTool.serParam.IsMatch(args[0]))) && (!FFUTool.clearIdParam.IsMatch(args[0]) && !FFUTool.setBootModeParam.IsMatch(args[0])))
        flag = true;
      if (!flag && (FFUTool.flashParam.IsMatch(args[0]) || FFUTool.uefiFlashParam.IsMatch(args[0]) || (FFUTool.fastFlashParam.IsMatch(args[0]) || FFUTool.wimParam.IsMatch(args[0]))))
      {
        if (args.Length <= 1)
        {
          flag = true;
        }
        else
        {
          path = args[1];
          if (!File.Exists(path))
          {
            Console.WriteLine(Resources.ERROR_FILE_NOT_FOUND, (object) path);
            Environment.ExitCode = -1;
            return;
          }
          if (FFUTool.flashParam.IsMatch(args[0]) && args.Length >= 3)
            str1 = args[2];
        }
      }
      if (!flag && FFUTool.setBootModeParam.IsMatch(args[0]))
      {
        if (args.Length <= 1)
        {
          flag = true;
        }
        else
        {
          try
          {
            num1 = Convert.ToUInt32(args[1], (IFormatProvider) CultureInfo.InvariantCulture);
            str2 = args.Length < 3 ? "" : args[2];
          }
          catch (Exception ex)
          {
            flag = true;
          }
        }
      }
      if (flag)
      {
        Console.WriteLine(Resources.USAGE);
        Environment.ExitCode = -1;
      }
      else
      {
        try
        {
          FFUManager.Start();
          ICollection<IFFUDevice> devices = (ICollection<IFFUDevice>) new List<IFFUDevice>();
          FFUManager.GetFlashableDevices(ref devices);
          if (devices.Count == 0)
          {
            Console.WriteLine(Resources.NO_CONNECTED_DEVICES);
            Environment.ExitCode = 0;
          }
          else if (FFUTool.listParam.IsMatch(args[0]))
          {
            Console.WriteLine(Resources.DEVICES_FOUND, (object) devices.Count);
            int num2 = 0;
            foreach (IFFUDevice ffuDevice in (IEnumerable<IFFUDevice>) devices)
            {
              Console.WriteLine(Resources.DEVICE_NO, (object) num2);
              Console.WriteLine(Resources.NAME, (object) ffuDevice.DeviceFriendlyName);
              Console.WriteLine(Resources.ID, (object) ffuDevice.DeviceUniqueID);
              Console.WriteLine();
              ++num2;
            }
            Environment.ExitCode = 0;
          }
          else
          {
            FlashParam[] flashParamArray1 = new FlashParam[devices.Count];
            using (EtwSession session = new EtwSession())
            {
              Console.CancelKeyPress += (ConsoleCancelEventHandler) ((param0, param1) => session.Dispose());
              Console.WriteLine(Resources.LOGGING_TO_ETL, (object) session.EtlPath);
              Console.WriteLine();
              ConsoleEx.Instance.Initialize(devices);
              int index1 = 0;
              try
              {
                foreach (IFFUDevice ffuDevice in (IEnumerable<IFFUDevice>) devices)
                {
                  AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                  if (FFUTool.uefiFlashParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      FfuFilePath = path,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoFlash(param)));
                  }
                  else if (FFUTool.fastFlashParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      FfuFilePath = path,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoFastFlash(param)));
                  }
                  else if (FFUTool.flashParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      FfuFilePath = path,
                      WimPath = str1,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoWimFlash(param)));
                  }
                  else if (FFUTool.skipParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoSkip(param)));
                  }
                  else if (FFUTool.massParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoMassStorage(param)));
                  }
                  else if (FFUTool.clearIdParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoClearId(param)));
                  }
                  else if (FFUTool.serParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoSerialNumber(param)));
                  }
                  else if (FFUTool.wimParam.IsMatch(args[0]))
                  {
                    FlashParam param = flashParamArray1[index1] = new FlashParam()
                    {
                      Device = ffuDevice,
                      FfuFilePath = path,
                      WaitHandle = autoResetEvent
                    };
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoWim(param)));
                  }
                  else if (FFUTool.setBootModeParam.IsMatch(args[0]))
                  {
                    FlashParam[] flashParamArray2 = flashParamArray1;
                    int index2 = index1;
                    SetBootModeParam setBootModeParam1 = new SetBootModeParam();
                    setBootModeParam1.Device = ffuDevice;
                    setBootModeParam1.BootMode = num1;
                    setBootModeParam1.ProfileName = str2;
                    setBootModeParam1.WaitHandle = autoResetEvent;
                    SetBootModeParam setBootModeParam2;
                    FlashParam flashParam = (FlashParam) (setBootModeParam2 = setBootModeParam1);
                    flashParamArray2[index2] = (FlashParam) setBootModeParam2;
                    FlashParam param = flashParam;
                    ThreadPool.QueueUserWorkItem((WaitCallback) (s => FFUTool.DoSetBootMode(param as SetBootModeParam)));
                  }
                  ++index1;
                }
                WaitHandle.WaitAll(((IEnumerable<FlashParam>) flashParamArray1).Select<FlashParam, WaitHandle>((Func<FlashParam, WaitHandle>) (p => (WaitHandle) p.WaitHandle)).ToArray<WaitHandle>());
                if (!((IEnumerable<FlashParam>) flashParamArray1).Any<FlashParam>((Func<FlashParam, bool>) (p => p.Result == -1)))
                  return;
                Console.WriteLine(Resources.ERROR_AT_LEAST_ONE_DEVICE_FAILED);
                Environment.ExitCode = -1;
              }
              finally
              {
                Console.CancelKeyPress -= (ConsoleCancelEventHandler) ((param0, param1) => session.Dispose());
              }
            }
          }
        }
        catch (FFUException ex)
        {
          Console.WriteLine();
          Console.WriteLine(Resources.ERROR_FFU + ex.Message);
          Environment.ExitCode = -1;
        }
        catch (TimeoutException ex)
        {
          Console.WriteLine();
          Console.WriteLine(Resources.ERROR_TIMED_OUT + ex.Message);
          Environment.ExitCode = -1;
        }
        finally
        {
          FFUManager.Stop();
        }
      }
    }

    private static void DoSerialNumber(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        byte[] byteArray = param.Device.SerialNumber.ToByteArray();
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.SERIAL_NO_FORMAT, (object) Resources.SERIAL_NO, (object) BitConverter.ToString(byteArray).Replace("-", string.Empty)));
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoSkip(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) Resources.STATUS_SKIPPING);
        if (param.Device.SkipTransfer())
        {
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) Resources.STATUS_SKIPPED);
        }
        else
        {
          param.Result = -1;
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, (object) Resources.ERROR_SKIP_TRANSFER);
        }
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoMassStorage(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        if (param.Device.EnterMassStorage())
        {
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) Resources.RESET_MASS_STORAGE_MODE);
        }
        else
        {
          param.Result = -1;
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, (object) Resources.ERROR_RESET_MASS_STORAGE_MODE);
        }
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoClearId(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        Console.WriteLine(Resources.DEVICE_ID, (object) param.Device.DeviceFriendlyName);
        if (param.Device.ClearIdOverride())
        {
          param.Result = 0;
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.REMOVE_PLATFORM_ID, (object) param.Device.DeviceFriendlyName));
        }
        else
        {
          param.Result = -1;
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, (object) Resources.ERROR_NO_PLATFORM_ID);
        }
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoWim(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.BOOTING_WIM, (object) Path.GetFileName(param.FfuFilePath)));
        Stopwatch stopwatch = Stopwatch.StartNew();
        param.Device.EndTransfer();
        bool flag = param.Device.WriteWim(param.FfuFilePath);
        stopwatch.Stop();
        if (flag)
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.WIM_TRANSFER_RATE, (object) stopwatch.Elapsed.TotalSeconds));
        else
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) Resources.ERROR_BOOT_WIM);
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void PrepareFlash(IFFUDevice device) => device.EndTransfer();

    private static void TransferWimIfPresent(
      ref IFFUDevice device,
      string ffuFilePath,
      string wimFilePath)
    {
      IFFUDevice wimDevice = (IFFUDevice) null;
      Guid id = device.DeviceUniqueID;
      ManualResetEvent deviceConnected = new ManualResetEvent(false);
      EventHandler<ConnectEventArgs> eventHandler = (EventHandler<ConnectEventArgs>) ((sender, e) =>
      {
        if (!(e.Device.DeviceUniqueID == id))
          return;
        wimDevice = e.Device;
        deviceConnected.Set();
      });
      if (string.IsNullOrEmpty(wimFilePath))
        wimFilePath = Path.Combine(Path.GetDirectoryName(ffuFilePath), "flashwim.wim");
      if (!File.Exists(wimFilePath))
        return;
      FFUManager.DeviceConnectEvent += eventHandler;
      ConsoleEx.Instance.UpdateStatus(device, DeviceStatus.BOOTING_WIM, (object) wimFilePath);
      bool flag1 = false;
      try
      {
        flag1 = device.WriteWim(wimFilePath);
      }
      catch (FFUException ex)
      {
      }
      if (!flag1)
        return;
      bool flag2 = deviceConnected.WaitOne(TimeSpan.FromSeconds(30.0));
      FFUManager.DeviceConnectEvent -= eventHandler;
      if (!flag2)
        throw new FFUException(device.DeviceFriendlyName, device.DeviceUniqueID, Resources.ERROR_WIM_BOOT);
      device = wimDevice;
    }

    private static void FlashFile(IFFUDevice device, string ffuFilePath, bool optimize)
    {
      ConsoleEx.Instance.UpdateStatus(device, DeviceStatus.FLASHING, (object) null);
      device.ProgressEvent += new EventHandler<ProgressEventArgs>(FFUTool.Device_ProgressEvent);
      device.EndTransfer();
      device.FlashFFUFile(ffuFilePath, optimize);
    }

    private static void DoWimFlash(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        FFUTool.PrepareFlash(param.Device);
        FFUTool.TransferWimIfPresent(ref param.Device, param.FfuFilePath, param.WimPath);
        FFUTool.FlashFile(param.Device, param.FfuFilePath, false);
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.DONE, (object) null);
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoFlash(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        FFUTool.PrepareFlash(param.Device);
        FFUTool.FlashFile(param.Device, param.FfuFilePath, false);
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.DONE, (object) null);
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoSetBootMode(SetBootModeParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        uint num = param.Device.SetBootMode(param.BootMode, param.ProfileName);
        if (num == 0U)
        {
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, (object) Resources.RESET_BOOT_MODE);
        }
        else
        {
          param.Result = -1;
          ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, (object) string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.ERROR_RESET_BOOT_MODE, (object) num));
        }
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void DoFastFlash(FlashParam param)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      try
      {
        FFUTool.PrepareFlash(param.Device);
        FFUTool.FlashFile(param.Device, param.FfuFilePath, true);
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.DONE, (object) null);
      }
      catch (Exception ex)
      {
        param.Result = -1;
        ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, (object) ex);
      }
      finally
      {
        param.WaitHandle.Set();
      }
    }

    private static void Device_ProgressEvent(object sender, ProgressEventArgs e) => ConsoleEx.Instance.UpdateProgress(e);
  }
}
