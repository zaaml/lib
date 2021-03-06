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
			return TryConvertFromString(colorString, formatProvider, out var color) ? color : throw new InvalidOperationException();
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

		public static double GetChannelMaximumValue(ColorChannel channel)
		{
			return channel switch
			{
				ColorChannel.Alpha => 1.0,
				ColorChannel.Red => 1.0,
				ColorChannel.Green => 1.0,
				ColorChannel.Blue => 1.0,
				ColorChannel.Hue => 360.0,
				ColorChannel.Saturation => 1.0,
				ColorChannel.Value => 1.0,
				_ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
			};
		}

		public static double GetChannelMinimumValue(ColorChannel channel)
		{
			return 0;
		}

		internal static ColorSpace GetChannelSpace(ColorChannel channel, ColorSpace alphaSpace)
		{
			return channel switch
			{
				ColorChannel.Alpha => alphaSpace,
				ColorChannel.Red => ColorSpace.Rgb,
				ColorChannel.Green => ColorSpace.Rgb,
				ColorChannel.Blue => ColorSpace.Rgb,
				ColorChannel.Hue => ColorSpace.Hsv,
				ColorChannel.Saturation => ColorSpace.Hsv,
				ColorChannel.Value => ColorSpace.Hsv,
				_ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
			};
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

		public static bool TryConvertFromString(string colorString, out Color color)
		{
			return TryConvertFromString(colorString, CultureInfo.CurrentCulture, out color);
		}

		public static bool TryConvertFromString(string colorString, IFormatProvider formatProvider, out Color color)
		{
			var normalized = colorString.Trim();
			var index = normalized.StartsWith("#") ? 1 : 0;

			if (index == 0 && KnownColors.TryGetColor(normalized, out color))
				return true;

			if (normalized.Length == index + 8)
			{
				var pa = byte.TryParse(colorString.Substring(index, 2), NumberStyles.HexNumber, formatProvider, out var a);
				var pr = byte.TryParse(colorString.Substring(index + 2, 2), NumberStyles.HexNumber, formatProvider, out var r);
				var pg = byte.TryParse(colorString.Substring(index + 4, 2), NumberStyles.HexNumber, formatProvider, out var g);
				var pb = byte.TryParse(colorString.Substring(index + 6, 2), NumberStyles.HexNumber, formatProvider, out var b);

				if (pa && pr && pg && pb)
				{
					color = Color.FromArgb(a, r, g, b);

					return true;
				}
			}
			else if (normalized.Length == index + 6)
			{
				var pr = byte.TryParse(colorString.Substring(index, 2), NumberStyles.HexNumber, formatProvider, out var r);
				var pg = byte.TryParse(colorString.Substring(index + 2, 2), NumberStyles.HexNumber, formatProvider, out var g);
				var pb = byte.TryParse(colorString.Substring(index + 4, 2), NumberStyles.HexNumber, formatProvider, out var b);

				if (pr && pg && pb)
				{
					color = Color.FromArgb(0xFF, r, g, b);

					return true;
				}
			}

			color = default;

			return false;
		}
	}
}