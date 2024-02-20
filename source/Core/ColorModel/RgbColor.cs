// <copyright file="RgbColor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.ColorModel
{
  internal struct RgbColor
  {
    #region Fields

    private double _a;
    private double _b;
    private double _g;
    private double _r;

    #endregion

    #region Ctors

    public RgbColor(double a, double r, double g, double b)
      : this()
    {
      A = a;
      R = r;
      G = g;
      B = b;
    }

    #endregion

    #region Properties

    public double A
    {
      get => _a.Clamp(0.0, 1.0);
      set => _a = value;
    }

    public double B
    {
      get => _b.Clamp(0.0, 1.0);
      set => _b = value;
    }

    public double G
    {
      get => _g.Clamp(0.0, 1.0);
      set => _g = value;
    }

    public double R
    {
      get => _r.Clamp(0.0, 1.0);
      set => _r = value;
    }

    public static RgbColor operator +(RgbColor first, RgbColor second)
    {
      return new RgbColor
      {
        _a = first._a + second._a,
        _r = first._r + second._r,
        _g = first._g + second._g,
        _b = first._b + second._b
      };
    }

    public static RgbColor operator -(RgbColor first, RgbColor second)
    {
      return new RgbColor
      {
        _a = first._a - second._a,
        _r = first._r - second._r,
        _g = first._g - second._g,
        _b = first._b - second._b
      };
    }

    public static RgbColor operator *(RgbColor color, double coefficient)
    {
      color._a *= coefficient;
      color._r *= coefficient;
      color._g *= coefficient;
      color._b *= coefficient;

      return color;
    }

    public static RgbColor Mix(RgbColor color1, RgbColor color2, double weight = 0.5)
    {
      var p = weight;
      var w = p * 2 - 1;
      var a = color1.A - color2.A;

      var w1 = ((w * a == -1 ? w : (w + a) / (1 + w * a)) + 1) / 2.0;

      var w2 = 1 - w1;


      return new RgbColor
      {
        _r = color1._r * w1 + color2._r * w2,
        _g = color1._g * w1 + color2._g * w2,
        _b = color1._b * w1 + color2._b * w2,
        _a = color1._a * p + color2._a * (1 - p)
      };
    }

    #endregion

    #region Methods

    public static RgbColor FromRgb(double alpha, double r, double g, double b)
    {
      return new RgbColor(alpha, r, g, b);
    }

    public void Saturate(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Saturate(this, amount, units);
    }

    public void Desaturate(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Desaturate(this, amount, units);
    }

    public void Lighten(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Lighten(this, amount, units);
    }

    public void Tint(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Tint(this, amount, units);
    }

    public void Shade(double amount, ColorFunctionUnits units)
    {
      this = ColorFunctions.Shade(this, amount, units);
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

    public static implicit operator HslColor(RgbColor rgbColor)
    {
      return rgbColor.ToHslColor();
    }

    public static implicit operator HsvColor(RgbColor rgbColor)
    {
      return rgbColor.ToHsvColor();
    }

    public void Greyscale()
    {
      this = ColorFunctions.Greyscale(this);
    }

    #endregion

    public RgbColor WithAlpha(double value)
    {
	    return new RgbColor(value, R, G, B);
    }

    public RgbColor WithRed(double value)
    {
	    return new RgbColor(A, value, G, B);
    }
    
    public RgbColor WithGreen(double value)
    {
	    return new RgbColor(A, R, value, B);
    }

    public RgbColor WithBlue(double value)
    {
	    return new RgbColor(A, R, G, value);
    }
  }
}