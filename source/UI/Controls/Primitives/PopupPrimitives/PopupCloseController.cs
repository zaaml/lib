// <copyright file="PopupCloseController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT
#else
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
#endif
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal class PopupCloseController : DependencyObject, IDisposable, IMouseEventListener
	{
		public static readonly DependencyProperty PopupChildProperty = DPM.Register<UIElement, PopupCloseController>
			("PopupChild", c => c.OnPopupChildChanged);

		private readonly Action _onClose;
		private readonly Popup _popup;
#if !SILVERLIGHT
		private IDisposable _foregroundSession;
#endif

		private bool _hadKeyboardFocus;
		private MouseButtonEventInfo _leftButtonDownInfo;
		private MouseButtonEventInfo _rightButtonDownInfo;
		private State _state;

		public event EventHandler<PopupCancelEventArgs> Closing;

		public event EventHandler<QueryCloseOnClickEventArgs> QueryCloseOnClick;

		public PopupCloseController(Popup popup) : this(popup, null)
		{
		}

		public PopupCloseController(Popup popup, Action onClose)
		{
			_popup = popup;
			_onClose = onClose;

			_popup.Opened += PopupControllerOnOpened;
			_popup.Closed += PopupControllerOnClosed;

			this.SetBinding(PopupChildProperty, new Binding {Path = new PropertyPath(Popup.ChildProperty), Source = _popup});
		}

		private bool HasKeyboardFocus
		{
			get
			{
				if (_popup.Owner?.FocusScopeElement != null && FocusHelper.IsKeyboardFocusWithin(_popup.Owner?.FocusScopeElement))
					return true;

				return FocusHelper.IsKeyboardFocusWithin(_popup);
			}
		}

		private static IGlobalMouseEventsProducer MouseEvents
		{
			get
			{
#if SILVERLIGHT
        return MouseRootsEventsProducer.Instance;
#else
				return MouseClassEventsProducer.Instance;
#endif
			}
		}

		private void AttachGlobalMouseEvents()
		{
			var mouseEvents = MouseEvents;

			mouseEvents.MouseLeftButtonDown += OnGlobalMouseLeftButtonDown;
			mouseEvents.MouseLeftButtonUp += OnGlobalMouseLeftButtonUp;

			mouseEvents.MouseRightButtonDown += OnGlobalMouseRightButtonDown;
			mouseEvents.MouseRightButtonUp += OnGlobalMouseRightButtonUp;

			mouseEvents.PreviewMouseLeftButtonDown += OnGlobalPreviewMouseLeftButtonDown;
			mouseEvents.PreviewMouseLeftButtonUp += OnGlobalPreviewMouseLeftButtonUp;

			mouseEvents.PreviewMouseRightButtonDown += OnGlobalPreviewMouseRightButtonDown;
			mouseEvents.PreviewMouseRightButtonUp += OnGlobalPreviewMouseRightButtonUp;
		}

		private void Close(bool immediate)
		{
			//CloseImpl(immediate == false);

			if (immediate)
				CloseImpl(false);
			else
				Dispatcher.BeginInvoke(() => CloseImpl(false));
		}

		private void CloseImpl(bool inputDispatcher)
		{
			_state = State.Closing;

			if (_onClose != null)
				_onClose();
			else
			{
				if (inputDispatcher)
				{
					_popup.CloseDispatcher();
					_popup.DisableOpenUntilNextLayoutUpdate();
				}
				else
					_popup.Close();
			}

			if (_popup.IsOpen && _state == State.Closing)
				_state = State.Open;
		}

		private void DetachGlobalMouseEvents()
		{
			var mouseEvents = MouseEvents;

			mouseEvents.MouseLeftButtonDown -= OnGlobalMouseLeftButtonDown;
			mouseEvents.MouseLeftButtonUp -= OnGlobalMouseLeftButtonUp;

			mouseEvents.MouseRightButtonDown -= OnGlobalMouseRightButtonDown;
			mouseEvents.MouseRightButtonUp -= OnGlobalMouseRightButtonUp;

			mouseEvents.PreviewMouseLeftButtonDown -= OnGlobalPreviewMouseLeftButtonDown;
			mouseEvents.PreviewMouseLeftButtonUp -= OnGlobalPreviewMouseLeftButtonUp;

			mouseEvents.PreviewMouseRightButtonDown -= OnGlobalPreviewMouseRightButtonDown;
			mouseEvents.PreviewMouseRightButtonUp -= OnGlobalPreviewMouseRightButtonUp;
		}

		private void EnterOpenState()
		{
			_state = State.Open;

			_hadKeyboardFocus = HasKeyboardFocus;

			FocusObserver.KeyboardFocusedElementChanged += OnKeyboardFocusedElementChanged;
			AttachGlobalMouseEvents();

#if !SILVERLIGHT
			_foregroundSession = _foregroundSession.DisposeExchange(ForegroundSession.Enter(_popup, QueryCloseCoreImmediate));
			HwndMouseObserver.AddListener(this);
#endif
		}

		private void ExitOpenState()
		{
			FocusObserver.KeyboardFocusedElementChanged -= OnKeyboardFocusedElementChanged;
			DetachGlobalMouseEvents();

#if !SILVERLIGHT
			_foregroundSession = _foregroundSession.DisposeExchange();
			HwndMouseObserver.RemoveListener(this);
#endif

			_leftButtonDownInfo = MouseButtonEventInfo.Empty;
			_rightButtonDownInfo = MouseButtonEventInfo.Empty;

			_state = State.Closed;
		}

		private MouseButtonEventInfo GetMouseDownInfo(MouseButtonEventArgsInt e)
		{
			var uie = PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource);

			return new MouseButtonEventInfo(uie, IsInsideClick(uie, e));
		}

		private bool IsInsideClick(UIElement uie, MouseButtonEventArgsInt e)
		{
			var isMouseEventSourceHitTestVisible = Popup.IsMouseEventSourceHitTestVisible(uie);
			var queryIsInsideClickEventArgs = new QueryCloseOnClickEventArgs(_popup, e.OriginalSource);

			QueryCloseOnClick?.Invoke(this, queryIsInsideClickEventArgs);

			if (isMouseEventSourceHitTestVisible == false)
				return false;

			if (queryIsInsideClickEventArgs.CanClose == false)
				return true;

			if (ReferenceEquals(uie, _popup.Panel))
				return false;

			var screenPosition = e.ScreenPosition;
			var isMouseInsideEventHelper = MouseInternal.IsMouseInsideEventHelper(_popup.TreeMode == PopupTreeMode.Visual ? (UIElement) _popup : _popup.Panel, uie, screenPosition);

			return isMouseInsideEventHelper;
		}

		protected virtual void OnClosing(PopupCancelEventArgs e)
		{
			Closing?.Invoke(this, e);
		}

		private void OnGlobalMouseLeftButtonDown(object sender, MouseButtonEventArgsInt e)
		{
			_leftButtonDownInfo = GetMouseDownInfo(e);

			QueryCloseOnDown(e);
		}

		private void OnGlobalMouseLeftButtonUp(object sender, MouseButtonEventArgsInt e)
		{
			if (_leftButtonDownInfo.IsEmpty == false)
				QueryCloseOnUp(_leftButtonDownInfo, GetMouseDownInfo(e), e);

			_leftButtonDownInfo = MouseButtonEventInfo.Empty;
		}

		private void OnGlobalMouseRightButtonDown(object sender, MouseButtonEventArgsInt e)
		{
			_rightButtonDownInfo = GetMouseDownInfo(e);

			QueryCloseOnDown(e);
		}

		private void OnGlobalMouseRightButtonUp(object sender, MouseButtonEventArgsInt e)
		{
			if (_rightButtonDownInfo.IsEmpty == false)
				QueryCloseOnUp(_rightButtonDownInfo, GetMouseDownInfo(e), e);

			_rightButtonDownInfo = MouseButtonEventInfo.Empty;
		}

		private void OnGlobalPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgsInt e)
		{
			_leftButtonDownInfo = GetMouseDownInfo(e);

			QueryCloseOnDown(e);
		}

		private void OnGlobalPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgsInt e)
		{
		}

		private void OnGlobalPreviewMouseRightButtonDown(object sender, MouseButtonEventArgsInt e)
		{
			_rightButtonDownInfo = GetMouseDownInfo(e);

			QueryCloseOnDown(e);
		}

		private void OnGlobalPreviewMouseRightButtonUp(object sender, MouseButtonEventArgsInt e)
		{
		}

		private void OnKeyboardFocusedElementChanged(object sender, EventArgs eventArgs)
		{
			if (_popup.Owner?.CloseOnLostKeyboardFocus != true)
				return;

			var hasKeyboardFocus = HasKeyboardFocus;

			if (_hadKeyboardFocus && hasKeyboardFocus == false)
				QueryCloseCoreImmediate();

			_hadKeyboardFocus |= hasKeyboardFocus;
		}

