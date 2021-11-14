// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class UserIdentifier
  {
    private const string AnyUserId = "AnyUser";
    private static readonly UserIdentifier AnyUserSingleton = new UserIdentifier(nameof (AnyUser), UserIdentifierType.UniqueId);

    public UserIdentifier(string id, UserIdentifierType type)
    {
      this.Id = !string.IsNullOrWhiteSpace(id) ? id : throw new ArgumentNullException(nameof (id));
      this.Type = type;
    }

    public UserIdentifierType Type { get; private set; }

    public string Id { get; private set; }

    public static UserIdentifier AnyUser => UserIdentifier.AnyUserSingleton;

    internal bool IsAnyUser => this.Type == UserIdentifier.AnyUser.Type && this.Id == UserIdentifier.AnyUser.Id;

    internal string UniqueId => this.IsAnyUser || this.Type != UserIdentifierType.UniqueId ? (string) null : this.Id;

    internal string DisplayableId => this.IsAnyUser || this.Type != UserIdentifierType.OptionalDisplayableId && this.Type != UserIdentifierType.RequiredDisplayableId ? (string) null : this.Id;
  }
}
