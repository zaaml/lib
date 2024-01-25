// <copyright file="ThemeSkinConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Theming
{
	[ContentProperty("SkinCollection")]
	public sealed class ThemeSkinConverter : IValueConverter
	{
		public DependencyObjectCollectionBase<ThemeSkinBase> SkinCollection { get; } = new();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return SkinCollection.FirstOrDefault(t => t.ThemeInternal == value) ?? SkinCollection.OfType<GenericThemeSkin>().FirstOrDefault();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value as ThemeSkinBase)?.ThemeInternal;
		}
	}
}