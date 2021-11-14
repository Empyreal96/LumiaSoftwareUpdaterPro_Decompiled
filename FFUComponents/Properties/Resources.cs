// Decompiled with JetBrains decompiler
// Type: FFUComponents.Properties.Resources
// Assembly: FFUComponents, Version=8.0.0.0, Culture=neutral, PublicKeyToken=5d653a1a5ba069fd
// MVID: 079409EC-FC99-4988-8EB4-20A87B1EBA8C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\FFUComponents.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace FFUComponents.Properties
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) FFUComponents.Properties.Resources.resourceMan, (object) null))
          FFUComponents.Properties.Resources.resourceMan = new ResourceManager("FFUComponents.Properties.Resources", typeof (FFUComponents.Properties.Resources).Assembly);
        return FFUComponents.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => FFUComponents.Properties.Resources.resourceCulture;
      set => FFUComponents.Properties.Resources.resourceCulture = value;
    }

    internal static byte[] bootsdi => (byte[]) FFUComponents.Properties.Resources.ResourceManager.GetObject(nameof (bootsdi), FFUComponents.Properties.Resources.resourceCulture);
  }
}
