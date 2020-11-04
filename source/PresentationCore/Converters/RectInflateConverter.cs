// <copyright file="RectInflateConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class RectInflateConverter : BaseValueConverter
	{
		public Thickness Inflate { get; set; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Rect rect)
				return rect.GetInflated(Inflate.Negate());

			return value;
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Rect rect)
				return rect.GetInflated(Inflate);

			return value;
		}
	}
}