// <copyright file="StringPrependConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class StringPrependConverter : BaseValueConverter
	{
		public string Value { get; set; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var actualValue = Value ?? parameter as string;

			if (value is string stringValue && actualValue != null && stringValue.StartsWith(actualValue, StringComparison.OrdinalIgnoreCase))
				return stringValue.Substring(actualValue.Length, stringValue.Length - actualValue.Length);

			return value;
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var actualValue = Value ?? parameter as string;

			if (value is string stringValue)
				return actualValue + stringValue;

			return value;
		}
	}
}