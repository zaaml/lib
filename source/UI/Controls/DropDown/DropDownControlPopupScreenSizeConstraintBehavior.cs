// <copyright file="DropDownControlPopupScreenSizeConstraintBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	public sealed class DropDownControlPopupScreenSizeConstraintBehavior : Behavior<DropDownControlBase>
	{
		public static readonly DependencyProperty MaxWidthRatioProperty = DPM.Register<double, DropDownControlPopupScreenSizeConstraintBehavior>
			("MaxWidthRatio", 1.0, d => d.OnMaxWidthRatioPropertyChangedPrivate);

		public static readonly DependencyProperty MinWidthRatioProperty = DPM.Register<double, DropDownControlPopupScreenSizeConstraintBehavior>
			("MinWidthRatio", 0.0, d => d.OnMinWidthRatioPropertyChangedPrivate);

		public static readonly DependencyProperty MaxHeightRatioProperty = DPM.Register<double, DropDownControlPopupScreenSizeConstraintBehavior>
			("MaxHeightRatio", 1.0, d => d.OnMaxHeightRatioPropertyChangedPrivate);

		public static readonly DependencyProperty MinHeightRatioProperty = DPM.Register<double, DropDownControlPopupScreenSizeConstraintBehavior>
			("MinHeightRatio", 0.0, d => d.OnMinHeightRatioPropertyChangedPrivate);

		private Screen _screen;

		public double MaxHeightRatio
		{
			get => (double) GetValue(MaxHeightRatioProperty);
			set => SetValue(MaxHeightRatioProperty, value);
		}

		public double MaxWidthRatio
		{
			get => (double) GetValue(MaxWidthRatioProperty);
			set => SetValue(MaxWidthRatioProperty, value);
		}

		public double MinHeightRatio
		{
			get => (double) GetValue(MinHeightRatioProperty);
			set => SetValue(MinHeightRatioProperty, value);
		}

		public double MinWidthRatio
		{
			get => (double) GetValue(MinWidthRatioProperty);
			set => SetValue(MinWidthRatioProperty, value);
		}

		private Screen Screen
		{
			get => _screen;
			set
			{
				if (ReferenceEquals(_screen, value))
					return;

				_screen = value;

				UpdateControlSize();
			}
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			Target.LayoutUpdated += OnTargetLayoutUpdated;

			UpdateScreen();
		}

		protected override void OnDetaching()
		{
			Target.LayoutUpdated -= OnTargetLayoutUpdated;

			Screen = null;

			base.OnDetaching();
		}

		private void OnMaxHeightRatioPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateControlSize(PopupControlBase.PopupMaxHeightProperty, MaxHeightRatioProperty, Orientation.Vertical);
		}

		private void OnMaxWidthRatioPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateControlSize(PopupControlBase.PopupMaxWidthProperty, MaxWidthRatioProperty, Orientation.Horizontal);
		}

		private void OnMinHeightRatioPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateControlSize(PopupControlBase.PopupMinHeightProperty, MinHeightRatioProperty, Orientation.Vertical);
		}

		private void OnMinWidthRatioPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateControlSize(PopupControlBase.PopupMinWidthProperty, MinWidthRatioProperty, Orientation.Horizontal);
		}

		private void OnTargetLayoutUpdated(object sender, EventArgs e)
		{
			UpdateScreen();
		}

		private void UpdateControlSize()
		{
			UpdateControlSize(DropDownControlBase.PopupMaxHeightProperty, MaxHeightRatioProperty, Orientation.Vertical);
			UpdateControlSize(DropDownControlBase.PopupMaxWidthProperty, MaxWidthRatioProperty, Orientation.Horizontal);
			UpdateControlSize(DropDownControlBase.PopupMinHeightProperty, MinHeightRatioProperty, Orientation.Vertical);
			UpdateControlSize(DropDownControlBase.PopupMinWidthProperty, MinWidthRatioProperty, Orientation.Horizontal);
		}

		private void UpdateControlSize(DependencyProperty controlSizeProperty, DependencyProperty behaviorSizeProperty, Orientation orientation)
		{
			if (Target == null || Screen == null)
				return;

			if (this.IsDefaultValue(behaviorSizeProperty))
				return;

			var ratio = (double) GetValue(behaviorSizeProperty);
			var size = ratio * Screen.Bounds.Size.AsOriented(orientation).Direct;

			Target.SetCurrentValueInternal(controlSizeProperty, size);
		}

		private void UpdateScreen()
		{
			Screen = Screen.FromElement(Target);
		}
	}
}