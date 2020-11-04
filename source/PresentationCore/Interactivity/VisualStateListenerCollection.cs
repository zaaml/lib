// <copyright file="VisualStateListenerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class VisualStateListenerCollection
  {
    #region Fields

    private bool _isActive;

    #endregion

    #region Ctors

    public VisualStateListenerCollection(string stateName)
    {
      StateName = stateName;
    }

    #endregion

    #region Properties

    public bool IsActive
    {
      get => _isActive;
      internal set
      {
        if (_isActive == value)
          return;

        _isActive = value;
        OnIsActiveChanged();
      }
    }

    public List<IVisualStateListener> Listeners { get; } = new List<IVisualStateListener>();

    public string StateName { get; }

    #endregion

    #region  Methods

    public void AttachListener(IVisualStateListener listener)
    {
      Listeners.Add(listener);
      if (IsActive)
        listener.EnterState(false);
    }

    public void DetachListener(IVisualStateListener listener)
    {
      Listeners.Remove(listener);
      if (IsActive)
        listener.LeaveState(false);
    }

    private void OnIsActiveChanged()
    {
      UpdateListeners(IsActive, false);
    }

    public void SetIsActive(bool value, bool useTransitions)
    {
      if (IsActive == value)
        return;

      _isActive = value;

      UpdateListeners(value, useTransitions);
    }

    private void UpdateListeners(bool isActive, bool useTransitions)
    {
      // ReSharper disable ForCanBeConvertedToForeach
      if (isActive)
        for (var index = 0; index < Listeners.Count; index++)
          Listeners[index].EnterState(useTransitions);
      else
        for (var index = 0; index < Listeners.Count; index++)
          Listeners[index].LeaveState(useTransitions);
      // ReSharper restore ForCanBeConvertedToForeach
    }

    #endregion
  }
}