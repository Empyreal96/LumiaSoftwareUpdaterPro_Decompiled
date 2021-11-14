// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SoftwareRepository.SoftwareRepositoryReporting
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using SoftwareRepository.Reporting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.SoftwareRepository
{
  public class SoftwareRepositoryReporting
  {
    private Reporter reporter;

    public SoftwareRepositoryReporting(
      IWebProxy proxy,
      string accessToken,
      string alternateBaseUrl = null)
    {
      this.reporter = new Reporter()
      {
        SoftwareRepositoryUserAgent = "Microsoft-Lumia Software Updater Pro",
        SoftwareRepositoryProxy = proxy,
        SoftwareRepositoryAuthenticationToken = accessToken
      };
      if (string.IsNullOrEmpty(alternateBaseUrl))
        return;
      this.reporter.SoftwareRepositoryAlternativeBaseUrl = alternateBaseUrl;
    }

    public void SetAccessToken(string accessToken) => this.reporter.SoftwareRepositoryAuthenticationToken = accessToken;

    public void SetWebProxy(IWebProxy getWebProxy) => this.reporter.SoftwareRepositoryProxy = getWebProxy;

    public bool SendReport(List<string> files)
    {
      Task<bool> task = this.reporter.UploadReportAsync("Microsoft", "Lumia Software Updater Pro", "Public", files, CancellationToken.None);
      task.Wait(60000);
      if (!task.IsCompleted)
      {
        Tracer.Information("UploadReportAsync timed out");
        throw new TimeoutException();
      }
      if (task.Result)
      {
        Tracer.Information("UploadReportAsync succeed");
        return true;
      }
      Tracer.Information("UploadReportAsync failed");
      return false;
    }
  }
}
