// <copyright file="WindowHookService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core;

namespace Zaaml.Platform.Hooks
{
	internal abstract class WindowHookServiceBase<TListener>
	{
		private readonly HashSet<TListener> _addQueue = new HashSet<TListener>();
		private readonly HookProc _hookProcDelegate;
		private readonly HookType _hookType;
		private readonly List<TListener> _listeners = new List<TListener>();
		private readonly HashSet<TListener> _removeQueue = new HashSet<TListener>();
		private IntPtr _hookPtr;
		private int _listenersLocked;

		protected WindowHookServiceBase(HookType hookType)
		{
			_hookType = hookType;
			_hookProcDelegate = OnHookProcPrivate;
		}

		private bool IsListenersLocked => _listenersLocked > 0;

		protected IReadOnlyList<TListener> Listeners => _listeners;

		public void AddListener(TListener listener)
		{
			AddListenerImpl(listener);
		}

		private void AddListenerImpl(TListener listener, bool updateHook = true)
		{
			if (IsListenersLocked)
			{
				_removeQueue.Remove(listener);
				_addQueue.Add(listener);
			}
			else
				_listeners.Add(listener);

			if (updateHook)
				UpdateHook();
		}

		private void CurrentDomainOnDomainUnload(object sender, EventArgs e)
		{
			Unhook();
		}

		private void CurrentDomainOnProcessExit(object sender, EventArgs e)
		{
			Unhook();
		}

		private void Hook()
		{
			if (_hookPtr != IntPtr.Zero)
				return;

			_hookPtr = NativeMethods.SetWindowsHookEx(_hookType, _hookProcDelegate, IntPtr.Zero, NativeMethods.GetCurrentThreadId());

			AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
		}

		protected void LockListeners()
		{
			_listenersLocked++;
		}

		protected abstract IntPtr OnHookProc(int code, IntPtr wParam, IntPtr lParam, ref bool handled);

		private IntPtr OnHookProcPrivate(int code, IntPtr wParam, IntPtr lParam)
		{
			LockListeners();

			try
			{
				var handled = false;
				var result = OnHookProc(code, wParam, lParam, ref handled);

				return handled ? result : NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
			}
			catch (Exception e)
			{
				LogService.LogError(e);

				return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
			}
			finally
			{
				UnlockListeners();
			}
		}

		private void ProcessListenersQueue()
		{
			if (IsListenersLocked)
				return;

			foreach (var listener in _addQueue)
				AddListenerImpl(listener, false);

			foreach (var listener in _removeQueue)
				RemoveListenerImpl(listener, false);

			_addQueue.Clear();
			_removeQueue.Clear();

			UpdateHook();
		}

		public void RemoveListener(TListener listener)
		{
			RemoveListenerImpl(listener);
		}

		private void RemoveListenerImpl(TListener listener, bool updateHook = true)
		{
			if (IsListenersLocked)
			{
				_removeQueue.Add(listener);
				_addQueue.Remove(listener);
			}
			else
				_listeners.Remove(listener);

			if (updateHook)
				UpdateHook();
		}

		private void Unhook()
		{
			if (_hookPtr == IntPtr.Zero)
				return;

			NativeMethods.UnhookWindowsHookEx(_hookPtr);
			_hookPtr = IntPtr.Zero;

			AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit;
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomainOnDomainUnload;
		}

		protected void UnlockListeners()
		{
			if (_listenersLocked == 0)
				throw new InvalidOperationException();

			_listenersLocked--;

			if (_listenersLocked == 0)
				ProcessListenersQueue();
		}

		private void UpdateHook()
		{
			if (_listeners.Count > 0)
				Hook();
			else
				Unhook();
		}
	}
}