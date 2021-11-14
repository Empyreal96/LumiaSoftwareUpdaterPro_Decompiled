// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SoftwareRepository.SoftwareRepositoryDiscovery
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using SoftwareRepository.Discovery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.SoftwareRepository
{
  public class SoftwareRepositoryDiscovery
  {
    private Discoverer msrDiscoverer;

    public event EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> SoftwareRepositoryDiscoveryEvent;

    public SoftwareRepositoryDiscovery(
      IWebProxy proxy,
      string accessToken,
      string alternativeBaseUrl = null)
    {
      this.msrDiscoverer = new Discoverer()
      {
        SoftwareRepositoryUserAgent = "LsuPro",
        SoftwareRepositoryProxy = proxy,
        SoftwareRepositoryAuthenticationToken = accessToken
      };
      if (!string.IsNullOrEmpty(alternativeBaseUrl))
        this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl = alternativeBaseUrl;
      Tracer.Information("Software Repository discovery base url: {0}", (object) alternativeBaseUrl);
    }

    public void SetAccessToken(string accessToken) => this.msrDiscoverer.SoftwareRepositoryAuthenticationToken = accessToken;

    public void SetWebProxy(IWebProxy getWebProxy) => this.msrDiscoverer.SoftwareRepositoryProxy = getWebProxy;

    public async Task GetListOfAllVariantsSlow(string productType)
    {
      Tracer.Information("GetListOfAllVariantsSlow for {0}", (object) productType);
      await this.ExecuteGetListOfAllVariantsSlow(productType, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken == null)
        return;
      await this.ExecuteGetListOfAllVariantsSlow(productType, "Testing");
    }

    private async Task ExecuteGetListOfAllVariantsSlow(
      string typeDesignator,
      string discoveryClass)
    {
      Discoverer d = new Discoverer();
      d.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      d.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      d.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      d.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      d.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages packages = await this.GetProductCodesSlow(d, typeDesignator, discoveryClass);
      Tracer.Information("Get product codes discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      List<Task<SoftwarePackages>> getSoftwareVersions = new List<Task<SoftwarePackages>>();
      int nextIndex;
      for (nextIndex = 0; nextIndex < 6 && nextIndex < packages.SoftwarePackageList.Count; ++nextIndex)
      {
        foreach (string productCode in packages.SoftwarePackageList[nextIndex].ManufacturerHardwareVariant)
          getSoftwareVersions.Add(this.GetSoftwareVersionsSlow(new Discoverer()
          {
            DiscoveryCondition = d.DiscoveryCondition,
            SoftwareRepositoryAlternativeBaseUrl = d.SoftwareRepositoryAlternativeBaseUrl,
            SoftwareRepositoryAuthenticationToken = d.SoftwareRepositoryAuthenticationToken,
            SoftwareRepositoryProxy = d.SoftwareRepositoryProxy,
            SoftwareRepositoryUserAgent = d.SoftwareRepositoryUserAgent
          }, typeDesignator, productCode, discoveryClass));
      }
      while (getSoftwareVersions.Count > 0)
      {
        try
        {
          Task<SoftwarePackages> task = await Task.WhenAny<SoftwarePackages>((IEnumerable<Task<SoftwarePackages>>) getSoftwareVersions);
          getSoftwareVersions.Remove(task);
          SoftwarePackages softwarePackages = await task;
          EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
          if (repositoryDiscoveryEvent != null)
            repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, softwarePackages.SoftwarePackageList));
        }
        catch (Exception ex)
        {
          Tracer.Information("error: {0}", (object) ex.Message);
        }
        if (nextIndex < packages.SoftwarePackageList.Count)
        {
          foreach (string productCode in packages.SoftwarePackageList[nextIndex].ManufacturerHardwareVariant)
            getSoftwareVersions.Add(this.GetSoftwareVersions(new Discoverer()
            {
              DiscoveryCondition = d.DiscoveryCondition,
              SoftwareRepositoryAlternativeBaseUrl = d.SoftwareRepositoryAlternativeBaseUrl,
              SoftwareRepositoryAuthenticationToken = d.SoftwareRepositoryAuthenticationToken,
              SoftwareRepositoryProxy = d.SoftwareRepositoryProxy,
              SoftwareRepositoryUserAgent = d.SoftwareRepositoryUserAgent
            }, typeDesignator, productCode, discoveryClass));
          ++nextIndex;
        }
      }
      Tracer.Information("GetListOfAllVariantsSlow executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetSoftwareVersionsSlow(
      Discoverer d2,
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      try
      {
        DiscoveryJsonResult discoveryJsonResult = await d2.DiscoverJsonAsync(new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageClass = discoveryClass,
            ManufacturerHardwareModel = typeDesignator,
            ManufacturerHardwareVariant = productCode
          },
          Response = new List<string>() { "default" },
          Condition = new List<string>()
          {
            this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "all" : "latest"
          }
        });
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetSoftwareVersionsSlow failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    private async Task<SoftwarePackages> GetProductCodesSlow(
      Discoverer discoverer,
      string typeDesignator,
      string discoveryClass)
    {
      try
      {
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageClass = discoveryClass,
            ManufacturerHardwareModel = typeDesignator
          },
          Response = new List<string>()
          {
            this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "manufacturerHardwareVariant" : "default"
          },
          Condition = new List<string>()
          {
            this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "unique" : "latest"
          }
        });
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetProductCodesSlow failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetLatestUpdatePackageSimple(string productType, string productCode)
    {
      Tracer.Information("GetLatestUpdatePackageSimple for {0} and {1}", (object) productType, (object) productCode);
      Task updatePackageSimple1 = this.ExecuteGetLatestUpdatePackageSimple(productType, productCode, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task updatePackageSimple2 = this.ExecuteGetLatestUpdatePackageSimple(productType, productCode, "Testing");
        Task.WaitAll(updatePackageSimple1, updatePackageSimple2);
      }
      else
        Task.WaitAll(updatePackageSimple1);
    }

    private async Task ExecuteGetLatestUpdatePackageSimple(
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages latestUpdatePackage = await this.GetLatestUpdatePackage(discoverer, typeDesignator, productCode, discoveryClass);
      Tracer.Information("Get product codes discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      List<Task<SoftwarePackages>> taskList = new List<Task<SoftwarePackages>>();
      foreach (SoftwarePackage softwarePackage in latestUpdatePackage.SoftwarePackageList)
      {
        foreach (string str in softwarePackage.ManufacturerHardwareVariant)
        {
          EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
          if (repositoryDiscoveryEvent != null)
            repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, new List<SoftwarePackage>()
            {
              softwarePackage
            }));
        }
      }
      Tracer.Information("ExecuteGetLatestUpdatePackageSimple executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetLatestUpdatePackage(
      Discoverer discoverer,
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      try
      {
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageClass = discoveryClass,
            ManufacturerHardwareModel = typeDesignator,
            ManufacturerHardwareVariant = productCode,
            PackageState = "Completed"
          },
          Response = new List<string>() { "default" },
          Condition = new List<string>() { "latest" }
        });
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetProductCodes failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetLatestUpdatePackage(string productType, string productCode)
    {
      Tracer.Information("GetLatestUpdatePackage for {0} and {1}", (object) productType, (object) productCode);
      Task latestUpdatePackage1 = this.ExecuteGetLatestUpdatePackage(productType, productCode, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task latestUpdatePackage2 = this.ExecuteGetLatestUpdatePackage(productType, productCode, "Testing");
        Task.WaitAll(latestUpdatePackage1, latestUpdatePackage2);
      }
      else
        Task.WaitAll(latestUpdatePackage1);
    }

    private async Task ExecuteGetLatestUpdatePackage(
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      Discoverer d = new Discoverer();
      d.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      d.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      d.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      d.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      d.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages productCodes = await this.GetProductCodes(d, typeDesignator, productCode, discoveryClass);
      Tracer.Information("Get product codes discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      List<Task<SoftwarePackages>> getSoftwareVersions = new List<Task<SoftwarePackages>>();
      foreach (SoftwarePackage softwarePackage in productCodes.SoftwarePackageList)
      {
        foreach (string a in softwarePackage.ManufacturerHardwareVariant)
        {
          if (string.Equals(a, productCode))
          {
            getSoftwareVersions.Add(this.GetSoftwareVersions(new Discoverer()
            {
              DiscoveryCondition = d.DiscoveryCondition,
              SoftwareRepositoryAlternativeBaseUrl = d.SoftwareRepositoryAlternativeBaseUrl,
              SoftwareRepositoryAuthenticationToken = d.SoftwareRepositoryAuthenticationToken,
              SoftwareRepositoryProxy = d.SoftwareRepositoryProxy,
              SoftwareRepositoryUserAgent = d.SoftwareRepositoryUserAgent
            }, typeDesignator, productCode, discoveryClass));
          }
          else
          {
            EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
            if (repositoryDiscoveryEvent != null)
              repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, new List<SoftwarePackage>()
              {
                softwarePackage
              }));
          }
        }
      }
      while (getSoftwareVersions.Count > 0)
      {
        try
        {
          Task<SoftwarePackages> task = await Task.WhenAny<SoftwarePackages>((IEnumerable<Task<SoftwarePackages>>) getSoftwareVersions);
          getSoftwareVersions.Remove(task);
          SoftwarePackages softwarePackages = await task;
          EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
          if (repositoryDiscoveryEvent != null)
            repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, softwarePackages.SoftwarePackageList));
        }
        catch (Exception ex)
        {
          Tracer.Information("error: {0}", (object) ex.Message);
        }
      }
      Tracer.Information("ExecuteGetLatestUpdatePackage executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetProductCodes(
      Discoverer discoverer,
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      try
      {
        DiscoveryParameters discoveryParameters;
        if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
          discoveryParameters = new DiscoveryParameters()
          {
            Query = {
              ManufacturerName = "Microsoft",
              ManufacturerProductLine = "Lumia",
              PackageType = "Firmware",
              PackageClass = discoveryClass,
              ManufacturerHardwareModel = typeDesignator,
              PackageState = "Completed"
            },
            Response = new List<string>()
            {
              "manufacturerHardwareVariant",
              "packageTitle"
            },
            Condition = new List<string>() { "unique" }
          };
        else
          discoveryParameters = new DiscoveryParameters()
          {
            Query = {
              ManufacturerName = "Microsoft",
              ManufacturerProductLine = "Lumia",
              PackageType = "Firmware",
              PackageClass = discoveryClass,
              ManufacturerHardwareModel = typeDesignator,
              ManufacturerHardwareVariant = productCode,
              PackageState = "Completed"
            },
            Response = new List<string>() { "default" },
            Condition = new List<string>() { "latest" }
          };
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(discoveryParameters);
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetProductCodes failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    private async Task<SoftwarePackages> GetSoftwareVersions(
      Discoverer d2,
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      try
      {
        DiscoveryJsonResult discoveryJsonResult = await d2.DiscoverJsonAsync(new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageState = "Completed",
            PackageClass = discoveryClass,
            ManufacturerHardwareModel = typeDesignator,
            ManufacturerHardwareVariant = productCode
          },
          Response = new List<string>() { "default" },
          Condition = new List<string>()
          {
            this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "all" : "latest"
          }
        });
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetSoftwareVersions failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetProductCodes(string productType)
    {
      Tracer.Information("GetProductCodes for {0}", (object) productType);
      Task productCodes1 = this.ExecuteGetProductCodes(productType, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task productCodes2 = this.ExecuteGetProductCodes(productType, "Testing");
        Task.WaitAll(productCodes1, productCodes2);
      }
      else
        Task.WaitAll(productCodes1);
    }

    private async Task ExecuteGetProductCodes(string typeDesignator, string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages listOfProductCodes = await this.GetListOfProductCodes(discoverer, typeDesignator, discoveryClass);
      Tracer.Information("Get product codes discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      foreach (SoftwarePackage softwarePackage in listOfProductCodes.SoftwarePackageList)
      {
        EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
        if (repositoryDiscoveryEvent != null)
          repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, new List<SoftwarePackage>()
          {
            softwarePackage
          }));
      }
      Tracer.Information("ExecuteGetProductCodes executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetListOfProductCodes(
      Discoverer discoverer,
      string typeDesignator,
      string discoveryClass)
    {
      try
      {
        DiscoveryParameters discoveryParameters = new DiscoveryParameters();
        discoveryParameters.Query.ManufacturerName = "Microsoft";
        discoveryParameters.Query.ManufacturerProductLine = "Lumia";
        discoveryParameters.Query.PackageType = "Firmware";
        discoveryParameters.Query.PackageClass = discoveryClass;
        discoveryParameters.Query.ManufacturerHardwareModel = typeDesignator;
        discoveryParameters.Query.PackageState = "Completed";
        List<string> stringList;
        if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken == null)
        {
          stringList = new List<string>() { "default" };
        }
        else
        {
          stringList = new List<string>();
          stringList.Add("manufacturerHardwareVariant");
          stringList.Add("packageTitle");
        }
        discoveryParameters.Response = stringList;
        discoveryParameters.Condition = new List<string>()
        {
          this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "unique" : "latest"
        };
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(discoveryParameters);
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetListOfProductCodes failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetProductCodesSw(string productType, string softwareVersion)
    {
      Tracer.Information("GetProductCodesSw for {0} and {1}", (object) productType, (object) softwareVersion);
      Task productCodesSw1 = this.ExecuteGetProductCodesSw(productType, softwareVersion, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task productCodesSw2 = this.ExecuteGetProductCodesSw(productType, softwareVersion, "Testing");
        Task.WaitAll(productCodesSw1, productCodesSw2);
      }
      else
        Task.WaitAll(productCodesSw1);
    }

    private async Task ExecuteGetProductCodesSw(
      string typeDesignator,
      string softwareVersion,
      string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      List<Task<SoftwarePackages>> getSoftwareVersions = new List<Task<SoftwarePackages>>();
      getSoftwareVersions.Add(this.GetProductCodesSw(new Discoverer()
      {
        DiscoveryCondition = discoverer.DiscoveryCondition,
        SoftwareRepositoryAlternativeBaseUrl = discoverer.SoftwareRepositoryAlternativeBaseUrl,
        SoftwareRepositoryAuthenticationToken = discoverer.SoftwareRepositoryAuthenticationToken,
        SoftwareRepositoryProxy = discoverer.SoftwareRepositoryProxy,
        SoftwareRepositoryUserAgent = discoverer.SoftwareRepositoryUserAgent
      }, typeDesignator, softwareVersion, discoveryClass));
      while (getSoftwareVersions.Count > 0)
      {
        try
        {
          Task<SoftwarePackages> task = await Task.WhenAny<SoftwarePackages>((IEnumerable<Task<SoftwarePackages>>) getSoftwareVersions);
          getSoftwareVersions.Remove(task);
          SoftwarePackages softwarePackages = await task;
          EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
          if (repositoryDiscoveryEvent != null)
            repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, softwarePackages.SoftwarePackageList));
        }
        catch (Exception ex)
        {
          Tracer.Information("error: {0}", (object) ex.Message);
        }
      }
      Tracer.Information("ExecuteGetProductCodesSw executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetProductCodesSw(
      Discoverer d2,
      string typeDesignator,
      string softwareVersion,
      string discoveryClass)
    {
      try
      {
        DiscoveryJsonResult discoveryJsonResult = await d2.DiscoverJsonAsync(new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageState = "Completed",
            PackageClass = discoveryClass,
            ManufacturerHardwareModel = typeDesignator,
            PackageRevision = softwareVersion
          },
          Response = new List<string>() { "default" },
          Condition = new List<string>()
          {
            this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "all" : "latest"
          }
        });
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetProductCodesSw failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetSoftwareVersions(string productType, string productCode)
    {
      Tracer.Information("GetSoftwareVersions for {0} and {1}", (object) productType, (object) productCode);
      Task softwareVersions1 = this.ExecuteGetSoftwareVersions(productType, productCode, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task softwareVersions2 = this.ExecuteGetSoftwareVersions(productType, productCode, "Testing");
        Task.WaitAll(softwareVersions1, softwareVersions2);
      }
      else
        Task.WaitAll(softwareVersions1);
    }

    private async Task ExecuteGetSoftwareVersions(
      string typeDesignator,
      string productCode,
      string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      List<Task<SoftwarePackages>> getSoftwareVersions = new List<Task<SoftwarePackages>>();
      getSoftwareVersions.Add(this.GetSoftwareVersions(new Discoverer()
      {
        DiscoveryCondition = discoverer.DiscoveryCondition,
        SoftwareRepositoryAlternativeBaseUrl = discoverer.SoftwareRepositoryAlternativeBaseUrl,
        SoftwareRepositoryAuthenticationToken = discoverer.SoftwareRepositoryAuthenticationToken,
        SoftwareRepositoryProxy = discoverer.SoftwareRepositoryProxy,
        SoftwareRepositoryUserAgent = discoverer.SoftwareRepositoryUserAgent
      }, typeDesignator, productCode, discoveryClass));
      while (getSoftwareVersions.Count > 0)
      {
        try
        {
          Task<SoftwarePackages> task = await Task.WhenAny<SoftwarePackages>((IEnumerable<Task<SoftwarePackages>>) getSoftwareVersions);
          getSoftwareVersions.Remove(task);
          SoftwarePackages softwarePackages = await task;
          EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
          if (repositoryDiscoveryEvent != null)
            repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, softwarePackages.SoftwarePackageList));
        }
        catch (Exception ex)
        {
          Tracer.Information("error: {0}", (object) ex.Message);
        }
      }
      Tracer.Information("ExecuteGetSoftwareVersions executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    public void GetSoftwareVersionsSw(string productType)
    {
      Tracer.Information("GetSoftwareVersionsSw for {0}", (object) productType);
      Task softwareVersionsSw1 = this.ExecuteGetSoftwareVersionsSw(productType, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task softwareVersionsSw2 = this.ExecuteGetSoftwareVersionsSw(productType, "Testing");
        Task.WaitAll(softwareVersionsSw1, softwareVersionsSw2);
      }
      else
        Task.WaitAll(softwareVersionsSw1);
    }

    private async Task ExecuteGetSoftwareVersionsSw(
      string typeDesignator,
      string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages softwareVersionsSw = await this.GetListOfSoftwareVersionsSw(discoverer, typeDesignator, discoveryClass);
      Tracer.Information("Get product codes Sw discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      foreach (SoftwarePackage softwarePackage in softwareVersionsSw.SoftwarePackageList)
      {
        EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
        if (repositoryDiscoveryEvent != null)
          repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, new List<SoftwarePackage>()
          {
            softwarePackage
          }));
      }
      Tracer.Information("ExecuteGetSoftwareVersionsSw executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetListOfSoftwareVersionsSw(
      Discoverer discoverer,
      string typeDesignator,
      string discoveryClass)
    {
      try
      {
        DiscoveryParameters discoveryParameters = new DiscoveryParameters();
        discoveryParameters.Query.ManufacturerName = "Microsoft";
        discoveryParameters.Query.ManufacturerProductLine = "Lumia";
        discoveryParameters.Query.PackageType = "Firmware";
        discoveryParameters.Query.PackageClass = discoveryClass;
        discoveryParameters.Query.ManufacturerHardwareModel = typeDesignator;
        discoveryParameters.Query.PackageState = "Completed";
        List<string> stringList;
        if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken == null)
        {
          stringList = new List<string>() { "default" };
        }
        else
        {
          stringList = new List<string>();
          stringList.Add("packageRevision");
          stringList.Add("manufacturerHardwareVariant");
          stringList.Add("packageTitle");
        }
        discoveryParameters.Response = stringList;
        discoveryParameters.Condition = new List<string>()
        {
          this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "unique" : "latest"
        };
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(discoveryParameters);
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetListOfSoftwareVersionsSw failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetProductCodesOs(string productType, string winOsVersion)
    {
      Tracer.Information("GetProductCodesOs for {0} and {1}", (object) productType, (object) winOsVersion);
      Task productCodesOs1 = this.ExecuteGetProductCodesOs(productType, winOsVersion, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task productCodesOs2 = this.ExecuteGetProductCodesOs(productType, winOsVersion, "Testing");
        Task.WaitAll(productCodesOs1, productCodesOs2);
      }
      else
        Task.WaitAll(productCodesOs1);
    }

    private async Task ExecuteGetProductCodesOs(
      string typeDesignator,
      string winOsVersion,
      string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      List<Task<SoftwarePackages>> getSoftwareVersions = new List<Task<SoftwarePackages>>();
      getSoftwareVersions.Add(this.GetProductCodesOs(new Discoverer()
      {
        DiscoveryCondition = discoverer.DiscoveryCondition,
        SoftwareRepositoryAlternativeBaseUrl = discoverer.SoftwareRepositoryAlternativeBaseUrl,
        SoftwareRepositoryAuthenticationToken = discoverer.SoftwareRepositoryAuthenticationToken,
        SoftwareRepositoryProxy = discoverer.SoftwareRepositoryProxy,
        SoftwareRepositoryUserAgent = discoverer.SoftwareRepositoryUserAgent
      }, typeDesignator, winOsVersion, discoveryClass));
      while (getSoftwareVersions.Count > 0)
      {
        try
        {
          Task<SoftwarePackages> task = await Task.WhenAny<SoftwarePackages>((IEnumerable<Task<SoftwarePackages>>) getSoftwareVersions);
          getSoftwareVersions.Remove(task);
          SoftwarePackages softwarePackages = await task;
          EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
          if (repositoryDiscoveryEvent != null)
            repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, softwarePackages.SoftwarePackageList));
        }
        catch (Exception ex)
        {
          Tracer.Information("error: {0}", (object) ex.Message);
        }
      }
      Tracer.Information("ExecuteGetProductCodesOs executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetProductCodesOs(
      Discoverer d2,
      string typeDesignator,
      string winOsVersion,
      string discoveryClass)
    {
      try
      {
        DiscoveryParameters discoveryParameters = new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageState = "Completed",
            PackageClass = discoveryClass,
            ManufacturerHardwareModel = typeDesignator
          },
          Response = new List<string>() { "default" },
          Condition = new List<string>()
          {
            this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "all" : "latest"
          }
        };
        ExtendedAttributes extendedAttributes = new ExtendedAttributes()
        {
          Dictionary = new Dictionary<string, string>()
          {
            {
              "OSVersion",
              winOsVersion
            }
          }
        };
        discoveryParameters.Query.ExtendedAttributes = extendedAttributes;
        DiscoveryJsonResult discoveryJsonResult = await d2.DiscoverJsonAsync(discoveryParameters);
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetProductCodesOs failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public void GetWinOsVersionsOs(string productType)
    {
      Tracer.Information("GetWinOsVersionsOs for {0}", (object) productType);
      Task winOsVersionsOs1 = this.ExecuteGetWinOsVersionsOs(productType, "Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        Task winOsVersionsOs2 = this.ExecuteGetWinOsVersionsOs(productType, "Testing");
        Task.WaitAll(winOsVersionsOs1, winOsVersionsOs2);
      }
      else
        Task.WaitAll(winOsVersionsOs1);
    }

    private async Task ExecuteGetWinOsVersionsOs(string typeDesignator, string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages ofWinOsVersionsOs = await this.GetListOfWinOsVersionsOs(discoverer, typeDesignator, discoveryClass);
      Tracer.Information("Get win os vesrions discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      foreach (SoftwarePackage softwarePackage in ofWinOsVersionsOs.SoftwarePackageList)
      {
        EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs> repositoryDiscoveryEvent = this.SoftwareRepositoryDiscoveryEvent;
        if (repositoryDiscoveryEvent != null)
          repositoryDiscoveryEvent((object) this, new SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs(typeDesignator, new List<SoftwarePackage>()
          {
            softwarePackage
          }));
      }
      Tracer.Information("ExecuteGetWinOsVersionsOs executed for {0}. Duration {1} s", (object) typeDesignator, (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
    }

    private async Task<SoftwarePackages> GetListOfWinOsVersionsOs(
      Discoverer discoverer,
      string typeDesignator,
      string discoveryClass)
    {
      try
      {
        DiscoveryParameters discoveryParameters = new DiscoveryParameters();
        discoveryParameters.Query.ManufacturerName = "Microsoft";
        discoveryParameters.Query.ManufacturerProductLine = "Lumia";
        discoveryParameters.Query.PackageType = "Firmware";
        discoveryParameters.Query.PackageClass = discoveryClass;
        discoveryParameters.Query.ManufacturerHardwareModel = typeDesignator;
        discoveryParameters.Query.PackageState = "Completed";
        List<string> stringList;
        if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken == null)
        {
          stringList = new List<string>() { "default" };
        }
        else
        {
          stringList = new List<string>();
          stringList.Add("extendedAttributes.OSVersion");
          stringList.Add("manufacturerHardwareVariant");
          stringList.Add("packageTitle");
        }
        discoveryParameters.Response = stringList;
        discoveryParameters.Condition = new List<string>()
        {
          this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null ? "unique" : "latest"
        };
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(discoveryParameters);
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetListOfSoftwareVersionsOs failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public SoftwarePackages GetEmergencyTypeDesignators()
    {
      Tracer.Information(nameof (GetEmergencyTypeDesignators));
      Task<SoftwarePackages> task = (Task<SoftwarePackages>) null;
      Task<SoftwarePackages> emergencyTypeDesignators = this.ExecuteGetEmergencyTypeDesignators("Public");
      if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
      {
        task = this.ExecuteGetEmergencyTypeDesignators("Testing");
        Task.WaitAll((Task[]) new Task<SoftwarePackages>[2]
        {
          emergencyTypeDesignators,
          task
        });
      }
      else
        Task.WaitAll((Task[]) new Task<SoftwarePackages>[1]
        {
          emergencyTypeDesignators
        });
      SoftwarePackages softwarePackages = new SoftwarePackages();
      softwarePackages.SoftwarePackageList = new List<SoftwarePackage>();
      if (emergencyTypeDesignators != null)
        softwarePackages.SoftwarePackageList.AddRange((IEnumerable<SoftwarePackage>) emergencyTypeDesignators.Result.SoftwarePackageList);
      if (task != null)
        softwarePackages.SoftwarePackageList.AddRange((IEnumerable<SoftwarePackage>) task.Result.SoftwarePackageList);
      return softwarePackages;
    }

    private async Task<SoftwarePackages> ExecuteGetEmergencyTypeDesignators(
      string discoveryClass)
    {
      Discoverer discoverer = new Discoverer();
      discoverer.SoftwareRepositoryAlternativeBaseUrl = this.msrDiscoverer.SoftwareRepositoryAlternativeBaseUrl;
      discoverer.SoftwareRepositoryProxy = this.msrDiscoverer.SoftwareRepositoryProxy;
      discoverer.SoftwareRepositoryUserAgent = this.msrDiscoverer.SoftwareRepositoryUserAgent;
      discoverer.SoftwareRepositoryAuthenticationToken = this.msrDiscoverer.SoftwareRepositoryAuthenticationToken;
      discoverer.DiscoveryCondition = this.msrDiscoverer.DiscoveryCondition;
      DateTime startDiscovery = DateTime.Now;
      SoftwarePackages productTypes = await this.GetProductTypes(discoverer, discoveryClass);
      Tracer.Information("Get product types discovery took {0} seconds.", (object) DateTime.Now.Subtract(startDiscovery).TotalSeconds);
      return productTypes;
    }

    private async Task<SoftwarePackages> GetProductTypes(
      Discoverer discoverer,
      string discoveryClass)
    {
      try
      {
        DiscoveryJsonResult discoveryJsonResult = await discoverer.DiscoverJsonAsync(new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "EMERGENCYFILES",
            PackageClass = discoveryClass,
            PackageState = "Completed"
          },
          Response = new List<string>()
          {
            "manufacturerHardwareModel"
          },
          Condition = new List<string>() { "unique" }
        });
        if (discoveryJsonResult == null)
        {
          Tracer.Information("returns null");
          return (SoftwarePackages) null;
        }
        if (discoveryJsonResult.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}, {1}", (object) discoveryJsonResult.StatusCode, (object) discoveryJsonResult.Result);
          if (discoveryJsonResult.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(discoveryJsonResult.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetProductTypes failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackages) null;
    }

    public SoftwarePackage GetEmergencyPackage(
      string productType,
      string discoveryClass)
    {
      Tracer.Information("GetEmergencyPackage for {0}", (object) productType);
      try
      {
        if (this.msrDiscoverer.SoftwareRepositoryAuthenticationToken != null)
          return this.DoDiscovery(this.msrDiscoverer, new DiscoveryParameters()
          {
            Query = {
              ManufacturerName = "Microsoft",
              ManufacturerProductLine = "Lumia",
              PackageType = "EMERGENCYFILES",
              PackageClass = discoveryClass,
              PackageState = "Completed",
              ManufacturerHardwareModel = productType
            },
            Response = new List<string>() { "default" },
            Condition = new List<string>() { "latest" }
          }).SoftwarePackageList.ElementAtOrDefault<SoftwarePackage>(0);
        Tracer.Information("Authetication needed");
        return (SoftwarePackage) null;
      }
      catch (Exception ex)
      {
        Tracer.Information("GetEmergencyPackage failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackage) null;
    }

    public SoftwarePackage GetVariant(SrdfItem srdfItem)
    {
      Tracer.Information("GetVariant for {0}", (object) srdfItem.Query.ManufacturerPackageId);
      try
      {
        DiscoveryParameters parameters = new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = srdfItem.Query.ManufacturerName,
            ManufacturerProductLine = srdfItem.Query.ManufacturerProductLine,
            PackageType = srdfItem.Query.PackageType,
            PackageClass = srdfItem.Query.PackageClass,
            PackageTitle = srdfItem.Query.PackageTitle,
            PackageSubtitle = srdfItem.Query.PackageSubtitle,
            PackageRevision = srdfItem.Query.PackageRevision,
            PackageSubRevision = srdfItem.Query.PackageSubRevision,
            ManufacturerPackageId = srdfItem.Query.ManufacturerPackageId,
            ManufacturerModelName = srdfItem.Query.ManufacturerModelName,
            ManufacturerVariantName = srdfItem.Query.ManufacturerVariantName,
            ManufacturerPlatformId = srdfItem.Query.ManufacturerPlatformId
          }
        };
        int num = (srdfItem.Query.ManufacturerModelName == null ? 0 : 1) + (srdfItem.Query.ManufacturerVariantName == null ? 0 : 1) + (srdfItem.Query.ManufacturerPlatformId == null ? 0 : 1);
        if (num < 3 && srdfItem.Query.ManufacturerHardwareModel != null)
        {
          parameters.Query.ManufacturerHardwareModel = srdfItem.Query.ManufacturerHardwareModel;
          ++num;
        }
        if (num < 3 && srdfItem.Query.ManufacturerHardwareVariant != null)
        {
          parameters.Query.ManufacturerHardwareVariant = srdfItem.Query.ManufacturerHardwareVariant[0];
          ++num;
        }
        if (num < 3 && srdfItem.Query.OperatorName != null)
        {
          parameters.Query.OperatorName = srdfItem.Query.OperatorName;
          ++num;
        }
        if (num < 3 && srdfItem.Query.CustomerName != null)
          parameters.Query.CustomerName = srdfItem.Query.CustomerName;
        parameters.Condition.Clear();
        parameters.Condition = srdfItem.Condition;
        parameters.Response = srdfItem.Response;
        return this.DoDiscovery(this.msrDiscoverer, parameters).SoftwarePackageList.ElementAtOrDefault<SoftwarePackage>(0);
      }
      catch (Exception ex)
      {
        Tracer.Information("GetVariant failed: {0}", (object) ex.Message);
      }
      return (SoftwarePackage) null;
    }

    public SrdfItem GetVariant(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion)
    {
      Tracer.Information("GetVariant for {0}, {1}", (object) productType, (object) productCode);
      try
      {
        DiscoveryParameters parameters = new DiscoveryParameters()
        {
          Query = {
            ManufacturerName = "Microsoft",
            ManufacturerProductLine = "Lumia",
            PackageType = "Firmware",
            PackageClass = "Public",
            ManufacturerHardwareModel = productType,
            ManufacturerHardwareVariant = productCode,
            PackageRevision = softwareVersion,
            PackageSubRevision = variantVersion
          }
        };
        parameters.Condition.Clear();
        parameters.Condition.Add("default");
        parameters.Response.Clear();
        parameters.Response.Add("all");
        SoftwarePackage softwarePackage = this.DoDiscovery(this.msrDiscoverer, parameters).SoftwarePackageList.ElementAtOrDefault<SoftwarePackage>(0);
        if (softwarePackage == null)
        {
          parameters.Query.PackageClass = "Testing";
          softwarePackage = this.DoDiscovery(this.msrDiscoverer, parameters).SoftwarePackageList.ElementAtOrDefault<SoftwarePackage>(0);
        }
        if (softwarePackage != null)
        {
          SrdfItem srdfItem = new SrdfItem();
          SrdfItemQuery srdfItemQuery = new SrdfItemQuery();
          srdfItemQuery.ManufacturerName = softwarePackage.ManufacturerName;
          srdfItemQuery.ManufacturerProductLine = softwarePackage.ManufacturerProductLine;
          srdfItemQuery.PackageType = softwarePackage.PackageType;
          srdfItemQuery.PackageClass = softwarePackage.PackageClass.OrderBy<string, int>((Func<string, int>) (s =>
          {
            if (s == "Public")
              return 0;
            return !(s == "Testing") ? 2 : 1;
          })).First<string>();
          srdfItemQuery.PackageTitle = string.IsNullOrEmpty(softwarePackage.PackageTitle) ? (string) null : softwarePackage.PackageTitle;
          srdfItemQuery.PackageSubtitle = string.IsNullOrEmpty(softwarePackage.PackageSubtitle) ? (string) null : softwarePackage.PackageSubtitle;
          srdfItemQuery.PackageRevision = string.IsNullOrEmpty(softwarePackage.PackageRevision) ? (string) null : softwarePackage.PackageRevision;
          srdfItemQuery.PackageSubRevision = string.IsNullOrEmpty(softwarePackage.PackageSubRevision) ? (string) null : softwarePackage.PackageSubRevision;
          srdfItemQuery.ManufacturerPackageId = string.IsNullOrEmpty(softwarePackage.ManufacturerPackageId) ? (string) null : softwarePackage.ManufacturerPackageId;
          srdfItemQuery.ManufacturerModelName = softwarePackage.ManufacturerModelName == null ? (string) null : softwarePackage.ManufacturerModelName.ElementAtOrDefault<string>(0);
          srdfItemQuery.ManufacturerVariantName = softwarePackage.ManufacturerVariantName == null ? (string) null : softwarePackage.ManufacturerVariantName.ElementAtOrDefault<string>(0);
          srdfItemQuery.ManufacturerPlatformId = softwarePackage.ManufacturerPlatformId == null ? (string) null : softwarePackage.ManufacturerPlatformId.ElementAtOrDefault<string>(0);
          srdfItemQuery.ManufacturerHardwareModel = softwarePackage.ManufacturerHardwareModel == null ? (string) null : softwarePackage.ManufacturerHardwareModel.ElementAtOrDefault<string>(0);
          List<string> stringList;
          if (softwarePackage.ManufacturerHardwareVariant != null)
            stringList = new List<string>()
            {
              softwarePackage.ManufacturerHardwareVariant.ElementAtOrDefault<string>(0)
            };
          else
            stringList = (List<string>) null;
          srdfItemQuery.ManufacturerHardwareVariant = stringList;
          srdfItemQuery.OperatorName = softwarePackage.OperatorName == null ? (string) null : softwarePackage.OperatorName.ElementAtOrDefault<string>(0);
          srdfItemQuery.CustomerName = softwarePackage.CustomerName == null ? (string) null : softwarePackage.CustomerName.ElementAtOrDefault<string>(0);
          srdfItem.Query = srdfItemQuery;
          srdfItem.Condition = new List<string>()
          {
            "default"
          };
          srdfItem.Response = new List<string>() { "all" };
          return srdfItem;
        }
      }
      catch (Exception ex)
      {
        Tracer.Information("GetVariant failed: {0}", (object) ex.Message);
        throw;
      }
      return (SrdfItem) null;
    }

    private SoftwarePackages DoDiscovery(
      Discoverer discoverer,
      DiscoveryParameters parameters)
    {
      using (Task<DiscoveryJsonResult> task = discoverer.DiscoverJsonAsync(parameters))
      {
        task.Wait(60000);
        if (!task.IsCompleted)
        {
          Tracer.Information("DiscoverJsonAsync timed out");
          return (SoftwarePackages) null;
        }
        if (task.Result == null)
        {
          Tracer.Information("DiscoverJsonAsync failed");
          return (SoftwarePackages) null;
        }
        if (task.Result.StatusCode != HttpStatusCode.OK)
        {
          Tracer.Information("DiscoverJsonAsync failed with code: {0}", (object) task.Result.StatusCode);
          if (task.Result.StatusCode == HttpStatusCode.Forbidden)
          {
            Tracer.Information("Authetication expired");
            throw new AuthenticationException("SoftwareRepository authentication expired");
          }
          return (SoftwarePackages) null;
        }
        if (!string.IsNullOrEmpty(task.Result.Result))
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(task.Result.Result)))
          {
            SoftwarePackages softwarePackages = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) memoryStream);
            if (softwarePackages != null)
              return softwarePackages;
          }
        }
        return (SoftwarePackages) null;
      }
    }

    public class SoftwareRepositoryDiscoveryEventArgs
    {
      public SoftwareRepositoryDiscoveryEventArgs(
        string typeDesignator,
        List<SoftwarePackage> softwarePackageList)
      {
        this.TypeDesignator = typeDesignator;
        this.SoftwarePackageList = softwarePackageList;
      }

      public string TypeDesignator { get; set; }

      public List<SoftwarePackage> SoftwarePackageList { get; set; }
    }
  }
}
