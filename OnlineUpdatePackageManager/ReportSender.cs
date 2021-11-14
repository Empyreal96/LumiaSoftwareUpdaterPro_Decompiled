// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ReportSender
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using LsuProReportingLib;
using Microsoft.LsuPro.Helpers;
using Microsoft.LsuPro.SoftwareRepository;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.LsuPro
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "reviewed")]
  public class ReportSender
  {
    private CancellationTokenSource poolingTaskAzureCancellationTokenSource;
    private CancellationToken poolingTaskAzureCancellationToken;
    private TaskHelper poolingTaskAzure;

    public ReportSender()
    {
      this.poolingTaskAzureCancellationTokenSource = new CancellationTokenSource();
      this.poolingTaskAzureCancellationToken = this.poolingTaskAzureCancellationTokenSource.Token;
    }

    public bool NetworkAvailable { get; set; }

    public static void SaveReportAsync(ReportDetails reportDetails, bool saveZip = false) => Task.Run((Action) (() => ReportSender.SaveReport(reportDetails, saveZip)));

    public static void SaveReport(ReportDetails reportDetails, bool saveZip = false)
    {
      ReportData reportData = new ReportData()
      {
        ProductType = reportDetails.ProductType,
        ProductCode = reportDetails.ProductCode,
        ProductCodeNew = reportDetails.ProductCodeNew,
        SoftwareVersion = reportDetails.SoftwareVersion,
        SoftwareVersionNew = reportDetails.SoftwareVersionNew,
        Imei = reportDetails.Imei,
        HwId = reportDetails.HwId,
        RdInfo = reportDetails.RdInfo,
        Uri = reportDetails.Uri,
        UriDescription = reportDetails.UriDescription,
        ApiError = reportDetails.ApiError,
        ApiErrorText = reportDetails.ApiErrorText,
        DebugField = reportDetails.DebugField,
        UpdateDuration = reportDetails.UpdateDuration,
        DownloadDuration = reportDetails.DownloadDuration,
        FlashDeviceInfo = reportDetails.FlashDeviceInfo,
        ActionDescription = reportDetails.ActionDescription,
        Ext4 = reportDetails.LocationInfo,
        Ext5 = reportDetails.ThorVersion,
        Ext7 = Convert.ToString(reportDetails.GetExt7Field(), (IFormatProvider) CultureInfo.InvariantCulture),
        Ext8 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) reportDetails.AverageDownloadSpeed),
        Ext9 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:F2}", (object) reportDetails.AverageFlashingSpeed)
      };
      try
      {
        ReportSender.SaveAzureReport(Path.ChangeExtension(Path.Combine(SpecialFolders.Reports, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0:yyyyMMdd}_{0:HHmmss_ff}.dat", (object) DateTime.Now)), "xml"), reportDetails);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Saving report failed", objArray);
      }
    }

    public static void SaveReportAsync(ReportDetails reportDetails, DateTime time)
    {
      ReportData reportData = new ReportData()
      {
        ProductType = reportDetails.ProductType,
        ProductCode = reportDetails.ProductCode,
        ProductCodeNew = reportDetails.ProductCodeNew,
        SoftwareVersion = reportDetails.SoftwareVersion,
        SoftwareVersionNew = reportDetails.SoftwareVersionNew,
        Imei = reportDetails.Imei,
        HwId = reportDetails.HwId,
        RdInfo = reportDetails.RdInfo,
        Uri = reportDetails.Uri,
        UriDescription = reportDetails.UriDescription,
        ApiError = reportDetails.ApiError,
        ApiErrorText = reportDetails.ApiErrorText,
        DebugField = reportDetails.DebugField,
        UpdateDuration = reportDetails.UpdateDuration,
        DownloadDuration = reportDetails.DownloadDuration,
        FlashDeviceInfo = reportDetails.FlashDeviceInfo,
        ActionDescription = reportDetails.ActionDescription,
        Ext4 = reportDetails.LocationInfo,
        Ext5 = reportDetails.ThorVersion,
        Ext7 = Convert.ToString(reportDetails.GetExt7Field(), (IFormatProvider) CultureInfo.InvariantCulture),
        Ext8 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) reportDetails.AverageDownloadSpeed),
        Ext9 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:F2}", (object) reportDetails.AverageFlashingSpeed)
      };
      try
      {
        ReportSender.SaveAzureReport(Path.ChangeExtension(Path.Combine(SpecialFolders.Reports, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0:yyyyMMdd}_{0:HHmmss_ff}.dat", (object) DateTime.Now)), "xml"), reportDetails);
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Saving report failed", objArray);
      }
    }

    private static void SaveAzureReport(string reportFileName, ReportDetails reportDetails)
    {
      ReportData reportData1 = new ReportData();
      LsuProReportItem reportData2 = new LsuProReportItem();
      reportData2.Timestamp = DateTime.Now;
      reportData2.ProductType = reportDetails.ProductType;
      reportData2.ProductCode = reportDetails.ProductCode;
      reportData2.NewProductCode = reportDetails.ProductCodeNew;
      reportData2.SoftwareVersion = reportDetails.SoftwareVersion;
      reportData2.NewSoftwareVersion = reportDetails.SoftwareVersionNew;
      reportData2.HwId = reportDetails.HwId;
      reportData2.RdcStatus = reportDetails.RdInfo;
      reportData2.URI = reportDetails.Uri.ToString();
      reportData2.UriDescription = reportDetails.UriDescription;
      reportData2.ApiError = reportDetails.ApiError;
      reportData2.ApiErrorDescription = reportDetails.ApiErrorText;
      reportData2.ComputerId = reportData1.ComputerId;
      reportData2.ApplicationVersion = reportData1.ClientApplicationVersion;
      reportData2.Thor2Version = reportDetails.ThorVersion;
      reportData2.AverageDownloadSpeed = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) reportDetails.AverageDownloadSpeed);
      reportData2.AverageFlashingSpeed = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:F2}", (object) reportDetails.AverageFlashingSpeed);
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (SoftwareRepositoryReportData));
      XmlWriter xmlWriter1 = (XmlWriter) new XmlTextWriter(reportFileName, Encoding.UTF8);
      XmlWriter xmlWriter2 = xmlWriter1;
      SoftwareRepositoryReportData repositoryReportData = new SoftwareRepositoryReportData(reportData2);
      xmlSerializer.Serialize(xmlWriter2, (object) repositoryReportData);
      xmlWriter1.Close();
    }

    public void SendReports()
    {
      Tracer.Information("Starting pooling task Azure");
      this.poolingTaskAzure = new TaskHelper(new Action(this.SendReportsTaskAzure), this.poolingTaskAzureCancellationToken);
      this.poolingTaskAzure.Start();
      this.poolingTaskAzure.ContinueWith((Action<object>) (t =>
      {
        if (this.poolingTaskAzure.Exception == null)
          return;
        foreach (Exception innerException in this.poolingTaskAzure.Exception.InnerExceptions)
          Tracer.Error(innerException.Message);
      }), TaskContinuationOptions.OnlyOnFaulted);
    }

    public void Stop()
    {
      if (this.poolingTaskAzure == null)
        return;
      this.poolingTaskAzureCancellationTokenSource.Cancel();
      try
      {
        this.poolingTaskAzure.Wait();
      }
      catch (Exception ex)
      {
        Tracer.Information("Pooling operation Azure was cancelled: {0}", (object) ex.Message);
      }
      this.poolingTaskAzure.Dispose();
    }

    private void SendReportsTaskAzure()
    {
      string path = Path.Combine(SpecialFolders.Logs, "LsuProStart_crashed.txt");
      if (System.IO.File.Exists(path))
      {
        StreamReader streamReader = new StreamReader(path);
        string end = streamReader.ReadToEnd();
        streamReader.Close();
        System.IO.File.Delete(path);
        DateTime result;
        if (DateTime.TryParse(end, out result))
          this.SaveLsuProCrashReport(result);
      }
      int failureCounter = 0;
      while (!this.poolingTaskAzureCancellationToken.IsCancellationRequested && (!this.NetworkAvailable || !this.SendReportsForAllXmlFiles(DirectoryHelper.GetFiles(SpecialFolders.Reports, "*.xml", SearchOption.AllDirectories), ref failureCounter)))
      {
        Task.Delay(10000, this.poolingTaskAzureCancellationToken).Wait();
        if (this.poolingTaskAzureCancellationToken.IsCancellationRequested)
          break;
      }
    }

    private bool SendReportsForAllXmlFiles(string[] reportFileNames, ref int failureCounter)
    {
      foreach (string reportFileName in reportFileNames)
      {
        try
        {
          Tracer.Information("Processing report '{0}'", (object) PathHelper.GetFileName(reportFileName));
          this.SendReportAzure(reportFileName);
        }
        catch (Exception ex1)
        {
          Tracer.Error(ex1, "Sending report failed: {0}", (object) ex1.Message);
          ++failureCounter;
          if (failureCounter > 3)
          {
            Tracer.Error("Failure counter reach maximum, sending task will be terminated");
            try
            {
              this.SaveFailureReport(ex1);
            }
            catch (Exception ex2)
            {
              Tracer.Warning("Failed to send failed report: {0}", (object) ex2.Message);
            }
            return true;
          }
          if (!Toolkit.CancellableWait(30, this.poolingTaskAzureCancellationToken))
          {
            Tracer.Information("Cancellation requested, sending task will be terminated");
            return true;
          }
        }
      }
      return false;
    }

    private void SendReportAzure(string reportFileName)
    {
      try
      {
        LsuProReportItem lsuProReportItem1 = new LsuProReportItem();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (SoftwareRepositoryReportData));
        XmlReader xmlReader1 = (XmlReader) new XmlTextReader(reportFileName);
        XmlReader xmlReader2 = xmlReader1;
        SoftwareRepositoryReportData repositoryReportData = xmlSerializer.Deserialize(xmlReader2) as SoftwareRepositoryReportData;
        xmlReader1.Close();
        lsuProReportItem1.AkVersion = repositoryReportData.AkVersion;
        lsuProReportItem1.ApiError = repositoryReportData.ApiError;
        lsuProReportItem1.ApiErrorDescription = repositoryReportData.ApiErrorDescription;
        lsuProReportItem1.ApplicationVersion = repositoryReportData.ApplicationVersion;
        lsuProReportItem1.AverageDownloadSpeed = repositoryReportData.AverageDownloadSpeed;
        lsuProReportItem1.AverageFlashingSpeed = repositoryReportData.AverageFlashingSpeed;
        lsuProReportItem1.BasicProductCode = repositoryReportData.BasicProductCode;
        lsuProReportItem1.BspVersion = repositoryReportData.BspVersion;
        lsuProReportItem1.ComputerId = repositoryReportData.ComputerId;
        lsuProReportItem1.DevelopmentPc = repositoryReportData.DevelopmentPc;
        lsuProReportItem1.DisablePiaReading = repositoryReportData.DisablePiaReading;
        lsuProReportItem1.FactoryResetEnabled = repositoryReportData.FactoryResetEnabled;
        lsuProReportItem1.HwId = repositoryReportData.HwId;
        lsuProReportItem1.Id = repositoryReportData.Id;
        lsuProReportItem1.NcsdVersion = repositoryReportData.NcsdVersion;
        lsuProReportItem1.NewProductCode = repositoryReportData.NewProductCode;
        lsuProReportItem1.NewSoftwareVersion = repositoryReportData.NewSoftwareVersion;
        lsuProReportItem1.OutsideCompanyNetwork = repositoryReportData.OutsideCompanyNetwork;
        lsuProReportItem1.ProductCode = repositoryReportData.ProductCode;
        lsuProReportItem1.ProductType = repositoryReportData.ProductType;
        lsuProReportItem1.RdcStatus = repositoryReportData.RdcStatus;
        lsuProReportItem1.SkipFfuIntegrityCheck = repositoryReportData.SkipFfuIntegrityCheck;
        lsuProReportItem1.SkipPlatformIdCheck = repositoryReportData.SkipPlatformIdCheck;
        lsuProReportItem1.SkipSignatureCheck = repositoryReportData.SkipSignatureCheck;
        lsuProReportItem1.SkipWrite = repositoryReportData.SkipWrite;
        lsuProReportItem1.SoftwareVersion = repositoryReportData.SoftwareVersion;
        lsuProReportItem1.TestingPc = repositoryReportData.TestingPc;
        lsuProReportItem1.Thor2Version = repositoryReportData.Thor2Version;
        lsuProReportItem1.Timestamp = repositoryReportData.Timestamp;
        lsuProReportItem1.URI = repositoryReportData.URI;
        lsuProReportItem1.UriDescription = repositoryReportData.UriDescription;
        DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (LsuProReportItem), new DataContractJsonSerializerSettings()
        {
          DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy-MM-ddThh:mm:ss")
        });
        MemoryStream memoryStream1 = new MemoryStream();
        MemoryStream memoryStream2 = memoryStream1;
        LsuProReportItem lsuProReportItem2 = lsuProReportItem1;
        contractJsonSerializer.WriteObject((Stream) memoryStream2, (object) lsuProReportItem2);
        memoryStream1.Seek(0L, SeekOrigin.Begin);
        string end = new StreamReader((Stream) memoryStream1).ReadToEnd();
        using (HttpClient httpClient = new HttpClient())
        {
          httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          Task<HttpResponseMessage> task = httpClient.PostAsync("http://lsureport.azurewebsites.net/api/addReport/", (HttpContent) new StringContent(end, Encoding.UTF8, "application/json"));
          task.Wait();
          if (task.Result.StatusCode != HttpStatusCode.OK)
            return;
          this.DeleteFileSafe(reportFileName);
        }
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Report sending failed", objArray);
        throw;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void SaveFailureReport(Exception ex) => ReportSender.SaveReportAsync(new ReportDetails()
    {
      Uri = 104999L,
      UriDescription = "Failed to send report logs",
      DebugField = ex.Message + ", StackTrace: " + ex.StackTrace
    });

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void SaveLsuProCrashReport(DateTime time) => ReportSender.SaveReportAsync(new ReportDetails()
    {
      Uri = 104900L,
      UriDescription = "LumiaSoftwareUpdaterPro main process crash"
    }, time);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private void DeleteFileSafe(string fileName)
    {
      try
      {
        if (string.IsNullOrEmpty(fileName))
          return;
        System.IO.File.Delete(fileName);
        Tracer.Information("File deleted: {0}", (object) fileName);
      }
      catch (Exception ex)
      {
        Tracer.Warning(ex, "Cannot delete file: {0}", (object) ex.Message);
      }
    }
  }
}
