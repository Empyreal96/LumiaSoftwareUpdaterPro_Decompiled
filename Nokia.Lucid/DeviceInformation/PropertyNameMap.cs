// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.PropertyNameMap
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nokia.Lucid.DeviceInformation
{
  [Serializable]
  public struct PropertyNameMap : IEquatable<PropertyNameMap>
  {
    private static PropertyNameMap defaultMap = PropertyNameMap.CreateDefaultMap();
    private readonly Dictionary<PropertyKey, string> mappings;

    public PropertyNameMap(PropertyKey propertyKey, string name) => this.mappings = new Dictionary<PropertyKey, string>()
    {
      {
        propertyKey,
        name
      }
    };

    public PropertyNameMap(
      IEnumerable<KeyValuePair<PropertyKey, string>> mappings)
    {
      if (mappings == null)
      {
        this.mappings = (Dictionary<PropertyKey, string>) null;
      }
      else
      {
        this.mappings = new Dictionary<PropertyKey, string>();
        foreach (KeyValuePair<PropertyKey, string> mapping in mappings)
          this.mappings.Add(mapping.Key, mapping.Value);
      }
    }

    private PropertyNameMap(
      IEnumerable<KeyValuePair<PropertyKey, string>> mappings,
      PropertyKey propertyKey,
      string name)
      : this(mappings)
    {
      if (this.mappings == null)
        this.mappings = new Dictionary<PropertyKey, string>(1);
      this.mappings[propertyKey] = name;
    }

    public static PropertyNameMap DefaultMap
    {
      get => PropertyNameMap.defaultMap;
      set => PropertyNameMap.defaultMap = value;
    }

    public bool IsEmpty => this.mappings == null || this.mappings.Count == 0;

    public static PropertyNameMap CreateDefaultMap() => new PropertyNameMap((IEnumerable<KeyValuePair<PropertyKey, string>>) new Dictionary<PropertyKey, string>()
    {
      {
        new PropertyKey(3072717104U, (ushort) 18415, (ushort) 4122, (byte) 165, (byte) 241, (byte) 2, (byte) 96, (byte) 140, (byte) 158, (byte) 235, (byte) 172, 10),
        "NAME"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 2),
        "Device_DeviceDesc"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 3),
        "Device_HardwareIds"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 4),
        "Device_CompatibleIds"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 6),
        "Device_Service"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 9),
        "Device_Class"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 10),
        "Device_ClassGuid"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 11),
        "Device_Driver"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 12),
        "Device_ConfigFlags"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 13),
        "Device_Manufacturer"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 14),
        "Device_FriendlyName"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 15),
        "Device_LocationInfo"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 16),
        "Device_PDOName"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 17),
        "Device_Capabilities"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 18),
        "Device_UINumber"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 19),
        "Device_UpperFilters"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 20),
        "Device_LowerFilters"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 21),
        "Device_BusTypeGuid"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 22),
        "Device_LegacyBusType"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 23),
        "Device_BusNumber"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 24),
        "Device_EnumeratorName"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 25),
        "Device_Security"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 26),
        "Device_SecuritySDS"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 27),
        "Device_DevType"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 28),
        "Device_Exclusive"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 29),
        "Device_Characteristics"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 30),
        "Device_Address"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 31),
        "Device_UINumberDescFormat"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 32),
        "Device_PowerData"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 33),
        "Device_RemovalPolicy"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 34),
        "Device_RemovalPolicyDefault"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 35),
        "Device_RemovalPolicyOverride"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 36),
        "Device_InstallState"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 37),
        "Device_LocationPaths"
      },
      {
        new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 38),
        "Device_BaseContainerId"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 2),
        "Device_DevNodeStatus"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 3),
        "Device_ProblemCode"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 4),
        "Device_EjectionRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 5),
        "Device_RemovalRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 6),
        "Device_PowerRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 7),
        "Device_BusRelations"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 8),
        "Device_Parent"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 9),
        "Device_Children"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 10),
        "Device_Siblings"
      },
      {
        new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 11),
        "Device_TransportRelations"
      },
      {
        new PropertyKey(2152296704U, (ushort) 35955, (ushort) 18617, (byte) 170, (byte) 217, (byte) 206, (byte) 56, (byte) 126, (byte) 25, (byte) 197, (byte) 110, 2),
        "Device_Reported"
      },
      {
        new PropertyKey(2152296704U, (ushort) 35955, (ushort) 18617, (byte) 170, (byte) 217, (byte) 206, (byte) 56, (byte) 126, (byte) 25, (byte) 197, (byte) 110, 3),
        "Device_Legacy"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 256),
        "Device_InstanceId"
      },
      {
        new PropertyKey(2357121542U, (ushort) 16266, (ushort) 18471, (byte) 179, (byte) 171, (byte) 174, (byte) 158, (byte) 31, (byte) 174, (byte) 252, (byte) 108, 2),
        "Device_ContainerId"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 2),
        "Device_ModelId"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 3),
        "Device_FriendlyNameAttributes"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 4),
        "Device_ManufacturerAttributes"
      },
      {
        new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 5),
        "Device_PresenceNotForDevice"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 1),
        "Numa_Proximity_Domain"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 2),
        "Device_DHP_Rebalance_Policy"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 3),
        "Device_Numa_Node"
      },
      {
        new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 4),
        "Device_BusReportedDeviceDesc"
      },
      {
        new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 6),
        "Device_SessionId"
      },
      {
        new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 100),
        "Device_InstallDate"
      },
      {
        new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 101),
        "Device_FirstInstallDate"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 2),
        "Device_DriverDate"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 3),
        "Device_DriverVersion"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 4),
        "Device_DriverDesc"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 5),
        "Device_DriverInfPath"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 6),
        "Device_DriverInfSection"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 7),
        "Device_DriverInfSectionExt"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 8),
        "Device_MatchingDeviceId"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 9),
        "Device_DriverProvider"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 10),
        "Device_DriverPropPageProvider"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 11),
        "Device_DriverCoInstallers"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 12),
        "Device_ResourcePickerTags"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 13),
        "Device_ResourcePickerExceptions"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 14),
        "Device_DriverRank"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 15),
        "Device_DriverLogoLevel"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 17),
        "Device_NoConnectSound"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 18),
        "Device_GenericDriverInstalled"
      },
      {
        new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 19),
        "Device_AdditionalSoftwareRequested"
      },
      {
        new PropertyKey(2950264384U, (ushort) 34467, (ushort) 16912, (byte) 182, (byte) 124, (byte) 40, (byte) 156, (byte) 65, (byte) 170, (byte) 190, (byte) 85, 2),
        "Device_SafeRemovalRequired"
      },
      {
        new PropertyKey(2950264384U, (ushort) 34467, (ushort) 16912, (byte) 182, (byte) 124, (byte) 40, (byte) 156, (byte) 65, (byte) 170, (byte) 190, (byte) 85, 3),
        "Device_SafeRemovalRequiredOverride"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 2),
        "DrvPkg_Model"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 3),
        "DrvPkg_VendorWebSite"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 4),
        "DrvPkg_DetailedDescription"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 5),
        "DrvPkg_DocumentationLink"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 6),
        "DrvPkg_Icon"
      },
      {
        new PropertyKey(3480468305U, (ushort) 15039, (ushort) 17570, (byte) 133, (byte) 224, (byte) 154, (byte) 61, (byte) 199, (byte) 161, (byte) 33, (byte) 50, 7),
        "DrvPkg_BrandingIcon"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 19),
        "DeviceClass_UpperFilters"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 20),
        "DeviceClass_LowerFilters"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 25),
        "DeviceClass_Security"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 26),
        "DeviceClass_SecuritySDS"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 27),
        "DeviceClass_DevType"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 28),
        "DeviceClass_Exclusive"
      },
      {
        new PropertyKey(1126273419U, (ushort) 63134, (ushort) 18189, (byte) 165, (byte) 222, (byte) 77, (byte) 136, (byte) 199, (byte) 90, (byte) 210, (byte) 75, 29),
        "DeviceClass_Characteristics"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 2),
        "DeviceClass_Name"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 3),
        "DeviceClass_ClassName"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 4),
        "DeviceClass_Icon"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 5),
        "DeviceClass_ClassInstaller"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 6),
        "DeviceClass_PropPageProvider"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 7),
        "DeviceClass_NoInstallClass"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 8),
        "DeviceClass_NoDisplayClass"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 9),
        "DeviceClass_SilentInstall"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 10),
        "DeviceClass_NoUseClass"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 11),
        "DeviceClass_DefaultService"
      },
      {
        new PropertyKey(630898684, (short) 20647, (short) 18382, (byte) 175, (byte) 8, (byte) 104, (byte) 201, (byte) 167, (byte) 215, (byte) 51, (byte) 102, 12),
        "DeviceClass_IconPath"
      },
      {
        new PropertyKey(3511500531U, (ushort) 26319, (ushort) 19362, (byte) 157, (byte) 56, (byte) 13, (byte) 219, (byte) 55, (byte) 171, (byte) 71, (byte) 1, 2),
        "DeviceClass_DHPRebalanceOptOut"
      },
      {
        new PropertyKey(1899828995U, (ushort) 41698, (ushort) 18933, (byte) 146, (byte) 20, (byte) 86, (byte) 71, (byte) 46, (byte) 243, (byte) 218, (byte) 92, 2),
        "DeviceClass_ClassCoInstallers"
      },
      {
        new PropertyKey(40784238U, (ushort) 47124, (ushort) 16715, (byte) 131, (byte) 205, (byte) 133, (byte) 109, (byte) 111, (byte) 239, (byte) 72, (byte) 34, 2),
        "DeviceInterface_FriendlyName"
      },
      {
        new PropertyKey(40784238U, (ushort) 47124, (ushort) 16715, (byte) 131, (byte) 205, (byte) 133, (byte) 109, (byte) 111, (byte) 239, (byte) 72, (byte) 34, 3),
        "DeviceInterface_Enabled"
      },
      {
        new PropertyKey(40784238U, (ushort) 47124, (ushort) 16715, (byte) 131, (byte) 205, (byte) 133, (byte) 109, (byte) 111, (byte) 239, (byte) 72, (byte) 34, 4),
        "DeviceInterface_ClassGuid"
      },
      {
        new PropertyKey(348666521, (short) 2879, (short) 17591, (byte) 190, (byte) 76, (byte) 161, (byte) 120, (byte) 211, (byte) 153, (byte) 5, (byte) 100, 2),
        "DeviceInterfaceClass_DefaultInterface"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 68),
        "DeviceDisplay_IsShowInDisconnectedState"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 74),
        "DeviceDisplay_IsNotInterestingForDisplay"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 90),
        "DeviceDisplay_Category"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 98),
        "DeviceDisplay_UnpairUninstall"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 99),
        "DeviceDisplay_RequiresUninstallElevation"
      },
      {
        new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 101),
        "DeviceDisplay_AlwaysShowDeviceAsConnected"
      }
    });

    public static bool operator !=(PropertyNameMap left, PropertyNameMap right) => !object.Equals((object) left, (object) right);

    public static bool operator ==(PropertyNameMap left, PropertyNameMap right) => object.Equals((object) left, (object) right);

    public PropertyNameMap SetMapping(PropertyKey propertyKey, string name) => new PropertyNameMap((IEnumerable<KeyValuePair<PropertyKey, string>>) this.mappings, propertyKey, name);

    public PropertyNameMap SetMappings(
      IEnumerable<KeyValuePair<PropertyKey, string>> range)
    {
      if (range == null)
        return this;
      return this.mappings == null ? new PropertyNameMap(range) : new PropertyNameMap(this.mappings.Union<KeyValuePair<PropertyKey, string>>(range));
    }

    public PropertyNameMap ClearMapping(PropertyKey propertyKey) => new PropertyNameMap(this.mappings.Where<KeyValuePair<PropertyKey, string>>((Func<KeyValuePair<PropertyKey, string>, bool>) (m => m.Key != propertyKey)));

    public PropertyNameMap ClearMappings(params PropertyKey[] range) => this.ClearMappings((IEnumerable<PropertyKey>) range);

    public PropertyNameMap ClearMappings(IEnumerable<PropertyKey> range) => range == null || this.mappings == null ? this : new PropertyNameMap(this.mappings.Except<KeyValuePair<PropertyKey, string>>(this.mappings.Join<KeyValuePair<PropertyKey, string>, PropertyKey, PropertyKey, KeyValuePair<PropertyKey, string>>(range, (Func<KeyValuePair<PropertyKey, string>, PropertyKey>) (m => m.Key), (Func<PropertyKey, PropertyKey>) (m => m), (Func<KeyValuePair<PropertyKey, string>, PropertyKey, KeyValuePair<PropertyKey, string>>) ((m, g) => m))));

    public bool TryGetMapping(PropertyKey propertyKey, out string name) => this.mappings.TryGetValue(propertyKey, out name);

    public string GetMapping(PropertyKey propertyKey)
    {
      string str;
      if (!this.mappings.TryGetValue(propertyKey, out str))
        throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.KeyNotFoundException_MessageFormat_PropertyKeyMappingNotFound, (object) propertyKey.Category, (object) propertyKey.PropertyId));
      return str;
    }

    public override bool Equals(object obj) => obj is PropertyNameMap other && this.Equals(other);

    public bool Equals(PropertyNameMap other)
    {
      if (this.IsEmpty)
        return other.IsEmpty;
      return !other.IsEmpty && this.mappings.Count == other.mappings.Count && this.mappings.OrderBy<KeyValuePair<PropertyKey, string>, PropertyKey>((Func<KeyValuePair<PropertyKey, string>, PropertyKey>) (m => m.Key)).SequenceEqual<KeyValuePair<PropertyKey, string>>((IEnumerable<KeyValuePair<PropertyKey, string>>) other.mappings.OrderBy<KeyValuePair<PropertyKey, string>, PropertyKey>((Func<KeyValuePair<PropertyKey, string>, PropertyKey>) (m => m.Key)));
    }

    public override int GetHashCode() => this.mappings == null ? 0 : this.mappings.OrderBy<KeyValuePair<PropertyKey, string>, PropertyKey>((Func<KeyValuePair<PropertyKey, string>, PropertyKey>) (m => m.Key)).GetHashCode();
  }
}
