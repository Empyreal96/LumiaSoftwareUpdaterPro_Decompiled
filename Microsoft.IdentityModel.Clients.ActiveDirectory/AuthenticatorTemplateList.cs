// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticatorTemplateList
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AuthenticatorTemplateList : List<AuthenticatorTemplate>
  {
    public AuthenticatorTemplateList()
    {
      string[] strArray = new string[4]
      {
        "login.windows.net",
        "login.chinacloudapi.cn",
        "login.cloudgovapi.us",
        "login.microsoftonline.com"
      };
      string environmentVariable = PlatformSpecificHelper.GetEnvironmentVariable("customTrustedHost");
      if (string.IsNullOrWhiteSpace(environmentVariable))
      {
        foreach (string host in strArray)
          this.Add(AuthenticatorTemplate.CreateFromHost(host));
      }
      else
        this.Add(AuthenticatorTemplate.CreateFromHost(environmentVariable));
    }

    public async Task<AuthenticatorTemplate> FindMatchingItemAsync(
      bool validateAuthority,
      string host,
      string tenant,
      CallState callState)
    {
      AuthenticatorTemplate matchingAuthenticatorTemplate = (AuthenticatorTemplate) null;
      if (validateAuthority)
      {
        matchingAuthenticatorTemplate = this.FirstOrDefault<AuthenticatorTemplate>((Func<AuthenticatorTemplate, bool>) (a => string.Compare(host, a.Host, StringComparison.OrdinalIgnoreCase) == 0));
        if (matchingAuthenticatorTemplate == null)
          await this.First<AuthenticatorTemplate>().VerifyAnotherHostByInstanceDiscoveryAsync(host, tenant, callState);
      }
      return matchingAuthenticatorTemplate ?? AuthenticatorTemplate.CreateFromHost(host);
    }
  }
}
