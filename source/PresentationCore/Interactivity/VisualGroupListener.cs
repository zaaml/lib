// <copyright file="VisualGroupListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class VisualGroupListener
  {
    #region Fields

    private readonly List<VisualStateListenerCollection> _listeners = new List<VisualStateListenerCollection>();

    #endregion

    #region Properties

    public string CurrentStateName { get; private set; }

    #endregion

    #region  Methods

    public void AddStateListenersCollection(VisualStateListenerCollection stateListeners)
    {
      _listeners.Add(UpdateListenerCollection(stateListeners));
    }

    public VisualStateListenerCollection GetListenerCollection(string name, bool create)
    {
      // ReSharper disable once ForCanBeConvertedToForeach
      for (var index = 0; index < _listeners.Count; index++)
      {
        var listener = _listeners[index];
        if (listener.StateName.Equals(name, StringComparison.Ordinal))
          return listener;
      }

      if (create == false)
        return null;

      var newListener = UpdateListenerCollection(new VisualStateListenerCollection(name));

      _listeners.Add(newListener);

      return newListener;
    }

    public void GoToState(string state, bool useTransitions)
    {
      if (string.Equals(state, CurrentStateName, StringComparison.OrdinalIgnoreCase))
        return;

      GoToState(state, CurrentStateName, useTransitions);
    }

    private void GoToState(string newState, string oldState, bool useTransitions)
    {
      var cnt = 2;
      VisualStateListenerCollection oldListener = null;
      VisualStateListenerCollection newListener = null;

      // ReSharper disable once ForCanBeConvertedToForeach
      for (var index = 0; index < _listeners.Count; index++)
      {
        var listener = _listeners[index];
        if (string.Equals(listener.StateName, newState, StringComparison.Ordinal))
        {
          newListener = listener;
          cnt--;
        }

        if (string.Equals(listener.StateName, oldState, StringComparison.Ordinal))
        {
          oldListener = listener;
          cnt--;
        }

        if (cnt == 0)
          break;
      }

      newListener?.SetIsActive(true, useTransitions);
      oldListener?.SetIsActive(false, useTransitions);

      CurrentStateName = newState;
    }

    private VisualStateListenerCollection UpdateListenerCollection(VisualStateListenerCollection listenerCollection)
    {
      listenerCollection.IsActive = string.Equals(CurrentStateName, listenerCollection.StateName, StringComparison.Ordinal);
      return listenerCollection;
    }

    #endregion
  }
}