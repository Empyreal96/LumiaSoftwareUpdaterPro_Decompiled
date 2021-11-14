// Decompiled with JetBrains decompiler
// Type: SelfExtractingPackageCreator.Properties.Resource
// Assembly: SelfExtractingPackageCreator, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 15D44AF4-CB22-48AD-A5ED-B49E916EE3E9
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SelfExtractingPackageCreator.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SelfExtractingPackageCreator.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resource
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resource()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (Resource.resourceMan == null)
          Resource.resourceMan = new ResourceManager("SelfExtractingPackageCreator.Properties.Resource", typeof (Resource).Assembly);
        return Resource.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => Resource.resourceCulture;
      set => Resource.resourceCulture = value;
    }

    public static Icon SelfExtractorIcon => (Icon) Resource.ResourceManager.GetObject(nameof (SelfExtractorIcon), Resource.resourceCulture);
  }
}
