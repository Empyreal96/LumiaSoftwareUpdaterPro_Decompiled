// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.BZip2Compressor
// Assembly: Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c
// MVID: BBD9ABA3-3797-4E5D-B8C5-A361E0F7EC0C
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Ionic.Zip.dll

using Ionic.Crc;
using System;

namespace Ionic.BZip2
{
  internal class BZip2Compressor
  {
    private int blockSize100k;
    private int currentByte = -1;
    private int runLength;
    private int last;
    private int outBlockFillThreshold;
    private BZip2Compressor.CompressionState cstate;
    private readonly CRC32 crc = new CRC32(true);
    private BitWriter bw;
    private int runs;
    private int workDone;
    private int workLimit;
    private bool firstAttempt;
    private bool blockRandomised;
    private int origPtr;
    private int nInUse;
    private int nMTF;
    private static readonly int SETMASK = 2097152;
    private static readonly int CLEARMASK = ~BZip2Compressor.SETMASK;
    private static readonly byte GREATER_ICOST = 15;
    private static readonly byte LESSER_ICOST = 0;
    private static readonly int SMALL_THRESH = 20;
    private static readonly int DEPTH_THRESH = 10;
    private static readonly int WORK_FACTOR = 30;
    private static readonly int[] increments = new int[14]
    {
      1,
      4,
      13,
      40,
      121,
      364,
      1093,
      3280,
      9841,
      29524,
      88573,
      265720,
      797161,
      2391484
    };

    public BZip2Compressor(BitWriter writer)
      : this(writer, Ionic.BZip2.BZip2.MaxBlockSize)
    {
    }

    public BZip2Compressor(BitWriter writer, int blockSize)
    {
      this.blockSize100k = blockSize;
      this.bw = writer;
      this.outBlockFillThreshold = blockSize * Ionic.BZip2.BZip2.BlockSizeMultiple - 20;
      this.cstate = new BZip2Compressor.CompressionState(blockSize);
      this.Reset();
    }

    private void Reset()
    {
      this.crc.Reset();
      this.currentByte = -1;
      this.runLength = 0;
      this.last = -1;
      int index = 256;
      while (--index >= 0)
        this.cstate.inUse[index] = false;
    }

    public int BlockSize => this.blockSize100k;

    public uint Crc32 { get; private set; }

    public int AvailableBytesOut { get; private set; }

    public int UncompressedBytes => this.last + 1;

    public int Fill(byte[] buffer, int offset, int count)
    {
      if (this.last >= this.outBlockFillThreshold)
        return 0;
      int num1 = 0;
      int num2 = offset + count;
      int num3;
      do
      {
        num3 = this.write0(buffer[offset++]);
        if (num3 > 0)
          ++num1;
      }
      while (offset < num2 && num3 == 1);
      return num1;
    }

    private int write0(byte b)
    {
      if (this.currentByte == -1)
      {
        this.currentByte = (int) b;
        ++this.runLength;
        return 1;
      }
      if (this.currentByte == (int) b)
      {
        if (++this.runLength <= 254)
          return 1;
        bool outputBlock = this.AddRunToOutputBlock(false);
        this.currentByte = -1;
        this.runLength = 0;
        return !outputBlock ? 1 : 2;
      }
      if (this.AddRunToOutputBlock(false))
      {
        this.currentByte = -1;
        this.runLength = 0;
        return 0;
      }
      this.runLength = 1;
      this.currentByte = (int) b;
      return 1;
    }

    private bool AddRunToOutputBlock(bool final)
    {
      ++this.runs;
      int last = this.last;
      if (last >= this.outBlockFillThreshold && !final)
        throw new Exception(string.Format("block overrun(final={2}): {0} >= threshold ({1})", (object) last, (object) this.outBlockFillThreshold, (object) final));
      byte currentByte = (byte) this.currentByte;
      byte[] block = this.cstate.block;
      this.cstate.inUse[(int) currentByte] = true;
      int runLength = this.runLength;
      this.crc.UpdateCRC(currentByte, runLength);
      switch (runLength)
      {
        case 1:
          block[last + 2] = currentByte;
          this.last = last + 1;
          break;
        case 2:
          block[last + 2] = currentByte;
          block[last + 3] = currentByte;
          this.last = last + 2;
          break;
        case 3:
          block[last + 2] = currentByte;
          block[last + 3] = currentByte;
          block[last + 4] = currentByte;
          this.last = last + 3;
          break;
        default:
          int index = runLength - 4;
          this.cstate.inUse[index] = true;
          block[last + 2] = currentByte;
          block[last + 3] = currentByte;
          block[last + 4] = currentByte;
          block[last + 5] = currentByte;
          block[last + 6] = (byte) index;
          this.last = last + 5;
          break;
      }
      return this.last >= this.outBlockFillThreshold;
    }

    public void CompressAndWrite()
    {
      if (this.runLength > 0)
        this.AddRunToOutputBlock(true);
      this.currentByte = -1;
      if (this.last == -1)
        return;
      this.blockSort();
      this.bw.WriteByte((byte) 49);
      this.bw.WriteByte((byte) 65);
      this.bw.WriteByte((byte) 89);
      this.bw.WriteByte((byte) 38);
      this.bw.WriteByte((byte) 83);
      this.bw.WriteByte((byte) 89);
      this.Crc32 = (uint) this.crc.Crc32Result;
      this.bw.WriteInt(this.Crc32);
      this.bw.WriteBits(1, this.blockRandomised ? 1U : 0U);
      this.moveToFrontCodeAndSend();
      this.Reset();
    }

