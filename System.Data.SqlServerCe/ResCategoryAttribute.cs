// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ResCategoryAttribute
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.ComponentModel;

namespace System.Data.SqlServerCe
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class ResCategoryAttribute : CategoryAttribute
  {
    public ResCategoryAttribute(string category)
      : base(category)
    {
    }

    protected override string GetLocalizedString(string value) => Res.GetString(value);
  }
}
