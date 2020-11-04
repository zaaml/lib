// <copyright file="MouseBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
	public class MouseBehavior : BehaviorBase
	{
		public static readonly DependencyProperty IsMouseOverProperty = DPM.Register<bool, MouseBehavior>
			("IsMouseOver", m => m.OnIsMouseOverChanged);

		public bool IsMouseOver
		{
			get => (bool) GetValue(IsMouseOverProperty);
			set => SetValue(IsMouseOverProperty, value);
		}

		private void MouseServiceOnMouseMove(object sender, MouseEventArgsInt mouseEventArgsInt)
		{
			IsMouseOver = FrameworkElement.GetIsMouseOver();
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			FrameworkElement.MouseEnter += OnMouseEnter;
			FrameworkElement.MouseLeave += OnMouseLeave;
			FrameworkElement.Unloaded += OnUnloaded;

			FrameworkElement.AddValueChanged(UIElement.VisibilityProperty, OnVisibilityPropertyChanged);
		}

		protected override void OnDetaching()
		{
			FrameworkElement.MouseEnter -= OnMouseEnter;
			FrameworkElement.MouseLeave -= OnMouseLeave;
			FrameworkElement.Unloaded -= OnUnloaded;

			FrameworkElement.RemoveValueChanged(UIElement.VisibilityProperty, OnVisibilityPropertyChanged);

			base.OnDetaching();
		}

		private void OnIsMouseOverChanged()
		{
			if (IsMouseOver)
				MouseInternal.MouseMove += MouseServiceOnMouseMove;
			else
				MouseInternal.MouseMove -= MouseServiceOnMouseMove;
		}

		private void OnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
		{
			IsMouseOver = true;
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			IsMouseOver = false;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			IsMouseOver = false;
		}

		private void OnVisibilityPropertyChanged(object sender, PropertyValueChangedEventArgs eventArgs)
		{
			if (FrameworkElement.Visibility == Visibility.Collapsed)
				IsMouseOver = false;
			else
				FrameworkElement.InvokeOnLayoutUpdate(f => IsMouseOver = f.GetIsMouseOver());
		}
	}
}