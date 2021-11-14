// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.MemoryChecker
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.LsuPro
{
  public class MemoryChecker : IDisposable
  {
    private IntPtr heap = IntPtr.Zero;
    private List<IntPtr> memoryBlocks = new List<IntPtr>(1024);
    private bool disposed;

    public static bool TryAlloc(int sizeInMegabytes)
    {
      try
      {
        Tracer.Information("Trying to allocate {0:N0} MBs", (object) sizeInMegabytes);
        MemoryChecker memoryChecker = new MemoryChecker();
        MemoryChecker.Trace();
        bool flag = memoryChecker.Alloc(sizeInMegabytes);
        memoryChecker.Free();
        MemoryChecker.Trace();
        Tracer.Information("Result is {0}", (object) flag);
        memoryChecker.Dispose();
        return flag;
      }
      catch (Exception ex)
      {
        Tracer.Warning("Error allocating {0:N0} MBs: {1}", (object) sizeInMegabytes, (object) ex.Message);
        return true;
      }
    }

    public static void Trace()
    {
      try
      {
        Tracer.Information("NonpagedSystemMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().NonpagedSystemMemorySize64);
        Tracer.Information("PagedSystemMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().PagedSystemMemorySize64);
        Tracer.Information("PagedMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().PagedMemorySize64);
        Tracer.Information("PeakPagedMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().PeakPagedMemorySize64);
        Tracer.Information("VirtualMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().VirtualMemorySize64);
        Tracer.Information("PeakVirtualMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().PeakVirtualMemorySize64);
        Tracer.Information("PrivateMemorySize64: {0:N0}", (object) Process.GetCurrentProcess().PrivateMemorySize64);
      }
      catch (Exception ex)
      {
        Tracer.Warning("Error tracing memory usage: {0}", (object) ex.Message);
      }
    }

    public MemoryChecker() => this.disposed = false;

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "reviewed")]
    ~MemoryChecker()
    {
    }

    public bool Alloc(int sizeInMegabytes)
    {
      Tracer.Information("Allocating {0:N0} MBs on heap", (object) sizeInMegabytes);
      if (IntPtr.Zero == this.heap)
      {
        this.heap = MemoryChecker.HeapCreate(0U, new UIntPtr(0U), new UIntPtr(0U));
        if (IntPtr.Zero == this.heap)
        {
          Tracer.Warning("HeapCreate failed with error {0}", (object) Marshal.GetLastWin32Error());
          return false;
        }
      }
      for (int index = 0; index < sizeInMegabytes; ++index)
      {
        IntPtr num = MemoryChecker.HeapAlloc(this.heap, 0U, 1048576U);
        if (IntPtr.Zero == num)
        {
          Tracer.Warning("HeapAlloc failed with error {0} allocating MB {1:N0}", (object) Marshal.GetLastWin32Error(), (object) index);
          return false;
        }
        this.memoryBlocks.Add(num);
      }
      Tracer.Information("Allocated {0:N0} MBs on heap", (object) sizeInMegabytes);
      return true;
    }

    public void Free()
    {
      int count = this.memoryBlocks.Count;
      Tracer.Information("Freeing {0:N0} MBs on heap", (object) count);
      for (int index = 0; index < count; ++index)
      {
        if (!MemoryChecker.HeapFree(this.heap, 0U, this.memoryBlocks[index]))
          Tracer.Warning("HeapFree failed with error {0} freeing MB {1:N0}", (object) Marshal.GetLastWin32Error(), (object) index);
      }
      this.memoryBlocks.Clear();
      Tracer.Information("Freed {0:N0} MBs on heap", (object) count);
    }

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "reviewed")]
    public void Dispose()
    {
      this.Free();
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      int num = disposing ? 1 : 0;
      this.disposed = true;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr HeapCreate(
      uint flOptions,
      UIntPtr dwInitialsize,
      UIntPtr dwMaximumSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, uint dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);
  }
}
