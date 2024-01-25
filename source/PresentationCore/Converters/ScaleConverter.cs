// <copyright file="ScaleConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class ScaleConverter : BaseValueConverter
	{
		public static readonly ScaleConverter Instance = new ScaleConverter();

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var scale = 1.0;

			switch (parameter)
			{
				case double d:

					scale = d;

					break;
				case int i:

					scale = i;

					break;
				case string s:

					double.TryParse(s, out scale);

					break;
			}

			return (double) value * scale;
		}
	}
}