    private void randomiseBlock()
    {
      bool[] inUse = this.cstate.inUse;
      byte[] block = this.cstate.block;
      int last = this.last;
      int index1 = 256;
      while (--index1 >= 0)
        inUse[index1] = false;
      int num1 = 0;
      int i = 0;
      int num2 = 0;
      int index2 = 1;
      while (num2 <= last)
      {
        if (num1 == 0)
        {
          num1 = (int) (ushort) Rand.Rnums(i);
          if (++i == 512)
            i = 0;
        }
        --num1;
        block[index2] ^= num1 == 1 ? (byte) 1 : (byte) 0;
        inUse[(int) block[index2] & (int) byte.MaxValue] = true;
        num2 = index2;
        ++index2;
      }
      this.blockRandomised = true;
    }

    private void mainSort()
    {
      BZip2Compressor.CompressionState cstate = this.cstate;
      int[] sortRunningOrder = cstate.mainSort_runningOrder;
      int[] mainSortCopy = cstate.mainSort_copy;
      bool[] mainSortBigDone = cstate.mainSort_bigDone;
      int[] ftab = cstate.ftab;
      byte[] block = cstate.block;
      int[] fmap = cstate.fmap;
      char[] quadrant = cstate.quadrant;
      int last = this.last;
      int workLimit = this.workLimit;
      bool firstAttempt = this.firstAttempt;
      int index1 = 65537;
      while (--index1 >= 0)
        ftab[index1] = 0;
      for (int index2 = 0; index2 < Ionic.BZip2.BZip2.NUM_OVERSHOOT_BYTES; ++index2)
        block[last + index2 + 2] = block[index2 % (last + 1) + 1];
      int index3 = last + Ionic.BZip2.BZip2.NUM_OVERSHOOT_BYTES + 1;
      while (--index3 >= 0)
        quadrant[index3] = char.MinValue;
      block[0] = block[last + 1];
      int num1 = (int) block[0] & (int) byte.MaxValue;
      for (int index2 = 0; index2 <= last; ++index2)
      {
        int num2 = (int) block[index2 + 1] & (int) byte.MaxValue;
        ++ftab[(num1 << 8) + num2];
        num1 = num2;
      }
      for (int index2 = 1; index2 <= 65536; ++index2)
        ftab[index2] += ftab[index2 - 1];
      int num3 = (int) block[1] & (int) byte.MaxValue;
      for (int index2 = 0; index2 < last; ++index2)
      {
        int num2 = (int) block[index2 + 2] & (int) byte.MaxValue;
        fmap[--ftab[(num3 << 8) + num2]] = index2;
        num3 = num2;
      }
      fmap[--ftab[(((int) block[last + 1] & (int) byte.MaxValue) << 8) + ((int) block[1] & (int) byte.MaxValue)]] = last;
      int index4 = 256;
      while (--index4 >= 0)
      {
        mainSortBigDone[index4] = false;
        sortRunningOrder[index4] = index4;
      }
      int num4 = 364;
      while (num4 != 1)
      {
        num4 /= 3;
        for (int index2 = num4; index2 <= (int) byte.MaxValue; ++index2)
        {
          int num2 = sortRunningOrder[index2];
          int num5 = ftab[num2 + 1 << 8] - ftab[num2 << 8];
          int num6 = num4 - 1;
          int index5 = index2;
          for (int index6 = sortRunningOrder[index5 - num4]; ftab[index6 + 1 << 8] - ftab[index6 << 8] > num5; index6 = sortRunningOrder[index5 - num4])
          {
            sortRunningOrder[index5] = index6;
            index5 -= num4;
            if (index5 <= num6)
              break;
          }
          sortRunningOrder[index5] = num2;
        }
      }
      for (int index2 = 0; index2 <= (int) byte.MaxValue; ++index2)
      {
        int index5 = sortRunningOrder[index2];
        for (int index6 = 0; index6 <= (int) byte.MaxValue; ++index6)
        {
          int index7 = (index5 << 8) + index6;
          int num2 = ftab[index7];
          if ((num2 & BZip2Compressor.SETMASK) != BZip2Compressor.SETMASK)
          {
            int loSt = num2 & BZip2Compressor.CLEARMASK;
            int hiSt = (ftab[index7 + 1] & BZip2Compressor.CLEARMASK) - 1;
            if (hiSt > loSt)
            {
              this.mainQSort3(cstate, loSt, hiSt, 2);
              if (firstAttempt && this.workDone > workLimit)
                return;
            }
            ftab[index7] = num2 | BZip2Compressor.SETMASK;
          }
        }
        for (int index6 = 0; index6 <= (int) byte.MaxValue; ++index6)
          mainSortCopy[index6] = ftab[(index6 << 8) + index5] & BZip2Compressor.CLEARMASK;
        int index8 = ftab[index5 << 8] & BZip2Compressor.CLEARMASK;
        for (int index6 = ftab[index5 + 1 << 8] & BZip2Compressor.CLEARMASK; index8 < index6; ++index8)
        {
          int index7 = fmap[index8];
          int index9 = (int) block[index7] & (int) byte.MaxValue;
          if (!mainSortBigDone[index9])
          {
            fmap[mainSortCopy[index9]] = index7 == 0 ? last : index7 - 1;
            ++mainSortCopy[index9];
          }
        }
        int num5 = 256;
        while (--num5 >= 0)
          ftab[(num5 << 8) + index5] |= BZip2Compressor.SETMASK;
        mainSortBigDone[index5] = true;
        if (index2 < (int) byte.MaxValue)
        {
          int num2 = ftab[index5 << 8] & BZip2Compressor.CLEARMASK;
          int num6 = (ftab[index5 + 1 << 8] & BZip2Compressor.CLEARMASK) - num2;
          int num7 = 0;
          while (num6 >> num7 > 65534)
            ++num7;
          for (int index6 = 0; index6 < num6; ++index6)
          {
            int index7 = fmap[num2 + index6];
            char ch = (char) (index6 >> num7);
            quadrant[index7] = ch;
            if (index7 < Ionic.BZip2.BZip2.NUM_OVERSHOOT_BYTES)
              quadrant[index7 + last + 1] = ch;
          }
        }
      }
    }

