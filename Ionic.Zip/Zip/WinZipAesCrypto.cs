// Decompiled with JetBrains decompiler
// Type: Ionic.Zip.WinZipAesCrypto
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using System;
using System.IO;
using System.Security.Cryptography;

namespace Ionic.Zip
{
  internal class WinZipAesCrypto
  {
    internal byte[] _Salt;
    internal byte[] _providedPv;
    internal byte[] _generatedPv;
    internal int _KeyStrengthInBits;
    private byte[] _MacInitializationVector;
    private byte[] _StoredMac;
    private byte[] _keyBytes;
    private short PasswordVerificationStored;
    private short PasswordVerificationGenerated;
    private int Rfc2898KeygenIterations = 1000;
    private string _Password;
    private bool _cryptoGenerated;
    public byte[] CalculatedMac;

    private WinZipAesCrypto(string password, int KeyStrengthInBits)
    {
      this._Password = password;
      this._KeyStrengthInBits = KeyStrengthInBits;
    }

    public static WinZipAesCrypto Generate(string password, int KeyStrengthInBits)
    {
      WinZipAesCrypto winZipAesCrypto = new WinZipAesCrypto(password, KeyStrengthInBits);
      int length = winZipAesCrypto._KeyStrengthInBytes / 2;
      winZipAesCrypto._Salt = new byte[length];
      new Random().NextBytes(winZipAesCrypto._Salt);
      return winZipAesCrypto;
    }

    public static WinZipAesCrypto ReadFromStream(
      string password,
      int KeyStrengthInBits,
      Stream s)
    {
      WinZipAesCrypto winZipAesCrypto = new WinZipAesCrypto(password, KeyStrengthInBits);
      int length = winZipAesCrypto._KeyStrengthInBytes / 2;
      winZipAesCrypto._Salt = new byte[length];
      winZipAesCrypto._providedPv = new byte[2];
      s.Read(winZipAesCrypto._Salt, 0, winZipAesCrypto._Salt.Length);
      s.Read(winZipAesCrypto._providedPv, 0, winZipAesCrypto._providedPv.Length);
      winZipAesCrypto.PasswordVerificationStored = (short) ((int) winZipAesCrypto._providedPv[0] + (int) winZipAesCrypto._providedPv[1] * 256);
      if (password != null)
      {
        winZipAesCrypto.PasswordVerificationGenerated = (short) ((int) winZipAesCrypto.GeneratedPV[0] + (int) winZipAesCrypto.GeneratedPV[1] * 256);
        if ((int) winZipAesCrypto.PasswordVerificationGenerated != (int) winZipAesCrypto.PasswordVerificationStored)
          throw new BadPasswordException("bad password");
      }
      return winZipAesCrypto;
    }

    public byte[] GeneratedPV
    {
      get
      {
        if (!this._cryptoGenerated)
          this._GenerateCryptoBytes();
        return this._generatedPv;
      }
    }

    public byte[] Salt => this._Salt;

    private int _KeyStrengthInBytes => this._KeyStrengthInBits / 8;

    public int SizeOfEncryptionMetadata => this._KeyStrengthInBytes / 2 + 10 + 2;

    public string Password
    {
      set
      {
        this._Password = value;
        if (this._Password == null)
          return;
        this.PasswordVerificationGenerated = (short) ((int) this.GeneratedPV[0] + (int) this.GeneratedPV[1] * 256);
        if ((int) this.PasswordVerificationGenerated != (int) this.PasswordVerificationStored)
          throw new BadPasswordException();
      }
      private get => this._Password;
    }

    private void _GenerateCryptoBytes()
    {
      Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(this._Password, this.Salt, this.Rfc2898KeygenIterations);
      this._keyBytes = rfc2898DeriveBytes.GetBytes(this._KeyStrengthInBytes);
      this._MacInitializationVector = rfc2898DeriveBytes.GetBytes(this._KeyStrengthInBytes);
      this._generatedPv = rfc2898DeriveBytes.GetBytes(2);
      this._cryptoGenerated = true;
    }

    public byte[] KeyBytes
    {
      get
      {
        if (!this._cryptoGenerated)
          this._GenerateCryptoBytes();
        return this._keyBytes;
      }
    }

    public byte[] MacIv
    {
      get
      {
        if (!this._cryptoGenerated)
          this._GenerateCryptoBytes();
        return this._MacInitializationVector;
      }
    }

    public void ReadAndVerifyMac(Stream s)
    {
      bool flag = false;
      this._StoredMac = new byte[10];
      s.Read(this._StoredMac, 0, this._StoredMac.Length);
      if (this._StoredMac.Length != this.CalculatedMac.Length)
        flag = true;
      if (!flag)
      {
        for (int index = 0; index < this._StoredMac.Length; ++index)
        {
          if ((int) this._StoredMac[index] != (int) this.CalculatedMac[index])
            flag = true;
        }
      }
      if (flag)
        throw new BadStateException("The MAC does not match.");
    }
  }
}
