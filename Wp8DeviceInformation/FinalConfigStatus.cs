// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.FinalConfigStatus
// Assembly: Wp8DeviceInformation, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 6707A4BB-F60A-40D7-A2BC-1ABC64317FDD
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8DeviceInformation.dll

using System;
using System.Globalization;

namespace Microsoft.LsuPro
{
  public class FinalConfigStatus
  {
    public static string FinalConfigValueToString(bool? input)
    {
      if (!input.HasValue)
        return (string) null;
      bool? nullable = input;
      bool flag = true;
      return (nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) == 0 ? "Disabled" : "Enabled";
    }

    public static FinalConfigStatus Parse(string finalConfigStatus)
    {
      FinalConfigStatus finalConfigStatus1 = new FinalConfigStatus();
      try
      {
        if (!string.IsNullOrEmpty(finalConfigStatus))
        {
          FinalConfigStatus.FinalConfig int32 = (FinalConfigStatus.FinalConfig) Convert.ToInt32(finalConfigStatus, (IFormatProvider) CultureInfo.InvariantCulture);
          finalConfigStatus1.SecureBoot = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigSecureBoot));
          finalConfigStatus1.FfuVerify = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigFfuVerify));
          finalConfigStatus1.Jtag = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigJtag));
          finalConfigStatus1.Shk = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigShk));
          finalConfigStatus1.Simlock = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigSimlock));
          finalConfigStatus1.ProductionDone = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigProductionDone));
          finalConfigStatus1.Rkh = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigRkh));
          finalConfigStatus1.PublicId = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigPublicId));
          finalConfigStatus1.Dak = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigDak));
          finalConfigStatus1.SecGen = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigSecGen));
          finalConfigStatus1.OemId = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigOemId));
          finalConfigStatus1.FastBoot = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigFastBoot));
          finalConfigStatus1.SpdmSecMode = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigSpdmSecMode));
          finalConfigStatus1.RpmWdog = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigRpmWdog));
          finalConfigStatus1.Ssm = new bool?(int32.HasFlag((Enum) FinalConfigStatus.FinalConfig.FinalConfigSsm));
        }
      }
      catch (Exception ex)
      {
      }
      return finalConfigStatus1;
    }

    public bool? SecureBoot { get; private set; }

    public bool? FfuVerify { get; private set; }

    public bool? Jtag { get; private set; }

    public bool? Shk { get; private set; }

    public bool? Simlock { get; private set; }

    public bool? ProductionDone { get; private set; }

    public bool? Rkh { get; private set; }

    public bool? PublicId { get; private set; }

    public bool? Dak { get; private set; }

    public bool? SecGen { get; private set; }

    public bool? OemId { get; private set; }

    public bool? FastBoot { get; private set; }

    public bool? SpdmSecMode { get; private set; }

    public bool? RpmWdog { get; private set; }

    public bool? Ssm { get; private set; }

    [Flags]
    internal enum FinalConfig
    {
      FinalConfigSecureBoot = 1,
      FinalConfigFfuVerify = 2,
      FinalConfigJtag = 4,
      FinalConfigShk = 8,
      FinalConfigSimlock = 16, // 0x00000010
      FinalConfigProductionDone = 32, // 0x00000020
      FinalConfigRkh = 64, // 0x00000040
      FinalConfigPublicId = 128, // 0x00000080
      FinalConfigDak = 256, // 0x00000100
      FinalConfigSecGen = 512, // 0x00000200
      FinalConfigOemId = 1024, // 0x00000400
      FinalConfigFastBoot = 2048, // 0x00000800
      FinalConfigSpdmSecMode = 4096, // 0x00001000
      FinalConfigRpmWdog = 8192, // 0x00002000
      FinalConfigSsm = 16384, // 0x00004000
    }
  }
}
