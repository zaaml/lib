// <copyright file="WindowLayerPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Windows
{
	internal class WindowLayerPanel : Panel
	{
		public static readonly DependencyProperty LeftProperty = DPM.RegisterAttached<double, WindowLayerPanel>
			("Left", DPM.StaticCallback<WindowBase>(OnLeftPropertyChanged));

		public static readonly DependencyProperty TopProperty = DPM.RegisterAttached<double, WindowLayerPanel>
			("Top", DPM.StaticCallback<WindowBase>(OnTopPropertyChanged));

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			foreach (var child in Children.Cast<FrameworkElement>())
				child.Arrange(GetChildRect(child, child.DesiredSize));

			return finalSize;
		}

		private Rect GetChildRect(FrameworkElement child, Size desiredSize)
		{
			return new Rect(new Point(GetLeft(child), GetTop(child)), desiredSize);
		}

		public static double GetLeft(DependencyObject element)
		{
			return (double)element.GetValue(LeftProperty);
		}

		public static double GetTop(DependencyObject element)
		{
			return (double)element.GetValue(TopProperty);
		}

		private static void InvalidatePanelArrange(WindowBase window)
		{
			var panel = window.Parent as WindowLayerPanel;
			panel?.InvalidateArrange();
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var finalRect = new Rect();

			foreach (var child in Children.Cast<FrameworkElement>())
			{
				child.Measure(XamlConstants.InfiniteSize);
				var childRect = GetChildRect(child, child.DesiredSize);
				if (childRect.Right < 0 || childRect.Bottom < 0)
					continue;

				if (childRect.Left < 0)
				{
					childRect.Width += childRect.Left;
					childRect.X = 0;
				}

				if (childRect.Top < 0)
				{
					childRect.Height += childRect.Top;
					childRect.Y = 0;
				}

				finalRect.Width = Math.Max(finalRect.Width, childRect.Right);
				finalRect.Height = Math.Max(finalRect.Height, childRect.Bottom);
			}

			return finalRect.Size();
		}

		private static void OnLeftPropertyChanged(WindowBase window)
		{
			InvalidatePanelArrange(window);
		}

		private static void OnTopPropertyChanged(WindowBase window)
		{
			InvalidatePanelArrange(window);
		}

		public static void SetLeft(DependencyObject element, double value)
		{
			element.SetValue(LeftProperty, value);
		}

		public static void SetTop(DependencyObject element, double value)
		{
			element.SetValue(TopProperty, value);
		}
	}
}