// <copyright file="IsSubclassOfVisibilityConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class IsSubclassOfVisibilityConverter : BaseValueConverter
	{
		public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

		public bool Self { get; set; } = true;

		public Visibility TrueVisibility { get; set; } = Visibility.Visible;

		public Type Type { get; set; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return false;

			var type = value.GetType();

			return type.IsSubclassOf(Type) || (Self && Type == type) ? TrueVisibility : FalseVisibility;
		}
	}
}