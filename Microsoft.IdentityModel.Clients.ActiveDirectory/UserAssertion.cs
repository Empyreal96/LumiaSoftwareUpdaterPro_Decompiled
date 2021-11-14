// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.UserAssertion
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public sealed class UserAssertion
  {
    public UserAssertion(string assertion) => this.Assertion = !string.IsNullOrWhiteSpace(assertion) ? assertion : throw new ArgumentNullException(nameof (assertion));

    public UserAssertion(string assertion, string assertionType)
      : this(assertion, assertionType, (string) null)
    {
    }

    public UserAssertion(string assertion, string assertionType, string userName)
    {
      if (string.IsNullOrWhiteSpace(assertion))
        throw new ArgumentNullException(nameof (assertion));
      this.AssertionType = !string.IsNullOrWhiteSpace(assertionType) ? assertionType : throw new ArgumentNullException(nameof (assertionType));
      this.Assertion = assertion;
      this.UserName = userName;
    }

    public string Assertion { get; private set; }

    public string AssertionType { get; private set; }

    public string UserName { get; internal set; }
  }
}
