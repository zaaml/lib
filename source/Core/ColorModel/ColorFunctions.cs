// <copyright file="ColorFunctions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.ColorModel
{
	internal enum ColorFunctionUnits
	{
		Absolute,
		Relative,
		Percentage
	}

	internal static class ColorFunctions
	{
		private static readonly RgbColor RgbWhite = RgbColor.FromRgb(1.0, 1.0, 1.0, 1.0);
		private static readonly RgbColor RgbBlack = RgbColor.FromRgb(1.0, 0.0, 0.0, 0.0);

		private static double CalculateFunction(FunctionKind functionKind, double first, double second)
		{
			switch (functionKind)
			{
				case FunctionKind.Increase:
					return first + second;
				case FunctionKind.Decrease:
					return first - second;
				default:
					throw new ArgumentOutOfRangeException(nameof(functionKind));
			}
		}

		private static double ChannelOperation(FunctionKind functionKind, double channel, double amount, ColorFunctionUnits units)
		{
			switch (units)
			{
				case ColorFunctionUnits.Absolute:
					return CalculateFunction(functionKind, channel, amount);
				case ColorFunctionUnits.Relative:
					return CalculateFunction(functionKind, channel, channel * amount);
				case ColorFunctionUnits.Percentage:
					return CalculateFunction(functionKind, channel, channel * amount / 100.0);
				default:
					throw new ArgumentOutOfRangeException(nameof(units));
			}
		}

		public static HslColor Darken(HslColor hsl, double amount, ColorFunctionUnits units)
		{
			hsl.Lightness = ChannelOperation(FunctionKind.Decrease, hsl.Lightness, amount, units);

			return hsl;
		}

		public static RgbColor Darken(RgbColor rgb, double amount, ColorFunctionUnits units)
		{
			return Darken(rgb.ToHslColor(), amount, units).ToRgbColor();
		}

		public static HsvColor Darken(HsvColor hsv, double amount, ColorFunctionUnits units)
		{
			return Darken(hsv.ToHslColor(), amount, units).ToHsvColor();
		}

		public static HslColor Desaturate(HslColor hsl, double amount, ColorFunctionUnits units)
		{
			hsl.Saturation = ChannelOperation(FunctionKind.Decrease, hsl.Saturation, amount, units);

			return hsl;
		}

		public static HsvColor Desaturate(HsvColor hsv, double amount, ColorFunctionUnits units)
		{
			hsv.Saturation = ChannelOperation(FunctionKind.Decrease, hsv.Saturation, amount, units);

			return hsv;
		}

		public static RgbColor Desaturate(RgbColor rgb, double amount, ColorFunctionUnits units)
		{
			return Desaturate(rgb.ToHslColor(), amount, units).ToRgbColor();
		}

		public static HslColor FadeIn(HslColor hsl, double amount, ColorFunctionUnits units)
		{
			hsl.Alpha = ChannelOperation(FunctionKind.Increase, hsl.Alpha, amount, units);

			return hsl;
		}

		public static HsvColor FadeIn(HsvColor hsv, double amount, ColorFunctionUnits units)
		{
			hsv.Alpha = ChannelOperation(FunctionKind.Increase, hsv.Alpha, amount, units);

			return hsv;
		}

		public static RgbColor FadeIn(RgbColor rgb, double amount, ColorFunctionUnits units)
		{
			rgb.A = ChannelOperation(FunctionKind.Increase, rgb.A, amount, units);

			return rgb;
		}

		public static HslColor FadeOut(HslColor hsl, double amount, ColorFunctionUnits units)
		{
			hsl.Alpha = ChannelOperation(FunctionKind.Decrease, hsl.Alpha, amount, units);
			return hsl;
		}

		public static HsvColor FadeOut(HsvColor hsv, double amount, ColorFunctionUnits units)
		{
			hsv.Alpha = ChannelOperation(FunctionKind.Decrease, hsv.Alpha, amount, units);

			return hsv;
		}

		public static RgbColor FadeOut(RgbColor rgb, double amount, ColorFunctionUnits units)
		{
			rgb.A = ChannelOperation(FunctionKind.Decrease, rgb.A, amount, units);

			return rgb;
		}

		private static double GetRelativeUnit(double amount, ColorFunctionUnits units)
		{
			switch (units)
			{
				case ColorFunctionUnits.Relative:
					return amount;
				case ColorFunctionUnits.Percentage:
					return amount / 100.0;
				default:
					throw new ArgumentOutOfRangeException(nameof(units));
			}
		}

		public static RgbColor Greyscale(RgbColor color)
		{
			return Desaturate(color, 100.0, ColorFunctionUnits.Percentage);
		}

		public static HsvColor Greyscale(HsvColor color)
		{
			return Desaturate(color, 100.0, ColorFunctionUnits.Percentage);
		}

		public static HslColor Greyscale(HslColor color)
		{
			return Desaturate(color, 100.0, ColorFunctionUnits.Percentage);
		}

		public static HslColor Lighten(HslColor hsl, double amount, ColorFunctionUnits units)
		{
			hsl.Lightness = ChannelOperation(FunctionKind.Increase, hsl.Lightness, amount, units);

			return hsl;
		}

		public static RgbColor Lighten(RgbColor rgb, double amount, ColorFunctionUnits units)
		{
			return Lighten(rgb.ToHslColor(), amount, units).ToRgbColor();
		}

		public static HsvColor Lighten(HsvColor hsv, double amount, ColorFunctionUnits units)
		{
			return Lighten(hsv.ToHslColor(), amount, units).ToHsvColor();
		}

		public static HslColor Mix(HslColor color1, HslColor color2, double weight = 0.5)
		{
			return RgbColor.Mix(color1, color2, weight);
		}

		public static HsvColor Mix(HsvColor color1, HsvColor color2, double weight = 0.5)
		{
			return RgbColor.Mix(color1, color2, weight);
		}

		public static RgbColor Mix(RgbColor color1, RgbColor color2, double weight = 0.5)
		{
			return RgbColor.Mix(color1, color2, weight);
		}

		public static HslColor Saturate(HslColor hsl, double amount, ColorFunctionUnits units)
		{
			hsl.Saturation = ChannelOperation(FunctionKind.Increase, hsl.Saturation, amount, units);

			return hsl;
		}

		public static HsvColor Saturate(HsvColor hsv, double amount, ColorFunctionUnits units)
		{
			hsv.Saturation = ChannelOperation(FunctionKind.Increase, hsv.Saturation, amount, units);

			return hsv;
		}

		public static RgbColor Saturate(RgbColor rgb, double amount, ColorFunctionUnits units)
		{
			return Saturate(rgb.ToHslColor(), amount, units).ToRgbColor();
		}

		public static RgbColor Shade(RgbColor color, double amount, ColorFunctionUnits units)
		{
			return Mix(RgbBlack, color, GetRelativeUnit(amount, units));
		}

		public static HslColor Shade(HslColor color, double amount, ColorFunctionUnits units)
		{
			return Mix(RgbBlack, (RgbColor) color, GetRelativeUnit(amount, units));
		}

		public static HsvColor Shade(HsvColor color, double amount, ColorFunctionUnits units)
		{
			return Mix(RgbBlack, (RgbColor) color, GetRelativeUnit(amount, units));
		}

		public static HslColor Spin(HslColor hsl, double amount)
		{
			var hue = (hsl.Hue + amount) % 360.0;

			hsl.Hue = hue < 0 ? 360.0 + hue : hue;

			return hsl;
		}

		public static HsvColor Spin(HsvColor hsv, double amount)
		{
			var hue = (hsv.Hue + amount) % 360.0;

			hsv.Hue = hue < 0 ? 360.0 + hue : hue;

			return hsv;
		}

		public static RgbColor Tint(RgbColor color, double amount, ColorFunctionUnits units)
		{
			return Mix(RgbWhite, color, GetRelativeUnit(amount, units));
		}

		public static HslColor Tint(HslColor color, double amount, ColorFunctionUnits units)
		{
			return Mix(RgbWhite, (RgbColor) color, GetRelativeUnit(amount, units));
		}

		public static HsvColor Tint(HsvColor color, double amount, ColorFunctionUnits units)
		{
			return Mix(RgbWhite, (RgbColor) color, GetRelativeUnit(amount, units));
		}

		private enum FunctionKind
		{
			Increase,
			Decrease
		}
	}
}