    private void blockSort()
    {
      this.workLimit = BZip2Compressor.WORK_FACTOR * this.last;
      this.workDone = 0;
      this.blockRandomised = false;
      this.firstAttempt = true;
      this.mainSort();
      if (this.firstAttempt && this.workDone > this.workLimit)
      {
        this.randomiseBlock();
        this.workLimit = this.workDone = 0;
        this.firstAttempt = false;
        this.mainSort();
      }
      int[] fmap = this.cstate.fmap;
      this.origPtr = -1;
      int index = 0;
      for (int last = this.last; index <= last; ++index)
      {
        if (fmap[index] == 0)
        {
          this.origPtr = index;
          break;
        }
      }
    }

    private bool mainSimpleSort(
      BZip2Compressor.CompressionState dataShadow,
      int lo,
      int hi,
      int d)
    {
      int num1 = hi - lo + 1;
      if (num1 < 2)
        return this.firstAttempt && this.workDone > this.workLimit;
      int index1 = 0;
      while (BZip2Compressor.increments[index1] < num1)
        ++index1;
      int[] fmap = dataShadow.fmap;
      char[] quadrant = dataShadow.quadrant;
      byte[] block = dataShadow.block;
      int last = this.last;
      int num2 = last + 1;
      bool firstAttempt = this.firstAttempt;
      int workLimit = this.workLimit;
      int workDone = this.workDone;
      while (--index1 >= 0)
      {
        int increment = BZip2Compressor.increments[index1];
        int num3 = lo + increment - 1;
        int index2 = lo + increment;
        while (index2 <= hi)
        {
          for (int index3 = 3; index2 <= hi && --index3 >= 0; ++index2)
          {
            int num4 = fmap[index2];
            int num5 = num4 + d;
            int index4 = index2;
            bool flag = false;
            int num6 = 0;
label_11:
            int num7;
            int num8;
            do
            {
              if (flag)
              {
                fmap[index4] = num6;
                if ((index4 -= increment) <= num3)
                  break;
              }
              else
                flag = true;
              num6 = fmap[index4 - increment];
              num7 = num6 + d;
              num8 = num5;
              if ((int) block[num7 + 1] == (int) block[num8 + 1])
              {
                if ((int) block[num7 + 2] == (int) block[num8 + 2])
                {
                  if ((int) block[num7 + 3] == (int) block[num8 + 3])
                  {
                    if ((int) block[num7 + 4] == (int) block[num8 + 4])
                    {
                      if ((int) block[num7 + 5] == (int) block[num8 + 5])
                      {
                        int index5;
                        int index6;
                        if ((int) block[index5 = num7 + 6] == (int) block[index6 = num8 + 6])
                        {
                          int num9 = last;
                          while (num9 > 0)
                          {
                            num9 -= 4;
                            if ((int) block[index5 + 1] == (int) block[index6 + 1])
                            {
                              if ((int) quadrant[index5] == (int) quadrant[index6])
                              {
                                if ((int) block[index5 + 2] == (int) block[index6 + 2])
                                {
                                  if ((int) quadrant[index5 + 1] == (int) quadrant[index6 + 1])
                                  {
                                    if ((int) block[index5 + 3] == (int) block[index6 + 3])
                                    {
                                      if ((int) quadrant[index5 + 2] == (int) quadrant[index6 + 2])
                                      {
                                        if ((int) block[index5 + 4] == (int) block[index6 + 4])
                                        {
                                          if ((int) quadrant[index5 + 3] == (int) quadrant[index6 + 3])
                                          {
                                            if ((index5 += 4) >= num2)
                                              index5 -= num2;
                                            if ((index6 += 4) >= num2)
                                              index6 -= num2;
                                            ++workDone;
                                          }
                                          else
                                          {
                                            if ((int) quadrant[index5 + 3] <= (int) quadrant[index6 + 3])
                                              break;
                                            goto label_11;
                                          }
                                        }
                                        else
                                        {
                                          if (((int) block[index5 + 4] & (int) byte.MaxValue) <= ((int) block[index6 + 4] & (int) byte.MaxValue))
                                            break;
                                          goto label_11;
                                        }
                                      }
                                      else
                                      {
                                        if ((int) quadrant[index5 + 2] <= (int) quadrant[index6 + 2])
                                          break;
                                        goto label_11;
                                      }
                                    }
                                    else
                                    {
                                      if (((int) block[index5 + 3] & (int) byte.MaxValue) <= ((int) block[index6 + 3] & (int) byte.MaxValue))
                                        break;
                                      goto label_11;
                                    }
                                  }
                                  else
                                  {
                                    if ((int) quadrant[index5 + 1] <= (int) quadrant[index6 + 1])
                                      break;
                                    goto label_11;
                                  }
                                }
                                else
                                {
                                  if (((int) block[index5 + 2] & (int) byte.MaxValue) <= ((int) block[index6 + 2] & (int) byte.MaxValue))
                                    break;
                                  goto label_11;
                                }
                              }
                              else
                              {
                                if ((int) quadrant[index5] <= (int) quadrant[index6])
                                  break;
                                goto label_11;
                              }
                            }
                            else
                            {
                              if (((int) block[index5 + 1] & (int) byte.MaxValue) <= ((int) block[index6 + 1] & (int) byte.MaxValue))
                                break;
                              goto label_11;
                            }
                          }
                          break;
                        }
                        if (((int) block[index5] & (int) byte.MaxValue) <= ((int) block[index6] & (int) byte.MaxValue))
                          break;
                      }
                      else if (((int) block[num7 + 5] & (int) byte.MaxValue) <= ((int) block[num8 + 5] & (int) byte.MaxValue))
                        break;
                    }
                    else if (((int) block[num7 + 4] & (int) byte.MaxValue) <= ((int) block[num8 + 4] & (int) byte.MaxValue))
                      break;
                  }
                  else if (((int) block[num7 + 3] & (int) byte.MaxValue) <= ((int) block[num8 + 3] & (int) byte.MaxValue))
                    break;
                }
                else if (((int) block[num7 + 2] & (int) byte.MaxValue) <= ((int) block[num8 + 2] & (int) byte.MaxValue))
                  break;
              }
            }
            while (((int) block[num7 + 1] & (int) byte.MaxValue) > ((int) block[num8 + 1] & (int) byte.MaxValue));
            fmap[index4] = num4;
          }
          if (firstAttempt && index2 <= hi && workDone > workLimit)
            goto label_54;
        }
      }
label_54:
      this.workDone = workDone;
      return firstAttempt && workDone > workLimit;
    }

