// <copyright file="PopupLengthTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Zaaml.Core.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class PopupLengthTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
		{
			switch (Type.GetTypeCode(sourceType))
			{
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.String:
					return true;
				default:
					return false;
			}
		}

		public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
		{
			return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
		{
			if (source == null)
				throw GetConvertFromExceptionInt(null);

			if (source is string stringValue)
				return PopupLengthConverter.FromString(stringValue, cultureInfo);

			var num = Convert.ToDouble(source, cultureInfo);

			PopupLengthUnitType type;

			if (DoubleUtils.IsNaN(num))
			{
				num = 1.0;
				type = PopupLengthUnitType.Auto;
			}
			else
				type = PopupLengthUnitType.Absolute;

			return new PopupLength(num, type);
		}

		public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
		{
			if (destinationType == null)
				throw new ArgumentNullException(nameof(destinationType));

			if (value is PopupLength == false)
				throw GetConvertToExceptionInt(value, destinationType);

			var flexLength = (PopupLength) value;

			if (destinationType == typeof(string))
				return PopupLengthConverter.ToString(flexLength, cultureInfo);

			if (destinationType == typeof(InstanceDescriptor))
				return new InstanceDescriptor(typeof(PopupLength).GetConstructor(new[] {typeof(double), typeof(PopupLengthUnitType)}), new object[] {flexLength.Value, flexLength.UnitType});

			throw GetConvertToExceptionInt(value, destinationType);
		}

		private Exception GetConvertFromExceptionInt(object value)
		{
			throw new NotSupportedException($"Can not convert from {(value == null ? "null" : value.GetType().FullName)}");
		}

		private Exception GetConvertToExceptionInt(object value, Type destinationType)
		{
			throw new NotSupportedException($"Can not convert to {destinationType.FullName}");
		}
	}
}