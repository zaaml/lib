// <copyright file="HoverableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
	public static class HoverableBehavior
	{
		private static readonly Dictionary<UIElement, HoverTimer> HoverTimers = new();

		public static readonly DependencyProperty IsEnabledProperty = DependencyPropertyManager.RegisterAttached
		("IsEnabled", typeof(bool), typeof(HoverableBehavior),
			new PropertyMetadata(BooleanBoxes.False, OnIsEnabledPropertyChanged));

		public static readonly DependencyProperty EnterDelayProperty = DependencyPropertyManager.RegisterAttached
		("EnterDelay", typeof(double), typeof(HoverableBehavior),
			new PropertyMetadata(0.0));

		public static readonly DependencyProperty LeaveDelayProperty = DependencyPropertyManager.RegisterAttached
		("LeaveDelay", typeof(double), typeof(HoverableBehavior),
			new PropertyMetadata(0.0));

		public static readonly DependencyProperty IsOpenProperty = DependencyPropertyManager.RegisterAttached
		("IsOpen", typeof(bool), typeof(HoverableBehavior),
			new PropertyMetadata(BooleanBoxes.False));

		public static readonly DependencyProperty PopupTargetProperty = DependencyPropertyManager.RegisterAttached
		("PopupTarget", typeof(Popup), typeof(HoverableBehavior),
			new PropertyMetadata(null));

		public static readonly DependencyProperty FrozenProperty = DependencyPropertyManager.RegisterAttached
		("Frozen", typeof(bool), typeof(HoverableBehavior),
			new PropertyMetadata(BooleanBoxes.False, OnFrozenPropertyChanged));

		private static readonly DependencyProperty IsMouseOverProperty = DependencyPropertyManager.RegisterAttached
		("IsMouseOver", typeof(bool), typeof(HoverableBehavior),
			new PropertyMetadata(BooleanBoxes.False));

		private static void Deinitialize(UIElement uie)
		{
			uie.MouseEnter -= OnElementMouseEnter;
			uie.MouseLeave -= OnElementMouseLeave;

			var timer = HoverTimers[uie];
			timer.Stop();
			timer.Tick -= OnTimerTick;

			HoverTimers.Remove(uie);
		}

		public static double GetEnterDelay(UIElement element)
		{
			return (double)element.GetValue(EnterDelayProperty);
		}

		public static bool GetFrozen(UIElement element)
		{
			return (bool)element.GetValue(FrozenProperty);
		}

		public static bool GetIsEnabled(UIElement element)
		{
			return (bool)element.GetValue(IsEnabledProperty);
		}

		public static bool GetIsOpen(UIElement element)
		{
			return (bool)element.GetValue(IsOpenProperty);
		}

		public static double GetLeaveDelay(UIElement element)
		{
			return (double)element.GetValue(LeaveDelayProperty);
		}

		public static Popup GetPopupTarget(UIElement element)
		{
			return (Popup)element.GetValue(PopupTargetProperty);
		}

		private static void Initialize(UIElement uie)
		{
			uie.MouseEnter += OnElementMouseEnter;
			uie.MouseLeave += OnElementMouseLeave;

			var timer = new HoverTimer(uie);
			timer.Tick += OnTimerTick;

			HoverTimers[uie] = timer;
		}

		private static void OnElementMouseEnter(object sender, MouseEventArgs mouseEventArgs)
		{
			var uie = (UIElement)sender;
			uie.SetValue(IsMouseOverProperty, true);

			var timer = HoverTimers[uie];
			timer.Stop();

			if (GetIsOpen(uie))
				return;

			timer.Interval = TimeSpan.FromMilliseconds(GetEnterDelay(uie));
			timer.Start();
		}

		private static void OnElementMouseLeave(object sender, MouseEventArgs e)
		{
			var uie = (UIElement)sender;
			uie.SetValue(IsMouseOverProperty, false);

			var timer = HoverTimers[uie];
			timer.Stop();

			if (GetIsOpen(uie) == false)
				return;

			timer.Interval = TimeSpan.FromMilliseconds(GetLeaveDelay(uie));
			timer.Start();
		}

		private static void OnFrozenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uie = (UIElement)d;

			if ((bool)uie.GetValue(IsMouseOverProperty))
				return;

			HoverTimer timer;

			if (HoverTimers.TryGetValue(uie, out timer))
			{
				timer.Stop();
				timer.Start();
			}
		}

		private static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uie = (UIElement)d;

			if ((bool)e.OldValue)
				Deinitialize(uie);
			else
				Initialize(uie);
		}

		private static void OnTimerTick(object sender, EventArgs e)
		{
			var timer = (HoverTimer)sender;
			timer.Stop();

			var uie = timer.HoverSource;

			if ((bool)uie.GetValue(IsMouseOverProperty) == false)
			{
				if (GetFrozen(uie))
					return;

				SetIsOpen(uie, false);

				var popup = GetPopupTarget(uie);
				if (popup != null)
					popup.IsOpen = false;
			}
			else
			{
				SetIsOpen(uie, true);

				var popup = GetPopupTarget(uie);
				if (popup != null)
					popup.IsOpen = true;
			}
		}

		public static void SetEnterDelay(UIElement element, double value)
		{
			element.SetValue(EnterDelayProperty, value);
		}

		public static void SetFrozen(UIElement element, bool value)
		{
			element.SetValue(FrozenProperty, value.Box());
		}

		public static void SetIsEnabled(UIElement element, bool value)
		{
			element.SetValue(IsEnabledProperty, value.Box());
		}

		public static void SetIsOpen(UIElement element, bool value)
		{
			element.SetValue(IsOpenProperty, value.Box());
		}

		public static void SetLeaveDelay(UIElement element, double value)
		{
			element.SetValue(LeaveDelayProperty, value);
		}

		public static void SetPopupTarget(UIElement element, Popup value)
		{
			element.SetValue(PopupTargetProperty, value);
		}

		internal class HoverTimer : DispatcherTimer
		{
			public HoverTimer(UIElement hoverSource)
			{
				HoverSource = hoverSource;
			}

			public UIElement HoverSource { get; }
		}
	}
}