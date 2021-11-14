// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Program
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;

namespace Microsoft.LsuPro
{
  public class Program
  {
    public static int Main(string[] args)
    {
      CommandLineParser commandLineParser = new CommandLineParser(args);
      int arguments = commandLineParser.ParseArguments();
      InstallationVerification installationVerification = new InstallationVerification();
      if (arguments > 0 && commandLineParser.SwitchIsSet("help"))
      {
        Program.PrintHelp();
        return 0;
      }
      if (commandLineParser.SwitchIsSet("createinstallationsnapshot"))
      {
        try
        {
          string optionValue1 = commandLineParser.GetOptionValue("installationscript");
          string optionValue2 = commandLineParser.GetOptionValue("applicationversion");
          if (string.IsNullOrEmpty(optionValue1))
          {
            Console.WriteLine("InstallationSnapshot failed: no installation script file.");
            return 1;
          }
          Console.WriteLine("InstallationSnapshot started: {0}, {1}", (object) optionValue1, (object) optionValue2);
          installationVerification.InstallationSnapshot(optionValue1, optionValue2);
          Console.WriteLine("InstallationSnapshot created.");
          return 0;
        }
        catch (Exception ex)
        {
          Console.WriteLine("InstallationSnapshot failed: {0}", (object) ex.Message);
          return 1;
        }
      }
      else
      {
        string optionValue = commandLineParser.GetOptionValue("installationsnapshot");
        try
        {
          return installationVerification.VerifyInstallation(optionValue);
        }
        catch (Exception ex)
        {
          Trace.Error("VerifyInstallation failed: {0}", (object) ex.Message);
          return 1;
        }
      }
    }

    internal static void PrintHelp()
    {
      Console.WriteLine("InstallationVerification.exe");
      Console.WriteLine("Parameters:");
      Console.WriteLine("-verifyinstallation                          Verify installation");
      Console.WriteLine("-installationsnapshot=[installationsnapshot] Installation snapshot");
      Console.WriteLine("-createinstallationsnapshot                  Create installation snapshot");
      Console.WriteLine("-installationscript=[installationscript]     Installation script");
      Console.WriteLine("-applicationversion=[applicationversion]     Application version");
    }
  }
}
