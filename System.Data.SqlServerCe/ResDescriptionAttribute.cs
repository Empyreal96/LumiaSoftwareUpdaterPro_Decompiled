// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ResDescriptionAttribute
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.ComponentModel;

namespace System.Data.SqlServerCe
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class ResDescriptionAttribute : DescriptionAttribute
  {
    private bool replaced;

    public ResDescriptionAttribute(string description)
      : base(description)
    {
    }

    public override string Description
    {
      get
      {
        if (!this.replaced)
        {
          this.replaced = true;
          this.DescriptionValue = Res.GetString(base.Description);
        }
        return base.Description;
      }
    }
  }
}
