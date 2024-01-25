// <copyright file="PopupTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public abstract class PopupTrigger : InheritanceContextObject
	{
		public static readonly DependencyProperty OpenDelayProperty = DPM.Register<TimeSpan, PopupTrigger>
			("OpenDelay", TimeSpan.Zero, d => d.OnOpenDelayPropertyChangedPrivate);

		public static readonly DependencyProperty CloseDelayProperty = DPM.Register<TimeSpan, PopupTrigger>
			("CloseDelay", TimeSpan.Zero, d => d.OnCloseDelayPropertyChangedPrivate);

		private static readonly DependencyPropertyKey PopupPropertyKey = DPM.RegisterReadOnly<Popup, PopupTrigger>
			("Popup", d => d.OnPopupPropertyChangedPrivate);

		public static readonly DependencyProperty PopupProperty = PopupPropertyKey.DependencyProperty;

		private readonly DelayAction _closeAction;
		private readonly DelayAction _openAction;
		private bool _actualIsOpen;

		public event EventHandler ActualIsOpenChanged;

		protected PopupTrigger()
		{
			_openAction = new DelayAction(OpenCore);
			_closeAction = new DelayAction(CloseCore);
		}

		public bool ActualIsOpen
		{
			get => _actualIsOpen;
			private set
			{
				if (_actualIsOpen == value)
					return;

				_actualIsOpen = value;

				ActualIsOpenChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public TimeSpan CloseDelay
		{
			get => (TimeSpan)GetValue(CloseDelayProperty);
			set => SetValue(CloseDelayProperty, value);
		}

		public TimeSpan OpenDelay
		{
			get => (TimeSpan)GetValue(OpenDelayProperty);
			set => SetValue(OpenDelayProperty, value);
		}

		public Popup Popup
		{
			get => (Popup)GetValue(PopupProperty);
			internal set => this.SetReadOnlyValue(PopupPropertyKey, value);
		}

		protected void Close()
		{
			_openAction.Revoke();
			_closeAction.Invoke();
		}

		private void CloseCore()
		{
			ActualIsOpen = false;
		}

		private void OnCloseDelayPropertyChangedPrivate(TimeSpan oldValue, TimeSpan newValue)
		{
			_closeAction.Delay = newValue;
		}

		private void OnOpenDelayPropertyChangedPrivate(TimeSpan oldValue, TimeSpan newValue)
		{
			_openAction.Delay = newValue;
		}

		protected virtual void OnPopupChanged(Popup oldValue, Popup newValue)
		{
		}

		private void OnPopupPropertyChangedPrivate(Popup oldValue, Popup newValue)
		{
			OnPopupChanged(oldValue, newValue);
		}

		protected void Open()
		{
			_closeAction.Revoke();
			_openAction.Invoke();
		}

		private void OpenCore()
		{
			ActualIsOpen = true;
		}
	}
}