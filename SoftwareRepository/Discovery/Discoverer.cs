// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Discovery.Discoverer
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareRepository.Discovery
{
  public class Discoverer
  {
    private const string DefaultSoftwareRepositoryBaseUrl = "https://api.swrepository.com";
    private const string DefaultSoftwareRepositoryDiscovery = "/rest-api/discovery/package";
    private const string DefaultSoftwareRepositoryUserAgent = "SoftwareRepository";

    public DiscoveryCondition DiscoveryCondition { get; set; }

    public string SoftwareRepositoryAlternativeBaseUrl { get; set; }

    public string SoftwareRepositoryAuthenticationToken { get; set; }

    public IWebProxy SoftwareRepositoryProxy { get; set; }

    public string SoftwareRepositoryUserAgent { get; set; }

    public async Task<DiscoveryResult> DiscoverAsync(string descriptor) => await this.DiscoverAsync(descriptor, CancellationToken.None);

    public async Task<DiscoveryJsonResult> DiscoverJsonAsync(
      string descriptor)
    {
      return await this.DiscoverJsonAsync(descriptor, CancellationToken.None);
    }

    public async Task<DiscoveryResult> DiscoverAsync(
      string descriptor,
      CancellationToken cancellationToken)
    {
      DiscoveryResult discoveryResult = new DiscoveryResult();
      DiscoveryJsonResult discoveryJsonResult = await this.DiscoverJsonAsync(descriptor, cancellationToken);
      discoveryResult.StatusCode = discoveryJsonResult.StatusCode;
      if (discoveryResult.StatusCode == HttpStatusCode.OK)
        discoveryResult.Result = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)));
      return discoveryResult;
    }

    public async Task<DiscoveryJsonResult> DiscoverJsonAsync(
      string descriptor,
      CancellationToken cancellationToken)
    {
      DiscoveryJsonResult discoveryResult = new DiscoveryJsonResult();
      try
      {
        DiscoveryParameters discoveryParameters = (DiscoveryParameters) new DataContractJsonSerializer(typeof (DiscoveryParameters)).ReadObject((Stream) new MemoryStream(Encoding.UTF8.GetBytes(descriptor)));
        if (discoveryParameters.APIVersion == null)
          discoveryParameters.APIVersion = "1";
        if (discoveryParameters.Condition == null)
        {
          discoveryParameters.Condition = new List<string>();
          discoveryParameters.Condition.Add("default");
        }
        if (discoveryParameters.Response == null)
        {
          discoveryParameters.Response = new List<string>();
          discoveryParameters.Response.Add("default");
        }
        discoveryResult = await this.DiscoverJsonAsync(discoveryParameters, cancellationToken);
      }
      catch (Exception ex)
      {
        throw new DiscoveryException("Discovery exception", ex);
      }
      return discoveryResult;
    }

    public async Task<DiscoveryResult> DiscoverAsync(
      string manufacturerName,
      string manufacturerProductLine,
      string packageType,
      string packageClass,
      [Optional] string packageTitle,
      [Optional] string packageSubtitle,
      [Optional] string packageRevision,
      [Optional] string packageSubRevision,
      [Optional] string packageState,
      [Optional] string manufacturerPackageId,
      [Optional] string manufacturerModelName,
      [Optional] string manufacturerVariantName,
      [Optional] string manufacturerPlatformId,
      [Optional] string manufacturerHardwareModel,
      [Optional] string manufacturerHardwareVariant,
      [Optional] string operatorName,
      [Optional] string customerName,
      [Optional] Dictionary<string, string> extendedAttributes,
      [Optional] List<string> responseFilter,
      [Optional] CancellationToken cancellationToken)
    {
      ExtendedAttributes extendedAttributes1 = (ExtendedAttributes) null;
      if (extendedAttributes != null && extendedAttributes.Count > 0)
        extendedAttributes1 = new ExtendedAttributes()
        {
          Dictionary = extendedAttributes
        };
      DiscoveryQueryParameters discoveryQueryParameters = new DiscoveryQueryParameters()
      {
        ManufacturerName = manufacturerName,
        ManufacturerProductLine = manufacturerProductLine,
        PackageType = packageType,
        PackageClass = packageClass,
        PackageTitle = packageTitle,
        PackageSubtitle = packageSubtitle,
        PackageRevision = packageRevision,
        PackageSubRevision = packageSubRevision,
        PackageState = packageState,
        ManufacturerPackageId = manufacturerPackageId,
        ManufacturerModelName = manufacturerModelName,
        ManufacturerVariantName = manufacturerVariantName,
        ManufacturerPlatformId = manufacturerPlatformId,
        ManufacturerHardwareModel = manufacturerHardwareModel,
        ManufacturerHardwareVariant = manufacturerHardwareVariant,
        OperatorName = operatorName,
        CustomerName = customerName,
        ExtendedAttributes = extendedAttributes1
      };
      DiscoveryParameters discoveryParameters = new DiscoveryParameters(this.DiscoveryCondition)
      {
        Query = discoveryQueryParameters
      };
      if (responseFilter != null && responseFilter.Count > 0)
        discoveryParameters.Response = responseFilter;
      return await this.DiscoverAsync(discoveryParameters, cancellationToken);
    }

    public async Task<DiscoveryResult> DiscoverAsync(
      DiscoveryParameters discoveryParameters)
    {
      return await this.DiscoverAsync(discoveryParameters, CancellationToken.None);
    }

    public async Task<DiscoveryResult> DiscoverAsync(
      DiscoveryParameters discoveryParameters,
      CancellationToken cancellationToken)
    {
      DiscoveryResult discoveryResult = new DiscoveryResult();
      DiscoveryJsonResult discoveryJsonResult = await this.DiscoverJsonAsync(discoveryParameters, cancellationToken);
      discoveryResult.StatusCode = discoveryJsonResult.StatusCode;
      if (discoveryResult.StatusCode == HttpStatusCode.OK)
        discoveryResult.Result = (SoftwarePackages) new DataContractJsonSerializer(typeof (SoftwarePackages)).ReadObject((Stream) new MemoryStream(Encoding.UTF8.GetBytes(discoveryJsonResult.Result)));
      return discoveryResult;
    }

    public async Task<DiscoveryJsonResult> DiscoverJsonAsync(
      string manufacturerName,
      string manufacturerProductLine,
      string packageType,
      string packageClass,
      [Optional] string packageTitle,
      [Optional] string packageSubtitle,
      [Optional] string packageRevision,
      [Optional] string packageSubRevision,
      [Optional] string packageState,
      [Optional] string manufacturerPackageId,
      [Optional] string manufacturerModelName,
      [Optional] string manufacturerVariantName,
      [Optional] string manufacturerPlatformId,
      [Optional] string manufacturerHardwareModel,
      [Optional] string manufacturerHardwareVariant,
      [Optional] string operatorName,
      [Optional] string customerName,
      [Optional] Dictionary<string, string> extendedAttributes,
      [Optional] List<string> responseFilter,
      [Optional] CancellationToken cancellationToken)
    {
      ExtendedAttributes extendedAttributes1 = (ExtendedAttributes) null;
      if (extendedAttributes != null && extendedAttributes.Count > 0)
        extendedAttributes1 = new ExtendedAttributes()
        {
          Dictionary = extendedAttributes
        };
      DiscoveryQueryParameters discoveryQueryParameters = new DiscoveryQueryParameters()
      {
        ManufacturerName = manufacturerName,
        ManufacturerProductLine = manufacturerProductLine,
        PackageType = packageType,
        PackageClass = packageClass,
        PackageTitle = packageTitle,
        PackageSubtitle = packageSubtitle,
        PackageRevision = packageRevision,
        PackageSubRevision = packageSubRevision,
        PackageState = packageState,
        ManufacturerPackageId = manufacturerPackageId,
        ManufacturerModelName = manufacturerModelName,
        ManufacturerVariantName = manufacturerVariantName,
        ManufacturerPlatformId = manufacturerPlatformId,
        ManufacturerHardwareModel = manufacturerHardwareModel,
        ManufacturerHardwareVariant = manufacturerHardwareVariant,
        OperatorName = operatorName,
        CustomerName = customerName,
        ExtendedAttributes = extendedAttributes1
      };
      DiscoveryParameters discoveryParameters = new DiscoveryParameters(this.DiscoveryCondition)
      {
        Query = discoveryQueryParameters
      };
      if (responseFilter != null && responseFilter.Count > 0)
        discoveryParameters.Response = responseFilter;
      return await this.DiscoverJsonAsync(discoveryParameters, cancellationToken);
    }

    public async Task<DiscoveryJsonResult> DiscoverJsonAsync(
      DiscoveryParameters discoveryParameters)
    {
      return await this.DiscoverJsonAsync(discoveryParameters, CancellationToken.None);
    }

    public async Task<DiscoveryJsonResult> DiscoverJsonAsync(
      DiscoveryParameters discoveryParameters,
      CancellationToken cancellationToken)
    {
      DiscoveryJsonResult discoveryJsonResult1 = new DiscoveryJsonResult();
      try
      {
        DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (DiscoveryParameters));
        MemoryStream memoryStream1 = new MemoryStream();
        MemoryStream memoryStream2 = memoryStream1;
        DiscoveryParameters discoveryParameters1 = discoveryParameters;
        contractJsonSerializer.WriteObject((Stream) memoryStream2, (object) discoveryParameters1);
        memoryStream1.Seek(0L, SeekOrigin.Begin);
        string end = new StreamReader((Stream) memoryStream1).ReadToEnd();
        string str1 = "https://api.swrepository.com";
        if (this.SoftwareRepositoryAlternativeBaseUrl != null)
          str1 = this.SoftwareRepositoryAlternativeBaseUrl;
        Uri requestUri = new Uri(str1 + "/rest-api/discovery/package");
        string input = "SoftwareRepository";
        if (this.SoftwareRepositoryUserAgent != null)
          input = this.SoftwareRepositoryUserAgent;
        HttpClient httpClient = (HttpClient) null;
        if (this.SoftwareRepositoryProxy != null)
          httpClient = new HttpClient((HttpMessageHandler) new HttpClientHandler()
          {
            Proxy = this.SoftwareRepositoryProxy,
            UseProxy = true
          });
        else
          httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(input);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (this.SoftwareRepositoryAuthenticationToken != null)
        {
          httpClient.DefaultRequestHeaders.Add("X-Authentication", this.SoftwareRepositoryAuthenticationToken);
          httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.SoftwareRepositoryAuthenticationToken);
        }
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, (HttpContent) new StringContent(end, Encoding.UTF8, "application/json"), cancellationToken);
        discoveryJsonResult1.StatusCode = httpResponseMessage.StatusCode;
        if (discoveryJsonResult1.StatusCode == HttpStatusCode.OK)
        {
          HttpContent content = httpResponseMessage.Content;
          DiscoveryJsonResult discoveryJsonResult2 = discoveryJsonResult1;
          string str2 = await content.ReadAsStringAsync();
          discoveryJsonResult2.Result = str2;
          discoveryJsonResult2 = (DiscoveryJsonResult) null;
        }
        else
          discoveryJsonResult1.Result = string.Empty;
        httpClient.Dispose();
        httpClient = (HttpClient) null;
      }
      catch (Exception ex)
      {
        throw new DiscoveryException("Discovery exception", ex);
      }
      return discoveryJsonResult1;
    }
  }
}
