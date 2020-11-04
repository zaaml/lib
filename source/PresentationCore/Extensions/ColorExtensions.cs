// <copyright file="ColorExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core.ColorModel;
using Zaaml.Core.Converters;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  public static class ColorExtensions
  {
    #region Static Fields

    private static readonly IPrimitiveConverter<string, Color> HexStrColorConverter = PrimitiveConverterFactory.GetConverter<string, Color>();

    #endregion

    #region Methods

    public static Color FromHexString(string hexColor)
    {
      return HexStrColorConverter.Convert(hexColor);
    }

    public static SolidColorBrush ToSolidColorBrush(this Color color)
    {
      return ColorUtils.CreateBrush(color);
    }

    public static Color WithOpacity(this Color color, byte opacity)
    {
      color.A = opacity;

      return color;
    }

    public static Color WithOpacity(this Color color, double opacity)
    {
      color.A = (byte) (Math.Max(0, Math.Min(1.0, opacity))*255);

      return color;
    }

    internal static HslColor ToHslColor(this Color color)
    {
      return color.ToRgbColor().ToHslColor();
    }

    internal static HsvColor ToHsvColor(this Color color)
    {
      return color.ToRgbColor().ToHsvColor();
    }

    internal static sRgbColor TosRgbColor(this Color color)
    {
      return new sRgbColor(color.A, color.R, color.G, color.B);
    }

    internal static RgbColor ToRgbColor(this Color color)
    {
      return color.TosRgbColor().ToRgbColor();
    }

    internal static Color ToXamlColor(this RgbColor rgbColor)
    {
      var sRgbColor = rgbColor.TosRgbColor();

      return Color.FromArgb(sRgbColor.A, sRgbColor.R, sRgbColor.G, sRgbColor.B);
    }

    internal static Color ToXamlColor(this HsvColor hsvColor)
    {
      return hsvColor.ToRgbColor().ToXamlColor();
    }

    internal static Color ToXamlColor(this HslColor hslColor)
    {
      return hslColor.ToRgbColor().ToXamlColor();
    }

    public static byte GetChannelByteValue(this Color color, RgbaChannel channel)
    {
      return ColorUtils.GetChannelByteValue(color, channel);
    }

    public static Color ModifyChannelByteValue(this Color color, RgbaChannel channel, byte value)
    {
      return ColorUtils.ModifyChannelByteValue(color, channel, value);
    }

    #endregion
  }
}