// Decompiled with JetBrains decompiler
// Type: FFUComponents.EventProviderVersionTwo
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Diagnostics.Eventing;
using System.Runtime.InteropServices;

namespace FFUComponents
{
  internal class EventProviderVersionTwo : EventProvider
  {
    internal EventProviderVersionTwo(Guid id)
      : base(id)
    {
    }

    internal unsafe bool TemplateDeviceSpecificEventWithString(
      ref EventDescriptor eventDescriptor,
      Guid DeviceId,
      string DeviceFriendlyName,
      string AssemblyFileVersion)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* numPtr = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) numPtr;
        eventDataPtr->DataPointer = (ulong) &DeviceId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((DeviceFriendlyName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((AssemblyFileVersion.Length + 1) * 2);
        fixed (char* chPtr1 = DeviceFriendlyName)
          fixed (char* chPtr2 = AssemblyFileVersion)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) numPtr);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateDeviceSpecificEvent(
      ref EventDescriptor eventDescriptor,
      Guid DeviceId,
      string DeviceFriendlyName)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* numPtr = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) numPtr;
        eventDataPtr->DataPointer = (ulong) &DeviceId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((DeviceFriendlyName.Length + 1) * 2);
        fixed (char* chPtr = DeviceFriendlyName)
        {
          eventDataPtr[1].DataPointer = (ulong) chPtr;
          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) numPtr);
        }
      }
      return flag;
    }

    internal unsafe bool TemplateDeviceEventWithErrorCode(
      ref EventDescriptor eventDescriptor,
      Guid DeviceId,
      string DeviceFriendlyName,
      int ErrorCode)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* numPtr = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) numPtr;
        eventDataPtr->DataPointer = (ulong) &DeviceId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((DeviceFriendlyName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &ErrorCode;
        eventDataPtr[2].Size = 4U;
        fixed (char* chPtr = DeviceFriendlyName)
        {
          eventDataPtr[1].DataPointer = (ulong) chPtr;
          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) numPtr);
        }
      }
      return flag;
    }

    internal unsafe bool TemplateNotifyException(
      ref EventDescriptor eventDescriptor,
      string DevicePath,
      string Exception)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* numPtr = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) numPtr;
        eventDataPtr->Size = (uint) ((DevicePath.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((Exception.Length + 1) * 2);
        fixed (char* chPtr1 = DevicePath)
          fixed (char* chPtr2 = Exception)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) numPtr);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateDeviceSpecificEventWithSize(
      ref EventDescriptor eventDescriptor,
      Guid DeviceId,
      string DeviceFriendlyName,
      int TransferSize)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* numPtr = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) numPtr;
        eventDataPtr->DataPointer = (ulong) &DeviceId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((DeviceFriendlyName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &TransferSize;
        eventDataPtr[2].Size = 4U;
        fixed (char* chPtr = DeviceFriendlyName)
        {
          eventDataPtr[1].DataPointer = (ulong) chPtr;
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
