// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Properties.Resources
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GalaSoft.MvvmLight.Properties
{
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) GalaSoft.MvvmLight.Properties.Resources.resourceMan, (object) null))
          GalaSoft.MvvmLight.Properties.Resources.resourceMan = new ResourceManager("GalaSoft.MvvmLight.Properties.Resources", typeof (GalaSoft.MvvmLight.Properties.Resources).Assembly);
        return GalaSoft.MvvmLight.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => GalaSoft.MvvmLight.Properties.Resources.resourceCulture;
      set => GalaSoft.MvvmLight.Properties.Resources.resourceCulture = value;
    }

    internal static string AnInterfaceCannotBeRegisteredAlone => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (AnInterfaceCannotBeRegisteredAlone), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string CannotBuildInstance => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (CannotBuildInstance), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string ClassIsAlreadyRegistered => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (ClassIsAlreadyRegistered), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string ClassIsAlreadyRegisteredWithKey => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (ClassIsAlreadyRegisteredWithKey), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string ThereIsAlreadyAClassRegisteredFor => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (ThereIsAlreadyAClassRegisteredFor), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string ThereIsAlreadyAFactoryRegisteredFor => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (ThereIsAlreadyAFactoryRegisteredFor), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string ThereIsAlreadyAFactoryRegisteredForSameKey => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (ThereIsAlreadyAFactoryRegisteredForSameKey), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string TypeNotFoundInCache => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (TypeNotFoundInCache), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);

    internal static string TypeNotFoundInCacheKeyLess => GalaSoft.MvvmLight.Properties.Resources.ResourceManager.GetString(nameof (TypeNotFoundInCacheKeyLess), GalaSoft.MvvmLight.Properties.Resources.resourceCulture);
  }
}
