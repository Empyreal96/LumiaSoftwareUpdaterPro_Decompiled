// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.InteractiveWebUI
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal class InteractiveWebUI : WebUI
  {
    private WindowsFormsWebAuthenticationDialog dialog;

    protected override string OnAuthenticate()
    {
      using (this.dialog = new WindowsFormsWebAuthenticationDialog(this.OwnerWindow))
        return this.dialog.AuthenticateAAD(this.RequestUri, this.CallbackUri);
    }
  }
}
