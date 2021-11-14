// Decompiled with JetBrains decompiler
// Type: FFUComponents.DeviceEventProvider
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Diagnostics.Eventing;
using System.Runtime.InteropServices;

namespace FFUComponents
{
  internal class DeviceEventProvider : EventProvider
  {
    internal DeviceEventProvider(Guid id)
      : base(id)
    {
    }

    internal unsafe bool TemplateDeviceEvent(
      ref EventDescriptor eventDescriptor,
      Guid DeviceUniqueId,
      string DeviceFriendlyName,
      string AdditionalInfoString)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* numPtr = stackalloc byte[sizeof (DeviceEventProvider.EventData) * dataCount];
        DeviceEventProvider.EventData* eventDataPtr = (DeviceEventProvider.EventData*) numPtr;
        eventDataPtr->DataPointer = (ulong) &DeviceUniqueId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((DeviceFriendlyName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((AdditionalInfoString.Length + 1) * 2);
        fixed (char* chPtr1 = DeviceFriendlyName)
          fixed (char* chPtr2 = AdditionalInfoString)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) numPtr);
          }
      }
      return flag;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct EventData
    {
      [FieldOffset(0)]
      internal ulong DataPointer;
      [FieldOffset(8)]
      internal uint Size;
      [FieldOffset(12)]
      internal int Reserved;
    }
  }
}
