// <copyright file="EnumCheckedConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
	public class EnumBoolConverter : BaseValueConverter
	{
		public object FalseEnumValue { get; set; }

		public object TrueEnumValue { get; set; }

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not bool boolValue)
				throw new NotSupportedException();

			if (boolValue == false)
				return ConvertEnumValue(FalseEnumValue, targetType, Binding.DoNothing);

			return ConvertEnumValue(TrueEnumValue, targetType, Binding.DoNothing);
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Enum enumValue)
			{
				var targetValue = ConvertEnumValue(TrueEnumValue, enumValue.GetType(), null);

				if (Equals(targetValue, value))
					return true;
			}

			return false;
		}

		private object ConvertEnumValue(object enumValue, Type enumType, object fallbackValue)
		{
			if (enumType.IsEnum == false)
				throw new InvalidOperationException();

			if (enumValue is Enum && enumValue.GetType() == enumType)
				return enumValue;

			if (enumValue is string stringValue)
				return Enum.Parse(enumType, stringValue, true);

			return fallbackValue;
		}
	}
}