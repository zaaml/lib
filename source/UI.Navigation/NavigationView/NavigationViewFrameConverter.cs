// <copyright file="NavigationViewFrameConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewFrameConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string stringValue)
			{
				return new Binding {ElementName = stringValue, BindsDirectlyToSource = true}.ProvideValue(context);
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}