// <copyright file="ColorUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Media;

namespace Zaaml.PresentationCore.Utils
{
  public static class ColorUtils
  {
    #region  Methods

    public static Color ConvertFromString(string colorString)
    {
      return ConvertFromString(colorString, CultureInfo.CurrentCulture);
    }

    public static Color ConvertFromString(string colorString, IFormatProvider formatProvider)
    {
      var normalized = colorString.Trim();

      if (normalized.StartsWith("#") && normalized.Length == 9)
      {
        if (normalized.Length == 9)
        {
          var a = byte.Parse(colorString.Substring(1, 2), NumberStyles.HexNumber, formatProvider);
          var r = byte.Parse(colorString.Substring(3, 2), NumberStyles.HexNumber, formatProvider);
          var g = byte.Parse(colorString.Substring(5, 2), NumberStyles.HexNumber, formatProvider);
          var b = byte.Parse(colorString.Substring(7, 2), NumberStyles.HexNumber, formatProvider);

          return Color.FromArgb(a, r, g, b);
        }

        if (normalized.Length == 7)
        {
          var r = byte.Parse(colorString.Substring(1, 2), NumberStyles.HexNumber, formatProvider);
          var g = byte.Parse(colorString.Substring(3, 2), NumberStyles.HexNumber, formatProvider);
          var b = byte.Parse(colorString.Substring(5, 2), NumberStyles.HexNumber, formatProvider);

          return Color.FromArgb(0xFF, r, g, b);
        }
      }

      Color color;
      return KnownColors.TryGetColor(normalized, out color) ? color : Colors.Transparent;
    }

    public static string ConvertToString(Color color)
    {
      return ConvertToString(color, CultureInfo.CurrentCulture);
    }

    public static string ConvertToString(Color color, IFormatProvider formatProvider)
    {
      string colorName;
      return KnownColors.TryGetColorName(color, out colorName) ? colorName : color.ToString(formatProvider);
    }

    public static SolidColorBrush CreateBrush(Color color)
    {
      return new SolidColorBrush(color);
    }

    public static byte GetChannelByteValue(Color color, RgbaChannel channel)
    {
      switch (channel)
      {
        case RgbaChannel.Alpha:
          return color.A;
        case RgbaChannel.Red:
          return color.R;
        case RgbaChannel.Green:
          return color.G;
        case RgbaChannel.Blue:
          return color.B;
        default:
          throw new ArgumentOutOfRangeException(nameof(channel));
      }
    }

    public static Color ModifyChannelByteValue(Color color, RgbaChannel channel, byte value)
    {
      switch (channel)
      {
        case RgbaChannel.Alpha:
          return Color.FromArgb(value, color.R, color.G, color.B);
        case RgbaChannel.Red:
          return Color.FromArgb(color.A, value, color.G, color.B);
        case RgbaChannel.Green:
          return Color.FromArgb(color.A, color.R, value, color.B);
        case RgbaChannel.Blue:
          return Color.FromArgb(color.A, color.R, color.G, value);
        default:
          throw new ArgumentOutOfRangeException(nameof(channel));
      }
    }

    #endregion
  }
}