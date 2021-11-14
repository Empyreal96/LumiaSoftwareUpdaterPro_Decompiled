// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.RequestParameters
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal class RequestParameters : Dictionary<string, string>
  {
    private readonly StringBuilder stringBuilderParameter;
    private Dictionary<string, SecureString> secureParameters;

    public RequestParameters(string resource, ClientKey clientKey)
    {
      if (!string.IsNullOrWhiteSpace(resource))
        this[nameof (resource)] = resource;
      this.AddClientKey(clientKey);
    }

    public RequestParameters(StringBuilder stringBuilderParameter) => this.stringBuilderParameter = stringBuilderParameter;

    public string ExtraQueryParameter { get; set; }

    public override string ToString() => this.ToStringBuilder().ToString();

    public void WriteToStream(Stream stream)
    {
      StringBuilder stringBuilder = this.ToStringBuilder();
      byte[] numArray = (byte[]) null;
      try
      {
        numArray = stringBuilder.ToByteArray();
        stream.Write(numArray, 0, numArray.Length);
      }
      finally
      {
        numArray.SecureClear();
        stringBuilder.SecureClear();
      }
    }

    private StringBuilder ToStringBuilder()
    {
      StringBuilder messageBuilder = new StringBuilder();
      if (this.stringBuilderParameter != null)
        messageBuilder.Append((object) this.stringBuilderParameter);
      foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) this)
        EncodingHelper.AddKeyValueString(messageBuilder, EncodingHelper.UrlEncode(keyValuePair.Key), EncodingHelper.UrlEncode(keyValuePair.Value));
      this.AddSecureParametersToMessageBuilder(messageBuilder);
      if (this.ExtraQueryParameter != null)
        messageBuilder.Append('&'.ToString() + this.ExtraQueryParameter);
      return messageBuilder;
    }

    public void AddSecureParameter(string key, SecureString value)
    {
      if (this.secureParameters == null)
        this.secureParameters = new Dictionary<string, SecureString>();
      this.secureParameters.Add(key, value);
    }

    private void AddSecureParametersToMessageBuilder(StringBuilder messageBuilder)
    {
      if (this.secureParameters == null)
        return;
      foreach (KeyValuePair<string, SecureString> secureParameter in this.secureParameters)
      {
        char[] chars = (char[]) null;
        try
        {
          chars = secureParameter.Value.ToCharArray();
          EncodingHelper.AddStringWithUrlEncoding(messageBuilder, secureParameter.Key, chars);
        }
        finally
        {
          chars.SecureClear();
        }
      }
    }

    private void AddClientKey(ClientKey clientKey)
    {
      if (clientKey.ClientId != null)
        this["client_id"] = clientKey.ClientId;
      if (clientKey.Credential != null)
      {
        if (clientKey.Credential.ClientSecret != null)
          this["client_secret"] = clientKey.Credential.ClientSecret;
        else
          this.AddSecureParameter("client_secret", clientKey.Credential.SecureClientSecret);
      }
      else if (clientKey.Assertion != null)
      {
        this["client_assertion_type"] = clientKey.Assertion.AssertionType;
        this["client_assertion"] = clientKey.Assertion.Assertion;
      }
      else
      {
        if (clientKey.Certificate == null)
          return;
        ClientAssertion clientAssertion = new JsonWebToken(clientKey.Certificate, clientKey.Authenticator.SelfSignedJwtAudience).Sign(clientKey.Certificate);
        this["client_assertion_type"] = clientAssertion.AssertionType;
        this["client_assertion"] = clientAssertion.Assertion;
      }
    }
  }
}