    private static void vswap(int[] fmap, int p1, int p2, int n)
    {
      n += p1;
      while (p1 < n)
      {
        int num = fmap[p1];
        fmap[p1++] = fmap[p2];
        fmap[p2++] = num;
      }
    }

    private static byte med3(byte a, byte b, byte c)
    {
      if ((int) a >= (int) b)
      {
        if ((int) b > (int) c)
          return b;
        return (int) a <= (int) c ? a : c;
      }
      if ((int) b < (int) c)
        return b;
      return (int) a >= (int) c ? a : c;
    }

    private void mainQSort3(
      BZip2Compressor.CompressionState dataShadow,
      int loSt,
      int hiSt,
      int dSt)
    {
      int[] stackLl = dataShadow.stack_ll;
      int[] stackHh = dataShadow.stack_hh;
      int[] stackDd = dataShadow.stack_dd;
      int[] fmap = dataShadow.fmap;
      byte[] block = dataShadow.block;
      stackLl[0] = loSt;
      stackHh[0] = hiSt;
      stackDd[0] = dSt;
      int index1 = 1;
      while (--index1 >= 0)
      {
        int index2 = stackLl[index1];
        int hi = stackHh[index1];
        int d = stackDd[index1];
        if (hi - index2 < BZip2Compressor.SMALL_THRESH || d > BZip2Compressor.DEPTH_THRESH)
        {
          if (this.mainSimpleSort(dataShadow, index2, hi, d))
            break;
        }
        else
        {
          int num1 = d + 1;
          int num2 = (int) BZip2Compressor.med3(block[fmap[index2] + num1], block[fmap[hi] + num1], block[fmap[index2 + hi >> 1] + num1]) & (int) byte.MaxValue;
          int p1 = index2;
          int index3 = hi;
          int index4 = index2;
          int index5 = hi;
          while (true)
          {
            while (p1 <= index3)
            {
              int num3 = ((int) block[fmap[p1] + num1] & (int) byte.MaxValue) - num2;
              if (num3 == 0)
              {
                int num4 = fmap[p1];
                fmap[p1++] = fmap[index4];
                fmap[index4++] = num4;
              }
              else if (num3 < 0)
                ++p1;
              else
                break;
            }
            while (p1 <= index3)
            {
              int num3 = ((int) block[fmap[index3] + num1] & (int) byte.MaxValue) - num2;
              if (num3 == 0)
              {
                int num4 = fmap[index3];
                fmap[index3--] = fmap[index5];
                fmap[index5--] = num4;
              }
              else if (num3 > 0)
                --index3;
              else
                break;
            }
            if (p1 <= index3)
            {
              int num3 = fmap[p1];
              fmap[p1++] = fmap[index3];
              fmap[index3--] = num3;
            }
            else
              break;
          }
          if (index5 < index4)
          {
            stackLl[index1] = index2;
            stackHh[index1] = hi;
            stackDd[index1] = num1;
            ++index1;
          }
          else
          {
            int n1 = index4 - index2 < p1 - index4 ? index4 - index2 : p1 - index4;
            BZip2Compressor.vswap(fmap, index2, p1 - n1, n1);
            int n2 = hi - index5 < index5 - index3 ? hi - index5 : index5 - index3;
            BZip2Compressor.vswap(fmap, p1, hi - n2 + 1, n2);
            int num3 = index2 + p1 - index4 - 1;
            int num4 = hi - (index5 - index3) + 1;
            stackLl[index1] = index2;
            stackHh[index1] = num3;
            stackDd[index1] = d;
            int index6 = index1 + 1;
            stackLl[index6] = num3 + 1;
            stackHh[index6] = num4 - 1;
            stackDd[index6] = num1;
            int index7 = index6 + 1;
            stackLl[index7] = num4;
            stackHh[index7] = hi;
            stackDd[index7] = d;
            index1 = index7 + 1;
          }
        }
      }
    }

