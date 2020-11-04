// <copyright file="MulticastVisualStateListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.Interactivity
{
  internal class MulticastVisualStateListener : IVisualStateListener
  {
    #region Fields

    private readonly List<IVisualStateListener> _listeners = new List<IVisualStateListener>();
    private bool _isActive;

    #endregion

    #region Ctors

    public MulticastVisualStateListener(string visualStateName)
    {
      VisualStateName = visualStateName;
    }

    #endregion

    #region  Methods

    public void AttachListener(IVisualStateListener listener)
    {
      ValidateListener(listener);

      _listeners.Add(listener);

      if (_isActive)
        listener.EnterState(false);
    }

    public void DetachListener(IVisualStateListener listener)
    {
      ValidateListener(listener);

      _listeners.Remove(listener);

      if (_isActive)
        listener.LeaveState(false);
    }

    private void ValidateListener(IVisualStateListener listener)
    {
      if (string.Equals(VisualStateName, listener.VisualStateName) == false)
        throw new ArgumentOutOfRangeException(nameof(listener));
    }

    #endregion

    #region Interface Implementations

    #region IVisualStateListener

    public string VisualStateName { get; }

    public void EnterState(bool useTransitions)
    {
      _isActive = true;
      foreach (var listener in _listeners)
        listener.EnterState(useTransitions);
    }

    public void LeaveState(bool useTransitions)
    {
      _isActive = false;
      foreach (var listener in _listeners)
        listener.LeaveState(useTransitions);
    }

    #endregion

    #endregion
  }
}