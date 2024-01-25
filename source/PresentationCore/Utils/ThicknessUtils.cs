// <copyright file="ThicknessUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
#if !SILVERLIGHT
using System;
using Zaaml.Platform;
#endif

namespace Zaaml.PresentationCore.Utils
{
	internal static class ThicknessUtils
	{
		public static bool AreClose(Thickness first, Thickness second)
		{
			return first.Left.IsCloseTo(second.Left) && first.Top.IsCloseTo(second.Top) && first.Right.IsCloseTo(second.Right) && first.Bottom.IsCloseTo(second.Bottom);
		}

		public static Thickness Compose(Thickness enabledThickness, Thickness disabledThickness, bool invert, MaskThicknessFlags flags)
		{
			var trueThickness = invert ? disabledThickness : enabledThickness;
			var falseThickness = invert ? enabledThickness : disabledThickness;

			if (flags.HasFlag(MaskThicknessFlags.Left) == false)
				trueThickness.Left = falseThickness.Left;

			if (flags.HasFlag(MaskThicknessFlags.Top) == false)
				trueThickness.Top = falseThickness.Top;

			if (flags.HasFlag(MaskThicknessFlags.Right) == false)
				trueThickness.Right = falseThickness.Right;

			if (flags.HasFlag(MaskThicknessFlags.Bottom) == false)
				trueThickness.Bottom = falseThickness.Bottom;

			return trueThickness;
		}

		public static Thickness Extend(Thickness thickness, Thickness extent)
		{
			return new Thickness(thickness.Left + extent.Left, thickness.Top + extent.Top, thickness.Right + extent.Right, thickness.Bottom + extent.Bottom);
		}

		public static Thickness Extend(Thickness thickness, double uniformExtent)
		{
			return new Thickness(thickness.Left + uniformExtent, thickness.Top + uniformExtent, thickness.Right + uniformExtent, thickness.Bottom + uniformExtent);
		}

		public static Thickness Scale(Thickness thickness, double scale)
		{
			return new Thickness
			{
				Left = thickness.Left * scale,
				Top = thickness.Top * scale,
				Right = thickness.Right * scale,
				Bottom = thickness.Bottom * scale
			};
		}
	}

	internal static class MaskThicknessFlagsExtensions
	{
		public static MaskThicknessFlags DisableFlag(this MaskThicknessFlags that, MaskThicknessFlags changeFlag)
		{
			return that & ~changeFlag;
		}

		public static MaskThicknessFlags EnableFlag(this MaskThicknessFlags that, MaskThicknessFlags changeFlag)
		{
			return that | changeFlag;
		}

		public static bool HasFlag(this MaskThicknessFlags that, MaskThicknessFlags flag)
		{
			return (that & flag) != 0;
		}

		public static MaskThicknessFlags WithFlagValue(this MaskThicknessFlags that, MaskThicknessFlags changeFlag, bool value)
		{
			return value ? that.EnableFlag(changeFlag) : that.DisableFlag(changeFlag);
		}
	}

	internal static class ThicknessExtensions
	{
		public static bool IsUniform(this Thickness thick)
		{
			return thick.Left.IsCloseTo(thick.Top)
			       && thick.Left.IsCloseTo(thick.Right)
			       && thick.Left.IsCloseTo(thick.Bottom);
		}

		public static Thickness GetExtended(this Thickness thickness, Thickness extent)
		{
			return ThicknessUtils.Extend(thickness, extent);
		}

		public static Thickness GetExtended(this Thickness thickness, double uniformExtent)
		{
			return ThicknessUtils.Extend(thickness, uniformExtent);
		}

		public static Thickness GetScaled(this Thickness thickness, double scale)
		{
			return ThicknessUtils.Scale(thickness, scale);
		}

		public static bool IsCloseTo(this Thickness thickness, Thickness other)
		{
			return ThicknessUtils.AreClose(thickness, other);
		}

		public static Size Collapse(this Thickness thick)
		{
			return new Size(thick.Left + thick.Right, thick.Top + thick.Bottom);
		}

		public static bool IsZero(this Thickness thick)
		{
			return thick.Left.IsZero()
			       && thick.Top.IsZero()
			       && thick.Right.IsZero()
			       && thick.Bottom.IsZero();
		}

		public static bool IsValid(this Thickness thick, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
		{
			if (!allowNegative)
			{
				if (thick.Left < 0d || thick.Right < 0d || thick.Top < 0d || thick.Bottom < 0d)
				{
					return false;
				}
			}

			if (!allowNaN)
			{
				if (DoubleUtils.IsNaN(thick.Left) || DoubleUtils.IsNaN(thick.Right)
				                      || DoubleUtils.IsNaN(thick.Top) || DoubleUtils.IsNaN(thick.Bottom))
				{
					return false;
				}
			}

			if (!allowPositiveInfinity)
			{
				if (double.IsPositiveInfinity(thick.Left) || double.IsPositiveInfinity(thick.Right)
				                                          || double.IsPositiveInfinity(thick.Top) || double.IsPositiveInfinity(thick.Bottom))
				{
					return false;
				}
			}

			if (!allowNegativeInfinity)
			{
				if (double.IsNegativeInfinity(thick.Left) || double.IsNegativeInfinity(thick.Right)
				                                          || double.IsNegativeInfinity(thick.Top) || double.IsNegativeInfinity(thick.Bottom))
				{
					return false;
				}
			}

			return true;
		}

#if !SILVERLIGHT
		public static MARGINS ToPlatformMargins(this Thickness thickness)
		{
			return new MARGINS
			{
				cxLeftWidth = (int) Math.Ceiling(thickness.Left),
				cxRightWidth = (int) Math.Ceiling(thickness.Right),
				cyTopHeight = (int) Math.Ceiling(thickness.Top),
				cyBottomHeight = (int) Math.Ceiling(thickness.Bottom)
			};
		}

		public static Thickness ToPresentationThickness(this MARGINS margins)
		{
			return new Thickness(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
		}
#endif
	}
}