    private void generateMTFValues()
    {
      int last = this.last;
      BZip2Compressor.CompressionState cstate = this.cstate;
      bool[] inUse = cstate.inUse;
      byte[] block = cstate.block;
      int[] fmap = cstate.fmap;
      char[] sfmap = cstate.sfmap;
      int[] mtfFreq = cstate.mtfFreq;
      byte[] unseqToSeq = cstate.unseqToSeq;
      byte[] generateMtfValuesYy = cstate.generateMTFValues_yy;
      int num1 = 0;
      for (int index = 0; index < 256; ++index)
      {
        if (inUse[index])
        {
          unseqToSeq[index] = (byte) num1;
          ++num1;
        }
      }
      this.nInUse = num1;
      int index1 = num1 + 1;
      for (int index2 = index1; index2 >= 0; --index2)
        mtfFreq[index2] = 0;
      int index3 = num1;
      while (--index3 >= 0)
        generateMtfValuesYy[index3] = (byte) index3;
      int index4 = 0;
      int num2 = 0;
      for (int index2 = 0; index2 <= last; ++index2)
      {
        byte num3 = unseqToSeq[(int) block[fmap[index2]] & (int) byte.MaxValue];
        byte num4 = generateMtfValuesYy[0];
        int index5 = 0;
        while ((int) num3 != (int) num4)
        {
          ++index5;
          byte num5 = num4;
          num4 = generateMtfValuesYy[index5];
          generateMtfValuesYy[index5] = num5;
        }
        generateMtfValuesYy[0] = num4;
        if (index5 == 0)
        {
          ++num2;
        }
        else
        {
          if (num2 > 0)
          {
            int num5 = num2 - 1;
            while (true)
            {
              if ((num5 & 1) == 0)
              {
                sfmap[index4] = Ionic.BZip2.BZip2.RUNA;
                ++index4;
                ++mtfFreq[(int) Ionic.BZip2.BZip2.RUNA];
              }
              else
              {
                sfmap[index4] = Ionic.BZip2.BZip2.RUNB;
                ++index4;
                ++mtfFreq[(int) Ionic.BZip2.BZip2.RUNB];
              }
              if (num5 >= 2)
                num5 = num5 - 2 >> 1;
              else
                break;
            }
            num2 = 0;
          }
          sfmap[index4] = (char) (index5 + 1);
          ++index4;
          ++mtfFreq[index5 + 1];
        }
      }
      if (num2 > 0)
      {
        int num3 = num2 - 1;
        while (true)
        {
          if ((num3 & 1) == 0)
          {
            sfmap[index4] = Ionic.BZip2.BZip2.RUNA;
            ++index4;
            ++mtfFreq[(int) Ionic.BZip2.BZip2.RUNA];
          }
          else
          {
            sfmap[index4] = Ionic.BZip2.BZip2.RUNB;
            ++index4;
            ++mtfFreq[(int) Ionic.BZip2.BZip2.RUNB];
          }
          if (num3 >= 2)
            num3 = num3 - 2 >> 1;
          else
            break;
        }
      }
      sfmap[index4] = (char) index1;
      ++mtfFreq[index1];
      this.nMTF = index4 + 1;
    }

    private static void hbAssignCodes(
      int[] code,
      byte[] length,
      int minLen,
      int maxLen,
      int alphaSize)
    {
      int num = 0;
      for (int index1 = minLen; index1 <= maxLen; ++index1)
      {
        for (int index2 = 0; index2 < alphaSize; ++index2)
        {
          if (((int) length[index2] & (int) byte.MaxValue) == index1)
          {
            code[index2] = num;
            ++num;
          }
        }
        num <<= 1;
      }
    }

    private void sendMTFValues()
    {
      byte[][] sendMtfValuesLen = this.cstate.sendMTFValues_len;
      int alphaSize = this.nInUse + 2;
      int ngroups = Ionic.BZip2.BZip2.NGroups;
      while (--ngroups >= 0)
      {
        byte[] numArray = sendMtfValuesLen[ngroups];
        int index = alphaSize;
        while (--index >= 0)
          numArray[index] = BZip2Compressor.GREATER_ICOST;
      }
      int nGroups = this.nMTF < 200 ? 2 : (this.nMTF < 600 ? 3 : (this.nMTF < 1200 ? 4 : (this.nMTF < 2400 ? 5 : 6)));
      this.sendMTFValues0(nGroups, alphaSize);
      int nSelectors = this.sendMTFValues1(nGroups, alphaSize);
      this.sendMTFValues2(nGroups, nSelectors);
      this.sendMTFValues3(nGroups, alphaSize);
      this.sendMTFValues4();
      this.sendMTFValues5(nGroups, nSelectors);
      this.sendMTFValues6(nGroups, alphaSize);
      this.sendMTFValues7(nSelectors);
    }

    private void sendMTFValues0(int nGroups, int alphaSize)
    {
      byte[][] sendMtfValuesLen = this.cstate.sendMTFValues_len;
      int[] mtfFreq = this.cstate.mtfFreq;
      int nMtf = this.nMTF;
      int num1 = 0;
      for (int index1 = nGroups; index1 > 0; --index1)
      {
        int num2 = nMtf / index1;
        int num3 = num1 - 1;
        int num4 = 0;
        int num5 = alphaSize - 1;
        while (num4 < num2 && num3 < num5)
          num4 += mtfFreq[++num3];
        if (num3 > num1 && index1 != nGroups && (index1 != 1 && (nGroups - index1 & 1) != 0))
          num4 -= mtfFreq[num3--];
        byte[] numArray = sendMtfValuesLen[index1 - 1];
        int index2 = alphaSize;
        while (--index2 >= 0)
          numArray[index2] = index2 < num1 || index2 > num3 ? BZip2Compressor.GREATER_ICOST : BZip2Compressor.LESSER_ICOST;
        num1 = num3 + 1;
        nMtf -= num4;
      }
    }

