// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalTrace
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Diagnostics;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public static class AdalTrace
  {
    static AdalTrace()
    {
      AdalTrace.TraceSource = new TraceSource("Microsoft.IdentityModel.Clients.ActiveDirectory", SourceLevels.All);
      AdalTrace.LegacyTraceSwitch = new TraceSwitch("ADALLegacySwitch", "ADAL Switch for System.Diagnostics.Trace", "Verbose");
    }

    public static TraceSource TraceSource { get; private set; }

    public static TraceSwitch LegacyTraceSwitch { get; private set; }
  }
}
