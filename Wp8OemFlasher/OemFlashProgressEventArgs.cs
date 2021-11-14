// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OemFlashProgressEventArgs
// Assembly: Wp8OemFlasher, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0F564F-0EF5-4D78-8BB5-4C7A3BFE4321
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8OemFlasher.dll

using System;

namespace Microsoft.LsuPro
{
  public class OemFlashProgressEventArgs : EventArgs
  {
    public OemFlashProgressEventArgs(long position, long length, int percentage, long speed)
    {
      this.Position = position;
      this.Length = length;
      this.Percentage = percentage;
      this.Speed = speed;
    }

    public long Position { get; private set; }

    public long Length { get; private set; }

    public int Percentage { get; private set; }

    public long Speed { get; private set; }
  }
}
