// <copyright file="DockLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class DockLayout : BaseLayout
	{
		public static readonly DependencyProperty DockProperty = DPM.RegisterAttached<Dock, DockLayout>
			("Dock", Dock.Left, OnDockChanged);

		public static readonly DependencyProperty WidthProperty = DPM.RegisterAttached<double, DockLayout>
			("Width", 200.0, OnWidthPropertyChanged);

		public static readonly DependencyProperty HeightProperty = DPM.RegisterAttached<double, DockLayout>
			("Height", 200.0, OnHeightPropertyChanged);

		public static readonly DependencyProperty OrderProperty = DPM.RegisterAttached<int, DockLayout>
			("Order", 0, OnOrderPropertyChanged);

		private static void OnOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnOrderChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnOrderChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var dockLayout = dockItem.ActualLayout as DockLayout;

			dockLayout?.OnItemOrderChanged(dockItem);
		}

		private void OnItemOrderChanged(DockItem dockItem)
		{
		}

		public static int GetOrder(DependencyObject depObj)
		{
			return (int)depObj.GetValue(OrderProperty);
		}

		public static void SetOrder(DependencyObject depObj, int orderIndex)
		{
			depObj.SetValue(OrderProperty, orderIndex);
		}

		private static readonly List<DependencyProperty> DockLayoutProperties = new()
		{
			DockProperty,
			WidthProperty,
			HeightProperty,
			OrderProperty
		};

		private static readonly List<DependencyProperty> DockLayoutSizeProperties = new()
		{
			WidthProperty,
			HeightProperty
		};

		static DockLayout()
		{
			RegisterLayoutProperties<DockLayout>(DockLayoutProperties);
			RegisterLayoutSerializer<DockLayout>(new DockLayoutSerializer());
		}

		internal DockLayoutView DockLayoutView => (DockLayoutView) View;

		public override LayoutKind LayoutKind => LayoutKind.Dock;

		public static double GetHeight(DependencyObject depObj)
		{
			return (double) depObj.GetValue(HeightProperty);
		}

		public static Dock GetDock(DependencyObject depObj)
		{
			return (Dock) depObj.GetValue(DockProperty);
		}

		public static Size GetSize(DependencyObject depObj)
		{
			return new Size(GetWidth(depObj), GetHeight(depObj));
		}

		public static double GetWidth(DependencyObject depObj)
		{
			return (double) depObj.GetValue(WidthProperty);
		}

		private static void OnHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnSizePropertyChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is DockItem dockItem)
			{
				var dockLayout = dockItem.ActualLayout as DockLayout;

				dockLayout?.OnItemDockChanged(dockItem);
			}

			OnLayoutPropertyChanged(d, e);
		}

		private static void OnSizePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var dockLayout = dockItem.ActualLayout as DockLayout;

			dockLayout?.OnItemSizeChanged(dockItem);
		}

		private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnSizePropertyChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		private void OnItemDockChanged(DockItem item)
		{
			if (item.AttachToView)
				DockLayoutView?.OnItemDockChanged(item);
		}

		private void OnItemSizeChanged(DockItem item)
		{
			if (item.AttachToView)
				DockLayoutView?.OnItemSizeChanged(item);
		}

		public static void SetHeight(DependencyObject depObj, double value)
		{
			depObj.SetValue(HeightProperty, value);
		}

		public static void SetDock(DependencyObject depObj, Dock value)
		{
			depObj.SetValue(DockProperty, value);
		}

		public static void SetSize(DependencyObject depObj, Size size)
		{
			SetWidth(depObj, size.Width);
			SetHeight(depObj, size.Height);
		}

		public static void SetWidth(DependencyObject depObj, double value)
		{
			depObj.SetValue(WidthProperty, value);
		}

		protected override int GetDockItemOrder(DockItem dockItem)
		{
			return GetOrder(dockItem);
		}

		private sealed class DockLayoutSerializer : LayoutSerializer
		{
			private static readonly Type LayoutType = typeof(DockLayout);

			public override void WriteProperties(DependencyObject dependencyObject, XElement element)
			{
				if (DockLayoutSizeProperties.Any(l => ShouldSerializeProperty(LayoutType, dependencyObject, l)))
				{
					var propertyName = FormatProperty(LayoutType, "Size");

					element.Add(new XAttribute(propertyName, GetSize(dependencyObject).ToString(CultureInfo.InvariantCulture)));
				}

				if (ShouldSerializeProperty(LayoutType, dependencyObject, DockProperty))
				{
					var propertyName = FormatProperty(LayoutType, "Dock");

					element.Add(new XAttribute(propertyName, GetDock(dependencyObject).ToString()));
				}
			}
		}
	}
}