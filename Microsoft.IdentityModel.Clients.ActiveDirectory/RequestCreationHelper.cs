// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.RequestCreationHelper
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class RequestCreationHelper : IRequestCreationHelper
  {
    public bool RecordClientMetrics => true;

    public void AddAdalIdParameters(IDictionary<string, string> parameters)
    {
      parameters["x-client-SKU"] = PlatformSpecificHelper.GetProductName();
      parameters["x-client-Ver"] = AdalIdHelper.GetAdalVersion();
      parameters["x-client-CPU"] = AdalIdHelper.GetProcessorArchitecture();
      parameters["x-client-OS"] = Environment.OSVersion.ToString();
    }

    public DateTime GetJsonWebTokenValidFrom() => DateTime.UtcNow;

    public string GetJsonWebTokenId() => Guid.NewGuid().ToString();
  }
}
