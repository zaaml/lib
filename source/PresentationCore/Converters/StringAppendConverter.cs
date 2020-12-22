// <copyright file="StringAppendConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class StringAppendConverter : BaseValueConverter
	{
		public string Value { get; set; }
			
		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var actualValue = Value ?? parameter as string;

			if (value is string stringValue && actualValue != null && stringValue.EndsWith(actualValue, StringComparison.OrdinalIgnoreCase))
				return stringValue.Substring(0, stringValue.Length - actualValue.Length);

			return value;
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var actualValue = Value ?? parameter as string;
			
			if (value is string stringValue)
				return value + actualValue;

			return value;
		}
	}
}