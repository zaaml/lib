// <copyright file="InteractivityRoot.VisualStateObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
  internal partial class InteractivityRoot : IVisualStateObserver
  {
    #region Fields

    private readonly Dictionary<string, MulticastVisualStateListener> _listeners = new Dictionary<string, MulticastVisualStateListener>();
    private readonly Dictionary<string, WeakVisualStateListener> _weakListeners = new Dictionary<string, WeakVisualStateListener>();
    private WeakReference _realObserverWeak;

    #endregion

    #region Properties

    public IVisualStateObserver RealVisualStateObserver
    {
      get => (IVisualStateObserver) _realObserverWeak?.Target;
      set
      {
        var realObserver = RealVisualStateObserver;

        if (ReferenceEquals(realObserver, value))
          return;

        _realObserverWeak = value != null ? new WeakReference(value) : null;

        foreach (var listener in WeakListeners)
        {
          realObserver?.DetachListener(listener);
          value?.AttachListener(listener);
        }
      }
    }

    private IEnumerable<IVisualStateListener> WeakListeners => _listeners.Values.Select(GetWeakListener);

    #endregion

    #region  Methods

    private MulticastVisualStateListener CreateListener(string visualStateName)
    {
      var listener = new MulticastVisualStateListener(visualStateName);

      RealVisualStateObserver?.AttachListener(GetWeakListener(listener));

      return listener;
    }

    protected abstract void EnsureVisualStateObserver();

    private WeakVisualStateListener GetWeakListener(MulticastVisualStateListener listener)
    {
      return _weakListeners.GetValueOrCreate(listener.VisualStateName, () => new WeakVisualStateListener(listener));
    }

    #endregion

    #region Interface Implementations

    #region IVisualStateObserver

    public void AttachListener(IVisualStateListener listener)
    {
      _listeners.GetValueOrCreate(listener.VisualStateName, CreateListener).AttachListener(listener);
    }

    public void DetachListener(IVisualStateListener listener)
    {
      _listeners.GetValueOrCreate(listener.VisualStateName, CreateListener).DetachListener(listener);
    }

    #endregion

    #endregion
  }
}