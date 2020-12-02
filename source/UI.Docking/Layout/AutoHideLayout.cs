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
		public static readonly DependencyProperty DockSideProperty = DPM.RegisterAttached<Dock, AutoHideLayout>
			("DockSide", Dock.Top, OnDockSidePropertyChanged);

		public static readonly DependencyProperty AutoHideWidthProperty = DPM.RegisterAttached<double, AutoHideLayout>
			("AutoHideWidth", 200, OnAutoHideWidthPropertyChanged);

		public static readonly DependencyProperty AutoHideHeightProperty = DPM.RegisterAttached<double, AutoHideLayout>
			("AutoHideHeight", 200, OnAutoHideHeightPropertyChanged);

		private static readonly List<DependencyProperty> AutoHideLayoutProperties = new List<DependencyProperty>
		{
			DockSideProperty,
			AutoHideHeightProperty,
			AutoHideWidthProperty
		};

		private static readonly List<DependencyProperty> AutoHideLayoutSizeProperties = new List<DependencyProperty>
		{
			AutoHideHeightProperty,
			AutoHideWidthProperty
		};

		static AutoHideLayout()
		{
			RegisterLayoutProperties<AutoHideLayout>(AutoHideLayoutProperties);
			RegisterLayoutSerializer<AutoHideLayout>(new AutoHideLayoutSerializer());
		}

		public override LayoutKind LayoutKind => LayoutKind.AutoHide;

		public static double GetAutoHideHeight(DependencyObject dependencyObject)
		{
			return (double) dependencyObject.GetValue(AutoHideHeightProperty);
		}

		public static Size GetAutoHideSize(DependencyObject depObj)
		{
			return new Size(GetAutoHideWidth(depObj), GetAutoHideHeight(depObj));
		}

		public static double GetAutoHideWidth(DependencyObject dependencyObject)
		{
			return (double) dependencyObject.GetValue(AutoHideWidthProperty);
		}

		public static Dock GetDockSide(DependencyObject depObj)
		{
			return (Dock) depObj.GetValue(DockSideProperty);
		}

		private static void OnAutoHideHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnAutoHideWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(d, e);
		}

		private static void OnDockSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(d, e);
		}

		public static void SetAutoHideHeight(DependencyObject dependencyObject, double height)
		{
			dependencyObject.SetValue(AutoHideHeightProperty, height);
		}

		public static void SetAutoHideSize(DependencyObject depObj, Size size)
		{
			SetAutoHideWidth(depObj, size.Width);
			SetAutoHideHeight(depObj, size.Height);
		}

		public static void SetAutoHideWidth(DependencyObject dependencyObject, double width)
		{
			dependencyObject.SetValue(AutoHideWidthProperty, width);
		}

		public static void SetDockSide(DependencyObject depObj, Dock value)
		{
			depObj.SetValue(DockSideProperty, value);
		}

		private sealed class AutoHideLayoutSerializer : LayoutSerializer
		{
			private static readonly Type LayoutType = typeof(AutoHideLayout);

			public override void WriteProperties(DependencyObject dependencyObject, XElement element)
			{
				if (AutoHideLayoutSizeProperties.Any(l => ShouldSerializeProperty(LayoutType, dependencyObject, l)))
				{
					var propertyName = FormatProperty(typeof(DockLayout), "AutoHideSize");

					element.Add(new XAttribute(propertyName, GetAutoHideSize(dependencyObject).ToString(CultureInfo.InvariantCulture)));
				}

				if (ShouldSerializeProperty(LayoutType, dependencyObject, DockSideProperty))
				{
					var propertyName = FormatProperty(LayoutType, "DockSide");

					element.Add(new XAttribute(propertyName, GetDockSide(dependencyObject).ToString()));
				}
			}
		}
	}
}