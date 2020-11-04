// <copyright file="PopupTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public abstract class PopupTrigger : InheritanceContextObject
	{
		public static readonly DependencyProperty OpenDelayProperty = DPM.Register<TimeSpan, PopupTrigger>
			("OpenDelay", TimeSpan.Zero, d => d.OnOpenDelayPropertyChangedPrivate);

		public static readonly DependencyProperty CloseDelayProperty = DPM.Register<TimeSpan, PopupTrigger>
			("CloseDelay", TimeSpan.Zero, d => d.OnCloseDelayPropertyChangedPrivate);

		private static readonly DependencyPropertyKey PopupPropertyKey = DPM.RegisterReadOnly<Popup, PopupTrigger>
			("Popup", default, d => d.OnPopupPropertyChangedPrivate);

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
			get => (TimeSpan) GetValue(CloseDelayProperty);
			set => SetValue(CloseDelayProperty, value);
		}

		public TimeSpan OpenDelay
		{
			get => (TimeSpan) GetValue(OpenDelayProperty);
			set => SetValue(OpenDelayProperty, value);
		}

		public Popup Popup
		{
			get => (Popup) GetValue(PopupProperty);
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

	[ContentProperty(nameof(Triggers))]
	public sealed class CompositePopupTrigger : PopupTrigger
	{
		private static readonly DependencyPropertyKey TriggersPropertyKey = DPM.RegisterReadOnly<PopupTriggerCollection, CompositePopupTrigger>
			("TriggersInternal");

		public static readonly DependencyProperty TriggersProperty = TriggersPropertyKey.DependencyProperty;

		public PopupTriggerCollection Triggers => this.GetValueOrCreate(TriggersPropertyKey, () => new PopupTriggerCollection(this));

		protected override void OnPopupChanged(Popup oldValue, Popup newValue)
		{
			base.OnPopupChanged(oldValue, newValue);

			foreach (var trigger in Triggers)
				trigger.Popup = newValue;
		}

		internal void UpdateIsOpen()
		{
			if (Triggers.Any(t => t.ActualIsOpen))
				Open();
			else
				Close();
		}
	}

	public sealed class PopupTriggerCollection : DependencyObjectCollectionBase<PopupTrigger>
	{
		internal PopupTriggerCollection(CompositePopupTrigger compositeTrigger)
		{
			CompositeTrigger = compositeTrigger;
		}

		private CompositePopupTrigger CompositeTrigger { get; }

		protected override void OnItemAdded(PopupTrigger trigger)
		{
			base.OnItemAdded(trigger);

			trigger.ActualIsOpenChanged += OnTriggerIsOpenChanged;
		}

		protected override void OnItemRemoved(PopupTrigger trigger)
		{
			base.OnItemRemoved(trigger);

			trigger.ActualIsOpenChanged -= OnTriggerIsOpenChanged;
		}

		private void OnTriggerIsOpenChanged(object sender, EventArgs e)
		{
			CompositeTrigger.UpdateIsOpen();
		}
	}

	public abstract class SourcePopupTrigger : PopupTrigger
	{
		public static readonly DependencyProperty SourceProperty = DPM.Register<FrameworkElement, SourcePopupTrigger>
			("Source", default, d => d.OnTargetPropertyChangedPrivate);

		public FrameworkElement Source
		{
			get => (FrameworkElement) GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		protected abstract void AttachTarget(FrameworkElement source);

		protected abstract void DetachTarget(FrameworkElement source);

		private void OnTargetPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (oldValue != null)
				DetachTarget(oldValue);

			if (newValue != null)
				AttachTarget(newValue);
		}
	}

	public sealed class MouseOverPopupTrigger : SourcePopupTrigger
	{
		protected override void AttachTarget(FrameworkElement source)
		{
			source.MouseEnter += OnSourceMouseEnter;
			source.MouseLeave += OnSourceMouseLeave;
		}

		protected override void DetachTarget(FrameworkElement source)
		{
			source.MouseEnter -= OnSourceMouseEnter;
			source.MouseLeave -= OnSourceMouseLeave;
		}

		protected override void OnPopupChanged(Popup oldValue, Popup newValue)
		{
			base.OnPopupChanged(oldValue, newValue);

			if (oldValue != null)
			{
				oldValue.Panel.MouseEnter -= OnPopupPanelMouseEnter;
				oldValue.Panel.MouseLeave -= OnPopupPanelMouseLeave;
			}

			if (newValue != null)
			{
				newValue.Panel.MouseEnter += OnPopupPanelMouseEnter;
				newValue.Panel.MouseLeave += OnPopupPanelMouseLeave;
			}
		}

		private void OnPopupPanelMouseEnter(object sender, MouseEventArgs e)
		{
			if (this.HasLocalValue(SourceProperty) == false)
				Open();
		}

		private void OnPopupPanelMouseLeave(object sender, MouseEventArgs e)
		{
			if (this.HasLocalValue(SourceProperty) == false)
				Close();
		}

		private void OnSourceMouseEnter(object sender, MouseEventArgs e)
		{
			Open();
		}

		private void OnSourceMouseLeave(object sender, MouseEventArgs e)
		{
			Close();
		}
	}

	public sealed class FocusPopupTrigger : SourcePopupTrigger
	{
		protected override void AttachTarget(FrameworkElement source)
		{
			source.GotKeyboardFocus += OnSourceGotFocus;
			source.LostKeyboardFocus += OnSourceLostFocus;
		}

		protected override void DetachTarget(FrameworkElement source)
		{
			source.GotKeyboardFocus -= OnSourceGotFocus;
			source.LostKeyboardFocus -= OnSourceLostFocus;
		}

		private void OnSourceGotFocus(object sender, RoutedEventArgs e)
		{
			Open();
		}

		private void OnSourceLostFocus(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}

	public sealed class ExplicitTrigger : PopupTrigger
	{
		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, ExplicitTrigger>
			("IsOpen", default, d => d.OnIsOpenPropertyChangedPrivate);

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		private void OnIsOpenPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (newValue)
				Open();
			else
				Close();
		}
	}
}