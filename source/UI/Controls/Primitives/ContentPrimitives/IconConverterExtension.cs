// <copyright file="IconConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Globalization;
using System.Windows.Data;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public class IconConverterExtension : MarkupExtensionBase, IValueConverter
	{
		public static readonly IconConverterExtension Instance = new();

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Instance;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return IconConverter.ConvertFrom(null, culture, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new NotSupportedException();
		}
	}
}