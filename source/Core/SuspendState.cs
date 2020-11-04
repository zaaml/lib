// <copyright file="SuspendState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Disposable;

namespace Zaaml.Core
{
  internal interface ISuspendState
  {
    #region Properties

    bool IsSuspended { get; }

    #endregion

    #region  Methods

    void Resume();
    void Suspend();

    #endregion
  }

  internal class DummySuspendState : ISuspendState
  {
    #region Static Fields and Constants

    public static readonly ISuspendState Instance = new DummySuspendState();

    #endregion

    #region Ctors

    private DummySuspendState()
    {
    }

    #endregion

    #region Interface Implementations

    #region ISuspendState

    public bool IsSuspended => false;

    public void Resume()
    {
    }

    public void Suspend()
    {
    }

    #endregion

    #endregion
  }

  internal class SuspendState : ISuspendState
  {
    #region Fields

    private int _suspendCount;

    #endregion

    #region Interface Implementations

    #region ISuspendState

    public bool IsSuspended => _suspendCount > 0;

    public void Resume()
    {
      if (_suspendCount > 0)
        _suspendCount--;
    }

    public void Suspend()
    {
      _suspendCount++;
    }

    #endregion

    #endregion
  }

  public abstract class SuspendStateBase : ISuspendState
  {
    #region Properties

    protected int SuspendCount { get; private set; }

    #endregion

    #region  Methods

    protected abstract void OnResume();

    protected abstract void OnSuspend();

    #endregion

    #region Interface Implementations

    #region ISuspendState

    public bool IsSuspended => SuspendCount > 0;

    public void Resume()
    {
      if (SuspendCount == 0)
        return;

      SuspendCount--;

      if (SuspendCount == 0)
        OnResume();
    }

    public void Suspend()
    {
      if (SuspendCount == 0)
        OnSuspend();

      SuspendCount++;
    }

    #endregion

    #endregion
  }

  public sealed class ObservableSuspendState : SuspendStateBase
  {
    #region Fields

    public event EventHandler Resumed;
    public event EventHandler Suspended;

    #endregion

    #region  Methods

    protected override void OnResume()
    {
      Resumed?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnSuspend()
    {
      Suspended?.Invoke(this, EventArgs.Empty);
    }

    #endregion
  }

  public sealed class DelegateObservableSuspendState : SuspendStateBase
  {
    #region Fields

    private readonly Action _onResume;
    private readonly Action _onSuspend;

    #endregion

    #region Ctors

    public DelegateObservableSuspendState(Action onSuspend, Action onResume)
    {
      _onSuspend = onSuspend;
      _onResume = onResume;
    }

    #endregion

    #region  Methods

    protected override void OnResume()
    {
      _onResume();
    }

    protected override void OnSuspend()
    {
      _onSuspend();
    }

    #endregion
  }

  public static class SuspendableAction
  {
    #region  Methods

    public static Action Create(Action action, ObservableSuspendState suspendState)
    {
      var suspendableAction = new SuspendableActionImpl(action, suspendState);
      return suspendableAction.Invoke;
    }

    #endregion

    #region  Nested Types

    private class SuspendableActionImpl
    {
      #region Fields

      private readonly Action _action;
      private readonly ObservableSuspendState _suspendState;
      private bool _invokeQueried;

      #endregion

      #region Ctors

      public SuspendableActionImpl(Action action, ObservableSuspendState suspendState)
      {
        _suspendState = suspendState;
        _action = action;
      }

      #endregion

      #region  Methods

      public void Invoke()
      {
        if (_suspendState.IsSuspended)
        {
          if (_invokeQueried) return;

          _suspendState.Resumed += SuspendStateOnResumed;
          _invokeQueried = true;
        }
        else
          _action();
      }

      private void SuspendStateOnResumed(object sender, EventArgs eventArgs)
      {
        _invokeQueried = false;
        _suspendState.Resumed -= SuspendStateOnResumed;
        _action();
      }

      #endregion
    }

    #endregion
  }

  internal static class Suspender
  {
    #region  Methods

    public static IDisposable EnterSuspendState(this ISuspendState suspendState)
    {
      return new DelegateDisposable(suspendState.Suspend, suspendState.Resume);
    }

    #endregion
  }

  internal class BoolSuspender
  {
    #region Properties

    public bool IsSuspended { get; private set; }

    #endregion

    #region  Methods

    public void Resume()
    {
      IsSuspended = false;
    }

    public void Suspend()
    {
      IsSuspended = true;
    }

    public static IDisposable Suspend(BoolSuspender suspender)
    {
      return DelegateDisposable.Create(suspender.Suspend, suspender.Resume);
    }

    #endregion
  }
}