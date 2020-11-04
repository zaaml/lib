// <copyright file="PlatformExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.Platform;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class PlatformExtensions
  {
    #region  Methods

    public static NativeBrush ToNativeBrush(this Color color)
    {
      return NativeBrush.FromColor(color.ToNativeColor());
    }

	  public static COLORREF ToNativeColor(this Color color)
	  {
			return new COLORREF(color.R, color.G, color.B);
		}

    public static POINT ToPlatformPoint(this Point point)
    {
      return new POINT
      {
        x = (int) point.X,
        y = (int) point.Y
      };
    }

    public static RECT ToPlatformRect(this Rect rect)
    {
      return new RECT
      {
        Left = (int) rect.Left,
        Top = (int) rect.Top,
        Right = (int) rect.Right,
        Bottom = (int) rect.Bottom
      };
    }

    public static Point ToPresentationPoint(this POINT point)
    {
      return new Point
      {
        X = point.x,
        Y = point.y
      };
    }

    public static Rect ToPresentationRect(this RECT platformRect)
    {
      return new Rect(new Point(platformRect.Left, platformRect.Top), new Point(platformRect.Right, platformRect.Bottom));
    }

    #endregion
  }
}