// Decompiled with JetBrains decompiler
// Type: FFUComponents.FlashingDeviceLogger
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System;
using System.Diagnostics.Eventing;
using System.IO;

namespace FFUComponents
{
  public class FlashingDeviceLogger : IDisposable
  {
    internal DeviceEventProvider m_provider = new DeviceEventProvider(new Guid("3bbd891e-180f-4386-94b5-d71ba7ac25a9"));

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.m_provider.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public bool LogDeviceEvent(
      byte[] logData,
      Guid deviceUniqueId,
      string deviceFriendlyName,
      out string errInfo)
    {
      BinaryReader binaryReader = new BinaryReader((Stream) new MemoryStream(logData));
      int num = (int) binaryReader.ReadByte();
      int id = (int) binaryReader.ReadInt16();
      byte version = binaryReader.ReadByte();
      byte channel = binaryReader.ReadByte();
      byte level = binaryReader.ReadByte();
      byte opcode = binaryReader.ReadByte();
      int task = (int) binaryReader.ReadInt16();
      long keywords = binaryReader.ReadInt64();
      EventDescriptor eventDescriptor = new EventDescriptor(id, version, channel, level, opcode, task, keywords);
      string AdditionalInfoString = binaryReader.ReadString();
      if (level <= (byte) 2)
      {
        errInfo = string.Format("{{ 0x{0:x}, 0x{1:x}, 0x{2:x}, 0x{3:x}, 0x{4:x}, 0x{5:x} }}", (object) id, (object) version, (object) channel, (object) level, (object) opcode, (object) task);
        if (AdditionalInfoString != "")
        {
          ref string local = ref errInfo;
          local = local + " : " + AdditionalInfoString;
        }
      }
      else
        errInfo = "";
      return this.m_provider.TemplateDeviceEvent(ref eventDescriptor, deviceUniqueId, deviceFriendlyName, AdditionalInfoString);
    }
  }
}
