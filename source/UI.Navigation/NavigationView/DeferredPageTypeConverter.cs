// <copyright file="DeferredPageTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.UI.Controls.NavigationView
{
	internal sealed class DeferredPageTypeConverter : BaseValueConverter
	{
		public DeferredPageTypeConverter(NavigationViewItem navigationViewItem)
		{
			NavigationViewItem = navigationViewItem;
		}

		public NavigationViewItem NavigationViewItem { get; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return NavigationViewItem.LoadPage((Type) value);
		}
	}
}