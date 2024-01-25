// <copyright file="IconSelectorConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	internal sealed class IconSelectorConverter : IValueConverter
	{
		private readonly IIconSelector _selector;

		public IconSelectorConverter(IIconSelector selector)
		{
			_selector = selector;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return _selector != null ? _selector.Select(value) : value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}