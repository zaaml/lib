// <copyright file="DoubleExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
	public static class DoubleExtensions
	{
		public static double Clamp(this double value, double min, double max)
		{
			return DoubleUtils.Clamp(value, min, max);
		}

		public static double Clamp(this double value, Range<double> range)
		{
			return DoubleUtils.Clamp(value, range);
		}

		internal static double ExpandTo(this double self, double value)
		{
			return self < value ? value : self;
		}

		public static bool IsCloseTo(this double self, double value)
		{
			return DoubleUtils.AreClose(self, value);
		}

		public static bool IsCloseTo(this double self, double value, int precision)
		{
			return DoubleUtils.AreClose(self, value, precision);
		}

		public static bool IsGreaterThan(this double self, double value)
		{
			return DoubleUtils.GreaterThan(self, value);
		}

		public static bool IsGreaterThan(this double self, double value, int precision)
		{
			return DoubleUtils.GreaterThan(self, value, precision);
		}

		public static bool IsGreaterThanOrClose(this double self, double value)
		{
			return DoubleUtils.GreaterThanOrClose(self, value);
		}

		public static bool IsGreaterThanOrClose(this double self, double value, int precision)
		{
			return DoubleUtils.GreaterThanOrClose(self, value, precision);
		}

		public static bool IsInfinity(this double value)
		{
			return double.IsInfinity(value);
		}

		public static bool IsLessThan(this double self, double value)
		{
			return DoubleUtils.LessThan(self, value);
		}

		public static bool IsLessThan(this double self, double value, int precision)
		{
			return DoubleUtils.LessThan(self, value, precision);
		}

		public static bool IsLessThanOrClose(this double self, double value)
		{
			return DoubleUtils.LessThanOrClose(self, value);
		}

		public static bool IsLessThanOrClose(this double self, double value, int precision)
		{
			return DoubleUtils.LessThanOrClose(self, value, precision);
		}

		public static bool IsNaN(this double value)
		{
			return double.IsNaN(value);
		}

		public static bool IsNegativeInfinity(this double value)
		{
			return double.IsNegativeInfinity(value);
		}

		public static bool IsNegativeZero(this double self)
		{
			return DoubleUtils.IsNegativeZero(self);
		}

		public static bool IsPositiveInfinity(this double value)
		{
			return double.IsPositiveInfinity(value);
		}

		public static bool IsZero(this double self)
		{
			return DoubleUtils.IsZero(self);
		}

		public static bool IsZero(this double self, int precision)
		{
			return DoubleUtils.IsZero(self, precision);
		}

		internal static double RoundFromZero(this double self)
		{
			return DoubleUtils.Round(self, RoundingMode.FromZero);
		}

		internal static double RoundFromZero(this double self, int precision)
		{
			return DoubleUtils.Round(self, precision, RoundingMode.FromZero);
		}

		internal static double RoundMidPointFromZero(this double self, int precision)
		{
			return DoubleUtils.Round(self, precision, RoundingMode.MidPointFromZero);
		}

		internal static double RoundMidPointFromZero(this double self)
		{
			return DoubleUtils.Round(self, RoundingMode.MidPointFromZero);
		}

		internal static double RoundMidPointToEven(this double self)
		{
			return DoubleUtils.Round(self, RoundingMode.MidPointToEven);
		}

		internal static double RoundMidPointToEven(this double self, int precision)
		{
			return DoubleUtils.Round(self, precision, RoundingMode.MidPointToEven);
		}

		internal static double RoundToZero(this double self)
		{
			return DoubleUtils.Round(self, RoundingMode.ToZero);
		}

		internal static double RoundToZero(this double self, int precision)
		{
			return DoubleUtils.Round(self, precision, RoundingMode.ToZero);
		}

		internal static double Truncate(this double self)
		{
			return DoubleUtils.Truncate(self);
		}
	}
}