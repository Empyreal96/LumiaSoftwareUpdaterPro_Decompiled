// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.ClientMetrics
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class ClientMetrics
  {
    private const string ClientMetricsHeaderLastError = "x-client-last-error";
    private const string ClientMetricsHeaderLastRequest = "x-client-last-request";
    private const string ClientMetricsHeaderLastResponseTime = "x-client-last-response-time";
    private const string ClientMetricsHeaderLastEndpoint = "x-client-last-endpoint";
    private static ClientMetrics pendingClientMetrics;
    private static readonly object PendingClientMetricsLock = new object();
    private Stopwatch metricsTimer;
    private string lastError;
    private Guid lastCorrelationId;
    private long lastResponseTime;
    private string lastEndpoint;

    public void BeginClientMetricsRecord(IHttpWebRequest request, CallState callState)
    {
      if (callState == null || callState.AuthorityType != AuthorityType.AAD)
        return;
      ClientMetrics.AddClientMetricsHeadersToRequest(request);
      this.metricsTimer = Stopwatch.StartNew();
    }

    public void EndClientMetricsRecord(string endpoint, CallState callState)
    {
      if (callState == null || callState.AuthorityType != AuthorityType.AAD || this.metricsTimer == null)
        return;
      this.metricsTimer.Stop();
      this.lastResponseTime = this.metricsTimer.ElapsedMilliseconds;
      this.lastCorrelationId = callState.CorrelationId;
      this.lastEndpoint = endpoint;
      lock (ClientMetrics.PendingClientMetricsLock)
      {
        if (ClientMetrics.pendingClientMetrics != null)
          return;
        ClientMetrics.pendingClientMetrics = this;
      }
    }

    public void SetLastError(string[] errorCodes) => this.lastError = errorCodes != null ? string.Join(",", errorCodes) : (string) null;

    private static void AddClientMetricsHeadersToRequest(IHttpWebRequest request)
    {
      lock (ClientMetrics.PendingClientMetricsLock)
      {
        if (ClientMetrics.pendingClientMetrics == null || !NetworkPlugin.RequestCreationHelper.RecordClientMetrics)
          return;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        if (ClientMetrics.pendingClientMetrics.lastError != null)
          headers["x-client-last-error"] = ClientMetrics.pendingClientMetrics.lastError;
        headers["x-client-last-request"] = ClientMetrics.pendingClientMetrics.lastCorrelationId.ToString();
        headers["x-client-last-response-time"] = ClientMetrics.pendingClientMetrics.lastResponseTime.ToString();
        headers["x-client-last-endpoint"] = ClientMetrics.pendingClientMetrics.lastEndpoint;
        HttpHelper.AddHeadersToRequest(request, headers);
        ClientMetrics.pendingClientMetrics = (ClientMetrics) null;
      }
    }
  }
}
