// <copyright file="HwndMouseObserver.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Platform;
using Zaaml.Platform.Hooks;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Input
{
	internal sealed class HwndMouseObserver : IGetMessageHookListener
	{
		#region Static Fields and Constants

		private static readonly Lazy<HwndMouseObserver> LazyInstance = new Lazy<HwndMouseObserver>(() => new HwndMouseObserver());

		#endregion

		#region Fields

		private readonly HashSet<IMouseEventListener> _addQueue = new HashSet<IMouseEventListener>();
		private readonly HashSet<IMouseEventListener> _removeQueue = new HashSet<IMouseEventListener>();
		private bool _hookInstalled;
		private bool _inEventHandler;

		#endregion

		#region Ctors

		private HwndMouseObserver()
		{
		}

		#endregion

		#region Properties

		private static HwndMouseObserver Instance => LazyInstance.Value;

		private List<IMouseEventListener> Listeners { get; } = new List<IMouseEventListener>();

		#endregion

		#region  Methods

		public static void AddListener(IMouseEventListener listener)
		{
			Instance.AddListenerImpl(listener);
		}

		private void AddListenerImpl(IMouseEventListener listener, bool updateHookService = true)
		{
			if (_inEventHandler)
			{
				_removeQueue.Remove(listener);
				_addQueue.Add(listener);
			}
			else
				Listeners.Add(listener);

			if (updateHookService)
				UpdateHookService();
		}

		private MouseButtons GetButtons(IntPtr wparam)
		{
			return new MouseButtons();
		}

		private static MouseButtons GetButtons()
		{
			var leftButton = MouseButtonStateFromBool(KeyState.IsKeyPressed(VirtualKeyStates.VK_LBUTTON));
			var rightButton = MouseButtonStateFromBool(KeyState.IsKeyPressed(VirtualKeyStates.VK_RBUTTON));
			var middleButton = MouseButtonStateFromBool(KeyState.IsKeyPressed(VirtualKeyStates.VK_MBUTTON));

			return MouseButtons.CreateButtonsState(leftButton, rightButton, middleButton);
		}

		private Point GetScreenPosition(IntPtr handle, IntPtr lParam)
		{
			return NativeMethodsSafe.ClientToScreen(handle, WinAPIHelper.GetPoint(lParam)).ToPresentationPoint();
		}

		private static Point GetScreenPosition()
		{
			return NativeMethodsSafe.GetCursorPos().ToPresentationPoint();
		}

		private static MouseButtonStateKind MouseButtonStateFromBool(bool isPressed)
		{
			return isPressed ? MouseButtonStateKind.Pressed : MouseButtonStateKind.Released;
		}

		private static MouseButtonStateKind MouseButtonStateFromInt(int isPressed)
		{
			return isPressed != 0 ? MouseButtonStateKind.Pressed : MouseButtonStateKind.Released;
		}

		void NotifyMouseEvent(MouseEventInfo eventInfo)
		{
			_inEventHandler = true;

			try
			{
				foreach (var hwndMouseListener in Listeners)
					hwndMouseListener.OnMouseEvent(eventInfo);
			}
			finally
			{
				_inEventHandler = false;

				ProcessListenersQueue();
			}
		}

		private void OnMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			switch ((WM) msg)
			{
				case WM.NCMOUSELEAVE:
					OnMouseLeaveEvent(hwnd, MouseEventAreaKind.NonClient, GetScreenPosition(), GetButtons());

					break;
				case WM.NCMOUSEMOVE:
					OnMouseMoveEvent(hwnd, MouseEventAreaKind.NonClient, GetScreenPosition(hwnd, lparam), GetButtons());

					break;
				case WM.MOUSELEAVE:
					OnMouseLeaveEvent(hwnd, MouseEventAreaKind.Client, GetScreenPosition(), GetButtons());

					break;
				case WM.MOUSEMOVE:
					OnMouseMoveEvent(hwnd, MouseEventAreaKind.Client, GetScreenPosition(hwnd, lparam), GetButtons(wparam));

					break;

				case WM.LBUTTONDOWN:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Left, MouseButtonEventKind.Down, MouseEventAreaKind.Client, GetScreenPosition(hwnd, lparam), GetButtons(wparam));

					break;
				case WM.LBUTTONUP:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Left, MouseButtonEventKind.Up, MouseEventAreaKind.Client, GetScreenPosition(hwnd, lparam), GetButtons(wparam));

					break;
				case WM.RBUTTONDOWN:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Right, MouseButtonEventKind.Down, MouseEventAreaKind.Client, GetScreenPosition(hwnd, lparam), GetButtons(wparam));

					break;
				case WM.RBUTTONUP:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Right, MouseButtonEventKind.Up, MouseEventAreaKind.Client, GetScreenPosition(hwnd, lparam), GetButtons(wparam));

					break;

				case WM.NCLBUTTONDOWN:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Left, MouseButtonEventKind.Down, MouseEventAreaKind.NonClient, GetScreenPosition(hwnd, lparam), GetButtons());

					break;
				case WM.NCLBUTTONUP:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Left, MouseButtonEventKind.Up, MouseEventAreaKind.NonClient, GetScreenPosition(hwnd, lparam), GetButtons());

					break;
				case WM.NCRBUTTONDOWN:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Right, MouseButtonEventKind.Down, MouseEventAreaKind.NonClient, GetScreenPosition(hwnd, lparam), GetButtons());

					break;
				case WM.NCRBUTTONUP:
					OnMouseButtonEvent(hwnd, MouseButtonKind.Right, MouseButtonEventKind.Up, MouseEventAreaKind.NonClient, GetScreenPosition(hwnd, lparam), GetButtons());

					break;
			}
		}

		private void OnMouseButtonEvent(IntPtr handle, MouseButtonKind button, MouseButtonEventKind eventKind, MouseEventAreaKind areaKind, Point screenPosition, MouseButtons buttons)
		{
			OnMouseEvent(MouseEventInfo.CreateMouseButtonInfo(handle, screenPosition, buttons, button, areaKind));
		}

		private void OnMouseEvent(MouseEventInfo eventInfo)
		{
			NotifyMouseEvent(eventInfo);
		}

		private void OnMouseLeaveEvent(IntPtr handle, MouseEventAreaKind areaKind, Point screenPosition, MouseButtons buttons)
		{
			OnMouseEvent(MouseEventInfo.CreateMouseLeaveInfo(handle, screenPosition, buttons, areaKind));
		}

		private void OnMouseMoveEvent(IntPtr handle, MouseEventAreaKind areaKind, Point screenPosition, MouseButtons buttons)
		{
			OnMouseEvent(MouseEventInfo.CreateMouseMoveInfo(handle, screenPosition, buttons, areaKind));
		}

		private void ProcessListenersQueue()
		{
			if (_inEventHandler)
				return;

			foreach (var listener in _addQueue)
				AddListenerImpl(listener, false);

			foreach (var listener in _removeQueue)
				RemoveListenerImpl(listener, false);

			_addQueue.Clear();
			_removeQueue.Clear();

			UpdateHookService();
		}

		public static void RemoveListener(IMouseEventListener listener)
		{
			Instance.RemoveListenerImpl(listener);
		}

		private void RemoveListenerImpl(IMouseEventListener listener, bool updateHookService = true)
		{
			if (_inEventHandler)
			{
				_removeQueue.Add(listener);
				_addQueue.Remove(listener);
			}
			else
				Listeners.Remove(listener);

			if (updateHookService)
				UpdateHookService();
		}

		private void UpdateHookService()
		{
			if (Listeners.Count > 0)
			{
				if (_hookInstalled == false)
				{
					GetMessageHookService.Instance.AddListener(this);
					_hookInstalled = true;
				}
			}
			else
			{
				if (_hookInstalled)
				{
					GetMessageHookService.Instance.RemoveListener(this);
					_hookInstalled = false;
				}
			}
		}

		#endregion

		#region Interface Implementations

		#region IGetMessageHookListener

		void IGetMessageHookListener.OnGetMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			OnMessage(hwnd, msg, wParam, lParam);
		}

		#endregion

		#endregion

		#region  Nested Types

		static class WinAPIHelper
		{
			#region Static Fields and Constants

			const int MK_LBUTTON = 0x1;
			const int MK_RBUTTON = 0x2;
			const int MK_MBUTTON = 0x10;

			#endregion

			#region  Methods

			public static MouseButtons GetButtons(IntPtr wParam)
			{
				var btns = GetInt(wParam);

				var leftButton = MouseButtonStateFromInt(btns & MK_LBUTTON);
				var rightButton = MouseButtonStateFromInt(btns & MK_RBUTTON);
				var middleButton = MouseButtonStateFromInt(btns & MK_MBUTTON);

				return MouseButtons.CreateButtonsState(leftButton, rightButton, middleButton);
			}

			static int GetInt(IntPtr ptr)
			{
				return IntPtr.Size == 8 ? unchecked((int) ptr.ToInt64()) : ptr.ToInt32();
			}

			public static POINT GetPoint(IntPtr lParam)
			{
				var i = GetInt(lParam);

				return new POINT
				{
					x = (short) LoWord(i),
					y = (short) HiWord(i)
				};
			}

			private static int HiWord(int n)
			{
				return n >> 16 & ushort.MaxValue;
			}

			private static int LoWord(int n)
			{
				return n & ushort.MaxValue;
			}

			#endregion
		}

		#endregion
	}
}