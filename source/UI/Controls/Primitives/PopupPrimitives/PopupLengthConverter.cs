// <copyright file="PopupLengthConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal static class PopupLengthConverter
	{
		private static readonly string[] PixelUnitStrings = {"px", "in", "cm", "pt"};

		private static readonly double[] PixelUnitFactors =
		{
			1.0,
			96.0, // Pixels per Inch
			96.0 / 2.54, // Pixels per Centimeter
			96.0 / 72.0, // Pixels per Point
		};

		internal static PopupLength FromString(string s, CultureInfo cultureInfo)
		{
			FromString(s, cultureInfo, out var num, out var unit, out var relativeElement);

			return new PopupLength(num, unit, relativeElement);
		}

		private static void FromString(string stringValue, CultureInfo cultureInfo, out double value, out PopupLengthUnitType unit, out string relativeElement)
		{
			var normalized = stringValue.Trim().ToLowerInvariant();

			// Auto
			if (string.Equals(normalized, "Auto", StringComparison.OrdinalIgnoreCase))
			{
				value = 0.0;
				unit = PopupLengthUnitType.Auto;
				relativeElement = null;

				return;
			}

			var starPosition = normalized.IndexOf("*", StringComparison.OrdinalIgnoreCase);

			// Absolute
			if (starPosition == -1)
			{
				relativeElement = null;
				unit = PopupLengthUnitType.Absolute;

				for (var i = 0; i < PixelUnitStrings.Length; i++)
				{
					var pixelUnitString = PixelUnitStrings[i];

					if (normalized.EndsWith(pixelUnitString, StringComparison.Ordinal) == false)
						continue;

					var valueString = normalized.Substring(0, normalized.Length - pixelUnitString.Length);

					value = Convert.ToDouble(valueString, cultureInfo) * PixelUnitFactors[i];

					return;
				}

				{
					var valueString = normalized.Substring(0, normalized.Length);

					value = Convert.ToDouble(valueString, cultureInfo);

					return;
				}
			}

			// Relative
			{
				unit = PopupLengthUnitType.Relative;

				if (starPosition > 0)
				{
					var valueString = normalized.Substring(0, starPosition);

					value = Convert.ToDouble(valueString, cultureInfo);
				}
				else
					value = 1.0;

				relativeElement = starPosition + 1 < normalized.Length ? normalized.Substring(starPosition + 1, normalized.Length - starPosition - 1) : null;
			}
		}

		internal static string ToString(PopupLength popupLength, CultureInfo cultureInfo)
		{
			switch (popupLength.UnitType)
			{
				case PopupLengthUnitType.Auto:
					return "Auto";

				case PopupLengthUnitType.Relative:
					return popupLength.Value.IsCloseTo(1.0) ? "*" : Convert.ToString(popupLength.Value, cultureInfo) + "*" + (popupLength.RelativeElement ?? string.Empty);

				default:
					return Convert.ToString(popupLength.Value, cultureInfo);
			}
		}
	}
}