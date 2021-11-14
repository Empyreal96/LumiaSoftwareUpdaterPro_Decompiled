// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceInformation.DevicePropertySetExtensions
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;

namespace Nokia.Lucid.DeviceInformation
{
  public static class DevicePropertySetExtensions
  {
    public static string ReadName(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(3072717104U, (ushort) 18415, (ushort) 4122, (byte) 165, (byte) 241, (byte) 2, (byte) 96, (byte) 140, (byte) 158, (byte) 235, (byte) 172, 10), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDescription(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadHardwareIds(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadCompatibleIds(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 4), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadService(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 6), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadSetupClassName(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 9), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static Guid ReadSetupClass(this IDevicePropertySet propertySet) => (Guid) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 10), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverName(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 11), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadConfiguration(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 12), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadManufacturer(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 13), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadFriendlyName(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 14), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadLocationInformation(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 15), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadPhysicalDeviceObjectName(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 16), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadCapabilities(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 17), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadUINumber(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 18), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadUpperFilters(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 19), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadLowerFilters(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 20), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static Guid ReadBusType(this IDevicePropertySet propertySet) => (Guid) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 21), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadLegacyBusType(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 22), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadBusNumber(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 23), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadEnumerator(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 24), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static byte[] ReadSecurityDescriptor(this IDevicePropertySet propertySet) => (byte[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 25), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadSecurityDescriptorString(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 26), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadDevType(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 27), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadExclusive(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 28), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadCharacteristics(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 29), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadAddress(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 30), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadUINumberDescriptionFormat(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 31), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static byte[] ReadPowerData(this IDevicePropertySet propertySet) => (byte[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 32), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadRemovalPolicy(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 33), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadRemovalPolicyDefault(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 34), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadRemovalPolicyOverride(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 35), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadInstallState(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 36), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadLocationPaths(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 37), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static Guid ReadBaseContainerId(this IDevicePropertySet propertySet) => (Guid) propertySet.ReadProperty(new PropertyKey(2757502286U, (ushort) 57116, (ushort) 20221, (byte) 128, (byte) 32, (byte) 103, (byte) 209, (byte) 70, (byte) 168, (byte) 80, (byte) 224, 38), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadInstanceStatus(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadProblemCode(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadEjectionRelations(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 4), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadRemovalRelations(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 5), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadPowerRelations(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 6), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadBusRelations(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 7), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadParentInstanceId(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 8), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadChildInstanceIds(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 9), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadSiblingInstanceIds(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 10), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadTransportRelations(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(1128310469U, (ushort) 37882, (ushort) 18182, (byte) 151, (byte) 44, (byte) 123, (byte) 100, (byte) 128, (byte) 8, (byte) 165, (byte) 167, 11), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsReported(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2152296704U, (ushort) 35955, (ushort) 18617, (byte) 170, (byte) 217, (byte) 206, (byte) 56, (byte) 126, (byte) 25, (byte) 197, (byte) 110, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsLegacy(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2152296704U, (ushort) 35955, (ushort) 18617, (byte) 170, (byte) 217, (byte) 206, (byte) 56, (byte) 126, (byte) 25, (byte) 197, (byte) 110, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadInstanceId(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2026065864, (short) 4170, (short) 19146, (byte) 158, (byte) 164, (byte) 82, (byte) 77, (byte) 82, (byte) 153, (byte) 110, (byte) 87, 256), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static Guid ReadContainerId(this IDevicePropertySet propertySet) => (Guid) propertySet.ReadProperty(new PropertyKey(2357121542U, (ushort) 16266, (ushort) 18471, (byte) 179, (byte) 171, (byte) 174, (byte) 158, (byte) 31, (byte) 174, (byte) 252, (byte) 108, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static Guid ReadModelId(this IDevicePropertySet propertySet) => (Guid) propertySet.ReadProperty(new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadFriendlyNameAttributes(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadManufacturerAttributes(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 4), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsPresenceNotForDevice(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2161647270U, (ushort) 29811, (ushort) 19212, (byte) 130, (byte) 22, (byte) 239, (byte) 193, (byte) 26, (byte) 44, (byte) 76, (byte) 139, 5), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadNonUniformMemoryArchitectureProximityDomain(
      this IDevicePropertySet propertySet)
    {
      return (int) propertySet.ReadProperty(new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 1), (IPropertyValueFormatter) PropertyValueFormatter.Default);
    }

    public static int ReadDynamicHardwarePartitioningRebalancePolicy(
      this IDevicePropertySet propertySet)
    {
      return (int) propertySet.ReadProperty(new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);
    }

    public static int ReadNonUniformMemoryArchitectureNode(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadBusReportedDeviceDescription(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(1410045054U, (ushort) 35648, (ushort) 17852, (byte) 168, (byte) 162, (byte) 106, (byte) 11, (byte) 137, (byte) 76, (byte) 189, (byte) 162, 4), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadSessionId(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 6), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static DateTime ReadInstallDate(this IDevicePropertySet propertySet) => (DateTime) propertySet.ReadProperty(new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 100), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static DateTime ReadFirstInstallDate(this IDevicePropertySet propertySet) => (DateTime) propertySet.ReadProperty(new PropertyKey(2212127526U, (ushort) 38822, (ushort) 16520, (byte) 148, (byte) 83, (byte) 161, (byte) 146, (byte) 63, (byte) 87, (byte) 59, (byte) 41, 101), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static DateTime ReadDriverDate(this IDevicePropertySet propertySet) => (DateTime) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverVersion(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverDescription(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 4), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverInfPath(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 5), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverInfSection(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 6), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverInfSectionExt(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 7), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadMatchingDeviceId(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 8), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverProvider(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 9), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadDriverPropPageProvider(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 10), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string[] ReadDriverCoInstallers(this IDevicePropertySet propertySet) => (string[]) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 11), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadResourcePickerTags(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 12), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static string ReadResourcePickerExceptions(this IDevicePropertySet propertySet) => (string) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 13), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadDriverRank(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 14), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static int ReadDriverLogoLevel(this IDevicePropertySet propertySet) => (int) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 15), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsNoConnectSound(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 17), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsGenericDriverInstalled(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 18), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsAdditionalSoftwareRequested(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2830656989U, (ushort) 11837, (ushort) 16532, (byte) 173, (byte) 151, (byte) 229, (byte) 147, (byte) 167, (byte) 12, (byte) 117, (byte) 214, 19), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsSafeRemovalRequired(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2950264384U, (ushort) 34467, (ushort) 16912, (byte) 182, (byte) 124, (byte) 40, (byte) 156, (byte) 65, (byte) 170, (byte) 190, (byte) 85, 2), (IPropertyValueFormatter) PropertyValueFormatter.Default);

    public static bool ReadIsSafeRemovalRequiredOverride(this IDevicePropertySet propertySet) => (bool) propertySet.ReadProperty(new PropertyKey(2950264384U, (ushort) 34467, (ushort) 16912, (byte) 182, (byte) 124, (byte) 40, (byte) 156, (byte) 65, (byte) 170, (byte) 190, (byte) 85, 3), (IPropertyValueFormatter) PropertyValueFormatter.Default);
  }
}
