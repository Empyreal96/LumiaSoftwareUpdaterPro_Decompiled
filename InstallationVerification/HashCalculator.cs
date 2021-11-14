// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.HashCalculator
// Assembly: InstallationVerification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B069B578-AC80-45C8-8C18-1EB17B2F19C8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\InstallationVerification.exe

using System;
using System.Globalization;
using System.IO;
using Vurdalakov;

namespace Microsoft.LsuPro
{
  public class HashCalculator
  {
    public event EventHandler<CalculationProgressEventArgs> CalculationProgress;

    public static string Crc32BytesToString(byte[] hash) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:X8}", (object) BitConverter.ToUInt32(hash, 0));

    public byte[] CalculateCrc32(string file) => this.CalculateCrc32(new FileInfo(file));

    public bool Cancelled { get; private set; }

    public byte[] CalculateCrc32(FileInfo file)
    {
      this.Cancelled = false;
      CRC32Managed crC32Managed = new CRC32Managed();
      byte[] numArray = new byte[65536];
      long num = 0;
      uint crc32Hash;
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
        this.Cancelled = true;
label_4:
        crC32Managed.TransformFinalBlock(numArray, 0, 0);
        crc32Hash = crC32Managed.Crc32Hash;
      }
      return BitConverter.GetBytes(crc32Hash);
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
