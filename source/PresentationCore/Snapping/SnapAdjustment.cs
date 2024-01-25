// <copyright file="SnapAdjustment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Snapping
{
	[TypeConverter(typeof(SnapAdjustmentTypeConverter))]
	public struct SnapAdjustment
	{
		private static readonly char[] Separators = { ',', ' ' };

		public static readonly SnapAdjustment ZeroAdjustment = new();
		public double SideOffset { get; set; }
		public double CornerOffset { get; set; }

		public static SnapAdjustment operator +(SnapAdjustment first, SnapAdjustment second)
		{
			return new SnapAdjustment
			{
				SideOffset = first.SideOffset + second.SideOffset,
				CornerOffset = first.CornerOffset + second.CornerOffset
			};
		}

		public static SnapAdjustment Parse(string strValue)
		{
			var delimitedValues = strValue.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

			var sideOffset = delimitedValues.Length > 0 ? int.Parse(delimitedValues[0], CultureInfo.InvariantCulture) : 0;
			var cornerOffset = delimitedValues.Length > 1 ? int.Parse(delimitedValues[1], CultureInfo.InvariantCulture) : 0;

			return new SnapAdjustment
			{
				SideOffset = sideOffset,
				CornerOffset = cornerOffset
			};
		}
	}

	public sealed class SnapAdjustmentTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string strValue) 
				return SnapAdjustment.Parse(strValue);

			throw new InvalidOperationException();
		}
	}
}