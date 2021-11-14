// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.BrowserDialogFactory
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal static class BrowserDialogFactory
  {
    internal static IWebUI CreateAuthenticationDialog(PromptBehavior promptBehavior)
    {
      switch (promptBehavior)
      {
        case PromptBehavior.Auto:
          return (IWebUI) new InteractiveWebUI();
        case PromptBehavior.Always:
        case PromptBehavior.RefreshSession:
          return (IWebUI) new InteractiveWebUI();
        case PromptBehavior.Never:
          return (IWebUI) new SilentWebUI();
        default:
          throw new InvalidOperationException("Unexpected PromptBehavior value");
      }
    }
  }
}
