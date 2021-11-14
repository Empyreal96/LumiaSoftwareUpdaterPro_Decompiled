// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Reporting.Reporter
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareRepository.Reporting
{
  public class Reporter
  {
    private const string DefaultSoftwareRepositoryBaseUrl = "https://api.swrepository.com";
    private const string DefaultSoftwareRepositoryDownloadReportUrl = "/rest-api/discovery/report";
    private const string DefaultSoftwareRepositoryUploadReport = "/rest-api/report";
    private const string DefaultSoftwareRepositoryReportUploadApiVersion = "/1";
    private const string DefaultSoftwareRepositoryReportUploadApi = "/uploadlocation";
    private const string DefaultSoftwareRepositoryUserAgent = "SoftwareRepository";

    public string SoftwareRepositoryAlternativeBaseUrl { get; set; }

    public string SoftwareRepositoryAuthenticationToken { get; set; }

    public IWebProxy SoftwareRepositoryProxy { get; set; }

    public string SoftwareRepositoryUserAgent { get; set; }

    public async Task<string> GetReportUploadLocationAsync(
      string manufacturerName,
      string manufacturerProductLine,
      string reportClassification,
      string fileName,
      CancellationToken cancellationToken)
    {
      string ret = string.Empty;
      ReportUploadLocationParameters locationParameters1 = new ReportUploadLocationParameters()
      {
        ManufacturerName = manufacturerName,
        ManufacturerProductLine = manufacturerProductLine,
        ReportClassification = reportClassification,
        FileName = fileName
      };
      try
      {
        DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (ReportUploadLocationParameters));
        MemoryStream memoryStream1 = new MemoryStream();
        MemoryStream memoryStream2 = memoryStream1;
        ReportUploadLocationParameters locationParameters2 = locationParameters1;
        contractJsonSerializer.WriteObject((Stream) memoryStream2, (object) locationParameters2);
        memoryStream1.Seek(0L, SeekOrigin.Begin);
        string end = new StreamReader((Stream) memoryStream1).ReadToEnd();
        string str = "https://api.swrepository.com";
        if (this.SoftwareRepositoryAlternativeBaseUrl != null)
          str = this.SoftwareRepositoryAlternativeBaseUrl;
        Uri requestUri = new Uri(str + "/rest-api/report/1/uploadlocation");
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
        httpResponseMessage.EnsureSuccessStatusCode();
        try
        {
          ret = httpResponseMessage.Headers.First<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (h => h.Key.Equals("X-Upload-Location"))).Value.First<string>();
        }
        catch (InvalidOperationException ex)
        {
          if (httpResponseMessage.Headers.Location != (Uri) null)
            ret = httpResponseMessage.Headers.Location.AbsoluteUri;
        }
        httpClient.Dispose();
        httpClient = (HttpClient) null;
      }
      catch (Exception ex)
      {
        throw new ReportException("Report exception", ex);
      }
      return ret;
    }

    public async Task<bool> UploadReportAsync(
      string manufacturerName,
      string manufacturerProductLine,
      string reportClassification,
      List<string> filePaths,
      CancellationToken cancellationToken)
    {
      try
      {
        foreach (string filePath1 in filePaths)
        {
          string filePath = filePath1;
          await new CloudBlockBlob(new Uri(await this.GetReportUploadLocationAsync(manufacturerName, manufacturerProductLine, reportClassification, Path.GetFileName(filePath), cancellationToken))).UploadFromFileAsync(filePath, FileMode.Open);
          filePath = (string) null;
        }
      }
      catch (Exception ex)
      {
        throw new ReportException("Cannot upload report.", ex);
      }
      return true;
    }

    internal async Task SendDownloadReport(
      string id,
      string filename,
      List<string> url,
      int status,
      long time,
      long size,
      int connections,
      CancellationToken cancellationToken)
    {
      DownloadReport downloadReport1 = new DownloadReport()
      {
        ApiVersion = "1",
        Id = id,
        FileName = filename,
        Url = url,
        Status = status,
        Time = time,
        Size = size,
        Connections = connections
      };
      DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (DownloadReport));
      MemoryStream memoryStream1 = new MemoryStream();
      MemoryStream memoryStream2 = memoryStream1;
      DownloadReport downloadReport2 = downloadReport1;
      contractJsonSerializer.WriteObject((Stream) memoryStream2, (object) downloadReport2);
      memoryStream1.Seek(0L, SeekOrigin.Begin);
      string end = new StreamReader((Stream) memoryStream1).ReadToEnd();
      string str = "https://api.swrepository.com";
      if (this.SoftwareRepositoryAlternativeBaseUrl != null)
        str = this.SoftwareRepositoryAlternativeBaseUrl;
      string input = "SoftwareRepository";
      if (this.SoftwareRepositoryUserAgent != null)
        input = this.SoftwareRepositoryUserAgent;
      Uri requestUri = new Uri(str + "/rest-api/discovery/report");
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
      HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, (HttpContent) new StringContent(end, Encoding.UTF8, "application/json"));
      if (httpResponseMessage.StatusCode != HttpStatusCode.OK && httpResponseMessage.StatusCode != HttpStatusCode.BadRequest)
      {
        int statusCode = (int) httpResponseMessage.StatusCode;
      }
      httpClient.Dispose();
    }
  }
}
