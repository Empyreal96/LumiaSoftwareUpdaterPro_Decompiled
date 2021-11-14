// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SoftwareRepository.SrdfItem
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro.SoftwareRepository
{
  public class SrdfItem
  {
    public SrdfItemQuery Query { get; set; }

    public List<string> Condition { get; set; }

    public List<string> Response { get; set; }
  }
}