#if !SILVERLIGHT
		private void OnLocalMouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			_leftButtonDownInfo = GetMouseDownInfo(e.ToMouseButtonEventArgsInt(MouseButton.Left, MouseButtonState.Pressed));
		}
#endif

		private void OnLocalMouseLeftUp(object sender, MouseButtonEventArgs e)
		{
			if (_leftButtonDownInfo.IsEmpty == false)
			{
				var mouseButtonEventArgsInt = e.ToMouseButtonEventArgsInt(MouseButton.Left, MouseButtonState.Released);

				QueryCloseOnUp(_leftButtonDownInfo, GetMouseDownInfo(mouseButtonEventArgsInt), mouseButtonEventArgsInt);
			}

			_leftButtonDownInfo = MouseButtonEventInfo.Empty;
		}

#if !SILVERLIGHT
		private void OnLocalMouseRightDown(object sender, MouseButtonEventArgs e)
		{
			_rightButtonDownInfo = GetMouseDownInfo(e.ToMouseButtonEventArgsInt(MouseButton.Right, MouseButtonState.Pressed));
		}
#endif

		private void OnLocalMouseRightUp(object sender, MouseButtonEventArgs e)
		{
			if (_rightButtonDownInfo.IsEmpty == false)
			{
				var mouseButtonEventArgsInt = e.ToMouseButtonEventArgsInt(MouseButton.Right, MouseButtonState.Released);

				QueryCloseOnUp(_rightButtonDownInfo, GetMouseDownInfo(mouseButtonEventArgsInt), mouseButtonEventArgsInt);
			}

			_rightButtonDownInfo = MouseButtonEventInfo.Empty;
		}

		private void OnPopupChildChanged(UIElement oldChild, UIElement newChild)
		{
			if (oldChild != null)
			{
				oldChild.RemoveHandler(UIElement.MouseLeftButtonUpEvent, (MouseButtonEventHandler) OnLocalMouseLeftUp);
				oldChild.RemoveHandler(UIElement.MouseRightButtonUpEvent, (MouseButtonEventHandler) OnLocalMouseRightUp);

#if !SILVERLIGHT
				oldChild.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, (MouseButtonEventHandler) OnLocalMouseLeftDown);
				oldChild.RemoveHandler(UIElement.PreviewMouseRightButtonDownEvent, (MouseButtonEventHandler) OnLocalMouseRightDown);
#endif
			}

			if (newChild != null)
			{
				newChild.AddHandler(UIElement.MouseLeftButtonUpEvent, (MouseButtonEventHandler) OnLocalMouseLeftUp, true);
				newChild.AddHandler(UIElement.MouseRightButtonUpEvent, (MouseButtonEventHandler) OnLocalMouseRightUp, true);

#if !SILVERLIGHT
				newChild.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (MouseButtonEventHandler) OnLocalMouseLeftDown, true);
				newChild.AddHandler(UIElement.PreviewMouseRightButtonDownEvent, (MouseButtonEventHandler) OnLocalMouseRightDown, true);
