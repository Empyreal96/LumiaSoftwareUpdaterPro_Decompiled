// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AuthorizationResult
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [DataContract]
  internal class AuthorizationResult
  {
    internal AuthorizationResult(string code)
    {
      this.Status = AuthorizationStatus.Success;
      this.Code = code;
    }

    internal AuthorizationResult(string error, string errorDescription)
    {
      this.Status = AuthorizationStatus.Failed;
      this.Error = error;
      this.ErrorDescription = errorDescription;
    }

    public AuthorizationStatus Status { get; private set; }

    [DataMember]
    public string Code { get; private set; }

    [DataMember]
    public string Error { get; private set; }

    [DataMember]
    public string ErrorDescription { get; private set; }
  }
}
