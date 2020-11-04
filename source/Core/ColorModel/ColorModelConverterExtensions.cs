// <copyright file="ColorModel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.ColorModel
{
  internal static class ColorModelConverterExtensions
  {
    #region Methods

    public static HslColor ToHslColor(this RgbColor rgbColor)
    {
      return ColorModelConverter.RgbColorToHslColor(rgbColor);
    }

    public static HslColor ToHslColor(this HsvColor hsvColor)
    {
      return ColorModelConverter.HsvColorToHslColor(hsvColor);
    }

    public static HsvColor ToHsvColor(this HslColor hslColor)
    {
      return ColorModelConverter.HslColorToHsvColor(hslColor);
    }

    public static HsvColor ToHsvColor(this RgbColor rgbColor)
    {
      return ColorModelConverter.RgbColorToHsvColor(rgbColor);
    }

    public static RgbColor ToRgbColor(this HsvColor hsvColor)
    {
      return ColorModelConverter.HsvColorToRgbColor(hsvColor);
    }

    public static RgbColor ToRgbColor(this HslColor hslColor)
    {
      return ColorModelConverter.HslColorToRgbColor(hslColor);
    }

    public static RgbColor ToRgbColor(this sRgbColor srgbColor)
    {
      return ColorModelConverter.sRgbToRgb(srgbColor);
    }

    public static sRgbColor TosRgbColor(this RgbColor rgbColor)
    {
      return ColorModelConverter.RgbTosRgb(rgbColor);
    }

    #endregion
  }
}