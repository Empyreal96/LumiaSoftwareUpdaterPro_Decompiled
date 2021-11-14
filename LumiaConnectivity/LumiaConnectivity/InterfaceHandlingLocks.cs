// Decompiled with JetBrains decompiler
// Type: Microsoft.LumiaConnectivity.InterfaceHandlingLocks
// Assembly: LumiaConnectivity, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 63695ECA-A8DD-4DC5-AD6C-E88851844E58
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LumiaConnectivity.dll

using Microsoft.LsuPro;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace Microsoft.LumiaConnectivity
{
  internal class InterfaceHandlingLocks
  {
    private readonly object syncObject;
    private readonly Dictionary<string, ManualResetEventSlim> locks;

    public InterfaceHandlingLocks()
    {
      this.syncObject = new object();
      this.locks = new Dictionary<string, ManualResetEventSlim>();
    }

    public void CreateLock(string id)
    {
      lock (this.syncObject)
      {
        id = this.ConvertId(id);
        if (this.locks.ContainsKey(id))
          this.locks[id] = new ManualResetEventSlim(true);
        else
          this.locks.Add(id, new ManualResetEventSlim(true));
        Tracer.Information("*** CREATE_LOCK: Thread {0} created interface lock for '{1}' ***", (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture), (object) id);
      }
    }

    public bool Lock(string id)
    {
      lock (this.syncObject)
      {
        id = this.ConvertId(id);
        Tracer.Information("*** LOCK: Interface handling for '{0}' locked by thread {1} ***", (object) id, (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture));
        if (!this.locks.ContainsKey(id))
          return false;
        this.locks[id].Reset();
        return true;
      }
    }

    public bool Unlock(string id)
    {
      lock (this.syncObject)
      {
        id = this.ConvertId(id);
        Tracer.Information("*** UNLOCK: Interface handling for '{0}' unlocked by thread {1} ***", (object) id, (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture));
        if (!this.locks.ContainsKey(id))
          return false;
        this.locks[id].Set();
        return true;
      }
    }

    public void Discard(string id)
    {
      lock (this.syncObject)
      {
        id = this.ConvertId(id);
        if (this.locks.ContainsKey(id))
        {
          this.locks.Remove(id);
          Tracer.Information("*** DISCARD_LOCK: Lock '{0}' discarded by thread {1} ***", (object) id, (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else
          Tracer.Warning("*** DISCARD_LOCK: Lock '{0}' could not be discarded by thread {1} ***", (object) id, (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture));
      }
    }

    public bool Wait(string id, int timeoutMs)
    {
      id = this.ConvertId(id);
      Tracer.Information("*** WAIT: Thread {0} is waiting for unlocking of '{1}' ***", (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture), (object) id);
      if (this.locks.ContainsKey(id))
      {
        bool flag = this.locks[id].Wait(timeoutMs);
        if (flag)
          Tracer.Information("*** SIGNAL: Thread {0} is allowed to continue handling interface(s) for '{1}' ***", (object) Thread.CurrentThread.ManagedThreadId.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture), (object) id);
        else
          Tracer.Warning("*** NO_SIGNAL: Waiting for unlocking of '{0}' timed out ***", (object) id);
        return flag;
      }
      Tracer.Warning("*** NO_LOCK: No interface lock found for '{0}' ***", (object) id);
      return false;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "reviewed")]
    private string ConvertId(string id) => id.ToUpperInvariant();
  }
}
