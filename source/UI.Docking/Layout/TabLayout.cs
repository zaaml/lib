// <copyright file="TabLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class TabLayout : TabLayoutBase
	{
		public static readonly DependencyProperty OrderProperty = DPM.RegisterAttached<int, TabLayout>
			("Order", 0, OnOrderPropertyChanged);

		private static readonly List<DependencyProperty> TabLayoutProperties = new()
		{
			OrderProperty
		};

		static TabLayout()
		{
			RegisterLayoutProperties<TabLayout>(TabLayoutProperties);
		}

		public override LayoutKind LayoutKind => LayoutKind.Tab;

		public static int GetOrder(DependencyObject depObj)
		{
			return (int)depObj.GetValue(OrderProperty);
		}

		protected override int GetDockItemOrder(DockItem dockItem)
		{
			return GetOrder(dockItem);
		}

		private void OnItemOrderChanged(DockItem dockItem)
		{
		}

		private static void OnOrderChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var tabLayout = dockItem.ActualLayout as TabLayout;

			tabLayout?.OnItemOrderChanged(dockItem);
		}

		private static void OnOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnOrderChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		public static void SetOrder(DependencyObject depObj, int orderIndex)
		{
			depObj.SetValue(OrderProperty, orderIndex);
		}
	}
}