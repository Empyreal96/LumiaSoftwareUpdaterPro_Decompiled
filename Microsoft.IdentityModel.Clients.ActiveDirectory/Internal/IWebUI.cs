// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.IWebUI
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.ComponentModel;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IWebUI
  {
    object OwnerWindow { get; set; }

    string Authenticate(Uri requestUri, Uri callbackUri);
  }
}
