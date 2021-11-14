// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.DeviceInfo
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Interop.Win32Types;
using Nokia.Lucid.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Nokia.Lucid.DeviceInformation
{
  public sealed class DeviceInfo : IDevicePropertySet
  {
    private readonly INativeDeviceInfoSet deviceInfoSet;
    private readonly string path;
    private readonly string instanceId;
    private readonly int instanceHandle;
    private readonly Guid setupClass;
    private readonly DeviceType deviceType;
    private readonly IntPtr reserved;
    private DeviceStatus status;

    internal DeviceInfo(
      INativeDeviceInfoSet deviceInfoSet,
      string devicePath,
      int instanceHandle,
      string instanceId,
      Guid setupClass,
      DeviceType deviceType,
      SP_DEVICE_INTERFACE_DATA interfaceData,
      IntPtr reserved)
    {
      this.deviceInfoSet = deviceInfoSet;
      this.path = devicePath;
      this.instanceHandle = instanceHandle;
      this.instanceId = instanceId;
      this.setupClass = setupClass;
      this.deviceType = deviceType;
      this.status = (DeviceStatus) interfaceData.Flags;
      this.DeviceInterfaceGuid = interfaceData.InterfaceClassGuid;
      this.reserved = reserved;
    }

    public Guid DeviceInterfaceGuid { get; private set; }

    public string InstanceId => this.instanceId;

    public string Path => this.path;

    public DeviceStatus Status => this.status;

    public bool IsPresent => (this.status & DeviceStatus.Present) == DeviceStatus.Present;

    public bool IsDefault => (this.status & DeviceStatus.Default) == DeviceStatus.Default;

    public bool IsRemoved => (this.status & DeviceStatus.Removed) == DeviceStatus.Removed;

    public DeviceType DeviceType => this.deviceType;

    IEnumerable<PropertyKey> IDevicePropertySet.EnumeratePropertyKeys()
    {
      SP_DEVINFO_DATA deviceData = new SP_DEVINFO_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVINFO_DATA)),
        ClassGuid = this.setupClass,
        DevInst = this.instanceHandle,
        Reserved = this.reserved
      };
      return (IEnumerable<PropertyKey>) this.deviceInfoSet.GetDevicePropertyKeys(ref deviceData);
    }

    object IDevicePropertySet.ReadProperty(
      PropertyKey key,
      IPropertyValueFormatter formatter)
    {
      if (formatter == null)
        throw new ArgumentNullException(nameof (formatter));
      SP_DEVINFO_DATA deviceData = new SP_DEVINFO_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVINFO_DATA)),
        ClassGuid = this.setupClass,
        DevInst = this.instanceHandle,
        Reserved = this.reserved
      };
      int propertyType;
      byte[] deviceProperty;
      try
      {
        deviceProperty = this.deviceInfoSet.GetDeviceProperty(ref deviceData, ref key, out propertyType);
      }
      catch (Win32Exception ex)
      {
        if (ex.NativeErrorCode == 1168)
        {
          string identifier;
          string str = DeviceInfo.TryGetPropertyKeyIdentifier(key, out identifier) ? " (" + identifier + ")" : string.Empty;
          throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.KeyNotFoundException_MessageFormat_PropertyNotFound, (object) key, (object) str), (Exception) ex);
        }
        throw;
      }
      return formatter.ReadFrom(deviceProperty, 0, deviceProperty.Length, (PropertyType) propertyType);
    }

    bool IDevicePropertySet.TryReadProperty(
      PropertyKey key,
      IPropertyValueFormatter formatter,
      out object value)
    {
      if (formatter == null)
        throw new ArgumentNullException(nameof (formatter));
      SP_DEVINFO_DATA deviceData = new SP_DEVINFO_DATA()
      {
        cbSize = Marshal.SizeOf(typeof (SP_DEVINFO_DATA)),
        ClassGuid = this.setupClass,
        DevInst = this.instanceHandle,
        Reserved = this.reserved
      };
      int propertyType;
      byte[] buffer;
      if (this.deviceInfoSet.TryGetDeviceProperty(ref deviceData, ref key, out propertyType, out buffer))
        return formatter.TryReadFrom(buffer, 0, buffer.Length, (PropertyType) propertyType, out value);
      value = (object) null;
      return false;
    }

    public void RefreshStatus() => this.status = (DeviceStatus) this.deviceInfoSet.GetDeviceInterface(this.path).Flags;

    public DeviceStatus ReadStatus()
    {
      this.RefreshStatus();
      return this.status;
    }

    public bool ReadIsPresent() => (this.ReadStatus() & DeviceStatus.Present) == DeviceStatus.Present;

    public bool ReadIsDefault() => (this.ReadStatus() & DeviceStatus.Default) == DeviceStatus.Default;

    public bool ReadIsRemoved() => (this.ReadStatus() & DeviceStatus.Removed) == DeviceStatus.Removed;

    private static bool TryGetPropertyKeyIdentifier(PropertyKey key, out string identifier) => new Dictionary<PropertyKey, string>()
    {
      {
        new PropertyKey(3072717104U, (ushort) 18415, (ushort) 4122, (byte) 165, (byte) 241, (byte) 2, (byte) 96, (byte) 140, (byte) 158, (byte) 235, (byte) 172, 10),
        "DEVPKEY_NAME"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 2),
        "DEVPKEY_Device_DeviceDesc"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 3),
        "DEVPKEY_Device_HardwareIds"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 4),
        "DEVPKEY_Device_CompatibleIds"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 6),
        "DEVPKEY_Device_Service"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 9),
        "DEVPKEY_Device_Class"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 10),
        "DEVPKEY_Device_ClassGuid"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 11),
        "DEVPKEY_Device_Driver"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 12),
        "DEVPKEY_Device_ConfigFlags"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 13),
        "DEVPKEY_Device_Manufacturer"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 14),
        "DEVPKEY_Device_FriendlyName"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 15),
        "DEVPKEY_Device_LocationInfo"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 16),
        "DEVPKEY_Device_PDOName"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 17),
        "DEVPKEY_Device_Capabilities"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 18),
        "DEVPKEY_Device_UINumber"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 19),
        "DEVPKEY_Device_UpperFilters"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 20),
        "DEVPKEY_Device_LowerFilters"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 21),
        "DEVPKEY_Device_BusTypeGuid"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 22),
        "DEVPKEY_Device_LegacyBusType"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 23),
        "DEVPKEY_Device_BusNumber"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 24),
        "DEVPKEY_Device_EnumeratorName"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 25),
        "DEVPKEY_Device_Security"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 26),
        "DEVPKEY_Device_SecuritySDS"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 27),
        "DEVPKEY_Device_DevType"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 28),
        "DEVPKEY_Device_Exclusive"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 29),
        "DEVPKEY_Device_Characteristics"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 30),
        "DEVPKEY_Device_Address"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 31),
        "DEVPKEY_Device_UINumberDescFormat"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 32),
        "DEVPKEY_Device_PowerData"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 33),
        "DEVPKEY_Device_RemovalPolicy"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 34),
        "DEVPKEY_Device_RemovalPolicyDefault"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 35),
        "DEVPKEY_Device_RemovalPolicyOverride"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 36),
        "DEVPKEY_Device_InstallState"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 37),
        "DEVPKEY_Device_LocationPaths"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 38),
        "DEVPKEY_Device_BaseContainerId"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 2),
        "DEVPKEY_Device_DevNodeStatus"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 3),
        "DEVPKEY_Device_ProblemCode"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 4),
        "DEVPKEY_Device_EjectionRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 5),
        "DEVPKEY_Device_RemovalRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 6),
        "DEVPKEY_Device_PowerRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 7),
        "DEVPKEY_Device_BusRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 8),
        "DEVPKEY_Device_Parent"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 9),
        "DEVPKEY_Device_Children"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 10),
        "DEVPKEY_Device_Siblings"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 11),
        "DEVPKEY_Device_TransportRelations"
      },
      {
        new PropertyKey(2152296704U, (ushort) 35955, (ushort) 18617, (byte) 170, (byte) 217, (byte) 206, (byte) 56, (byte) 126, (byte) 25, (byte) 197, (byte) 110, 2),
        "DEVPKEY_Device_Reported"
      },
      {
        new PropertyKey(2152296704U, (ushort) 35955, (ushort) 18617, (byte) 170, (byte) 217, (byte) 206, (byte) 56, (byte) 126, (byte) 25, (byte) 197, (byte) 110, 3),
        "DEVPKEY_Device_Legacy"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 256),
        "DEVPKEY_Device_InstanceId"
      },
      {
        new PropertyKey(2357121542U, (ushort) 16266, (ushort) 18471, (byte) 179, (byte) 171, (byte) 174, (byte) 158, (byte) 31, (byte) 174, (byte) 252, (byte) 108, 2),
        "DEVPKEY_Device_ContainerId"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 2),
        "DEVPKEY_Device_ModelId"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 3),
        "DEVPKEY_Device_FriendlyNameAttributes"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 4),
        "DEVPKEY_Device_ManufacturerAttributes"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 5),
        "DEVPKEY_Device_PresenceNotForDevice"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 1),
        "DEVPKEY_Numa_Proximity_Domain"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 2),
        "DEVPKEY_Device_DHP_Rebalance_Policy"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 3),
        "DEVPKEY_Device_Numa_Node"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 4),
        "DEVPKEY_Device_BusReportedDeviceDesc"
      },
      {
        new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 6),
        "DEVPKEY_Device_SessionId"
      },
      {
        new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 100),
        "DEVPKEY_Device_InstallDate"
      },
      {
        new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 101),
        "DEVPKEY_Device_FirstInstallDate"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 2),
        "DEVPKEY_Device_DriverDate"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 3),
        "DEVPKEY_Device_DriverVersion"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 4),
        "DEVPKEY_Device_DriverDesc"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 5),
        "DEVPKEY_Device_DriverInfPath"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 6),
        "DEVPKEY_Device_DriverInfSection"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 7),
        "DEVPKEY_Device_DriverInfSectionExt"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 8),
        "DEVPKEY_Device_MatchingDeviceId"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 9),
        "DEVPKEY_Device_DriverProvider"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 10),
        "DEVPKEY_Device_DriverPropPageProvider"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 11),
        "DEVPKEY_Device_DriverCoInstallers"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 12),
        "DEVPKEY_Device_ResourcePickerTags"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 13),
        "DEVPKEY_Device_ResourcePickerExceptions"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 14),
        "DEVPKEY_Device_DriverRank"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 15),
        "DEVPKEY_Device_DriverLogoLevel"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 17),
        "DEVPKEY_Device_NoConnectSound"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 18),
        "DEVPKEY_Device_GenericDriverInstalled"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 19),
        "DEVPKEY_Device_AdditionalSoftwareRequested"
      },
      {
        new PropertyKey(2950264384U, (ushort) 34467, (ushort) 16912, (byte) 182, (byte) 124, (byte) 40, (byte) 156, (byte) 65, (byte) 170, (byte) 190, (byte) 85, 2),
        "DEVPKEY_Device_SafeRemovalRequired"
      },
      {
        new PropertyKey(2950264384U, (ushort) 34467, (ushort) 16912, (byte) 182, (byte) 124, (byte) 40, (byte) 156, (byte) 65, (byte) 170, (byte) 190, (byte) 85, 3),
        "DEVPKEY_Device_SafeRemovalRequiredOverride"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 2),
        "DEVPKEY_DrvPkg_Model"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 3),
        "DEVPKEY_DrvPkg_VendorWebSite"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 4),
        "DEVPKEY_DrvPkg_DetailedDescription"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 5),
        "DEVPKEY_DrvPkg_DocumentationLink"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 6),
        "DEVPKEY_DrvPkg_Icon"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 7),
        "DEVPKEY_DrvPkg_BrandingIcon"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 19),
        "DEVPKEY_DeviceClass_UpperFilters"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 20),
        "DEVPKEY_DeviceClass_LowerFilters"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 25),
        "DEVPKEY_DeviceClass_Security"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 26),
        "DEVPKEY_DeviceClass_SecuritySDS"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 27),
        "DEVPKEY_DeviceClass_DevType"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 28),
        "DEVPKEY_DeviceClass_Exclusive"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 29),
        "DEVPKEY_DeviceClass_Characteristics"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 2),
        "DEVPKEY_DeviceClass_Name"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 3),
        "DEVPKEY_DeviceClass_ClassName"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 4),
        "DEVPKEY_DeviceClass_Icon"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 5),
        "DEVPKEY_DeviceClass_ClassInstaller"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 6),
        "DEVPKEY_DeviceClass_PropPageProvider"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 7),
        "DEVPKEY_DeviceClass_NoInstallClass"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 8),
        "DEVPKEY_DeviceClass_NoDisplayClass"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 9),
        "DEVPKEY_DeviceClass_SilentInstall"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 10),
        "DEVPKEY_DeviceClass_NoUseClass"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 11),
        "DEVPKEY_DeviceClass_DefaultService"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 12),
        "DEVPKEY_DeviceClass_IconPath"
      },
      {
        new PropertyKey(3511500531U, (ushort) 26319, (ushort) 19362, (byte) 157, (byte) 56, (byte) 13, (byte) 219, (byte) 55, (byte) 171, (byte) 71, (byte) 1, 2),
        "DEVPKEY_DeviceClass_DHPRebalanceOptOut"
      },
      {
        new PropertyKey(1899828995U, (ushort) 41698, (ushort) 18933, (byte) 146, (byte) 20, (byte) 86, (byte) 71, (byte) 46, (byte) 243, (byte) 218, (byte) 92, 2),
        "DEVPKEY_DeviceClass_ClassCoInstallers"
      },
      {
        new PropertyKey(40784238U, (ushort) 47124, (ushort) 16715, (byte) 131, (byte) 205, (byte) 133, (byte) 109, (byte) 111, (byte) 239, (byte) 72, (byte) 34, 2),
        "DEVPKEY_DeviceInterface_FriendlyName"
      },
      {
        new PropertyKey(40784238U, (ushort) 47124, (ushort) 16715, (byte) 131, (byte) 205, (byte) 133, (byte) 109, (byte) 111, (byte) 239, (byte) 72, (byte) 34, 3),
        "DEVPKEY_DeviceInterface_Enabled"
      },
      {
        new PropertyKey(40784238U, (ushort) 47124, (ushort) 16715, (byte) 131, (byte) 205, (byte) 133, (byte) 109, (byte) 111, (byte) 239, (byte) 72, (byte) 34, 4),
        "DEVPKEY_DeviceInterface_ClassGuid"
      },
      {
        new PropertyKey(348666521, (short) 2879, (short) 17591, (byte) 190, (byte) 76, (byte) 161, (byte) 120, (byte) 211, (byte) 153, (byte) 5, (byte) 100, 2),
        "DEVPKEY_DeviceInterfaceClass_DefaultInterface"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 68),
        "DEVPKEY_DeviceDisplay_IsShowInDisconnectedState"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 74),
        "DEVPKEY_DeviceDisplay_IsNotInterestingForDisplay"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 90),
        "DEVPKEY_DeviceDisplay_Category"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 98),
        "DEVPKEY_DeviceDisplay_UnpairUninstall"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 99),
        "DEVPKEY_DeviceDisplay_RequiresUninstallElevation"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 101),
        "DEVPKEY_DeviceDisplay_AlwaysShowDeviceAsConnected"
      }
    }.TryGetValue(key, out identifier);
  }
}
