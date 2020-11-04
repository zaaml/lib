// <copyright file="TextTrimmingConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.UI.Controls.Core.Compatibility
{
	internal class TextTrimmingConverter : IValueConverter
	{
		#region Static Fields and Constants

		public static IValueConverter Instance = new TextTrimmingConverter();

		#endregion

		#region Interface Implementations

		#region IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var textTrimming = (TextTrimming) value;
			return textTrimming == TextTrimming.WordEllipsis ? System.Windows.TextTrimming.WordEllipsis : System.Windows.TextTrimming.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion

		#endregion
	}
}