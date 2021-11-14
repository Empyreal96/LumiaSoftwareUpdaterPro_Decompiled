// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceTypeMap
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nokia.Lucid
{
  [Serializable]
  public struct DeviceTypeMap : IEquatable<DeviceTypeMap>
  {
    private static DeviceTypeMap defaultMap = DeviceTypeMap.CreateDefaultMap();
    private readonly Dictionary<Guid, DeviceType> mappings;

    public DeviceTypeMap(Guid interfaceClass, DeviceType deviceType) => this.mappings = new Dictionary<Guid, DeviceType>()
    {
      {
        interfaceClass,
        deviceType
      }
    };

    public DeviceTypeMap(
      IEnumerable<KeyValuePair<Guid, DeviceType>> mappings)
    {
      if (mappings == null)
      {
        this.mappings = (Dictionary<Guid, DeviceType>) null;
      }
      else
      {
        this.mappings = new Dictionary<Guid, DeviceType>();
        foreach (KeyValuePair<Guid, DeviceType> mapping in mappings)
          this.mappings.Add(mapping.Key, mapping.Value);
      }
    }

    private DeviceTypeMap(
      IEnumerable<KeyValuePair<Guid, DeviceType>> mappings,
      Guid interfaceClass,
      DeviceType deviceType)
      : this(mappings)
    {
      if (this.mappings == null)
        this.mappings = new Dictionary<Guid, DeviceType>(1);
      this.mappings[interfaceClass] = deviceType;
    }

    public static DeviceTypeMap DefaultMap
    {
      get => DeviceTypeMap.defaultMap;
      set => DeviceTypeMap.defaultMap = value;
    }

    public IEnumerable<Guid> InterfaceClasses
    {
      get
      {
        if (this.mappings != null)
        {
          foreach (KeyValuePair<Guid, DeviceType> mapping in this.mappings)
            yield return mapping.Key;
        }
      }
    }

    public bool IsEmpty => this.mappings == null || this.mappings.Count == 0;

    public static DeviceTypeMap CreateDefaultMap() => new DeviceTypeMap((IEnumerable<KeyValuePair<Guid, DeviceType>>) new Dictionary<Guid, DeviceType>()
    {
      {
        WindowsPhoneIdentifiers.CareConnectivityDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.LumiaConnectivityDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.ApolloDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.TestServerDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.LabelAppDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.NcsdDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.UefiDeviceInterfaceGuid,
        DeviceType.Interface
      },
      {
        WindowsPhoneIdentifiers.EdDeviceInterfaceGuid,
        DeviceType.Interface
      }
    });

    public static bool operator !=(DeviceTypeMap left, DeviceTypeMap right) => !object.Equals((object) left, (object) right);

    public static bool operator ==(DeviceTypeMap left, DeviceTypeMap right) => object.Equals((object) left, (object) right);

    public DeviceTypeMap SetMapping(Guid interfaceClass, DeviceType deviceType) => new DeviceTypeMap((IEnumerable<KeyValuePair<Guid, DeviceType>>) this.mappings, interfaceClass, deviceType);

    public DeviceTypeMap SetMappings(IEnumerable<KeyValuePair<Guid, DeviceType>> range)
    {
      if (range == null)
        return this;
      return this.mappings == null ? new DeviceTypeMap(range) : new DeviceTypeMap(this.mappings.Union<KeyValuePair<Guid, DeviceType>>(range));
    }

    public DeviceTypeMap ClearMapping(Guid interfaceClass) => new DeviceTypeMap(this.mappings.Where<KeyValuePair<Guid, DeviceType>>((Func<KeyValuePair<Guid, DeviceType>, bool>) (m => m.Key != interfaceClass)));

    public DeviceTypeMap ClearMappings(params Guid[] range) => this.ClearMappings((IEnumerable<Guid>) range);

    public DeviceTypeMap ClearMappings(IEnumerable<Guid> range) => range == null || this.mappings == null ? this : new DeviceTypeMap(this.mappings.Except<KeyValuePair<Guid, DeviceType>>(this.mappings.Join<KeyValuePair<Guid, DeviceType>, Guid, Guid, KeyValuePair<Guid, DeviceType>>(range, (Func<KeyValuePair<Guid, DeviceType>, Guid>) (m => m.Key), (Func<Guid, Guid>) (m => m), (Func<KeyValuePair<Guid, DeviceType>, Guid, KeyValuePair<Guid, DeviceType>>) ((m, g) => m))));

    public bool TryGetMapping(Guid interfaceClass, out DeviceType deviceType) => this.mappings.TryGetValue(interfaceClass, out deviceType);

    public DeviceType GetMapping(Guid interfaceClass)
    {
      DeviceType deviceType;
      if (!this.mappings.TryGetValue(interfaceClass, out deviceType))
        throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.KeyNotFoundException_MessageFormat_DeviceTypeMappingNotFound, (object) interfaceClass));
      return deviceType;
    }

    public override bool Equals(object obj) => obj is DeviceTypeMap other && this.Equals(other);

    public bool Equals(DeviceTypeMap other)
    {
      if (this.IsEmpty)
        return other.IsEmpty;
      return !other.IsEmpty && this.mappings.Count == other.mappings.Count && this.mappings.OrderBy<KeyValuePair<Guid, DeviceType>, Guid>((Func<KeyValuePair<Guid, DeviceType>, Guid>) (m => m.Key)).SequenceEqual<KeyValuePair<Guid, DeviceType>>((IEnumerable<KeyValuePair<Guid, DeviceType>>) other.mappings.OrderBy<KeyValuePair<Guid, DeviceType>, Guid>((Func<KeyValuePair<Guid, DeviceType>, Guid>) (m => m.Key)));
    }

    public override int GetHashCode() => this.mappings == null ? 0 : this.mappings.OrderBy<KeyValuePair<Guid, DeviceType>, Guid>((Func<KeyValuePair<Guid, DeviceType>, Guid>) (m => m.Key)).GetHashCode();
  }
}
