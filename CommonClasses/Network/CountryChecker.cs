// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Network.CountryChecker
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

namespace Microsoft.LsuPro.Network
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
  public static class CountryChecker
  {
    private static string queryUrl = "http://api.hostip.info/get_json.php";
    private static bool isChecked;

    public static string LocationInfo { get; private set; }

    public static void RetrieveLocationInfo(IWebProxy proxy = null)
    {
      if (CountryChecker.isChecked)
        return;
      try
      {
        WebClient webClient = new WebClient();
        if (proxy != null)
          webClient.Proxy = proxy;
        JsonLocationInfo jsonLocationInfo = JsonConvert.DeserializeObject<JsonLocationInfo>(webClient.DownloadString(CountryChecker.queryUrl));
        CountryChecker.LocationInfo = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) jsonLocationInfo.City, (object) jsonLocationInfo.CountryCode);
      }
      catch (Exception ex)
      {
        CountryChecker.LocationInfo = "Unknown/Unknown";
      }
      Tracer.Information("Location info is '{0}'", (object) CountryChecker.LocationInfo);
      CountryChecker.isChecked = true;
    }
  }
}
