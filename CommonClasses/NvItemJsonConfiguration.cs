// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NvItemJsonConfiguration
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System.Collections.Generic;

namespace Microsoft.LsuPro
{
  internal class NvItemJsonConfiguration
  {
    internal NvItemJsonConfiguration(string itemIdentifier) => this.ItemIdentifier = itemIdentifier;

    internal string XpathToItemDescription { get; set; }

    internal Dictionary<int, string> XpathToItemValueDescriptions { get; set; }

    internal string Convert { get; set; }

    internal string ItemIdentifier { get; private set; }
  }
}
