// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.NetworkPlugin
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal static class NetworkPlugin
  {
    static NetworkPlugin() => NetworkPlugin.SetToDefault();

    public static IWebUIFactory WebUIFactory { get; set; }

    public static IHttpWebRequestFactory HttpWebRequestFactory { get; set; }

    public static IRequestCreationHelper RequestCreationHelper { get; set; }

    public static void SetToDefault()
    {
      NetworkPlugin.WebUIFactory = (IWebUIFactory) new Microsoft.IdentityModel.Clients.ActiveDirectory.WebUIFactory();
      NetworkPlugin.HttpWebRequestFactory = (IHttpWebRequestFactory) new Microsoft.IdentityModel.Clients.ActiveDirectory.HttpWebRequestFactory();
      NetworkPlugin.RequestCreationHelper = (IRequestCreationHelper) new Microsoft.IdentityModel.Clients.ActiveDirectory.RequestCreationHelper();
    }
  }
}
