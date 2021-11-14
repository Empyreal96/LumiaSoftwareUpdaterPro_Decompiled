// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.CallState
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class CallState
  {
    public CallState(Guid correlationId, bool callSync)
    {
      this.CorrelationId = correlationId;
      this.CallSync = callSync;
    }

    public Guid CorrelationId { get; set; }

    public bool CallSync { get; private set; }

    public AuthorityType AuthorityType { get; internal set; }
  }
}
