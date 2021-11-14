// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Diagnostics.KnownNames
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.Diagnostics
{
  internal static class KnownNames
  {
    public static bool TryGetInterfaceClassName(Guid interfaceClass, out string identifier)
    {
      Guid guid = new Guid(2782707472U, (ushort) 25904, (ushort) 4562, (byte) 144, (byte) 31, (byte) 0, (byte) 192, (byte) 79, (byte) 185, (byte) 81, (byte) 237);
      identifier = interfaceClass == guid ? "GUID_DEVINTERFACE_USB_DEVICE" : (string) null;
      return identifier != null;
    }

    public static bool TryGetEventTypeName(int eventType, out string identifier)
    {
      switch (eventType)
      {
        case 7:
          identifier = "DBT_DEVNODES_CHANGED";
          break;
        case 23:
          identifier = "DBT_QUERYCHANGECONFIG";
          break;
        case 24:
          identifier = "DBT_CONFIGCHANGED";
          break;
        case 25:
          identifier = "DBT_CONFIGCHANGECANCELED";
          break;
        case 32768:
          identifier = "DBT_DEVICEARRIVAL";
          break;
        case 32769:
          identifier = "DBT_DEVICEQUERYREMOVE";
          break;
        case 32770:
          identifier = "DBT_DEVICEQUERYREMOVEFAILED";
          break;
        case 32771:
          identifier = "DBT_DEVICEREMOVEPENDING";
          break;
        case 32772:
          identifier = "DBT_DEVICEREMOVECOMPLETE";
          break;
        case 32773:
          identifier = "DBT_DEVICETYPESPECIFIC";
          break;
        case 32774:
          identifier = "DBT_CUSTOMEVENT";
          break;
        case (int) ushort.MaxValue:
          identifier = "DBT_USERDEFINED";
          break;
        default:
          identifier = (string) null;
          return false;
      }
      return true;
    }

    public static bool TryGetDeviceTypeName(int deviceType, out string identifier)
    {
      switch (deviceType)
      {
        case 0:
          identifier = "DBT_DEVTYP_OEM";
          break;
        case 2:
          identifier = "DBT_DEVTYP_VOLUME";
          break;
        case 3:
          identifier = "DBT_DEVTYP_PORT";
          break;
        case 5:
          identifier = "DBT_DEVTYP_DEVICEINTERFACE";
          break;
        case 6:
          identifier = "DBT_DEVTYP_HANDLE";
          break;
        default:
          identifier = (string) null;
          return false;
      }
      return true;
    }

    public static string GetMessageName(int message)
    {
      if (message >= 0 && message < 1024)
      {
        switch (message)
        {
          case 0:
            return "WM_NULL";
          case 1:
            return "WM_CREATE";
          case 2:
            return "WM_DESTROY";
          case 13:
            return "WM_GETTEXT";
          case 14:
            return "WM_GETTEXTLENGTH";
          case 16:
            return "WM_CLOSE";
          case 18:
            return "WM_QUIT";
          case 36:
            return "WM_GETMINMAXINFO";
          case 129:
            return "WM_NCCREATE";
          case 130:
            return "WM_NCDESTROY";
          case 131:
            return "WM_NCCALCSIZE";
          case 537:
            return "WM_DEVICECHANGE";
          default:
            return "system message";
        }
      }
      else
        return message >= 1024 && message < 32768 ? (message != 1024 ? "WM_USER + " + (object) (message - 1024) : "WM_USER") : (message >= 32768 && message < 49152 ? (message != 32768 ? "WM_APP + " + (object) (message - 1024) : "WM_APP") : (message >= 49152 && message < (int) ushort.MaxValue ? "registered message" : "reserved"));
    }
  }
}
