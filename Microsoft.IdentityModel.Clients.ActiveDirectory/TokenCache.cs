// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCache
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  public class TokenCache
  {
    private const int SchemaVersion = 2;
    private const string Delimiter = ":::";
    private const string LocalSettingsContainerName = "ActiveDirectoryAuthenticationLibrary";
    private const int ExpirationMarginInMinutes = 5;
    internal readonly IDictionary<TokenCacheKey, AuthenticationResult> tokenCacheDictionary;
    private volatile bool hasStateChanged;

    static TokenCache() => TokenCache.DefaultShared = new TokenCache();

    public TokenCache() => this.tokenCacheDictionary = (IDictionary<TokenCacheKey, AuthenticationResult>) new ConcurrentDictionary<TokenCacheKey, AuthenticationResult>();

    public TokenCache([ReadOnlyArray] byte[] state)
      : this()
    {
      this.Deserialize(state);
    }

    public static TokenCache DefaultShared { get; private set; }

    public TokenCacheNotification BeforeAccess { get; set; }

    public TokenCacheNotification BeforeWrite { get; set; }

    public TokenCacheNotification AfterAccess { get; set; }

    public bool HasStateChanged
    {
      get => this.hasStateChanged;
      set => this.hasStateChanged = value;
    }

    public int Count => this.tokenCacheDictionary.Count;

    public byte[] Serialize()
    {
      using (Stream stream = (Stream) new MemoryStream())
      {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(2);
        Logger.Information((CallState) null, "Serializing token cache with {0} items.", (object) this.tokenCacheDictionary.Count);
        binaryWriter.Write(this.tokenCacheDictionary.Count);
        foreach (KeyValuePair<TokenCacheKey, AuthenticationResult> tokenCache in (IEnumerable<KeyValuePair<TokenCacheKey, AuthenticationResult>>) this.tokenCacheDictionary)
        {
          binaryWriter.Write(string.Format("{1}{0}{2}{0}{3}{0}{4}", (object) ":::", (object) tokenCache.Key.Authority, (object) tokenCache.Key.Resource, (object) tokenCache.Key.ClientId, (object) (int) tokenCache.Key.TokenSubjectType));
          binaryWriter.Write(tokenCache.Value.Serialize());
        }
        int position = (int) stream.Position;
        stream.Position = 0L;
        return new BinaryReader(stream).ReadBytes(position);
      }
    }

    public void Deserialize([ReadOnlyArray] byte[] state)
    {
      if (state == null)
      {
        this.tokenCacheDictionary.Clear();
      }
      else
      {
        using (Stream stream = (Stream) new MemoryStream())
        {
          BinaryWriter binaryWriter = new BinaryWriter(stream);
          binaryWriter.Write(state);
          binaryWriter.Flush();
          stream.Position = 0L;
          BinaryReader binaryReader = new BinaryReader(stream);
          if (binaryReader.ReadInt32() != 2)
          {
            Logger.Warning((CallState) null, "The version of the persistent state of the cache does not match the current schema, so skipping deserialization.");
          }
          else
          {
            this.tokenCacheDictionary.Clear();
            int num = binaryReader.ReadInt32();
            for (int index = 0; index < num; ++index)
            {
              string[] strArray = binaryReader.ReadString().Split(new string[1]
              {
                ":::"
              }, StringSplitOptions.None);
              AuthenticationResult authenticationResult = AuthenticationResult.Deserialize(binaryReader.ReadString());
              this.tokenCacheDictionary.Add(new TokenCacheKey(strArray[0], strArray[1], strArray[2], (TokenSubjectType) int.Parse(strArray[3]), authenticationResult.UserInfo), authenticationResult);
            }
            Logger.Information((CallState) null, "Deserialized {0} items to token cache.", (object) num);
          }
        }
      }
    }

    public virtual IEnumerable<TokenCacheItem> ReadItems()
    {
      TokenCacheNotificationArgs args = new TokenCacheNotificationArgs()
      {
        TokenCache = this
      };
      this.OnBeforeAccess(args);
      List<TokenCacheItem> tokenCacheItemList = new List<TokenCacheItem>();
      foreach (KeyValuePair<TokenCacheKey, AuthenticationResult> tokenCache in (IEnumerable<KeyValuePair<TokenCacheKey, AuthenticationResult>>) this.tokenCacheDictionary)
        tokenCacheItemList.Add(new TokenCacheItem(tokenCache.Key, tokenCache.Value));
      this.OnAfterAccess(args);
      return (IEnumerable<TokenCacheItem>) tokenCacheItemList;
    }

    public virtual void DeleteItem(TokenCacheItem item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      TokenCacheNotificationArgs args = new TokenCacheNotificationArgs()
      {
        TokenCache = this,
        Resource = item.Resource,
        ClientId = item.ClientId,
        UniqueId = item.UniqueId,
        DisplayableId = item.DisplayableId
      };
      this.OnBeforeAccess(args);
      this.OnBeforeWrite(args);
      TokenCacheKey key = this.tokenCacheDictionary.Keys.FirstOrDefault<TokenCacheKey>(new Func<TokenCacheKey, bool>(item.Match));
      if (key != null)
        this.tokenCacheDictionary.Remove(key);
      this.HasStateChanged = true;
      this.OnAfterAccess(args);
    }

    public virtual void Clear()
    {
      TokenCacheNotificationArgs args = new TokenCacheNotificationArgs()
      {
        TokenCache = this
      };
      this.OnBeforeAccess(args);
      this.OnBeforeWrite(args);
      this.tokenCacheDictionary.Clear();
      this.HasStateChanged = true;
      this.OnAfterAccess(args);
    }

    internal void OnAfterAccess(TokenCacheNotificationArgs args)
    {
      if (this.AfterAccess == null)
        return;
      this.AfterAccess(args);
    }

    internal void OnBeforeAccess(TokenCacheNotificationArgs args)
    {
      if (this.BeforeAccess == null)
        return;
      this.BeforeAccess(args);
    }

    internal void OnBeforeWrite(TokenCacheNotificationArgs args)
    {
      if (this.BeforeWrite == null)
        return;
      this.BeforeWrite(args);
    }

    internal AuthenticationResult LoadFromCache(
      string authority,
      string resource,
      string clientId,
      TokenSubjectType subjectType,
      string uniqueId,
      string displayableId,
      CallState callState)
    {
      Logger.Verbose(callState, "Looking up cache for a token...");
      AuthenticationResult authenticationResult1 = (AuthenticationResult) null;
      KeyValuePair<TokenCacheKey, AuthenticationResult>? nullable = this.LoadSingleItemFromCache(authority, resource, clientId, subjectType, uniqueId, displayableId, callState);
      if (nullable.HasValue)
      {
        TokenCacheKey key = nullable.Value.Key;
        authenticationResult1 = nullable.Value.Value;
        if (authenticationResult1.ExpiresOn <= (DateTimeOffset) (DateTime.UtcNow + TimeSpan.FromMinutes(5.0)))
        {
          authenticationResult1.AccessToken = (string) null;
          Logger.Verbose(callState, "An expired or near expiry token was found in the cache");
        }
        else if (!key.ResourceEquals(resource))
        {
          Logger.Verbose(callState, string.Format("Multi resource refresh token for resource '{0}' will be used to acquire token for '{1}'", (object) key.Resource, (object) resource));
          AuthenticationResult authenticationResult2 = new AuthenticationResult((string) null, (string) null, authenticationResult1.RefreshToken, DateTimeOffset.MinValue);
          authenticationResult2.UpdateTenantAndUserInfo(authenticationResult1.TenantId, authenticationResult1.IdToken, authenticationResult1.UserInfo);
          authenticationResult1 = authenticationResult2;
        }
        else
          Logger.Verbose(callState, string.Format("{0} minutes left until token in cache expires", (object) (authenticationResult1.ExpiresOn - (DateTimeOffset) DateTime.UtcNow).TotalMinutes));
        if (authenticationResult1.AccessToken == null && authenticationResult1.RefreshToken == null)
        {
          this.tokenCacheDictionary.Remove(key);
          Logger.Information(callState, "An old item was removed from the cache");
          this.HasStateChanged = true;
          authenticationResult1 = (AuthenticationResult) null;
        }
        if (authenticationResult1 != null)
          Logger.Information(callState, "A matching item (access token or refresh token or both) was found in the cache");
      }
      else
        Logger.Information(callState, "No matching token was found in the cache");
      return authenticationResult1;
    }

    internal void StoreToCache(
      AuthenticationResult result,
      string authority,
      string resource,
      string clientId,
      TokenSubjectType subjectType,
      CallState callState)
    {
      Logger.Verbose(callState, "Storing token in the cache...");
      string str1 = result.UserInfo != null ? result.UserInfo.UniqueId : (string) null;
      string str2 = result.UserInfo != null ? result.UserInfo.DisplayableId : (string) null;
      this.OnBeforeWrite(new TokenCacheNotificationArgs()
      {
        Resource = resource,
        ClientId = clientId,
        UniqueId = str1,
        DisplayableId = str2
      });
      this.tokenCacheDictionary[new TokenCacheKey(authority, resource, clientId, subjectType, result.UserInfo)] = result;
      Logger.Verbose(callState, "An item was stored in the cache");
      this.UpdateCachedMrrtRefreshTokens(result, authority, clientId, subjectType);
      this.HasStateChanged = true;
    }

    private void UpdateCachedMrrtRefreshTokens(
      AuthenticationResult result,
      string authority,
      string clientId,
      TokenSubjectType subjectType)
    {
      if (result.UserInfo == null || !result.IsMultipleResourceRefreshToken)
        return;
      foreach (KeyValuePair<TokenCacheKey, AuthenticationResult> keyValuePair in this.QueryCache(authority, clientId, subjectType, result.UserInfo.UniqueId, result.UserInfo.DisplayableId).Where<KeyValuePair<TokenCacheKey, AuthenticationResult>>((Func<KeyValuePair<TokenCacheKey, AuthenticationResult>, bool>) (p => p.Value.IsMultipleResourceRefreshToken)).ToList<KeyValuePair<TokenCacheKey, AuthenticationResult>>())
        keyValuePair.Value.RefreshToken = result.RefreshToken;
    }

    private KeyValuePair<TokenCacheKey, AuthenticationResult>? LoadSingleItemFromCache(
      string authority,
      string resource,
      string clientId,
      TokenSubjectType subjectType,
      string uniqueId,
      string displayableId,
      CallState callState)
    {
      List<KeyValuePair<TokenCacheKey, AuthenticationResult>> source = this.QueryCache(authority, clientId, subjectType, uniqueId, displayableId);
      List<KeyValuePair<TokenCacheKey, AuthenticationResult>> list1 = source.Where<KeyValuePair<TokenCacheKey, AuthenticationResult>>((Func<KeyValuePair<TokenCacheKey, AuthenticationResult>, bool>) (p => p.Key.ResourceEquals(resource))).ToList<KeyValuePair<TokenCacheKey, AuthenticationResult>>();
      int num = list1.Count<KeyValuePair<TokenCacheKey, AuthenticationResult>>();
      KeyValuePair<TokenCacheKey, AuthenticationResult>? nullable = new KeyValuePair<TokenCacheKey, AuthenticationResult>?();
      if (num == 1)
      {
        Logger.Information(callState, "An item matching the requested resource was found in the cache");
        nullable = new KeyValuePair<TokenCacheKey, AuthenticationResult>?(list1.First<KeyValuePair<TokenCacheKey, AuthenticationResult>>());
      }
      else
      {
        if (num != 0)
          throw new AdalException("multiple_matching_tokens_detected");
        List<KeyValuePair<TokenCacheKey, AuthenticationResult>> list2 = source.Where<KeyValuePair<TokenCacheKey, AuthenticationResult>>((Func<KeyValuePair<TokenCacheKey, AuthenticationResult>, bool>) (p => p.Value.IsMultipleResourceRefreshToken)).ToList<KeyValuePair<TokenCacheKey, AuthenticationResult>>();
        if (list2.Any<KeyValuePair<TokenCacheKey, AuthenticationResult>>())
        {
          nullable = new KeyValuePair<TokenCacheKey, AuthenticationResult>?(list2.First<KeyValuePair<TokenCacheKey, AuthenticationResult>>());
          Logger.Information(callState, "A Multi Resource Refresh Token for a different resource was found which can be used");
        }
      }
      return nullable;
    }

    private List<KeyValuePair<TokenCacheKey, AuthenticationResult>> QueryCache(
      string authority,
      string clientId,
      TokenSubjectType subjectType,
      string uniqueId,
      string displayableId)
    {
      return this.tokenCacheDictionary.Where<KeyValuePair<TokenCacheKey, AuthenticationResult>>((Func<KeyValuePair<TokenCacheKey, AuthenticationResult>, bool>) (p => p.Key.Authority == authority && (string.IsNullOrWhiteSpace(clientId) || p.Key.ClientIdEquals(clientId)) && ((string.IsNullOrWhiteSpace(uniqueId) || p.Key.UniqueId == uniqueId) && (string.IsNullOrWhiteSpace(displayableId) || p.Key.DisplayableIdEquals(displayableId))) && p.Key.TokenSubjectType == subjectType)).ToList<KeyValuePair<TokenCacheKey, AuthenticationResult>>();
    }

    internal delegate Task<AuthenticationResult> RefreshAccessTokenAsync(
      AuthenticationResult result,
      string resource,
      ClientKey clientKey,
      CallState callState);
  }
}
