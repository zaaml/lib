// <copyright file="ColorModelConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Extensions;

namespace Zaaml.Core.ColorModel
{
  internal static class ColorModelConverter
  {
    #region  Methods

    private static double GetHue(RgbColor rgbColor, out double rgbMax, out double rgbMin)
    {
      rgbMax = MaxRgb(rgbColor);
      rgbMin = MinRgb(rgbColor);

      if (Equals(rgbColor.R, rgbColor.G) && Equals(rgbColor.G, rgbColor.B))
        return 0.0;

      double hue;

      var delta = rgbMax - rgbMin;

      if (Equals(rgbColor.R, rgbMax))
        hue = (rgbColor.G - rgbColor.B) / delta;
      else if (Equals(rgbColor.G, rgbMax))
        hue = 2 + (rgbColor.B - rgbColor.R) / delta;
      else
        hue = 4 + (rgbColor.R - rgbColor.G) / delta;

      hue *= 60;

      if (hue < 0.0)
        hue += 360.0;

      return hue;
    }

    public static RgbColor HslColorToRgbColor(HslColor hslColor)
    {
      var l = hslColor.Lightness;
      var s = hslColor.Saturation;
      var h = hslColor.Hue;

      var q = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
      var p = 2.0 * l - q;
      var hk = h / 360.0;

      var tr = hk + 1.0 / 3.0;
      var tg = hk;
      var tb = hk - 1.0 / 3.0;

      return new RgbColor(hslColor.Alpha, HslConvert(q, p, tr), HslConvert(q, p, tg), HslConvert(q, p, tb));
    }

    private static double HslConvert(double q, double p, double t)
    {
      var qp = q - p;

      if (t < 0.0)
        t += 1.0;
      
      if (t > 1.0)
        t -= 1.0;

      if (t < 1.0 / 6.0)
        return p + qp * 6.0 * t;
      
      if (t >= 1.0 / 6.0 && t < 0.5)
        return q;
      
      if (t >= 0.5 && t < 2.0 / 3.0)
        return p + qp * 6.0 * (2.0 / 3.0 - t);

      return p;
    }

    public static RgbColor HsvColorToRgbColor(HsvColor hsvColor)
    {
      var a = hsvColor.Alpha;
      var h = hsvColor.Hue;
      var s = hsvColor.Saturation;
      var v = hsvColor.Value;

      var hn = Math.Floor(h / 60.0);

      var hi = (int) hn % 6;
      var f = (h / 60.0) - hn;

      var p = v * (1.0 - s);
      var q = v * (1.0 - (f * s));
      var t = v * (1.0 - ((1.0 - f) * s));

      switch (hi)
      {
        case 0:
          return RgbColor.FromRgb(a, v, t, p);
        case 1:
          return RgbColor.FromRgb(a, q, v, p);
        case 2:
          return RgbColor.FromRgb(a, p, v, t);
        case 3:
          return RgbColor.FromRgb(a, p, q, v);
        case 4:
          return RgbColor.FromRgb(a, t, p, v);
        case 5:
          return RgbColor.FromRgb(a, v, p, q);
        default:
          return RgbColor.FromRgb(a, 0.0, 0.0, 0.0);
      }
    }

    private static double MaxRgb(RgbColor rgbColor)
    {
      return Math.Max(rgbColor.R, Math.Max(rgbColor.G, rgbColor.B));
    }

    private static double MinRgb(RgbColor rgbColor)
    {
      return Math.Min(rgbColor.R, Math.Min(rgbColor.G, rgbColor.B));
    }

    public static HslColor RgbColorToHslColor(RgbColor rgbColor)
    {
	    var hue = GetHue(rgbColor, out var rgbMax, out var rgbMin);
      var lightness = 0.5 * (rgbMax + rgbMin);

      var d = rgbMax - rgbMin;

      var saturation = lightness > 0.5 ? d / (2 - rgbMax - rgbMin) : d / (rgbMax + rgbMin);

      return new HslColor(rgbColor.A, hue, saturation.IsNaN() ? 0 : saturation, lightness);
    }

    public static HslColor HsvColorToHslColor(HsvColor hsv)
    {
      return RgbColorToHslColor(HsvColorToRgbColor(hsv));
    }

    public static HsvColor HslColorToHsvColor(HslColor hsl)
    {
      return RgbColorToHsvColor(HslColorToRgbColor(hsl));
    }

    public static HsvColor RgbColorToHsvColor(RgbColor rgbColor)
    {
	    var hue = GetHue(rgbColor, out var rgbMax, out var rgbMin);
      var value = rgbMax;
      var saturation = Equals(rgbMax, 0.0) ? 0.0 : 1.0 - rgbMin / rgbMax;

      return new HsvColor(rgbColor.A, hue, saturation, value);
    }

    public static sRgbColor RgbTosRgb(RgbColor rgbColor)
    {
      return new sRgbColor((byte) (rgbColor.A * byte.MaxValue + 0.5), ScRgbTosRgb(rgbColor.R), ScRgbTosRgb(rgbColor.G), ScRgbTosRgb(rgbColor.B));
    }

    private static byte ScRgbTosRgb(double val)
    {
      if (val <= 0.0)
        return 0;
     
      if (val <= 0.0031308)
        return (byte) (byte.MaxValue * val * 12.9200000762939 + 0.5);
      
      if (val < 1.0)
        return (byte) (byte.MaxValue * (1.05499994754791 * Math.Pow(val, 5.0 / 12.0) - 0.0549999997019768) + 0.5);

      return byte.MaxValue;
    }
		
    public static RgbColor sRgbToRgb(sRgbColor srgb)
    {
      return new RgbColor(srgb.A / (double) byte.MaxValue, sRgbToScRgb(srgb.R), sRgbToScRgb(srgb.G), sRgbToScRgb(srgb.B));
    }

    private static double sRgbToScRgb(byte bval)
    {
      var num = bval / (double) byte.MaxValue;

      if (num <= 0.0)
        return 0.0;
      
      if (num <= 0.04045)
        return num / 12.92;
      
      if (num < 1.0)
        return Math.Pow((num + 0.055) / 1.055, 2.4);
      
      return 1f;
    }

    #endregion
  }
}