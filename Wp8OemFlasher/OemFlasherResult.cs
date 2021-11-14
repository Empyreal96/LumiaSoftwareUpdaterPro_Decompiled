// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OemFlasherResult
// Assembly: Wp8OemFlasher, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: DD0F564F-0EF5-4D78-8BB5-4C7A3BFE4321
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8OemFlasher.dll

using FFUComponents;
using System;

namespace Microsoft.LsuPro
{
  public class OemFlasherResult
  {
    internal OemFlasherResult(Exception exception) => this.Exception = exception;

    public bool Success => this.Exception == null;

    public Exception Exception { get; private set; }

    public bool FailureIsFfuException => !this.Success && this.Exception is FFUException;
  }
}
