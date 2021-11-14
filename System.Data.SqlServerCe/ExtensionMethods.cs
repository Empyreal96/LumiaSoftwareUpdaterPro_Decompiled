// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ExtensionMethods
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlServerCe
{
  internal static class ExtensionMethods
  {
    private static readonly string DataSqlServerCeEntityAssembly = "System.Data.SqlServerCe.Entity";
    private static readonly string DataSqlServerCeAssembly = "System.Data.SqlServerCe";
    private static readonly string DataEntityAssembly = "System.Data.Entity";
    private static string SystemDataCommonDbProviderServices_TypeName = Assembly.CreateQualifiedName(ExtensionMethods.GetFullAssemblyName(ExtensionMethods.DataEntityAssembly), "System.Data.Common.DbProviderServices");
    internal static Type SystemDataCommonDbProviderServices_Type = Type.GetType(ExtensionMethods.SystemDataCommonDbProviderServices_TypeName, false);
    private static string SqlCeProviderServices_TypeName = "System.Data.SqlServerCe.SqlCeProviderServices";
    private static FieldInfo SqlCeProviderServices_Instance_FieldInfo;

    static ExtensionMethods() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal static object SystemDataSqlServerCeSqlCeProviderServices_Instance()
    {
      if (ExtensionMethods.SqlCeProviderServices_Instance_FieldInfo == null)
      {
        string str = ExtensionMethods.ConstructFullAssemblyName(ExtensionMethods.DataSqlServerCeEntityAssembly);
        string qualifiedName = Assembly.CreateQualifiedName(str, ExtensionMethods.SqlCeProviderServices_TypeName);
        try
        {
          Assembly.Load(str);
        }
        catch (FileNotFoundException ex)
        {
          throw new FileNotFoundException(Res.GetString("SQLCE_CantLoadEntityDll"), (Exception) ex);
        }
        catch (FileLoadException ex)
        {
          throw new FileLoadException(Res.GetString("SQLCE_CantLoadEntityDll"), (Exception) ex);
        }
        catch (BadImageFormatException ex)
        {
          throw new BadImageFormatException(Res.GetString("SQLCE_CantLoadEntityDll"), (Exception) ex);
        }
        Type type = Type.GetType(qualifiedName, false);
        if (type != null)
          ExtensionMethods.SqlCeProviderServices_Instance_FieldInfo = type.GetField("Instance", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
      }
      return ExtensionMethods.SystemDataSqlServerCeSqlCeProviderServices_Instance_GetValue();
    }

    [SecurityCritical]
    [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
    private static object SystemDataSqlServerCeSqlCeProviderServices_Instance_GetValue()
    {
      object obj = (object) null;
      if (ExtensionMethods.SqlCeProviderServices_Instance_FieldInfo != null)
        obj = ExtensionMethods.SqlCeProviderServices_Instance_FieldInfo.GetValue((object) null);
      return obj;
    }

    private static string ConstructFullAssemblyName(string assemblyName)
    {
      if (assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
      return Assembly.GetExecutingAssembly().FullName.Replace(ExtensionMethods.DataSqlServerCeAssembly, assemblyName);
    }

    private static string GetFullAssemblyName(string assemblyName)
    {
      if (assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        AssemblyName assemblyName1 = new AssemblyName(assembly.FullName);
        if (string.Compare(assemblyName1.Name, assemblyName) == 0)
          return assemblyName1.FullName;
      }
      throw new ArgumentException(assemblyName);
    }
  }
}
