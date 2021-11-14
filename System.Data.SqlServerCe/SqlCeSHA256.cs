// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.SqlCeSHA256
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace System.Data.SqlServerCe
{
  internal sealed class SqlCeSHA256 : SHA256
  {
    public const string MicrosoftEnhancedRsaAes = "Microsoft Enhanced RSA and AES Cryptographic Provider";
    public const string MicrosoftEnhancedRsaAesPrototype = "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)";
    private SqlCeSHA256.AlgorithmId m_algorithmId;
    [SecurityCritical]
    private CspSafeHandle m_cspHandle;
    [SecurityCritical]
    private HashSafeHandle m_hashHandle;

    static SqlCeSHA256() => KillBitHelper.ThrowIfKillBitIsSet();

    [SecurityCritical]
    internal SqlCeSHA256()
    {
      string provider = "Microsoft Enhanced RSA and AES Cryptographic Provider";
      if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1)
        provider = "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)";
      this.InitFields(provider, SqlCeSHA256.ProviderType.RsaAes, SqlCeSHA256.AlgorithmId.Sha256);
    }

    [SecurityCritical]
    private void InitFields(
      string provider,
      SqlCeSHA256.ProviderType providerType,
      SqlCeSHA256.AlgorithmId algorithm)
    {
      this.m_algorithmId = algorithm;
      this.m_cspHandle = SqlCeSHA256.AcquireCsp((string) null, provider, providerType, SqlCeSHA256.CryptAcquireContextFlags.VerifyContext);
      this.Initialize();
    }

    [SecurityCritical]
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (ibStart < 0 || ibStart > array.Length - cbSize)
        throw new ArgumentOutOfRangeException(nameof (ibStart));
      if (cbSize < 0 || cbSize > array.Length)
        throw new ArgumentOutOfRangeException(nameof (cbSize));
      byte[] pbData = new byte[cbSize];
      Buffer.BlockCopy((Array) array, ibStart, (Array) pbData, 0, cbSize);
      if (!SqlCeSHA256.UnsafeNativeMethods.CryptHashData(this.m_hashHandle, pbData, cbSize, 0))
        throw new CryptographicException(Marshal.GetLastWin32Error());
    }

    [SecurityCritical]
    protected override byte[] HashFinal() => SqlCeSHA256.GetHashParameter(this.m_hashHandle, SqlCeSHA256.HashParameter.HashValue);

    [SecurityCritical]
    public override void Initialize()
    {
      HashSafeHandle phHash = (HashSafeHandle) null;
      if (!SqlCeSHA256.UnsafeNativeMethods.CryptCreateHash(this.m_cspHandle, this.m_algorithmId, 0, 0, out phHash))
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (lastWin32Error == -2146893816)
          throw new PlatformNotSupportedException();
        throw new CryptographicException(lastWin32Error);
      }
      if (this.m_hashHandle != null)
        this.m_hashHandle.Dispose();
      this.m_hashHandle = phHash;
    }

    [SecurityCritical]
    public void Dispose()
    {
      if (this.m_hashHandle != null)
        this.m_hashHandle.Dispose();
      if (this.m_cspHandle == null)
        return;
      this.m_cspHandle.Dispose();
    }

    [SecurityCritical]
    private static CspSafeHandle AcquireCsp(
      string keyContainer,
      string providerName,
      SqlCeSHA256.ProviderType providerType,
      SqlCeSHA256.CryptAcquireContextFlags flags)
    {
      CspSafeHandle phProv = (CspSafeHandle) null;
      if (SqlCeSHA256.UnsafeNativeMethods.CryptAcquireContext(out phProv, keyContainer, providerName, providerType, flags))
        return phProv;
      int lastWin32Error = Marshal.GetLastWin32Error();
      switch (lastWin32Error)
      {
        case -2146893801:
        case -2146893799:
          throw new PlatformNotSupportedException();
        default:
          throw new CryptographicException(lastWin32Error);
      }
    }

    [SecurityCritical]
    private static byte[] GetHashParameter(
      HashSafeHandle hashHandle,
      SqlCeSHA256.HashParameter parameter)
    {
      int pdwDataLen = 0;
      if (!SqlCeSHA256.UnsafeNativeMethods.CryptGetHashParam(hashHandle, parameter, (byte[]) null, ref pdwDataLen, 0))
        throw new CryptographicException(Marshal.GetLastWin32Error());
      byte[] pbData = new byte[pdwDataLen];
      if (!SqlCeSHA256.UnsafeNativeMethods.CryptGetHashParam(hashHandle, parameter, pbData, ref pdwDataLen, 0))
        throw new CryptographicException(Marshal.GetLastWin32Error());
      if (pdwDataLen != pbData.Length)
      {
        byte[] numArray = new byte[pdwDataLen];
        Buffer.BlockCopy((Array) pbData, 0, (Array) numArray, 0, pdwDataLen);
        pbData = numArray;
      }
      return pbData;
    }

    private enum AlgorithmId
    {
      None = 0,
      Aes128 = 26126, // 0x0000660E
      Aes192 = 26127, // 0x0000660F
      Aes256 = 26128, // 0x00006610
      MD5 = 32771, // 0x00008003
      Sha1 = 32772, // 0x00008004
      Sha256 = 32780, // 0x0000800C
      Sha384 = 32781, // 0x0000800D
      Sha512 = 32782, // 0x0000800E
    }

    [Flags]
    private enum CryptAcquireContextFlags
    {
      None = 0,
      VerifyContext = -268435456, // 0xF0000000
    }

    private enum ErrorCode
    {
      BadData = -2146893819, // 0x80090005
      BadAlgorithmId = -2146893816, // 0x80090008
      ProviderTypeNotDefined = -2146893801, // 0x80090017
      KeysetNotDefined = -2146893799, // 0x80090019
      Success = 0,
      MoreData = 234, // 0x000000EA
      NoMoreItems = 259, // 0x00000103
    }

    private enum HashParameter
    {
      None = 0,
      AlgorithmId = 1,
      HashValue = 2,
      HashSize = 4,
    }

    private enum ProviderType
    {
      None = 0,
      RsaAes = 24, // 0x00000018
    }

    [SecurityCritical(SecurityCriticalScope.Everything)]
    [SuppressUnmanagedCodeSecurity]
    private static class UnsafeNativeMethods
    {
      static UnsafeNativeMethods() => KillBitHelper.ThrowIfKillBitIsSet();

      [SecurityCritical]
      [SuppressUnmanagedCodeSecurity]
      [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptAcquireContext(
        out CspSafeHandle phProv,
        string pszContainer,
        string pszProvider,
        SqlCeSHA256.ProviderType dwProvType,
        SqlCeSHA256.CryptAcquireContextFlags dwFlags);

      [SuppressUnmanagedCodeSecurity]
      [SecurityCritical]
      [DllImport("advapi32", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptCreateHash(
        CspSafeHandle hProv,
        SqlCeSHA256.AlgorithmId Algid,
        int hKey,
        int dwFlags,
        out HashSafeHandle phHash);

      [SuppressUnmanagedCodeSecurity]
      [SecurityCritical]
      [DllImport("advapi32", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptGetHashParam(
        HashSafeHandle hHash,
        SqlCeSHA256.HashParameter dwParam,
        [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbData,
        [In, Out] ref int pdwDataLen,
        int dwFlags);

      [SuppressUnmanagedCodeSecurity]
      [SecurityCritical]
      [DllImport("advapi32", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptHashData(
        HashSafeHandle hHash,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pbData,
        int dwDataLen,
        int dwFlags);
    }
  }
}
