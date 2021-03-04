// <copyright file="ColorExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core.ColorModel;
using Zaaml.Core.Converters;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
	public static class ColorExtensions
	{
		private static readonly IPrimitiveConverter<string, Color> HexStrColorConverter = PrimitiveConverterFactory.GetConverter<string, Color>();

		public static Color FromHexString(string hexColor)
		{
			return HexStrColorConverter.Convert(hexColor);
		}

		public static double GetChannelValue(this Color color, ColorChannel channel)
		{
			return ColorUtils.GetChannelValue(color, channel);
		}

		public static Color ModifyChannelValue(this Color color, ColorChannel channel, double value)
		{
			return ColorUtils.ModifyChannelValue(color, channel, value);
		}

		internal static HslColor ToHslColor(this Color color)
		{
			return color.ToRgbColor().ToHslColor();
		}

		internal static HsvColor ToHsvColor(this Color color)
		{
			return color.ToRgbColor().ToHsvColor();
		}

		internal static RgbColor ToRgbColor(this Color color)
		{
			return new RgbColor(color.A / 255.0, color.R / 255.0, color.G / 255.0, color.B / 255.0);
		}

		public static SolidColorBrush ToSolidColorBrush(this Color color)
		{
			return ColorUtils.CreateBrush(color);
		}

		internal static Color ToXamlColor(this RgbColor rgbColor)
		{
			var alpha = (rgbColor.A * 255.0).Clamp(0, 255.0);
			var red = (rgbColor.R * 255.0).Clamp(0, 255.0);
			var green = (rgbColor.G * 255.0).Clamp(0, 255.0);
			var blue = (rgbColor.B * 255.0).Clamp(0, 255.0);

			return Color.FromArgb((byte) alpha, (byte) red, (byte) green, (byte) blue);
		}

		internal static Color ToXamlColor(this HsvColor hsvColor)
		{
			return hsvColor.ToRgbColor().ToXamlColor();
		}

		internal static Color ToXamlColor(this HslColor hslColor)
		{
			return hslColor.ToRgbColor().ToXamlColor();
		}

		public static Color WithOpacity(this Color color, byte opacity)
		{
			color.A = opacity;

			return color;
		}

		public static Color WithOpacity(this Color color, double opacity)
		{
			color.A = (byte) (Math.Max(0, Math.Min(1.0, opacity)) * 255);

			return color;
		}
	}
}