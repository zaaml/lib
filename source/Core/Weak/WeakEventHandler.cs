// <copyright file="WeakEventHandler.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.Core.Weak
{
  internal class WeakEventHandlerProxy<THandler, TEventArgs>
    where TEventArgs : EventArgs
    where THandler : class
  {
    #region Static Fields and Constants

    private static readonly MethodInfo InvokeMethod = typeof(WeakEventHandlerProxy<THandler, TEventArgs>).GetMethod
      ("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);

    #endregion

    #region Fields

    private readonly Action<object, TEventArgs> _handler;

    #endregion

    #region Ctors

    public WeakEventHandlerProxy(Action<object, TEventArgs> handler)
    {
      _handler = handler;
      Handler = (THandler) (object) Delegate.CreateDelegate(typeof(THandler), this, InvokeMethod, false);
    }

    #endregion

    #region Properties

    public THandler Handler { get; }

    #endregion

    #region  Methods

    // ReSharper disable once UnusedMember.Local
    private void Invoke(object sender, EventArgs args)
    {
      _handler.DynamicInvoke(sender, args);
    }

    #endregion
  }

  internal class WeakEventHandler<TInstance, THandler, TEventArgs> : IDisposable
    where TEventArgs : EventArgs
    where THandler : class
  {
    #region Fields

    private Action<TInstance, object, TEventArgs> _handler;
    private WeakEventHandlerProxy<THandler, TEventArgs> _innerDelegateProxy;
    private bool _isDisposed;
    private Action<THandler> _removeHandler;
    private WeakReference<TInstance> _targetReference;

    #endregion

    #region Ctors

    public WeakEventHandler(TInstance target, Action<TInstance, object, TEventArgs> handler, Action<THandler> addHandler, Action<THandler> remove)
    {
      _innerDelegateProxy = new WeakEventHandlerProxy<THandler, TEventArgs>(Invoke);

      _targetReference = new WeakReference<TInstance>(target);
      _handler = handler;
      _removeHandler = remove;

      addHandler(_innerDelegateProxy.Handler);
    }

    #endregion

    #region  Methods

    private void Invoke(object sender, TEventArgs args)
    {
      if (_isDisposed)
        return;

      var target = _targetReference.Target;

      if (target == null)
      {
        Dispose();
        return;
      }

      _handler.Invoke(target, sender, args);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      if (_isDisposed)
        return;

      _removeHandler(_innerDelegateProxy.Handler);
      _removeHandler = null;
      _targetReference = null;
      _innerDelegateProxy = null;
      _handler = null;

      _isDisposed = true;
    }

    #endregion

    #endregion
  }

  public static class WeakEventHandler
  {
    #region  Methods

    public static IDisposable CreateWeakEventListener<TInstance, THandler, TEventArgs>(this TInstance @this, Action<TInstance, object, TEventArgs> handler, Action<THandler> addHandler, Action<THandler> removeHandler)
      where TEventArgs : EventArgs
      where THandler : class
    {
      return new WeakEventHandler<TInstance, THandler, TEventArgs>(@this, handler, addHandler, removeHandler);
    }

    public static IDisposable CreateWeakEventListener<TInstance, TEventArgs>(this TInstance @this, Action<TInstance, object, TEventArgs> handler, Action<EventHandler<TEventArgs>> addHandler,
      Action<EventHandler<TEventArgs>> removeHandler)
      where TEventArgs : EventArgs
    {
      return new WeakEventHandler<TInstance, EventHandler<TEventArgs>, TEventArgs>(@this, handler, addHandler, removeHandler);
    }

    public static IDisposable CreateWeakEventListener<TInstance>(this TInstance @this, Action<TInstance, object, EventArgs> handler, Action<EventHandler> addHandler, Action<EventHandler> removeHandler)
    {
      return new WeakEventHandler<TInstance, EventHandler, EventArgs>(@this, handler, addHandler, removeHandler);
    }

    #endregion
  }
}