    private static void hbMakeCodeLengths(
      byte[] len,
      int[] freq,
      BZip2Compressor.CompressionState state1,
      int alphaSize,
      int maxLen)
    {
      int[] heap = state1.heap;
      int[] weight = state1.weight;
      int[] parent = state1.parent;
      int index1 = alphaSize;
      while (--index1 >= 0)
        weight[index1 + 1] = (freq[index1] == 0 ? 1 : freq[index1]) << 8;
      bool flag = true;
      while (flag)
      {
        flag = false;
        int index2 = alphaSize;
        int index3 = 0;
        heap[0] = 0;
        weight[0] = 0;
        parent[0] = -2;
        for (int index4 = 1; index4 <= alphaSize; ++index4)
        {
          parent[index4] = -1;
          ++index3;
          heap[index3] = index4;
          int index5 = index3;
          int index6;
          for (index6 = heap[index5]; weight[index6] < weight[heap[index5 >> 1]]; index5 >>= 1)
            heap[index5] = heap[index5 >> 1];
          heap[index5] = index6;
        }
        while (index3 > 1)
        {
          int index4 = heap[1];
          heap[1] = heap[index3];
          int index5 = index3 - 1;
          int num1 = 0;
          int index6 = 1;
          int index7 = heap[1];
          while (true)
          {
            int index8 = index6 << 1;
            if (index8 <= index5)
            {
              if (index8 < index5 && weight[heap[index8 + 1]] < weight[heap[index8]])
                ++index8;
              if (weight[index7] >= weight[heap[index8]])
              {
                heap[index6] = heap[index8];
                index6 = index8;
              }
              else
                break;
            }
            else
              break;
          }
          heap[index6] = index7;
          int index9 = heap[1];
          heap[1] = heap[index5];
          int num2 = index5 - 1;
          num1 = 0;
          int index10 = 1;
          int index11 = heap[1];
          while (true)
          {
            int index8 = index10 << 1;
            if (index8 <= num2)
            {
              if (index8 < num2 && weight[heap[index8 + 1]] < weight[heap[index8]])
                ++index8;
              if (weight[index11] >= weight[heap[index8]])
              {
                heap[index10] = heap[index8];
                index10 = index8;
              }
              else
                break;
            }
            else
              break;
          }
          heap[index10] = index11;
          ++index2;
          parent[index4] = parent[index9] = index2;
          int num3 = weight[index4];
          int num4 = weight[index9];
          weight[index2] = (num3 & -256) + (num4 & -256) | 1 + ((num3 & (int) byte.MaxValue) > (num4 & (int) byte.MaxValue) ? num3 & (int) byte.MaxValue : num4 & (int) byte.MaxValue);
          parent[index2] = -1;
          index3 = num2 + 1;
          heap[index3] = index2;
          int index12 = index3;
          int index13 = heap[index12];
          for (int index8 = weight[index13]; index8 < weight[heap[index12 >> 1]]; index12 >>= 1)
            heap[index12] = heap[index12 >> 1];
          heap[index12] = index13;
        }
        for (int index4 = 1; index4 <= alphaSize; ++index4)
        {
          int num1 = 0;
          int index5 = index4;
          int num2;
          while ((num2 = parent[index5]) >= 0)
          {
            index5 = num2;
            ++num1;
          }
          len[index4 - 1] = (byte) num1;
          if (num1 > maxLen)
            flag = true;
        }
        if (flag)
        {
          for (int index4 = 1; index4 < alphaSize; ++index4)
          {
            int num = 1 + (weight[index4] >> 8 >> 1);
            weight[index4] = num << 8;
          }
        }
      }
    }

