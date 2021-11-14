// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.DeviceDetection.DeviceWatcher
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using Nokia.Lucid.DeviceDetection.Primitives;
using Nokia.Lucid.Diagnostics;
using Nokia.Lucid.Interop.Win32Types;
using Nokia.Lucid.Primitives;
using Nokia.Lucid.Properties;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Nokia.Lucid.DeviceDetection
{
  public sealed class DeviceWatcher : IHandleDeviceChanged, IHandleThreadException
  {
    private int currentStatus;
    private Func<DeviceIdentifier, bool> compiledFilter;
    private DeviceTypeMap cachedTypeMap;
    private Expression<Func<DeviceIdentifier, bool>> filter = FilterExpression.DefaultExpression;
    private DeviceTypeMap deviceTypeMap = DeviceTypeMap.DefaultMap;

    public event EventHandler<DeviceChangedEventArgs> DeviceChanged;

    public event EventHandler<ThreadExceptionEventArgs> ThreadException;

    public DeviceTypeMap DeviceTypeMap
    {
      get => this.deviceTypeMap;
      set => this.deviceTypeMap = value;
    }

    public Expression<Func<DeviceIdentifier, bool>> Filter
    {
      get => this.filter;
      set => this.filter = value;
    }

    public DeviceWatcherStatus Status => (DeviceWatcherStatus) this.currentStatus;

    public IDisposable Start()
    {
      Expression<Func<DeviceIdentifier, bool>> filter = this.Filter;
      if (filter != null)
      {
        RobustTrace.Trace<Expression<Func<DeviceIdentifier, bool>>>(new Action<Expression<Func<DeviceIdentifier, bool>>>(DeviceDetectionTraceSource.Instance.FilterExpressionCompilation_Start), filter);
        try
        {
          this.compiledFilter = filter.Compile();
        }
        catch (Exception ex)
        {
          if (!ExceptionServices.IsCriticalException(ex))
            RobustTrace.Trace<Expression<Func<DeviceIdentifier, bool>>, Exception>(new Action<Expression<Func<DeviceIdentifier, bool>>, Exception>(DeviceDetectionTraceSource.Instance.FilterExpressionCompilation_Error), filter, ex);
          throw;
        }
        RobustTrace.Trace<Expression<Func<DeviceIdentifier, bool>>>(new Action<Expression<Func<DeviceIdentifier, bool>>>(DeviceDetectionTraceSource.Instance.FilterExpressionCompilation_Stop), filter);
      }
      this.cachedTypeMap = this.deviceTypeMap;
      DeviceWatcherStatus deviceWatcherStatus = (DeviceWatcherStatus) Interlocked.CompareExchange(ref this.currentStatus, 1, 0);
      if (deviceWatcherStatus != DeviceWatcherStatus.Created)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidOperationException_MessageFormat_CouldNotStartDeviceWatcher, (object) deviceWatcherStatus));
      MessageWindow window = (MessageWindow) null;
      AggregateException exception = (AggregateException) null;
      ManualResetEventSlim threadReady = new ManualResetEventSlim();
      Thread thread = new Thread((ThreadStart) (() =>
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          try
          {
            MessageWindow.Create((IHandleDeviceChanged) this, (IHandleThreadException) this, ref window);
          }
          catch (AggregateException ex)
          {
            threadReady.Set();
            throw;
          }
          AggregateException aggregateException = (AggregateException) null;
          try
          {
            window.AttachWindowProc();
            window.RegisterDeviceNotification(this.cachedTypeMap.InterfaceClasses);
          }
          catch (AggregateException ex)
          {
            aggregateException = ex;
          }
          finally
          {
            threadReady.Set();
          }
          MessageLoop.Run();
          if (aggregateException != null && window.Exception != null)
            throw new AggregateException(new Exception[2]
            {
              (Exception) aggregateException,
              (Exception) window.Exception
            });
          if (window.Exception != null)
            throw new AggregateException(new Exception[1]
            {
              (Exception) window.Exception
            });
          if (aggregateException != null)
            throw new AggregateException(new Exception[1]
            {
              (Exception) aggregateException
            });
        }
        catch (AggregateException ex)
        {
          exception = ex;
        }
        finally
        {
          window?.Dispose();
        }
      }))
      {
        IsBackground = true
      };
      thread.Start();
      threadReady.Wait();
      if (window == null)
      {
        thread.Join();
        Interlocked.CompareExchange(ref this.currentStatus, 3, 1);
        throw exception;
      }
      return (IDisposable) new DeviceWatcher.InvokeOnceWhenDisposed((Action) (() =>
      {
        if (window.Status == MessageWindowStatus.Created)
        {
          try
          {
            window.CloseAsync();
          }
          catch (Win32Exception ex)
          {
          }
        }
        thread.Join();
        if (exception == null)
        {
          Interlocked.CompareExchange(ref this.currentStatus, 2, 1);
        }
        else
        {
          Interlocked.CompareExchange(ref this.currentStatus, 3, 1);
          throw exception;
        }
      }));
    }

    void IHandleDeviceChanged.HandleDeviceChanged(
      int eventType,
      ref DEV_BROADCAST_DEVICEINTERFACE data)
    {
      EventHandler<DeviceChangedEventArgs> deviceChanged = this.DeviceChanged;
      DeviceType deviceType;
      if (deviceChanged == null || !this.cachedTypeMap.TryGetMapping(data.dbcc_classguid, out deviceType))
        return;
      if (deviceType != DeviceType.Interface && deviceType != DeviceType.PhysicalDevice)
      {
        RobustTrace.Trace<Guid, DeviceType>(new Action<Guid, DeviceType>(DeviceDetectionTraceSource.Instance.InvalidDeviceMapping), data.dbcc_classguid, deviceType);
      }
      else
      {
        DeviceChangeAction action;
        switch (eventType)
        {
          case 32768:
            action = DeviceChangeAction.Attach;
            break;
          case 32772:
            action = DeviceChangeAction.Detach;
            break;
          default:
            return;
        }
        DeviceIdentifier result;
        if (!DeviceIdentifier.TryParse(data.dbcc_name, out result))
          return;
        RobustTrace.Trace<string>(new Action<string>(DeviceDetectionTraceSource.Instance.FilterExpressionEvaluation_Start), result.Value);
        bool flag;
        try
        {
          flag = this.compiledFilter != null && this.compiledFilter(result);
        }
        catch (Exception ex)
        {
          if (!ExceptionServices.IsCriticalException(ex))
            RobustTrace.Trace<string, Exception>(new Action<string, Exception>(DeviceDetectionTraceSource.Instance.FilterExpressionEvaluation_Error), result.Value, ex);
          throw;
        }
        if (!flag)
        {
          RobustTrace.Trace<string, bool>(new Action<string, bool>(DeviceDetectionTraceSource.Instance.FilterExpressionEvaluation_Stop), result.Value, false);
        }
        else
        {
          RobustTrace.Trace<string, bool>(new Action<string, bool>(DeviceDetectionTraceSource.Instance.FilterExpressionEvaluation_Stop), result.Value, true);
          RobustTrace.Trace<DeviceChangeAction, string, DeviceType>(new Action<DeviceChangeAction, string, DeviceType>(DeviceDetectionTraceSource.Instance.DeviceChangeEvent_Start), action, result.Value, deviceType);
          try
          {
            deviceChanged((object) this, new DeviceChangedEventArgs(action, result.Value, deviceType));
          }
          catch (Exception ex)
          {
            if (!ExceptionServices.IsCriticalException(ex))
              RobustTrace.Trace<DeviceChangeAction, string, DeviceType, Exception>(new Action<DeviceChangeAction, string, DeviceType, Exception>(DeviceDetectionTraceSource.Instance.DeviceChangeEvent_Error), action, result.Value, deviceType, ex);
            throw;
          }
          RobustTrace.Trace<DeviceChangeAction, string, DeviceType>(new Action<DeviceChangeAction, string, DeviceType>(DeviceDetectionTraceSource.Instance.DeviceChangeEvent_Stop), action, result.Value, deviceType);
        }
      }
    }

    bool IHandleThreadException.TryHandleThreadException(Exception exception)
    {
      EventHandler<ThreadExceptionEventArgs> threadException = this.ThreadException;
      if (threadException == null)
        return false;
      ThreadExceptionEventArgs e = new ThreadExceptionEventArgs(exception);
      threadException((object) this, e);
      return e.IsHandled;
    }

    private sealed class InvokeOnceWhenDisposed : IDisposable
    {
      private Action action;

      public InvokeOnceWhenDisposed(Action action) => this.action = action;

      public void Dispose()
      {
        Action action = Interlocked.Exchange<Action>(ref this.action, (Action) null);
        if (action == null)
          return;
        action();
      }
    }
  }
}
