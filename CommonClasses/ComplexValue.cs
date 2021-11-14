// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ComplexValue
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

namespace Microsoft.LsuPro
{
  public class ComplexValue
  {
    public ComplexValue(bool bitmaskState, string valueName, string valueDescription)
    {
      this.BitmaskState = bitmaskState;
      this.ValueName = valueName;
      this.ValueDescription = valueDescription;
    }

    public bool BitmaskState { get; set; }

    public string ValueName { get; private set; }

    public string ValueDescription { get; private set; }
  }
}