    private int sendMTFValues1(int nGroups, int alphaSize)
    {
      BZip2Compressor.CompressionState cstate = this.cstate;
      int[][] sendMtfValuesRfreq = cstate.sendMTFValues_rfreq;
      int[] sendMtfValuesFave = cstate.sendMTFValues_fave;
      short[] sendMtfValuesCost = cstate.sendMTFValues_cost;
      char[] sfmap = cstate.sfmap;
      byte[] selector = cstate.selector;
      byte[][] sendMtfValuesLen = cstate.sendMTFValues_len;
      byte[] numArray1 = sendMtfValuesLen[0];
      byte[] numArray2 = sendMtfValuesLen[1];
      byte[] numArray3 = sendMtfValuesLen[2];
      byte[] numArray4 = sendMtfValuesLen[3];
      byte[] numArray5 = sendMtfValuesLen[4];
      byte[] numArray6 = sendMtfValuesLen[5];
      int nMtf = this.nMTF;
      int index1 = 0;
      for (int index2 = 0; index2 < Ionic.BZip2.BZip2.N_ITERS; ++index2)
      {
        int index3 = nGroups;
        while (--index3 >= 0)
        {
          sendMtfValuesFave[index3] = 0;
          int[] numArray7 = sendMtfValuesRfreq[index3];
          int index4 = alphaSize;
          while (--index4 >= 0)
            numArray7[index4] = 0;
        }
        index1 = 0;
        int num1;
        for (int index4 = 0; index4 < this.nMTF; index4 = num1 + 1)
        {
          num1 = Math.Min(index4 + Ionic.BZip2.BZip2.G_SIZE - 1, nMtf - 1);
          if (nGroups == Ionic.BZip2.BZip2.NGroups)
          {
            int[] numArray7 = new int[6];
            for (int index5 = index4; index5 <= num1; ++index5)
            {
              int index6 = (int) sfmap[index5];
              numArray7[0] += (int) numArray1[index6] & (int) byte.MaxValue;
              numArray7[1] += (int) numArray2[index6] & (int) byte.MaxValue;
              numArray7[2] += (int) numArray3[index6] & (int) byte.MaxValue;
              numArray7[3] += (int) numArray4[index6] & (int) byte.MaxValue;
              numArray7[4] += (int) numArray5[index6] & (int) byte.MaxValue;
              numArray7[5] += (int) numArray6[index6] & (int) byte.MaxValue;
            }
            sendMtfValuesCost[0] = (short) numArray7[0];
            sendMtfValuesCost[1] = (short) numArray7[1];
            sendMtfValuesCost[2] = (short) numArray7[2];
            sendMtfValuesCost[3] = (short) numArray7[3];
            sendMtfValuesCost[4] = (short) numArray7[4];
            sendMtfValuesCost[5] = (short) numArray7[5];
          }
          else
          {
            int index5 = nGroups;
            while (--index5 >= 0)
              sendMtfValuesCost[index5] = (short) 0;
            for (int index6 = index4; index6 <= num1; ++index6)
            {
              int index7 = (int) sfmap[index6];
              int index8 = nGroups;
              while (--index8 >= 0)
                sendMtfValuesCost[index8] += (short) ((int) sendMtfValuesLen[index8][index7] & (int) byte.MaxValue);
            }
          }
          int index9 = -1;
          int index10 = nGroups;
          int num2 = 999999999;
          while (--index10 >= 0)
          {
            int num3 = (int) sendMtfValuesCost[index10];
            if (num3 < num2)
            {
              num2 = num3;
              index9 = index10;
            }
          }
          ++sendMtfValuesFave[index9];
          selector[index1] = (byte) index9;
          ++index1;
          int[] numArray8 = sendMtfValuesRfreq[index9];
          for (int index5 = index4; index5 <= num1; ++index5)
            ++numArray8[(int) sfmap[index5]];
        }
        for (int index4 = 0; index4 < nGroups; ++index4)
          BZip2Compressor.hbMakeCodeLengths(sendMtfValuesLen[index4], sendMtfValuesRfreq[index4], this.cstate, alphaSize, 20);
      }
      return index1;
    }

    private void sendMTFValues2(int nGroups, int nSelectors)
    {
      BZip2Compressor.CompressionState cstate = this.cstate;
      byte[] sendMtfValues2Pos = cstate.sendMTFValues2_pos;
      int index1 = nGroups;
      while (--index1 >= 0)
        sendMtfValues2Pos[index1] = (byte) index1;
      for (int index2 = 0; index2 < nSelectors; ++index2)
      {
        byte num1 = cstate.selector[index2];
        byte num2 = sendMtfValues2Pos[0];
        int index3 = 0;
        while ((int) num1 != (int) num2)
        {
          ++index3;
          byte num3 = num2;
          num2 = sendMtfValues2Pos[index3];
          sendMtfValues2Pos[index3] = num3;
        }
        sendMtfValues2Pos[0] = num2;
        cstate.selectorMtf[index2] = (byte) index3;
      }
    }

    private void sendMTFValues3(int nGroups, int alphaSize)
    {
      int[][] sendMtfValuesCode = this.cstate.sendMTFValues_code;
      byte[][] sendMtfValuesLen = this.cstate.sendMTFValues_len;
      for (int index1 = 0; index1 < nGroups; ++index1)
      {
        int minLen = 32;
        int maxLen = 0;
        byte[] numArray = sendMtfValuesLen[index1];
        int index2 = alphaSize;
        while (--index2 >= 0)
        {
          int num = (int) numArray[index2] & (int) byte.MaxValue;
          if (num > maxLen)
            maxLen = num;
          if (num < minLen)
            minLen = num;
        }
        BZip2Compressor.hbAssignCodes(sendMtfValuesCode[index1], sendMtfValuesLen[index1], minLen, maxLen, alphaSize);
      }
    }

    private void sendMTFValues4()
    {
      bool[] inUse = this.cstate.inUse;
      bool[] mtfValues4InUse16 = this.cstate.sentMTFValues4_inUse16;
      int index1 = 16;
      while (--index1 >= 0)
      {
        mtfValues4InUse16[index1] = false;
        int num1 = index1 * 16;
        int num2 = 16;
        while (--num2 >= 0)
        {
          if (inUse[num1 + num2])
            mtfValues4InUse16[index1] = true;
        }
      }
      uint num3 = 0;
      for (int index2 = 0; index2 < 16; ++index2)
      {
        if (mtfValues4InUse16[index2])
          num3 |= (uint) (1 << 16 - index2 - 1);
      }
      this.bw.WriteBits(16, num3);
      for (int index2 = 0; index2 < 16; ++index2)
      {
        if (mtfValues4InUse16[index2])
        {
          int num1 = index2 * 16;
          uint num2 = 0;
          for (int index3 = 0; index3 < 16; ++index3)
          {
            if (inUse[num1 + index3])
              num2 |= (uint) (1 << 16 - index3 - 1);
          }
          this.bw.WriteBits(16, num2);
        }
      }
    }

