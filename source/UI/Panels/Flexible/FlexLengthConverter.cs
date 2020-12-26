// <copyright file="FlexLengthConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Globalization;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

namespace Zaaml.UI.Panels.Flexible
{
	internal static class FlexLengthConverter
	{
		private static readonly string[] UnitStrings = {"auto", "px", "*"};
		private static readonly string[] PixelUnitStrings = {"in", "cm", "pt"};

		private static readonly double[] PixelUnitFactors =
		{
			96.0, // Pixels per Inch
			96.0 / 2.54, // Pixels per Centimeter
			96.0 / 72.0, // Pixels per Point
		};

		internal static FlexLength FromString(string s, CultureInfo cultureInfo)
		{
			FromString(s, cultureInfo, out var num, out var unit);

			return new FlexLength(num, unit);
		}

		private static void FromString(string s, CultureInfo cultureInfo, out double value, out FlexLengthUnitType unit)
		{
			var normalized = s.Trim().ToLowerInvariant();

			unit = FlexLengthUnitType.Pixel;

			var strLen = normalized.Length;
			var strLenUnit = 0;
			var unitFactor = 1.0;

			var i = 0;

			if (normalized == UnitStrings[i])
			{
				strLenUnit = UnitStrings[i].Length;
				unit = (FlexLengthUnitType) i;
			}
			else
			{
				for (i = 1; i < UnitStrings.Length; ++i)
				{
					if (normalized.EndsWith(UnitStrings[i], StringComparison.Ordinal) == false)
						continue;

					strLenUnit = UnitStrings[i].Length;
					unit = (FlexLengthUnitType) i;

					break;
				}
			}

			if (i >= UnitStrings.Length)
			{
				for (i = 0; i < PixelUnitStrings.Length; ++i)
				{
					if (normalized.EndsWith(PixelUnitStrings[i], StringComparison.Ordinal) == false)
						continue;

					strLenUnit = PixelUnitStrings[i].Length;
					unitFactor = PixelUnitFactors[i];

					break;
				}
			}

			if (strLen == strLenUnit && (unit == FlexLengthUnitType.Auto || unit == FlexLengthUnitType.Star))
				value = 1;
			else
			{
				Debug.Assert(unit == FlexLengthUnitType.Pixel || DoubleUtils.AreClose(unitFactor, 1.0));

				var valueString = normalized.Substring(0, strLen - strLenUnit);

				value = Convert.ToDouble(valueString, cultureInfo) * unitFactor;
			}
		}

		internal static string ToString(FlexLength flexLength, CultureInfo cultureInfo)
		{
			switch (flexLength.UnitType)
			{
				case FlexLengthUnitType.Auto:
					return "Auto";

				case FlexLengthUnitType.Star:
					return flexLength.Value.IsCloseTo(1.0) ? "*" : Convert.ToString(flexLength.Value, cultureInfo) + "*";

				default:
					return Convert.ToString(flexLength.Value, cultureInfo);
			}
		}
	}
}