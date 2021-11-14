// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.UserInfo
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [DataContract]
  public sealed class UserInfo
  {
    internal UserInfo()
    {
    }

    internal UserInfo(UserInfo other)
    {
      this.UniqueId = other.UniqueId;
      this.DisplayableId = other.DisplayableId;
      this.GivenName = other.GivenName;
      this.FamilyName = other.FamilyName;
      this.IdentityProvider = other.IdentityProvider;
      this.PasswordChangeUrl = other.PasswordChangeUrl;
      this.PasswordExpiresOn = other.PasswordExpiresOn;
    }

    [DataMember]
    public string UniqueId { get; internal set; }

    [DataMember]
    public string DisplayableId { get; internal set; }

    [DataMember]
    public string GivenName { get; internal set; }

    [DataMember]
    public string FamilyName { get; internal set; }

    [DataMember]
    public DateTimeOffset? PasswordExpiresOn { get; internal set; }

    [DataMember]
    public Uri PasswordChangeUrl { get; internal set; }

    [DataMember]
    public string IdentityProvider { get; internal set; }

    internal bool ForcePrompt { get; private set; }
  }
}
