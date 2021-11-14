// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.ImageTools.ProgressReporter
// Assembly: FFUTool, Version=8.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5620B86A-1D2E-4A9B-AF31-782974775DC3
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\ffutool.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Microsoft.Windows.ImageTools
{
  public class ProgressReporter
  {
    private const double OneMegabyte = 1048576.0;
    private int width;
    private Stopwatch stopwatch;
    private int summaryCount;
    private Queue<Tuple<double, long>> progressPoints;

    public ProgressReporter()
    {
      this.width = Console.WindowWidth;
      this.stopwatch = Stopwatch.StartNew();
      this.summaryCount = 0;
      this.progressPoints = new Queue<Tuple<double, long>>();
    }

    public string CreateProgressDisplay(long position, long totalLength)
    {
      StringBuilder stringBuilder = new StringBuilder(2 * this.width);
      if (position == totalLength)
      {
        this.stopwatch.Stop();
        if (Interlocked.Add(ref this.summaryCount, 1) == 1)
        {
          double num = (double) totalLength / 1048576.0;
          stringBuilder.AppendFormat(Resources.TRANSFER_STATISTICS, (object) num, (object) this.stopwatch.Elapsed.TotalSeconds, (object) (num / this.stopwatch.Elapsed.TotalSeconds));
        }
        else
          stringBuilder.Clear();
      }
      else
      {
        double num1 = (double) position / (double) totalLength;
        if (num1 > 1.0)
          num1 = 1.0;
        int num2 = (int) Math.Floor(50.0 * num1);
        for (int index = 0; index < this.width; ++index)
          stringBuilder.Append('\b');
        stringBuilder.Append('[');
        for (int index = 0; index < num2; ++index)
          stringBuilder.Append('=');
        if (num2 < 50)
        {
          stringBuilder.Append('>');
          ++num2;
        }
        for (int index = num2; index < 50; ++index)
          stringBuilder.Append(' ');
        stringBuilder.Append("]  ");
        stringBuilder.AppendFormat("{0:0.00%}", (object) num1);
        stringBuilder.AppendFormat(" {0}", (object) this.GetSpeedString(position));
        for (int length = stringBuilder.Length; length < 2 * this.width - 1; ++length)
          stringBuilder.Append(' ');
      }
      return stringBuilder.ToString();
    }

    private string GetSpeedString(long position)
    {
      string str = string.Empty;
      this.progressPoints.Enqueue(new Tuple<double, long>(this.stopwatch.Elapsed.TotalSeconds, position));
      if (this.progressPoints.Count >= 8)
      {
        str = this.GetSpeedFromPoints(this.progressPoints.ToArray());
        this.progressPoints.Dequeue();
      }
      return str;
    }

    private string GetSpeedFromPoints(Tuple<double, long>[] points)
    {
      double num1 = 0.0;
      for (int index = 1; index < points.Length; ++index)
      {
        double num2 = (double) (points[index].Item2 - points[index - 1].Item2) / 1048576.0;
        double num3 = points[index].Item1 - points[index - 1].Item1;
        num1 += num2 / num3 / (double) (points.Length - 1);
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FORMAT_SPEED, (object) num1);
    }
  }
}
