// <copyright file="DetachLogicalParentConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
	internal sealed class DetachLogicalParentConverter : BaseValueConverter
	{
		public static readonly IValueConverter Instance = new DetachLogicalParentConverter();

		private DetachLogicalParentConverter()
		{
		}

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not FrameworkElement freValue)
				return value;

			freValue.DetachFromLogicalParent();

			return value;
		}
	}
}