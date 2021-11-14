// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.Properties.Resources
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Nokia.Lucid.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
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
        if (object.ReferenceEquals((object) Nokia.Lucid.Properties.Resources.resourceMan, (object) null))
          Nokia.Lucid.Properties.Resources.resourceMan = new ResourceManager("Nokia.Lucid.Properties.Resources", typeof (Nokia.Lucid.Properties.Resources).Assembly);
        return Nokia.Lucid.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Nokia.Lucid.Properties.Resources.resourceCulture;
      set => Nokia.Lucid.Properties.Resources.resourceCulture = value;
    }

    internal static string InvalidOperationException_MessageFormat_CouldNotRetrieveDeviceInfo => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (InvalidOperationException_MessageFormat_CouldNotRetrieveDeviceInfo), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string InvalidOperationException_MessageFormat_CouldNotStartDeviceWatcher => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (InvalidOperationException_MessageFormat_CouldNotStartDeviceWatcher), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string InvalidOperationException_MessageText_CallingThreadDoesNotHaveAccessToThisMessageWindowInstance => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (InvalidOperationException_MessageText_CallingThreadDoesNotHaveAccessToThisMessageWindowInstance), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string InvalidOperationException_MessageText_CouldNotEndThreadAffinity => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (InvalidOperationException_MessageText_CouldNotEndThreadAffinity), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string InvalidOperationException_MessageText_CoundNotEndCriticalRegion => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (InvalidOperationException_MessageText_CoundNotEndCriticalRegion), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string InvalidOperationException_MessageText_ExceptionAlreadyMarkedAsHandled => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (InvalidOperationException_MessageText_ExceptionAlreadyMarkedAsHandled), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string KeyNotFoundException_MessageFormat_DeviceTypeMappingNotFound => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (KeyNotFoundException_MessageFormat_DeviceTypeMappingNotFound), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string KeyNotFoundException_MessageFormat_PropertyKeyMappingNotFound => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (KeyNotFoundException_MessageFormat_PropertyKeyMappingNotFound), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string KeyNotFoundException_MessageFormat_PropertyNotFound => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (KeyNotFoundException_MessageFormat_PropertyNotFound), Nokia.Lucid.Properties.Resources.resourceCulture);

    internal static string NotSupportedException_MessageFormat_PropertyTypeNotSupported => Nokia.Lucid.Properties.Resources.ResourceManager.GetString(nameof (NotSupportedException_MessageFormat_PropertyTypeNotSupported), Nokia.Lucid.Properties.Resources.resourceCulture);
  }
}
