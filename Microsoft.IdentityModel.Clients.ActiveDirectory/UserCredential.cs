// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.UserCredential
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Security;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class UserCredential
  {
    public UserCredential() => this.UserAuthType = UserAuthType.IntegratedAuth;

    public UserCredential(string userName)
    {
      this.UserName = userName;
      this.UserAuthType = UserAuthType.IntegratedAuth;
    }

    public string UserName { get; internal set; }

    internal UserAuthType UserAuthType { get; private set; }

    public UserCredential(string userName, string password)
    {
      this.UserName = userName;
      this.Password = password;
      this.UserAuthType = UserAuthType.UsernamePassword;
    }

    public UserCredential(string userName, SecureString securePassword)
    {
      this.UserName = userName;
      this.SecurePassword = securePassword;
      this.UserAuthType = UserAuthType.UsernamePassword;
    }

    internal string Password { get; private set; }

    internal SecureString SecurePassword { get; private set; }

    internal char[] PasswordToCharArray()
    {
      if (this.SecurePassword != null)
        return this.SecurePassword.ToCharArray();
      return this.Password == null ? (char[]) null : this.Password.ToCharArray();
    }
  }
}
