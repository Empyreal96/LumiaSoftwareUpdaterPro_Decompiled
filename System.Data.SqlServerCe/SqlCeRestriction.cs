// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeRestriction
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Globalization;
using System.Reflection;

namespace System.Data.SqlServerCe
{
  internal class SqlCeRestriction
  {
    static SqlCeRestriction() => KillBitHelper.ThrowIfKillBitIsSet();

    private static bool IsWebHosted()
    {
      flag7 = false;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (assembly.GetName().Name == "System.Web" && assembly.GlobalAssemblyCache)
        {
          Type type = assembly.GetType("System.Web.Hosting.HostingEnvironment");
          if (type != null)
          {
            PropertyInfo property = type.GetProperty("IsHosted", typeof (bool));
            if (property != null)
            {
              object obj = property.GetValue((object) type, (object[]) null);
              if (obj == null || !(obj is bool flag7))
                break;
              break;
            }
            break;
          }
          break;
        }
      }
      return flag7;
    }

    private static bool IsExplicitlyEnabled()
    {
      flag = false;
      object data = AppDomain.CurrentDomain.GetData("SQLServerCompactEditionUnderWebHosting");
      if (data == null || !(data is bool flag))
        ;
      return flag;
    }

    public static void CheckExplicitWebHosting()
    {
      if (!SqlCeRestriction.IsExplicitlyEnabled() && SqlCeRestriction.IsWebHosted())
        throw new NotSupportedException(Res.GetString(CultureInfo.CurrentCulture, "SQLCE_WebHostingRestriction"));
    }
  }
}
