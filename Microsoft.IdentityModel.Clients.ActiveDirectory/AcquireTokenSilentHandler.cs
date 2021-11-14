// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AcquireTokenSilentHandler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class AcquireTokenSilentHandler : AcquireTokenHandlerBase
  {
    public AcquireTokenSilentHandler(
      Authenticator authenticator,
      TokenCache tokenCache,
      string resource,
      ClientKey clientKey,
      UserIdentifier userId,
      bool callSync)
      : base(authenticator, tokenCache, resource, clientKey, clientKey.HasCredential ? TokenSubjectType.UserPlusClient : TokenSubjectType.User, callSync)
    {
      this.UniqueId = userId != null ? userId.UniqueId : throw new ArgumentNullException(nameof (userId), "If you do not need access token for any specific user, pass userId=UserIdentifier.AnyUser instead of userId=null.");
      this.DisplayableId = userId.DisplayableId;
      this.UserIdentifierType = userId.Type;
      this.SupportADFS = true;
    }

    protected override Task<AuthenticationResult> SendTokenRequestAsync()
    {
      Logger.Verbose(this.CallState, "No token matching arguments found in the cache");
      throw new AdalSilentTokenAcquisitionException();
    }

    protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
    {
    }
  }
}
