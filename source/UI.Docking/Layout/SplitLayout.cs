// <copyright file="SplitLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class SplitLayout : BaseLayout
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, SplitLayout>
			("Orientation", Orientation.Horizontal, l => l.OnOrientationChanged);

		public static readonly DependencyProperty WidthProperty = DPM.RegisterAttached<double, SplitLayout>
			("Width", 200, OnWidthChanged);

		public static readonly DependencyProperty HeightProperty = DPM.RegisterAttached<double, SplitLayout>
			("Height", 200, OnHeightChanged);

		public static readonly DependencyProperty OrderProperty = DPM.RegisterAttached<int, SplitLayout>
			("Order", 0, OnOrderPropertyChanged);

		private static readonly List<DependencyProperty> SplitLayoutProperties = new()
		{
			WidthProperty,
			HeightProperty,
			OrderProperty
		};

		static SplitLayout()
		{
			RegisterLayoutProperties<SplitLayout>(SplitLayoutProperties);
		}

		public override LayoutKind LayoutKind => LayoutKind.Split;

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public static double GetHeight(DependencyObject depObj)
		{
			return (double)depObj.GetValue(HeightProperty);
		}

		public static int GetOrder(DependencyObject depObj)
		{
			return (int)depObj.GetValue(OrderProperty);
		}

		protected override int GetDockItemOrder(DockItem dockItem)
		{
			return GetOrder(dockItem);
		}

		public static double GetWidth(DependencyObject depObj)
		{
			return (double)depObj.GetValue(WidthProperty);
		}

		private static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(dependencyObject, e);
		}

		private void OnItemOrderChanged(DockItem dockItem)
		{
		}

		private static void OnOrderChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var splitLayout = dockItem.ActualLayout as SplitLayout;

			splitLayout?.OnItemOrderChanged(dockItem);
		}

		private static void OnOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnOrderChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		private void OnOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
		{
		}

		private static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(dependencyObject, e);
		}

		public static void SetHeight(DependencyObject depObj, double value)
		{
			depObj.SetValue(HeightProperty, value);
		}

		public static void SetOrder(DependencyObject depObj, int orderIndex)
		{
			depObj.SetValue(OrderProperty, orderIndex);
		}

		public static void SetWidth(DependencyObject depObj, double value)
		{
			depObj.SetValue(WidthProperty, value);
		}
	}
}