// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.NokiaNetwork
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Microsoft.LsuPro
{
  public static class NokiaNetwork
  {
    private static bool isInside;
    private static bool isChecked;
    private static bool forcedInside;

    public static void SetForcedInside()
    {
      Tracer.Information("Forced inside mode set.");
      NokiaNetwork.forcedInside = true;
    }

    public static void SetForcedOutside()
    {
      Tracer.Information("Forced outside mode set.");
      NokiaNetwork.forcedInside = false;
      NokiaNetwork.isChecked = true;
      NokiaNetwork.isInside = false;
    }

    public static bool IsInside()
    {
      if (!NokiaNetwork.isChecked)
      {
        NokiaNetwork.isInside = NokiaNetwork.DoInsideCheck();
        NokiaNetwork.isChecked = true;
      }
      if (NokiaNetwork.forcedInside)
      {
        Tracer.Information("Forced inside mode is set.");
        return true;
      }
      Tracer.Information("Result is {0}.", (object) NokiaNetwork.isInside);
      return NokiaNetwork.isInside;
    }

    private static bool DoInsideCheck()
    {
      try
      {
        NokiaNetwork.GetHostEntryHandler hostEntryHandler = new NokiaNetwork.GetHostEntryHandler(Dns.GetHostEntry);
        IAsyncResult result = hostEntryHandler.BeginInvoke("nokes.nokia.com", (AsyncCallback) null, (object) null);
        for (int index = 1; index <= 3; ++index)
        {
          if (result.AsyncWaitHandle.WaitOne(200 * index, false))
          {
            hostEntryHandler.EndInvoke(result);
            Tracer.Information("Successfully checked on try {0}.", (object) index);
            return true;
          }
        }
        Tracer.Information("Timeout occured while checking Nokia network.");
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Information(ex, "Error occured while checking Nokia network.", objArray);
      }
      string uri = Path.Combine(SpecialFolders.Root, "LumiaSoftwareUpdaterPro.config");
      try
      {
        return XDocument.Load(uri).Elements((XName) "LsuProSettings").Any<XElement>((Func<XElement, bool>) (x => x.Element((XName) "IsInsideNokiaNetwork").Value == "true"));
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Information(ex, "DoInsideCheck caught an exception when parsing settings file.", objArray);
        return false;
      }
    }

    private delegate IPHostEntry GetHostEntryHandler(string hostNameOrAddress);
  }
}
