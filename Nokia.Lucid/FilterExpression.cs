// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.FilterExpression
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.Primitives;
using System;
using System.Linq.Expressions;

namespace Nokia.Lucid
{
  public static class FilterExpression
  {
    public static readonly Expression<Func<DeviceIdentifier, bool>> EmptyExpression = (Expression<Func<DeviceIdentifier, bool>>) (s => false);
    public static readonly Expression<Func<DeviceIdentifier, bool>> NoFilter = (Expression<Func<DeviceIdentifier, bool>>) (s => true);
    private static Expression<Func<DeviceIdentifier, bool>> defaultExpression = FilterExpression.CreateDefaultExpression();

    public static Expression<Func<DeviceIdentifier, bool>> DefaultExpression
    {
      get => FilterExpression.defaultExpression;
      set => FilterExpression.defaultExpression = value;
    }

    public static Expression<Func<DeviceIdentifier, bool>> CreateDefaultExpression() => (Expression<Func<DeviceIdentifier, bool>>) (s => s.Vid("0421") && (s.Pid("0660") && s.MI(new int[]
    {
      4,
      5,
      6
    }) || s.Pid("0661") && s.MI(new int[]{ 2, 3 }) || s.Pid("066E")) || s.Guid(WindowsPhoneIdentifiers.NcsdDeviceInterfaceGuid) || s.Guid(WindowsPhoneIdentifiers.TestServerDeviceInterfaceGuid) || s.Guid(WindowsPhoneIdentifiers.LabelAppDeviceInterfaceGuid) || s.Guid(WindowsPhoneIdentifiers.UefiDeviceInterfaceGuid) || s.Guid(WindowsPhoneIdentifiers.EdDeviceInterfaceGuid) || (s.Vid("0421") || s.Vid("045E")) && s.Guid(WindowsPhoneIdentifiers.GenericUsbDeviceInterfaceGuid));
  }
}
