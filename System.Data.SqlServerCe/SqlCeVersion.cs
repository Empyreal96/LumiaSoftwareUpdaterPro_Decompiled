// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeVersion
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal sealed class SqlCeVersion
  {
    public const string FileVersion = "4.0.8876.1";
    public const string AssemblyVersion = "4.0.0.1";
    public const string ProductVersion = "4.0";
    public const string ProductName = "Microsoft® SQL Server® Compact";
    private static readonly Version _fileVersion = new Version("4.0.8876.1");
    private static readonly Version _productVersion = new Version("4.0");

    public static int ProductMajor => SqlCeVersion._fileVersion.Major;

    public static int ProductMinor => SqlCeVersion._fileVersion.Minor;

    public static int BuildMajor => SqlCeVersion._fileVersion.Build;

    public static int BuildMinor => SqlCeVersion._fileVersion.Revision;

    public static int VersionSuffix => int.Parse(SqlCeVersion._fileVersion.Major.ToString() + SqlCeVersion._fileVersion.Minor.ToString());

    public static int ServicePackLevel => SqlCeVersion._productVersion.Build;
  }
}
