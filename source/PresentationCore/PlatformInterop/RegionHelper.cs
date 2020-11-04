using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.Platform;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.PlatformInterop
{
	internal static class RegionHelper
	{
		#region  Methods

		public static IntPtr CreateRoundedCornerRegion(Rect rect, CornerRadius cornerRadius)
		{
			var cr = cornerRadius.Coerce(rect);

			if (cornerRadius.IsUniform())
				return CreateRoundRectRgn(rect, cr.TopLeft);

			var hwidth = rect.Width/2;
			var hheight = rect.Height/2;

			var hrgn = CreateRoundRectRgn(new Rect(0, 0, hwidth + cr.TopLeft, hheight + cr.TopLeft), cr.TopLeft);

			var topRightRegionRect = new Rect(0, 0, hwidth + cr.TopRight, hheight + cr.TopRight);
			topRightRegionRect.Offset(hwidth - cr.TopRight, 0);

			CreateAndCombineRoundRectRgn(hrgn, topRightRegionRect, cr.TopRight);

			var bottomLeftRegionRect = new Rect(0, 0, hwidth + cr.BottomLeft, hheight + cr.BottomLeft);
			bottomLeftRegionRect.Offset(0, hheight - cr.BottomLeft);

			CreateAndCombineRoundRectRgn(hrgn, bottomLeftRegionRect, cr.BottomLeft);

			var bottomRightRegionRect = new Rect(0, 0, hwidth + cr.BottomRight, hheight + cr.BottomRight);
			bottomRightRegionRect.Offset(hwidth - cr.BottomRight, hheight - cr.BottomRight);

			CreateAndCombineRoundRectRgn(hrgn, bottomRightRegionRect, cr.BottomRight);

			return hrgn;
		}

		private static void CreateAndCombineRoundRectRgn(IntPtr hrgnSource, Rect region, double radius)
		{
			var hrgn = IntPtr.Zero;
			try
			{
				hrgn = CreateRoundRectRgn(region, radius);
				var result = NativeMethods.CombineRgn(hrgnSource, hrgnSource, hrgn, RGN.OR);
				if (result == CombineRgnResult.ERROR)
					throw new InvalidOperationException("Unable to combine two HRGNs.");
			}
			catch
			{
				PlatformUtil.SafeDeleteObject(ref hrgn);
				throw;
			}
		}

		private static IntPtr CreateRoundRectRgn(Rect region, double radius)
		{
			// Round outwards.

			if (radius.IsCloseTo(0))
			{
				return NativeMethods.CreateRectRgn
					(
						(int) Math.Floor(region.Left),
						(int) Math.Floor(region.Top),
						(int) Math.Ceiling(region.Right),
						(int) Math.Ceiling(region.Bottom));
			}

			// RoundedRect HRGNs require an additional pixel of padding on the bottom right to look correct.
			return NativeMethods.CreateRoundRectRgn
				(
					(int) Math.Floor(region.Left),
					(int) Math.Floor(region.Top),
					(int) Math.Ceiling(region.Right) + 1,
					(int) Math.Ceiling(region.Bottom) + 1,
					(int) Math.Ceiling(radius),
					(int) Math.Ceiling(radius));
		}

		#endregion
	}
}