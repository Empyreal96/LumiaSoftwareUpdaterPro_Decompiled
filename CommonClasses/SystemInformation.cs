// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SystemInformation
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management;

namespace Microsoft.LsuPro
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class SystemInformation
  {
    static SystemInformation()
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      try
      {
        foreach (ManagementObject managementObject in new ManagementObjectSearcher(new ManagementScope("root\\CIMV2"), new ObjectQuery("Select * From Win32_Processor")).Get())
        {
          SystemInformation.ProcessorName = (string) managementObject.GetPropertyValue("Name");
          SystemInformation.ProcessorClockFrequencyMHz = (uint) managementObject.GetPropertyValue("MaxClockSpeed");
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "Error getting Win32_Processor: {0}", (object) ex.Message);
      }
      try
      {
        foreach (ManagementObject managementObject in new ManagementObjectSearcher(new ManagementScope("root\\CIMV2"), new ObjectQuery("Select * From Win32_ComputerSystem")).Get())
        {
          SystemInformation.NumberOfLogicalProcessors = (uint) managementObject.GetPropertyValue(nameof (NumberOfLogicalProcessors));
          SystemInformation.NumberOfProcessors = (uint) managementObject.GetPropertyValue(nameof (NumberOfProcessors));
          SystemInformation.PcSystemType = (ushort) managementObject.GetPropertyValue("PCSystemType");
          SystemInformation.TotalPhysicalMemory = (ulong) managementObject.GetPropertyValue(nameof (TotalPhysicalMemory)) / 1024UL / 1024UL;
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "Error getting Win32_ComputerSystem: {0}", (object) ex.Message);
      }
      try
      {
        foreach (ManagementObject managementObject in new ManagementObjectSearcher(new ManagementScope("root\\CIMV2"), new ObjectQuery("Select * From Win32_OperatingSystem")).Get())
        {
          SystemInformation.FreePhysicalMemory = (ulong) managementObject.GetPropertyValue(nameof (FreePhysicalMemory)) / 1024UL;
          SystemInformation.FreeVirtualMemory = (ulong) managementObject.GetPropertyValue(nameof (FreeVirtualMemory)) / 1024UL;
          SystemInformation.OsLanguage = (uint) managementObject.GetPropertyValue("OSLanguage");
          SystemInformation.TotalVirtualMemorySize = (ulong) managementObject.GetPropertyValue(nameof (TotalVirtualMemorySize)) / 1024UL;
          SystemInformation.TotalVisibleMemorySize = (ulong) managementObject.GetPropertyValue(nameof (TotalVisibleMemorySize)) / 1024UL;
        }
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "Error getting Win32_OperatingSystem: {0}", (object) ex.Message);
      }
      Tracer.Information("System info read in {0}", (object) stopwatch.Elapsed);
    }

    public static string ProcessorName { get; private set; }

    public static uint ProcessorClockFrequencyMHz { get; private set; }

    public static uint NumberOfLogicalProcessors { get; private set; }

    public static uint NumberOfProcessors { get; private set; }

    public static ushort PcSystemType { get; private set; }

    public static ulong TotalPhysicalMemory { get; private set; }

    public static ulong FreePhysicalMemory { get; private set; }

    public static ulong FreeVirtualMemory { get; private set; }

    public static uint OsLanguage { get; private set; }

    public static ulong TotalVirtualMemorySize { get; private set; }

    public static ulong TotalVisibleMemorySize { get; private set; }
  }
}
