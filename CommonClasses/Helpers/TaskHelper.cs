// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.TaskHelper
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LsuPro.Helpers
{
  [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "reviewed")]
  public class TaskHelper : IDisposable
  {
    private Task task;

    public TaskHelper(Action action) => this.task = new Task(action);

    public TaskHelper(Action action, CancellationToken cancellationToken) => this.task = new Task(action, cancellationToken);

    public AggregateException Exception => this.task.Exception;

    public void Start() => this.task.Start();

    public void ContinueWith(Action<object> action, TaskContinuationOptions taskContinuationOptions) => this.task.ContinueWith((Action<Task>) action, taskContinuationOptions);

    public TaskStatus Status() => this.task.Status;

    public void Wait() => this.task.Wait();

    public void Wait(int time) => this.task.Wait(time);

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "reviewed")]
    public void Dispose()
    {
      this.task.Dispose();
      GC.SuppressFinalize((object) this);
    }

    public void StartAndContinueWith(Action<System.Exception> action)
    {
      this.task.Start();
      this.task.ContinueWith((Action<Task>) (t => action((System.Exception) this.task.Exception)));
    }
  }
}
