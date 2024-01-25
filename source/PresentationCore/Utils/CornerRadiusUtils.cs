// <copyright file="CornerRadiusUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

namespace Zaaml.PresentationCore.Utils
{
	internal static class CornerRadiusUtils
	{
		public static CornerRadius Coerce(CornerRadius cornerRadius, Rect rect)
		{
			var shortestDimension = Math.Min(rect.Width, rect.Height);
			var halfDimension = shortestDimension / 2;

			cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, halfDimension);
			cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, halfDimension);
			cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, halfDimension);
			cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, halfDimension);

			return cornerRadius;
		}

		public static CornerRadius Compose(CornerRadius enabledCornerRadius, CornerRadius disabledCornerRadius, bool invert, MaskCornerRadiusFlags flags)
		{
			var trueCornerRadius = invert ? disabledCornerRadius : enabledCornerRadius;
			var falseCornerRadius = invert ? enabledCornerRadius : disabledCornerRadius;

			if (flags.HasFlag(MaskCornerRadiusFlags.TopLeft) == false)
				trueCornerRadius.TopLeft = falseCornerRadius.TopLeft;

			if (flags.HasFlag(MaskCornerRadiusFlags.TopRight) == false)
				trueCornerRadius.TopRight = falseCornerRadius.TopRight;

			if (flags.HasFlag(MaskCornerRadiusFlags.BottomRight) == false)
				trueCornerRadius.BottomRight = falseCornerRadius.BottomRight;

			if (flags.HasFlag(MaskCornerRadiusFlags.BottomLeft) == false)
				trueCornerRadius.BottomLeft = falseCornerRadius.BottomLeft;

			return trueCornerRadius;
		}

		public static bool IsUniform(CornerRadius cornerRadius)
		{
			return cornerRadius.TopLeft.IsCloseTo(cornerRadius.TopRight) &&
			       cornerRadius.BottomLeft.IsCloseTo(cornerRadius.BottomRight) &&
			       cornerRadius.TopLeft.IsCloseTo(cornerRadius.BottomRight);
		}
	}

	internal static class MaskCornerRadiusExtensions
	{
		public static MaskCornerRadiusFlags DisableFlag(this MaskCornerRadiusFlags that, MaskCornerRadiusFlags changeFlag)
		{
			return that & ~changeFlag;
		}

		public static MaskCornerRadiusFlags EnableFlag(this MaskCornerRadiusFlags that, MaskCornerRadiusFlags changeFlag)
		{
			return that | changeFlag;
		}

		public static bool HasFlag(this MaskThicknessFlags that, MaskThicknessFlags flag)
		{
			return (that & flag) != 0;
		}

		public static MaskCornerRadiusFlags WithFlagValue(this MaskCornerRadiusFlags that, MaskCornerRadiusFlags changeFlag, bool value)
		{
			return value ? that.EnableFlag(changeFlag) : that.DisableFlag(changeFlag);
		}
	}

	internal static class CornerRadiusExtensions
	{
		public static CornerRadius Coerce(this CornerRadius cornerRadius, Rect rect)
		{
			return CornerRadiusUtils.Coerce(cornerRadius, rect);
		}

		public static bool IsUniform(this CornerRadius cornerRadius)
		{
			return CornerRadiusUtils.IsUniform(cornerRadius);
		}

		public static bool IsValid(this CornerRadius corner, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
		{
			if (!allowNegative)
			{
				if (corner.TopLeft < 0d || corner.TopRight < 0d || corner.BottomLeft < 0d || corner.BottomRight < 0d)
				{
					return (false);
				}
			}

			if (!allowNaN)
			{
				if (DoubleUtils.IsNaN(corner.TopLeft) || DoubleUtils.IsNaN(corner.TopRight) ||
				    DoubleUtils.IsNaN(corner.BottomLeft) || DoubleUtils.IsNaN(corner.BottomRight))
				{
					return (false);
				}
			}

			if (!allowPositiveInfinity)
			{
				if (double.IsPositiveInfinity(corner.TopLeft) || double.IsPositiveInfinity(corner.TopRight) ||
				    double.IsPositiveInfinity(corner.BottomLeft) || double.IsPositiveInfinity(corner.BottomRight))
				{
					return (false);
				}
			}

			if (!allowNegativeInfinity)
			{
				if (double.IsNegativeInfinity(corner.TopLeft) || double.IsNegativeInfinity(corner.TopRight) ||
				    double.IsNegativeInfinity(corner.BottomLeft) || double.IsNegativeInfinity(corner.BottomRight))
				{
					return (false);
				}
			}

			return true;
		}

	}
}