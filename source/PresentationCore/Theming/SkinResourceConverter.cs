// <copyright file="SkinResourceConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class SkinResourceConverter : IValueConverter
	{
		public static readonly IValueConverter Instance = new SkinResourceConverter();

		private SkinResourceConverter()
		{
		}

		private object GetSkinValue(object value, object parameter)
		{
			if (value is not SkinBase skin)
				return null;

			if (parameter is ThemeResourceKey key)
				return skin.GetValueInternal(key);

			return parameter is string valuePath ? skin.GetValueInternal(valuePath) : null;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return TargetNullValueConverter.Instance.Convert(GetSkinValue(value, parameter), targetType, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}