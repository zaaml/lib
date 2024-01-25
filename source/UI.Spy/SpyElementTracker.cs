// <copyright file="SpyElementTracker.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Spy
{
	public abstract class SpyElementTracker : InheritanceContextObject, IDisposable
	{
		private static readonly TimeSpan DefaultThrottleDelay = TimeSpan.FromMilliseconds(50);

		public static readonly DependencyProperty TriggerProperty = DPM.Register<SpyTrigger, SpyElementTracker>
			("Trigger", d => d.OnTriggerPropertyChangedPrivate);

		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyElementTracker>
			("Element", d => d.OnElementPropertyChangedPrivate);

		public static readonly DependencyProperty ThrottleProperty = DPM.Register<TimeSpan, SpyElementTracker>
			("Throttle", DefaultThrottleDelay, d => d.OnThrottlePropertyChangedPrivate);

		private static readonly DependencyPropertyKey IsTrackingPropertyKey = DPM.RegisterReadOnly<bool, SpyElementTracker>
			("IsTracking", d => d.OnIsTrackingPropertyChangedPrivate);

		public static readonly DependencyProperty IsTrackingProperty = IsTrackingPropertyKey.DependencyProperty;

		private readonly DelayAction<UIElement> _delayAction;
		private UIElement _elementCore;

		public event EventHandler ElementChanged;

		protected SpyElementTracker()
		{
			_delayAction = new DelayAction<UIElement>(PushElement, DefaultThrottleDelay);
		}

		public UIElement Element
		{
			get => (UIElement)GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		protected UIElement ElementCore
		{
			get => _elementCore;
			set
			{
				if (ReferenceEquals(_elementCore, value))
					return;

				_elementCore = value;
				_delayAction.Invoke(Element);

				SpyElementAdorner.Element = value;
			}
		}

		public bool IsTracking
		{
			get => (bool)GetValue(IsTrackingProperty);
			private set => this.SetReadOnlyValue(IsTrackingPropertyKey, value);
		}

		private SpyElementAdorner SpyElementAdorner { get; } = new();

		public TimeSpan Throttle
		{
			get => (TimeSpan)GetValue(ThrottleProperty);
			set => SetValue(ThrottleProperty, value);
		}

		public SpyTrigger Trigger
		{
			get => (SpyTrigger)GetValue(TriggerProperty);
			set => SetValue(TriggerProperty, value);
		}

		private void BeginTrack()
		{
			BeginTrackCore();
		}

		protected abstract void BeginTrackCore();

		private void EndTrack()
		{
			EndTrackCore();

			_delayAction.ForceDelayComplete();
		}

		protected abstract void EndTrackCore();

		private void OnElementPropertyChangedPrivate(UIElement oldValue, UIElement newValue)
		{
			_elementCore = newValue;

			SpyElementAdorner.Element = newValue;

			ElementChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsTrackingPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (newValue)
				BeginTrack();
			else
				EndTrack();
		}

		private void OnThrottlePropertyChangedPrivate(TimeSpan oldValue, TimeSpan newValue)
		{
			_delayAction.Delay = newValue;
		}

		private void OnTriggerIsOpenChanged(object sender, EventArgs e)
		{
			UpdateTracking();
		}

		private void UpdateTracking()
		{
			IsTracking = Trigger?.IsOpen ?? false;
		}

		private void OnTriggerPropertyChangedPrivate(SpyTrigger oldValue, SpyTrigger newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.IsOpenChanged -= OnTriggerIsOpenChanged;

			if (newValue != null)
				newValue.IsOpenChanged += OnTriggerIsOpenChanged;

			UpdateTracking();
		}

		private void PushElement(UIElement element)
		{
			Element = ElementCore;
		}

		public void Dispose()
		{
			Trigger = null;
			Element = null;
		}
	}
}