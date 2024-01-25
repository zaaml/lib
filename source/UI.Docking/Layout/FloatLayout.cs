// <copyright file="FloatLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Zaaml.Core;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class FloatLayout : BaseLayout
	{
		internal static readonly double DefaultWidth = 640;
		internal static readonly double DefaultHeight= 480;

		public static readonly DependencyProperty LeftProperty = DPM.RegisterAttached<double, FloatLayout>
			("Left", OnLeftPropertyChanged);

		public static readonly DependencyProperty TopProperty = DPM.RegisterAttached<double, FloatLayout>
			("Top", OnTopPropertyChanged);

		public static readonly DependencyProperty WidthProperty = DPM.RegisterAttached<double, FloatLayout>
			("Width", DefaultWidth, OnWidthPropertyChanged);

		public static readonly DependencyProperty HeightProperty = DPM.RegisterAttached<double, FloatLayout>
			("Height", DefaultHeight, OnHeightPropertyChanged);

		public static readonly DependencyProperty OrderProperty = DPM.RegisterAttached<int, FloatLayout>
			("Order", 0, OnOrderPropertyChanged);

		private static readonly List<DependencyProperty> FloatLayoutProperties = new()
		{
			LeftProperty,
			TopProperty,
			WidthProperty,
			HeightProperty,
			OrderProperty
		};

		private static readonly List<DependencyProperty> FloatLayoutSizeProperties = new()
		{
			LeftProperty,
			TopProperty,
			WidthProperty,
			HeightProperty
		};

		static FloatLayout()
		{
			RegisterLayoutProperties<FloatLayout>(FloatLayoutProperties);
			RegisterLayoutSerializer<FloatLayout>(new FloatLayoutSerializer());
		}

		internal FloatLayoutView FloatLayoutView => (FloatLayoutView)View;

		public override LayoutKind LayoutKind => LayoutKind.Float;

		public void BringWindowToFront(DockItem item)
		{
			//SetNewIndex(item);
			throw Error.Refactoring;
		}

		protected override int GetDockItemOrder(DockItem dockItem)
		{
			return GetOrder(dockItem);
		}

		public static double GetHeight(DependencyObject depObj)
		{
			return (double)depObj.GetValue(HeightProperty);
		}

		public static double GetLeft(DependencyObject depObj)
		{
			return (double)depObj.GetValue(LeftProperty);
		}

		public static int GetOrder(DependencyObject depObj)
		{
			return (int)depObj.GetValue(OrderProperty);
		}

		public static Point GetPosition(DependencyObject depObj)
		{
			return new Point(GetLeft(depObj), GetTop(depObj));
		}

		public static Rect GetRect(DependencyObject depObj)
		{
			return new Rect(GetPosition(depObj), GetSize(depObj));
		}

		public static Size GetSize(DependencyObject depObj)
		{
			return new Size(GetWidth(depObj), GetHeight(depObj));
		}

		public static double GetTop(DependencyObject depObj)
		{
			return (double)depObj.GetValue(TopProperty);
		}

		public static double GetWidth(DependencyObject depObj)
		{
			return (double)depObj.GetValue(WidthProperty);
		}

		private static void OnFloatLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dockItem = d as DockItem;

			dockItem?.FloatingWindow?.UpdateLocationAndSize(dockItem);
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnFloatLayoutPropertyChanged(d, e);
		}

		private void OnItemOrderChanged(DockItem dockItem)
		{
		}

		private static void OnLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnFloatLayoutPropertyChanged(d, e);
		}

		private static void OnOrderChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var floatLayout = dockItem.ActualLayout as FloatLayout;

			floatLayout?.OnItemOrderChanged(dockItem);
		}

		private static void OnOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnOrderChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnFloatLayoutPropertyChanged(d, e);
		}

		private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnFloatLayoutPropertyChanged(d, e);
		}

		public static void SetHeight(DependencyObject depObj, double value)
		{
			depObj.SetValue(HeightProperty, value);
		}

		public static void SetLeft(DependencyObject depObj, double value)
		{
			depObj.SetValue(LeftProperty, value);
		}

		public static void SetOrder(DependencyObject depObj, int orderIndex)
		{
			depObj.SetValue(OrderProperty, orderIndex);
		}

		public static void SetPosition(DependencyObject depObj, Point position)
		{
			SetLeft(depObj, position.X);
			SetTop(depObj, position.Y);
		}

		public static void SetPosition(DependencyObject depObj, double x, double y)
		{
			depObj.SetValue(LeftProperty, x);
			depObj.SetValue(TopProperty, y);
		}

		public static void SetRect(DependencyObject depObj, Rect box)
		{
			SetPosition(depObj, box.GetTopLeft());
			SetSize(depObj, box.Size());
		}

		public static void SetSize(DependencyObject depObj, Size size)
		{
			SetWidth(depObj, size.Width);
			SetHeight(depObj, size.Height);
		}

		public static void SetTop(DependencyObject depObj, double value)
		{
			depObj.SetValue(TopProperty, value);
		}

		[TypeConverter(typeof(NullableTypeConverter<double>))]
		public static void SetWidth(DependencyObject depObj, double? value)
		{
			depObj.SetValue(WidthProperty, value);
		}

		private sealed class FloatLayoutSerializer : LayoutSerializer
		{
			private static readonly Type LayoutType = typeof(FloatLayout);

			public override void WriteProperties(DependencyObject dependencyObject, XElement element)
			{
				if (FloatLayoutSizeProperties.Any(l => ShouldSerializeProperty(LayoutType, dependencyObject, l)))
				{
					var propertyName = FormatProperty(LayoutType, "Rect");

					element.Add(new XAttribute(propertyName, GetRect(dependencyObject).ToString(CultureInfo.InvariantCulture)));
				}
			}
		}
	}
}