#endif
			}
		}

		private void PopupControllerOnClosed(object sender, EventArgs e)
		{
			ExitOpenState();
		}

		private void PopupControllerOnOpened(object sender, EventArgs eventArgs)
		{
			Dispatcher.BeginInvoke(EnterOpenState);
		}

		private void QueryCloseCore(PopupCloseReason closeReason, bool immediate = false)
		{
			if (_popup.StaysOpen)
				return;

			if (_state != State.Open)
				return;

			var args = new PopupCancelEventArgs(closeReason, false);

			OnClosing(args);

			if (args.Cancel)
				return;

			Close(immediate);
		}

		private void QueryCloseCoreImmediate()
		{
			QueryCloseCore(ExplicitCloseReason.Instance, true);
		}

		private void QueryCloseOnDown(MouseButtonEventArgsInt e)
		{
			var uieSource = PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource);
			var inside = IsInsideClick(uieSource, e);

			if (inside)
				return;

			QueryCloseCore(new MouseButtonEventPopupCloseReason(e.OriginalArgs));
		}

		private void QueryCloseOnUp(MouseButtonEventInfo downButtonEventInfo, MouseButtonEventInfo upEventInfo, MouseButtonEventArgsInt e)
		{
			var inside = upEventInfo.IsInside;

			if (inside == false)
			{
				if (downButtonEventInfo.IsInside == false)
					QueryCloseCore(new MouseButtonEventPopupCloseReason(e.OriginalArgs));
			}
		}

		public void Dispose()
		{
			DetachGlobalMouseEvents();

			ClearValue(PopupChildProperty);
		}

		void IMouseEventListener.OnMouseEvent(MouseEventInfo eventInfo)
		{
			if (eventInfo.EventKind == MouseEventKind.Button && eventInfo.AreaKind == MouseEventAreaKind.NonClient)
				QueryCloseCoreImmediate();
		}

		private enum State
		{
			Open,
			Closing,
			Closed
		}

		private readonly struct MouseButtonEventInfo
		{
			public static readonly MouseButtonEventInfo Empty = new MouseButtonEventInfo();

			#region Fields

			public readonly UIElement Element;
			public readonly bool IsInside;

			#endregion

			#region Ctors

			public MouseButtonEventInfo(UIElement element, bool isInside)
			{
				Element = element;
				IsInside = isInside;
			}

			#endregion

			public bool IsEmpty => Element == null;
		}
	}
}