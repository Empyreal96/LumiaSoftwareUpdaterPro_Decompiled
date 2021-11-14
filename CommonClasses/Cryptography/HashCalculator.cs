// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Cryptography.HashCalculator
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using Vurdalakov;

namespace Microsoft.LsuPro.Cryptography
{
  public class HashCalculator
  {
    public event EventHandler<CalculationProgressEventArgs> CalculationProgress;

    public static string GetMd5ChecksumForFile(string pathToFile)
    {
      using (MD5 md5 = MD5.Create())
      {
        using (FileStream fileStream = File.OpenRead(pathToFile))
          return BitConverter.ToString(md5.ComputeHash((Stream) fileStream)).Replace("-", string.Empty);
      }
    }

    public static string Crc32BytesToString(byte[] hash) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:X8}", (object) BitConverter.ToUInt32(hash, 0));

    public byte[] CalculateCrc32(string file) => this.CalculateCrc32(new FileInfo(file));

    public bool Canceled { get; private set; }

    public byte[] CalculateCrc32(FileInfo file)
    {
      this.Canceled = false;
      CRC32Managed crC32Managed = new CRC32Managed();
      byte[] numArray = new byte[65536];
      long num = 0;
      uint crC32Hash;
      using (Stream stream = (Stream) new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        do
        {
          int inputCount = stream.Read(numArray, 0, 65536);
          if (inputCount != 0)
          {
            num += (long) inputCount;
            crC32Managed.TransformBlock(numArray, 0, inputCount, numArray, 0);
          }
          else
            goto label_4;
        }
        while (!this.SendProgressAndCheckCancellation((int) (100L * num / file.Length)));
        this.Canceled = true;
label_4:
        crC32Managed.TransformFinalBlock(numArray, 0, 0);
        crC32Hash = crC32Managed.CRC32Hash;
      }
      return BitConverter.GetBytes(crC32Hash);
    }

    private bool SendProgressAndCheckCancellation(int progressPercentage)
    {
      EventHandler<CalculationProgressEventArgs> calculationProgress = this.CalculationProgress;
      if (calculationProgress == null)
        return false;
      CalculationProgressEventArgs e = new CalculationProgressEventArgs(progressPercentage);
      calculationProgress((object) this, e);
      return e.Cancel;
    }
  }
}