    private void sendMTFValues5(int nGroups, int nSelectors)
    {
      this.bw.WriteBits(3, (uint) nGroups);
      this.bw.WriteBits(15, (uint) nSelectors);
      byte[] selectorMtf = this.cstate.selectorMtf;
      for (int index1 = 0; index1 < nSelectors; ++index1)
      {
        int num = 0;
        for (int index2 = (int) selectorMtf[index1] & (int) byte.MaxValue; num < index2; ++num)
          this.bw.WriteBits(1, 1U);
        this.bw.WriteBits(1, 0U);
      }
    }

    private void sendMTFValues6(int nGroups, int alphaSize)
    {
      byte[][] sendMtfValuesLen = this.cstate.sendMTFValues_len;
      for (int index1 = 0; index1 < nGroups; ++index1)
      {
        byte[] numArray = sendMtfValuesLen[index1];
        uint num1 = (uint) numArray[0] & (uint) byte.MaxValue;
        this.bw.WriteBits(5, num1);
        for (int index2 = 0; index2 < alphaSize; ++index2)
        {
          int num2;
          for (num2 = (int) numArray[index2] & (int) byte.MaxValue; (long) num1 < (long) num2; ++num1)
            this.bw.WriteBits(2, 2U);
          for (; (long) num1 > (long) num2; --num1)
            this.bw.WriteBits(2, 3U);
          this.bw.WriteBits(1, 0U);
        }
      }
    }

    private void sendMTFValues7(int nSelectors)
    {
      byte[][] sendMtfValuesLen = this.cstate.sendMTFValues_len;
      int[][] sendMtfValuesCode = this.cstate.sendMTFValues_code;
      byte[] selector = this.cstate.selector;
      char[] sfmap = this.cstate.sfmap;
      int nMtf = this.nMTF;
      int index1 = 0;
      int index2 = 0;
      while (index2 < nMtf)
      {
        int num = Math.Min(index2 + Ionic.BZip2.BZip2.G_SIZE - 1, nMtf - 1);
        int index3 = (int) selector[index1] & (int) byte.MaxValue;
        int[] numArray1 = sendMtfValuesCode[index3];
        byte[] numArray2 = sendMtfValuesLen[index3];
        for (; index2 <= num; ++index2)
        {
          int index4 = (int) sfmap[index2];
          this.bw.WriteBits((int) numArray2[index4] & (int) byte.MaxValue, (uint) numArray1[index4]);
        }
        index2 = num + 1;
        ++index1;
      }
    }

    private void moveToFrontCodeAndSend()
    {
      this.bw.WriteBits(24, (uint) this.origPtr);
      this.generateMTFValues();
      this.sendMTFValues();
    }

    private class CompressionState
    {
      public readonly bool[] inUse = new bool[256];
      public readonly byte[] unseqToSeq = new byte[256];
      public readonly int[] mtfFreq = new int[Ionic.BZip2.BZip2.MaxAlphaSize];
      public readonly byte[] selector = new byte[Ionic.BZip2.BZip2.MaxSelectors];
      public readonly byte[] selectorMtf = new byte[Ionic.BZip2.BZip2.MaxSelectors];
      public readonly byte[] generateMTFValues_yy = new byte[256];
      public byte[][] sendMTFValues_len;
      public int[][] sendMTFValues_rfreq;
      public readonly int[] sendMTFValues_fave = new int[Ionic.BZip2.BZip2.NGroups];
      public readonly short[] sendMTFValues_cost = new short[Ionic.BZip2.BZip2.NGroups];
      public int[][] sendMTFValues_code;
      public readonly byte[] sendMTFValues2_pos = new byte[Ionic.BZip2.BZip2.NGroups];
      public readonly bool[] sentMTFValues4_inUse16 = new bool[16];
      public readonly int[] stack_ll = new int[Ionic.BZip2.BZip2.QSORT_STACK_SIZE];
      public readonly int[] stack_hh = new int[Ionic.BZip2.BZip2.QSORT_STACK_SIZE];
      public readonly int[] stack_dd = new int[Ionic.BZip2.BZip2.QSORT_STACK_SIZE];
      public readonly int[] mainSort_runningOrder = new int[256];
      public readonly int[] mainSort_copy = new int[256];
      public readonly bool[] mainSort_bigDone = new bool[256];
      public int[] heap = new int[Ionic.BZip2.BZip2.MaxAlphaSize + 2];
      public int[] weight = new int[Ionic.BZip2.BZip2.MaxAlphaSize * 2];
      public int[] parent = new int[Ionic.BZip2.BZip2.MaxAlphaSize * 2];
      public readonly int[] ftab = new int[65537];
      public byte[] block;
      public int[] fmap;
      public char[] sfmap;
      public char[] quadrant;

      public CompressionState(int blockSize100k)
      {
        int length = blockSize100k * Ionic.BZip2.BZip2.BlockSizeMultiple;
        this.block = new byte[length + 1 + Ionic.BZip2.BZip2.NUM_OVERSHOOT_BYTES];
        this.fmap = new int[length];
        this.sfmap = new char[2 * length];
        this.quadrant = this.sfmap;
        this.sendMTFValues_len = Ionic.BZip2.BZip2.InitRectangularArray<byte>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
        this.sendMTFValues_rfreq = Ionic.BZip2.BZip2.InitRectangularArray<int>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
        this.sendMTFValues_code = Ionic.BZip2.BZip2.InitRectangularArray<int>(Ionic.BZip2.BZip2.NGroups, Ionic.BZip2.BZip2.MaxAlphaSize);
      }
    }
  }
}
