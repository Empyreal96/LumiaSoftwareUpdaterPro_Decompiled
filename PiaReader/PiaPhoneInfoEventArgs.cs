// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.PiaPhoneInfoEventArgs
// Assembly: PiaReader, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 27422629-2045-43F0-B2EB-AE7A366F98F1
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\PiaReader.dll

using System;

namespace Microsoft.LsuPro
{
  public class PiaPhoneInfoEventArgs : EventArgs
  {
    public PiaPhoneInfoEventArgs(PiaPhoneInfo piaPhoneInfo) => this.PiaPhoneInfo = piaPhoneInfo;

    public PiaPhoneInfo PiaPhoneInfo { get; private set; }
  }
}
