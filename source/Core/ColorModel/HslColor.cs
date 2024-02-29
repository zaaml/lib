// <copyright file="HslColor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.ColorModel
{
  internal struct HslColor
  {
    #region Fields

    private double _alpha;
    private double _hue;
    private double _lightness;
    private double _saturation;

    #endregion

    #region Ctors

    public HslColor(double alpha, double hue, double saturation, double lightness)
      : this()
    {
      Alpha = alpha;
      Hue = hue;
      Saturation = saturation;
      Lightness = lightness;
    }

    #endregion

    #region Properties

    public double Alpha
    {
      get => _alpha;
      set => _alpha = value.Clamp(0.0, 1.0);
    }

    public double Hue
    {
      get => _hue;
      set => _hue = value < 0.0 || value >= 360.0 ? (value % 360.0 + 360.0) % 360.0 : value;
    }

    public double Lightness
    {
      get => _lightness;
      set => _lightness = value.Clamp(0.0, 1.0);
    }

    public double Saturation
    {
      get => _saturation;
      set => _saturation = value.Clamp(0.0, 1.0);
    }

    public void Saturate(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Saturate(this, amount, units);
    }

    public void Desaturate(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Desaturate(this, amount, units);
    }

    public void Tint(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Tint(this, amount, units);
    }

    public void Shade(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Shade(this, amount, units);
    }

    public void Lighten(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Lighten(this, amount, units);
    }

    public void Darken(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Darken(this, amount, units);
    }

    public void FadeIn(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.FadeIn(this, amount, units);
    }

    public void FadeOut(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.FadeOut(this, amount, units);
    }

    public void Spin(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Spin(this, amount);
    }

    public static implicit operator HsvColor(HslColor hslColor)
    {
      return hslColor.ToHsvColor();
    }

    public static implicit operator RgbColor(HslColor hslColor)
    {
      return hslColor.ToRgbColor();
    }

    public void Greyscale()
    {
      this = ColorFunctions.Greyscale(this);
    }

    #endregion
  }
}