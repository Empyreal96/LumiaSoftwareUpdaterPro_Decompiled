// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemException
// Assembly: NvItemModule, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0B184167-245E-49B5-887C-F5F0E401EE86
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\NvItemModule.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "reviewed")]
  [Serializable]
  public class NvItemException : Exception
  {
    public NvItemException(string message, string nvItem)
    {
      this.Message = message;
      this.NvItem = nvItem;
    }

    public new string Message { get; private set; }

    public string NvItem { get; private set; }
  }
}
