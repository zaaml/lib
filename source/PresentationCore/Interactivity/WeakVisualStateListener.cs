// <copyright file="WeakVisualStateListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Interfaces;

namespace Zaaml.PresentationCore.Interactivity
{
  internal class WeakVisualStateListener : IVisualStateListener, IWeakReference
  {
    #region Fields

    private readonly WeakReference _weakReference;

    #endregion

    #region Ctors

    public WeakVisualStateListener(IVisualStateListener listener)
    {
      VisualStateName = listener.VisualStateName;
      _weakReference = new WeakReference(listener);
    }

    #endregion

    #region Properties

    public IVisualStateListener Listener => (IVisualStateListener) _weakReference.Target;

    #endregion

    #region Interface Implementations

    #region IVisualStateListener

    public string VisualStateName { get; }

    public void EnterState(bool useTransitions)
    {
      Listener?.EnterState(useTransitions);
    }

    public void LeaveState(bool useTransitions)
    {
      Listener?.LeaveState(useTransitions);
    }

    #endregion

    #region IWeakReference

    bool IWeakReference.IsAlive => _weakReference.IsAlive;

    #endregion

    #endregion
  }
}