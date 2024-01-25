// <copyright file="GridViewColumnWidthConstraints.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.Core.GridView
{
	public readonly struct GridViewColumnWidthConstraints
	{
		public static GridViewColumnWidthConstraints Default => new(FlexLength.Star);

		public GridViewColumnWidthConstraints(FlexLength value, FlexLength minimum, FlexLength maximum)
		{
			Value = value;
			Minimum = minimum;
			Maximum = maximum;
		}

		public GridViewColumnWidthConstraints(FlexLength value) : this(value, new FlexLength(0), new FlexLength(double.PositiveInfinity))
		{
		}

		public FlexLength Minimum { get; }

		public FlexLength Maximum { get; }

		public FlexLength Value { get; }

		private static double CoerceStarConstraint(FlexLength length, double starLength, double fallbackLength)
		{
			if (starLength.IsNaN())
				return fallbackLength;

			return length.Value * starLength;
		}

		internal static void CalcConstraints(FlexLength value, FlexLength minimum, FlexLength maximum, double autoLength, double starLength, out double valueConstraint, out double minimumConstraint, out double maximumConstraint)
		{
			valueConstraint = value.IsAuto ? autoLength : value.IsAbsolute ? value.Value : CoerceStarConstraint(value, starLength, double.PositiveInfinity);
			minimumConstraint = minimum.IsAuto ? autoLength : minimum.IsAbsolute ? minimum.Value : CoerceStarConstraint(minimum, starLength, 0);
			maximumConstraint = maximum.IsAuto ? autoLength : maximum.IsAbsolute ? maximum.Value : CoerceStarConstraint(minimum, starLength, double.PositiveInfinity);

			CoerceConstraints(minimum, maximum, ref valueConstraint, ref minimumConstraint, ref maximumConstraint);
		}

		private static void CoerceConstraints(FlexLength minimum, FlexLength maximum, ref double valueConstraint, ref double minimumConstraint, ref double maximumConstraint)
		{
			if (minimum.IsAbsolute && (maximum.IsAuto || maximum.IsStar))
			{
				if (maximumConstraint.IsLessThan(minimumConstraint))
					maximumConstraint = minimumConstraint;
			}
			else if (maximum.IsAbsolute && (minimum.IsAuto || minimum.IsStar))
			{
				if (minimumConstraint.IsGreaterThan(maximumConstraint))
					minimumConstraint = maximumConstraint;
			}
			else if (maximumConstraint.IsLessThan(minimumConstraint))
				maximumConstraint = minimumConstraint;

			valueConstraint = valueConstraint.Clamp(minimumConstraint, maximumConstraint);
		}
	}
}