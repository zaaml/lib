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
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class FloatLayout : BaseLayout
	{
		public static readonly DependencyProperty FloatLeftProperty = DPM.RegisterAttached<double, FloatLayout>
			("FloatLeft", OnFloatLayoutPropertyChanged);

		public static readonly DependencyProperty FloatTopProperty = DPM.RegisterAttached<double, FloatLayout>
			("FloatTop", OnFloatLayoutPropertyChanged);

		public static readonly DependencyProperty FloatWidthProperty = DPM.RegisterAttached<double, FloatLayout>
			("FloatWidth", 200, OnFloatLayoutPropertyChanged);

		public static readonly DependencyProperty FloatHeightProperty = DPM.RegisterAttached<double, FloatLayout>
			("FloatHeight", 200, OnFloatLayoutPropertyChanged);

		private static readonly List<DependencyProperty> FloatLayoutProperties = new List<DependencyProperty>
		{
			FloatLeftProperty,
			FloatTopProperty,
			FloatWidthProperty,
			FloatHeightProperty
		};

		private static readonly List<DependencyProperty> FloatLayoutSizeProperties = new List<DependencyProperty>
		{
			FloatLeftProperty,
			FloatTopProperty,
			FloatWidthProperty,
			FloatHeightProperty
		};

		static FloatLayout()
		{
			RegisterLayoutProperties<FloatLayout>(FloatLayoutProperties);
			RegisterLayoutSerializer<FloatLayout>(new FloatLayoutSerializer());
		}

		internal FloatLayoutView FloatLayoutView => (FloatLayoutView) View;

		public override LayoutKind LayoutKind => LayoutKind.Float;

		public void BringWindowToFront(DockItem item)
		{
			SetDockItemIndex(item, IndexProvider.NewIndex);
		}

		public static double GetFloatHeight(DependencyObject depObj)
		{
			return (double) depObj.GetValue(FloatHeightProperty);
		}

		public static double GetFloatLeft(DependencyObject depObj)
		{
			return (double) depObj.GetValue(FloatLeftProperty);
		}

		public static Point GetFloatPosition(DependencyObject depObj)
		{
			return new Point(GetFloatLeft(depObj), GetFloatTop(depObj));
		}

		public static Rect GetFloatRect(DependencyObject depObj)
		{
			return new Rect(GetFloatPosition(depObj), GetFloatSize(depObj));
		}

		public static Size GetFloatSize(DependencyObject depObj)
		{
			return new Size(GetFloatWidth(depObj), GetFloatHeight(depObj));
		}

		public static double GetFloatTop(DependencyObject depObj)
		{
			return (double) depObj.GetValue(FloatTopProperty);
		}

		public static double GetFloatWidth(DependencyObject depObj)
		{
			return (double) depObj.GetValue(FloatWidthProperty);
		}

		private static void OnFloatLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dockItem = d as DockItem;

			dockItem?.FloatingWindow?.UpdateLocationAndSize(dockItem);
			OnLayoutPropertyChanged(d, e);
		}

		public static void SetFloatHeight(DependencyObject depObj, double value)
		{
			depObj.SetValue(FloatHeightProperty, value);
		}

		public static void SetFloatLeft(DependencyObject depObj, double value)
		{
			depObj.SetValue(FloatLeftProperty, value);
		}

		public static void SetFloatPosition(DependencyObject depObj, Point position)
		{
			SetFloatLeft(depObj, position.X);
			SetFloatTop(depObj, position.Y);
		}

		public static void SetFloatRect(DependencyObject depObj, Rect box)
		{
			SetFloatPosition(depObj, box.GetTopLeft());
			SetFloatSize(depObj, box.Size());
		}

		public static void SetFloatSize(DependencyObject depObj, Size size)
		{
			SetFloatWidth(depObj, size.Width);
			SetFloatHeight(depObj, size.Height);
		}

		public static void SetFloatTop(DependencyObject depObj, double value)
		{
			depObj.SetValue(FloatTopProperty, value);
		}

		[TypeConverter(typeof(NullableTypeConverter<double>))]
		public static void SetFloatWidth(DependencyObject depObj, double? value)
		{
			depObj.SetValue(FloatWidthProperty, value);
		}

		public static void SetLocation(DependencyObject depObj, double x, double y)
		{
			depObj.SetValue(FloatLeftProperty, x);
			depObj.SetValue(FloatTopProperty, y);
		}

		public static void SetLocation(DependencyObject depObj, Point location)
		{
			SetLocation(depObj, location.X, location.Y);
		}

		private sealed class FloatLayoutSerializer : LayoutSerializer
		{
			private static readonly Type LayoutType = typeof(FloatLayout);

			public override void WriteProperties(DependencyObject dependencyObject, XElement element)
			{
				if (FloatLayoutSizeProperties.Any(l => ShouldSerializeProperty(LayoutType, dependencyObject, l)))
				{
					var propertyName = FormatProperty(LayoutType, "FloatRect");

					element.Add(new XAttribute(propertyName, GetFloatRect(dependencyObject).ToString(CultureInfo.InvariantCulture)));
				}
			}
		}
	}
}