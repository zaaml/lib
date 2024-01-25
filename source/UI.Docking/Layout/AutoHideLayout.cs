// <copyright file="AutoHideLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
	public sealed class AutoHideLayout : BaseLayout
	{
		public static readonly DependencyProperty DockProperty = DPM.RegisterAttached<Dock, AutoHideLayout>
			("Dock", Dock.Left, OnDockPropertyChanged);

		public static readonly DependencyProperty WidthProperty = DPM.RegisterAttached<double, AutoHideLayout>
			("Width", 200, OnWidthPropertyChanged);

		public static readonly DependencyProperty HeightProperty = DPM.RegisterAttached<double, AutoHideLayout>
			("Height", 200, OnHeightPropertyChanged);

		public static readonly DependencyProperty OrderProperty = DPM.RegisterAttached<int, AutoHideLayout>
			("Order", 0, OnOrderPropertyChanged);

		private static readonly List<DependencyProperty> AutoHideLayoutProperties = new()
		{
			DockProperty,
			HeightProperty,
			WidthProperty,
			OrderProperty
		};

		private static readonly List<DependencyProperty> AutoHideLayoutSizeProperties = new()
		{
			HeightProperty,
			WidthProperty
		};

		static AutoHideLayout()
		{
			RegisterLayoutProperties<AutoHideLayout>(AutoHideLayoutProperties);
			RegisterLayoutSerializer<AutoHideLayout>(new AutoHideLayoutSerializer());
		}

		public override LayoutKind LayoutKind => LayoutKind.AutoHide;

		public static Size GetSize(DependencyObject depObj)
		{
			return new Size(GetWidth(depObj), GetHeight(depObj));
		}

		public static double GetHeight(DependencyObject dependencyObject)
		{
			return (double)dependencyObject.GetValue(HeightProperty);
		}

		public static int GetOrder(DependencyObject depObj)
		{
			return (int)depObj.GetValue(OrderProperty);
		}

		protected override int GetDockItemOrder(DockItem dockItem)
		{
			return GetOrder(dockItem);
		}

		public static Dock GetDock(DependencyObject depObj)
		{
			return (Dock)depObj.GetValue(DockProperty);
		}

		public static double GetWidth(DependencyObject dependencyObject)
		{
			return (double)dependencyObject.GetValue(WidthProperty);
		}

		private static void OnHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(d, e);
		}

		private void OnItemOrderChanged(DockItem dockItem)
		{
		}

		private static void OnOrderChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var autoHideLayout = dockItem.ActualLayout as AutoHideLayout;

			autoHideLayout?.OnItemOrderChanged(dockItem);
		}

		private static void OnOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnOrderChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(d, e);
		}

		public static void SetSize(DependencyObject depObj, Size size)
		{
			SetWidth(depObj, size.Width);
			SetHeight(depObj, size.Height);
		}

		public static void SetHeight(DependencyObject dependencyObject, double height)
		{
			dependencyObject.SetValue(HeightProperty, height);
		}

		public static void SetOrder(DependencyObject depObj, int orderIndex)
		{
			depObj.SetValue(OrderProperty, orderIndex);
		}

		public static void SetDock(DependencyObject depObj, Dock value)
		{
			depObj.SetValue(DockProperty, value);
		}

		public static void SetWidth(DependencyObject dependencyObject, double width)
		{
			dependencyObject.SetValue(WidthProperty, width);
		}

		private sealed class AutoHideLayoutSerializer : LayoutSerializer
		{
			private static readonly Type LayoutType = typeof(AutoHideLayout);

			public override void WriteProperties(DependencyObject dependencyObject, XElement element)
			{
				if (AutoHideLayoutSizeProperties.Any(l => ShouldSerializeProperty(LayoutType, dependencyObject, l)))
				{
					var propertyName = FormatProperty(typeof(AutoHideLayout), "Size");

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