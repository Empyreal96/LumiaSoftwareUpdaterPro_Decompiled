// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.WebUIFactory
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class WebUIFactory : IWebUIFactory
  {
    private const string WebAuthenticationDialogAssemblyNameTemplate = "Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version={0}, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
    private static MethodInfo dialogFactory;

    public static void ThrowIfUIAssemblyUnavailable() => WebUIFactory.InitializeFactoryMethod();

    public IWebUI Create(PromptBehavior promptBehavior, object ownerWindow)
    {
      WebUIFactory.InitializeFactoryMethod();
      object[] parameters = new object[1]
      {
        (object) promptBehavior
      };
      IWebUI webUi = (IWebUI) WebUIFactory.dialogFactory.Invoke((object) null, parameters);
      webUi.OwnerWindow = ownerWindow;
      return webUi;
    }

    private static void InitializeFactoryMethod()
    {
      if ((MethodInfo) null != WebUIFactory.dialogFactory)
        return;
      string str = string.Format("Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version={0}, Culture=neutral, PublicKeyToken=31bf3856ad364e35", (object) AdalIdHelper.GetAdalVersion());
      try
      {
        WebUIFactory.dialogFactory = Assembly.Load(str).GetType("Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.BrowserDialogFactory").GetMethod("CreateAuthenticationDialog", BindingFlags.Static | BindingFlags.NonPublic);
      }
      catch (FileNotFoundException ex)
      {
        WebUIFactory.ThrowAssemlyLoadFailedException(str, (Exception) ex);
      }
      catch (FileLoadException ex)
      {
        WebUIFactory.ThrowAssemlyLoadFailedException(str, (Exception) ex);
      }
    }

    private static void ThrowAssemlyLoadFailedException(
      string webAuthenticationDialogAssemblyName,
      Exception innerException)
    {
      throw new AdalException("assembly_load_failed", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Loading an assembly required for interactive user authentication failed. Make sure assembly '{0}' exists", (object) webAuthenticationDialogAssemblyName), innerException);
    }
  }
}
