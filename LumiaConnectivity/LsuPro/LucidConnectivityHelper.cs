// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.LucidConnectivityHelper
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using Microsoft.LumiaConnectivity;
using Nokia.Lucid.DeviceInformation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.LsuPro
{
  public static class LucidConnectivityHelper
  {
    public static void GetVidAndPid(string deviceId, out string vid, out string pid)
    {
      vid = string.Empty;
      pid = string.Empty;
      try
      {
        string str = deviceId.Substring(deviceId.IndexOf("VID", StringComparison.OrdinalIgnoreCase), 17);
        vid = str.Substring(str.IndexOf("VID", StringComparison.OrdinalIgnoreCase) + 4, 4).ToUpperInvariant();
        pid = str.Substring(str.IndexOf("PID", StringComparison.OrdinalIgnoreCase) + 4, 4).ToUpperInvariant();
      }
      catch (Exception ex)
      {
        Tracer.Error(ex, "Error extracting VID and PID: {0}", (object) ex.Message);
      }
    }

    public static ConnectedDeviceMode GetDeviceMode(string vid, string pid)
    {
      string str1 = vid + "&" + pid;
      if (str1.Contains("&") && !str1.Contains("VID_") && !str1.Contains("PID_"))
      {
        string str2 = str1.Insert(0, "VID_");
        str1 = str2.Insert(str2.IndexOf("&", StringComparison.OrdinalIgnoreCase) + 1, "PID_");
      }
      switch (str1)
      {
        case "VID_0421&PID_0660":
        case "VID_045E&PID_0A01":
          return ConnectedDeviceMode.Label;
        case "VID_0421&PID_0661":
        case "VID_0421&PID_06FC":
        case "VID_045E&PID_0A00":
          return ConnectedDeviceMode.Normal;
        case "VID_0421&PID_066E":
        case "VID_0421&PID_0714":
        case "VID_045E&PID_0A02":
          return ConnectedDeviceMode.Uefi;
        case "VID_0421&PID_0713":
          return ConnectedDeviceMode.Test;
        case "VID_045E&PID_062A":
          return ConnectedDeviceMode.MsFlashing;
        case "VID_045E&PID_0A03":
        case "VID_05C6&PID_9008":
          return ConnectedDeviceMode.QcomDload;
        case "VID_045E&PID_9006":
        case "VID_05C6&PID_9006":
          return ConnectedDeviceMode.MassStorage;
        case "VID_05C6&PID_319B":
          return ConnectedDeviceMode.QcomSerialComposite;
        case "VID_05C6&PID_9001":
          return ConnectedDeviceMode.QcomRmnetComposite;
        case "VID_3495&PID_00E0":
          return ConnectedDeviceMode.KernelModeDebugging;
        default:
          return ConnectedDeviceMode.Unknown;
      }
    }

    public static ConnectedDeviceMode GetDeviceModeFromDevicePath(
      string devicePath)
    {
      Tracer.Information("DevicePath: {0}", (object) devicePath);
      try
      {
        string vid;
        string pid;
        LucidConnectivityHelper.GetVidAndPid(devicePath, out vid, out pid);
        Tracer.Information("Vid {0}, Pid {1}", (object) vid, (object) pid);
        return LucidConnectivityHelper.GetDeviceMode(vid, pid);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Failed to determine device mode from device path", objArray);
        return ConnectedDeviceMode.Unknown;
      }
    }

    public static void ParseTypeDesignatorAndSalesName(
      string busReportedDeviceDescription,
      out string typeDesignator,
      out string salesName)
    {
      typeDesignator = string.Empty;
      salesName = string.Empty;
      if (busReportedDeviceDescription.Contains("|"))
      {
        string[] strArray = busReportedDeviceDescription.Split('|');
        typeDesignator = strArray.Length != 0 ? strArray[0] : string.Empty;
        salesName = strArray.Length > 1 ? strArray[1] : string.Empty;
      }
      else if (busReportedDeviceDescription.Contains("(") && busReportedDeviceDescription.Contains(")"))
      {
        int val1 = busReportedDeviceDescription.LastIndexOf('(');
        int num = busReportedDeviceDescription.IndexOf(')', Math.Max(val1, 0));
        int length = busReportedDeviceDescription.IndexOf(" (", StringComparison.OrdinalIgnoreCase);
        if (val1 > -1 && num > val1)
          typeDesignator = busReportedDeviceDescription.Substring(val1 + 1, num - val1 - 1);
        if (length <= -1)
          return;
        salesName = busReportedDeviceDescription.Substring(0, length);
      }
      else
        salesName = busReportedDeviceDescription;
    }

    public static string GetHubAndPort(string locationInfo)
    {
      try
      {
        int num1 = locationInfo.IndexOf("HUB_#", StringComparison.OrdinalIgnoreCase);
        int num2 = locationInfo.IndexOf("PORT_#", StringComparison.OrdinalIgnoreCase);
        if (num1 < 0 || num2 < 0)
          throw new Exception("Wrong string format");
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) locationInfo.Substring(num1 + 5, 4), (object) locationInfo.Substring(num2 + 6, 4));
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Error extracting hub and port IDs", objArray);
        return string.Empty;
      }
    }

    public static string LocationPath2ControllerId(string controllerLocationPath)
    {
      Tracer.Information("Getting controller ID for {0}", (object) controllerLocationPath);
      string empty = string.Empty;
      int length = controllerLocationPath.IndexOf("#USBROOT(", StringComparison.OrdinalIgnoreCase);
      if (length > 0)
      {
        controllerLocationPath = controllerLocationPath.Substring(0, length);
        Tracer.Information("Location path fixed: {0}", (object) controllerLocationPath);
      }
      foreach (Match match in Regex.Matches(controllerLocationPath, "\\(([a-z0-9]+)\\)", RegexOptions.IgnoreCase))
      {
        if (2 == match.Groups.Count)
        {
          if (!string.IsNullOrEmpty(empty))
            empty += ".";
          empty += match.Groups[1].Value;
        }
      }
      Tracer.Information("Controller ID: {0}", (object) empty);
      return empty;
    }

    internal static string GetSuitableLabelModeInterfaceDevicePath(
      ReadOnlyCollection<UsbDeviceEndpoint> endpoints)
    {
      string str = string.Empty;
      using (IEnumerator<UsbDeviceEndpoint> enumerator = endpoints.Where<UsbDeviceEndpoint>((Func<UsbDeviceEndpoint, bool>) (i => i.DevicePath.IndexOf("mi_04", StringComparison.OrdinalIgnoreCase) > 0)).GetEnumerator())
      {
        if (enumerator.MoveNext())
          str = enumerator.Current.DevicePath;
      }
      return str;
    }

    internal static ConnectedDevice GetConnectedDeviceFromUsbDevice(
      UsbDevice usbDevice)
    {
      ConnectedDeviceMode deviceMode = LucidConnectivityHelper.GetDeviceMode(usbDevice.Vid, usbDevice.Pid);
      ConnectedDevice connectedDevice = new ConnectedDevice(usbDevice.PortId, usbDevice.Vid, usbDevice.Pid, deviceMode, true, usbDevice.TypeDesignator, usbDevice.SalesName);
      string str = string.Empty;
      switch (deviceMode)
      {
        case ConnectedDeviceMode.Normal:
        case ConnectedDeviceMode.Uefi:
          if (usbDevice.Interfaces.Count > 0)
          {
            str = usbDevice.Interfaces[0].DevicePath;
            break;
          }
          break;
        case ConnectedDeviceMode.Label:
          str = LucidConnectivityHelper.GetSuitableLabelModeInterfaceDevicePath(usbDevice.Interfaces);
          break;
      }
      if (string.IsNullOrEmpty(str))
      {
        connectedDevice.DeviceReady = false;
        connectedDevice.DevicePath = string.Empty;
      }
      else
      {
        connectedDevice.DeviceReady = true;
        connectedDevice.DevicePath = str;
      }
      return connectedDevice;
    }

    internal static bool IsWrongDefaultNcsdInterface(DeviceInfo device)
    {
      try
      {
        string vid = string.Empty;
        string pid = string.Empty;
        LucidConnectivityHelper.GetVidAndPid(device.InstanceId, out vid, out pid);
        if (LucidConnectivityHelper.GetDeviceMode(vid, pid) == ConnectedDeviceMode.Normal)
        {
          string identifierFromDeviceId = LucidConnectivityHelper.GetMiIdentifierFromDeviceId(device.InstanceId);
          if (device.ReadSiblingInstanceIds().Length == 3 && identifierFromDeviceId != "03")
          {
            Tracer.Information("Interface {0} has 3 siblings and is not MI_03. Ignoring interface.", (object) device.Path);
            return true;
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Failed to check default NCSd interface", objArray);
        return false;
      }
    }

    internal static string GetMiIdentifierFromDeviceId(string deviceId)
    {
      string str = string.Empty;
      int num1 = deviceId.IndexOf("mi_", StringComparison.OrdinalIgnoreCase);
      if (num1 > 0)
      {
        int startIndex = num1 + 3;
        int num2 = deviceId.IndexOf("\\", startIndex, StringComparison.OrdinalIgnoreCase);
        if (num2 > 0)
          str = deviceId.Substring(startIndex, num2 - startIndex);
      }
      return str;
    }
  }
}
