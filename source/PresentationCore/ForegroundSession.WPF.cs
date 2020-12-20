// <copyright file="ForegroundSession.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Interop;
using Zaaml.Core;
using Zaaml.Platform;
using Zaaml.Platform.Hooks;

namespace Zaaml.PresentationCore
{
	internal class ForegroundSession : IDisposable, ICallWndProcHookListener
	{
		#region Fields

		private readonly UIElement _element;
		private HwndSource _hwnd;
		private IntPtr _hwndHandle;
		private bool _layoutEventAttached;
		private Action _onLeave;

		#endregion

		#region Ctors

		private ForegroundSession(UIElement element, Action onLeave)
		{
			_element = element;
			_onLeave = onLeave;

			AttachLayoutEvent();

			HwndSource = PresentationSource.FromDependencyObject(_element) as HwndSource;

			CallWndProcHookService.Instance.AddListener(this);
		}

		#endregion

		#region Properties

		private HwndSource HwndSource
		{
			set
			{
				if (_hwnd?.Handle == value?.Handle || value == null)
					return;

				_hwndHandle = IntPtr.Zero;

				_hwnd = value;

				DetachLayoutEvent();

				if (_hwnd != null)
				{
					var owner = NativeMethods.GetAncestor(_hwnd.Handle, GetAncestorFlags.GetRoot);

					_hwndHandle = owner == IntPtr.Zero ? _hwnd.Handle : owner;
				}

				try
				{
					if (PlatformUtil.IsForegroundWindowFromCurrentProcess() == false)
						NativeMethods.SetForegroundWindow(_hwnd.Handle);
				}
				catch (Exception e)
				{
					LogService.LogError(e);
				}
			}
		}

		#endregion

		#region  Methods

		private void ActualWndProc(int msg, IntPtr wParam, IntPtr lParam)
		{
			var tmsg = (WM) msg;

			//if (tmsg == WM.ACTIVATE && wParam.ToInt32() == 0)
			//{
			// _onLeave();
			// Dispose();
			//}

			if (tmsg == WM.ACTIVATEAPP)
			{
				if (wParam.ToInt32() == 0)
				{
					_onLeave();
					
					Dispose();
				}
			}
		}

		private void AttachLayoutEvent()
		{
			if (_layoutEventAttached)
				return;

			_element.LayoutUpdated += ElementOnLayoutUpdated;

			_layoutEventAttached = true;
		}

		private void DetachLayoutEvent()
		{
			if (_layoutEventAttached == false)
				return;

			_element.LayoutUpdated -= ElementOnLayoutUpdated;

			_layoutEventAttached = false;
		}

		private void ElementOnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			HwndSource = PresentationSource.FromDependencyObject(_element) as HwndSource;
		}

		internal static IDisposable Enter(UIElement element, Action onLeave)
		{
			return new ForegroundSession(element, onLeave);
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		public void Dispose()
		{
			DetachLayoutEvent();

			CallWndProcHookService.Instance.RemoveListener(this);

			_hwnd = null;
			_onLeave = null;
			_hwndHandle = IntPtr.Zero;
		}

		#endregion

		#region IWindowMessageListener

		void ICallWndProcHookListener.OnCallWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			if (hwnd == _hwndHandle)
				ActualWndProc(msg, wParam, lParam);
		}

		#endregion

		#endregion
	}
}