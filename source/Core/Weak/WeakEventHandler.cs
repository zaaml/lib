// <copyright file="WeakEventHandler.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.Core.Weak
{
	internal class WeakEventHandlerProxy<THandler, TEventArgs>
		where TEventArgs : EventArgs
		where THandler : class
	{
		private static readonly MethodInfo InvokeMethod = typeof(WeakEventHandlerProxy<THandler, TEventArgs>).GetMethod
			(nameof(Invoke), BindingFlags.Instance | BindingFlags.NonPublic);

		private readonly Action<object, TEventArgs> _handler;

		public WeakEventHandlerProxy(Action<object, TEventArgs> handler)
		{
			_handler = handler;
			Handler = (THandler) (object) Delegate.CreateDelegate(typeof(THandler), this, InvokeMethod, false);
		}

		public THandler Handler { get; }

		
		private void Invoke(object sender, EventArgs args)
		{
			_handler.DynamicInvoke(sender, args);
		}
	}

	internal class WeakEvent<TSource, TEventArgs> : IDisposable
		where TEventArgs : EventArgs
	{
		private readonly Action<TSource, EventHandler<TEventArgs>> _addHandler;
		private readonly Action<TSource, EventHandler<TEventArgs>> _removeHandler;
		private readonly WeakReference<TSource> _source;

		public event EventHandler<TEventArgs> Event;

		public WeakEvent(TSource source, Action<TSource, EventHandler<TEventArgs>> addHandler, Action<TSource, EventHandler<TEventArgs>> removeHandler)
		{
			_source = new WeakReference<TSource>(source);
			_addHandler = addHandler;
			_removeHandler = removeHandler;
			_addHandler(source, OnEvent);
		}

		private void OnEvent(object source, TEventArgs e)
		{
			Event?.Invoke(source, e);
		}

		public void Dispose()
		{
			var target = _source.Target;

			if (target != null)
				_removeHandler(target, OnEvent);
		}
	}

	internal static class WeakEventExtensions
	{
		public static WeakEvent<TSource, TEventArgs> WeakEvent<TSource, TEventArgs>(this TSource source, Action<TSource, EventHandler<TEventArgs>> addHandler, Action<TSource, EventHandler<TEventArgs>> removeHandler)
			where TEventArgs : EventArgs
		{
			return new WeakEvent<TSource, TEventArgs>(source, addHandler, removeHandler);
		}
	}

	internal class WeakEventHandler<TInstance, THandler, TEventArgs> : IDisposable
		where TEventArgs : EventArgs
		where THandler : class
	{
		private Action<TInstance, object, TEventArgs> _handler;
		private WeakEventHandlerProxy<THandler, TEventArgs> _innerDelegateProxy;
		private bool _isDisposed;
		private Action<THandler> _removeHandler;
		private WeakReference<TInstance> _targetReference;

		public WeakEventHandler(TInstance target, Action<TInstance, object, TEventArgs> handler, Action<THandler> addHandler, Action<THandler> remove)
		{
			_innerDelegateProxy = new WeakEventHandlerProxy<THandler, TEventArgs>(Invoke);
			_targetReference = new WeakReference<TInstance>(target);
			_handler = handler;
			_removeHandler = remove;

			addHandler(_innerDelegateProxy.Handler);
		}

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
	}

	public static class WeakEventHandler
	{
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
	}
}