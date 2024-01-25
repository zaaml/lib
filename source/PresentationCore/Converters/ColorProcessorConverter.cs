// <copyright file="ModifyColorChannelConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media;

namespace Zaaml.PresentationCore.Converters
{
	[ContentProperty(nameof(Processor))]
	public sealed class ColorProcessorConverter : BaseValueConverter
	{
		public ColorProcessor Processor { get; set; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var color = (Color) value;

			return Processor?.Process(color) ?? color;
		}
	}
}