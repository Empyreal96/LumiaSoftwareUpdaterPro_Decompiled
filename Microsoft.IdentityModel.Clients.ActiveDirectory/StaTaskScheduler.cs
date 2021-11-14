// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.StaTaskScheduler
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  internal sealed class StaTaskScheduler : TaskScheduler, IDisposable
  {
    private BlockingCollection<Task> _tasks;
    private readonly List<Thread> _threads;

    public StaTaskScheduler(int numberOfThreads)
    {
      if (numberOfThreads < 1)
        throw new ArgumentOutOfRangeException(nameof (numberOfThreads));
      this._tasks = new BlockingCollection<Task>();
      this._threads = Enumerable.Range(0, numberOfThreads).Select<int, Thread>((Func<int, Thread>) (i =>
      {
        Thread thread = new Thread((ThreadStart) (() =>
        {
          foreach (Task consuming in this._tasks.GetConsumingEnumerable())
            this.TryExecuteTask(consuming);
        }))
        {
          IsBackground = true
        };
        thread.SetApartmentState(ApartmentState.STA);
        return thread;
      })).ToList<Thread>();
      this._threads.ForEach((Action<Thread>) (t => t.Start()));
    }

    protected override void QueueTask(Task task) => this._tasks.Add(task);

    protected override IEnumerable<Task> GetScheduledTasks() => (IEnumerable<Task>) this._tasks.ToArray();

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => Thread.CurrentThread.GetApartmentState() == ApartmentState.STA && this.TryExecuteTask(task);

    public override int MaximumConcurrencyLevel => this._threads.Count;

    public void Dispose()
    {
      if (this._tasks == null)
        return;
      this._tasks.CompleteAdding();
      foreach (Thread thread in this._threads)
        thread.Join();
      this._tasks.Dispose();
      this._tasks = (BlockingCollection<Task>) null;
    }
  }
}
