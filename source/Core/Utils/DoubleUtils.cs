// <copyright file="DoubleUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Zaaml.Core.Utils
{
	public static class DoubleUtils
	{
		public static bool AreClose(double value1, double value2)
		{
			if (value1 == value2)
				return true;

			var num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON_RELATIVE_1;
			var num2 = value1 - value2;

			return -num1 < num2 && num1 > num2;
		}

		public static bool AreClose(double value1, double value2, int precision)
		{
			if (value1 == value2)
				return true;

			if (precision == 0)
				return AreClose(value1, value2);

			if (precision < 0 || precision >= Precisions.Length)
				throw new ArgumentOutOfRangeException(nameof(precision));

			return Precisions[precision - 1].IsEqual(value1, value2);
		}


		public static double Clamp(double value, double min, double max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException();

			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		public static double Clamp(double value, Range<double> range)
		{
			return Clamp(value, range.Minimum, range.Maximum);
		}

		public static bool GreaterThan(double value1, double value2)
		{
			return value1 > value2 && !AreClose(value1, value2);
		}

		public static bool GreaterThan(double value1, double value2, int precision)
		{
			return value1 > value2 && !AreClose(value1, value2, precision);
		}

		public static bool GreaterThanOrClose(double value1, double value2)
		{
			return !(value1 <= value2) || AreClose(value1, value2);
		}

		public static bool GreaterThanOrClose(double value1, double value2, int precision)
		{
			return !(value1 <= value2) || AreClose(value1, value2, precision);
		}

		internal static bool IsDoubleFinite(double value)
		{
			if (!double.IsInfinity(value))
				return !IsNaN(value);
			return false;
		}

		internal static bool IsDoubleFiniteNonNegative(double value)
		{
			if (!double.IsInfinity(value) && !IsNaN(value))
				return value >= 0.0;
			return false;
		}

		public static bool IsDoubleFiniteOrNaN(object o)
		{
			return !double.IsInfinity((double)o);
		}

		internal static bool IsNaN(double value)
		{
			var nanUnion = new DoubleUnion { Double = value };
			var num1 = nanUnion.ULong & 18442240474082181120UL;
			var num2 = nanUnion.ULong & 4503599627370495UL;
			if ((long)num1 == 9218868437227405312L || (long)num1 == -4503599627370496L)
				return num2 > 0UL;
			return false;
		}

		public static bool IsNegativeZero(double value)
		{
			return value == 0 && BitConverter.DoubleToInt64Bits(value) == BitConverter.DoubleToInt64Bits(-0.0);
		}

		public static bool IsZero(double value)
		{
			return Math.Abs(value) < DBL_EPSILON_RELATIVE_10;
		}

		public static bool IsZero(double value, int precision)
		{
			if (precision < 0 || precision >= Precisions.Length)
				throw new ArgumentOutOfRangeException(nameof(precision));

			return Precisions[precision].IsEqual(value, 0.0);
		}

		public static bool LessThan(double value1, double value2)
		{
			return value1 < value2 && !AreClose(value1, value2);
		}

		public static bool LessThan(double value1, double value2, int precision)
		{
			return value1 < value2 && !AreClose(value1, value2, precision);
		}

		public static bool LessThanOrClose(double value1, double value2)
		{
			return !(value1 >= value2) || AreClose(value1, value2);
		}

		public static bool LessThanOrClose(double value1, double value2, int precision)
		{
			return !(value1 >= value2) || AreClose(value1, value2, precision);
		}

		internal static double Round(double value, RoundingMode mode)
		{
			return Round(value, 0, mode);
		}

		internal static double Round(double value, int digits, RoundingMode mode)
		{
			if (Math.Abs(value) < DoubleRoundLimit == false)
				return value;

			if (mode == RoundingMode.MidPointToEven)
				return Math.Round(value, digits, MidpointRounding.ToEven);

			var power10 = RoundPower10Double[digits];

			value *= power10;

			var fractional = SplitFractional(ref value);

			if (mode == RoundingMode.ToZero)
				return value / power10;

			if (mode == RoundingMode.FromZero || Math.Abs(fractional) >= 0.5)
				value += Math.Sign(fractional);

			value /= power10;

			return value;
		}

		internal static double SplitFractional(ref double value)
		{
			var fractional = SplitFractional(value, out var integral);

			value = integral;

			return fractional;
		}

		internal static double SplitFractional(double value, out double integral)
		{
			var doubleUnion = new DoubleUnion(value);

			var e = (int)(doubleUnion.ULong >> 52 & 0x7ff) - 0x3ff;

			// No fractional part
			if (e >= 52)
			{
				integral = value;

				// NaN
				if (e == 0x400 && doubleUnion.ULong << 12 != 0)
					return value;

				doubleUnion.ULong &= (ulong)1 << 63;
				return doubleUnion.Double;
			}

			// No integral part
			if (e < 0)
			{
				doubleUnion.ULong &= (ulong)1 << 63;
				integral = doubleUnion.Double;

				return value;
			}

			var mask = 0xffffffffffffffff >> 12 >> e;

			if ((doubleUnion.ULong & mask) == 0)
			{
				integral = value;
				doubleUnion.ULong &= (ulong)1 << 63;

				return doubleUnion.Double;
			}

			doubleUnion.ULong &= ~mask;

			integral = doubleUnion.Double;

			return value - integral;
		}

		public static double Truncate(double value)
		{
#if SILVERLIGHT
      SplitFractional(ref value);
      return value;
#else
			return Math.Truncate(value);
#endif
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleUnion
		{
			public DoubleUnion(double value) : this()
			{
				Double = value;
			}

			public DoubleUnion(ulong value) : this()
			{
				ULong = value;
			}

			[FieldOffset(0)] public double Double;

			[FieldOffset(0)] public ulong ULong;
		}

		private struct Epsilon
		{
			#region Fields

			private readonly double _value;

			#endregion

			#region Ctors

			public Epsilon(double value)
			{
				_value = value;
			}

			#endregion

			#region Methods

			internal bool IsEqual(double a, double b)
			{
				return (a == b) || (Math.Abs(a - b) < _value);
			}

// ReSharper disable once UnusedMember.Local
			internal bool IsNotEqual(double a, double b)
			{
				return (a != b) && !(Math.Abs(a - b) < _value);
			}

			#endregion
		}

		// ReSharper disable InconsistentNaming

		#region Static Fields

		private static double DoubleRoundLimit = 1E+16;

		internal const double DBL_EPSILON_RELATIVE_1 = 1.11022302462516E-16;

		internal const double DBL_EPSILON_RELATIVE_10 = 1.11022302462516E-15;
		// ReSharper restore InconsistentNaming

		private static readonly double[] RoundPower10Double =
		{
			1.0,
			10.0,
			100.0,
			1000.0,
			10000.0,
			100000.0,
			1000000.0,
			10000000.0,
			100000000.0,
			1000000000.0,
			10000000000.0,
			100000000000.0,
			1000000000000.0,
			10000000000000.0,
			100000000000000.0,
			1E+15
		};

		private static readonly Epsilon[] Precisions =
		{
			new Epsilon(0.1),
			new Epsilon(0.01),
			new Epsilon(0.001),
			new Epsilon(0.0001),
			new Epsilon(0.00001),
			new Epsilon(0.000001),
			new Epsilon(0.0000001),
			new Epsilon(0.00000001),
			new Epsilon(0.000000001),
			new Epsilon(0.0000000001),
			new Epsilon(0.00000000001),
			new Epsilon(0.000000000001),
			new Epsilon(0.0000000000001),
			new Epsilon(0.00000000000001),
			new Epsilon(0.000000000000001)
		};

		#endregion
	}

	internal enum RoundingMode
	{
		ToZero,
		FromZero,
		MidPointToEven,
		MidPointFromZero
	}
}

// ReSharper restore CompareOfFloatsByEqualityOperator