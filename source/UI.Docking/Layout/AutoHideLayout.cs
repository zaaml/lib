// <copyright file="AutoHideLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

		static AutoHideLayout()
		{
			RegisterLayoutProperties<AutoHideLayout>(AutoHideLayoutProperties);
		}

		public override LayoutKind LayoutKind => LayoutKind.AutoHide;

		public static double GetAutoHideHeight(DependencyObject dependencyObject)
		{
			return (double) dependencyObject.GetValue(AutoHideHeightProperty);
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

		public static void SetAutoHideWidth(DependencyObject dependencyObject, double width)
		{
			dependencyObject.SetValue(AutoHideWidthProperty, width);
		}

		public static void SetDockSide(DependencyObject depObj, Dock value)
		{
			depObj.SetValue(DockSideProperty, value);
		}
	}
}