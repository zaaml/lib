// <copyright file="ScreenBoundsBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
	public class ScreenBoundsBehavior : BehaviorBase
	{
		public static readonly DependencyProperty ActualScreenBoxProperty = DPM.Register<Rect, ScreenBoundsBehavior>
			("ActualScreenBox");

		public static readonly DependencyProperty ActualScreenLocationProperty = DPM.Register<Point, ScreenBoundsBehavior>
			("ActualScreenLocation");

		public Rect ActualScreenBox
		{
			get => (Rect)GetValue(ActualScreenBoxProperty);
			set => SetValue(ActualScreenBoxProperty, value);
		}

		public Point ActualScreenLocation
		{
			get => (Point)GetValue(ActualScreenLocationProperty);
			set => SetValue(ActualScreenLocationProperty, value);
		}

		private void FrameworkElementOnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			UpdateSize();
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			FrameworkElement.LayoutUpdated += FrameworkElementOnLayoutUpdated;

			UpdateSize();
		}

		protected override void OnDetaching()
		{
			FrameworkElement.LayoutUpdated -= FrameworkElementOnLayoutUpdated;

			base.OnDetaching();
		}

		private void UpdateSize()
		{
			var screenBox = FrameworkElement.GetScreenLogicalBox();

			ActualScreenLocation = screenBox.GetTopLeft();
			ActualScreenBox = screenBox;
		}
	}
}