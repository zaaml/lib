// <copyright file="ColorUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
	public static class ColorUtils
	{
		public static Color ConvertFromString(string colorString)
		{
			return ConvertFromString(colorString, CultureInfo.CurrentCulture);
		}

		public static Color ConvertFromString(string colorString, IFormatProvider formatProvider)
		{
			var normalized = colorString.Trim();

			if (normalized.StartsWith("#") && normalized.Length == 9)
			{
				if (normalized.Length == 9)
				{
					var a = byte.Parse(colorString.Substring(1, 2), NumberStyles.HexNumber, formatProvider);
					var r = byte.Parse(colorString.Substring(3, 2), NumberStyles.HexNumber, formatProvider);
					var g = byte.Parse(colorString.Substring(5, 2), NumberStyles.HexNumber, formatProvider);
					var b = byte.Parse(colorString.Substring(7, 2), NumberStyles.HexNumber, formatProvider);

					return Color.FromArgb(a, r, g, b);
				}

				if (normalized.Length == 7)
				{
					var r = byte.Parse(colorString.Substring(1, 2), NumberStyles.HexNumber, formatProvider);
					var g = byte.Parse(colorString.Substring(3, 2), NumberStyles.HexNumber, formatProvider);
					var b = byte.Parse(colorString.Substring(5, 2), NumberStyles.HexNumber, formatProvider);

					return Color.FromArgb(0xFF, r, g, b);
				}
			}

			return KnownColors.TryGetColor(normalized, out var color) ? color : Colors.Transparent;
		}

		public static string ConvertToString(Color color)
		{
			return ConvertToString(color, CultureInfo.CurrentCulture);
		}

		public static string ConvertToString(Color color, IFormatProvider formatProvider)
		{
			return KnownColors.TryGetColorName(color, out var colorName) ? colorName : color.ToString(formatProvider);
		}

		public static SolidColorBrush CreateBrush(Color color)
		{
			return new SolidColorBrush(color);
		}

		public static double GetChannelValue(Color color, ColorChannel channel)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:
					return color.ToRgbColor().A;

				case ColorChannel.Red:
					return color.ToRgbColor().R;

				case ColorChannel.Green:
					return color.ToRgbColor().G;

				case ColorChannel.Blue:
					return color.ToRgbColor().B;

				case ColorChannel.Hue:
					return color.ToHsvColor().Hue;

				case ColorChannel.Saturation:
					return color.ToHsvColor().Saturation;

				case ColorChannel.Value:
					return color.ToHsvColor().Value;
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}

		public static Color ModifyChannelValue(Color color, ColorChannel channel, double value)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:
					return color.ToRgbColor().WithAlpha(value).ToXamlColor();

				case ColorChannel.Red:
					return color.ToRgbColor().WithRed(value).ToXamlColor();

				case ColorChannel.Green:
					return color.ToRgbColor().WithGreen(value).ToXamlColor();

				case ColorChannel.Blue:
					return color.ToRgbColor().WithBlue(value).ToXamlColor();

				case ColorChannel.Hue:
					return color.ToHsvColor().WithHue(value).ToXamlColor();

				case ColorChannel.Saturation:
					return color.ToHsvColor().WithSaturation(value).ToXamlColor();

				case ColorChannel.Value:
					return color.ToHsvColor().WithValue(value).ToXamlColor();

				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}
	}
}