// <copyright file="InvertedRangeValueConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
	internal sealed class InvertedRangeValueConverter : BaseValueConverter
	{
		public InvertedRangeValueConverter()
		{
		}

		public InvertedRangeValueConverter(double minimum, double maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public double Maximum { get; set; }

		public double Minimum { get; set; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var doubleValue = (double) value;

			return Maximum + Minimum - doubleValue;
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var doubleValue = (double) value;

			return Maximum + Minimum - doubleValue;
		}
	}
}