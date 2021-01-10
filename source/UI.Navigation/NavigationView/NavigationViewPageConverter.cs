// <copyright file="NavigationViewPageConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewPageConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is Type typeValue)
			{
				if (context.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget targetProvider && targetProvider.TargetObject is NavigationViewItem navigationViewItem)
					return new Binding {Source = typeValue, BindsDirectlyToSource = true, Converter = new DeferredPageTypeConverter(navigationViewItem)}.ProvideValue(context);

				return null;
			}

			return new NavigationViewPage {Content = value};
		}
	}
}